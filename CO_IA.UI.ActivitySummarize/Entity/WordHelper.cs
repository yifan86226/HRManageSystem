//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Windows.Forms;
//using Microsoft.Office.Interop.Word;

//namespace WordHelper
//{
//    class WordHelperCS
//    {
//        private Microsoft.Office.Interop.Word.Document wDoc = null;
//        private Microsoft.Office.Interop.Word.Application wApp = null;

//        public Microsoft.Office.Interop.Word.Document Document
//        {
//            get { return wDoc; }
//            set { wDoc = value; }
//        }

//        public Microsoft.Office.Interop.Word.Application Application
//        {
//            get { return wApp; }
//            set { wApp = value; }
//        }


//        #region 从模板创建新的Word文档
//        /// <summary>   
//        /// 从模板创建新的Word文档   
//        /// </summary>   
//        /// <param name="templateName">模板文件名</param>   
//        /// <returns></returns>  
//        public bool CreateNewWordDocument(string templateName)
//        {
//            try
//            {
//                return CreateNewWordDocument(templateName, ref wDoc, ref wApp);
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }

//        #endregion



//        #region 从模板创建新的Word文档,并且返回对象Document,Application
//        /// <summary>   
//        /// 从模板创建新的Word文档，   
//        /// </summary>   
//        /// <param name="templateName">模板文件名</param>   
//        /// <param name="wDoc">返回的Word.Document对象</param>   
//        /// <param name="WApp">返回的Word.Application对象</param>   
//        /// <returns></returns>  
//        public static bool CreateNewWordDocument(string templateName, ref Microsoft.Office.Interop.Word.Document wDoc, ref Microsoft.Office.Interop.Word.Application WApp)
//        {
//            killWinWordProcess();
//            Microsoft.Office.Interop.Word.Document thisDocument = null;
//            Microsoft.Office.Interop.Word.Application thisApplication = new Microsoft.Office.Interop.Word.Application();
//            thisApplication.Visible = false;
//            thisApplication.Caption = "";  //标题
//            thisApplication.Options.CheckSpellingAsYouType = false;
//            thisApplication.Options.CheckGrammarAsYouType = false;

//            Object Template = templateName;// Optional Object. The name of the template to be used for the new document. If this argument is omitted, the Normal template is used.

//            Object NewTemplate = false;// Optional Object. True to open the document as a template. The default value is False.

//            Object DocumentType = Microsoft.Office.Interop.Word.WdNewDocumentType.wdNewBlankDocument; // Optional Object. Can be one of the following WdNewDocumentType constants: wdNewBlankDocument, wdNewEmailMessage, wdNewFrameset, or wdNewWebPage. The default constant is wdNewBlankDocument.

//            Object Visible = true;//Optional Object. True to open the document in a visible window. If this value is False, Microsoft Word opens the document but sets the Visible property of the document window to False. The default value is True.

//            try
//            {
//                Microsoft.Office.Interop.Word.Document wordDoc = thisApplication.Documents.Add(ref Template, ref NewTemplate, ref DocumentType, ref Visible);

//                thisDocument = wordDoc;
//                wDoc = wordDoc;
//                WApp = thisApplication;
//                return true;
//            }
//            catch (Exception ex)
//            {
//                string err = string.Format("创建Word文档出错，错误原因:{0}", ex.Message);
//                throw new Exception(err, ex);
//            }

//        }

//        private static void killWinWordProcess()
//        {

//            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName("WINWORD");
//            foreach (System.Diagnostics.Process process in processes)
//            {
//                bool b = process.MainWindowTitle == "";
//                if (process.MainWindowTitle == "")
//                {
//                    process.Kill();
//                }
//            }
//        }

       
//        #endregion


