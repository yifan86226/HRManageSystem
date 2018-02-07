using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
//using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using Word = Microsoft.Office.Interop.Word;

namespace Office_To_XPS
{
    public class OfficeToXps
    {
        #region Properties & Constants
        private static List<string> wordExtensions = new List<string>
        {
            ".doc",
            ".docx"
        };

        private static List<string> excelExtensions = new List<string>
        {
            ".xls",
            ".xlsx"
        };

        private static List<string> powerpointExtensions = new List<string>
        {
            ".ppt",
            ".pptx"
        };

        #endregion

        #region Public Methods

        public static OfficeToXpsConversionResult ConvertToXps(string sourceFilePath, ref string resultFilePath)
        {
            var result = new OfficeToXpsConversionResult(ConversionResult.UnexpectedError);

            // Check to see if it's a valid file
            if (!IsValidFilePath(sourceFilePath))
            {
                result.Result = ConversionResult.InvalidFilePath;
                result.ResultText = sourceFilePath;
                return result;
            }



            var ext = Path.GetExtension(sourceFilePath).ToLower();

            // Check to see if it's in our list of convertable extensions
            if (!IsConvertableFilePath(sourceFilePath))
            {
                result.Result = ConversionResult.InvalidFileExtension;
                result.ResultText = ext;
                return result;
            }

            // Convert if Word
            if (wordExtensions.Contains(ext))
            {
                return ConvertFromWord(sourceFilePath, ref resultFilePath);
            }

            // Convert if Excel
            if (excelExtensions.Contains(ext))
            {
                return ConvertFromExcel(sourceFilePath, ref resultFilePath);
            }

            // Convert if PowerPoint
            if (powerpointExtensions.Contains(ext))
            {
                // return ConvertFromPowerPoint(sourceFilePath, ref resultFilePath);
            }

            return result;
        }
        #endregion

        #region Private Methods
        public static bool IsValidFilePath(string sourceFilePath)
        {
            if (string.IsNullOrEmpty(sourceFilePath))
                return false;

            try
            {
                return File.Exists(sourceFilePath);
            }
            catch (Exception)
            {
            }

            return false;
        }

        public static bool IsConvertableFilePath(string sourceFilePath)
        {
            var ext = Path.GetExtension(sourceFilePath).ToLower();

            return IsConvertableExtension(ext);
        }
        public static bool IsConvertableExtension(string extension)
        {
            return wordExtensions.Contains(extension) ||
                   excelExtensions.Contains(extension) ||
                   powerpointExtensions.Contains(extension);
        }

