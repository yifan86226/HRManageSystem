using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CO_IA.UI.Collection.helper
{
    public class DbHelper
    {
        /// <summary>
        /// 创建频率数据表
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool CreateFreqTable(string FreqTableName)
        {
            try
            {
#if !DEBUG
                EntityCommand ecmd = new EntityCommand(new FreqDataTemplate(), new SqlServer());
                Console.WriteLine(ecmd.CreateTableCommand);
                string sql = ecmd.CreateTableCommand.Replace("FreqDataTemplate", FreqTableName);
                int res = MyDB.Instance.ExecuteNonQuery(sql);
#endif

                return true;
                //MyDB.Instance.ExecuteNonQuery(string.Format(DbHelper.DROP_FREQ_TABLE, "2016_10_1"));
            }
            catch (Exception ex)
            {
                return false;
            }
        }




        /// <summary>
        /// 创建cdma1x数据表
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool CreateCdma1xTable(string FreqTableName)
        {
            try
            {
#if !DEBUG
                EntityCommand ecmd = new EntityCommand(new Decoder_CDMA2G_Data_Template(), new SqlServer());
                Console.WriteLine(ecmd.CreateTableCommand);
                string sql = ecmd.CreateTableCommand.Replace("Decoder_CDMA2G_Data_Template", FreqTableName + preCDMA2GTableName);
                int res = MyDB.Instance.ExecuteNonQuery(sql);
#endif
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static string preCDMA2GTableName = "_CDMA2G";
    }
}
