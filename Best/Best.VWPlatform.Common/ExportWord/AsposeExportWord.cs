//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Aspose.Words;
//using System.Data;
//using Aspose.Words.Tables;
//using System.Collections.ObjectModel;
//using Best.VWPlatform.Common.Rmtp.MeasureHandler;
//using Best.VWPlatform.Common.Types;
//using Best.VWPlatform.Common.Utility;
//using Best.VWPlatform.Common.Rmtp.DataFrames;

//namespace Best.VWPlatform.Common.ExportWord
//{
//    public class AsposeExportWord
//    {
//        private Document _document;
//        private DocumentBuilder _builder;

//        public void CreateDoc()
//        {
//            _document = new Document();
//            _builder = new DocumentBuilder(_document);
//        }

//        /// <summary>  
//        /// 保存文件  
//        /// </summary>  
//        /// <param name="strFileName"></param>  
//        public void SaveAs(string strFileName)
//        {
//            _document.Save(strFileName, SaveFormat.Doc);
//        }

//        /// <summary>  
//        /// 添加内容换行 
//        /// </summary>  
//        /// <param name="strText"></param>  
//        public void InsertText(string strText, double conSize, bool conBold, string conAlign)
//        {
//            _builder.Bold = conBold;
//            _builder.Font.Size = conSize;
//            switch (conAlign)
//            {
//                case "left":
//                    _builder.ParagraphFormat.Alignment = ParagraphAlignment.Left;
//                    break;
//                case "center":
//                    _builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
//                    break;
//                case "right":
//                    _builder.ParagraphFormat.Alignment = ParagraphAlignment.Right;
//                    break;
//                default:
//                    _builder.ParagraphFormat.Alignment = ParagraphAlignment.Left;
//                    break;
//            }
//            _builder.Writeln(strText);
//        }

//        /// <summary>  
//        /// 添加内容不换行  
//        /// </summary>  
//        /// <param name="strText"></param>  
//        public void WriteText(string strText, double conSize, bool conBold, string conAlign)
//        {
//            _builder.Bold = conBold;
//            _builder.Font.Size = conSize;
//            switch (conAlign)
//            {
//                case "left":
//                    _builder.ParagraphFormat.Alignment = ParagraphAlignment.Left;
//                    break;
//                case "center":
//                    _builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
//                    break;
//                case "right":
//                    _builder.ParagraphFormat.Alignment = ParagraphAlignment.Right;
//                    break;
//                default:
//                    _builder.ParagraphFormat.Alignment = ParagraphAlignment.Left;
//                    break;
//            }
//            _builder.Write(strText);
//        }
//        /// <summary>
//        /// 创建设备信息表
//        /// </summary>
//        /// <param name="row"></param>
//        /// <param name="column"></param>
//        public void CreateDeviceTable(DataTable dt)
//        {
//            if (dt == null)
//                return;
//            Table table = _builder.StartTable();//开始画Table  
//            ParagraphAlignment paragraphAlignmentValue = _builder.ParagraphFormat.Alignment;
//            _builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;

//            for (var i = 0; i < dt.Rows.Count + 1; i++)
//            {
//                _builder.RowFormat.Height = 25;
//                for (var j = 0; j < dt.Columns.Count; j++)
//                {
//                    _builder.InsertCell();// 添加一个单元格
//                    _builder.CellFormat.Borders.LineStyle = LineStyle.Single;
//                    _builder.CellFormat.Borders.Color = System.Drawing.Color.Black;
//                    _builder.Font.Size = 10;
//                    _builder.Font.Name = "宋体";
//                    _builder.CellFormat.HorizontalMerge = CellMerge.None;
//                    _builder.CellFormat.VerticalMerge = CellMerge.None;
//                    _builder.CellFormat.VerticalAlignment = Aspose.Words.Tables.CellVerticalAlignment.Center;//垂直居中对齐  
//                    _builder.ParagraphFormat.Alignment = ParagraphAlignment.Left;//水平居左对齐  
//                    _builder.CellFormat.Width = 150.0;
//                    _builder.Font.Bold = false;

//                    if (i == 0)
//                    {
//                        if (j == 0)
//                        {
//                            _builder.CellFormat.HorizontalMerge = CellMerge.First;
//                            _builder.Font.Bold = true;
//                            _builder.Write("设备信息");
//                        }
//                        else
//                        {
//                            _builder.CellFormat.HorizontalMerge = CellMerge.Previous;
//                        }
//                        continue;
//                    }

