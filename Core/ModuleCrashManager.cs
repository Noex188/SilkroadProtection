using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

namespace sroprot.Core
{
    public sealed class ModuleCrashManager
    {
        //-----------------------------------------------------------------------------

        static ModuleCrashManager m_Instance = null;
        private ModuleCrashManager()
        {
            m_ModulePollThread = new Thread(ModulePollWorker);
        }

        Thread m_ModulePollThread = null;
        bool m_IsRunning = false;

        List<KeyValuePair<ServerType, string>> m_ServersToPoll =
            new List<KeyValuePair<ServerType, string>>();

        readonly object m_ClassLock = new object();

        public static ModuleCrashManager getInstance()
        {
            if (m_Instance == null)
            {
                m_Instance = new ModuleCrashManager();
            }
            return m_Instance;
        }

        void ModulePollWorker()
        {
            while(m_IsRunning)
            {
                try
                {
                    lock(m_ClassLock)
                    {
                        foreach (var item in m_ServersToPoll)
                        {
                            Process[] processes = Process.GetProcessesByName(item.Value);
                            if(processes.Length == 0)
                            {
                                //module not running
                                //SilkroadSecurityApi.Packet pck = Global.g_ServerManager.GetLastPacketByServType(item.Key);
                                //Global.g_LogManager.WriteLog(LogLevel.Notify, "Last packet before module exit: 0x{0:X}", pck.Opcode);
                            }
                        }
                        
                    }
                    Thread.Sleep(1);
                }
                catch
                {

                }
            }
        }
        public void Start()
        {
            if (m_IsRunning)
                throw new Exception("Module crash handler already running");

            m_IsRunning = true;
            m_ModulePollThread.Start();
        }

        public void Stop()
        {
            if (!m_IsRunning)
                return;

            try
            {
                m_ModulePollThread.Abort();
            }
            catch
            {
            }
            finally
            {
                m_ModulePollThread = null;
            }

            m_ServersToPoll.Clear();

            m_IsRunning = false;
        }

        public void AddServer(ServerType SrvType, string ModuleProcessName)
        {
            KeyValuePair<ServerType, string> item = new KeyValuePair<ServerType, string>();
            lock(m_ClassLock)
            {
                if (m_ServersToPoll.Contains(item))
                {
                    throw new Exception("Trying to add already existing server to crash poll");
                }

                m_ServersToPoll.Add(item);
            }

        }
    }
}
