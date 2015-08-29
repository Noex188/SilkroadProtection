using System;
using System.Collections.Generic;
using SilkroadSecurityApi;

namespace sroprot.NetEngine.AgentServer
{
    class SilkDisplay
    {
        public static PacketProcessResult HandleClient(Packet pck, RelaySession session, SilkroadServer server)
        {
            if (Global.EnableSilkDisplayFix)
            {
                try
                {
                    string uname = session.State["username"] as string;

                    if (uname.Length == 0)
                    {
                        Global.logmgr.WriteLog(LogLevel.Error, "username len == 0 (request silk info)");
                        return PacketProcessResult.Disconnect;
                    }

                    List<int> silk_info = Global.dbmgr.GetSilkDataByUsername(uname);
                    Packet resp = new Packet(0x3153);

                    resp.WriteUInt32(silk_info[0]);
                    resp.WriteUInt32(silk_info[1]);
                    resp.WriteUInt32(silk_info[2]);
                    //  m_ClientSecurity.Send(resp);
                    session.SendPacketToClient(resp);

                    // Global.g_LogManager.WriteLog(LogLevel.Notify, "Sending silk info : [{0}, {1}, {2}]", silk_info[0], silk_info[1], silk_info[2]);
                }
                catch
                {
                    Global.logmgr.WriteLog(LogLevel.Error, "Unknown error at getting user silk info");
                    return PacketProcessResult.Disconnect;
                }
            }
            return PacketProcessResult.DoNothing;
        }
    }
}