//        #region 文档另存为其他文件名
//        /// <summary>   
//        /// 文档另存为其他文件名   
//        /// </summary>   
//        /// <param name="fileName">文件名</param>   
//        /// <param name="wDoc">Document对象</param> 
//        public bool SaveAs(string fileName)
//        {
//            try
//            {
//                return SaveAs(fileName, wDoc);
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }
//        #endregion


//        #region 文档另存为其他文件名
//        /// <summary>   
//        /// 文档另存为其他文件名   
//        /// </summary>   
//        /// <param name="fileName">文件名</param>   
//        /// <param name="wDoc">Document对象</param> 
//        public static bool SaveAs(string fileName, Microsoft.Office.Interop.Word.Document wDoc)
//        {
//            Object FileName = fileName;//文档的名称。默认值是当前文件夹名和文件名。如果文档在以前没有保存过，则使用默认名称（例如，Doc1.doc）。如果已经存在具有指定文件名的文档，则会在不先提示用户的情况下改写文档。   

//            Object FileFormat = Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatDocument;// 文档的保存格式。可以是任何 WdSaveFormat 值。要以另一种格式保存文档，请为 SaveFormat 属性指定适当的值。

//            Object LockComments = false;// 如果为 true，则锁定文档以进行注释。默认值为 false。

//            Object Password = System.Type.Missing;// 用来打开文档的密码字符串。（请参见下面的备注。）   

//            Object AddToRecentFiles = false; // 如果为 true，则将该文档添加到“文件”菜单上最近使用的文件列表中。默认值为 true。   

//            Object WritePassword = System.Type.Missing; // 用来保存对文件所做更改的密码字符串。（请参见下面的备注。）   

//            Object ReadOnlyRecommended = false;// 如果为 true，则让 Microsoft Office Word 在打开文档时建议只读状态。默认值为 false

//            Object EmbedTrueTypeFonts = false;//如果为 true，则将 TrueType 字体随文档一起保存。如果省略的话，则 EmbedTrueTypeFonts 参数假定 EmbedTrueTypeFonts 属性的值。

//            Object SaveNativePictureFormat = true;// 如果图形是从另一个平台（例如，Macintosh）导入的，则 true 表示仅保存导入图形的 Windows 版本。 

//            Object SaveFormsData = false;// 如果为 true，则将用户在窗体中输入的数据另存为数据记录。   

//            Object SaveAsAOCELetter = false;// 如果文档附加了邮件程序，则 true 表示会将文档另存为 AOCE 信函（邮件程序会进行保存）。   

//            Object Encoding = System.Type.Missing;// MsoEncoding。要用于另存为编码文本文件的文档的代码页或字符集。默认值是系统代码页。   

//            Object InsertLineBreaks = true;// 如果文档另存为文本文件，则 true 表示在每行文本末尾插入分行符。   

//            Object AllowSubstitutions = false;//如果文档另存为文本文件，则 true 允许 Word 将某些符号替换为外观与之类似的文本。例如，将版权符号显示为 (c)。默认值为 false。

//            Object LineEnding = Microsoft.Office.Interop.Word.WdLineEndingType.wdCRLF;// Word 在另存为文本文件的文档中标记分行符和换段符。可以是任何 WdLineEndingType 值。   

//            Object AddBiDiMarks = true;//如果为 true，则向输出文件添加控制字符，以便保留原始文档中文本的双向布局。   

//            try
//            {
//                wDoc.SaveAs(ref FileName, ref FileFormat, ref LockComments, ref Password, ref AddToRecentFiles, ref WritePassword, ref ReadOnlyRecommended, ref EmbedTrueTypeFonts, ref SaveNativePictureFormat, ref SaveFormsData, ref SaveAsAOCELetter, ref Encoding, ref InsertLineBreaks, ref AllowSubstitutions, ref LineEnding, ref AddBiDiMarks);
//                return true;
//            }
//            catch (Exception ex)
//            {
//                string err = string.Format("另存文件出错，错误原因：{0}", ex.Message);
//                throw new Exception(err, ex);
//            }
//        }
//        #endregion


