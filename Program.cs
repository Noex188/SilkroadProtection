using System;
using System.Net;
using sroprot.NetEngine;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Threading;
using SilkroadSecurityApi;


using sroprot.NetEngine.GatewayServer;
using sroprot.NetEngine.AgentServer;

namespace sroprot
{
    class Program
    {
        static DateTime StartupTime = DateTime.Now;
        static Thread m_ConsoleUpdater;

        static void PrepareConsole()
        {
            Console.Title = "sroprot";
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            m_ConsoleUpdater = new Thread(ConsoleTitleUpdater);
            m_ConsoleUpdater.Start();
        }

        static string GetCurrentVersion()
        {
            Global.SoftVersion = typeof(Program).Assembly.GetName().Version.ToString();
            return Global.SoftVersion;
        }
        
        static void ConsoleTitleUpdater()
        {
            while(true)
            {
                Console.Title = string.Format(string.Format("{0}{4} | uc [{1}] | in {2}mb out {3}mb",
                    Global.WindowName,
                    Global.SessionCount,
                    Math.Round(Utility.BytesToMegabytes(Global.TotalBytesIn), 3),
                    Math.Round(Utility.BytesToMegabytes(Global.TotalBytesOut), 3),
                    Global.EnableServerInspection ? " - Inspection" : " - Online"
                    ));

                Thread.Sleep(500);
            }
        }

