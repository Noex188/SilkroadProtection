using System;
using SilkroadSecurityApi;


namespace sroprot.NetEngine.AgentServer
{
    class GmAccessControl
    {
        public static PacketProcessResult HandleClient(Packet pck, RelaySession session, SilkroadServer server)
        {
            if (Global.EnableGmAccessControl)
            {
                //Делаем запрос в базу и проверяем разрешеная ли команда
                uint commandID = pck.ReadUInt16();

                string uname = session.State["username"] as string;
                if (Global.dbmgr.checkGmAccessControl(uname, commandID) == 1)
                {

                    if (commandID == 6 || commandID == 7)
                    {
                        //Делаем проверку на разрешение использовать этот ID
                        uint obj_id = pck.ReadUInt32();
                        uint amountOrOptLevel = pck.ReadUInt8();

                        if (Global.dbmgr.checkGmObjAccessControl(uname, obj_id, amountOrOptLevel) == 0)
                        {
                            session.SendClientNotice("UIIT_STT_ANTICHEAT_GM_USE_CREATE");
                            return PacketProcessResult.ContinueLoop;

                        }
                    }
                    //Убираем возможность полноценно килять мобов гму.
                    if (commandID == 20)
                    {
                        uint k_mob_id = pck.ReadUInt32();
                        Packet p = new Packet(0x7010);
                        p.WriteUInt16(20);
                        p.WriteUInt32(k_mob_id);
                        p.WriteUInt8(1);
                        session.SendPacketToModule(p);
                        return PacketProcessResult.ContinueLoop;
                    }
                }
                else
                {
                    session.SendClientNotice("UIIT_STT_ANTICHEAT_GM_USE_COMMAND");
                    return PacketProcessResult.ContinueLoop;
                }
            }

            return PacketProcessResult.DoNothing;
        }
    }
}
