using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;
using System.Windows;
using System.IO;
using AT_BC.Offices.Excel;
//using System.Windows.Forms;

namespace CO_IA.Client
{
    public class ExcelHelper
    {
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out   int ID);

        /// <summary>
        /// 获取保存路径
        /// </summary>
        /// <returns></returns>
        public static string GetPath()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "(*.xls)|*.xls";
            //dialog.Filter = "(*.xls)|*.xls|(*.xlsx)|*.xlsx";
            dialog.DefaultExt = "xls";

            bool? result = dialog.ShowDialog();
            if (result.Value)
            {
                return dialog.FileName;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 导出到Excel方法
        /// </summary>
        /// <param name="objEquData">DataGrid.ItemsSource转换后的二维数组</param>
        /// <param name="savePath">保存路径</param>
        /// <param name="title">sheet标题</param>
        /// <returns></returns>
        public static bool ExportToExcel(object[,] objData, string savePath, string title)
        {
            AT_BC.Offices.Excel.BeExcel.ExportToExcel(savePath, title, objData);
            return true;
            //Microsoft.Office.Interop.Excel.Application appExcel = new Microsoft.Office.Interop.Excel.Application();

            //if (appExcel == null)
            //{
            //    MessageBox.Show("无法打开EXcel，请检查Excel是否可用或者是否安装好Excel", "系统提示");
            //    return false;
            //}


            //System.Globalization.CultureInfo currentCI = System.Threading.Thread.CurrentThread.CurrentCulture;
            //System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            //Microsoft.Office.Interop.Excel.Workbook workbook = appExcel.Workbooks.Add(Missing.Value);
            //Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets.get_Item(1);

            //worksheet.Name = title;//一个sheet的名称        
            //System.Windows.Forms.Application.DoEvents();
            //Microsoft.Office.Interop.Excel.Range range = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[objData.GetLength(0), objData.GetLength(1)]];
            //range.NumberFormat = "@";//设置数字文本格式
            //range.Value = objData;
            //appExcel.Columns.ColumnWidth = 15;

            ////恢复文化环境
            //System.Threading.Thread.CurrentThread.CurrentCulture = currentCI;
            //try
            //{
            //    workbook.Saved = true;
            //    workbook.SaveAs(savePath);//以复制的形式保存在已有的文档里           
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("导出文件出错，文件可能正被打开，具体原因：" + ex.Message, "出错信息");
            //    return false;
            //}
            //finally
            //{
            //    appExcel.Quit();
            //    KillCurrentExcel(appExcel);
            //    System.Runtime.InteropServices.Marshal.ReleaseComObject(appExcel);
            //    System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
            //    System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
            //}
            //return true;
        }

        public static bool ExportToExcel(object[,] objOrgData, object[,] objEquData, string savePath)
        {
            string strExcelPathName = AppDomain.CurrentDomain.BaseDirectory + "Template\\Equipment\\单位设备导出模板.xls";
            if (File.Exists(strExcelPathName))
            {
                BeExcel excel = BeExcel.OpenFile(strExcelPathName);
                excel.ExportToWorkSheet("单位信息", objOrgData, 0, 1);
                excel.ExportToWorkSheet("设备信息", objEquData, 1, 0);
                excel.Save(savePath);
                //System.Globalization.CultureInfo currentCI = System.Threading.Thread.CurrentThread.CurrentCulture;
                //Microsoft.Office.Interop.Excel.Application application = new Microsoft.Office.Interop.Excel.Application();

                ////设置禁止弹出保存和覆盖的询问提示框  
                //application.DisplayAlerts = false;
                //application.AlertBeforeOverwriting = false;

                //Workbook workBook = application.Workbooks.Open(strExcelPathName, Type.Missing, Type.Missing,
                //  Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                //  Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                //Worksheet orgworkSheet = (Worksheet)workBook.Sheets["单位信息"];
                //Worksheet equworkSheet = (Worksheet)workBook.Sheets["设备信息"];

                //if (objOrgData != null)
                //{
                //    Range orgrange = orgworkSheet.Range[orgworkSheet.Cells[1, 2], orgworkSheet.Cells[objOrgData.GetLength(0), objOrgData.GetLength(1) + 1]];
                //    orgrange.Value = objOrgData;
                //}

                //if (objEquData != null)
                //{
                //    Range equrange = equworkSheet.Range[equworkSheet.Cells[2, 1], equworkSheet.Cells[objEquData.GetLength(0) + 1, objEquData.GetLength(1)]];
                //    equrange.Value = objEquData;
                //}

                ////恢复文化环境
                //System.Threading.Thread.CurrentThread.CurrentCulture = currentCI;
                //try
                //{
                //    workBook.Saved = true;
                //    workBook.SaveAs(savePath);//以复制的形式保存在已有的文档里           
                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show("导出文件出错，文件可能正被打开，具体原因：" + ex.Message, "出错信息");
                //    return false;
                //}
                //finally
                //{
                //    application.Quit();
                //    KillCurrentExcel(application);
                //    System.Runtime.InteropServices.Marshal.ReleaseComObject(application);
                //    System.Runtime.InteropServices.Marshal.ReleaseComObject(orgworkSheet);
                //    System.Runtime.InteropServices.Marshal.ReleaseComObject(equworkSheet);

                //}
            }
            else
            {
                MessageBox.Show("导出模板不存在,请于系统管理员联系");
                return false;
            }
            return true;
        }

        ///// <summary>
        ///// 杀当前Excel线程
        ///// </summary>
        //private static void KillCurrentExcel(Microsoft.Office.Interop.Excel.Application appExcel)
        //{
        //    IntPtr t = new IntPtr(appExcel.Hwnd);
        //    int k = 0;
        //    GetWindowThreadProcessId(t, out k);
        //    System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById(k);
        //    p.Kill();
        //}

    }
}
