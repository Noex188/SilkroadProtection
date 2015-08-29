using System;
using SilkroadSecurityApi;

namespace sroprot.NetEngine.AgentServer
{
    class UnionInvite
    {
        public static PacketProcessResult HandleClient(Packet pck, RelaySession session, SilkroadServer server)
        {
            string charname = session.State["charname"] as string;
            if (Global.MaxGuildInUnion > 0 && Global.dbmgr.GuildMembers(charname, 2, Global.MaxGuildInUnion) == 0)
            {
                session.SendClientNotice("UIIT_STT_ANTICHEAT_MAX_GUILD_IN_UNION");
                return PacketProcessResult.ContinueLoop;
            }
            return PacketProcessResult.DoNothing;
        }
    }
}
