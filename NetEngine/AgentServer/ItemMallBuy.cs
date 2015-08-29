using System;
using System.Collections.Generic;
using SilkroadSecurityApi;

namespace sroprot.NetEngine.AgentServer
{
    class ItemMallBuy
    {
        public static PacketProcessResult HandleClient(Packet pck, RelaySession session, SilkroadServer server)
        {
            if (Global.EnableItemMallBuyFix)
            {
                string uname = session.State["username"] as string;

                if (uname.Length == 0)
                {
                    Global.logmgr.WriteLog(LogLevel.Error, "username len == 0 (request buy mall item)");
                    return PacketProcessResult.Disconnect;

                }

                byte ShopType = pck.ReadUInt8();

                if (ShopType == 24)
                {
                    //------------------------------------------
                    UInt16 uint1 = pck.ReadUInt16();
                    byte b1 = pck.ReadUInt8();
                    byte b2 = pck.ReadUInt8();
                    byte b3 = pck.ReadUInt8();
                    string package_item_codename = pck.ReadAscii();
                    ushort nItems = pck.ReadUInt16();
                    pck.ReadUInt64();
                    uint refpackage_id = pck.ReadUInt32();
                    //------------------------------------------

                    string charname = session.State["charname"] as string;

                    if (charname.Length == 0)
                    {
                        Global.logmgr.WriteLog(LogLevel.Error, "charname len == 0 (buy in item mall)");
                        return PacketProcessResult.Disconnect;
                    }

                    int res = Global.dbmgr.GetBuyMallResult(charname, package_item_codename, nItems);


                    if (res == -1)
                    {
                        //------------------------------------------
                        session.SendClientNotice("UIIT_STT_ANTICHEAT_ITEM_MALL_ERROR");
                        //------------------------------------------
                    }
                    else
                    {
                        //update silk
                        List<int> silk_info = Global.dbmgr.GetSilkDataByUsername(uname);


                        //------------------------------------------
                        Packet resp = new Packet(0x3153);

                        resp.WriteUInt32(silk_info[0]);
                        resp.WriteUInt32(silk_info[1]);
                        resp.WriteUInt32(silk_info[2]);
                        // m_ClientSecurity.Send(resp);
                        session.SendPacketToClient(resp);

                        //------------------------------------------
                        Packet inventory = new Packet(0xB034);
                        inventory.WriteUInt8(1);
                        inventory.WriteUInt8(24);
                        inventory.WriteUInt16(uint1);
                        inventory.WriteUInt8(b1);
                        inventory.WriteUInt8(b2);
                        inventory.WriteUInt8(b3);
                        inventory.WriteUInt8(1);
                        inventory.WriteUInt8(res);
                        inventory.WriteUInt16(nItems);
                        inventory.WriteUInt32(0);
                        //m_ClientSecurity.Send(inventory);
                        session.SendPacketToClient(inventory);
                        //------------------------------------------

                        Packet inventory2 = new Packet(0xB034);
                        inventory2.WriteUInt8(1);
                        inventory2.WriteUInt8(7);
                        inventory2.WriteUInt8((byte)(res));
                        //  m_ClientSecurity.Send(inventory2);
                        session.SendPacketToClient(inventory2);

                        //------------------------------------------

                        //  Global.g_LogManager.WriteLog(LogLevel.Notify, "GetBuyMallResult OK slot {0}", res);
                    }
                    return PacketProcessResult.ContinueLoop;
                }
            }

            return PacketProcessResult.DoNothing;
        }
    }
}
