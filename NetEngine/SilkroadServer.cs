using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

using SilkroadSecurityApi;

namespace sroprot.NetEngine
{
    public sealed class SilkroadServer
    {
        //-----------------------------------------------------------------------------

        readonly object m_class_lock;

        Socket m_listener_sock;

        IPEndPoint m_bind_addr;
        public string BindIp { get { return m_bind_addr.Address.ToString(); } }
        public int BindPort { get { return m_bind_addr.Port; } }

        IPEndPoint m_module_addr;

        Thread m_accept_thread;
        Thread m_poll_thread;

        ManualResetEvent m_connection_completed = new ManualResetEvent(false);
        bool m_is_running;
        bool m_is_stopped;

        ServerType m_srv_type = ServerType.Unknown;
        public ServerType SrvType { get { return m_srv_type; } }


        SessionEventHandler m_session_create_handler, m_session_destroy_handler;

        List<RelaySession> m_sessions;

        public int SessionCount { get { return m_sessions.Count; } }

        List<RedirectRule> m_redirect_rules;
        public List<RedirectRule> RedirectRules { get { return m_redirect_rules; } }

        bool m_blowfish;
        bool m_sec_bytes;
        bool m_handshake;

        PacketDispatcher m_pck_processor;
        DelayedPacketDispatcher m_delayed_pck_dispatcher;


        public List<RelaySessionState> GetRelaySessionStates()
        {
            List<RelaySessionState> result = new List<RelaySessionState>();

            foreach (var context in m_sessions)
            {
                result.Add(context.State);
            }
            return result;
        }

        //Finds count of users which are logged in (by ip)
        public int GetUserCountForIp(string ipAddress)
        {
            int res = 0;
            var items = GetRelaySessionStates();

            foreach(var item in items)
            {
                string username = item["username"] as string;

                if (item["ip_address"] as string == ipAddress && username.Length > 0)
                    res++;

            }
            return res;
        }

        //-----------------------------------------------------------------------------

        public SilkroadServer(IPEndPoint bind_addr, IPEndPoint module_addr, ServerType srv_type, bool blowfish, bool sec_bytes, bool handshake, PacketDispatcher packetProcessor, DelayedPacketDispatcher delayedPacketDispatcher, List<RedirectRule> redirs = null)
        {
            m_listener_sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_sessions = new List<RelaySession>();
            m_bind_addr = bind_addr;
            m_module_addr = module_addr;
            m_srv_type = srv_type;
            m_class_lock = new object();
            m_session_create_handler = new SessionEventHandler(OnContextCreate);
            m_session_destroy_handler = new SessionEventHandler(OnContextDestroy);

            m_blowfish = blowfish;
            m_sec_bytes = sec_bytes;
            m_handshake = handshake;

            

            m_redirect_rules = redirs;
            m_pck_processor = packetProcessor;
            m_delayed_pck_dispatcher = delayedPacketDispatcher;
        }

        public bool HasDownloadRedirectRules()
        {
            for (int i = 0; i < m_redirect_rules.Count; i++)
            {
                if (m_redirect_rules[i].DestModuleType == ServerType.DownloadServer)
                    return true;
            }
            return false;
        }

        public bool HasAgentRedirectRules()
        {
            for (int i = 0; i < m_redirect_rules.Count; i++)
            {
                if (m_redirect_rules[i].DestModuleType == ServerType.AgentServer)
                    return true;
            }
            return false;
        }


        //-----------------------------------------------------------------------------


        void OnContextCreate(RelaySession context)
        {
            lock (m_class_lock)
            {
                m_sessions.Add(context);
                Global.SessionCount++;
            }
        }

        void OnContextDestroy(RelaySession context)
        {
            lock (m_class_lock)
            {
                m_sessions.Remove(context);
                Global.SessionCount--;
            }

            if (m_srv_type == ServerType.AgentServer)
            {
                bool isProperLogout = false;
                bool.TryParse(context.State["proper_logout"] as string, out isProperLogout);
                string username = context.State["username"] as string;

                if (isProperLogout)
                {
                    Global.logmgr.WriteLog(LogLevel.Notify, "User [{0}] logged out", username);
                }
                else
                {
                    Global.logmgr.WriteLog(LogLevel.Notify,
                        "User with ip [{0}] uname [{1}] has been disconnected", 
                        context.State["ip_address"] as string,
                        (username.Length> 0 ) ? username : "<none>");
                }
            }
        }



        //-----------------------------------------------------------------------------

