using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Xps.Packaging;
using CO_IA.Data.ActivitySummarize;
using System.Runtime.InteropServices;

namespace CO_IA.UI.ActivitySummarize
{
    public class SummarizeHelpData
    {
        #region 活动总结相关
        /// <summary>
        /// 获取全部活动信息列表
        /// </summary>
        /// <returns></returns>
        public static SummarizeDoc[] GetAllSummarizeDoc(string activityId)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivitySummarize.I_CO_IA_SummarizeDoc, CO_IA.Data.ActivitySummarize.SummarizeDoc[]>(
               channel =>
               {
                   return channel.GetAllSummarizeDoc(activityId);
               });
        }
        #endregion

        #region word和excel文件转换xps相关
        /// <summary>
        /// 判断xps是否转换成功，如果成功生成到本地
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="filePath"></param>
        /// <param name="xpsFilePath"></param>
        /// <param name="convertResults"></param>
        //public static void JudgeXPSType(string sourceFile, string filePath, string xpsFilePath, OfficeToXpsConversionResult convertResults, DocumentViewer xpsDocViewr)
        //{
        //    switch (convertResults.Result)
        //    {
        //        case ConversionResult.OK:
        //            try
        //            {
        //                if (sourceFile.EndsWith("doc", StringComparison.CurrentCultureIgnoreCase) || sourceFile.EndsWith("docx", StringComparison.CurrentCultureIgnoreCase))
        //                {
        //                    WordToXps(filePath, xpsFilePath);//生成xps文件到本地
        //                }
        //                else if (sourceFile.EndsWith("xls", StringComparison.CurrentCultureIgnoreCase) || sourceFile.EndsWith("xlsx", StringComparison.CurrentCultureIgnoreCase))
        //                {
        //                    ExcelToXps(filePath, xpsFilePath);//生成xps文件到本地
        //                }
        //                using (XpsDocument xpsDoc = new XpsDocument(xpsFilePath, FileAccess.Read))
        //                {
        //                    var fsxps = xpsDoc.GetFixedDocumentSequence();
        //                    xpsDocViewr.Document = fsxps;
        //                }
        //            }
        //            catch (Exception ex)
        //            {

        //            }
        //            break;
        //        case ConversionResult.InvalidFilePath:
        //            // 处理文件路径错误或文件不存在
        //            break;
        //        case ConversionResult.UnexpectedError:

        //            break;
        //        case ConversionResult.ErrorUnableToInitializeOfficeApp:
        //            // Office 未安装会出现这个异常
        //            break;
        //        case ConversionResult.ErrorUnableToOpenOfficeFile:
        //            // 文件被占用会出现这个异常
        //            break;
        //        case ConversionResult.ErrorUnableToAccessOfficeInterop:
        //            // Office 未安装会出现这个异常
        //            break;
        //        case ConversionResult.ErrorUnableToExportToXps:
        //            // 微软 OFFICE Save As PDF 或 XPS  插件未安装异常
        //            break;
        //    }
        //}
        ///// <summary>
        ///// Word转换为XPS文档
        ///// </summary>
        ///// <param name="doc">Word原始文件</param>
        ///// <param name="xps">生成的Xps文件</param>
        //public static void WordToXps(string doc, string xps)
        //{
        //    object file = doc;
        //    object missing = System.Type.Missing;
        //    var app = new Microsoft.Office.Interop.Word.Application();
        //    app.Visible = false;
        //    var document = app.Documents.Open(ref file,
        //    ref missing, ref missing, ref missing, ref missing, ref missing,
        //    ref missing, ref missing, ref missing, ref missing, ref missing,
        //    ref missing, ref missing, ref missing, ref missing, ref missing);

        //    object Background = false;
        //    object Range = Microsoft.Office.Interop.Word.WdPrintOutRange.wdPrintAllDocument;
        //    object Copies = 1;
        //    object PageType = Microsoft.Office.Interop.Word.WdPrintOutPages.wdPrintAllPages;
        //    object PrintToFile = true;
        //    object Collate = false;
        //    object ActivePrinterMacGX = missing;
        //    object ManualDuplexPrint = false;
        //    object PrintZoomColumn = 1;
        //    object PrintZoomRow = 1;
        //    object OutputFileName = xps;
        //    object SaveChanges = false;
        //    app.ActivePrinter = "Microsoft XPS Document Writer";

