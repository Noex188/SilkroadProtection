using System;
using System.Collections;
using System.Collections.Generic;


using SilkroadSecurityApi;

namespace sroprot.NetEngine
{
    public enum PacketProcessResult
    {
        ContinueLoop,
        BreakLoop,
        Disconnect,
        DoNothing,
        Unknown
    }

    public delegate PacketProcessResult PacketHandler(Packet pck, RelaySession session, SilkroadServer server);

    /// <summary>
    /// One PacketProcessor instance has to be created for each running server
    /// </summary>
    public class PacketDispatcher
    {
        Hashtable m_client_handlers;
        public int ClientHandlerCount { get { return m_client_handlers.Count; } }

        Hashtable m_module_handlers;
        public int ModuleHandlerCount { get { return m_module_handlers.Count; } }

        List<ushort> m_client_debug_opcodes;
        public int ClientDebugOpcodeCount { get { return m_client_debug_opcodes.Count;  } }

        List<ushort> m_module_debug_opcodes;
        public int ModuleDebugOpcodeCount { get { return m_module_debug_opcodes.Count; } }

        Hashtable m_client_filter_handlers;
        public int ClientFilterMsgCount { get { return m_client_filter_handlers.Count; } }

        Hashtable m_module_filter_handlers;
        public int ModuleFilterMsgCount { get { return m_module_filter_handlers.Count; } }

        public int FilterMsgCount { get { return m_client_filter_handlers.Count + m_module_filter_handlers.Count;  } }

        bool m_log_all_client_pck;
        public bool LogAllClientPackets
        {
            get { return m_log_all_client_pck; }
            set { m_log_all_client_pck = value;  }
        }

        bool m_log_all_module_pck;
        public bool LogAllModulePackets
        {
            get { return m_log_all_module_pck; }
            set { m_log_all_module_pck = value; }
        }

        public int HandlerCount { get { return m_client_handlers.Count + m_module_handlers.Count; } }

        
        SilkroadServer m_server;

        public PacketDispatcher()
        {
            m_client_handlers = new Hashtable();
            m_module_handlers = new Hashtable();
            m_client_debug_opcodes = new List<ushort>();
            m_module_debug_opcodes = new List<ushort>();
            m_log_all_client_pck = false;
            m_log_all_module_pck = false;

            m_client_filter_handlers = new Hashtable();
            m_module_filter_handlers = new Hashtable();
        }

        public void AssignServer(SilkroadServer server)
        {
            m_server = server;
        }

        public bool RegisterClientDebugMsg(ushort opcode)
        {
            if(m_client_debug_opcodes.Contains(opcode))
                return false;

            m_client_debug_opcodes.Add(opcode);
            return true;
        }

        public bool RegisterModuleDebugMsg(ushort opcode)
        {
            if (m_module_debug_opcodes.Contains(opcode))
                return false;

            m_module_debug_opcodes.Add(opcode);
            return true;
        }

        public bool RegisterClientFilterMsg(ushort opcode, PacketHandler handler)
        {
            if (m_client_filter_handlers.ContainsKey(opcode))
                return false;

            m_client_filter_handlers.Add(opcode, handler);
            return true;
        }
        
        public bool RegisterClientFilterMsg(ushort opcode)
        {
            return RegisterClientFilterMsg(opcode, null);
        }

        public bool RegisterModuleFilterMsg(ushort opcode, PacketHandler handler)
        {
            if (m_module_filter_handlers.ContainsKey(opcode))
                return false;

            m_module_filter_handlers.Add(opcode, handler);
            return true;
        }

        public bool RegisterModuleFilterMsg(ushort opcode)
        {
            return RegisterModuleFilterMsg(opcode, null);
        }

        public bool RegisterClientMsg(ushort opcode, PacketHandler handler)
        {
            if (m_client_handlers.ContainsKey(opcode))
                return false;

            m_client_handlers.Add(opcode, handler);
            return true;
        }

        public bool RegisterModuleMsg(ushort opcode, PacketHandler handler)
        {
            if (m_module_handlers.ContainsKey(opcode))
                return false;

            m_module_handlers.Add(opcode, handler);
            return true;
        }


        public PacketProcessResult ProcessClient(Packet pck, RelaySession session)
        {
            if(m_client_filter_handlers.ContainsKey(pck.Opcode))
            {
                PacketHandler handler = m_client_filter_handlers[pck.Opcode] as PacketHandler;
                if (handler != null)
                {
                    return handler(pck, session, m_server);
                }
                else
                {
                    if (Global.EnableBanExploitAbuser)
                    {
                        Global.BlockedIpAddresses.Add(Utility.GetRemoteEpString(session.Arguments.ClientSocket));
                    }

                    return PacketProcessResult.Disconnect;
                }
            }

            if(m_client_debug_opcodes.Contains(pck.Opcode) || m_log_all_client_pck)
            {
                //Debug log
                Global.logmgr.WritePacketLog(pck, PacketDirection.ClientToModule, session.State);
            }

            if (m_client_handlers.ContainsKey(pck.Opcode))
            {
                PacketHandler handler = m_client_handlers[pck.Opcode] as PacketHandler;
                if (m_server == null)
                {
                    throw new Exception("PacketDispatcher::ProcessClient m_server not set");
                }
                return handler(pck, session, m_server);
            }
            //We simply ignore packet and continue looping if handler doesent do something else
            return PacketProcessResult.DoNothing;
        }

        public PacketProcessResult ProcessModule(Packet pck, RelaySession session)
        {
            if (m_module_filter_handlers.ContainsKey(pck.Opcode))
            {
                PacketHandler handler = m_module_filter_handlers[pck.Opcode] as PacketHandler;
                if (handler != null)
                {
                    return handler(pck, session, m_server);
                }
                else
                {
                    if (Global.EnableBanExploitAbuser)
                    {
                        Global.BlockedIpAddresses.Add(Utility.GetRemoteEpString(session.Arguments.ClientSocket));
                    }

                    return PacketProcessResult.Disconnect;
                }
            }




            if(m_module_debug_opcodes.Contains(pck.Opcode) || m_log_all_client_pck)
            {
                //Debug log
                Global.logmgr.WritePacketLog(pck, PacketDirection.ModuleToClient, session.State);
            }

            if (m_module_handlers.ContainsKey(pck.Opcode))
            {
                PacketHandler handler = m_module_handlers[pck.Opcode] as PacketHandler;
                if (m_server == null)
                {
                    throw new Exception("PacketDispatcher::ProcessModule m_server not set");
                }
                return handler(pck, session, m_server);

            }
            //We simply ignore packet and continue looping if handler doesent do something else
            return PacketProcessResult.DoNothing;
        }
    }
}