        public bool Start()
        {
            if (m_is_stopped)
            {
                throw new Exception("This server object was already used. Create new.");
            }

            try
            {
                m_listener_sock.Bind(m_bind_addr);
            }
            catch (SocketException)
            {
                Global.logmgr.WriteLog(LogLevel.Error, "Failed to start server because of BIND error");
                return false;
            }

            try
            {
                m_listener_sock.Listen(50);
            }
            catch (SocketException)
            {
                Global.logmgr.WriteLog(LogLevel.Error, "Failed to start server because of LISTEN error");
                return false;
            }

            m_accept_thread = new Thread(AcceptClientWorker);
            m_poll_thread = new Thread(UserContextPollWorker);


            Global.logmgr.WriteLog(LogLevel.Notify, "Server started [{0}] -> [{1}] [bf:{2} sb:{3} hs:{4}] type {5}", m_bind_addr, m_module_addr, m_blowfish, m_sec_bytes, m_handshake, m_srv_type);
            m_is_running = true;

            m_accept_thread.Start();
            m_poll_thread.Start();

            return true;
        }

        

        //-----------------------------------------------------------------------------

        int GetConnectionCountBySession(RelaySession context)
        {
            int nCount = 0;
            lock (m_class_lock)
            {
                try
                {
                    string my_ip = context.State["ip_address"] as string;

                    foreach (var item in m_sessions)
                    {
                        if (item.State["ip_address"].ToString() == my_ip)
                        {
                            nCount++;
                        }
                    }
                }
                catch { }

            }
            return nCount;
        }

        void KillConnectionsForIP(string ip)
        {

            List<RelaySession> SessionsToRemove = new List<RelaySession>();

            lock (m_class_lock)
            {
                foreach (var item in m_sessions)
                {

                    if (item.State["ip_address"].ToString() == ip)
                    {
                        SessionsToRemove.Add(item);
                        //fix me ?
                        item.Stop(false);

                    }
                }
            }


            foreach (var item in SessionsToRemove)
            {
                OnContextDestroy(item);
            }

        }



        //Пул контекстов - обнаружение превышения ограничений трафика, отключения и т.п.
        void UserContextPollWorker()
        {
            while (m_is_running)
            {
                try
                {
                    //lock(m_class_lock)
                    {
                       
                        foreach (var item in m_sessions)
                        {
                            if (!item.SocketsRunning)
                            {
                                item.Stop(true);
                            }

                            //Check for new banned IP addresses
                           
                            int ipConnCount = GetConnectionCountBySession(item);
                            string remoteEp = item.State["ip_address"].ToString();

                            //Blocked IP address can be also added from other places (PacketDispatcher)
                            if(Global.BlockedIpAddresses.Contains(remoteEp))
                            {
                                if(Global.EnableTrafficAbuserReport)
                                {
                                    string username = item.State["username"] as string;
                                    Global.logmgr.WriteLog(LogLevel.Notify, "Exploit abuser detected. Connections: {0} IP: {1} uname: {2} srv: {3}",
                                        ipConnCount,
                                        remoteEp,
                                        (username.Length > 0) ? username : "none",
                                        m_srv_type);
                                }
                                KillConnectionsForIP(remoteEp);
                                continue;
                            }

                            if (ipConnCount > Global.PerAddressConnectionLimit)
                            {
                                if (!Global.BlockedIpAddresses.Contains(remoteEp))
                                {
                                    Global.BlockedIpAddresses.Add(remoteEp);
                                    if (Global.EnableTrafficAbuserReport)
                                    {
                                        Global.logmgr.WriteLog(LogLevel.Warning,
                                            "Traffic abuser detected. Connections: {0} Addr: {1}",
                                            ipConnCount, remoteEp);
                                    }

                                    KillConnectionsForIP(remoteEp);

                                    continue;
                                }
                            }
                        }
                        

                    }
                }
                catch
                {
                   // Global.g_LogManager.WriteLog(LogLevel.Warning, "Error at user context poll");
                }
                Thread.Sleep(100);
            }
        }

        //-----------------------------------------------------------------------------

        public int GetTotalBpsRecv()
        {
            int result = 0;
            /*
            lock(m_ContextUpdateLock)
            {
                for (int i = 0; i < m_UserContexts.Count; i++)
                {
                    result += m_UserContexts[i].State.BytesPerSecondRecv;
                }
            }
            */
            return result;
        }

