using System;
using System.Linq;
using SilkroadSecurityApi;

namespace sroprot.Core
{
    public sealed class CommandManager
    {
        //-----------------------------------------------------------------------------

        static CommandManager m_instance = null;

        private CommandManager() { }

        public static CommandManager getInstance()
        {
            if (m_instance == null)
            {
                m_instance = new CommandManager();
            }
            return m_instance;
        }

        //-----------------------------------------------------------------------------

        public void PrintCommandList()
        {
            Console.WriteLine("/gc - force GC.Collect");
            Console.WriteLine("/getblocked - get blocked ip addresses");
            Console.WriteLine("/clearblocked - unblock all currently blocked ip addresses");
            Console.WriteLine("/uptime - get sroprot uptime");
            Console.WriteLine("/userlist - get all connected users");
            Console.WriteLine("/stopcontexts - kill all current user connections");
            Console.WriteLine("/usernotice arg1 arg2 - send notice message to specific user (arg1 - username, arg2 - message)");
            Console.WriteLine("/charnotice arg1 arg2 - send notice message to specific char (arg1 - charname, arg2 - message)");
            Console.WriteLine("/clear - Clear console");
            Console.WriteLine("/updateconfig arg1 - Обновить настройки (arg1 - название конифга)");
            Console.WriteLine("/findUserById arg1 - найти пользователя по логину (arg1 - логин)");
            Console.WriteLine("/findUserByIp arg1 - найти пользователя по IP (arg1 - IP)");
            Console.WriteLine("/findUserByName arg1 - найти пользователя по имени чара (arg1 - имя чара)");
            Console.WriteLine("/printNormalUserList - список аккаунтов без бота");
            Console.WriteLine("/printBotlUserList - список аккаунтов на ботах");
            
        }
        
        //-----------------------------------------------------------------------------

        public void PrintBlockedUsers()
        {
            int n = 0;
            foreach (string blocked_ip in Global.BlockedIpAddresses)
            {
                Console.WriteLine(blocked_ip);
                n++;
            }
            Console.WriteLine("Total blocked ip count: {0}", n);
        }

        public void ClearBlockedUsers()
        {
            Global.BlockedIpAddresses.Clear();
            Console.WriteLine("Blocked IP Addresses cleared");
        }

        //-----------------------------------------------------------------------------

