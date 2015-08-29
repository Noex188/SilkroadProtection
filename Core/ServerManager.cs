using System;
using System.Net;
using System.Collections.Generic;
using sroprot.NetEngine;

using SilkroadSecurityApi;

namespace sroprot.Core
{
    public sealed class ServerManager
    {
        //-----------------------------------------------------------------------------

        static ServerManager m_instance = null;
        private ServerManager() { }

        public static ServerManager getInstance()
        {
            if (m_instance == null)
            {
                m_instance = new ServerManager();
            }
            return m_instance;
        }


        List<SilkroadServer> m_servers = new List<SilkroadServer>();
        public List<SilkroadServer> Servers { get { return m_servers; } }

        //-----------------------------------------------------------------------------

        //Проеряет есть ли уже сервер с указанным прослушывающим адресом
        bool BindExists(string bind_addr, int bind_port)
        {
            for (int i = 0; i < m_servers.Count; i++)
            {
                if (m_servers[i].BindIp == bind_addr && m_servers[i].BindPort == bind_port)
                {
                    return true;
                }
            }
            return false;
        }

        //-----------------------------------------------------------------------------

        public SilkroadServer CreateNew(string bind_addr, int bind_port, string module_addr, int module_port, ServerType srvtype, bool blowfish, bool sec_bytes, bool handshake, PacketDispatcher packetProcessor, DelayedPacketDispatcher delayedPacketDispatcher, List<RedirectRule> redirs = null)
        {
            if (BindExists(bind_addr, bind_port))
            {
                Global.logmgr.WriteLog(LogLevel.Error, "Server with given bind address already exists [{0}:{1}]", bind_addr, bind_port);
                return null;
            }
            try
            {
                SilkroadServer ServerItem =
                    new SilkroadServer(
                        new IPEndPoint(IPAddress.Parse(bind_addr), bind_port),
                        new IPEndPoint(IPAddress.Parse(module_addr), module_port),
                        srvtype,
                        blowfish,
                        sec_bytes,
                        handshake,
                        packetProcessor,
                        delayedPacketDispatcher,
                        redirs
                        );

                m_servers.Add(ServerItem);
                ServerItem.Start();
                return ServerItem;
            }
            catch
            {
                Global.logmgr.WriteLog(LogLevel.Error, "ServerManager failed to start server, SilkroadServer start error");
            }
            return null;
        }

        //-----------------------------------------------------------------------------

        public void StopAllServers()
        {

            for (int i = 0; i < m_servers.Count; i++)
            {
                m_servers[i].StopContexts();
                m_servers[i].Stop();
            }

            m_servers.Clear();
        }


        public void StopAllContexts()
        {
            for (int i = 0; i < m_servers.Count; i++)
            {
                m_servers[i].StopContexts();
            }
        }

        //-----------------------------------------------------------------------------

        public int GetAvgBpsRecv()
        {
            int result = 0;
            foreach (var item in m_servers)
            {
                result += item.GetTotalBpsRecv();
            }

            
            int nSrvCount = m_servers.Count;
            if (nSrvCount == 0)
                nSrvCount = 1;


            if (result == 0)
                return 0;

            result = (result / m_servers.Count);
            return result;
        }

        public int GetAvgBpsSend()
        {
            int result = 0;
            foreach (var item in m_servers)
            {
                result += item.GetTotalBpsSend();
            }

            int nSrvCount = m_servers.Count;
            if (nSrvCount == 0)
                nSrvCount = 1;


            if (result == 0)
                return 0;

            result = (result / m_servers.Count);

            return result;
        }

        //-----------------------------------------------------------------------------

        public List<RelaySessionState> GetAllContextStates()
        {
            List<RelaySessionState> result = new List<RelaySessionState>();
            foreach (var srv in m_servers)
            {
                result.AddRange(srv.GetRelaySessionStates());
            }
            return result;
        }

        public void BroadcastPacketToServerClients(Packet pck, ServerType srvtype)
        {
            foreach (var srv in m_servers)
            {
                if (srv.SrvType == srvtype)
                {
                    srv.BroadcastClientPacket(pck);
                }
            }
        }

        public bool SendPacketToClientUsername(Packet pck, ServerType srvtype, string username)
        {
            bool flag = false;
            foreach(var srv in m_servers)
            {
                if(srv.SendPacketToUsername(pck, username))
                {
                    if (!flag)
                        flag = true;
                }
            }
            return flag;
        }

        public bool SendPacketToClientCharname(Packet pck, ServerType srvtype, string charname)
        {
            bool flag = false;
            foreach(var srv in m_servers)
            {
                if(srv.SendPacketToCharname(pck, charname))
                {
                    if (!flag)
                    {
                        flag = true;
                    }
                }
            }
            return flag;
        }

        public int GetUserCountByIpForAgent(string ipAddress)
        {
            int res = 0;
            Console.WriteLine("srvcnt: {0}", m_servers.Count);

            foreach(var srv in m_servers)
            {
                if(srv.SrvType == ServerType.AgentServer)
                {
                    Console.WriteLine("is agent");
                    res += srv.GetUserCountForIp(ipAddress);
                }
            }
            return res;
        }

        /*
        public Packet GetLastPacketByServType(ServerType SrvType)
        {
            for(int i = 0; i < m_SilkroadServers.Count; i++)
            {
                if (m_SilkroadServers[i].SrvType == SrvType)
                    return m_SilkroadServers[i].LastPacket;
            }
            return null;
        }
        */
        //-----------------------------------------------------------------------------
    }
}