        public int GetTotalBpsSend()
        {
            int result = 0;

            /*
            lock(m_ContextUpdateLock)
            {
                for (int i = 0; i < m_UserContexts.Count; i++)
                {
                    result += m_UserContexts[i].State.BytesPerSecondSend;
                }
            }
             * */

            return result;
        }


        public void StopContexts()
        {
            lock (m_class_lock)
            {
                foreach (var item in m_sessions)
                {
                    item.Stop();
                }
                m_sessions.Clear();
            }
        }

        //-----------------------------------------------------------------------------

        public void Stop()
        {
            lock (m_class_lock)
            {
                m_is_running = false;


                StopContexts();

                try
                {
                    m_accept_thread.Abort();
                }
                finally
                {
                    m_accept_thread = null;
                }

                try
                {
                    m_poll_thread.Abort();
                }
                finally
                {
                    m_poll_thread = null;
                }

                try
                {
                    m_listener_sock.Close();
                }
                finally
                {
                    m_listener_sock = null;
                }

                m_is_stopped = true;
            }
        }

        //-----------------------------------------------------------------------------

        void AcceptClientWorker()
        {
            while (m_is_running)
            {
                try
                {
                    m_connection_completed.Reset();
                    m_listener_sock.BeginAccept(new AsyncCallback(OnAcceptCallback), null);
                    m_connection_completed.WaitOne();
                }
                catch (Exception)
                {
                    Global.logmgr.WriteLog(LogLevel.Warning, "Failed to BeginAccept on server [{0}]", m_bind_addr);
                }
            }
        }

        //-----------------------------------------------------------------------------

        void OnAcceptCallback(IAsyncResult iar)
        {
            m_connection_completed.Set();

            try
            {
                Socket client = m_listener_sock.EndAccept(iar);


                RelaySessionArgs ContextArguments = new RelaySessionArgs()
                {
                    ClientSocket = client,
                    ModuleEp = m_module_addr,
                    eOnCreate = m_session_create_handler,
                    eOnDestroy = m_session_destroy_handler,
                    Blowfish = m_blowfish,
                    SecBytes = m_sec_bytes,
                    Handshake = m_handshake,
                    SrvType = m_srv_type
                };

                RelaySession context = new RelaySession(ContextArguments, m_pck_processor );


                //is set from constructor of UserContext
                string ip_address = context.State["ip_address"] as string;

                if (!Global.BlockedIpAddresses.Contains(ip_address))
                {
                    context.Start();
                }
                else
                {
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                }

            }
            catch(Exception e)
            {
                Global.logmgr.WriteLog(LogLevel.Notify, "Error at silkroadserver onacceptcallback");
                Global.logmgr.WriteLog(LogLevel.Notify, e.ToString());
            }
        }

        //-----------------------------------------------------------------------------

        public void BroadcastClientPacket(Packet pck)
        {
            lock (m_class_lock)
            {
                foreach (var item in m_sessions)
                {
                    item.SendPacketToClient(pck);
                }
            }
        }

        public void BroadcastToLoggedInChars(Packet pck, int afterLoggedInSeconds = 0)
        {
            
            lock (m_class_lock)
            {
                foreach (var item in m_sessions)
                {
                    string cname = item.State["charname"] as string;
                    
                    if (cname.Length > 0)
                    {

                        if(afterLoggedInSeconds > 0)
                        {
                            TimeSpan diff = DateTime.Now - item.State.StartupTime;
                            if(diff.TotalSeconds > afterLoggedInSeconds)
                            {
                                item.SendPacketToClient(pck);
                            }
                        }
                        else item.SendPacketToClient(pck);
                    }
                }
            }
        }

        public void BroadcastModulePacket(Packet pck)
        {
            lock (m_class_lock)
            {
                foreach (var item in m_sessions)
                {
                    item.SendPacketToModule(pck);
                }
            }
        }

        public bool SendPacketToCharname(Packet pck, string charname)
        {
            bool flag = false;
            lock (m_class_lock)
            {
                foreach (var item in m_sessions)
                {
                    if (item.State["charname"] as string == charname)
                    {
                        flag = true;
                        item.SendPacketToClient(pck);
                        break;
                    }
                }
            }
            return flag;
        }

        public bool SendPacketToUsername(Packet pck, string username)
        {
            bool flag = false;
            lock (m_class_lock)
            {
                foreach (var item in m_sessions)
                {
                    if (item.State["username"] as string == username)
                    {
                        flag = true;
                        item.SendPacketToClient(pck);
                        break;
                    }
                }
            }
            return flag;
        }

        //-----------------------------------------------------------------------------
    }
}
