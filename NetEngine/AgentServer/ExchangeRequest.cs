using System;
using SilkroadSecurityApi;

namespace sroprot.NetEngine.AgentServer
{
    class ExchangeRequest
    {
        public static PacketProcessResult HandleClient(Packet pck, RelaySession session, SilkroadServer server)
        {
            if (Global.EnableExchangeCooldown)
            {
                if (Convert.ToDateTime(session.State["lastExchange"]) < DateTime.Now)
                {
                    session.State["lastExchange"] = DateTime.Now.AddSeconds(Global.ExchangeCooldownInSecond);
                }
                else
                {
                    session.SendClientNotice("UIIT_STT_ANTICHEAT_EXCHANGE");
                    return PacketProcessResult.ContinueLoop;
                }

            }

            return PacketProcessResult.DoNothing;
        }
    }
}
