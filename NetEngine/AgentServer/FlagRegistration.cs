using System;
using SilkroadSecurityApi;
namespace sroprot.NetEngine.AgentServer
{
    class FlagRegistration
    {
        public static PacketProcessResult HandleClient(Packet pck, RelaySession session, SilkroadServer server)
        {
            int level = 0;
            int.TryParse(session.State["level"] as string, out level);
            if (Global.FlagRegistrationLevel > level && Global.FlagRegistrationLevel > 0)
            {
                session.SendClientNotice("UIIT_STT_ANTICHEAT_ARENA_LEVEL");
                return PacketProcessResult.ContinueLoop;
            }

            if (Global.DisableBotArenaRegistration)
            {
                bool isBot = false;
                bool.TryParse(session.State["isBot"] as string, out isBot);

                //регистрация на арену
                if (isBot)
                {
                    session.SendClientNotice("UIIT_STT_ANTICHEAT_USE_FUNCTION");
                    return PacketProcessResult.ContinueLoop;
                }

            }
            return PacketProcessResult.DoNothing;
        }
    }
}
