using System;
using System.Collections.ObjectModel;
using SilkroadSecurityApi;
using System.Linq;

namespace sroprot.NetEngine.AgentServer
{
    class ChatMessage
    {
        static Collection<char> s_ForbiddenChars;
        static Collection<string> s_ForbiddenAbuseChars;

        #region Static constructor

        static ChatMessage()
        {
            s_ForbiddenChars = new Collection<char>();
            s_ForbiddenAbuseChars = new Collection<string>();

            s_ForbiddenChars.Add('\'');
            s_ForbiddenChars.Add('\"');
            s_ForbiddenChars.Add('[');
            s_ForbiddenChars.Add(']');
            s_ForbiddenChars.Add('{');
            s_ForbiddenChars.Add('}');
            s_ForbiddenChars.Add('@');
            s_ForbiddenChars.Add('#');
            s_ForbiddenChars.Add('$');
            s_ForbiddenChars.Add('%');
            s_ForbiddenChars.Add('^');
            s_ForbiddenChars.Add('*');
            s_ForbiddenChars.Add('(');
            s_ForbiddenChars.Add(')');
            s_ForbiddenChars.Add('_');
            s_ForbiddenChars.Add('+');
            s_ForbiddenChars.Add('№');
            s_ForbiddenChars.Add(';');
            s_ForbiddenChars.Add('<');
            s_ForbiddenChars.Add('>');
            s_ForbiddenChars.Add('/');
            s_ForbiddenChars.Add('\\');

            //Список очистки сообщний 
            s_ForbiddenAbuseChars.Add("~");
            s_ForbiddenAbuseChars.Add("!");
            s_ForbiddenAbuseChars.Add("#");
            s_ForbiddenAbuseChars.Add("$");
            s_ForbiddenAbuseChars.Add("%");
            s_ForbiddenAbuseChars.Add("^");
            s_ForbiddenAbuseChars.Add("@");
            s_ForbiddenAbuseChars.Add("#");
            s_ForbiddenAbuseChars.Add("$");
            s_ForbiddenAbuseChars.Add("&");
            s_ForbiddenAbuseChars.Add("*");
            s_ForbiddenAbuseChars.Add("(");
            s_ForbiddenAbuseChars.Add(")");
            s_ForbiddenAbuseChars.Add("_");
            s_ForbiddenAbuseChars.Add("+");
            s_ForbiddenAbuseChars.Add("№");
            s_ForbiddenAbuseChars.Add(";");
            s_ForbiddenAbuseChars.Add("<");
            s_ForbiddenAbuseChars.Add(">");
            s_ForbiddenAbuseChars.Add(":");
            s_ForbiddenAbuseChars.Add("?");
            s_ForbiddenAbuseChars.Add("-");
            s_ForbiddenAbuseChars.Add("+");
            s_ForbiddenAbuseChars.Add("{");
            s_ForbiddenAbuseChars.Add("}");
            s_ForbiddenAbuseChars.Add("[");
            s_ForbiddenAbuseChars.Add("]");
            s_ForbiddenAbuseChars.Add("\"");
            s_ForbiddenAbuseChars.Add("|");
            s_ForbiddenAbuseChars.Add("\\");
            s_ForbiddenAbuseChars.Add("/");
            s_ForbiddenAbuseChars.Add(".");
            s_ForbiddenAbuseChars.Add(",");
            s_ForbiddenAbuseChars.Add("'");
            s_ForbiddenAbuseChars.Add("=");
            s_ForbiddenAbuseChars.Add(" ");
            s_ForbiddenAbuseChars.Add(" ");

        }
        #endregion

        static bool AbuseFilter(string msg, RelaySession session)
        {
            bool result = false;
            if (Global.EnableAbuseFilter)
            {
                //Убираемт  не нужные символы
                foreach (var ch in s_ForbiddenAbuseChars)
                {
                    msg = msg.Replace(ch, "").ToLower();
                }
                foreach (var word in Global.AbuseWord)
                {
                    if (msg.Contains(word))
                    {
                        session.SendClientPM("Некоторые выражение запрещены в игровом чате");
                        result = true;
                    }
                }

            }

            return result;

        }

        static string FilterMsg(string msg)
        {
            foreach (var ch in s_ForbiddenChars)
            {
                msg = msg.Replace(ch, ' ');
            }
            return msg;
        }

