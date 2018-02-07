using AT_BC.Data;
using CO_IA.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Windows;

namespace CO_IA.UI.PersonSchedule
{
    /// <summary>
    /// XLS导入
    /// </summary>
    public class ExcelImportHelper
    {
        /// <summary>
        /// 从Excel中加载表
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public static DataTable LoadDataFromExcel(string FileName)
        {
            string SheetName = string.Empty;
            string ConnStr;
            ConnStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\"" + FileName + "\";Extended Properties=\"Excel 12.0;HDR=YES\"";

            OleDbConnection conn_excel = new OleDbConnection();
            conn_excel.ConnectionString = ConnStr;
            conn_excel.Open();
            //读取Excel的sheetNames
            DataTable dt = conn_excel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
            conn_excel.Close();

            SheetName = dt.Rows[0]["Table_Name"].ToString();
            OleDbDataAdapter da_excel = new OleDbDataAdapter("Select * From [" + SheetName + "]", conn_excel);
            DataTable dt_excel = new DataTable();
            dt_excel.TableName = SheetName.TrimEnd('$');
            da_excel.Fill(dt_excel);

            return dt_excel;
        }

        
    }
}