//                    if (j % 2 == 0)
//                        _builder.Font.Bold = true;

//                    _builder.Write(dt.Rows[i - 1][j].ToString());
//                }
//                _builder.EndRow();
//            }

//            _builder.EndTable();
//            _builder.ParagraphFormat.Alignment = paragraphAlignmentValue;
//        }

//        public List<string> CreateDeviceList(DeviceAbilityDataFrame abilityDataFrame,string pos)
//        {
//            List<string> devicelist = new List<string>();
//            devicelist.Add("生产厂家");
//            devicelist.Add(abilityDataFrame.Manufacturer);
//            devicelist.Add("监测位置");
//            devicelist.Add(pos);
//            devicelist.Add("类别");
//            devicelist.Add(abilityDataFrame.DeviceType);
//            devicelist.Add("型号");
//            devicelist.Add(abilityDataFrame.Model);
//            return devicelist;
//        }
//        /// <summary>
//        /// 创建射频全景参数信息表
//        /// </summary>
//        /// <param name="row"></param>
//        /// <param name="column"></param>
//        public void CreateParamTable(DataTable dtparam)
//        {
//            if (dtparam == null)
//                return;
//            Table table = _builder.StartTable();//开始画Table  
//            ParagraphAlignment paragraphAlignmentValue = _builder.ParagraphFormat.Alignment;
//            _builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;

//            for (var i = 0; i < dtparam.Rows.Count + 1; i++)
//            {
//                _builder.RowFormat.Height = 25;
//                for (var j = 0; j < dtparam.Columns.Count; j++)
//                {
//                    _builder.InsertCell();// 添加一个单元格
//                    _builder.CellFormat.Borders.LineStyle = LineStyle.Single;
//                    _builder.CellFormat.Borders.Color = System.Drawing.Color.Black;
//                    _builder.Font.Size = 10;
//                    _builder.Font.Name = "宋体";
//                    _builder.CellFormat.HorizontalMerge = CellMerge.None;
//                    _builder.CellFormat.VerticalMerge = CellMerge.None;
//                    _builder.CellFormat.VerticalAlignment = Aspose.Words.Tables.CellVerticalAlignment.Center;//垂直居中对齐  
//                    _builder.ParagraphFormat.Alignment = ParagraphAlignment.Left;//水平居左对齐  
//                    _builder.CellFormat.Width = 150.0;
//                    _builder.Font.Bold = false;

//                    if(i == 0 )
//                    {
//                        if (j == 0)
//                        {
//                            _builder.CellFormat.HorizontalMerge = CellMerge.First;
//                            _builder.Font.Bold = true;
//                            _builder.Write("参数信息");
//                        }
//                        else
//                        {
//                            _builder.CellFormat.HorizontalMerge = CellMerge.Previous;
//                        }
//                        continue;
//                    }

//                    if(j % 2 == 0)
//                        _builder.Font.Bold = true;

//                    _builder.Write(dtparam.Rows[i - 1][j].ToString());
//                }
//                _builder.EndRow();
//            }

//            _builder.EndTable();
//            _builder.ParagraphFormat.Alignment = paragraphAlignmentValue;
//        }

//        /// <summary>
//        /// 创建中频全景ITU测量结果表格
//        /// </summary>
//        /// <param name="paramlist"></param>
//        public void CreateIfqexItuTable(DataTable dt)
//        {
//            if (dt == null)
//                return;
//            Table table = _builder.StartTable();//开始画Table  
//            ParagraphAlignment paragraphAlignmentValue = _builder.ParagraphFormat.Alignment;
//            _builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;

//            for (var i = 0; i < dt.Rows.Count; i++)
//            {
//                _builder.RowFormat.Height = 25;
//                for (var j = 0; j < dt.Columns.Count; j++)
//                {
//                    _builder.InsertCell();// 添加一个单元格
//                    _builder.CellFormat.Borders.LineStyle = LineStyle.Single;
//                    _builder.CellFormat.Borders.Color = System.Drawing.Color.Black;
//                    _builder.Font.Size = 10;
//                    _builder.Font.Name = "宋体";
//                    _builder.CellFormat.HorizontalMerge = CellMerge.None;
//                    _builder.CellFormat.VerticalMerge = CellMerge.None;
//                    _builder.CellFormat.VerticalAlignment = Aspose.Words.Tables.CellVerticalAlignment.Center;//垂直居中对齐  
//                    _builder.ParagraphFormat.Alignment = ParagraphAlignment.Left;//水平居左对齐  
//                    _builder.CellFormat.Width = 150.0;
//                    _builder.Font.Bold = false;

