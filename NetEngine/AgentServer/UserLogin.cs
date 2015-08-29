using System;
using SilkroadSecurityApi;

namespace sroprot.NetEngine.GatewayServer
{
    /// <summary>
    /// FIX ME
    /// </summary>
    class UserLogin
    {
        public static PacketProcessResult HandleClient(Packet pck, RelaySession session)
        {
            byte locale = pck.ReadUInt8();
            string username = pck.ReadAscii();
            string password = pck.ReadAscii();
            ushort ServerID = pck.ReadUInt16();
            //проверить
            if (Global.EnableIpAccountLimitation)
            {

                int nResult = Global.dbmgr.GetIpLimitationResult(username, Utility.GetRemoteEpString(session.Arguments.ClientSocket));
                if (nResult == 0)
                {
                    Packet login_response = new Packet(0xA102, false);
                    login_response.WriteUInt8(0x02);
                    login_response.WriteUInt8(12);
                    session.SendPacketToClient(login_response);
                    //drop conn
                    return PacketProcessResult.Disconnect;
                }
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
