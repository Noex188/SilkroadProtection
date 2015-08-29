using System;
using SilkroadSecurityApi;

namespace sroprot.NetEngine.AgentServer
{
    class WaterTempleTeleport
    {
        public static PacketProcessResult HandleClient(Packet pck, RelaySession session, SilkroadServer server)
        {
            if (Global.FixWaterTampleTeleport)
            {
                pck.ReadUInt32();
                byte teleport_type = pck.ReadUInt8();
                if (teleport_type == 2)
                {
                    uint teleport_id = pck.ReadUInt32();
                    string cname = session.State["charname"] as string;

                    if (cname.Length == 0)
                    {
                        Global.logmgr.WriteLog(LogLevel.Warning, "charname len == 0 ! (teleport fix)");
                        return PacketProcessResult.ContinueLoop;
                    }
                    //проверка прав доступа на второй уровень
                    if (teleport_id == 166 || teleport_id == 167)
                    {
                        if (Global.dbmgr.AnticheatCheckTeleportAccess(cname, (int)teleport_id) == 0)
                        {
                            session.SendClientNotice("UIIT_STT_ANTICHEAT_TELEPORT_TAMPLE");
                            return PacketProcessResult.ContinueLoop;
                        }

                    }
                }

            }

            return PacketProcessResult.DoNothing;
        }
    }
}
