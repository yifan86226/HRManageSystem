using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESRI.ArcGIS.Client.Tasks;

namespace Best.VWPlatform.Common.ExportWord
{
    /// <summary>
    /// 图片,列表导出到word中
    /// </summary>
    public class ExportWord
    {
        public Microsoft.Office.Interop.Word._Application oWord;
        public Microsoft.Office.Interop.Word._Document oDoc;
        public object oMissing = System.Reflection.Missing.Value;
        public ExportWord()
        {
            oWord = new Microsoft.Office.Interop.Word.Application();
        }
        /// <summary>
        /// 创建word
        /// </summary>
        public void CreatWord()
        {
            oDoc = oWord.Documents.Add(ref oMissing, ref oMissing,
            ref oMissing, ref oMissing);
        }
        /// <summary>
        /// word中创建一个段落
        /// </summary>
        /// <param name="odoc"></param>
        /// <param name="bookmark">书签</param>
        /// <param name="oRng">书签位置</param>
        /// <param name="text">显示文本</param>
        /// <param name="blod">加粗</param>
        /// <param name="spaceafter">段间间隔</param>
        /// <param name="aliment">对齐方式</param>
        public void CreatParagraph(Microsoft.Office.Interop.Word._Document odoc, object bookmark, ref object oRng, string text, int blod, int spaceafter, Microsoft.Office.Interop.Word.WdParagraphAlignment aliment)
        {
            Microsoft.Office.Interop.Word.Paragraph oPara;
            if (bookmark != null)
                oRng = odoc.Bookmarks.get_Item(ref bookmark).Range;
            oPara = odoc.Content.Paragraphs.Add(ref oRng);
            oPara.Range.Text = text;
            oPara.Range.Font.Bold = blod;
            oPara.Format.SpaceAfter = spaceafter;
            oPara.Alignment = aliment;
            oPara.Range.InsertParagraphAfter();
        }
        
        /// <summary>
        /// 向Word书签的位置插入表格
        /// </summary>
        /// <param name="odoc"></param>
        /// <param name="bookmark"></param>
        /// <param name="oMissing"></param>
        /// <returns></returns>
        public Microsoft.Office.Interop.Word.Table AddTableToWord(Microsoft.Office.Interop.Word._Document odoc, ref Microsoft.Office.Interop.Word.Range wrdRng, object bookmark, ref object oMissing, int row, int col, int space)
        {
            Microsoft.Office.Interop.Word.Table oTable;
            wrdRng = odoc.Bookmarks.get_Item(ref bookmark).Range;
            oTable = odoc.Tables.Add(wrdRng, row, col, ref oMissing, ref oMissing);
            oTable.Range.ParagraphFormat.SpaceAfter = space;
            //设置表格样式
            oTable.Borders.OutsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
            oTable.Borders.InsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
            return oTable;
        }
        /// <summary>
        /// 设置表格文本和字体粗细
        /// </summary>
        /// <param name="table">表</param>
        /// <param name="row">行</param>
        /// <param name="col">列</param>
        /// <param name="blod">字体粗细</param>
        /// <param name="text">显示文本</param>
        public void SetTableTextAndFont(ref Microsoft.Office.Interop.Word.Table table, int row, int col, int blod, string text,int size)
        {
            table.Cell(row, col).Range.Text = text;
            table.Cell(row, col).Range.Font.Bold = blod;
            table.Cell(row, col).Range.Font.Size = size;
        }
        /// <summary>
        /// 向Word书签的位置插入图片
        /// </summary>
        /// <param name="odoc"></param>
        /// <param name="bookmark">书签</param>
        /// <param name="picturePath">图片路径</param>
        /// <param name="width">图片宽度设定</param>
        /// <param name="hight">图片高度设定</param>
        public void AddPictureToWord(Microsoft.Office.Interop.Word._Document odoc, object bookmark, string picturePath, float width, float hight)
        {
            if (!File.Exists(picturePath))
            {
                return;
            }

            Microsoft.Office.Interop.Word.InlineShape oShape;
            Microsoft.Office.Interop.Word.Range imageRng = odoc.Bookmarks.get_Item(ref bookmark).Range;
            object LinkToFile = false;
            object SaveWithDocument = true;
            object range = imageRng;
            oShape = imageRng.InlineShapes.AddPicture(picturePath, ref LinkToFile, ref SaveWithDocument, ref range);
            oShape.Width = width;//图片宽度 
            oShape.Height = hight;//图片高度
        }

        /// <summary>
        /// 记录书签位置
        /// </summary>
        /// <param name="odoc"></param>
        /// <param name="bookmark"></param>
        /// <param name="oRng"></param>
        /// <param name="spaceafter"></param>
        public void BookmarkPos(Microsoft.Office.Interop.Word._Document odoc, object bookmark, ref Microsoft.Office.Interop.Word.Range wrdRng, int spaceafter)
        {
            wrdRng = odoc.Bookmarks.get_Item(ref bookmark).Range;
            wrdRng.ParagraphFormat.SpaceAfter = spaceafter;
            wrdRng.InsertParagraphAfter();
        }
        /// <summary>
        /// 添加分页
        /// </summary>
        public void AddPageing(Microsoft.Office.Interop.Word._Document odoc, object bookmark, ref Microsoft.Office.Interop.Word.Range wrdRng, int spaceafter)
        {
            object oPos;
            double dPos = oWord.InchesToPoints(7);

            do
            {
                wrdRng = oDoc.Bookmarks.get_Item(ref bookmark).Range;
                wrdRng.ParagraphFormat.SpaceAfter = spaceafter;
                wrdRng.InsertParagraphAfter();
                oPos = wrdRng.get_Information
                                           (Microsoft.Office.Interop.Word.WdInformation.wdVerticalPositionRelativeToPage);
            }
            while (dPos >= Convert.ToDouble(oPos));

            object oCollapseEnd = Microsoft.Office.Interop.Word.WdCollapseDirection.wdCollapseEnd;
            object oPageBreak = Microsoft.Office.Interop.Word.WdBreakType.wdPageBreak;
            wrdRng.Collapse(ref oCollapseEnd);
            wrdRng.InsertBreak(ref oPageBreak);
            wrdRng.Collapse(ref oCollapseEnd);
            wrdRng.InsertParagraphAfter();
        }
        /// <summary>
        /// 文件保存
        /// </summary>
        /// <param name="filename"></param>
        public void SaveToWord(object filename)
        {
            //文件保存
            oDoc.SaveAs(ref filename, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);
            oDoc.Close(ref oMissing, ref oMissing, ref oMissing);
            oWord.Quit(ref oMissing, ref oMissing, ref oMissing);
        }

        /// <summary>
        /// 文件打开
        /// </summary>
        /// <param name="filename"></param>
        public void OpenWord(object filename)
        {
            try
            {
                oWord.Visible = true;
                oDoc = oWord.Documents.Open(ref filename,
                ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing); 
            }
            catch (Exception)
            {
                return;
            }
            
        }
    }
}
