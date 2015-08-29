using System;
using SilkroadSecurityApi;

namespace sroprot.NetEngine.AgentServer
{
    class ArenaNotify
    {
        public static PacketProcessResult HandleModule(Packet pck, RelaySession session, SilkroadServer server)
        {
            if (Global.EnableArenaStatusNotify)
            {
                if (pck.ReadInt8() == 9)
                {
                    pck.ReadInt8();
                    int status = pck.ReadInt8();
                    string cname = session.State["charname"] as string;

                    //передаем в базу данные о статусе боя
                    Global.dbmgr.AnticheatArenaStatusNotify(cname, status);
                    if (status == 1)
                    {
                        //Вам бЫли начислены очки славы textu'system lel ye perfect.
                        session.SendClientNotice("UIIT_STT_ANTICHEAT_ADDED_HONOR_POINT");
                    }
                    else
                    {
                        //Вы потеряли очки славы
                        session.SendClientNotice("UIIT_STT_ANTICHEAT_MISSING_HONOR_POINT");
                    }

                }
            }

            return PacketProcessResult.DoNothing;
        }
    }
}