//                    if (i == 0)
//                    {
//                        if (j == 2 || j == 3)
//                        {
//                            _builder.Font.Bold = true;
//                        }
//                    }

//                    if ((i == 4 || i == 7) && j == 0)
//                    {
//                        _builder.CellFormat.VerticalMerge = CellMerge.First;
//                    }

//                    if ((i == 5 || i == 6 || i == 8 || i == 9) && j == 0)
//                    {
//                        _builder.CellFormat.VerticalMerge = CellMerge.Previous;
//                    }

//                    _builder.Write(dt.Rows[i][j].ToString());
//                }
//                _builder.EndRow();
//            }

//            _builder.EndTable();
//            _builder.ParagraphFormat.Alignment = paragraphAlignmentValue;
//        }

//        public DataTable CreatDataTable(int row, int col, List<string> datalist)
//        {
//            if (datalist == null || datalist.Count < (row * col))
//                return null;

//            DataTable dt = new DataTable("列表");
//            for (int i = 0; i < col; i++)
//                dt.Columns.Add(string.Format("{0}",i.ToString()));

//            DataRow datarow = null;
//            for (int i = 0; i < row; i++)
//            {
//                datarow = dt.NewRow();
//                for (int j = 0; j < col; j++)
//                    datarow[j.ToString()] = datalist[i * col + j];

//                dt.Rows.Add(datarow);
//            }

//            return dt;
//        }
//        /// <summary>
//        /// 信号列表
//        /// </summary>
//        /// <param name="row"></param>
//        /// <param name="signalList"></param>
//        /// <param name="scanType"></param>
//        /// <returns></returns>
//        public DataTable CreatSignalDataTable(int row, ObservableCollection<SignalStatisticsItem> signalList,string scanType)
//        {
//            if (signalList == null || signalList.Count < row)
//                return null;

//            DataTable nameList = new DataTable("信号列表");
//            nameList.Columns.Add("性质");
//            nameList.Columns.Add("频率");
//            nameList.Columns.Add("带宽");
//            nameList.Columns.Add("场强");
//            nameList.Columns.Add("占用度");
//            nameList.Columns.Add("第一次捕获时间");
//            nameList.Columns.Add("最后一次捕获时间");
//            nameList.Columns.Add("台站名称");

//            DataRow datarow = null;
//            for (int i = 0; i < row; i++)
//            {
//                var xz = SignalDescribe.GetSignalText((SignalDescribe.SignalType)signalList[i].Nature);
//                datarow = nameList.NewRow();
//                datarow["性质"] = string.IsNullOrWhiteSpace(xz) ? "/" : xz;
//                datarow["频率"] = string.Format("{0:N3}", signalList[i].Frequency);
//                datarow["带宽"] = string.Format("{0:N3}", signalList[i].Bandwidth);
//                datarow["场强"] = string.Format("{0}",scanType.Equals("MSCAN") ? "/" : string.Format("{0:N2}", Utile.MathNoRound((double)signalList[i].FieldStrength, 2)));
//                datarow["占用度"] = string.Format("{0}",scanType.Equals("MSCAN") ? "/" : Utile.MathNoRound(signalList[i].OccupancyRate, 2).ToString());
//                datarow["第一次捕获时间"] = string.Format("{0}",scanType.Equals("MSCAN") ? "/" : signalList[i].FirstTime.ToString());
//                datarow["最后一次捕获时间"] = string.Format("{0}",scanType.Equals("MSCAN") ? "/" : signalList[i].LastTime.ToString());
//                datarow["台站名称"] = string.IsNullOrEmpty(signalList[i].Belong) ? "/" : signalList[i].Belong;
//                nameList.Rows.Add(datarow);
//            }

//            return nameList;
//        }

//        /// <summary>
//        /// 插入图片
//        /// </summary>
//        /// <param name="imagefile"></param>
//        public void InsertImage(string imagefile, double width, double height)
//        {
//            ParagraphAlignment paragraphAlignmentValue = _builder.ParagraphFormat.Alignment;
//            _builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
//            _builder.InsertImage(imagefile, width, height);
//            _builder.ParagraphFormat.Alignment = paragraphAlignmentValue;
//        }
//        /// <summary>
//        /// 创建信号列表
//        /// </summary>
//        /// <param name="count"></param>
//        public void CreateSignalTable(DataTable dt)
//        {
//            Table table = _builder.StartTable();
//            ParagraphAlignment paragraphAlignmentValue = _builder.ParagraphFormat.Alignment;
//            _builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;

