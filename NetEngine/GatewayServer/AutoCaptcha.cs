using System;
using SilkroadSecurityApi;

namespace sroprot.NetEngine.GatewayServer
{
    class AutoCaptcha
    {
        /// <summary>
        /// Sends C -> S 0x6323 packet instead of user
        /// </summary>
        /// <param name="pck"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public static PacketProcessResult HandleClient(Packet pck, RelaySession session, SilkroadServer server)
         {
             if (Global.EnableAutoCaptcha)
             {
                 Packet p = new Packet(0x6323, false);
                 p.WriteAscii(Global.AutoCaptchaValue);
                 // m_ModuleSecurity.Send(p);
                 session.SendPacketToModule(p);
                 return PacketProcessResult.ContinueLoop;
             }
             return PacketProcessResult.DoNothing;
         }
     
    }
}
