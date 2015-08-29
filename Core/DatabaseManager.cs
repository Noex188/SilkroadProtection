using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace sroprot.Core
{
    public sealed class DatabaseManager
    {
        //-----------------------------------------------------------------------------
        SqlConnection m_connection = null;

        object m_class_lock = new object();

        static DatabaseManager m_instance = null;

        private DatabaseManager() { }

        public static DatabaseManager getInstance()
        {
            if (m_instance == null)
            {
                m_instance = new DatabaseManager();
            }
            return m_instance;
        }


        public string InjectionsFilter(string str)
        {
            if (str.Contains("'"))
            {
                Global.logmgr.WriteLog(LogLevel.Warning, "Sql Injection detected");
                str = str.Replace("'", "''");
            }
            if (str.Contains("\""))
            {
                Global.logmgr.WriteLog(LogLevel.Warning, "Sql Injection detected");
                str = str.Replace("\"", "\"\"");
            }
            return str;
        }
        //-----------------------------------------------------------------------------

        public bool OpenConnection(string connstr, params object[] args)
        {
            lock (m_class_lock)
            {
                //try to close existing connection ?
                if (m_connection != null)
                {
                    try
                    {
                        m_connection.Close();
                    }
                    catch { }
                }

                m_connection = new SqlConnection();
                m_connection.ConnectionString = string.Format(connstr, args);

                try
                {
                    m_connection.Open();
                }
                catch (SqlException)
                {
                    return false;
                }
                catch (InvalidOperationException)
                {
                    return false;
                }
            }
            return true;
        }

        //-----------------------------------------------------------------------------

        public void CloseConnection()
        {
            lock (m_class_lock)
            {
                try
                {
                    m_connection.Close();
                }
                catch (SqlException)
                {
                }
            }
        }



        //-----------------------------------------------------------------------------

        public bool ExecuteNonQuery(string cmd, params object[] args)
        {
            lock (m_class_lock)
            {
                if (IsConnected())
                {
                    try
                    {
                        SqlCommand sqlcmd = new SqlCommand(string.Format(cmd, args), m_connection);
                        sqlcmd.ExecuteNonQuery();
                        return true;
                    }
                    catch (SqlException)
                    {
                    }
                }
            }
            return false;
        }

        //-----------------------------------------------------------------------------

        public List<object> ReadSingleRow(string cmd, params object[] args)
        {
            List<object> result = null;
            lock (m_class_lock)
            {
                if (IsConnected())
                {
                    result = new List<object>();

                    try
                    {
                        SqlCommand sqlcmd = new SqlCommand(string.Format(cmd, args), m_connection);
                        SqlDataReader reader = sqlcmd.ExecuteReader();

                        bool could_read = reader.Read();
                        if (!could_read)
                        {
                            reader.Close();
                            return null;
                        }

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            result.Add(reader[i]);
                        }

                        reader.Close();
                    }
                    catch (SqlException)
                    {
                        return null;
                    }

                }
            }
            return result;
        }

        //-----------------------------------------------------------------------------

        public List<KeyValuePair<int, List<object>>> ReadAllRows(string cmd, params object[] args)
        {
            List<KeyValuePair<int, List<object>>> result = new List<KeyValuePair<int, List<object>>>();
            lock (m_class_lock)
            {
                if (IsConnected())
                {
                    try
                    {
                        SqlCommand sqlcmd = new SqlCommand(string.Format(cmd, args), m_connection);
                        SqlDataReader reader = sqlcmd.ExecuteReader();

                        int nRow = 0;
                        KeyValuePair<int, List<object>> item;
                        List<object> tmp;

                        while (reader.Read())
                        {
                            //read row

                            tmp = new List<object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                tmp.Add(reader[i]);
                            }

                            item = new KeyValuePair<int, List<object>>(nRow, tmp);

                            result.Add(item);
                            nRow++;
                        }

                        reader.Close();
                    }
                    catch (SqlException)
                    {
                        return null;
                    }

                }
                if (result != null)
                    if (result.Count == 0)
                        result = null;
            }
            return result;
        }

        //-----------------------------------------------------------------------------

        public bool IsConnected()
        {
            lock (m_class_lock)
            {
                if (m_connection != null)
                {
                    return (m_connection.State == ConnectionState.Open);
                }
            }
            return false;
        }



        //-----------------------------------------------------------------------------

        //Необходимые для RelaySession методы
        public List<string> GetJidAndToken(string uname)
        {
            checkConnected();
            uname = InjectionsFilter(uname);
            List<string> res = new List<string>();
            lock (m_class_lock)
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("_web_mall_token", m_connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@StrUserID", SqlDbType.VarChar).Value = uname;

                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    int jid = int.Parse(reader[0].ToString());
                    string token = reader[1].ToString();
                    reader.Close();
                    res.Add(jid.ToString());

                    res.Add(token);
                }
                catch
                {
                    Global.logmgr.WriteLog(LogLevel.Error, "_web_mall_token exec error");
                }
            }
            return res;
        }

        /// <summary>
        /// Gets user silk data from DB
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>Silk data (3 args as list of ints)</returns>
        public List<int> GetSilkDataByUsername(string username)
        {
            checkConnected();
            username = InjectionsFilter(username);
            List<int> res = new List<int>();

            lock (m_class_lock)
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("_GetSilk", m_connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    //test
                    cmd.Parameters.Add("@username", SqlDbType.VarChar).Value = username;

                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    int silk1 = int.Parse(reader[0].ToString());
                    int silk2 = int.Parse(reader[1].ToString());
                    int silk3 = int.Parse(reader[2].ToString());

                    reader.Close();
                    res.Add(silk1);
                    res.Add(silk2);
                    res.Add(silk3);

                }
                catch
                {
                    Global.logmgr.WriteLog(LogLevel.Error, "_Getsilk exec error");
                }
            }
            return res;
        }

        /// <summary>
        /// Gets item mall buy item result from DB
        /// </summary>
        /// <param name="charname">Charname</param>
        /// <param name="pckg_code">Package item codename</param>
        /// <param name="count">Item count</param>
        /// <returns>-1 if error, positive value on success</returns>
        public int GetBuyMallResult(string charname, string pckg_code, int count)
        {
            checkConnected();
            charname = InjectionsFilter(charname);
            int res = -1;

            lock (m_class_lock)
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("_BUY_INGAME_MALL_ITEM", m_connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@CharName", SqlDbType.NVarChar).Value = charname;
                    cmd.Parameters.Add("@package_code", SqlDbType.VarChar).Value = pckg_code;
                    cmd.Parameters.Add("@Anount", SqlDbType.Int).Value = count;

                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();

                    int nResult = int.Parse(reader[0].ToString());
                    if (nResult <= 0 || nResult == 255)
                    {
                        res = -1;
                    }
                    else
                    {
                        res = nResult;
                    }
                    reader.Close();
                }
                catch
                {
                    Global.logmgr.WriteLog(LogLevel.Notify, "_BUY_INGAME_MALL_ITEM exec error");
                }
            }
            return res;
        }

        public int checkGmAccessControl(string username, uint cmd_id)
        {
            checkConnected();
            username = InjectionsFilter(username);
            int result = 0;

            lock (m_class_lock)
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("_AnticheatCheckGmAccessControl", m_connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@username", SqlDbType.VarChar).Value = username;
                    cmd.Parameters.Add("@cmd", SqlDbType.Int).Value = cmd_id;
                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();

                    result = int.Parse(reader[0].ToString());
                    reader.Close();
                }
                catch
                {
                    Global.logmgr.WriteLog(LogLevel.Notify, "_AnticheatCheckGmAccessControl exec error");
                }
            }
            return result;
        }

        public int checkGmObjAccessControl(string username, uint obg_id, uint amount)
        {
            checkConnected();
            username = InjectionsFilter(username);
            int result = 0;

            lock (m_class_lock)
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("_AnticheatCheckGmAccessObjID", m_connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@username", SqlDbType.VarChar).Value = username;
                    cmd.Parameters.Add("@objID", SqlDbType.Int).Value = obg_id;
                    cmd.Parameters.Add("@Amount", SqlDbType.Int).Value = amount;
                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();

                    result = int.Parse(reader[0].ToString());
                    reader.Close();
                }
                catch
                {
                    Global.logmgr.WriteLog(LogLevel.Notify, "_AnticheatCheckGmAccessObjID exec error");
                }
            }
            return result;
        }

        public void AnticheatAuthLog(string username, string ip)
        {

            checkConnected();
            username = InjectionsFilter(username);
            lock (m_class_lock)
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("_AnticheatAuthLog", m_connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@username", SqlDbType.VarChar).Value = username;
                    cmd.Parameters.Add("@ip", SqlDbType.VarChar).Value = ip;

                    cmd.ExecuteNonQuery();


                }
                catch (Exception e)
                {
                    Global.logmgr.WriteLog(LogLevel.Notify, "_AnticheatAuthLog exec error");
                    Console.WriteLine(e);
                }
            }
        }


        public void AnticheatArenaStatusNotify(string name, int status)
        {
            checkConnected();
            name = InjectionsFilter(name);

            lock (m_class_lock)
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("_AnticheatArenaStatusNotify", m_connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@charname", SqlDbType.NVarChar).Value = name;
                    cmd.Parameters.Add("@status", SqlDbType.Int).Value = status;
                    cmd.ExecuteNonQuery();
                }
                catch
                {
                    Global.logmgr.WriteLog(LogLevel.Notify, "_AnticheatArenaStatusNotify exec error");
                }
            }
        }


        public int AnticheatCheckTeleportAccess(string name, int teleport)
        {
            checkConnected();
            name = InjectionsFilter(name);
            int result = 0;
            lock (m_class_lock)
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("_AnticheatCheckTeleportAccess", m_connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@charname", SqlDbType.NVarChar).Value = name;
                    cmd.Parameters.Add("@teleport", SqlDbType.Int).Value = teleport;
                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    result = int.Parse(reader[0].ToString());
                    reader.Close();
                }
                catch
                {
                    Global.logmgr.WriteLog(LogLevel.Notify, "_AnticheatCheckTeleportAccess exec error");
                }
            }
            return result;
        }

        public void UniqueDeathNotify(string name, int id)
        {

            checkConnected();
            name = InjectionsFilter(name);
            lock (m_class_lock)
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("_AnticheatUniqueDeathNotify", m_connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Global.logmgr.WriteLog(LogLevel.Notify, "_AnticheatUniqueDathNotify exec error");
                }
            }
        }


        public int getCharLvl(string name)
        {

            checkConnected();
            name = InjectionsFilter(name);
            int result = 0;

            lock (m_class_lock)
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("_AnticheatGetCharSata", m_connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@name", SqlDbType.NChar).Value = name;
                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    result = int.Parse(reader[0].ToString());
                    reader.Close();
                }
                catch
                {
                    Global.logmgr.WriteLog(LogLevel.Notify, "_AnticheatGetCharSata exec error");
                }
            }
            return result;
        }

        public int checkOptLevel(string name,int slot,int max_opt)
        {
            checkConnected();
            name = InjectionsFilter(name);
            int result = 0;

            lock (m_class_lock)
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("_AnticheatChekItemOpt", m_connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@name", SqlDbType.NChar).Value = name;
                    cmd.Parameters.Add("@slot", SqlDbType.Int).Value = slot;
                    cmd.Parameters.Add("@max", SqlDbType.Int).Value = max_opt;
                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    result = int.Parse(reader[0].ToString());
                    reader.Close();
                }
                catch
                {
                    Global.logmgr.WriteLog(LogLevel.Notify, "_AnticheatChekItemOpt exec error");
                }
            }
            return result;
        }

        public int GuildMembers(string name, int type, int max)
        {
            checkConnected();
            name = InjectionsFilter(name);
            int result = 0;

            lock (m_class_lock)
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("_AnticheatGuild", m_connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@name", SqlDbType.NChar).Value = name;
                    cmd.Parameters.Add("@type", SqlDbType.Int).Value = type;
                    cmd.Parameters.Add("@max", SqlDbType.Int).Value = max;
                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    result = int.Parse(reader[0].ToString());
                    reader.Close();
                }
                catch
                {
                    Global.logmgr.WriteLog(LogLevel.Notify, "_AnticheatGuild exec error");
                }
            }
            return result;
        }




        public void checkConnected()
        {
            lock (m_class_lock)
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("_AnticheatBicycle", m_connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
                catch
                {
                    if (!IsConnected())
                    {
                        bool conn_res = Global.dbmgr.OpenConnection(Global.sql_str);
                        if (!conn_res)
                        {
                            Global.logmgr.WriteLog(LogLevel.Error, "Database connection failed");

                        }
                    }
                }
            }
        }

        //-----------------------------------------------------------------------------
    }
}
