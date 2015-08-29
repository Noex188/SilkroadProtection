using System;
using SilkroadSecurityApi;

namespace sroprot.NetEngine.AgentServer
{
    class SafeRegionSkill
    {
        public static PacketProcessResult HandleClient(Packet pck, RelaySession session, SilkroadServer server)
        {
            if (Global.UseSafeRegion)
            {
                bool isSafe = false;
                bool.TryParse(session.State["isSafe"] as string, out isSafe);

                bool isBot = false;
                bool.TryParse(session.State["isBot"] as string, out isBot);

                //Запрет использования скилов в регионе
                if (isSafe)
                {
                    if (pck.ReadUInt8() == 1)
                    {
                        //запрет на использование скилов и (обычная атака ==1)
                        if (pck.ReadUInt8() == 4)
                        {
                            session.SendClientNotice("UIIT_STT_ANTICHEAT_USE_SKILL");
                            return PacketProcessResult.ContinueLoop;
                        }
                    }

                }

            }
            return PacketProcessResult.DoNothing;
        }
    }
}