//            _builder.RowFormat.Height = 20.0;
//            _builder.CellFormat.Shading.BackgroundPatternColor = System.Drawing.Color.FromArgb(198, 217, 241);
//            _builder.ParagraphFormat.Alignment = ParagraphAlignment.Left;
//            _builder.CellFormat.Borders.LineStyle = LineStyle.Single;
//            _builder.CellFormat.Borders.Color = System.Drawing.Color.Black;
//            _builder.Font.Size = 7;
//            _builder.Font.Name = "宋体";
//            _builder.Font.Bold = true;

//            _builder.InsertCell();
//            _builder.Write("性质");

//            _builder.InsertCell();
//            _builder.Write("频率(MHz)");

//            _builder.InsertCell();
//            _builder.Write("带宽(kHz)");

//            _builder.InsertCell();
//            _builder.Write("场强(dBμV/m)");

//            _builder.InsertCell();
//            _builder.Write("占用度(%)");

//            _builder.InsertCell();
//            _builder.Write("第一次捕获时间");

//            _builder.InsertCell();
//            _builder.Write("最后一次捕获时间");

//            _builder.InsertCell();
//            _builder.Write("台站名称");

//            _builder.EndRow();

//            List<double> widthList = new List<double>();
//            for (int i = 0; i < dt.Columns.Count; i++)
//            {
//                _builder.MoveToCell(2, 0, i, 0); //移动单元格
//                double width = _builder.CellFormat.Width;//获取单元格宽度
//                widthList.Add(width);
//            }


//            _builder.Bold = false;
//            for (var i = 0; i < dt.Rows.Count; i++)
//            {
//                for (var j = 0; j < dt.Columns.Count; j++)
//                {
//                    _builder.InsertCell();// 添加一个单元格 
//                    _builder.CellFormat.Shading.BackgroundPatternColor = System.Drawing.Color.Transparent;
//                    _builder.CellFormat.Borders.LineStyle = LineStyle.Single;
//                    _builder.CellFormat.Borders.Color = System.Drawing.Color.Black;
//                    _builder.CellFormat.Width = widthList[j];
//                    _builder.CellFormat.VerticalMerge = Aspose.Words.Tables.CellMerge.None;
//                    _builder.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;//垂直居中对齐
//                    _builder.ParagraphFormat.Alignment = ParagraphAlignment.Left; //水平居左对齐
//                    _builder.Write(dt.Rows[i][j].ToString());
//                }
//                _builder.EndRow();
//            }
//            _builder.EndTable();
//            _builder.ParagraphFormat.Alignment = paragraphAlignmentValue;
//        }

//        /// <summary>
//        /// 插入书签
//        /// </summary>
//        /// <param name="BookMark"></param>
//        public void InsertBookMark(string BookMark)
//        {
//            _builder.StartBookmark(BookMark);
//            _builder.EndBookmark(BookMark);
//        }
//        /// <summary>
//        /// 定位到指定书签
//        /// </summary>
//        /// <param name="strBookMarkName"></param>
//        public void GotoBookMark(string strBookMarkName)
//        {
//            _builder.MoveToBookmark(strBookMarkName);
//            //Bookmark bookmark = _document.Range.Bookmarks["strBookMarkName"];
//        }
//        /// <summary>
//        /// 清除书签
//        /// </summary>
//        public void ClearBookMark()
//        {
//            _document.Range.Bookmarks.Clear();
//        }

//        /// <summary>  
//        /// 换行  
//        /// </summary>  
//        public void InsertLineBreak()
//        {
//            _builder.InsertBreak(BreakType.LineBreak);
//        }
//        /// <summary>  
//        /// 换多行  
//        /// </summary>  
//        /// <param name="nline"></param>  
//        public void InsertLineBreak(int nline)
//        {
//            for (int i = 0; i < nline; i++)
//                _builder.InsertBreak(BreakType.LineBreak);
//        }
//        /// <summary>
//        /// 设置行间距
//        /// </summary>
//        public void SetLineSpace(double space)
//        {
//            _builder.ParagraphFormat.LineSpacing = space;
//        }
//    }
//}