        //    document.PrintOut(ref Background, ref missing, ref Range, ref OutputFileName,
        //        ref missing, ref missing, ref missing, ref Copies,
        //        ref missing, ref PageType, ref PrintToFile, ref Collate,
        //        ref missing, ref ManualDuplexPrint, ref PrintZoomColumn,
        //        ref PrintZoomRow, ref missing, ref missing);
        //    app.Quit(ref SaveChanges, ref missing, ref missing);
        //}
        ///// <summary>
        ///// Excel转换为XPS文档
        ///// </summary>
        ///// <param name="doc">Excel原始文件</param>
        ///// <param name="xps">生成的Xps文件</param>
        //public static void GetExcelToXps(string xls, string xps)
        //{
        //    var app = new Microsoft.Office.Interop.Excel.Application();
        //    var workbook = OpenWorkBook(app, xls);
        //    workbook.PrintOut(1, Type.Missing, 1, false, "Microsoft XPS Document Writer", true, false, xps);
        //    workbook.Save();
        //    app.Quit();
        //}
        //public static Microsoft.Office.Interop.Excel.Workbook OpenWorkBook(Microsoft.Office.Interop.Excel.Application app, string fileName)
        //{

        //    return app.Workbooks.Open(fileName, Type.Missing, Type.Missing, Type.Missing,
        //    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

        //}

        /// <summary>
        /// word文件转换二进制数据(用于保存数据库)
        /// </summary>
        /// <param name="wordPath">word文件路径</param>
        /// <returns>二进制</returns>
        private static byte[] DocConvertByte(string wordPath)
        {
            byte[] bytContent = null;
            System.IO.FileStream fs = null;
            System.IO.BinaryReader br = null;
            try
            {
                fs = new FileStream(wordPath, System.IO.FileMode.Open);
            }
            catch
            {
            }
            br = new BinaryReader((Stream)fs);
            bytContent = br.ReadBytes((Int32)fs.Length);


            return bytContent;
        }
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern int GetWindowThreadProcessId(IntPtr hwnd, out   int ID);
        ///// <summary>
        ///// 杀当前Excel线程
        ///// </summary>
        //public static void KillCurrentExcel(Microsoft.Office.Interop.Excel.Application appExcel)
        //{
        //    IntPtr t = new IntPtr(appExcel.Hwnd);
        //    int k = 0;
        //    GetWindowThreadProcessId(t, out k);
        //    System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById(k);
        //    p.Kill();
        //}
        #endregion

        #region 上传下载相关
        ///// <summary>
        ///// 二进制数据转换为word文件(缓存到服务器)
        ///// </summary>
        ///// <param name="data">二进制数据</param>
        ///// <param name="fileName">word文件名</param>
        ///// <returns>文件保存的相对路径</returns>
        //public static string ByteConvertDocService(byte[] data, string fileName)
        //{
        //    FileStream fs;
        //    string savePath = GetPath();
        //    if (!System.IO.Directory.Exists(savePath))
        //    {
        //        Directory.CreateDirectory(savePath);
        //    }
        //    savePath += fileName;
        //    if (System.IO.File.Exists(savePath))
        //    {
        //        fs = new FileStream(savePath, FileMode.Truncate);
        //    }
        //    else
        //    {
        //        //先删除文件
        //        System.IO.File.Delete(savePath);
        //        //重新创建
        //        fs = new FileStream(savePath, FileMode.CreateNew);
        //    }
        //    BinaryWriter br = new BinaryWriter(fs);
        //    br.Write(data, 0, data.Length);
        //    br.Close();
        //    fs.Close();
        //    return savePath;
        //}
        ///// <summary>
        ///// 项目所在路径
        ///// </summary>
        ///// <returns></returns>
        //public static string GetPath()
        //{
        //    string savePath = @"\SysDoc\" + FormatNowTime(2) + @"\";
        //    string allPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        //    int proPathLength = allPath.Substring(allPath.LastIndexOf("Output") - 1).Length;
        //    string proPath = allPath.Substring(0, allPath.Length - proPathLength);
        //    string path = proPath + @"\Output\" + "textoffice" + savePath;
        //    if (!File.Exists(path))
        //    {
        //        Directory.CreateDirectory(path);
        //    }
        //    return path;
        //}
        ///// <summary>
        ///// 格式化当前时间: 
        ///// 1:yyMMddHHmmss; 2:yyyy-MM\dd\
        ///// </summary>
        ///// <returns></returns>
        //public static string FormatNowTime(int num)
        //{
        //    if (num == 1)
        //    {
        //        return DateTime.Now.ToString("yyMMddHHmmss");
        //    }
        //    else if (num == 2)
        //    {
        //        return DateTime.Now.ToString("yyyy-MM") + @"\" + DateTime.Now.Day;
        //    }
        //    return "";
        //}
        #endregion
    }
}
