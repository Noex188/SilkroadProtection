using System;
using System.Collections;
using System.Collections.Generic;


namespace sroprot.NetEngine
{

    public sealed class RelaySessionState
    {
        Hashtable m_settings;
        public int SettingCount { get { return m_settings.Count; } }

        DateTime m_startup_time;
        public DateTime StartupTime { get { return m_startup_time; } }


        public RelaySessionState()
        {
            m_settings = new Hashtable();
            m_startup_time = DateTime.Now;

        }

        //Returns string with 0 length if no parameter found
        public object this[object cfg]
        {
            get
            {
                if (m_settings.ContainsKey(cfg))
                    return m_settings[cfg];
                return "";
            }
            set
            {
                m_settings[cfg] = value.ToString();

            }
        }

        public string Dump()
        {
            string res = string.Empty;
            res += string.Format("StartupTime = {0}\r\n", m_startup_time);

            foreach(DictionaryEntry item in m_settings)
            {
                res += string.Format("{0} = {1}\r\n", item.Key, item.Value);
            }

            return res;
        }
    }
}