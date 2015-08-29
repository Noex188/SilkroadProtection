using System;
using System.Timers;
using System.Collections.Generic;

using sroprot.NetEngine.AgentServer;
using SilkroadSecurityApi;

namespace sroprot.NetEngine
{
    public enum DelayedPacketProcessor
    {
        AutoNotice
    }

    /// <summary>
    /// Currently supports only autonotice
    /// </summary>
    public class DelayedPacketDispatcher
    {
        public delegate void DelayedPacketHandler(SilkroadServer srv, string noticeText);
        readonly object m_class_lock;

        class PacketTimer : Timer
        {
            public SilkroadServer Server;
            DelayedPacketProcessor HandlerType;
            public object StateObj;

            public PacketTimer(DelayedPacketProcessor handlerType, SilkroadServer server, int delay, object stateObj)
            {
                this.HandlerType = handlerType;
                this.Server = server;
                this.Interval = delay;
                this.Enabled = true;
                this.StateObj = stateObj;
                this.Elapsed += new ElapsedEventHandler(OnTimerElapsed);
            }

            void OnTimerElapsed(object sender, ElapsedEventArgs e)
            {
                switch (this.HandlerType)
                {
                    case DelayedPacketProcessor.AutoNotice:
                        {
                            AutoNotice.HandleDelayedPacket(this.Server, this.StateObj as string);
                        }
                        break;
                }
            }
            
            
        }

        List<PacketTimer> m_client_timers;
        List<PacketTimer> m_module_timers;

        public DelayedPacketDispatcher()
        {
            m_class_lock = new object();
            m_client_timers = new List<PacketTimer>();
            m_module_timers = new List<PacketTimer>();
        }
        

       

        public void RegisterClientMsg(DelayedPacketProcessor handlerType, SilkroadServer server, int delay, string noticeMsg)
        {
            lock (m_class_lock)
            {
                m_client_timers.Add(new PacketTimer(handlerType, server, delay, noticeMsg));
            }
        }

        public void RegisterModuleMsg(DelayedPacketProcessor handlerType, SilkroadServer server, int delay, string noticeMsg)
        {
            lock (m_class_lock)
            {
                m_module_timers.Add(new PacketTimer(handlerType, server, delay, noticeMsg));
            }
        }

        public void Start()
        {
            lock (m_class_lock)
            {
                foreach (var item in m_client_timers)
                {
                    item.Start();
                }

                foreach (var item in m_module_timers)
                {
                    item.Start();
                }
            }
        }

        public void Stop()
        {
            lock (m_class_lock)
            {
                foreach (var item in m_client_timers)
                {
                    item.Stop();
                }

                foreach (var item in m_module_timers)
                {
                    item.Stop();
                }
            }
        }

        public void Clear()
        {
            lock (m_class_lock)
            {
                m_client_timers.Clear();
                m_module_timers.Clear();
            }
        }
    }
}