//        #region 关闭文档
//        /// <summary>   
//        /// 关闭文档   
//        /// </summary>   
//        public void Close()
//        {
//            Close(wDoc, wApp);
//            wDoc = null;
//            wApp = null;
//        }
//        #endregion


//        #region 关闭文档
//        /// <summary>   
//        /// 关闭文档   
//        /// </summary>   
//        /// <param name="wDoc">Document对象</param>   
//        /// <param name="WApp">Application对象</param>  
//        public static void Close(Microsoft.Office.Interop.Word.Document wDoc, Microsoft.Office.Interop.Word.Application WApp)
//        {
//            Object SaveChanges = Microsoft.Office.Interop.Word.WdSaveOptions.wdSaveChanges;// 指定文档的保存操作。可以是下列 WdSaveOptions 值之一：wdDoNotSaveChanges、wdPromptToSaveChanges 或 wdSaveChanges

//            Object OriginalFormat = Microsoft.Office.Interop.Word.WdOriginalFormat.wdOriginalDocumentFormat;// 指定文档的保存格式。可以是下列 WdOriginalFormat 值之一：wdOriginalDocumentFormat、wdPromptUser 或 wdWordDocument。   

//            Object RouteDocument = false;// 如果为 true，则将文档传送给下一个收件人。如果没有为文档附加传送名单，则忽略此参数。   

//            try
//            {
//                if (wDoc != null)
//                    wDoc.Close(ref SaveChanges, ref OriginalFormat, ref RouteDocument);

//                if (WApp != null)
//                    WApp.Quit(ref SaveChanges, ref OriginalFormat, ref RouteDocument);

//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }

//        }
//        #endregion


//        #region
//        ///获取文档中所有标签
//        ///
//        public void GetDocumentBookmarkData(string FileName,ref string[] bookmarks)
//        {

//            Microsoft.Office.Interop.Word.Document wDoc = null;
//            Microsoft.Office.Interop.Word.Application wApp = null;
//            CreateNewWordDocument(FileName, ref wDoc, ref wApp);
//            object oEndOfDoc = "\\endofdoc";
//            object missing = System.Reflection.Missing.Value;
//            int i = 0;

//            System.Collections.IEnumerator enu = wApp.ActiveDocument.Bookmarks.GetEnumerator();

            
//            while (enu.MoveNext())
//            {
//                Microsoft.Office.Interop.Word.Bookmark bk = (Microsoft.Office.Interop.Word.Bookmark)enu.Current;

//               bookmarks[i++] = bk.Name.ToString();
//              // str += "{" + bk.Name.ToString() + ":" + bk.Range.Text + "}";
//            }

//            object o = null;

//            wDoc.Close(ref missing, ref missing, ref missing);
//            wApp.Quit(ref missing, ref missing, ref missing);

//            if (wDoc != null)
//            {
//                System.Runtime.InteropServices.Marshal.ReleaseComObject(wDoc);
//                wDoc = null;
//            }

//            if (wApp != null)
//            {
//                System.Runtime.InteropServices.Marshal.ReleaseComObject(wApp);
//                wApp = null;
//            }

//            GC.Collect();

//        }
//        #endregion


//        #region 填充书签
//        /// <summary>   
//        /// 填充书签   
//        /// </summary>   
//        /// <param name="bookmark">书签</param>   
//        /// <param name="value">值</param> 
//        public void Replace(string bookmark, string value)
//        {
//            try
//            {
//                object bkObj = bookmark;
//                if (wApp.ActiveDocument.Bookmarks.Exists(bookmark) == true)
//                {
//                    wApp.ActiveDocument.Bookmarks.get_Item(ref bkObj).Select();
//                }
//                else return;
//                wApp.Selection.TypeText(value);
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }
//        #endregion

