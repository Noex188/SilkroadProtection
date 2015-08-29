using System;
using System.Text;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

using SilkroadSecurityApi;

namespace sroprot.NetEngine
{
    public sealed class RelaySession
    {
        //-----------------------------------------------------------------------------
        readonly object m_io_lock;


        RelaySessionArgs m_args;
        public RelaySessionArgs Arguments { get { return m_args; } }

        Socket m_module_sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        bool m_is_stopped = false;


        RelaySessionState m_state;
        public RelaySessionState State { get { return m_state; } }


        bool m_module_connected;
        public bool ModuleConnected { get { return m_module_connected; } }

        PacketDispatcher m_pck_processor;

        byte[] m_client_buffer, m_module_buffer;

        Security m_client_sec;
        Security m_module_sec;

        public bool SocketsRunning
        {
            get
            {
                return (SocketConnected(m_args.ClientSocket) && SocketConnected(m_module_sock));
            }
        }



        //-----------------------------------------------------------------------------

        public RelaySession(RelaySessionArgs arg, PacketDispatcher packetProcessor)
        {
            m_io_lock = new object();
            m_args = arg;
            m_pck_processor = packetProcessor;
            m_state = new RelaySessionState();
            m_module_connected = false;
            m_client_sec = new Security();
            m_module_sec = new Security();
            m_client_buffer = new byte[Global.RecvBuffersSize];
            m_module_buffer = new byte[Global.RecvBuffersSize];

            m_state["ip_address"] = Utility.GetRemoteEpString(m_args.ClientSocket);

            m_client_sec.GenerateSecurity(m_args.Blowfish, m_args.SecBytes, m_args.Handshake);

            m_args.ClientSocket.NoDelay = true;
            m_module_sock.NoDelay = true;
        }

        //-----------------------------------------------------------------------------


        //Контекст запускается из SilkroadContext 
        public void Start()
        {
            m_args.eOnCreate.Invoke(this);

            if (ConnectToModule())
            {
                m_module_connected = true;

                DoRecvFromClient();
                DoRecvFromModule();
            }
        }

        //-----------------------------------------------------------------------------

        //Подключаемся к модулю
        bool ConnectToModule()
        {
            try
            {
                IAsyncResult iar = m_module_sock.BeginConnect(m_args.ModuleEp, null, null);
                m_module_sock.EndConnect(iar);

                return true;
            }
            catch
            {
                Stop();
            }
            return false;
        }

        //-----------------------------------------------------------------------------

        void DoRecvFromClient()
        {
            try
            {
                m_args.ClientSocket.BeginReceive(m_client_buffer, 0, m_client_buffer.Length, SocketFlags.None, new AsyncCallback(DoRecvFromClientCallback), null);
            }
            catch
            {
                Stop();
            }
        }

        //-----------------------------------------------------------------------------


        //Передаём пакеты клиента
        void TransferClient()
        {
            try
            {
                var kvp = m_client_sec.TransferOutgoing();
                if (kvp != null)
                {
                    foreach (var item in kvp)
                    {
                        DoSendToClient(item.Key.Buffer, item.Key.Buffer.Length);
                    }

                    kvp.Clear();
                }
            }
            catch
            {
                Stop();
            }
        }

        //-----------------------------------------------------------------------------

        //Передаём пакеты модуля

        void TransferModule()
        {
            try
            {
                var kvp = m_module_sec.TransferOutgoing();
                if (kvp != null)
                {
                    foreach (var item in kvp)
                    {
                        DoSendToModule(item.Key.Buffer, item.Key.Buffer.Length);
                    }
                    kvp.Clear();
                }
            }
            catch
            {
                Stop();
            }
        }

        //меиод для генерации пароль, пока пусть тут так как хз куда его приткнуть
        public string createPassword(string uname, string password)
        {
            return Utility.GetSha1Hash(uname + password + Global.Sha1PasswordSalt);
        }



        //-----------------------------------------------------------------------------

