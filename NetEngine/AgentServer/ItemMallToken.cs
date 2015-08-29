using System;
using System.Collections.Generic;
using SilkroadSecurityApi;

namespace sroprot.NetEngine.AgentServer
{
    class ItemMallToken
    {
        public static PacketProcessResult HandleClient(Packet pck, RelaySession session, SilkroadServer server)
        {
            if (Global.EnableItemMallBuyFix)
            {
                Packet MyResponse = new Packet(0xB566, true);

                string uname = session.State["username"] as string;

                if (uname.Length == 0)
                {
                    Global.logmgr.WriteLog(LogLevel.Warning, "username len == 0 !");
                    return PacketProcessResult.Disconnect;
                }


                if (uname.Contains("'"))
                {
                    Global.logmgr.WriteLog(LogLevel.Warning, "User trying to exploit shop ! Uname str: {0}", uname);
                    return PacketProcessResult.Disconnect;
                }

                List<string> query_res = Global.dbmgr.GetJidAndToken(uname);

                UInt32 jid = uint.Parse(query_res[0]);
                string token = query_res[1];

                MyResponse.WriteUInt8(1);
                MyResponse.WriteUInt32(jid);
                MyResponse.WriteAscii(token);

                session.SendPacketToClient(MyResponse);
                //   Global.g_LogManager.WriteLog(LogLevel.Notify, "Shop packet OK [{0}] JID [{1}] TOKEN [{2}]",uname,jid,token);

                return PacketProcessResult.ContinueLoop;
            }
            return PacketProcessResult.DoNothing;
        }
    }
}