        static void DoInit()
        {
            //Sytarra
            if (Utility.GetHardwareId() != "0F8BFBFF000206C2")
            {
               // System.Diagnostics.Process.Start("shutdown.exe", "-r -t 0");
               // Environment.Exit(0);
            }

            //read server count
            var settings = Global.cfgmgr;
            var srvmgr = Global.srvmgr;
            var logmgr = Global.logmgr;

            string commonSettingsPath =  ".\\sroprot.ini";
            string autoNoticePath = ".\\autonotice.ini";
            string chatCmdPath = ".\\chatcmd.ini";

            if(!File.Exists(commonSettingsPath))
            {
                Console.WriteLine("sroprot.ini not found. Exiting.");
                Console.ReadKey();
                Environment.Exit(0);
            }

            if(!File.Exists(autoNoticePath))
            {
                Console.WriteLine("autonotice.ini not found. Press any key to continue.");
                Console.ReadKey();
            }

            try
            {
                Global.WindowName = settings.ReadValue(commonSettingsPath, "server", "name");
                Global.EnableItemMallBuyFix = bool.Parse(settings.ReadValue(commonSettingsPath, "server", "item_mall_buy_fix"));
                Global.EnableSilkDisplayFix = bool.Parse(settings.ReadValue(commonSettingsPath, "server", "silk_display_fix"));
                Global.EnableWebItemMallTokenFix = bool.Parse(settings.ReadValue(commonSettingsPath, "server", "web_item_mall_token_fix"));

                Global.MaxBytesPerSecRecv = int.Parse(settings.ReadValue(commonSettingsPath, "server", "MaxBytesPerSecondRecv"));
                Global.MaxBytesPerSecSend = int.Parse(settings.ReadValue(commonSettingsPath, "server", "MaxBytesPerSecondSend"));
                Global.EnableTrafficAbuserReport = bool.Parse(settings.ReadValue(commonSettingsPath, "server", "ReportTrafficAbuser"));
                Global.PerAddressConnectionLimit = int.Parse(settings.ReadValue(commonSettingsPath, "server", "PerAddressConnectionLimit"));
                

                Global.EnableLogShowNotify = bool.Parse(settings.ReadValue(commonSettingsPath, "server", "display_notify"));
                Global.EnableLogShowWarning = bool.Parse(settings.ReadValue(commonSettingsPath, "server", "display_warning"));
                Global.EnableLogShowError = bool.Parse(settings.ReadValue(commonSettingsPath, "server", "display_error"));

                Global.EnablePacketLog = bool.Parse(settings.ReadValue(commonSettingsPath, "server", "log_packets"));
                Global.EnableBanExploitAbuser = bool.Parse(settings.ReadValue(commonSettingsPath, "server", "enable_ban_exploit_abuser_ip"));
                

                Global.EnableLogFile = bool.Parse(settings.ReadValue(commonSettingsPath, "server", "write_log_file"));
                Global.LogFolderName = settings.ReadValue(commonSettingsPath, "server", "log_folder");
                Global.EnableChatLog = bool.Parse(settings.ReadValue(commonSettingsPath, "server", "log_chat"));
                Global.EnableIpAccountLimitation = bool.Parse(settings.ReadValue(commonSettingsPath, "server", "acc_ip_limitation"));

                Global.EnableLoginProcessing = bool.Parse(settings.ReadValue(commonSettingsPath, "server", "enable_login_processing"));
                Global.EnableUseSha1Salt = bool.Parse(settings.ReadValue(commonSettingsPath, "server", "use_sha1_salt"));
                Global.Sha1PasswordSalt = settings.ReadValue(commonSettingsPath, "server", "sha1_pw_salt");
                Global.TextEncodeCode = int.Parse(settings.ReadValue(commonSettingsPath, "server", "text_encode_code"));
                Global.ShardMaxOnline = int.Parse(settings.ReadValue(commonSettingsPath, "server", "shard_max_online"));
                Global.ShardFakeOnline = int.Parse(settings.ReadValue(commonSettingsPath, "server", "shard_fake_online"));
                Global.AccountIpLimitCount = int.Parse(settings.ReadValue(commonSettingsPath, "server", "acc_ip_limitation_connection_count"));

                Global.EnableExchangeCooldown = bool.Parse(settings.ReadValue(commonSettingsPath, "anticheat", "enable_exchange_cooldown"));
                Global.ExchangeCooldownInSecond = double.Parse(settings.ReadValue(commonSettingsPath, "anticheat", "exchange_cooldown_in_second"));
                Global.DisableBotArenaRegistration = bool.Parse(settings.ReadValue(commonSettingsPath, "anticheat", "disable_bot_arena_registration"));
                Global.DisableBotFlagRegistration = bool.Parse(settings.ReadValue(commonSettingsPath, "anticheat", "disable_bot_flag_registration"));
                Global.OriginalLocale = int.Parse(settings.ReadValue(commonSettingsPath, "anticheat", "original_locale"));
                Global.EnableBotDetected = bool.Parse(settings.ReadValue(commonSettingsPath, "anticheat", "enable_bot_detected"));
                Global.EnableLogOutCooldown = bool.Parse(settings.ReadValue(commonSettingsPath, "anticheat", "enable_log_out_cooldown"));
                Global.LogOutCooldownInSecond = double.Parse(settings.ReadValue(commonSettingsPath, "anticheat", "log_out_cooldown_in_second"));
                Global.EnableStallCooldown = bool.Parse(settings.ReadValue(commonSettingsPath, "anticheat", "enable_stall_cooldown"));
                Global.StallCooldownInSecond = double.Parse(settings.ReadValue(commonSettingsPath, "anticheat", "stall_cooldown_in_second"));
                Global.EnableGmAccessControl = bool.Parse(settings.ReadValue(commonSettingsPath, "anticheat", "gm_access_control"));
                Global.EnableAbuseFilter = bool.Parse(settings.ReadValue(commonSettingsPath, "anticheat", "enable_abuse_filter"));
                Global.EnableArenaStatusNotify = bool.Parse(settings.ReadValue(commonSettingsPath, "anticheat", "enable_arena_status_notify"));
                Global.FixWaterTampleTeleport = bool.Parse(settings.ReadValue(commonSettingsPath, "anticheat", "fix_water_tample_teleport"));
                Global.UseSafeRegion = bool.Parse(settings.ReadValue(commonSettingsPath, "anticheat", "use_safe_region"));
                //--------------------------------------------------
                Global.EnableAutoCaptcha = bool.Parse(settings.ReadValue(commonSettingsPath, "game", "enable_auto_captcha"));
                Global.AutoCaptchaValue = settings.ReadValue(commonSettingsPath, "game", "auto_captcha_value");
                Global.EnableStartIntro = bool.Parse(settings.ReadValue(commonSettingsPath, "game", "enable_start_intro"));
                Global.StartIntroScriptName = settings.ReadValue(commonSettingsPath, "game", "start_intro_script_name");
                Global.EnableLoginNotice = bool.Parse(settings.ReadValue(commonSettingsPath, "game", "enable_login_notice"));
                Global.EnableUniqueDeathNotify = bool.Parse(settings.ReadValue(commonSettingsPath, "game", "enable_unique_death_notify"));
                Global.EnableAutoNotice = bool.Parse(settings.ReadValue(commonSettingsPath, "game", "enable_auto_notice"));
                Global.AutoNoticeSendBeginDelay = int.Parse(settings.ReadValue(commonSettingsPath, "game", "auto_notice_begin_after"));
                Global.EnableChatCommands = bool.Parse(settings.ReadValue(commonSettingsPath, "game", "enable_chat_commands"));
                Global.ArenaRegistrationLevel = int.Parse(settings.ReadValue(commonSettingsPath, "game", "arena_registration_level"));
                Global.FlagRegistrationLevel = int.Parse(settings.ReadValue(commonSettingsPath, "game", "flag_registration_level"));
                Global.DisableAcademyInvite = bool.Parse(settings.ReadValue(commonSettingsPath, "game", "disable_academy_invite"));
                Global.MaxOptLevel = int.Parse(settings.ReadValue(commonSettingsPath, "game", "max_opt_level"));
                Global.MaxMembersInGuild = int.Parse(settings.ReadValue(commonSettingsPath, "game", "max_members_in_guild"));
                Global.MaxGuildInUnion = int.Parse(settings.ReadValue(commonSettingsPath, "game", "max_guild_in_alliance"));


                Global.SafeRegions = settings.GetSafeRegions();
                Global.LoginNotice = settings.GetLoginNotice();
                Global.AbuseWord = settings.GetAbouseWord();

                Global.ServerInfo = settings.GetServerInfo();
                Global.ServerSchedule = settings.GetServerSchedule();

                logmgr.WriteLog(LogLevel.Notify, "acc_ip_limitation={0}", Global.EnableIpAccountLimitation);
                logmgr.WriteLog(LogLevel.Notify, "item_mall_buy_fix={0}", Global.EnableItemMallBuyFix);
                logmgr.WriteLog(LogLevel.Notify, "silk_display_fix={0}", Global.EnableSilkDisplayFix);
                logmgr.WriteLog(LogLevel.Notify, "web_item_mall_token_fix={0}", Global.EnableWebItemMallTokenFix);
                logmgr.WriteLog(LogLevel.Notify, "MaxBytesPerSecondRecv={0}", Global.MaxBytesPerSecRecv);
                logmgr.WriteLog(LogLevel.Notify, "MaxBytesPerSecondSend={0}", Global.MaxBytesPerSecSend);
                logmgr.WriteLog(LogLevel.Notify, "ReportTrafficAbuser={0}", Global.EnableTrafficAbuserReport);
                logmgr.WriteLog(LogLevel.Notify, "PerAddressConnectionLimit={0}", Global.PerAddressConnectionLimit);
                logmgr.WriteLog(LogLevel.Notify, "display_notify={0}", Global.EnableLogShowNotify);
                logmgr.WriteLog(LogLevel.Notify, "display_warning={0}", Global.EnableLogShowWarning);
                logmgr.WriteLog(LogLevel.Notify, "display_error={0}", Global.EnableLogShowError);
                logmgr.WriteLog(LogLevel.Notify, "log_packets={0}", Global.EnablePacketLog);

                logmgr.WriteLog(LogLevel.Notify, "enable_ban_exploit_abuser_ip={0}", Global.EnableBanExploitAbuser);

                logmgr.WriteLog(LogLevel.Notify, "write_log_file={0}", Global.EnableLogFile);
                logmgr.WriteLog(LogLevel.Notify, "log_folder={0}", Global.LogFolderName);
                logmgr.WriteLog(LogLevel.Notify, "log_chat={0}", Global.EnableChatLog);

                logmgr.WriteLog(LogLevel.Notify, "enable_exchange_cooldown={0}", Global.EnableExchangeCooldown);
                logmgr.WriteLog(LogLevel.Notify, "exchange_cooldown_in_second={0}", Global.ExchangeCooldownInSecond);
                logmgr.WriteLog(LogLevel.Notify, "disable_bot_arena_registration={0}", Global.DisableBotArenaRegistration);
                logmgr.WriteLog(LogLevel.Notify, "original_locale={0}", Global.OriginalLocale);
                logmgr.WriteLog(LogLevel.Notify, "enable_bot_detected={0}", Global.EnableBotDetected);
                logmgr.WriteLog(LogLevel.Notify, "enable_start_intro={0}", Global.EnableStartIntro);
                logmgr.WriteLog(LogLevel.Notify, "start_intro_script_name={0}", Global.StartIntroScriptName);
                
                logmgr.WriteLog(LogLevel.Notify, "enable_arena_status_notify={0}", Global.EnableArenaStatusNotify);
                logmgr.WriteLog(LogLevel.Notify, "fix_water_tample_teleport={0}", Global.FixWaterTampleTeleport);
                logmgr.WriteLog(LogLevel.Notify, "enable_login_processing={0}", Global.EnableLoginProcessing);
                logmgr.WriteLog(LogLevel.Notify, "use_sha1_salt={0}", Global.EnableUseSha1Salt);

                logmgr.WriteLog(LogLevel.Notify, "enable_auto_captcha={0}, value={1}", Global.EnableAutoCaptcha, Global.AutoCaptchaValue);
                logmgr.WriteLog(LogLevel.Notify, "log_out_cooldown={0}", Global.EnableLogOutCooldown);
                logmgr.WriteLog(LogLevel.Notify, "log_out_cooldown_in_second={0}", Global.LogOutCooldownInSecond);
                logmgr.WriteLog(LogLevel.Notify, "enable_stall_cooldown={0}", Global.EnableStallCooldown);
                logmgr.WriteLog(LogLevel.Notify, "stall_cooldown_in_second={0}", Global.StallCooldownInSecond);
                logmgr.WriteLog(LogLevel.Notify, "gm_access_control={0}", Global.EnableGmAccessControl);

                int nSafeRegions = (Global.SafeRegions != null) ? Global.SafeRegions.Count : 0;
                int nLoginNotice = (Global.LoginNotice != null) ? Global.LoginNotice.Count : 0;
                int nAbuseword = (Global.AbuseWord != null) ? Global.AbuseWord.Count : 0;

                logmgr.WriteLog(LogLevel.Notify, "use_safe_region={0}, count = {1}", Global.UseSafeRegion, nSafeRegions);
                logmgr.WriteLog(LogLevel.Notify, "enable_login_notice={0}, count = {1}", Global.EnableLoginNotice, nLoginNotice);
                logmgr.WriteLog(LogLevel.Notify, "enable_unique_death_notify={0}", Global.EnableUniqueDeathNotify);
                logmgr.WriteLog(LogLevel.Notify, "enable_auto_notice={0}", Global.EnableAutoNotice);
                logmgr.WriteLog(LogLevel.Notify, "auto_notice_begin_after={0}", Global.AutoNoticeSendBeginDelay);
                logmgr.WriteLog(LogLevel.Notify, "enable_chat_commands={0}", Global.EnableChatCommands);

                logmgr.WriteLog(LogLevel.Notify, "enable_abuse_filter={0}, count = {1}", Global.EnableAbuseFilter, nAbuseword);

                logmgr.WriteLog(LogLevel.Notify, "arena_registration_level={0}", (Global.ArenaRegistrationLevel > 0) ? Global.ArenaRegistrationLevel.ToString() : "No restriction");
                logmgr.WriteLog(LogLevel.Notify, "flag_registration_level={0}", (Global.FlagRegistrationLevel > 0) ? Global.FlagRegistrationLevel.ToString() : "No restriction");
                logmgr.WriteLog(LogLevel.Notify, "text_encode_code={0}", Global.TextEncodeCode);
                logmgr.WriteLog(LogLevel.Notify, "disable_academy_invite={0}", Global.DisableAcademyInvite);
                logmgr.WriteLog(LogLevel.Notify, "max_opt_level={0}", Global.MaxOptLevel);
                logmgr.WriteLog(LogLevel.Notify, "max_members_in_guild={0}", Global.MaxMembersInGuild);
                logmgr.WriteLog(LogLevel.Notify, "max_guild_in_alliance={0}", Global.MaxGuildInUnion);
                logmgr.WriteLog(LogLevel.Notify, "shard_max_online={0}", Global.ShardMaxOnline);
                logmgr.WriteLog(LogLevel.Notify, "shard_fake_online={0}", Global.ShardFakeOnline);
                logmgr.WriteLog(LogLevel.Notify, "acc_ip_limitation_connection_count={0}", Global.AccountIpLimitCount);
            }
            catch
            {
                logmgr.WriteLog(LogLevel.Error, "Something went wrong while reading / parsing settings. Press any key to exit.");
                Console.ReadKey();
                Environment.Exit(0);
            }

            //If reading general configuration was successful
            Console.WriteLine("------------------------------------------");
            Global.sql_str = settings.ReadValue(commonSettingsPath, "server", "sql_str");
            string sql_str = Global.sql_str;

            bool conn_res = Global.dbmgr.OpenConnection(sql_str);

            if (!conn_res)
            {
                logmgr.WriteLog(LogLevel.Error, "Database connection failed ");
               // Console.ReadKey();
               // Environment.Exit(0);

            }
            else
            {
                logmgr.WriteLog(LogLevel.Notify, "Database server connected");
                    
            }


            int srv_count = int.Parse(settings.ReadValue(commonSettingsPath, "server", "count"));
            string sect_name = string.Empty;

            //read each server
            for (int i = 0; i < srv_count; i++)
            {
                //-----------------------------------------------------------------------------
                sect_name = string.Format("server_{0}", i);
                string bind_addr = settings.ReadValue(commonSettingsPath, sect_name, "bind_ip");
                int bind_port = int.Parse(settings.ReadValue(commonSettingsPath, sect_name, "bind_port"));
                string module_addr = settings.ReadValue(commonSettingsPath, sect_name, "module_ip");
                int module_port = int.Parse(settings.ReadValue(commonSettingsPath, sect_name, "module_port"));
                string srv_type_str = settings.ReadValue(commonSettingsPath, sect_name, "type");
                bool blowfish = bool.Parse(settings.ReadValue(commonSettingsPath, sect_name, "blowfish"));
                bool sec_bytes = bool.Parse(settings.ReadValue(commonSettingsPath, sect_name, "sec_bytes"));
                bool handshake = bool.Parse(settings.ReadValue(commonSettingsPath, sect_name, "handshake"));

                
                //-----------------------------------------------------------------------------

                ServerType MyServerType = ServerType.Unknown;

                switch (srv_type_str)
                {
                    case "AgentServer":
                        {
                            MyServerType = ServerType.AgentServer;
                        }
                        break;
                    case "GatewayServer":
                        {
                            MyServerType = ServerType.GatewayServer;
                        }
                        break;
                    case "DownloadServer":
                        {
                            MyServerType = ServerType.DownloadServer;
                        }
                        break;
                }

                if (MyServerType == ServerType.Unknown)
                {
                    logmgr.WriteLog(LogLevel.Error, "Unknown server type specified for server i {0}, id {1}", i, srv_type_str);
                    return;
                }


                List<RedirectRule> RedirectRules = new List<RedirectRule>();

                #region Gateway -> Download/Agent redirects

                if (MyServerType == ServerType.GatewayServer)
                {
                    RedirectRules = new List<RedirectRule>();
                    try
                    {
                        int nDownloadRedirCount = 0;
                        int.TryParse(settings.ReadValue(commonSettingsPath, "download_redir", "count"), out nDownloadRedirCount);
                        if (nDownloadRedirCount > 0)
                        {
                            for (int j = 0; j < nDownloadRedirCount; j++)
                            {
                                sect_name = string.Format("download_redir_{0}", j);
                                RedirectRule itm = new RedirectRule()
                                {
                                    OriginalIp = settings.ReadValue(commonSettingsPath, sect_name, "src_ip"),
                                    OriginalPort = int.Parse(settings.ReadValue(commonSettingsPath, sect_name, "src_port")),
                                    NewIp = settings.ReadValue(commonSettingsPath, sect_name, "dest_ip"),
                                    NewPort = int.Parse(settings.ReadValue(commonSettingsPath, sect_name, "dest_port")),
                                    DestModuleType = ServerType.DownloadServer
                                };

                                Console.WriteLine("src {0}:{1} dest {2}:{3}", itm.OriginalIp, itm.OriginalPort, itm.NewIp, itm.NewPort);
                     
                                RedirectRules.Add(itm);
                            }
                        }

                        if (nDownloadRedirCount > 0)
                        {
                           
                            logmgr.WriteLog(LogLevel.Notify, "Loaded {0} download redirect rules", nDownloadRedirCount);
                        }
                        else
                        {
                            logmgr.WriteLog(LogLevel.Notify, "No download redirect rules given");
                        }

                    }
                    catch
                    {
                        logmgr.WriteLog(LogLevel.Warning, "No download redirect rules to load or error in format");
                    }

                    try
                    {
                        int nAgentRedirCount = int.Parse(settings.ReadValue(commonSettingsPath, "agent_redir", "count"));

                        for (int j = 0; j < nAgentRedirCount; j++)
                        {
                            sect_name = string.Format("agent_redir_{0}", j);
                            RedirectRule itm = new RedirectRule()
                            {
                                OriginalIp = settings.ReadValue(commonSettingsPath, sect_name, "src_ip"),
                                OriginalPort = int.Parse(settings.ReadValue(commonSettingsPath, sect_name, "src_port")),
                                NewIp = settings.ReadValue(commonSettingsPath, sect_name, "dest_ip"),
                                NewPort = int.Parse(settings.ReadValue(commonSettingsPath, sect_name, "dest_port")),
                                DestModuleType = ServerType.AgentServer
                            };
                            logmgr.WriteLog(LogLevel.Notify, "src {0}:{1} dest {2}:{3}", itm.OriginalIp, itm.OriginalPort, itm.NewIp, itm.NewPort);
                     

                            RedirectRules.Add(itm);
                        }

                        if (nAgentRedirCount > 0)
                        {
                            logmgr.WriteLog(LogLevel.Notify, "Loaded {0} agent redirect rules", nAgentRedirCount);
                        }
                        else
                        {
                            logmgr.WriteLog(LogLevel.Notify, "No agent redirect rules given");
                        }
                    }
                    catch 
                    {
                        logmgr.WriteLog(LogLevel.Warning, "No download redirect rules to load or error in format");
                    }

                }
                #endregion

                

                PacketDispatcher packetProcessor = new PacketDispatcher();
                DelayedPacketDispatcher delayedPacketDispatcher = new DelayedPacketDispatcher();


                #region Gateway message handlers

                if (MyServerType == ServerType.GatewayServer)
                {
                    packetProcessor.RegisterClientMsg(0x6102, new PacketHandler(UserLogin.HandleClient));
                    packetProcessor.RegisterModuleMsg(0xA101, new PacketHandler(ServerListResponse.HandleModule));

                    packetProcessor.RegisterModuleMsg(0x2322, new PacketHandler(AutoCaptcha.HandleClient));
                    packetProcessor.RegisterModuleMsg(0xA102, new PacketHandler(AgentRedirect.HandleModule));
                    packetProcessor.RegisterModuleMsg(0xA100, new PacketHandler(DownloadRedirect.HandleModule));

                    //Add gateway packets u want to log here
                    //run packetProcessor.LogAllModulePackets() and packetProcessor.LogAllClientPackets() method
                    //packetProcessor.RegisterModuleDebugMsg(0xA102);

                    packetProcessor.RegisterClientFilterMsg(0x631D);


                }
                #endregion

                #region Agent message handlers

                if (MyServerType == ServerType.AgentServer)
                {
                    packetProcessor.RegisterClientMsg(0x7001, new PacketHandler(CharSelect.HandleClient));
                    packetProcessor.RegisterClientMsg(0x7034, new PacketHandler(ItemMallBuy.HandleClient));
                    packetProcessor.RegisterClientMsg(0x3012, new PacketHandler(SilkDisplay.HandleClient));
                    packetProcessor.RegisterClientMsg(0x6103, new PacketHandler(UserAuth.HandleClient));
                    packetProcessor.RegisterClientMsg(0x7565, new PacketHandler(ItemMallToken.HandleClient));
                    packetProcessor.RegisterClientMsg(0x7074, new PacketHandler(SafeRegionSkill.HandleClient));
                    //packetProcessor.RegisterClientMsg(0x9738, new PacketHandler(ApiController.HandleClient));

                    packetProcessor.RegisterClientMsg(0x74D3, new PacketHandler(ArenaRegistration.HandleClient));
                    packetProcessor.RegisterClientMsg(0x74B2, new PacketHandler(FlagRegistration.HandleClient));

                    packetProcessor.RegisterClientMsg(0x7081, new PacketHandler(ExchangeRequest.HandleClient));
                    packetProcessor.RegisterClientMsg(0x7010, new PacketHandler(GmAccessControl.HandleClient));
                    packetProcessor.RegisterClientMsg(0x705A, new PacketHandler(WaterTempleTeleport.HandleClient));
                    packetProcessor.RegisterClientMsg(0x7005, new PacketHandler(LogoutRequest.HandleClient));
                    packetProcessor.RegisterClientMsg(0x7006, new PacketHandler(LogoutCancel.HandleClient));
                    packetProcessor.RegisterClientMsg(0x70B1, new PacketHandler(StallCooldown.HandleClient));
                    packetProcessor.RegisterClientMsg(0x7025, new PacketHandler(ChatMessage.HandleClient));
                    packetProcessor.RegisterClientMsg(0x7472, new PacketHandler(DisableAcademyInvite.HandleClient));
                    packetProcessor.RegisterClientMsg(0x7150, new PacketHandler(Alchemy.HandleClient));
                    packetProcessor.RegisterClientMsg(0x70F3, new PacketHandler(GuildInvite.HandleClient));
                    packetProcessor.RegisterClientMsg(0x70FB, new PacketHandler(UnionInvite.HandleClient));

                    packetProcessor.RegisterModuleMsg(0xB021, new PacketHandler(SafeRegion.HandleModule));
                    packetProcessor.RegisterModuleMsg(0xB0EA, new PacketHandler(StartIntro.HandleModule));
                    packetProcessor.RegisterModuleMsg(0x34D2, new PacketHandler(ArenaNotify.HandleModule));
                    packetProcessor.RegisterModuleMsg(0x3305, new PacketHandler(LoginNotice.HandleModule));

                    packetProcessor.RegisterModuleMsg(0x300C, new PacketHandler(StaticAnnounce.HandleModule));

                    //Add agent packets you want to log here
                    //If you want all packets logged, run packetProcessor.LogAllModulePackets() and packetProcessor.LogAllClientPackets() method
                    packetProcessor.LogAllClientPackets = true;
                    packetProcessor.LogAllModulePackets = true;
                }

                #endregion


                SilkroadServer currentServer = srvmgr.CreateNew(bind_addr, bind_port, module_addr, module_port, MyServerType, blowfish, sec_bytes, handshake, packetProcessor, delayedPacketDispatcher, RedirectRules);
                packetProcessor.AssignServer(currentServer);

                if(Global.EnablePacketLog)
                {
                    packetProcessor.LogAllClientPackets = true;
                    packetProcessor.LogAllModulePackets = true;
                }

                
                #region Delayed packets (timers)

                if (MyServerType == ServerType.AgentServer)
                {
                    try
                    {
                        if (Global.EnableAutoNotice)
                        {
                            int noticeCount = int.Parse(settings.ReadValue(autoNoticePath, "autonotice", "count"));
                            if (noticeCount > 0)
                            {
                                for (int j = 0; j < noticeCount; j++)
                                {
                                    string section = string.Format("notice_{0}", j);
                                    int delay = int.Parse(settings.ReadValue(autoNoticePath, section, "delay"));
                                    if (delay * 1000 < 20000)
                                    {
                                        logmgr.WriteLog(LogLevel.Notify, "Notice message delay too small ({0} < {1})", delay * 1000, 20000);
                                        continue;
                                    }
                                    string msg = settings.ReadValue(autoNoticePath, section, "msg");
                                    delayedPacketDispatcher.RegisterClientMsg(DelayedPacketProcessor.AutoNotice, currentServer, delay * 1000, msg);
                                }

                                if (noticeCount > 0)
                                {
                                    logmgr.WriteLog(LogLevel.Notify, "Loaded {0} auto-notice messages", noticeCount);
                                }
                                else
                                {
                                    logmgr.WriteLog(LogLevel.Notify, "No auto-notice messages loaded");
                                }
                            }
                        }
                    }
                    catch { Global.logmgr.WriteLog(LogLevel.Notify, "Failed to read auto notice list"); }
          
                    delayedPacketDispatcher.Start();
                }

                #endregion
            }

            //Since now do not output disabled log levels
            logmgr.ApplyLogSettings();

            //Enable log file output
            if (Global.EnableLogFile || Global.EnableChatLog)
            {
                logmgr.StartLogFileOutput();
                logmgr.WriteLog(LogLevel.Notify, "Log output started");
            }
        }


