using System;
using SilkroadSecurityApi;

namespace sroprot.NetEngine.AgentServer
{
    class Alchemy
    {
        public static PacketProcessResult HandleClient(Packet pck, RelaySession session, SilkroadServer server)
        {
            byte type_1 = pck.ReadUInt8();
            if(Global.MaxOptLevel > 0 && type_1 == 2)
            {
                byte type_2 = pck.ReadUInt8();
                if(type_2 == 3)
                {
                    pck.ReadUInt8();
                    byte slot = pck.ReadUInt8();
                    string charname = session.State["charname"] as string;

                    if (Global.dbmgr.checkOptLevel(charname,slot,Global.MaxOptLevel) == 0)
                    {
                        session.SendClientNotice("UIIT_STT_ANTICHEAT_MAX_OPT_LEVEL");
                        return PacketProcessResult.ContinueLoop;
                    }
                }
            }
            return PacketProcessResult.DoNothing;
        }
    }
}
