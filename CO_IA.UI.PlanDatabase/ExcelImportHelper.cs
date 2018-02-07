using AT_BC.Data;
using CO_IA.Data;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace CO_IA.UI.PlanDatabase
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
        public static DataTable[] LoadDataFromExcel(string FileName)
        {
            string SheetName = string.Empty;
            string ConnStr;
            //ConnStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=\"" + FileName + "\";Extended Properties=\"Excel 8.0;HDR=YES\"";
            ConnStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\"" + FileName + "\";Extended Properties=\"Excel 12.0;HDR=YES\"";

            OleDbConnection conn_excel = new OleDbConnection();
            conn_excel.ConnectionString = ConnStr;
            conn_excel.Open();
            //读取Excel的sheetNames
            DataTable dt = conn_excel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
            conn_excel.Close();

            DataTable[] dataTables = new DataTable[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                SheetName = dt.Rows[i]["Table_Name"].ToString();
                OleDbDataAdapter da_excel = new OleDbDataAdapter("Select * From [" + SheetName + "]", conn_excel);
                DataTable dt_excel = new DataTable();
                dt_excel.TableName = SheetName.TrimEnd('$');
                da_excel.Fill(dt_excel);
                dataTables[i] = dt_excel;
            }
            return dataTables;
        }

        /// <summary>
        /// 验证单位信息
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static bool ValidateORG(DataTable dt)
        {
            bool isSuccess = true;
            StringBuilder errormsg = new StringBuilder();
            if (dt != null)
            {
                string error = string.Empty;
                int index = 1;

                string orgname = dt.Columns[1].ColumnName;

                if (dt.Columns[1] != null)
                {
                    if (IsNull(dt.Columns[1].ColumnName))
                    {
                        errormsg.AppendFormat("{0}:单位名称不能为空 \r", index);
                        isSuccess = false;
                        index++;
                    }
                    else
                    {
                        if (dt.Columns[1].ColumnName.Length > 100)
                        {
                            errormsg.AppendFormat("{0}:单位名称不能超过100个字符串 \r", index);
                            isSuccess = false;
                            index++;
                        }
                    }
                }
                else
                {
                    errormsg.AppendFormat("{0}:单位名称不能为空 \r", index);
                    isSuccess = false;
                    index++;
                }

                if (IsNull(dt.Rows[0][1].ToString()))
                {
                    errormsg.AppendFormat("{0}:单位简称不能为空 \r", index);
                    isSuccess = false;
                    index++;
                }
                if (dt.Rows[0][1].ToString().Length > 20)
                {
                    errormsg.AppendFormat("{0}:单位简称不能超过20个字符串 \r", index);
                    isSuccess = false;
                    index++;
                }

                if (IsNull(dt.Rows[1][1].ToString()))
                {
                    errormsg.AppendFormat("{0}:保障类别不能为空 \r", index);
                    isSuccess = false;
                    index++;
                }
                if (dt.Rows[1][1].ToString().Length > 20)
                {
                    errormsg.AppendFormat("{0}:保障类别不能超过20个字符串 \r", index);
                    isSuccess = false;
                    index++;
                }

                if (dt.Rows[2][1].ToString().Length > 100)
                {
                    errormsg.AppendFormat("{0}:单位地址不能超过100个字符 \r", index);
                    isSuccess = false;
                    index++;

                }
                if (IsNull(dt.Rows[3][1].ToString()))
                {
                    errormsg.AppendFormat("{0}:联系人不能为空 \r", index);
                    isSuccess = false;
                    index++;

                }
                if (dt.Rows[3][1].ToString().Length > 20)
                {
                    errormsg.AppendFormat("{0}:联系人不能超过20个字符串 \r", index);
                    isSuccess = false;
                    index++;
                }
                if (IsNull(dt.Rows[4][1].ToString()))
                {
                    errormsg.AppendFormat("{0}:联系方式不能为空 \r", index);
                    isSuccess = false;
                    index++;
                }
                if (dt.Rows[4][1].ToString().Length > 20)
                {
                    errormsg.AppendFormat("{0}:联系方式不能超过20个字符串 \r", index);
                    isSuccess = false;
                    index++;
                }
            }
            else
            {
                errormsg.AppendFormat("导入单位信息为空,请先填写单位信息!");
            }
            if (!isSuccess)
            {
                string title = string.Format("单位{0}错误信息如下:\r", dt.Rows[0][1].ToString());
                StringBuilder msg = new StringBuilder();
                msg.Append(title);
                msg.Append(errormsg.ToString());

                ErrorDialog dialog = new ErrorDialog(msg.ToString());
                dialog.ShowDialog();
            }
            return isSuccess;
        }

        /// <summary>
        /// 验证设备
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static bool ValidateEquipmentTables(DataTable[] tables, out StringBuilder errormsg)
        {
            errormsg = new StringBuilder();
            bool result = true;
            DataTable dt = tables.FirstOrDefault(r => r.TableName.Contains("设备信息"));
            List<DataRow> samerows = new List<DataRow>();
            if (dt != null && dt.Rows.Count > .0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow row = dt.Rows[i];
                    object[] content = row.ItemArray;
                    object obj = content.FirstOrDefault(r => !string.IsNullOrWhiteSpace(r.ToString())); //是否存在不为空的项

                    if (obj != null)
                    {
                        string error = string.Empty; ;
                        if (!ValidateEquipment(row, out  error)) //验证是否存在空项
                        {
                            result = false;
                            errormsg.AppendLine("设备错误信息:");

                            if (string.IsNullOrEmpty(row["设备名称"].ToString()))
                            {
                                errormsg.AppendLine(string.Format("设备{0}:", i));
                            }
                            else
                            {
                                errormsg.AppendLine(string.Format("{0}:", row["设备名称"].ToString()));
                            }
                            errormsg.AppendLine(error);
                        }
                        else  //验证名称+编号是否存在重复
                        {
                            bool exist = ExistSameNameAndCode(row, dt);
                            if (exist)
                            {
                                samerows.Add(row);
                                result = false;
                            }
                        }
                    }
                }
                if (samerows.Count > 0)
                {
                    errormsg.AppendLine("Excel中存在相同的设备名称+编号:");
                    foreach (DataRow item in samerows)
                    {
                        errormsg.AppendLine(string.Format("名称:{0},编号:{1}", item["设备名称"], item["设备编号"]));
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 验证设备信息
        /// </summary>
        /// <returns></returns>
        private static bool ValidateEquipment(DataRow row, out string outerror)
        {
            outerror = string.Empty;
            StringBuilder errormsg = new StringBuilder();
            bool isSuccess = true;
            int index = 1;
            string error = string.Empty;

            //名称
            if (IsNull(row["设备名称"].ToString()))
            {
                errormsg.AppendFormat("{0}:设备名称不能为空\r", index);
                index++;
                isSuccess = false;
            }

            //编号
            if (IsNull(row["设备编号"].ToString()))
            {
                errormsg.AppendFormat("{0}:设备编号不能为空\r", index);
                index++;
                isSuccess = false;
            }

            //业务类型
            if (IsNull(row["业务类型"].ToString()))
            {
                errormsg.AppendFormat("{0}:业务类型不能为空\r", index);
                index++;
                isSuccess = false;
            }

            //数量
            if (IsNull(row["数量"].ToString()))
            {
                errormsg.AppendFormat("{0}:设备数量不能为空\r", index);
                index++;
                isSuccess = false;
            }
            else
            {
                if (!IsDouble(row["数量"].ToString()))
                {
                    errormsg.AppendFormat("{0}:设备数量应为数字\r", index);
                    index++;
                    isSuccess = false;
                }
                else
                {
                    if (double.Parse(row["数量"].ToString()) <= 0)
                    {

                    }
                }
            }
            //业务类型
            if (IsNull(row["设备型号"].ToString()))
            {
                errormsg.AppendFormat("{0}:设备型号不能为空\r", index);
                index++;
                isSuccess = false;
            }


            //移动设备
            if (IsNull(row["移动设备"].ToString()))
            {
                errormsg.AppendFormat("{0}:请选择是否移动设备\r", index);
                index++;
                isSuccess = false;
            }
            else
            {
                if (row["移动设备"].ToString() == "否")
                {
                    if (IsNull(row["经度"].ToString()))
                    {
                        errormsg.AppendFormat("{0}:经度不能为空\r", index);
                        index++;
                        isSuccess = false;
                    }
                    else
                    {
                        if (!IsDouble(row["经度"].ToString()))
                        {
                            errormsg.AppendFormat("{0}:经度应为数字\r", index);
                            index++;
                            isSuccess = false;
                        }
                    }

                    if (IsNull(row["纬度"].ToString()))
                    {
                        errormsg.AppendFormat("{0}:纬度不能为空\r", index);
                        index++;
                        isSuccess = false;
                    }
                    else
                    {
                        if (!IsDouble(row["纬度"].ToString()))
                        {
                            errormsg.AppendFormat("{0}:纬度应为数字\r", index);
                            index++;
                            isSuccess = false;
                        }
                    }

                    if (IsNull(row["地点"].ToString()))
                    {
                        errormsg.AppendFormat("{0}:地点不能为空\r", index);
                        index++;
                        isSuccess = false;
                    }
                }
            }

            //已建站
            if (IsNull(row["已建站"].ToString()))
            {
                errormsg.AppendFormat("{0}:已建站不能为空\r", index);
                index++;
                isSuccess = false;
            }
            else
            {
                //已建站名称
                if (row["已建站"].ToString() == "是")
                {
                    if (IsNull(row["已建站名称"].ToString()))
                    {

                        errormsg.AppendFormat("{0}:已建站名称不能为空\r", index);
                        index++;
                        isSuccess = false;
                    }
                    if (IsNull(row["台站编号"].ToString()))
                    {

                        errormsg.AppendFormat("{0}:台站编号不能为空\r", index);
                        index++;
                        isSuccess = false;
                    }
                }
            }

            //发射频率
            if (IsNull(row["发射频率(MHz)"].ToString()))
            {
                errormsg.AppendFormat("{0}:发射频率不能为空\r", index);
                index++;
                isSuccess = false;
            }
            else
            {
                if (!IsDouble(row["发射频率(MHz)"].ToString()))
                {

                    errormsg.AppendFormat("{0}:发射频率应为数字 ", index);
                    index++;
                    isSuccess = false;
                }
            }

            //频率可调
            if (IsNull(row["频率可调"].ToString()))
            {
                errormsg.AppendFormat("{0}:频率可调不能为\r", index);
                index++;
                isSuccess = false;
            }
            else
            {
                if (row["频率可调"].ToString() == "是")
                {
                    //频率范围起始
                    if (IsNull(row["频率范围起始(MHz)"].ToString()))
                    {
                        errormsg.AppendFormat("{0}:频率范围起始不能为空\r", index);
                        index++;
                        isSuccess = false;
                    }
                    else
                    {
                        if (!IsDouble(row["频率范围起始(MHz)"].ToString()))
                        {
                            errormsg.AppendFormat("{0}:频率范围起始应为数字\r", index);
                            index++;
                            isSuccess = false;
                        }
                    }

                    //频率范围终止
                    if (IsNull(row["频率范围终止(MHz)"].ToString()))
                    {
                        errormsg.AppendFormat("{0}:频率范围终止不能为空\r", index);
                        index++;
                        isSuccess = false;
                    }
                    else
                    {
                        if (!IsDouble(row["频率范围终止(MHz)"].ToString()))
                        {
                            errormsg.AppendFormat("{0}:频率范围终止应为数字\r" + error, index);
                            index++;
                            isSuccess = false;
                        }
                    }
                }
            }

            //波道带宽
            if (IsNull(row["波道带宽(kHz)"].ToString()))
            {
                errormsg.AppendFormat("{0}:波道带宽不能为空\r", index);
                index++;
                isSuccess = false;
            }
            else
            {
                if (!IsDouble(row["波道带宽(kHz)"].ToString()))
                {
                    errormsg.AppendFormat("{0}:波道带宽应为数字\r", index);
                    index++;
                    isSuccess = false;
                }
            }

            //发射功率
            if (IsNull(row["发射功率(W)"].ToString()))
            {
                errormsg.AppendFormat("{0}:发射功率不能为空\r", index);
                index++;
                isSuccess = false;
            }
            else
            {
                if (!IsDouble(row["发射功率(W)"].ToString()))
                {
                    errormsg.AppendFormat("{0}:发射功率应为数字\r", index);
                    index++;
                    isSuccess = false;
                }
            }

            //调制方式
            if (IsNull(row["调制方式"].ToString()))
            {
                errormsg.AppendFormat("{0}:调制方式不能为空\r" + error, index);
                index++;
                isSuccess = false;
            }
            outerror = errormsg.ToString();
            return isSuccess;
        }

        /// <summary>
        /// 验证是否有重复的名称和编号
        /// </summary>
        /// <returns></returns>
        private static bool ExistSameNameAndCode(DataRow row, DataTable dt)
        {
            //List
            int count = 0;
            for (int j = 0; j < dt.Rows.Count; j++)
            {
                DataRow dr = dt.Rows[j];
                if (dr["设备名称"].ToString() == row["设备名称"].ToString() && dr["设备编号"].ToString() == row["设备编号"].ToString())
                {
                    count++;
                }
            }
            if (count > 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 移除表明中的特殊字符
        /// </summary>
        /// <param name="tablename"></param>
        /// <returns></returns>
        public static string TableNameConvert(string tablename)
        {
            tablename = tablename.TrimStart('\'').TrimEnd('\'');
            if (tablename.Contains('$'))
            {
                tablename = tablename.Replace('$', ' ').Trim();
            }
            if (tablename.Contains('|'))
            {
                tablename = tablename.Replace('|', '/');
            }
            if (tablename.Contains('#'))
            {
                tablename = tablename.Replace('#', '.').Trim();
            }
            return tablename;
        }

        /// <summary>
        /// 获取保障类别GUID
        /// </summary>
        /// <param name="tablename"></param>
        /// <returns></returns>
        public static string GetEquipmentClassId(string tablename)
        {
            //EquipmentClass[] classes = CO_IA.Client.Utility.EquipmentClasses;
            //EquipmentClass type = classes.FirstOrDefault(r => r.Name == tablename);

            //if (type == null)
            //{
                return null;
            //}
            //else
            //{
            //    return type.Guid;
            //}
        }

        /// <summary>
        /// 验证是否为空
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool IsNull(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 验证是否是Double值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        private static bool IsDouble(string value)
        {
            double dresult = 0;
            if (double.TryParse(value, out dresult))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 模板下载
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="filepath"></param>
        public static void TemplateDownLoad(string filename, string filepath)
        {
            string downFilepath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filepath);
            string downFilename = string.Format(downFilepath + filename);
            string filter = downFilename.Split('.')[1];
            if (!Directory.Exists(downFilepath))
            {
                Directory.CreateDirectory(downFilepath);
                MessageBox.Show("模板文件夹中不存在模板文件！");
                return;
            }

            if (File.Exists(downFilename))
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = string.Format("Excel files(*.{0})|(*.{0}) ", filter);
                //saveFileDialog.Filter = "zip files(*.zip)|*.zip|All files(*.*)|*.*";

                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.FileName = filename;

                bool? result = saveFileDialog.ShowDialog();
                if (result == true)
                {
                    string localFilePath = saveFileDialog.FileName.ToString();

                    try
                    {
                        File.Copy(downFilename, localFilePath, true);
                        MessageBox.Show("保存模板成功！");
                    }
                    catch
                    {
                        MessageBox.Show("保存模板失败！");
                    }
                }
            }
            else
            {
                MessageBox.Show("模板不存在请联系系统管理员！");
            }
        }
    }
}