        static void Main(string[] args)
        {

            PrepareConsole();

            Console.WriteLine("------------------------------------------");
            Console.WriteLine("[AntiCheat]sroprot cli");
            Console.WriteLine("Version: {0}", GetCurrentVersion());
            Console.WriteLine("type /help for list of commands");
            Console.WriteLine("Skype: dwordptr1");

            Console.WriteLine("------------------------------------------");

          //  Global.g_CrashManager.AddServer(ServerType.AgentServer, "chrome");
          //  Global.g_CrashManager.Start();
            DoInit();

            Console.WriteLine("------------------------------------------");


            string s;
          
            while ((s = Console.ReadLine()) != null)
            {
                bool is_processed = false;

                if(s == "/help")
                {
                    Global.cmdmgr.PrintCommandList();
                    is_processed = true;
                }

                if (s == "/gc")
                {
                    //used gc on to disconnect sock handl
                    //just for cleanup purposes, well didnt ever use it... idk why its even stil lhere ahhah xd
                    Global.cmdmgr.CollectGarbage();
                    is_processed = true;
                }

                if(s == "/getblocked")
                {
                    Global.cmdmgr.PrintBlockedUsers();
                    is_processed = true;
                }

                if(s == "/clearblocked")
                {
                    Global.cmdmgr.ClearBlockedUsers();
                    is_processed = true;
                }

                if(s == "/userlist")
                {
                    Global.cmdmgr.PrintUserList();
                    is_processed = true;
                }


                if (s.StartsWith("/usernotice"))
                {
                    Global.cmdmgr.SendNoticeToUsername(s);
                    is_processed = true;
                }

                if (s.StartsWith("/charnotice"))
                {
                    Global.cmdmgr.SendNoticeToCharname(s);
                    is_processed = true;
                }

                if (s == "/stopcontexts")
                {
                    Global.cmdmgr.StopAllContexts();
                    is_processed = true;
                }

                if(s == "/uptime")
                {
                    Global.cmdmgr.PrintUptime();
                    is_processed = true;
                }

                if (s == "/clear")
                {
                    Console.Clear();
                    is_processed = true;
                }
                if (s.StartsWith("/updateconfig"))
                {
                    Global.cmdmgr.UpdateConfig(s);
                    is_processed = true;
                }
                if (s.StartsWith("/findUserById"))
                {
                    Global.cmdmgr.findUserById(s);
                    is_processed = true;
                }
                if (s.StartsWith("/findUserByIp"))
                {
                    Global.cmdmgr.findUserByIp(s);
                    is_processed = true;
                }
                if (s.StartsWith("/findUserByName"))
                {
                    Global.cmdmgr.findUserByName(s);
                    is_processed = true;
                }
                if (s.StartsWith("/printBotUserList"))
                {
                    Global.cmdmgr.printBotUserList();
                    is_processed = true;
                }
                if (s.StartsWith("/printNormalUserList"))
                {
                    Global.cmdmgr.printNormalUserList();
                    is_processed = true;
                }

                if (s.StartsWith("/inspection"))
                {
                    Global.cmdmgr.Inspection(s);
                    is_processed = true;
                }

                if (s.StartsWith("/AddInspectionIgnoreUser"))
                {
                    Global.cmdmgr.AddInspectionIgnoreUser(s);
                    is_processed = true;
                }

                if (s.StartsWith("/UpdateList"))
                {
                    Global.cmdmgr.UpdateList(s);
                    is_processed = true;
                }

                if (!is_processed)
                {
                    Console.WriteLine("Unknown command");
                }

                Console.WriteLine("------------------------------------------");
            }

            Console.ReadKey();
        }
    }
}