        public static PacketProcessResult HandleClient(Packet pck, RelaySession session, SilkroadServer server)
        {
            string username = session.State["username"] as string;
            string charname = session.State["charname"] as string;

            //Read type
            byte msgTypeId = pck.ReadUInt8();
            byte msgCount = pck.ReadUInt8();

            ChatType msgType = (ChatType)(msgTypeId);
            string msg = string.Empty;

            switch (msgType)
            {
                case ChatType.Normal:
                    {
                        msg = pck.ReadAscii();

                        //ProcessChatCommand(msg);
                        if (AbuseFilter(msg, session))
                        {
                            return PacketProcessResult.ContinueLoop;
                        }

                        if (ProcessChatCommand(msg, session))
                        {
                            return PacketProcessResult.ContinueLoop;
                        }

                        msg = FilterMsg(msg);
                        Global.logmgr.WriteChatLog(msgType, charname, msg);

                    }
                    break;
                case ChatType.Private:
                    {
                        string name = pck.ReadAscii();
                        msg = pck.ReadAscii();
                        if (AbuseFilter(msg, session))
                        {
                            return PacketProcessResult.ContinueLoop;
                        }
                        msg = FilterMsg(msg);
                        Global.logmgr.WriteChatLog(msgType, charname, name, msg);
                    }
                    break;
                case ChatType.Academy:
                    {
                        msg = pck.ReadAscii();
                        if (AbuseFilter(msg, session))
                        {
                            return PacketProcessResult.ContinueLoop;
                        }
                        msg = FilterMsg(msg);
                        Global.logmgr.WriteChatLog(msgType, charname, msg);
                    }
                    break;
                case ChatType.Gm:
                    {
                        msg = pck.ReadAscii();
                        if (AbuseFilter(msg, session))
                        {
                            return PacketProcessResult.ContinueLoop;
                        }
                        msg = FilterMsg(msg);
                        Global.logmgr.WriteChatLog(msgType, charname, msg);
                    }
                    break;
                case ChatType.Group:
                    {
                        msg = pck.ReadAscii();
                        if (AbuseFilter(msg, session))
                        {
                            return PacketProcessResult.ContinueLoop;
                        }
                        msg = FilterMsg(msg);
                        Global.logmgr.WriteChatLog(msgType, charname, msg);
                    }
                    break;
                case ChatType.Guild:
                    {
                        msg = pck.ReadAscii();
                        if (AbuseFilter(msg, session))
                        {
                            return PacketProcessResult.ContinueLoop;
                        }
                        msg = FilterMsg(msg);
                        Global.logmgr.WriteChatLog(msgType, charname, msg);
                    }
                    break;
                case ChatType.Union:
                    {
                        msg = pck.ReadAscii();
                        if (AbuseFilter(msg, session))
                        {
                            return PacketProcessResult.ContinueLoop;
                        }
                        msg = FilterMsg(msg);
                        Global.logmgr.WriteChatLog(msgType, charname, msg);
                    }
                    break;
                case ChatType.Stall:
                    {
                        msg = pck.ReadAscii();
                        if (AbuseFilter(msg, session))
                        {
                            return PacketProcessResult.ContinueLoop;
                        }
                        msg = FilterMsg(msg);
                        Global.logmgr.WriteChatLog(msgType, charname, msg);
                    }
                    break;
                case ChatType.Anounce:
                    {
                        if (Global.EnableGmAccessControl)
                        {
                            if (Global.dbmgr.checkGmAccessControl(username, 3000) == 0)
                            {
                                session.SendClientNotice("UIIT_STT_ANTICHEAT_GM_USE_COMMAND");
                                //Block this packet, since we spoof it
                                return PacketProcessResult.ContinueLoop;
                            }
                        }

                    }
                    break;
                default:
                    {
                        Global.logmgr.WriteLog(LogLevel.Notify, "Unknown chat msg id {0}", msgTypeId);
                    }
                    break;
            }

            return PacketProcessResult.DoNothing;

        }

        /// <summary>
        /// Do chat message processing here
        /// Messages sent to this method aren't filtered
        /// </summary>
        /// <param name="msg"></param>
        static bool ProcessChatCommand(string msg, RelaySession session)
        {
            bool result = false;
            if (Global.EnableChatCommands && msg.StartsWith("."))
            {
                if (msg.StartsWith(".penis"))
                {
                    string[] split = msg.Split(' ');
                    if (split.Count() == 2 && split[1].Length <= 16)
                    {
                        session.SendClientPM("Длинна письки " + split[1].Length + " см.");
                        result = true;
                    }
                }

                if (msg.StartsWith(".info"))
                {
                    session.SendClientPM("Software: Anticheat[sroprot]");
                    session.SendClientPM("Developers: Xoka && dwordptr");
                    session.SendClientPM("Special for: sairos-online.com");
                    session.SendClientPM("Version: "+ Global.SoftVersion);
                    result = true;
                }

                if (msg.StartsWith(".server"))
                {
                   foreach(var str in Global.ServerInfo)
                    {
                        session.SendClientPM(str);
                    }
                    result = true;
                }

                if (msg.StartsWith(".schedule"))
                {
                    foreach (var str in Global.ServerSchedule)
                    {
                        session.SendClientPM(str);
                    }
                    result = true;
                }
            }
            return result;
        }
    }
}
