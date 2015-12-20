using System.Collections.Generic;
using Ultrapowa_Clash_Server_GUI.Helpers;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Packet 20103
    internal class LoginFailedMessage : Message
    {
        private string m_vContentURL;

        private int m_vErrorCode;

        //48
        private string m_vReason;

        private string m_vRedirectDomain;

        //68
        private int m_vRemainingTime;

        private string m_vResourceFingerprintData;

        //56
        //60
        private string m_vUpdateURL;

        //64
        public LoginFailedMessage(Client client) : base(client)
        {
            SetMessageType(20103);
            SetMessageVersion(3);

            //errorcodes:
            //9: remove redirect domain
            //8: new game version available (removeupdateurl)
            //7: remove resource fingerprint data
            //10: maintenance
            //11: temporarily banned
            //12: played too much
            //13: locked account
        }

        //52
        public override void Encode()
        {
            var pack = new List<byte>();

            pack.AddInt32(m_vErrorCode);
            pack.AddString(m_vResourceFingerprintData);
            pack.AddString(m_vRedirectDomain);
            pack.AddString(m_vContentURL);
            pack.AddString(m_vUpdateURL);
            pack.AddString(m_vReason);
            pack.AddInt32(m_vRemainingTime);
            pack.AddInt32(-1);
            pack.Add(0);

            SetData(pack.ToArray());
        }

        public void RemainingTime(int code)
        {
            m_vRemainingTime = code;
        }

        public void SetContentURL(string url)
        {
            m_vContentURL = url;
        }

        public void SetErrorCode(int code)
        {
            m_vErrorCode = code;
        }

        public void SetReason(string reason)
        {
            m_vReason = reason;
        }

        public void SetRedirectDomain(string domain)
        {
            m_vRedirectDomain = domain;
        }

        public void SetResourceFingerprintData(string data)
        {
            m_vResourceFingerprintData = data;
        }

        public void SetUpdateURL(string url)
        {
            m_vUpdateURL = url;
        }
    }
}