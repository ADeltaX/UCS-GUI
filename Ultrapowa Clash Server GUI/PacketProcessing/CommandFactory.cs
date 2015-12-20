using System;
using System.Collections.Generic;
using System.IO;
using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.Helpers;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Command list: LogicCommand::createCommand
    internal static class CommandFactory
    {
        private static readonly Dictionary<uint, Type> m_vCommands;

        static CommandFactory()
        {
            m_vCommands = new Dictionary<uint, Type>();
            m_vCommands.Add(0x0001, typeof (JoinAllianceCommand));
            m_vCommands.Add(0x0002, typeof (LeaveAllianceCommand));
            m_vCommands.Add(0x01F4, typeof (BuyBuildingCommand));
            m_vCommands.Add(0x01F5, typeof (MoveBuildingCommand));
            m_vCommands.Add(0x01F6, typeof (UpgradeBuildingCommand));
            m_vCommands.Add(0x01F7, typeof (SellBuildingCommand));
            m_vCommands.Add(0x01F8, typeof (SpeedUpConstructionCommand));
            m_vCommands.Add(0x01F9, typeof (CancelConstructionCommand));
            m_vCommands.Add(0x01FA, typeof (CollectResourcesCommand));

            m_vCommands.Add(0x01FB, typeof (ClearObstacleCommand));
            m_vCommands.Add(0x01FC, typeof (TrainUnitCommand));
            m_vCommands.Add(0x01FD, typeof (CancelUnitProductionCommand));
            m_vCommands.Add(0x01FE, typeof (BuyTrapCommand));

            //m_vCommands.Add(0x01FF, typeof(RequestAllianceUnits));
            m_vCommands.Add(0x0200, typeof (BuyDecoCommand));
            m_vCommands.Add(0x0201, typeof (SpeedUpTrainingCommand));
            m_vCommands.Add(0x0202, typeof (SpeedUpClearingCommand));

            //m_vCommands.Add(0x0203, typeof(CancelUpgradeUnit));
            m_vCommands.Add(0x0204, typeof (UpgradeUnitCommand));
            m_vCommands.Add(0x0205, typeof (SpeedUpUpgradeUnitCommand));
            m_vCommands.Add(0x0206, typeof (BuyResourcesCommand));

            //m_vCommands.Add(0x0207, typeof(MissionProgressCommand));
            m_vCommands.Add(0x0208, typeof (UnlockBuildingCommand));
            m_vCommands.Add(0x0209, typeof (FreeWorkerCommand));

            //m_vCommands.Add(0x020A, typeof(BuyShield));
            m_vCommands.Add(0x020B, typeof (ClaimAchievementRewardCommand));

            //m_vCommands.Add(0x020C, typeof(ToggleAttackMode));
            m_vCommands.Add(0x020D, typeof (LoadTurretCommand));
            m_vCommands.Add(0x020E, typeof (BoostBuildingCommand));
            m_vCommands.Add(0x020F, typeof (UpgradeHeroCommand));
            m_vCommands.Add(0x0210, typeof (SpeedUpHeroUpgradeCommand));

            //m_vCommands.Add(0x0211, typeof(ToggleHeroSleep));
            //m_vCommands.Add(0x0212, typeof(SpeedUpHeroHealth));
            m_vCommands.Add(0x0213, typeof (CancelHeroUpgradeCommand));

            //m_vCommands.Add(0x0214, typeof(NewShopItemsSeen));
            m_vCommands.Add(0x0215, typeof (MoveMultipleBuildingsCommand));
            m_vCommands.Add(0x0219, typeof (SendAllianceMailCommand));
            m_vCommands.Add(0x021B, typeof (Unknown539Command));
            m_vCommands.Add(0x021F, typeof (KickAllianceMemberCommand));
            m_vCommands.Add(0x0225, typeof (UpgradeMultipleBuildingsCommand));
            m_vCommands.Add(0x0226, typeof (RemoveUnitsCommand));
            m_vCommands.Add(0x0258, typeof (PlaceAttackerCommand));
            m_vCommands.Add(0x025C, typeof (CastSpellCommand));
            m_vCommands.Add(0x02BC, typeof (SearchOpponentCommand));
        }

        public static object Read(BinaryReader br)
        {
            var cm = br.ReadUInt32WithEndian();
            if (m_vCommands.ContainsKey(cm))
            {
                return Activator.CreateInstance(m_vCommands[cm], br);
            }
            Console.Write("\t");
            Debugger.WriteLine("Unhandled Command " + cm + " (ignored)", null, 4, ConsoleColor.Red);
            return null;
        }
    }
}