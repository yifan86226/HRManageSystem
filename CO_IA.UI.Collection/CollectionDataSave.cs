using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;
using System.Collections;
using CO_IA.UI.Collection.Chart;
using CO_IA.UI.Collection.Model;
using System.Collections.ObjectModel;
using CO_IA.Data.Portable.Collection;

namespace CO_IA.UI.Collection
{
    public delegate void SaveAanalysisDataDelegate(string fileAddr);
    public class CollectionDataSave
    {
        Boolean testTableExists = false;

        public SQLiteConnection con = null;

        string sqliteAddr = "";

        string fileAddr = "";

        /// <summary>
        /// 当前数据库连接状态
        /// </summary>
        private bool _isConnectionOpen;

        public bool IsConnectionOpen
        {
            get { return _isConnectionOpen; }
            set { _isConnectionOpen = value; }
        }

        public CollectionDataSave() 
        {
            
        }

        /// <summary>
        /// 打开SQLITE连接
        /// </summary>
        public void openSQLiteConnection() 
        {
            sqliteAddr = Application.StartupPath + "/SqliteData/freqCollectionData.db";
            con = new SQLiteConnection("Data Source= " + sqliteAddr + ";Version=3;");
            con.Open();
            _isConnectionOpen = true;
        }

        /// <summary>
        /// 关闭SQLITE连接
        /// </summary>
        public void closeSQLiteConnection() 
        {
            con.Close();
            _isConnectionOpen = false;
        }

