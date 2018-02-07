#region 文件描述
/*************************************************************************
 * 创建人：王若兴
 * 摘  要：SQLite数据库连接和操作
 * 日  期：2016-09-08
 * ***********************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CO_IA.UI.Collection.DbEntity
{
    public class SQLiteConnect
    {
        static SQLiteConnection m_dbConnection;
        //private static string _dbFileName = "RIAS_EME_6100";
        private static string _dbFileName = string.Empty;
        public static string DbFileName
        {
            get { return _dbFileName; }
            set 
            {
                if (value.EndsWith(".db"))
                {
                    value = value.Substring(0, value.Length - 3);
                }
                _dbFileName = value; 
            }
        }

        //创建一个空的数据库
        void createNewDatabase()
        {
            SQLiteConnection.CreateFile("MyDatabase.sqlite");
        }
        //在指定数据库中创建一个table

        void createTable()
        {
            string sql = "create table highscores (name varchar(20), score int)";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
        }

        private static SQLiteParameterCollection AttachParameters(SQLiteCommand cmd, string commandText, params  object[] paramList)
        {
            if (paramList == null || paramList.Length == 0) return null;

            SQLiteParameterCollection coll = cmd.Parameters;
            string parmString = commandText.Substring(commandText.IndexOf("@"));
            // pre-process the string so always at least 1 space after a comma.
            parmString = parmString.Replace(",", " ,");
            // get the named parameters into a match collection
            string pattern = @"(@)\S*(.*?)\b";
            Regex ex = new Regex(pattern, RegexOptions.IgnoreCase);
            MatchCollection mc = ex.Matches(parmString);
            string[] paramNames = new string[mc.Count];
            int i = 0;
            foreach (Match m in mc)
            {
                paramNames[i] = m.Value;
                i++;
            }

            // now let's type the parameters
            int j = 0;
            Type t = null;
            foreach (object o in paramList)
            {
                t = o.GetType();

                SQLiteParameter parm = new SQLiteParameter();
                switch (t.ToString())
                {

                    case ("DBNull"):
                    case ("Char"):
                    case ("SByte"):
                    case ("UInt16"):
                    case ("UInt32"):
                    case ("UInt64"):
                        throw new SystemException("Invalid data type");


                    case ("System.String"):
                        parm.DbType = DbType.String;
                        parm.ParameterName = paramNames[j];
                        parm.Value = (string)paramList[j];
                        coll.Add(parm);
                        break;

                    case ("System.Byte[]"):
                        parm.DbType = DbType.Binary;
                        parm.ParameterName = paramNames[j];
                        parm.Value = (byte[])paramList[j];
                        coll.Add(parm);
                        break;

                    case ("System.Int32"):
                        parm.DbType = DbType.Int32;
                        parm.ParameterName = paramNames[j];
                        parm.Value = (int)paramList[j];
                        coll.Add(parm);
                        break;

                    case ("System.Boolean"):
                        parm.DbType = DbType.Boolean;
                        parm.ParameterName = paramNames[j];
                        parm.Value = (bool)paramList[j];
                        coll.Add(parm);
                        break;

                    case ("System.DateTime"):
                        parm.DbType = DbType.DateTime;
                        parm.ParameterName = paramNames[j];
                        parm.Value = Convert.ToDateTime(paramList[j]);
                        coll.Add(parm);
                        break;

                    case ("System.Double"):
                        parm.DbType = DbType.Double;
                        parm.ParameterName = paramNames[j];
                        parm.Value = Convert.ToDouble(paramList[j]);
                        coll.Add(parm);
                        break;

                    case ("System.Decimal"):
                        parm.DbType = DbType.Decimal;
                        parm.ParameterName = paramNames[j];
                        parm.Value = Convert.ToDecimal(paramList[j]);
                        break;

                    case ("System.Guid"):
                        parm.DbType = DbType.Guid;
                        parm.ParameterName = paramNames[j];
                        parm.Value = (System.Guid)(paramList[j]);
                        break;

                    case ("System.Object"):

                        parm.DbType = DbType.Object;
                        parm.ParameterName = paramNames[j];
                        parm.Value = paramList[j];
                        coll.Add(parm);
                        break;

                    default:
                        throw new SystemException("Value is of unknown data type");

                } // end switch

                j++;
            }
            return coll;
        }

        public static SQLiteParameter CreateParameter(string parameterName, System.Data.DbType parameterType, object parameterValue)
        {
            SQLiteParameter parameter = new SQLiteParameter();
            parameter.DbType = parameterType;
            parameter.ParameterName = parameterName;
            parameter.Value = parameterValue;
            return parameter;
        }

        public static DataSet ExecuteDataSet(string connectionString, string commandText, object[] paramList)
        {
            SQLiteConnection cn = new SQLiteConnection(connectionString);
            SQLiteCommand cmd = cn.CreateCommand();


            cmd.CommandText = commandText;
            if (paramList != null)
            {
                AttachParameters(cmd, commandText, paramList);
            }
            DataSet ds = new DataSet();
            if (cn.State == ConnectionState.Closed)
                cn.Open();
            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
            da.Fill(ds);
            da.Dispose();
            cmd.Dispose();
            cn.Close();
            return ds;
        }

        public static DataSet ExecuteDataSet(SQLiteConnection cn, string commandText, object[] paramList)
        {
            DataSet ds = new DataSet();
            try
            {
                SQLiteCommand cmd = cn.CreateCommand();
                cmd.CommandText = commandText;
                if (paramList != null)
                {
                    AttachParameters(cmd, commandText, paramList);
                }

                if (cn.State == ConnectionState.Closed)
                    cn.Open();
                SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
                da.Fill(ds);
                da.Dispose();
                cmd.Dispose();
                cn.Close();
            }
            catch
            {

            }
            return ds;
        }
        /// <summary>
        /// 不关连接
        /// </summary>
        /// <param name="cn"></param>
        /// <param name="commandText"></param>
        /// <param name="paramList"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet2(SQLiteConnection cn, string commandText, object[] paramList)
        {
            DataSet ds = new DataSet();
            try
            {
                SQLiteCommand cmd = cn.CreateCommand();
                cmd.CommandText = commandText;
                if (paramList != null)
                {
                    AttachParameters(cmd, commandText, paramList);
                }

                if (cn.State == ConnectionState.Closed)
                    cn.Open();
                SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
                da.Fill(ds);
                da.Dispose();
                cmd.Dispose();   
            }
           catch
            {

            }
            return ds;
        }
        //public static int ExecuteNonQuery(string connectionString, string commandText, params object[] paramList)
        //{

        //    SQLiteConnection cn = new SQLiteConnection(connectionString);
        //    SQLiteCommand cmd = cn.CreateCommand();
        //    cmd.CommandText = commandText;
        //    AttachParameters(cmd, commandText, paramList);
        //    if (cn.State == ConnectionState.Closed)
        //        cn.Open();
        //    int result = cmd.ExecuteNonQuery();
        //    cmd.Dispose();
        //    cn.Close();

        //    return result;
        //}
        
        //public static int ExecuteNonQuery(SQLiteConnection cn, string commandText, params  object[] paramList)
        //{
        //    SQLiteCommand cmd = cn.CreateCommand();
        //    cmd.CommandText = commandText;
        //    AttachParameters(cmd, commandText, paramList);
        //    if (cn.State == ConnectionState.Closed)
        //        cn.Open();
        //    int result = cmd.ExecuteNonQuery();
        //    cmd.Dispose();
        //    cn.Close();

        //    return result;
        //}

        public static int ExecuteNonQuery(SQLiteTransaction transaction, string commandText, params  object[] paramList)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rolled back or committed,                                                        please provide an open transaction.", "transaction");
            IDbCommand cmd = transaction.Connection.CreateCommand();
            cmd.CommandText = commandText;
            //AttachParameters((SQLiteCommand)cmd, cmd.CommandText, paramList);
            if (paramList != null && paramList.Length > 0)
            {
                ((SQLiteCommand)cmd).Parameters.AddRange(paramList);
            }
            if (transaction.Connection.State == ConnectionState.Closed)
                transaction.Connection.Open();
            int result = cmd.ExecuteNonQuery();
            cmd.Dispose();
            return result;
        }
        /// <summary>
        /// 创建一个事务
        /// </summary>
        /// <param name="p_dbFileName">操作的SQLite数据库名称，也就是当前 活动名称</param>
        /// <returns></returns>
        public static SQLiteTransaction GetSQLiteTransaction(string p_dbFileName)
        {
            DbFileName = p_dbFileName;
            //if (string.IsNullOrEmpty(p_dbFileName))
            //{
            //    _dbFileName = "RIAS_EME_EMPTY";
            //}
            SQLiteConnection conn = ConnectToDatabase(p_dbFileName);
            SQLiteTransaction trans = conn.BeginTransaction();
            
            return trans;
        }
       
        /// <summary>
        /// 创建一个连接到指定数据库 
        /// </summary>
        /// <param name="p_dbFileName">操作的SQLite数据库名称，也就是当前 活动名称</param>
        /// <returns></returns>
        public static SQLiteConnection ConnectToDatabase(string p_dbFileName)
        {
            DbFileName = p_dbFileName;
            //if (string.IsNullOrEmpty(p_dbFileName))
            //{
            //    _dbFileName = "RIAS_EME_EMPTY";
            //}
            string fileDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + string.Format("SqliteData\\Activities\\{0}.db", _dbFileName);
            m_dbConnection = new SQLiteConnection(string.Format("Data Source={0}", fileDirectory));
            m_dbConnection.Open();
            return m_dbConnection;
        }

        public static void CopyDBFile(string destFileName)
        {
            //string strPath = ConfigurationManager.AppSettings["Path"]; 

            string sourceFile = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "SqliteData\\Activities\\RIAS_EME_EMPTY.db";
            string destFile = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + string.Format("SqliteData\\Activities\\{0}.db", destFileName);
            System.IO.File.Copy(sourceFile, destFile, true);
        }

        public static List<string> GetFileNames()
        {
            List<string> fileNameList = new List<string>();
            string sourcePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "SqliteData\\Activities";
            DirectoryInfo TheFolder = new DirectoryInfo(sourcePath);
            foreach (FileInfo NextFile in TheFolder.GetFiles())
            {
                if (NextFile.Length == 0 || NextFile.Name.Contains("db-journal"))
                {
                    try
                    {
                        File.Delete(sourcePath + "\\" + NextFile.Name);
                        continue;
                    }
                    catch
                    {
                        continue;
                    }
                }
                if(NextFile.Name.ToUpper() != "RIAS_EME_EMPTY.DB")
                fileNameList.Add(NextFile.Name);
            }
            return fileNameList;
        }
    }
}