        private static string GetTempXpsFilePath()
        {
            return Path.ChangeExtension(Path.GetTempFileName(), ".xps");
        }

      
        /// <summary>
        /// word转换xps 并在本地指定路径生成xps
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="resultFilePath"></param>
        /// <returns></returns>
        private static OfficeToXpsConversionResult ConvertFromWord(string sourceFilePath, ref string resultFilePath)
        {
            object pSourceDocPath = sourceFilePath;

            string pExportFilePath = string.IsNullOrEmpty(resultFilePath) ? GetTempXpsFilePath() : resultFilePath;

            try
            {
                var pExportFormat = Word.WdExportFormat.wdExportFormatXPS;
                bool pOpenAfterExport = false;
                var pExportOptimizeFor = Word.WdExportOptimizeFor.wdExportOptimizeForOnScreen;
                var pExportRange = Word.WdExportRange.wdExportAllDocument;
                int pStartPage = 0;
                int pEndPage = 0;
                var pExportItem = Word.WdExportItem.wdExportDocumentContent;
                var pIncludeDocProps = true;
                var pKeepIRM = true;
                var pCreateBookmarks = Word.WdExportCreateBookmarks.wdExportCreateWordBookmarks;
                var pDocStructureTags = true;
                var pBitmapMissingFonts = true;
                var pUseISO19005_1 = false;


                Word.Application wordApplication = null;
                Word.Document wordDocument = null;

                try
                {
                    wordApplication = new Word.Application();
                    //此处利用xps打印机在本地生成XPS，此时Application对象要调用同一个对象，否则会出现占用进程问题
                    GetXPSFile(sourceFilePath, resultFilePath, wordApplication);
                }
                catch (Exception exc)
                {
                    wordApplication.Documents.Close();
                    GC.Collect();
                    return new OfficeToXpsConversionResult(ConversionResult.ErrorUnableToInitializeOfficeApp, "Word", exc);
                }

                try
                {
                    try
                    {
                        wordDocument = wordApplication.Documents.Open(ref pSourceDocPath);
                    }
                    catch (Exception exc)
                    {
                        return new OfficeToXpsConversionResult(ConversionResult.ErrorUnableToOpenOfficeFile, exc.Message, exc);
                    }

                    if (wordDocument != null)
                    {
                        try
                        {
                            wordDocument.ExportAsFixedFormat(
                                                pExportFilePath,
                                                pExportFormat,
                                                pOpenAfterExport,
                                                pExportOptimizeFor,
                                                pExportRange,
                                                pStartPage,
                                                pEndPage,
                                                pExportItem,
                                                pIncludeDocProps,
                                                pKeepIRM,
                                                pCreateBookmarks,
                                                pDocStructureTags,
                                                pBitmapMissingFonts,
                                                pUseISO19005_1
                                            );
                        }
                        catch (Exception exc)
                        {
                            return new OfficeToXpsConversionResult(ConversionResult.ErrorUnableToExportToXps, "Word", exc);
                        }
                    }
                    else
                    {
                        return new OfficeToXpsConversionResult(ConversionResult.ErrorUnableToOpenOfficeFile);
                    }
                }
                finally
                {
                    // Close and release the Document object.
                    if (wordDocument != null)
                    {
                        wordDocument.Close();
                        wordDocument = null;
                    }

                    // Quit Word and release the ApplicationClass object.
                    if (wordApplication != null)
                    {
                        wordApplication.Quit();
                        wordApplication = null;
                    }

                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }
            catch (Exception exc)
            {
                return new OfficeToXpsConversionResult(ConversionResult.ErrorUnableToAccessOfficeInterop, "Word", exc);
            }

            resultFilePath = pExportFilePath;

            return new OfficeToXpsConversionResult(ConversionResult.OK, pExportFilePath);
        }

        /// <summary>
        /// excel转换xps
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="resultFilePath"></param>
        /// <returns></returns>
        private static OfficeToXpsConversionResult ConvertFromExcel(string sourceFilePath, ref string resultFilePath)
        {
            string pSourceDocPath = sourceFilePath;

            string pExportFilePath = string.IsNullOrEmpty(resultFilePath) ? GetTempXpsFilePath() : resultFilePath;

            try
            {
                var pExportFormat = Excel.XlFixedFormatType.xlTypeXPS;
                var pExportQuality = Excel.XlFixedFormatQuality.xlQualityStandard;
                var pOpenAfterPublish = false;
                var pIncludeDocProps = true;
                var pIgnorePrintAreas = true;


                Excel.Application excelApplication = null;
                Excel.Workbook excelWorkbook = null;

                try
                {
                    excelApplication = new Excel.Application();
                    //此处利用xps打印机在本地生成XPS，此时Application对象要调用同一个对象，否则会出现占用进程问题
                    GetExcelToXps(sourceFilePath, resultFilePath, excelApplication);
                }
                catch (Exception exc)
                {
                    return new OfficeToXpsConversionResult(ConversionResult.ErrorUnableToInitializeOfficeApp, "Excel", exc);
                }

                try
                {
                    try
                    {
                        excelWorkbook = excelApplication.Workbooks.Open(pSourceDocPath);
                    }
                    catch (Exception exc)
                    {
                        return new OfficeToXpsConversionResult(ConversionResult.ErrorUnableToOpenOfficeFile, exc.Message, exc);
                    }
                    //暂时去掉 文件被占用的判断，不然加载不出来
                    //if (excelWorkbook != null)
                    //{
                    //    try
                    //    {
                    //        excelWorkbook.ExportAsFixedFormat(
                    //                            pExportFormat,
                    //                            pExportFilePath,
                    //                            pExportQuality,
                    //                            pIncludeDocProps,
                    //                            pIgnorePrintAreas,

                    //                            OpenAfterPublish: pOpenAfterPublish
                    //                        );
                    //    }
                    //    catch (Exception exc)
                    //    {
                    //        return new OfficeToXpsConversionResult(ConversionResult.ErrorUnableToExportToXps, "Excel", exc);
                    //    }
                    //}
                    //else
                    //{
                    //    return new OfficeToXpsConversionResult(ConversionResult.ErrorUnableToOpenOfficeFile);
                    //}
                }
                finally
                {
                    // Close and release the Document object.
                    if (excelWorkbook != null)
                    {
                        excelWorkbook.Close();
                       System.Runtime.InteropServices.Marshal.ReleaseComObject(excelWorkbook);
                        excelWorkbook = null;
                    }

                    // Quit Word and release the ApplicationClass object.
                    if (excelApplication != null)
                    {
                        excelApplication.Quit();
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApplication);
                        excelApplication = null;
                    }

                    GC.Collect();// 前提是：所有的对象都=null后才能调用此函数，然后才会结束“任务管理器”中的excel.exe进程
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }
            catch (Exception exc)
            {
                return new OfficeToXpsConversionResult(ConversionResult.ErrorUnableToAccessOfficeInterop, "Excel", exc);
            }

            resultFilePath = pExportFilePath;

            return new OfficeToXpsConversionResult(ConversionResult.OK, pExportFilePath);
        }
        #endregion

        #region 生成xps文件到本地
        /// <summary>
        /// Excel转换为XPS文档
        /// </summary>
        /// <param name="doc">Excel原始文件</param>
        /// <param name="xps">生成的Xps文件</param>
        public static void GetExcelToXps(string xls, string xps,Microsoft.Office.Interop.Excel.Application app)
        {
            var workbook = OpenWorkBook(app, xls);
            workbook.PrintOut(1, Type.Missing, 1, false, "Microsoft XPS Document Writer", true, false, xps);
            workbook.Save();
            app.Quit();
        }
        public static Microsoft.Office.Interop.Excel.Workbook OpenWorkBook(Microsoft.Office.Interop.Excel.Application app, string fileName)
        {

            return app.Workbooks.Open(fileName, Type.Missing, Type.Missing, Type.Missing,
            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

        }
        /// <summary>
        /// （借用xps打印机）指定本地路径生成xps文件
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="resultFilePath"></param>
        /// <param name="wordApplication"></param>
        private static void GetXPSFile(string sourceFilePath, string resultFilePath, Word.Application wordApplication)
        {
            object file = sourceFilePath;
            object missing = System.Type.Missing;
            var app = wordApplication;
            app.Visible = false;
            var document = app.Documents.Open(ref file,
            ref missing, ref missing, ref missing, ref missing, ref missing,
            ref missing, ref missing, ref missing, ref missing, ref missing,
            ref missing, ref missing, ref missing, ref missing, ref missing);

            object Background = false;
            object Range = Microsoft.Office.Interop.Word.WdPrintOutRange.wdPrintAllDocument;
            object Copies = 1;
            object PageType = Microsoft.Office.Interop.Word.WdPrintOutPages.wdPrintAllPages;
            object PrintToFile = true;
            object Collate = false;
            object ActivePrinterMacGX = missing;
            object ManualDuplexPrint = false;
            object PrintZoomColumn = 1;
            object PrintZoomRow = 1;
            object OutputFileName = resultFilePath;
            object SaveChanges = false;
            app.ActivePrinter = "Microsoft XPS Document Writer";

            document.PrintOut(ref Background, ref missing, ref Range, ref OutputFileName,
                ref missing, ref missing, ref missing, ref Copies,
                ref missing, ref PageType, ref PrintToFile, ref Collate,
                ref missing, ref ManualDuplexPrint, ref PrintZoomColumn,
                ref PrintZoomRow, ref missing, ref missing);
            //app.Quit(ref SaveChanges, ref missing, ref missing);
        }
        //此方法不用
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out   int ID);
        /// <summary>
        /// 杀当前Excel线程
        /// </summary>
        private static void KillCurrentExcel(Microsoft.Office.Interop.Excel.Application appExcel)
        {
            IntPtr t = new IntPtr(appExcel.Hwnd);
            int k = 0;
            GetWindowThreadProcessId(t, out k);
            System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById(k);
            p.Kill();
        }
        #endregion


    }

    public class OfficeToXpsConversionResult
    {
        #region Properties
        public ConversionResult Result { get; set; }
        public string ResultText { get; set; }
        public Exception ResultError { get; set; }
        #endregion

        #region Constructors
        public OfficeToXpsConversionResult()
        {
            Result = ConversionResult.UnexpectedError;
            ResultText = string.Empty;
        }
        public OfficeToXpsConversionResult(ConversionResult result)
            : this()
        {
            Result = result;
        }
        public OfficeToXpsConversionResult(ConversionResult result, string resultText)
            : this(result)
        {
            ResultText = resultText;
        }
        public OfficeToXpsConversionResult(ConversionResult result, string resultText, Exception exc)
            : this(result, resultText)
        {
            ResultError = exc;
        }
        #endregion
    }

    public enum ConversionResult
    {
        OK = 0,
        InvalidFilePath = 1,
        InvalidFileExtension = 2,
        UnexpectedError = 3,
        ErrorUnableToInitializeOfficeApp = 4,
        ErrorUnableToOpenOfficeFile = 5,
        ErrorUnableToAccessOfficeInterop = 6,
        ErrorUnableToExportToXps = 7
    }
}
