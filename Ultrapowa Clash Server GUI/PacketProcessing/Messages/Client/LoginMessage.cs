using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.Logic;
using Ultrapowa_Clash_Server_GUI.Network;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Packet 10101
    internal class LoginMessage : Message
    {
        private long m_vAccountId;

        private int m_vClientBuild;

        private int m_vClientContentVersion;

        private int m_vClientMajorVersion;

        private uint m_vClientSeed;

        private string m_vDevice;

        private string m_vGameVersion;

        private string m_vMacAddress;

        private string m_vOpenUDID;

        private string m_vPassToken;

        //unchecked
        private string m_vPhoneId;

        private string m_vPreferredDeviceLanguage;

        private string m_vResourceSha;

        private string m_vSignature2;

        private string m_vSignature3;

        private string m_vSignature4;

        private string m_vUDID;

        public LoginMessage(Client client, BinaryReader br) : base(client, br)
        {
        }

        public override void Decode()
        {
            using (var br = new BinaryReader(new MemoryStream(GetData())))
            {
                m_vAccountId = br.ReadInt64WithEndian();
                m_vPassToken = br.ReadScString();
                m_vClientMajorVersion = br.ReadInt32WithEndian();
                m_vClientContentVersion = br.ReadInt32WithEndian();
                m_vClientBuild = br.ReadInt32WithEndian();
                m_vResourceSha = br.ReadScString();
                m_vUDID = br.ReadScString();
                m_vOpenUDID = br.ReadScString();
                m_vMacAddress = br.ReadScString();
                m_vDevice = br.ReadScString();
                br.ReadInt32WithEndian(); //00 1E 84 81, readDataReference for m_vPreferredLanguage
                m_vPreferredDeviceLanguage = br.ReadScString();

                //unchecked
                m_vPhoneId = br.ReadScString();
                m_vGameVersion = br.ReadScString();
                br.ReadByte(); //01
                br.ReadInt32WithEndian(); //00 00 00 00
                m_vSignature2 = br.ReadScString();
                m_vSignature3 = br.ReadScString();
                br.ReadByte(); //01
                m_vSignature4 = br.ReadScString();
                m_vClientSeed = br.ReadUInt32WithEndian();
                Debugger.WriteLine("[M] Client with user id " + m_vAccountId + " accessing with " + m_vDevice + " and " + m_vPassToken + " as users token", null, 5);
                if (GetMessageVersion() > 8) //7.200
                {
                    br.ReadByte();
                    br.ReadUInt32WithEndian();
                    br.ReadUInt32WithEndian();
                }
            }
        }

        public override void Process(Level level)
        {
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["maintenanceProMode"]))
            {
                level = ResourcesManager.GetPlayer(m_vAccountId);
                if (level != null && level.GetAccountPrivileges() > 3)
                {
                }
                else
                {
                    var p = new LoginFailedMessage(Client);
                    p.SetErrorCode(10);
                    p.RemainingTime(0);
                    p.SetReason(ConfigurationManager.AppSettings["maintenanceProMessage"]);
                    PacketManager.ProcessOutgoingPacket(p);
                    return;
                }
            }
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["maintenanceMode"]))
            {
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["adminSpecialMode"]))
                {
                    level = ResourcesManager.GetPlayer(m_vAccountId);
                    if (level != null && level.GetAccountPrivileges() > 3)
                    {
                    }
                    else
                    {
                        var p = new LoginFailedMessage(Client);
                        p.SetErrorCode(10);
                        p.RemainingTime(Convert.ToInt32(ConfigurationManager.AppSettings["maintenanceTimeLeft"]));
                        PacketManager.ProcessOutgoingPacket(p);
                        return;
                    }
                }
                else
                {
                    var p = new LoginFailedMessage(Client);
                    p.SetErrorCode(10);
                    p.RemainingTime(Convert.ToInt32(ConfigurationManager.AppSettings["maintenanceTimeLeft"]));
                    PacketManager.ProcessOutgoingPacket(p);
                    return;
                }
            }

            var versionData = ConfigurationManager.AppSettings["clientVersion"].Split('.');
            if (versionData.Length >= 2)
            {
                if (m_vClientMajorVersion != Convert.ToInt32(versionData[0]) ||
                    m_vClientBuild != Convert.ToInt32(versionData[1]))
                {
                    var p = new LoginFailedMessage(Client);
                    p.SetErrorCode(8);
                    p.SetUpdateURL(ConfigurationManager.AppSettings["oldClientVersion"]);
                    PacketManager.ProcessOutgoingPacket(p);
                    return;
                }
            }
            else
            {
                Debugger.WriteLine("Connection failed. UCS config key clientVersion is not properly set.");
            }

            level = ResourcesManager.GetPlayer(m_vAccountId);
            if (level != null)
            {
                if (level.GetAccountStatus() == 99)
                {
                    var p = new LoginFailedMessage(Client);
                    p.SetErrorCode(11);
                    PacketManager.ProcessOutgoingPacket(p);
                    return;
                }
            }

            if (Convert.ToBoolean(ConfigurationManager.AppSettings["useCustomPatch"]))
            {
                if (m_vResourceSha != ObjectManager.FingerPrint.sha)
                {
                    var p = new LoginFailedMessage(Client);
                    p.SetErrorCode(7);
                    p.SetResourceFingerprintData(ObjectManager.FingerPrint.SaveToJson());
                    p.SetContentURL(ConfigurationManager.AppSettings["patchingServer"]);
                    p.SetUpdateURL(ConfigurationManager.AppSettings["oldClientVersion"]);
                    PacketManager.ProcessOutgoingPacket(p);
                    return;
                }
            }

            Client.ClientSeed = m_vClientSeed;
            PacketManager.ProcessOutgoingPacket(new SessionKeyMessage(Client));
            Debugger.WriteLine("[M] Retrieve Player Data for player " + m_vAccountId, null, 5);

            //New player
            if (level == null)
            {
                level = ObjectManager.CreateAvatar(m_vAccountId);
                var tokenSeed = new byte[20];
                new Random().NextBytes(tokenSeed);
                SHA1 sha = new SHA1CryptoServiceProvider();
                m_vPassToken = BitConverter.ToString(sha.ComputeHash(tokenSeed)).Replace("-", "");
            }
            if (level.GetAccountPrivileges() == 1)
                level.GetPlayerAvatar().SetLeagueId(12);
            if (level.GetAccountPrivileges() == 2)
                level.GetPlayerAvatar().SetLeagueId(15);
            if (level.GetAccountPrivileges() == 3)
                level.GetPlayerAvatar().SetLeagueId(18);
            if (level.GetAccountPrivileges() == 4)
                level.GetPlayerAvatar().SetLeagueId(21);
            if (level.GetAccountPrivileges() == 5)
                level.GetPlayerAvatar().SetLeagueId(22);

            ResourcesManager.LogPlayerIn(level, Client);
            level.Tick();

            var loginOk = new LoginOkMessage(Client);
            var avatar = level.GetPlayerAvatar();
            loginOk.SetAccountId(avatar.GetId());
            loginOk.SetPassToken(m_vPassToken);
            loginOk.SetServerMajorVersion(m_vClientMajorVersion);
            loginOk.SetServerBuild(m_vClientBuild);
            loginOk.SetContentVersion(m_vClientContentVersion);
            loginOk.SetServerEnvironment("stage");
            loginOk.SetDaysSinceStartedPlaying(21222);
            loginOk.SetPlayTimeSeconds(62072000);

            //loginOk.SetFacebookId("100001230452744");
            //loginOk.SetGamecenterId("");
            loginOk.SetServerTime(
                Math.Round(level.GetTime().Subtract(new DateTime(1970, 1, 1)).TotalSeconds*1000).ToString());
            loginOk.SetAccountCreatedDate("1414003838000");
            loginOk.SetStartupCooldownSeconds(0);
            loginOk.SetCountryCode("US");
            PacketManager.ProcessOutgoingPacket(loginOk);

            var alliance = ObjectManager.GetAlliance(level.GetPlayerAvatar().GetAllianceId());
            if (alliance == null)
                level.GetPlayerAvatar().SetAllianceId(0);
            PacketManager.ProcessOutgoingPacket(new OwnHomeDataMessage(Client, level));
            if (alliance != null)
                PacketManager.ProcessOutgoingPacket(new AllianceStreamMessage(Client, alliance));

            if (ResourcesManager.IsPlayerOnline(level))
            {
                var mail = new AllianceMailStreamEntry();
                mail.SetId((int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
                mail.SetSenderId(0);
                mail.SetSenderAvatarId(0);
                mail.SetSenderName("System Manager");
                mail.SetIsNew(0);
                mail.SetAllianceId(0);
                mail.SetSenderLeagueId(22);
                mail.SetAllianceBadgeData(1728059989);
                mail.SetAllianceName("UCS System");
                mail.SetMessage(
                    "Welcome to Ultrapowa Clash Server Emulator.If you found any bug,Please report it to ultrapowa.com/forum");
                mail.SetSenderLevel(500);
                var p = new AvatarStreamEntryMessage(level.GetClient());
                p.SetAvatarStreamEntry(mail);
                PacketManager.ProcessOutgoingPacket(p);
            }
        }
    }
}