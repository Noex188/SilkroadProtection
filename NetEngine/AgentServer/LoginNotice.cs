using System;
using SilkroadSecurityApi;

namespace sroprot.NetEngine.AgentServer
{
    class LoginNotice
    {
        public static PacketProcessResult HandleModule(Packet pck, RelaySession session, SilkroadServer server)
        {
            if (Global.EnableLoginNotice)
            {
                bool noticeDone = false;
                bool.TryParse(session.State["noticeDone"] as string, out noticeDone);
                if (!noticeDone)
                {
                    foreach (string text in Global.LoginNotice)
                    {
                        Packet notice = new Packet(0x3026);
                        notice.WriteUInt8(7);
                        notice.WriteAscii(text, Global.TextEncodeCode);
                        session.SendPacketToClient(notice);
                    }
                    session.State["noticeDone"] = true;
                }
            }
            return PacketProcessResult.DoNothing;
        }
    }
}
