using System;
using SilkroadSecurityApi;


namespace sroprot.NetEngine.AgentServer
{
    class StaticAnnounce
    {
        public static PacketProcessResult HandleModule(Packet pck, RelaySession session, SilkroadServer server)
        {
            uint type = pck.ReadUInt16();
            if (Global.EnableUniqueDeathNotify && type == 3078)
            {
                uint mob_id = pck.ReadUInt32();
                string char_name = pck.ReadAscii();
                //Проверяем тормоз, дабы с каждого потока не спамило
                if(Global.UniqueDeathNotifyName == char_name && Global.UniqueDeathNotifyID == mob_id && Global.UniqueDeathNotifyTime > DateTime.Now)
                {
                    return PacketProcessResult.DoNothing; 
                }

                Global.UniqueDeathNotifyName = char_name;
                Global.UniqueDeathNotifyID = mob_id;
                Global.UniqueDeathNotifyTime = DateTime.Now.AddSeconds(10);
                Global.dbmgr.UniqueDeathNotify(char_name, (int)mob_id);
            }

            return PacketProcessResult.DoNothing;
        }
    }
}