        public void PrintUserList()
        {
            try
            {
                var states = Global.srvmgr.GetAllContextStates();

                if (states.Count > 0)
                {

                    for (int i = 0; i < states.Count; i++)
                    {
                        //id username charname ip
                        string uname = states[i]["username"] as string;
                        string cname = states[i]["charname"] as string;
                        string ipaddr = states[i]["ip_address"] as string;

                        bool isBot = false;
                        bool.TryParse(states[i]["isBot"] as string, out isBot);

                        Console.WriteLine("{0} -> [{1}] [{2}] [Player status: {3}] [{4}]",
                            i + 1,
                            uname,
                            cname,
                            (isBot) ? "Bot" : "Normal",
                            ipaddr);


                        if (i + 1 % 50 == 0)
                        {
                            Console.WriteLine("Press any key to show more");
                            Console.ReadKey();
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No connected users found");
                }
            }
            catch (Exception e)
            {

                Global.logmgr.WriteLog(LogLevel.Error, "Ошибака обращения к команде [/userlist]");
            }

        }


        public void findUserById(string s)
        {
            try
            {
                string[] arg = s.Split(new char[] { ' ' });
                if (arg.Count() == 2)
                {
                    var states = Global.srvmgr.GetAllContextStates();

                    if (states.Count > 0)
                    {
                        bool found = false;
                        for (int i = 0; i < states.Count; i++)
                        {
                            if (states[i]["username"] as string == arg[1])
                            {
                                found = true;
                                //id username charname ip
                                string uname = states[i]["username"] as string;
                                string cname = states[i]["charname"] as string;
                                string ipaddr = states[i]["ip_address"] as string;

                                bool isBot = false;
                                bool.TryParse(states[i]["isBot"] as string, out isBot);

                                Console.WriteLine("{0} -> [{1}] [{2}] [Player status: {3}] [{4}]",
                                    i + 1,
                                    uname,
                                    cname,
                                    (isBot) ? "Bot" : "Normal",
                                    ipaddr);

                            }

                            if (!found)
                            {
                                Global.logmgr.WriteLog(LogLevel.Notify, "User {0} not found", arg[1]);
                            }

                            if (i + 1 % 50 == 0)
                            {
                                Console.WriteLine("Press any key to show more");
                                Console.ReadKey();
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("No connected users found");

                    }
                }
                else
                {
                    Global.logmgr.WriteLog(LogLevel.Error, "Argument Error");
                }
            }
            catch (Exception e)
            {

                Global.logmgr.WriteLog(LogLevel.Error, "Ошибака обращения к команде [/findUserByid]");
            }
        }


        public void findUserByIp(string s)
        {
            try
            {
                string[] arg = s.Split(new char[] { ' ' });
                if (arg.Count() == 2)
                {
                    var states = Global.srvmgr.GetAllContextStates();

                    if (states.Count > 0)
                    {
                        bool found = false;
                        for (int i = 0; i < states.Count; i++)
                        {
                            if (states[i]["ip_address"] as string == arg[1])
                            {
                                found = true;
                                //id username charname ip
                                //id username charname ip
                                string uname = states[i]["username"] as string;
                                string cname = states[i]["charname"] as string;
                                string ipaddr = states[i]["ip_address"] as string;

                                bool isBot = false;
                                bool.TryParse(states[i]["isBot"] as string, out isBot);

                                Console.WriteLine("{0} -> [{1}] [{2}] [Player status: {3}] [{4}]",
                                    i + 1,
                                    uname,
                                    cname,
                                    (isBot) ? "Bot" : "Normal",
                                    ipaddr);


                            }
                            if (!found)
                            {
                                Global.logmgr.WriteLog(LogLevel.Notify, "user with IP {0} not found", arg[1]);
                            }

                            if (i + 1 % 50 == 0)
                            {
                                Console.WriteLine("Press any key to show more");
                                Console.ReadKey();
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("No connected users found");

                    }
                }
                else
                {
                    Global.logmgr.WriteLog(LogLevel.Error, "Argument Error");
                }
            }
            catch (Exception e)
            {

                Global.logmgr.WriteLog(LogLevel.Error, "Ошибака обращения к команде [/findUserByIp]");
            }
        }

        public void findUserByName(string s)
        {
            try
            {
                string[] arg = s.Split(new char[] { ' ' });
                if (arg.Count() == 2)
                {
                    var states = Global.srvmgr.GetAllContextStates();

                    if (states.Count > 0)
                    {
                        bool found = false;
                        for (int i = 0; i < states.Count; i++)
                        {
                            if (states[i]["charname"] as string == arg[1])
                            {
                                found = true;
                                //id username charname ip
                                string uname = states[i]["username"] as string;
                                string cname = states[i]["charname"] as string;
                                string ipaddr = states[i]["ip_address"] as string;

                                bool isBot = false;
                                bool.TryParse(states[i]["isBot"] as string, out isBot);

                                Console.WriteLine("{0} -> [{1}] [{2}] [Player status: {3}] [{4}]",
                                    i + 1,
                                    uname,
                                    cname,
                                    (isBot) ? "Bot" : "Normal",
                                    ipaddr);


                            }
                            if (!found)
                            {
                                Global.logmgr.WriteLog(LogLevel.Notify, "user with name {0} not found", arg[1]);
                            }

                            if (i + 1 % 50 == 0)
                            {
                                Console.WriteLine("Press any key to show more");
                                Console.ReadKey();
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("No connected users found");

                    }
                }
                else
                {
                    Global.logmgr.WriteLog(LogLevel.Error, "Argument Error");
                }
            }
            catch (Exception e)
            {

                Global.logmgr.WriteLog(LogLevel.Error, "Ошибака обращения к команде [/findUserByName]");
            }
        }


        public void printBotUserList()
        {
            try
            {
                    var states = Global.srvmgr.GetAllContextStates();
                    if (states.Count > 0)
                    {
                        bool found = false;
                        for (int i = 0; i < states.Count; i++)
                        {
                            bool isBot = false;
                            bool.TryParse(states[i]["isBot"] as string, out isBot);

                            if (isBot)
                            {
                                found = true;
                                //id username charname ip
                                //id username charname ip
                                string uname = states[i]["username"] as string;
                                string cname = states[i]["charname"] as string;
                                string ipaddr = states[i]["ip_address"] as string;

                               
                                Console.WriteLine("{0} -> [{1}] [{2}] [Player status: {3}] [{4}]",
                                    i + 1,
                                    uname,
                                    cname,
                                    (isBot) ? "Bot" : "Normal",
                                    ipaddr);



                            }
                            if (!found)
                            {
                                Global.logmgr.WriteLog(LogLevel.Notify, "bot not found");
                            }

                            if (i + 1 % 50 == 0)
                            {
                                Console.WriteLine("Press any key to show more");
                                Console.ReadKey();
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("No connected users found");

                    }
             
            }
            catch (Exception e)
            {

                Global.logmgr.WriteLog(LogLevel.Error, "Ошибака обращения к команде [/printBotList]");
            }
        }

        public void printNormalUserList()
        {
            try
            {
                var states = Global.srvmgr.GetAllContextStates();
                if (states.Count > 0)
                {
                    bool found = false;
                    for (int i = 0; i < states.Count; i++)
                    {

                        bool isBot = false;
                        bool.TryParse(states[i]["isBot"] as string, out isBot);

                        if (isBot)
                        {
                            found = true;
                            //id username charname ip
                            string uname = states[i]["username"] as string;
                            string cname = states[i]["charname"] as string;
                            string ipaddr = states[i]["ip_address"] as string;

                         

                            Console.WriteLine("{0} -> [{1}] [{2}] [Player status: {3}] [{4}]",
                                i + 1,
                                uname,
                                cname,
                                (isBot) ? "Bot" : "Normal",
                                ipaddr);



                        }
                        if (!found)
                        {
                            Global.logmgr.WriteLog(LogLevel.Notify, "Normal User not found");
                        }

                        if (i + 1 % 50 == 0)
                        {
                            Console.WriteLine("Press any key to show more");
                            Console.ReadKey();
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No connected users found");

                }

            }
            catch (Exception e)
            {

                Global.logmgr.WriteLog(LogLevel.Error, "Ошибака обращения к команде [/printNormalUserList]");
            }
        }


        //-----------------------------------------------------------------------------

        public void CollectGarbage()
        {
            GC.Collect();
            Console.WriteLine("Garbage collected");
        }

        //-----------------------------------------------------------------------------

        public void SendNoticeToUsername(string cmd_str)
        {
            string[] cmd_args = cmd_str.Split(new char[] { ' ' });
            string username = cmd_args[1];

            string notice_str = string.Empty;
            for (int i = 2; i < cmd_args.Length; i++)
            {
                notice_str += string.Format("{0} ", cmd_args[i]);
            }

            Packet pck = new Packet(0x3026);
            pck.WriteUInt8(7);
            pck.WriteAscii(notice_str);

            bool send_res = Global.srvmgr.SendPacketToClientUsername(pck, ServerType.AgentServer, username);

            if (send_res)
            {
                Console.WriteLine("Message successfully sent to user");
            }
            else
            {
                Console.WriteLine("User with such username is not found");
            }
        }

        public void UpdateConfig(string s)
        {
            bool isDone = false;
            string category = "anticheat";
            try
            {
                string[] config = s.Split(new char[] { ' ' });
                if (config.Count() == 2)
                {
                    switch (config[1])
                    {
                        case "enable_bot_detected":
                            Global.EnableBotDetected = bool.Parse(Global.cfgmgr.ReadValue(Global.ConfigFile, "anticheat", "enable_bot_detected"));
                            isDone = true;
                            category = "anticheat";
                            break;
                        case "enable_auto_capcha":
                            Global.EnableAutoCaptcha = bool.Parse(Global.cfgmgr.ReadValue(Global.ConfigFile, "anticheat", "enable_auto_capcha"));
                            isDone = true;
                            category = "anticheat";
                            break;
                        case "disable_bot_arena_registration":
                            Global.DisableBotArenaRegistration = bool.Parse(Global.cfgmgr.ReadValue(Global.ConfigFile, "anticheat", "disable_bot_arena_registration"));
                            isDone = true;
                            category = "anticheat";
                            break;
                        case "use_safe_region":
                            Global.UseSafeRegion = bool.Parse(Global.cfgmgr.ReadValue(Global.ConfigFile, "anticheat", "use_safe_region"));
                            isDone = true;
                            category = "anticheat";
                            break;
                        case "enable_exchange_cooldown":
                            Global.EnableExchangeCooldown = bool.Parse(Global.cfgmgr.ReadValue(Global.ConfigFile, "anticheat", "enable_exchange_cooldown"));
                            isDone = true;
                            category = "anticheat";
                            break;
                        case "enable_start_intro":
                            Global.EnableExchangeCooldown = bool.Parse(Global.cfgmgr.ReadValue(Global.ConfigFile, "game", "enable_start_intro"));
                            isDone = true;
                            category = "game";
                            break;
                        case "start_intro_script_name":
                            Global.StartIntroScriptName = Global.cfgmgr.ReadValue(Global.ConfigFile, "game", "enable_start_intro");
                            isDone = true;
                            category = "game";
                            break;
                        case "fix_water_tample_teleport":
                            Global.FixWaterTampleTeleport = bool.Parse(Global.cfgmgr.ReadValue(Global.ConfigFile, "anticheat", "fix_water_tample_teleport "));
                            isDone = true;
                            category = "anticheat";
                            break;
                        case "enable_login_processing":
                            Global.EnableLoginProcessing = bool.Parse(Global.cfgmgr.ReadValue(Global.ConfigFile, "server", "enable_login_processing"));
                            isDone = true;
                            category = "server";
                            break;
                        case "enable_arena_status_notify":
                            Global.EnableArenaStatusNotify = bool.Parse(Global.cfgmgr.ReadValue(Global.ConfigFile, "anticheat", "enable_arena_status_notify"));
                            isDone = true;
                            category = "anticheat";
                            break;
                        case "text_encode_code":
                            Global.TextEncodeCode = int.Parse(Global.cfgmgr.ReadValue(Global.ConfigFile, "server", "text_encode_code"));
                            isDone = true;
                            category = "server";
                            break;
                        case "shard_max_online":
                            Global.ShardMaxOnline = int.Parse(Global.cfgmgr.ReadValue(Global.ConfigFile, "server", "shard_max_online"));
                            isDone = true;
                            category = "server";
                            break;
                        case "shard_fake_online":
                            Global.ShardFakeOnline = int.Parse(Global.cfgmgr.ReadValue(Global.ConfigFile, "server", "shard_fake_online"));
                            isDone = true;
                            category = "server";
                            break;
                        case "enable_log_out_cooldown":
                            Global.EnableLogOutCooldown= bool.Parse(Global.cfgmgr.ReadValue(Global.ConfigFile, "anticheat", "enable_log_out_cooldown"));
                            isDone = true;
                            category = "anticheat";
                            break;
                        case "enable_stall_cooldown":
                            Global.EnableStallCooldown = bool.Parse(Global.cfgmgr.ReadValue(Global.ConfigFile, "anticheat", "enable_stall_cooldown"));
                            isDone = true;
                            category = "anticheat";
                            break;
                        case "gm_access_control":
                            Global.EnableGmAccessControl = bool.Parse(Global.cfgmgr.ReadValue(Global.ConfigFile, "anticheat", "gm_access_control"));
                            isDone = true;
                            category = "anticheat";
                            break;
                        case "enable_abuse_filter":
                            Global.EnableAbuseFilter = bool.Parse(Global.cfgmgr.ReadValue(Global.ConfigFile, "anticheat", "gm_access_control"));
                            isDone = true;
                            category = "anticheat";
                            break;
                        case "enable_login_notice":
                            Global.EnableLoginNotice = bool.Parse(Global.cfgmgr.ReadValue(Global.ConfigFile, "game", "enable_login_notice"));
                            isDone = true;
                            category = "game";
                            break;
                        case "enable_unique_death_notify":
                            Global.EnableUniqueDeathNotify = bool.Parse(Global.cfgmgr.ReadValue(Global.ConfigFile, "game", "enable_unique_death_notify"));
                            isDone = true;
                            category = "game";
                            break;
                        case "enable_auto_notice":
                            Global.EnableAutoNotice = bool.Parse(Global.cfgmgr.ReadValue(Global.ConfigFile, "game", "enable_auto_notice"));
                            isDone = true;
                            category = "game";
                            break;
                        case "auto_notice_begin_after":
                            Global.AutoNoticeSendBeginDelay = int.Parse(Global.cfgmgr.ReadValue(Global.ConfigFile, "game", "auto_notice_begin_after"));
                            isDone = true;
                            category = "game";
                            break;
                        case "enable_chat_commands":
                            Global.EnableChatCommands = bool.Parse(Global.cfgmgr.ReadValue(Global.ConfigFile, "game", "enable_chat_commands"));
                            isDone = true;
                            category = "game";
                            break;
                        case "arena_registration_level":
                            Global.ArenaRegistrationLevel = int.Parse(Global.cfgmgr.ReadValue(Global.ConfigFile, "game", "arena_registration_level"));
                            isDone = true;
                            category = "game";
                            break;
                        case "flag_registration_level":
                            Global.FlagRegistrationLevel = int.Parse(Global.cfgmgr.ReadValue(Global.ConfigFile, "game", "flag_registration_level"));
                            isDone = true;
                            category = "game";
                            break;
                        case "max_opt_level":
                            Global.MaxOptLevel = int.Parse(Global.cfgmgr.ReadValue(Global.ConfigFile, "game", "max_opt_level"));
                            isDone = true;
                            category = "game";
                            break;
                        case "max_members_in_guild":
                            Global.MaxMembersInGuild = int.Parse(Global.cfgmgr.ReadValue(Global.ConfigFile, "game", "max_members_in_guild"));
                            isDone = true;
                            category = "game";
                            break;
                        case "max_guild_in_alliance":
                            Global.MaxGuildInUnion = int.Parse(Global.cfgmgr.ReadValue(Global.ConfigFile, "game", "max_guild_in_alliance"));
                            isDone = true;
                            category = "game";
                            break;
                        case "disable_academy_invite":
                            Global.DisableAcademyInvite = bool.Parse(Global.cfgmgr.ReadValue(Global.ConfigFile, "game", "disable_academy_invite"));
                            isDone = true;
                            category = "game";
                            break;
                        default:
                            Console.WriteLine("Unknown config");
                            break;
                    }

                    if (isDone)
                    {
                        Console.WriteLine("Config {0} value set to {1}", config[1], Global.cfgmgr.ReadValue(Global.ConfigFile, category, config[1]));
                    }
                }
                else
                {
                    Console.WriteLine("Update Config Argument Error {0}", config[1]);
                }
            }
            catch
            {
                Console.WriteLine("Update Config Error");
            }
        }

        //-----------------------------------------------------------------------------

        public void SendNoticeToCharname(string cmd_str)
        {
            string[] cmd_args = cmd_str.Split(new char[] { ' ' });
            string charname = cmd_args[1];

            string notice_str = string.Empty;
            for (int i = 2; i < cmd_args.Length; i++)
            {
                notice_str += string.Format("{0} ", cmd_args[i]);
            }

            Packet pck = new Packet(0x3026);
            pck.WriteUInt8(7);
            pck.WriteAscii(notice_str);

            bool send_res = Global.srvmgr.SendPacketToClientCharname(pck, ServerType.AgentServer, charname);

            if (send_res)
            {
                Console.WriteLine("Message successfully sent to char");
            }
            else
            {
                Console.WriteLine("Character with such charname is not found");
            }
        }

        //-----------------------------------------------------------------------------

        public void StopAllContexts()
        {
            Global.srvmgr.StopAllContexts();
            Console.WriteLine("All users have been disconnected");
        }

        //-----------------------------------------------------------------------------

        public void PrintUptime()
        {
            TimeSpan TimeDiff = DateTime.Now - Global.StartupTime;
            Console.WriteLine("Uptime: {0}", TimeDiff);
        }

        //-----------------------------------------------------------------------------

        public void Inspection(string s)
        {
            string[] split = s.Split(' ');
            if(split.Count() == 2)
            {
                Global.EnableServerInspection = split[1] == "on" ? true : false;
                Console.WriteLine("Server Inspection is {0}", Global.EnableServerInspection.ToString());
            }
        }

        public void AddInspectionIgnoreUser(string s)
        {
            string[] split = s.Split(' ');
            if (split.Count() == 2)
            {
                Global.InspectionLoginIgnore.Add(split[1]);
                Console.WriteLine("Username {0} added to inspection ignore list",split[1]);
            }
        }

        public void UpdateList(string s)
        {
            string[] split = s.Split(' ');
            if(split.Count() == 2)
            {
                if(split[1] == "SafeRegions")
                {
                    Global.SafeRegions.Clear();
                    Global.SafeRegions = Global.cfgmgr.GetSafeRegions();

                    Console.WriteLine("SafeRegions Reload, cur count: {0}",Global.SafeRegions.Count());
                }

                if (split[1] == "LoginNotice")
                {
                    Global.LoginNotice.Clear();
                    Global.LoginNotice = Global.cfgmgr.GetLoginNotice();

                    Console.WriteLine("LoginNotice Reload, cur count: {0}", Global.LoginNotice.Count());
                }
                if (split[1] == "AbuseWord")
                {
                    Global.AbuseWord.Clear();
                    Global.AbuseWord = Global.cfgmgr.GetAbouseWord();

                    Console.WriteLine("AbuseWord Reload, cur count: {0}", Global.AbuseWord.Count());
                }

                if (split[1] == "ServerInfo")
                {
                    Global.ServerInfo.Clear();
                    Global.ServerInfo = Global.cfgmgr.GetServerInfo();

                    Console.WriteLine("ServerInfo Reload, cur count: {0}", Global.ServerInfo.Count());
                }

                if (split[1] == "ServerSchedule")
                {
                    Global.ServerSchedule.Clear();
                    Global.ServerSchedule = Global.cfgmgr.GetServerSchedule();

                    Console.WriteLine("ServerSchedule Reload, cur count: {0}", Global.ServerSchedule.Count());
                }

            }
        }
    }
}
