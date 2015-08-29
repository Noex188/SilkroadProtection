using System;
using SilkroadSecurityApi;

namespace sroprot.NetEngine.AgentServer
{
    class LogoutCancel
    {
        public static PacketProcessResult HandleClient(Packet pck, RelaySession session, SilkroadServer server)
        {
            session.State["proper_logout"] = false;
            return PacketProcessResult.DoNothing;
        }
    }
}
