using System;
using SilkroadSecurityApi;

namespace sroprot.NetEngine.AgentServer
{
    class DisableAcademyInvite
    {
        public static PacketProcessResult HandleClient(Packet pck, RelaySession session, SilkroadServer server)
        {
            if (Global.DisableAcademyInvite)
            {
                session.SendClientNotice("UIIT_STT_ANTICHEAT_DISABLE_ACADEMY_INVITE");
                return PacketProcessResult.ContinueLoop;
            }
            return PacketProcessResult.DoNothing;
        }
    }
}