        //Хэндлим пакеты от клиента к модулю
        void DoRecvFromClientCallback(IAsyncResult iar)
        {
            try
            {

                int nRecv = m_args.ClientSocket.EndReceive(iar);

                if (nRecv == 0)
                {
                    Stop();
                    return;
                }


                //m_State.RegisterRecvTraffic(nRecv);

                Global.TotalBytesIn += (ulong)(nRecv);

                //process Client -> Module
                m_client_sec.Recv(m_client_buffer, 0, nRecv);

                List<Packet> ClientPackets = m_client_sec.TransferIncoming();
                if (ClientPackets != null)
                {
                    for (int i = 0; i < ClientPackets.Count; i++)
                    {
                        var pck = ClientPackets[i];

                        
                        //Даём SSA хэндлить внутренние пакеты
                        if (pck.Opcode == 0x9000 || pck.Opcode == 0x5000 || pck.Opcode == 0x2001)
                        {

                            continue;
                        }

                        PacketProcessResult pckRes = m_pck_processor.ProcessClient(pck, this);
  
                        bool doBreakLoop = false;

                        switch (pckRes)
                        {
                            case PacketProcessResult.ContinueLoop:
                                {
                                    continue;
                                }
                            case PacketProcessResult.BreakLoop:
                                {
                                    doBreakLoop = true;
                                }
                                break;
                            case PacketProcessResult.Disconnect:
                                {
                                    //Console.WriteLine("Stop called from Disconnect");
                                    Stop();
                                    return;
                                }
                            case PacketProcessResult.DoNothing:
                                {
                                    m_module_sec.Send(pck);
                                    continue;
                                }
                                
                            default:
                                {
                                    //PacketProcessResult.Unknown
                                }
                                break;
                        }

                        if (doBreakLoop)
                            break;
                    }
                }


                TransferModule();
                DoRecvFromClient();
            }
            catch
            {
                Stop();
            }
        }





        public void SendClientNotice(string str)
        {
            Packet err = new Packet(0x300C);
            err.WriteUInt16(3100);
            err.WriteUInt8(1);
            err.WriteAscii(str);
            SendPacketToClient(err);
            //Инфа с боку
            Packet err_2 = new Packet(0x300C);
            err_2.WriteUInt16(3100);
            err_2.WriteUInt8(2);
            err_2.WriteAscii(str);
            SendPacketToClient(err_2);
        }


        public void SendClientPM(string str)
        {
            Packet send = new Packet(0x3026);
            send.WriteUInt8(2);
            send.WriteAscii("[Сервер] Sairos",Global.TextEncodeCode);
            send.WriteAscii(str, Global.TextEncodeCode);
            SendPacketToClient(send);
        }

        //-----------------------------------------------------------------------------

        void DoRecvFromModule()
        {
            try
            {
                m_module_sock.BeginReceive(m_module_buffer, 0, m_module_buffer.Length, SocketFlags.None, new AsyncCallback(DoRecvFromModuleCallback), null);
            }
            catch (Exception)
            {
                Stop();
            }
        }

        //-----------------------------------------------------------------------------

        //Хэндлим пакеты от модуля к клиенту
        void DoRecvFromModuleCallback(IAsyncResult iar)
        {
            try
            {
                int nRecv = m_module_sock.EndReceive(iar);

                if (nRecv == 0)
                {
                    Stop();
                    return;
                }


                //m_State.RegisterRecvTraffic(nRecv);

                Global.TotalBytesIn += (ulong)(nRecv);

                m_module_sec.Recv(m_module_buffer, 0, nRecv);

                List<Packet> ModulePackets = m_module_sec.TransferIncoming();
                if (ModulePackets != null)
                {

                    for (int i = 0; i < ModulePackets.Count; i++)
                    {
                        var pck = ModulePackets[i];

                        
                        //Даём SSA хэндлить внутренние пакеты
                        if (pck.Opcode == 0x9000 || pck.Opcode == 0x5000)
                        {
                            continue;
                        }


                        PacketProcessResult pckRes = m_pck_processor.ProcessModule(pck, this);
                        //pckRes = PacketProcessResult.DoNothing;
                        bool doBreakLoop = false;

                        switch (pckRes)
                        {
                            case PacketProcessResult.ContinueLoop:
                                {
                                    continue;
                                }
                            case PacketProcessResult.BreakLoop:
                                {
                                    doBreakLoop = true;
                                }
                                break;
                            case PacketProcessResult.Disconnect:
                                {
                                    Stop();
                                   // Console.WriteLine("Stop called from Disconnect");
                                    return;
                                }
                            case PacketProcessResult.DoNothing:
                                {
                                    m_client_sec.Send(pck);
                                    continue;
                                }
                            default:
                                {
                                    //PacketProcessResult.Unknown
                                }
                                break;
                        }

                        if (doBreakLoop)
                            break;
                    }

                    ModulePackets.Clear();
                }

                TransferClient();
                DoRecvFromModule();
            }
            catch
            {
                Stop();
            }
        }

