using System;
using SilkroadSecurityApi;

namespace sroprot.NetEngine.AgentServer
{
    class GuildInvite
    {
        public static PacketProcessResult HandleClient(Packet pck, RelaySession session, SilkroadServer server)
        {
            string charname = session.State["charname"] as string;
            if (Global.MaxMembersInGuild > 0 && Global.dbmgr.GuildMembers(charname, 1, Global.MaxMembersInGuild) == 0)
            {
                session.SendClientNotice("UIIT_STT_ANTICHEAT_MAX_MEMBERS_IN_GUILD");
                return PacketProcessResult.ContinueLoop;
            }

            return PacketProcessResult.DoNothing;
        }
    }
}
