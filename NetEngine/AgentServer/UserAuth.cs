using System;
using SilkroadSecurityApi;
namespace sroprot.NetEngine.AgentServer
{
    class UserAuth
    {
        public static PacketProcessResult HandleClient(Packet pck, RelaySession session, SilkroadServer server)
        {
            UInt32 uint32_1 = pck.ReadUInt32();
            string uname = pck.ReadAscii();
            string passw = pck.ReadAscii();
            byte locale = pck.ReadUInt8();
            uint ukn1 = pck.ReadUInt32();
            uint ukn = pck.ReadUInt16();
            if (uname.Contains("'"))
            {
                Global.logmgr.WriteLog(LogLevel.Error, "User trying to exploit username: {0}", uname);
                return PacketProcessResult.Disconnect;
            }
            //инициализурем данные
            session.State["username"] = uname;
            session.State["isBot"] = false;
            session.State["isSafe"] = false;
            session.State["noticeDone"] = false;
            session.State["lastExchange"] = DateTime.Now;
            session.State["lastLogOut"] = DateTime.Now;
            session.State["lastStall"] = DateTime.Now;
            session.State["proper_logout"] = false;
            session.State["level"] = 0;
            //пишем в базу что юзер залогинился
            Global.dbmgr.AnticheatAuthLog(uname, session.State["ip_address"] as string);

            Global.logmgr.WriteLog(LogLevel.Notify, "User logged in [{0}]", uname);

            if (Global.EnableBotDetected && locale == Global.OriginalLocale)
            {
                session.State["isBot"] = true;
            }
            if (Global.EnableLoginProcessing)
            {
                Packet p = new Packet(0x6103);
                p.WriteUInt32(uint32_1);
                p.WriteAscii(uname);
                p.WriteAscii(Global.EnableUseSha1Salt ? Utility.HashPassword(uname, passw) : passw);
                p.WriteUInt8(Global.OriginalLocale > 0 && locale > 0 ? Global.OriginalLocale : locale);
                p.WriteUInt32(ukn1);
                p.WriteUInt16(ukn);
                // m_ModuleSecurity.Send(p);
                session.SendPacketToModule(p);
                return PacketProcessResult.ContinueLoop;
            }
            return PacketProcessResult.DoNothing;
        }
    }
}
