using System;
using SilkroadSecurityApi;

namespace sroprot.NetEngine.GatewayServer
{
    class DownloadRedirect
    {
        /// <summary>
        /// DownloadServer info / patch info request packet (Module -> Client)
        /// </summary>
        /// <param name="pck"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public static PacketProcessResult HandleModule(Packet pck, RelaySession session, SilkroadServer server)
        {
            if (server.HasDownloadRedirectRules())
            {
                //Read original data
                byte result = pck.ReadUInt8();

                //UpdateInfo
                if (result == 0x02)
                {
                    byte errorCode = pck.ReadUInt8();

                    if (errorCode == 0x02)
                    {
                        Packet myPacket = new Packet(0xA100, false, true);

                        string tmpIp = pck.ReadAscii();
                        ushort tmpPort = pck.ReadUInt16();

                        bool redirectRuleFound = false;
                        for (int i = 0; i < server.RedirectRules.Count; i++)
                        {
                            if (server.RedirectRules[i].OriginalIp == tmpIp && server.RedirectRules[i].OriginalPort == tmpPort)
                            {
                                tmpIp = server.RedirectRules[i].NewIp;
                                tmpPort = (ushort)server.RedirectRules[i].NewPort;
                                redirectRuleFound = true;
                            }
                        }

                        if (!redirectRuleFound)
                        {
                            Global.logmgr.WriteLog(LogLevel.Warning, "Download redirect rules given, but [{0}:{1}] is unknown download server", tmpIp, tmpPort);
                        }

                        uint version = pck.ReadUInt32();
                        byte fileFlag = pck.ReadUInt8();

                        myPacket.WriteUInt8(result);
                        myPacket.WriteUInt8(errorCode);

                        myPacket.WriteAscii(tmpIp);
                        myPacket.WriteUInt16(tmpPort);
                        myPacket.WriteUInt32(version);
                        myPacket.WriteUInt8(fileFlag);

                        while (fileFlag == 0x01)
                        {
                            uint fileId = pck.ReadUInt32();
                            string fileName = pck.ReadAscii();
                            string filePath = pck.ReadAscii();
                            uint fileLength = pck.ReadUInt32();
                            byte doPack = pck.ReadUInt8();

                            fileFlag = pck.ReadUInt8();

                            myPacket.WriteUInt32(fileId);
                            myPacket.WriteAscii(fileName);
                            myPacket.WriteAscii(filePath);
                            myPacket.WriteUInt32(fileLength);
                            myPacket.WriteUInt8(doPack);
                            myPacket.WriteUInt8(fileFlag);
                        }
                        session.SendPacketToClient(myPacket);
                    }
                    //Do not send original packet
                    return PacketProcessResult.ContinueLoop;
                }
            }
            return PacketProcessResult.DoNothing;
        }
    }
}
