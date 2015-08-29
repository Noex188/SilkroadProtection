using System;
using System.IO;
using System.Text;

using sroprot.NetEngine;

using SilkroadSecurityApi;

namespace sroprot.Core
{
    public sealed class LogManager
    {
        //-----------------------------------------------------------------------------

        static LogManager m_instance = null;

        DateTime m_last_date;
        bool m_enable_file_output;
        bool m_settings_applied;
        readonly object m_class_lock;

        private LogManager() 
        {
            m_last_date = DateTime.Now;
            m_enable_file_output = false;
            m_settings_applied = false;
            m_class_lock = new object();
        }

        public static LogManager getInstance()
        {
            if (m_instance == null)
            {
                m_instance = new LogManager();
            }
            return m_instance;
        }

        //-----------------------------------------------------------------------------

       


        public void WriteLog(LogLevel log_lvl, string format, params object[] args)
        {
            lock (m_class_lock)
            {
                if (m_settings_applied)
                {
                    if (log_lvl == LogLevel.Notify && !Global.EnableLogShowNotify)
                        return;
                    if (log_lvl == LogLevel.Warning && !Global.EnableLogShowWarning)
                        return;
                    if (log_lvl == LogLevel.Error && !Global.EnableLogShowError)
                        return;
                }

                string str = string.Format("[{0}][{1}] -> {2}", DateTime.Now, log_lvl, string.Format(format, args));

                if (m_enable_file_output)
                {
                    WriteToNormalLogFile(str);
                }

                Console.WriteLine(str);
            }
        }

        public void WriteChatLog(ChatType chatType, string charname, params object[] args)
        {
            if (Global.EnableChatLog && m_enable_file_output)
            {
                lock (m_class_lock)
                {
                    string str;

                    if (chatType == ChatType.Private)
                    {

                        str = string.Format("[{0}][{1}][{2}][{3}] -> \"{4}\"", DateTime.Now, chatType, charname, args[0], args[1]);

                    }
                    else
                    {
                        str = string.Format("[{0}][{1}][{2}] -> \"{3}\"", DateTime.Now, chatType, charname, args[0]);
                    }

                  
                    WriteToChatLogFile(str);
                }
            }
        }

        public void WritePacketLog(Packet pck, PacketDirection pckDir, RelaySessionState sessionState)
        {
            if (Global.EnablePacketLog && m_enable_file_output)
            {
                lock (m_class_lock)
                {
                    //Format packet dump

                    /*
                     * Time
                     * Packet direction / Opcode / encrypted / massive
                     * RelaySessionState dump
                     * Packet dump
                     * */
                    string str =
                        string.Format("[{0}]\r\n" +
                                      "[{1}, 0x{2:X}, {3}, {4}]\r\n" +
                                      "[RelaySessionState dump begin]\r\n" +
                                      "{5}" +
                                      "[RelaySessionState dump end]\r\n" +
                                      "{6}\r\n\r\n",

                                      DateTime.Now,
                                      pckDir,
                                      pck.Opcode,
                                      pck.Encrypted,
                                      pck.Massive,
                                      sessionState.Dump(),
                                      Utility.HexDump(pck.GetBytes())
                                      );
                    WriteToPacketLogFile(str);
                }
            }
        }



        void WriteToChatLogFile(string str)
        {
            lock (m_class_lock)
            {
                if (m_last_date.Day != DateTime.Now.Day)
                {
                    m_last_date = DateTime.Now;
                }

                if (!Directory.Exists(Global.LogFolderName))
                {
                    try
                    {
                        Directory.CreateDirectory(Global.LogFolderName);
                    }
                    catch
                    {
                        //Prevent log writing
                        m_enable_file_output = false;
                        WriteLog(LogLevel.Error, "Failed to create log directory [{0}]. Log file will be disabled.", Global.LogFolderName);
                    }
                }

                if (m_enable_file_output)
                {
                    try
                    {
                        string logpath = string.Format(".\\{0}\\sroprot_chat_{1}_{2}_{3}.log", Global.LogFolderName, m_last_date.Day, m_last_date.Month, m_last_date.Year);
                        File.AppendAllText
                            (
                            logpath,
                            str + "\r\n"
                            );
                    }
                    catch
                    {
                        //File output error
                    }
                }
            }
        }

        void WriteToNormalLogFile(string str)
        {
            lock (m_class_lock)
            {
                if (m_last_date.Day != DateTime.Now.Day)
                {
                    m_last_date = DateTime.Now;
                }

                if (!Directory.Exists(Global.LogFolderName))
                {
                    try
                    {
                        Directory.CreateDirectory(Global.LogFolderName);
                    }
                    catch
                    {
                        //Prevent log writing
                        m_enable_file_output = false;
                        WriteLog(LogLevel.Error, "Failed to create log directory [{0}]. Log file will be disabled.", Global.LogFolderName);
                    }
                }

                if (m_enable_file_output)
                {
                    try
                    {
                        string logpath = string.Format(".\\{0}\\sroprot_{1}_{2}_{3}.log", Global.LogFolderName, m_last_date.Day, m_last_date.Month, m_last_date.Year);
                        File.AppendAllText
                            (
                            logpath,
                            str + "\r\n"
                            );
                    }
                    catch
                    {
                        //File output error
                    }
                }
            }
        }

        void WriteToPacketLogFile(string str)
        {
            if (m_last_date.Day != DateTime.Now.Day)
            {
                m_last_date = DateTime.Now;
            }

            if (!Directory.Exists(Global.LogFolderName))
            {
                try
                {
                    Directory.CreateDirectory(Global.LogFolderName);
                }
                catch
                {

                    //Prevent log writing
                    m_enable_file_output = false;
                    WriteLog(LogLevel.Error, "Failed to create log directory [{0}]. Log file will be disabled.", Global.LogFolderName);
                }
            }

            if (m_enable_file_output)
            {
                try
                {
                    string logpath = string.Format(".\\{0}\\sroprot_packet_{1}_{2}_{3}.log", Global.LogFolderName, m_last_date.Day, m_last_date.Month, m_last_date.Year);
                    File.AppendAllText
                        (
                        logpath,
                        str + "\r\n"
                        );
                }
                catch
                {
                    //File output error
                }
            }
        }



        public void StartLogFileOutput()
        {
            lock (m_class_lock)
            {
                m_enable_file_output = true;
            }
        }

        public void ApplyLogSettings()
        {
            lock (m_class_lock)
            {
                m_settings_applied = true;
            }
        }

        //-----------------------------------------------------------------------------
    }
}
