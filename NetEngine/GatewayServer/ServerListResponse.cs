using System;
using SilkroadSecurityApi;

namespace sroprot.NetEngine.GatewayServer
{
    class ServerListResponse
    {
        public static PacketProcessResult HandleModule(Packet pck, RelaySession session, SilkroadServer server)
        {
            Packet ServerList = new Packet(0xA101);
            byte GlobalOperationFlag = pck.ReadUInt8();

            ServerList.WriteUInt8(GlobalOperationFlag);

            while (GlobalOperationFlag == 1)
            {
                byte GlobalOperationType = pck.ReadUInt8();
                string GlobalOperationName = pck.ReadAscii();

                GlobalOperationFlag = pck.ReadUInt8();
                ServerList.WriteUInt8(GlobalOperationType);
                ServerList.WriteAscii(GlobalOperationName);
                ServerList.WriteUInt8(GlobalOperationFlag);
            }
            byte ShardFlag = pck.ReadUInt8();

            ServerList.WriteUInt8(ShardFlag);

            while (ShardFlag == 1)
            {
                uint ShardID = pck.ReadUInt16();
                string ShardName = pck.ReadAscii();
                uint ShardCurrent = pck.ReadUInt16();
                uint ShardCapacity = pck.ReadUInt16();
                byte ShardStatus = pck.ReadUInt8();
                byte GlobalOperationID = pck.ReadUInt8();
                ShardFlag = pck.ReadUInt8();
                session.State["server_" + ShardID] = ShardCurrent;

                ServerList.WriteUInt16(ShardID);
                ServerList.WriteAscii("Возрождение", Global.TextEncodeCode);
                ServerList.WriteUInt16(ShardCurrent + Global.ShardFakeOnline);
                ServerList.WriteUInt16(Global.ShardMaxOnline > 0 ? (uint)Global.ShardMaxOnline : ShardCapacity);
                ServerList.WriteUInt8(ShardStatus);
                ServerList.WriteUInt8(GlobalOperationID);
                ServerList.WriteUInt8(ShardFlag);

            }
            session.SendPacketToClient(ServerList);
            return PacketProcessResult.ContinueLoop;
        }
    }
}
