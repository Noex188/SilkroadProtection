using System;
using SilkroadSecurityApi;

namespace sroprot.NetEngine.AgentServer
{
    class StartIntro
    {
        public static PacketProcessResult HandleModule(Packet pck, RelaySession session, SilkroadServer server)
        {
            if (Global.EnableStartIntro)
            {
                byte hz1 = pck.ReadUInt8();
                if (hz1 == 1)
                {
                    ulong hz2 = pck.ReadUInt64();
                    if (hz2 == 1)
                    {
                        Packet intro = new Packet(0x3CA2);
                        intro.WriteAscii(Global.StartIntroScriptName);
                        intro.WriteUInt32(12);
                        session.SendPacketToClient(intro);
                    }

                }
                return PacketProcessResult.ContinueLoop;
            }
            return PacketProcessResult.DoNothing;
        }
    }
}