        /// <summary>
        /// 创建SQLITE表
        /// </summary>
        public void createTable() 
        {
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = con;
            cmd.CommandText = "SELECT * FROM sqlite_master WHERE type='table' and name='FreqCollectionIndex'";
            using (SQLiteDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                {
                    testTableExists = true;
                }
            }
            if (!testTableExists)
            {
                string sql = "CREATE TABLE FreqCollectionIndex (FreqMeasureIndex INTEGER PRIMARY KEY AUTOINCREMENT, StartTime DATETIME, EndTime DATETIME,MeasureID nvarchar(32), StartFreq REAL, EndFreq REAL, Step REAL,FileAddr nvarchar(200),CurrentActivityGuid nvarchar(32),CurrentActivityName nvarchar(100),CurrentActivityPlaceGuid nvarchar(32),CurrentActivityPlaceName nvarchar(100))";
                SQLiteCommand command = new SQLiteCommand(sql, con);
                command.ExecuteNonQuery();

                string sqlStatic = "CREATE TABLE FreqAnalysis (AnalysisID INTEGER PRIMARY KEY AUTOINCREMENT, MeasureID nvarchar(32), AnalysisFileAddr nvarchar(200))";
                SQLiteCommand commandStatic = new SQLiteCommand(sqlStatic, con);
                commandStatic.ExecuteNonQuery();

                string sqlFreqCount = "CREATE TABLE FreqCount (MeasureID nvarchar(32) PRIMARY KEY, FreqNumber INTEGER)";
                SQLiteCommand commandFreqCount = new SQLiteCommand(sqlFreqCount, con);
                commandFreqCount.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 保存采集数据索引
        /// </summary>
        /// <param name="fldi"></param>
        public void saveSqliteIndex(FreqLineDataItem fldi) 
        {
            DateTime startTime = DateTime.Now;
            DateTime endTime = startTime.AddMinutes(15);
            fileAddr = "freqData" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".dat";
            string sqls = string.Format("INSERT INTO FreqCollectionIndex(StartTime, EndTime,MeasureID,StartFreq,EndFreq,Step,FileAddr,CurrentActivityGuid,CurrentActivityName,CurrentActivityPlaceGuid,CurrentActivityPlaceName) VALUES ('{0}','{1}','{2}',{3},{4},{5},'{6}','{7}','{8}','{9}','{10}')", startTime.ToString("s"), endTime.ToString("s"), MeasureID, fldi.TestFreqStart, fldi.TestFreqEnd, fldi.frequencyStep, fileAddr, LoginService.CurrentActivity.Guid, LoginService.CurrentActivity.Name, LoginService.CurrentActivityPlace.Guid, LoginService.CurrentActivityPlace.Name);
            SQLiteCommand commands = new SQLiteCommand(sqls, con);
            commands.ExecuteNonQuery();
            //con.Close();
        }

        public void updateSqliteIndex()
        {
            DateTime startTime = DateTime.Now;
            string sqls = string.Format("UPDATE FreqCollectionIndex SET EndTime = '{0}' WHERE MeasureID = '{1}'", startTime.ToString("s"), MeasureID);
            SQLiteCommand commands = new SQLiteCommand(sqls, con);
            commands.ExecuteNonQuery();
        }

        /// <summary>
        /// 保存分析数据索引
        /// </summary>
        /// <param name="fileAddr"></param>
        public void saveAnalysisTable(string fileAddr)
        {
            string sqls = string.Format("INSERT INTO FreqAnalysis(MeasureID, AnalysisFileAddr) VALUES ('{0}','{1}')", MeasureID, fileAddr);
            SQLiteCommand commands = new SQLiteCommand(sqls, con);
            commands.ExecuteNonQuery();
            //con.Close();
        }

        /// <summary>
        /// 保存采集数据点数
        /// </summary>
        /// <param name="fileAddr"></param>
        public void saveFreqCount(int  freqCount)
        {
            string sqls = string.Format("INSERT INTO FreqCount(MeasureID, FreqNumber) VALUES ('{0}',{1})", MeasureID, freqCount);
            SQLiteCommand commands = new SQLiteCommand(sqls, con);
            commands.ExecuteNonQuery();
            //con.Close();
        }

        /// <summary>
        /// 是否有已经生成的分析数据
        /// </summary>
        /// <param name="MeasureID"></param>
        /// <returns></returns>
        public string IsHaveAnalysisData(string MeasureID) 
        {
            string fileAddr = "";
            SQLiteCommand commands = new SQLiteCommand();
            commands.Connection = con;
            commands.CommandText = "SELECT * FROM FreqAnalysis WHERE MeasureID='" + MeasureID + "'";
            SQLiteDataReader reader = commands.ExecuteReader();
            while (reader.Read())
            {
                fileAddr = Application.StartupPath + "/SqliteData/" + reader["AnalysisFileAddr"].ToString();
            }
            return fileAddr;
        }

        private int flag = 0;

        public int Flag
        {
            get { return flag; }
            set { flag = value; }
        }

        /// <summary>
        /// 保存采集数据
        /// </summary>
        /// <param name="freqData"></param>
        /// <param name="fldi"></param>
        public void saveFreqData(float[] freqData, FreqLineDataItem fldi) 
        {
            if (flag == 0) 
            {
                saveSqliteIndex(fldi);
                flag++;
            }
            MemoryStream _stream = new MemoryStream();
            for (int i = 0; i < freqData.Length; i++) 
            {
                _stream.Write(BitConverter.GetBytes(freqData[i]), 0, sizeof(float));
            }
            byte[] myByte = _stream.ToArray();
            //string msg = freqData.ToArray().ToString();
            string path = Application.StartupPath + "/SqliteData/" + fileAddr;
            using (FileStream fsWrite = new FileStream(path, FileMode.Append))
            {
                fsWrite.Write(myByte, 0, myByte.Length);
            }; 
        }

        /// <summary>
        /// 获取数据库所有原始文件路径
        /// </summary>
        /// <returns></returns>
        public ArrayList getFileNames() 
        {
            ArrayList tempList = new ArrayList();
            string sql = "select * from FreqCollectionIndex where MeasureID='" + MeasureID + "'  order by StartTime";
            SQLiteCommand commands = new SQLiteCommand(sql, con);
            SQLiteDataReader reader = commands.ExecuteReader();
            while (reader.Read())
            {
                FreqCollectionIndex fci = new FreqCollectionIndex();
                fci.StartFreq = Convert.ToDouble(reader["StartFreq"])*1000;
                fci.EndFreq = Convert.ToDouble(reader["EndFreq"])*1000;
                fci.FreqStep = Convert.ToDouble(reader["Step"]);
                fci.FileAddr = Application.StartupPath + "/SqliteData/" + reader["FileAddr"];
                //tempList.Add(Application.StartupPath + "/SqliteData/" + reader["FileAddr"]);
                tempList.Add(fci);
            }
            return tempList;
        }

        /// <summary>
        /// 获取所有采集ID
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<FreqCollectionIndex> getMeasureIds()
        {
            ObservableCollection<FreqCollectionIndex> tempList = new ObservableCollection<FreqCollectionIndex>();
            string sql = "select DISTINCT MeasureID,StartFreq,EndFreq,CurrentActivityPlaceName,StartTime,EndTime from FreqCollectionIndex order by StartTime desc";
            SQLiteCommand commands = new SQLiteCommand(sql, con);
            SQLiteDataReader reader = commands.ExecuteReader();
            while (reader.Read())
            {
                FreqCollectionIndex fci = new FreqCollectionIndex();
                fci.StartFreq = Convert.ToDouble(reader["StartFreq"]);
                fci.EndFreq = Convert.ToDouble(reader["EndFreq"]);
                fci.MeasureID =reader["MeasureID"].ToString();
                fci.CurrentActivityPlaceName = reader["CurrentActivityPlaceName"].ToString();
                fci.DisplayMem = fci.MeasureID.Substring(7, fci.MeasureID.Length - 7);
                DateTime dt = DateTime.ParseExact(fci.DisplayMem, "yyyyMMddHHmmss", null);
                fci.DisplayMem = dt.ToString("M月d h时m分s秒");
                DateTime.TryParse(reader["StartTime"].ToString(), out dt);
                fci.StartTime = dt;
                DateTime.TryParse(reader["EndTime"].ToString(), out dt);
                fci.EndTime = dt;
                tempList.Add(fci);
            }
            return tempList;
        }

        public int getFreqCountByMeasureId(string MeasureId) 
        {
            int freqCount = 0;
            string sql = "select FreqNumber from FreqCount where MeasureID ='" + MeasureId + "'";
            SQLiteCommand commands = new SQLiteCommand(sql, con);
            SQLiteDataReader reader = commands.ExecuteReader();
            while (reader.Read())
            {
                freqCount = Convert.ToInt32(reader["FreqNumber"]);
            }
            return freqCount;
        }

        /// <summary>
        /// 删除采集序列
        /// </summary>
        /// <param name="MeasureId"></param>
        /// <returns></returns>
        public int deleteByMeasureId(string MeasureId)
        {
            int result = 0;
            string sql1 = "delete from FreqCount where MeasureID ='" + MeasureId + "'";
            string sql2 = "delete from FreqAnalysis where MeasureID ='" + MeasureId + "'";
            string sql3 = "delete from FreqCollectionIndex where MeasureID ='" + MeasureId + "'";
            SQLiteCommand commands = new SQLiteCommand(sql1, con);
            result = commands.ExecuteNonQuery();

            SQLiteCommand commands2 = new SQLiteCommand(sql2, con);
            result = commands2.ExecuteNonQuery();

            SQLiteCommand commands3 = new SQLiteCommand(sql3, con);
            result = commands3.ExecuteNonQuery();

            return result;
        }

        /// <summary>
        /// 当前采集ID
        /// </summary>
        private string _measureID;

        public string MeasureID
        {
            get { return _measureID; }
            set { _measureID = value; }
        }
    }
}
