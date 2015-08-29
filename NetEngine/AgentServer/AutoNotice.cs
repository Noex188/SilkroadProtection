using System;
using SilkroadSecurityApi;

namespace sroprot.NetEngine.AgentServer
{
    class AutoNotice
    {
        public static void HandleDelayedPacket(SilkroadServer server, string noticeText)
        {
            if (server.SessionCount > 0)
            {
                Packet notice = new Packet(0x3026);
                notice.WriteUInt8(7);
                notice.WriteAscii(noticeText,Global.TextEncodeCode);

                //Only chars which are logged in for 60 seconds will receive the message
                server.BroadcastToLoggedInChars(notice, Global.AutoNoticeSendBeginDelay);
            }
        }
    }
}
