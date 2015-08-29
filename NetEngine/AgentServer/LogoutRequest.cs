using System;
using SilkroadSecurityApi;
namespace sroprot.NetEngine.AgentServer
{
    class LogoutRequest
    {
        public static PacketProcessResult HandleClient(Packet pck, RelaySession session, SilkroadServer server)
        {
            if (Global.EnableLogOutCooldown)
            {
                if (Convert.ToDateTime(session.State["lastLogOut"]) < DateTime.Now)
                {
                    session.State["lastLogOut"] = DateTime.Now.AddSeconds(Global.LogOutCooldownInSecond);
                }
                else
                {
                    session.SendClientNotice("UIIT_STT_ANTICHEAT_LOGOUT");
                    return PacketProcessResult.ContinueLoop;
                }
            }
            session.State["proper_logout"] = true;

            return PacketProcessResult.DoNothing;
        }
    }
}
