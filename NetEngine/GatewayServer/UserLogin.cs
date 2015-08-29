using System;
using SilkroadSecurityApi;

namespace sroprot.NetEngine.GatewayServer
{
    /// <summary>
    /// FIX ME
    /// </summary>
    class UserLogin
    {
        public static PacketProcessResult HandleClient(Packet pck, RelaySession session, SilkroadServer server)
        {
            byte locale = pck.ReadUInt8();
            string username = pck.ReadAscii();
            string password = pck.ReadAscii();
            ushort ServerID = pck.ReadUInt16();

            if (Global.EnableServerInspection && !Global.InspectionLoginIgnore.Contains(username))
            {
                Packet login_response = new Packet(0xA102, false);
                login_response.WriteUInt8(0x02);
                login_response.WriteUInt8(0x02);
                login_response.WriteUInt8(0x02);
                session.SendPacketToClient(login_response);

                return PacketProcessResult.Disconnect;
            }

            if (Global.EnableIpAccountLimitation)
            {
                //We use Utility.GetRemoteEpString because username is assigned after AgentServer.UserAuth
                string clientAddr = Utility.GetRemoteEpString(session.Arguments.ClientSocket);
                int connectionCount = Global.srvmgr.GetUserCountByIpForAgent(clientAddr);

                if (connectionCount >= Global.AccountIpLimitCount)
                {
                    Packet login_resp = new Packet(0xA102, false);
                    login_resp.WriteUInt8(0x02);
                    login_resp.WriteUInt8(12);
                    session.SendPacketToClient(login_resp);

                    return PacketProcessResult.Disconnect;
                }
            }

            int serverOnline = 0;
            int.TryParse(session.State["server_" + ServerID] as string, out serverOnline);
            if (Global.ShardMaxOnline > 0 && Global.ShardMaxOnline <= serverOnline)
            {
                Packet login_response = new Packet(0xA102, false);
                login_response.WriteUInt8(0x02);
                login_response.WriteUInt8(5);
                session.SendPacketToClient(login_response);
                return PacketProcessResult.Disconnect;
            }

    
            if (Global.EnableLoginProcessing)
            {

                Packet login = new Packet(0x6102);
                login.WriteUInt8(Global.OriginalLocale > 0 && locale > 0 ? Global.OriginalLocale : locale);
                login.WriteAscii(username);
                login.WriteAscii(Global.EnableUseSha1Salt ? Utility.HashPassword(username, password) : password);
                login.WriteUInt16(ServerID);
                session.SendPacketToModule(login);
                //return PacketProcessResult.DoNothing;
                return PacketProcessResult.ContinueLoop;
            }
            return PacketProcessResult.DoNothing;
        }
    }
}
