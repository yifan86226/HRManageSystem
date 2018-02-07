using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Word = Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using System.Windows.Xps.Packaging;
using System.IO;
using System.Windows;

namespace CO_IA.Offices
{
    public class OfficeToPdfConverter
    {
        public static void ConvertFromWord(string sourcePath, string targetPath)
        {
            Word.Application application = null;
            Word.Document document = null;
            try
            {
                application = new Microsoft.Office.Interop.Word.Application();
                application.Application.Visible = false;
                document=application.Documents.Open(sourcePath);
                document.SaveAs(targetPath, Word.WdSaveFormat.wdFormatPDF);
            }
            finally
            {
                if (document != null)
                {
                    ((Word._Document)document).Close();
                    document = null;
                }
                if (application != null)
                {
                    ((Word._Application)application).Quit(Word.WdSaveOptions.wdDoNotSaveChanges);
                    application = null;
                }
            } 
        }

        public static void ConvertFromExcel(string sourcePath, string targetPath)
        {
            Excel.Application excelApplication = null;
            Excel.Workbook excelWorkbook = null;
            try
            {
                excelApplication = new Excel.Application();
                excelWorkbook = excelApplication.Workbooks.Open(sourcePath);
                excelWorkbook.ExportAsFixedFormat(
                            Excel.XlFixedFormatType.xlTypePDF,
                            targetPath,
                            Excel.XlFixedFormatQuality.xlQualityStandard,
                            true,
                            true,
                            OpenAfterPublish: false
                        );
            }
            finally
            {
                if (excelWorkbook != null)
                {
                    excelWorkbook.Close();
                    excelWorkbook = null;
                }
                if (excelApplication != null)
                {
                    excelApplication.Quit();
                    excelApplication = null;
                }
            }
        }

        public static void ConvertFromPowerPoint(string sourcePath, string targetPath)
        {
            PowerPoint.Application application = new PowerPoint.Application();
            PowerPoint.Presentation presentation = null; 
            try
            {
                application = new PowerPoint.Application();
                presentation = application.Presentations.Open(sourcePath, Microsoft.Office.Core.MsoTriState.msoTrue, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoFalse);
                presentation.SaveAs(targetPath, PowerPoint.PpSaveAsFileType.ppSaveAsPDF);
            }
            finally
            {
                if (presentation != null)
                {
                    presentation.Close();
                    presentation = null;
                }
                if (application != null)
                {
                    application.Quit();
                    application = null;
                }
            }
        }
    }
}
