using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

using sroprot.Core;
using sroprot.NetEngine;

namespace sroprot
{
    //-----------------------------------------------------------------------------
    public enum ServerType
    {
        AgentServer,
        GatewayServer,
        DownloadServer,
        Unknown
    }

    public enum LogLevel
    {
        Notify,
        Warning,
        Error,
    }

    public enum ChatType
    {
        Normal = 0x01,
        Union = 0x0B,
        Academy = 0x10,
        Guild = 0x05,
        Private = 0x02,
        Group = 0x04,
        Gm = 0x03,
        Anounce = 0x07,
        Stall = 0x09,
        Unknown = 250
    }

    //Used for logging packets
    public enum PacketDirection
    {
        ClientToModule,
        ModuleToClient
    }

    public struct RelaySessionArgs
    {
        public Socket ClientSocket;
        public IPEndPoint ModuleEp;
        public SessionEventHandler eOnCreate;
        public SessionEventHandler eOnDestroy;

        public bool Blowfish;
        public bool SecBytes;
        public bool Handshake;

        public ServerType SrvType;
    }

    public class RedirectRule
    {
        public string OriginalIp;
        public int OriginalPort;
        public string NewIp;
        public int NewPort;
        public ServerType DestModuleType;
    }



    //-----------------------------------------------------------------------------

    public delegate void SessionEventHandler(RelaySession context);

    //-----------------------------------------------------------------------------

    public sealed class Global
    {
        public static LogManager logmgr = LogManager.getInstance();
        public static ServerManager srvmgr = ServerManager.getInstance();
        public static ConfigManager cfgmgr = ConfigManager.getInstance();
        public static DatabaseManager dbmgr = DatabaseManager.getInstance();
        public static CommandManager cmdmgr = CommandManager.getInstance();
        public static ModuleCrashManager crashmgr = ModuleCrashManager.getInstance();

        public static DateTime StartupTime = DateTime.Now;

        static int m_total_session_count = 0;

        public static string WindowName = string.Empty;

        public static int SessionCount
        {
            get
            {
                return m_total_session_count;
            }
            set
            {
                m_total_session_count = value;
              
            }
        }
        //
        public static string sql_str = null;
        public static string SoftVersion = "none";
        //-----------------------------------------------------------------------------
        //configs for user context
        public static bool EnableItemMallBuyFix = false;
        public static bool EnableSilkDisplayFix = false;
        public static bool EnableWebItemMallTokenFix = false;
        //-----------------------------------------------------------------------------

        //configs for log manager
        public static bool EnableLogShowNotify = false;
        public static bool EnableLogShowWarning = false;
        public static bool EnableLogShowError = false;
        public static bool EnablePacketLog = false;

        public static bool EnableIpAccountLimitation = false;
        public static int AccountIpLimitCount = 0;

        public static bool EnableBanExploitAbuser = false;

        //md5(sha1(username.password.salt))
        public static bool EnableUseSha1Salt = false;
        public static string Sha1PasswordSalt = "";

        //configs for user context state and traffic filter
        public static int MaxBytesPerSecRecv = 0;
        public static int MaxBytesPerSecSend = 0;
        public static int PerAddressConnectionLimit = 0;
        public static bool EnableTrafficAbuserReport = false;

        //настройки античита
        public static bool EnableExchangeCooldown = false;
        public static double ExchangeCooldownInSecond = 20;
        public static bool DisableBotArenaRegistration = false;
        public static bool DisableBotFlagRegistration = false;
        public static int OriginalLocale = 22;
        public static bool EnableBotDetected = false;
        public static bool EnableStartIntro = false;
        public static string StartIntroScriptName = "roc_entrance";

        public static bool EnableArenaStatusNotify = false;
        public static bool FixWaterTampleTeleport = false;
        //
        public static bool EnableSecurityFilter = false;

        public static bool EnableLoginProcessing = false;


        public static string ConfigFile = Environment.CurrentDirectory + "\\sroprot.ini";

        public static int RecvBuffersSize = 4096;

        public static List<string> BlockedIpAddresses = new List<string>();

        public static bool UseSafeRegion = false;
        public static List<int> SafeRegions = new List<int>();
        public static List<string> LoginNotice = new List<string>();
        public static List<string> AbuseWord = new List<string>();
        public static List<string> InspectionLoginIgnore = new List<string>();
        public static List<string> ServerInfo = new List<string>();
        public static List<string> ServerSchedule = new List<string>();
        //-----------------------------------------------------------------------------
        public static ulong TotalBytesIn = 0;
        public static ulong TotalBytesOut = 0;

        public static bool EnableLogFile = false;
        public static string LogFolderName = "log";
        public static bool EnableChatLog = false;


        //-----------------------------------------------------------------------------
        public static bool EnableAutoCaptcha = false;
        public static string AutoCaptchaValue = "";
        //-----------------------------------------------------------------------------

        public static bool EnableLogOutCooldown = false;
        public static double LogOutCooldownInSecond = 20;
        public static bool EnableGmAccessControl = false;
        //-----------------------------------------------------------------------------
        public static bool EnableStallCooldown = false;
        public static double StallCooldownInSecond = 20;
        //-----------------------------------------------------------------------------
        public static bool EnableLoginNotice = false;
        public static bool EnableAbuseFilter = false;
        public static bool EnableUniqueDeathNotify = false;
        public static bool EnableAutoNotice = false;

        //Seconds
        public static int AutoNoticeSendBeginDelay = 60;

        public static bool EnableChatCommands = false;

        public static int ArenaRegistrationLevel = 0;
        public static int FlagRegistrationLevel = 0;

        public static int TextEncodeCode = 1251;

        public static bool DisableAcademyInvite = false;

        public static int MaxOptLevel = 0;


        public static int MaxMembersInGuild = 0;
        public static int MaxGuildInUnion = 0;

        public static int ShardMaxOnline = 0;
        public static int ShardFakeOnline = 0;

        public static bool EnableServerInspection = false;


        //
        public static string UniqueDeathNotifyName = "none";
        public static uint UniqueDeathNotifyID = 0;
        public static DateTime UniqueDeathNotifyTime = DateTime.Now;

       
        //

    }

    //-----------------------------------------------------------------------------
}
