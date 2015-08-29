using System;
using SilkroadSecurityApi;

namespace sroprot.NetEngine.AgentServer
{
    class StallCooldown
    {
        public static PacketProcessResult HandleClient(Packet pck, RelaySession session, SilkroadServer server)
        {
            //лоток
            if (Global.EnableStallCooldown)
            {
                if (Convert.ToDateTime(session.State["lastStall"]) < DateTime.Now)
                {
                    session.State["lastStall"] = DateTime.Now.AddSeconds(Global.StallCooldownInSecond);
                }
                else
                {
                    session.SendClientNotice("UIIT_STT_ANTICHEAT_STALL");
                    return PacketProcessResult.ContinueLoop;
                }
            }
            return PacketProcessResult.DoNothing;
        }
    }
}