        //-----------------------------------------------------------------------------

        void DoSendToClient(byte[] buf, int len)
        {
            try
            {
                m_args.ClientSocket.BeginSend(buf, 0, len, SocketFlags.None, new AsyncCallback(DoSendToClientCallback), null);
            }
            catch
            {
                Stop();
            }
        }

        //-----------------------------------------------------------------------------

        void DoSendToClientCallback(IAsyncResult iar)
        {
            try
            {
                int nSent = m_args.ClientSocket.EndSend(iar);
                Global.TotalBytesOut += (ulong)(nSent);

                // m_State.RegisterSentTraffic(nSent);
            }
            catch
            {
                Stop();
            }
        }

        //-----------------------------------------------------------------------------

        void DoSendToModule(byte[] buf, int len)
        {
            try
            {
                m_module_sock.BeginSend(buf, 0, len, SocketFlags.None, new AsyncCallback(DoSendToModuleCallback), null);
            }
            catch
            {
                Stop();
            }
        }


        //-----------------------------------------------------------------------------

        void DoSendToModuleCallback(IAsyncResult iar)
        {
            try
            {
                int nSent = m_module_sock.EndSend(iar);
                Global.TotalBytesOut += (ulong)(nSent);
                //m_State.RegisterSentTraffic(nSent);
            }
            catch
            {
                Stop();
            }
        }

        //-----------------------------------------------------------------------------

        public void Stop(bool DoDestroyInvoke = true)
        {
            /*
            if (SocketsRunning)
                return;
            */

            //Required to prevent errors on ]high server load
            lock (m_io_lock)
            {
                //...
                if (!m_is_stopped)
                {


                    if (DoDestroyInvoke)
                        m_args.eOnDestroy.Invoke(this);





                    try
                    {
                        m_args.ClientSocket.Close();
                        m_args.ClientSocket.Shutdown(SocketShutdown.Both);
   
                    }
                    catch
                    {
                    }
                    finally
                    {
                        m_args.ClientSocket = null;
                    }


                    try
                    {
                        m_module_sock.Close();
                        m_module_sock.Shutdown(SocketShutdown.Both);
                    }
                    catch
                    {
                    }
                    finally
                    {
                        m_module_sock = null;
                    }



                    try
                    {
                        m_client_buffer = null;
                    }
                    catch { }

                    try
                    {
                        m_module_buffer = null;
                    }
                    catch { }
                    m_is_stopped = true;
                }
            }
        }

        //-----------------------------------------------------------------------------


        bool SocketConnected(Socket s)
        {
            bool res = true;

            bool part1 = false, part2 = false;
            try
            {
                part1 = s.Poll(0, SelectMode.SelectRead);
                part2 = (s.Available == 0);
            }
            catch (Exception)
            {
                res = false;
            }

            if ((part1 && part2))
            {
                res = false;
            }
            else
                res = true;

            return res;
        }

        //-----------------------------------------------------------------------------

        public void SendPacketToClient(Packet pck)
        {
            m_client_sec.Send(pck);
            TransferClient();
        }

        public void SendPacketToModule(Packet pck)
        {
            m_module_sec.Send(pck);
            TransferModule();
        }

        //-----------------------------------------------------------------------------


    }
}