//        public void Replace(string bookmark, DataGridView dgv)
//        {
//            Microsoft.Office.Interop.Word.Table mytable;
//            Microsoft.Office.Interop.Word.Document mydoc = wDoc;
//            Microsoft.Office.Interop.Word.Application wordApp = wApp;
//            Object myobj= System.Reflection.Missing.Value;        

//            try
//            {
//                object bkObj = bookmark;
//                if (wApp.ActiveDocument.Bookmarks.Exists(bookmark) == true)
//                {
//                    wApp.ActiveDocument.Bookmarks.get_Item(ref bkObj).Select();
//                }
//                else return;

//                if (dgv.RowCount > 0)
//                {
//                    Microsoft.Office.Interop.Word.Range rng3 = wordApp.Selection.Range;
//                    ////将数据生成Word表格文件
//                    mytable = mydoc.Tables.Add(rng3, dgv.RowCount, dgv.ColumnCount, ref myobj, ref myobj);
//                    mytable.Borders.Enable = 1;

//                    int intTableCnt = mydoc.Tables.Count;

                    
//                    //设置列宽
//                    //mydoc.Tables.Item(intTableCnt).Rows.HeightRule = Word.WdRowHeightRule.wdRowHeightAtLeast;
//                    //mydoc.Tables.Item(intTableCnt).Rows.Height = wordApp.CentimetersToPoints(float.Parse("0.8"));
//                    //mydoc.Tables.Item(intTableCnt).Range.Font.Size = 10;
//                    //mydoc.Tables.Item(intTableCnt).Range.Font.Name = "宋体";
//                    //mydoc.Tables.Item(intTableCnt).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
//                    //mydoc.Tables.Item(intTableCnt).Range.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;


//                    ////设置表格样式
//                    //mydoc.Tables.Item(intTableCnt).Borders.Item(Word.WdBorderType.wdBorderLeft).LineStyle = Word.WdLineStyle.wdLineStyleSingle;
//                    //mydoc.Tables.Item(intTableCnt).Borders.Item(Word.WdBorderType.wdBorderLeft).LineWidth = Word.WdLineWidth.wdLineWidth150pt;
//                    //mydoc.Tables.Item(intTableCnt).Borders.Item(Word.WdBorderType.wdBorderLeft).Color = Word.WdColor.wdColorAutomatic;

//                    //mydoc.Tables.Item(intTableCnt).Borders.Item(Word.WdBorderType.wdBorderTop).LineStyle = Word.WdLineStyle.wdLineStyleSingle;
//                    //mydoc.Tables.Item(intTableCnt).Borders.Item(Word.WdBorderType.wdBorderTop).LineWidth = Word.WdLineWidth.wdLineWidth150pt;
//                    //mydoc.Tables.Item(intTableCnt).Borders.Item(Word.WdBorderType.wdBorderTop).Color = Word.WdColor.wdColorAutomatic;

//                    //mydoc.Tables.Item(intTableCnt).Borders.Item(Word.WdBorderType.wdBorderBottom).LineStyle = Word.WdLineStyle.wdLineStyleSingle;
//                    //mydoc.Tables.Item(intTableCnt).Borders.Item(Word.WdBorderType.wdBorderBottom).LineWidth = Word.WdLineWidth.wdLineWidth150pt;
//                    //mydoc.Tables.Item(intTableCnt).Borders.Item(Word.WdBorderType.wdBorderBottom).Color = Word.WdColor.wdColorAutomatic;

//                    //mydoc.Tables.Item(intTableCnt).Borders.Item(Word.WdBorderType.wdBorderRight).LineStyle = Word.WdLineStyle.wdLineStyleSingle;
//                    //mydoc.Tables.Item(intTableCnt).Borders.Item(Word.WdBorderType.wdBorderRight).LineWidth = Word.WdLineWidth.wdLineWidth150pt;
//                    //mydoc.Tables.Item(intTableCnt).Borders.Item(Word.WdBorderType.wdBorderRight).Color = Word.WdColor.wdColorAutomatic;

