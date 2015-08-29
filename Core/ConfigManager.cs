using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;


namespace sroprot.Core
{
    public sealed class ConfigManager
    {
        //-----------------------------------------------------------------------------

        static ConfigManager m_instance = null;
        private ConfigManager() { }

        public static ConfigManager getInstance()
        {
            if (m_instance == null)
            {
                m_instance = new ConfigManager();
            }
            return m_instance;
        }

        //-----------------------------------------------------------------------------

        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(string section,
                 string key, string def, StringBuilder retVal,
            int size, string filePath);

        //-----------------------------------------------------------------------------

        public string ReadValue(string file, string sect, string key)
        {
            StringBuilder temp = new StringBuilder(255);
            int nRead = GetPrivateProfileString(sect, key, "", temp, 255, file);

            //so exception is thrown when trying to output the str or parse it
            if(nRead == 0)
            {
                Global.logmgr.WriteLog(LogLevel.Error, "Failed to read settings sect {0} param {1}", sect, key);

                return null;
            }
            return temp.ToString();
        }

        public List<int> GetSafeRegions()
        {
            var result = new List<int>();

            try
            {
                string[] lines = File.ReadAllLines(".\\safe_region.txt");
                int region;

                for (int i = 0; i < lines.Length; i++)
                {
                    bool parsed = int.TryParse(lines[i], out region);
                    if (parsed)
                        result.Add(region);
                }
            }
            catch { }
            return result;
        }



        public List<string> GetLoginNotice()
        {
            var result = new List<string>();

            try
            {
                string[] lines = File.ReadAllLines(".\\login_notice.txt");

                for (int i = 0; i < lines.Length; i++)
                {
                        result.Add(lines[i]);
                }
            }
            catch { }
            return result;
        }


        public List<string> GetAbouseWord()
        {
            var result = new List<string>();

            try
            {
                string[] lines = File.ReadAllLines(".\\abouse_word_list.txt");

                for (int i = 0; i < lines.Length; i++)
                {
                    result.Add(lines[i]);
                }
            }
            catch { }
            return result;
        }


        public List<string> GetServerInfo()
        {
            var result = new List<string>();

            try
            {
                string[] lines = File.ReadAllLines(".\\serverinfo.txt");

                for (int i = 0; i < lines.Length; i++)
                {
                    result.Add(lines[i]);
                }
            }
            catch { }
            return result;
        }

        public List<string> GetServerSchedule()
        {
            var result = new List<string>();

            try
            {
                string[] lines = File.ReadAllLines(".\\server_schedule.txt");

                for (int i = 0; i < lines.Length; i++)
                {
                    result.Add(lines[i]);
                }
            }
            catch { }
            return result;
        }
        //-----------------------------------------------------------------------------
    }
}
