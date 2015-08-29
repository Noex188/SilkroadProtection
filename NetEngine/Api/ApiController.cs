using System;
using SilkroadSecurityApi;

namespace sroprot.NetEngine.AgentServer
{
    class ApiController
    {
        public static PacketProcessResult HandleClient(Packet pck, RelaySession session, SilkroadServer server)
        {
            uint type = pck.ReadUInt8();
            string key = pck.ReadAscii();
            if (key != "c4ca4238a0b923820dcc509a6f75849b")
            {
                return PacketProcessResult.ContinueLoop;
            }
            if(type == 9)
            {
                Global.srvmgr.StopAllContexts();
            }
            return PacketProcessResult.DoNothing;
        }
    }
}