//                    //mydoc.Tables.Item(intTableCnt).Borders.Item(Word.WdBorderType.wdBorderHorizontal).LineStyle = Word.WdLineStyle.wdLineStyleSingle;
//                    //mydoc.Tables.Item(intTableCnt).Borders.Item(Word.WdBorderType.wdBorderHorizontal).LineWidth = Word.WdLineWidth.wdLineWidth050pt;
//                    //mydoc.Tables.Item(intTableCnt).Borders.Item(Word.WdBorderType.wdBorderHorizontal).Color = Word.WdColor.wdColorAutomatic;

//                    //mydoc.Tables.Item(intTableCnt).Borders.Item(Word.WdBorderType.wdBorderVertical).LineStyle = Word.WdLineStyle.wdLineStyleSingle;
//                    //mydoc.Tables.Item(intTableCnt).Borders.Item(Word.WdBorderType.wdBorderVertical).LineWidth = Word.WdLineWidth.wdLineWidth050pt;
//                    //mydoc.Tables.Item(intTableCnt).Borders.Item(Word.WdBorderType.wdBorderVertical).Color = Word.WdColor.wdColorAutomatic;


//                    wordApp.Selection.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;//设置右对齐

//                    mytable.Columns.SetWidth(100, Microsoft.Office.Interop.Word.WdRulerStyle.wdAdjustNone);

//                    //输出列标题数据
//                    for (int i = 0; i < dgv.ColumnCount; i++)
//                    {
//                        mytable.Cell(1, i + 1).Range.Text=dgv.Columns[i].HeaderText;
//                    }

//                    //输出控件中的记录
//                    for (int i = 0; i < dgv.RowCount - 1; i++)
//                    {
//                        for (int j = 0; j < dgv.ColumnCount; j++)
//                        {
//                            //MessageBox.Show(dgv[j, i].Value.ToString());
//                            if (dgv[j, i].Value!=null)
//                            mytable.Cell(i + 2, j + 1).Range.Text=dgv[j, i].Value.ToString();
//                        }
//                        System.Windows.Forms.Application.DoEvents();
//                    }

//                    mydoc.Paragraphs.Last.Range.Text = "\n";
//                }

//               // wApp.Selection.ta
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show(ex.Message);
//                //throw ex;
//            }
            
//        }

//        /// <summary>
//        /// 插入图片至书签处
//        /// </summary>
//        /// <param name="bookmark">书签</param>
//        /// <param name="value">图片路径</param>
//        /// <param name="type">插入图片环绕方式</param>
//        public void Replace(string bookmark, string value, string type)
//        { 
        
//        }


//        //查找表格
//        public bool FindTable(string bookmarkTable)
//        {
//            try
//            {
//                object bkObj = bookmarkTable;
//                if (wApp.ActiveDocument.Bookmarks.Exists(bookmarkTable) == true)
//                {
//                    wApp.ActiveDocument.Bookmarks.get_Item(ref bkObj).Select();
//                    return true;
//                }
//                else
//                    return false;
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }

//        }

//        //移动到下一个单元格
//        public void MoveNextCell()
//        {
//            try
//            {
//                Object unit = Microsoft.Office.Interop.Word.WdUnits.wdCell;
//                Object count = 1;
//                wApp.Selection.Move(ref unit, ref count);
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }

//        //填充单元格内容
//        public void SetCellValue(string value)
//        {
//            try
//            {
//                wApp.Selection.TypeText(value);
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }

//        }

//        //移动到下一行
//        public void MoveNextRow()
//        {
//            try
//            {
//                Object extend = Microsoft.Office.Interop.Word.WdMovementType.wdExtend;
//                Object unit = Microsoft.Office.Interop.Word.WdUnits.wdCell;
//                Object count = 1;
//                wApp.Selection.MoveRight(ref unit, ref count, ref extend);
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }

//        }

        
//    }
//}
