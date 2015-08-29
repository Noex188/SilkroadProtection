using System;
using SilkroadSecurityApi;

namespace sroprot.NetEngine.GatewayServer
{
    class AgentRedirect
    {
        public static PacketProcessResult HandleModule(Packet pck, RelaySession session, SilkroadServer server)
        {
            if (server.HasAgentRedirectRules())
            {
                string src_host;
                int src_port;
                byte flag1 = pck.ReadUInt8();


                if (flag1 == 1)
                {
                    UInt32 uint1 = pck.ReadUInt32();
                    src_host = pck.ReadAscii();
                    src_port = pck.ReadUInt16();

                    bool redirectRuleFound = false;
                    for (int j = 0; j < server.RedirectRules.Count; j++)
                    {

                        if (server.RedirectRules[j].OriginalIp == src_host && server.RedirectRules[j].OriginalPort == src_port)
                        {
                            Packet mypck = new Packet(0xA102, false);
                            mypck.WriteUInt8(flag1);
                            mypck.WriteUInt32(uint1);
                            mypck.WriteAscii(server.RedirectRules[j].NewIp);
                            mypck.WriteUInt16((ushort)server.RedirectRules[j].NewPort);

                            //  m_ClientSecurity.Send(mypck);
                            session.SendPacketToClient(mypck);
                            redirectRuleFound = true;

                            break;


                        }


                    }
                    if (!redirectRuleFound)
                    {
                        Global.logmgr.WriteLog(LogLevel.Warning, "Agent redirect rules given, but [{0}:{1}] is unknown agent server", src_host, src_port);
                    }
                    else
                    {
                        return PacketProcessResult.ContinueLoop;
                    }

                }

            }
            return PacketProcessResult.DoNothing;
        }
    }
}
