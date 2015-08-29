using System;
using SilkroadSecurityApi;


namespace sroprot.NetEngine.AgentServer
{
    class SafeRegion
    {
        public static PacketProcessResult HandleModule(Packet pck, RelaySession session, SilkroadServer server)
        {
            bool isBot = false;
            bool.TryParse(session.State["isBot"] as string, out isBot);
            if (Global.UseSafeRegion && isBot)
            {
                pck.ReadUInt32();
                pck.ReadUInt8();
                int region = pck.ReadUInt16();

                if (Global.SafeRegions.Contains(region))
                {
                    session.State["isSafe"] = true;
                }
                else
                {
                    session.State["isSafe"] = false;
                }
            }

            return PacketProcessResult.DoNothing;
        }
    }
}
