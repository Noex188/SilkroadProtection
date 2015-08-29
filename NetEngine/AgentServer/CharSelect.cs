using System;
using SilkroadSecurityApi;

namespace sroprot.NetEngine.AgentServer
{
    class CharSelect
    {
        public static PacketProcessResult HandleClient(Packet pck, RelaySession session, SilkroadServer server)
        {
            string name = pck.ReadAscii();
            session.State["charname"] = name;
            session.State["level"] = Global.dbmgr.getCharLvl(name);
            return PacketProcessResult.DoNothing;
        }
    }
}
