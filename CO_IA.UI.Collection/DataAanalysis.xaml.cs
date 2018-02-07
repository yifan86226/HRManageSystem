using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CO_IA.UI.Collection.ViewModel;
using System.Collections.ObjectModel;
using DevExpress.Xpf.NavBar;
using CO_IA.UI.Collection.DataAnalysis;
using CO_IA.UI.Collection.DbEntity;
using CO_IA.UI.Collection.Model;
using CO_IA.Data.Collection;
using System.ComponentModel;
using DevExpress.Xpf.Charts;
using ZedGraph;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay;
using CO_IA.Data;
using CO_IA.Client;
using AT_BC.Data;

namespace CO_IA.UI.Collection
{
    public delegate void DeleteNavBar();
    public delegate void RefreshAnalysisResultDelete(ObservableCollection<AnalysisResult> analysisResultList);
    public delegate void RefreshChartResultDelete(ChartSeriesPoints chartSeriesPoints);
    public delegate void RefreshChartFreqStatDelete(Dictionary<double, FreqStatModel> dicFreqStatModel, ChartSeriesPoints chartSeriesPoints);
    public delegate void NavBarClickDelegate(object sender, EventArgs e);
    public delegate void FreqNavBarButtonClickDelegate(object sender, RoutedEventArgs e);
    public delegate void ReminderBoxDelegate(string message);
    public delegate void SetCollectionLabelDelegate(FreqCollectionIndex freqCollectionIndex);
    public delegate void InitComboxDataDelegate();
   
    /// <summary>
    /// DataAanalysis.xaml 的交互逻辑
    /// </summary>
    public partial class DataAanalysis : UserControl
    {
        //点击台站超链接触发 参数为频率GUID 用于主界面调用，显示台站详细信息
        public event Action<string> StationHyperlinkClick;
        public EnTryModeEnum EnTryMode = EnTryModeEnum.采集端;
        ObservableCollection<AnalysisResult> freqList;
        //SeriesPointCollection maxSeriesPointCollection;
        //SeriesPointCollection midSeriesPointCollection;
        CollectionDataSave cds;
        //LineItem myCurve;
        //LineItem myCurve2;
        ObservableCollection<FreqCollectionIndex> data;
        private ActivityPlaceInfo activityPlaceInfo;
        public DataAanalysis()
        {
            InitializeComponent();
            initComboxData();
            ControlDisplay();
            //initZedGraph();
        }

        public DataAanalysis(ActivityPlaceInfo activityPlaceInfo)
        {
            InitializeComponent();
            //initComboxData();
            this.activityPlaceInfo = activityPlaceInfo;
        }
        public void initDataSource(ObservableCollection<AnalysisResult> AnalysisList)
        {
            freqList = AnalysisList;
            this.gridControl_freqInfo.Dispatcher.Invoke(
            new Action(
                    delegate
                    {
                        //if (freqList!=null && freqList.Count > 0 )
                        //{
                        //    foreach (var f in freqList)
                        //    {
                        //        f.StationName = "测试台站名称";
                        //    }
                        //}
                        gridControl_freqInfo.ItemsSource = freqList;
                    }
                )
            );
        }

        /// <summary>
        /// 控制管理端界面显示
        /// </summary>
        public void ControlDisplay()
        {
            if (EnTryMode == EnTryModeEnum.管理端)
            {
                lab_source.Content = "活动频率规划";
                lab_source.FontSize = 24;
                lab_source.Foreground = Brushes.Orange;
                lab_collectIndex.Visibility = System.Windows.Visibility.Collapsed;
                btn_ImportFreq.Visibility = System.Windows.Visibility.Collapsed;
                btn_freqRange.Visibility = System.Windows.Visibility.Collapsed;
                img_select.Visibility = System.Windows.Visibility.Collapsed;
                border_button.Visibility = System.Windows.Visibility.Collapsed;
                lp_spectrum.Visibility = System.Windows.Visibility.Collapsed;
                fillNavGroupDataSouce();
            }
        }
        /// <summary>
        /// 初始化采集列表
        /// </summary>
        private void initComboxData()
        {
            cds = new CollectionDataSave();
            cds.openSQLiteConnection();
            data = cds.getMeasureIds();
            if (data.Count > 0)
            {
                lab_collectIndex.Content = data[0].CurrentActivityPlaceName + "  " + data[0].DisplayMem;
                lab_collectIndex.Tag = data[0];
                //defaultComboBox.ItemsSource = data;
            }
        }

        /// <summary>
        /// 初始化采集列表
        /// </summary>
        private void initZedGraph()
        {
            //zedGraphControlOccpy.GraphPane.YAxis.Title.FontSpec.Size = 24;
            //zedGraphControlOccpy.GraphPane.XAxis.Title.FontSpec.Size = 24;
            //this.zedGraphControlOccpy.GraphPane.YAxis.Title.Text = "占用度";
            //this.zedGraphControlOccpy.GraphPane.XAxis.Title.Text = "频率";

            //zedGraphControl.GraphPane.YAxis.Title.FontSpec.Size = 24;
            //zedGraphControl.GraphPane.XAxis.Title.FontSpec.Size = 24;
            //this.zedGraphControl.GraphPane.YAxis.Title.Text = "(dBμV)";
            //this.zedGraphControl.GraphPane.XAxis.Title.Text = "频率";
        }


        /// <summary>
        /// 手动增加频段
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_freqRange_Click(object sender, RoutedEventArgs e)
        {
            NavBarFreqAdd nbfa = new NavBarFreqAdd(navBar);
            nbfa.Width = 300;
            nbfa.Height = 300;
            nbfa.navBarClickDelegate = NavBarGroup_Click;
            nbfa.freqNavBarButtonClickDelegate = FreqNavBarButtonClick;
            nbfa.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            nbfa.ShowDialog(this);
            //NavBarGroup nbg = new NavBarGroup();
            //nbg.Header = "频率段 90--98MHz";
            //FreqNavBar fnb = new FreqNavBar();
            //nbg.Items.Add(fnb);
            //navBar.Groups.Add(nbg);
        }

        /// <summary>
        /// 导航条事件采集端
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NavBarGroup_Click(object sender, EventArgs e)
        {
            busyIndicator.IsBusy = true;
            NavBarGroup nbg = (NavBarGroup)sender;
            //NavBarItem nbi = (NavBarItem)nbg.Content;
            FreqNavBar fnr = (FreqNavBar)nbg.Items[0];
            cds.MeasureID = ((FreqCollectionIndex)lab_collectIndex.Tag).MeasureID;
            fnr.PlaceGuid = LoginService.CurrentActivityPlace.Guid;
            //MessageBox.Show("点HEADER变谱图" + fnr.FreqStart + "<>" + fnr.FreqStop + "<>"+fnr.BandWidth);
            AnalysisFreqData(fnr, false);
        }

        /// <summary>
        /// 导航条事件管理端
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NavBarGroupManager_Click(object sender, EventArgs e)
        {
            //busyIndicator.IsBusy = true;

            NavBarGroup nbg = (NavBarGroup)sender;
            //NavBarItem nbi = (NavBarItem)nbg.Content;
            FreqNavBar fnr = (FreqNavBar)nbg.Items[0];
            //cds.MeasureID = ((FreqCollectionIndex)lab_collectIndex.Tag).MeasureID;
            fnr.PlaceGuid = activityPlaceInfo.Guid;
            //MessageBox.Show("点HEADER变谱图" + fnr.FreqStart + "<>" + fnr.FreqStop + "<>"+fnr.BandWidth);
            //AnalysisFreqData(fnr, false);
            DrawChartByOraData(fnr);
        }

        //管理端绘制图形用oracle数据
        private void DrawChartByOraData(FreqNavBar fnr)
        {
            List<AnalysisResult> analysisList = new List<AnalysisResult>();
            //fnr.FreqStart = "80";
            //fnr.FreqStop = "107";
            //fnr.PlaceGuid = "0638536e6c5d4a358b579f94d8f170b1";
            //测试 34，66，0638536e6c5d4a358b579f94d8f170b1
            //PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan>(channel =>
            //{
            //    analysisList = channel.GetAnalysisResultList(Convert.ToDouble(fnr.FreqStart), Convert.ToDouble(fnr.FreqStop), fnr.PlaceGuid);
            //});            

            analysisList = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqStation.I_CO_IA_FreqStation, List<AnalysisResult>>(channel =>
                {
                     return channel.GetAnalysisResultList(Convert.ToDouble(fnr.FreqStart), Convert.ToDouble(fnr.FreqStop), fnr.PlaceGuid);
                });

            if (analysisList.Count > 0)
            {
                freqList = new ObservableCollection<AnalysisResult>(analysisList);
                initDataSource(freqList);
                removeBar();
                foreach (AnalysisResult ar in analysisList)
                {
                    if (ar.Occupy == 0)
                        continue;
                    RectangleHighlight rh = new RectangleHighlight();
                    rh.Bounds = new Microsoft.Research.DynamicDataDisplay.DataRect(new Rect(ar.Frequency, 0, 0, ar.Occupy));
                    rh.Stroke = Brushes.Yellow;
                    rh.StrokeThickness = 10;
                    rh.ToolTip = ar.Frequency.ToString("0.0000") + "MHz";
                    BarPlotter.Children.Add(rh);
                }
                BarPlotter.MainHorizontalAxis.Background = Brushes.White;
                BarPlotter.MainVerticalAxis.Background = Brushes.White;
                BarPlotter.MainHorizontalAxis.Foreground = Brushes.Orange;
                BarPlotter.MainVerticalAxis.Foreground = Brushes.Orange;
                BarPlotter.Visible = new Microsoft.Research.DynamicDataDisplay.DataRect(new Rect(Convert.ToDouble(fnr.FreqStart), 0, Convert.ToDouble(fnr.FreqStop) - Convert.ToDouble(fnr.FreqStart), 100));
            }
            else
            {
                MessageBox.Show("未查询到分析结果数据！");
            }
        }

        /// <summary>
        /// 管理端自动填充频率规划列表
        /// </summary>
        private void fillNavGroupDataSouce()
        {
            //List<FreqPlanActivity> freqPlanList = GetActivityFreqPlanInfoSource(activityPlaceInfo.Guid);
            //if (freqPlanList.Count > 0)
            //{
            //    for (int i = 0; i < freqPlanList.Count; i++)
            //    {
            //        FreqPlanSegment fps = freqPlanList[i];
            //        NavBarGroup nbg = new NavBarGroup();
            //        AT_BC.Data.Range<double> freqValue = fps.FreqValue;
            //        //nbg.Header = fps.FreqPlanName + "    " + freqValue.Little / 1000000 + "--" + freqValue.Great / 1000000 + "MHz";
            //        nbg.Header = fps.FreqPlanName;
            //        FreqNavBar fnb = new FreqNavBar();
            //        fnb.FreqStart = (freqValue.Little / 1000000).ToString();
            //        fnb.FreqStop = (freqValue.Great / 1000000).ToString();

            //        //fnb.text_FreqStart.Text = (freqValue.Little / 1000000).ToString();
            //        //fnb.text_FreqStop.Text = (freqValue.Great / 1000000).ToString();
            //        //fnb.text_BandWidth.Text = (Convert.ToDouble(fps.FreqBand.Split('/')[0]) / 1000).ToString();

            //        fnb.reAnalysisFreqRange.Visibility = System.Windows.Visibility.Collapsed;
            //        fnb.delete_freqNavBar.Visibility = System.Windows.Visibility.Collapsed;
            //        fnb.BandWidth = (Convert.ToDouble(fps.FreqBand.Split('/')[0]) / 1000).ToString();
            //        fnb.DataContext = fnb;
            //        nbg.Items.Add(fnb);
            //        nbg.Click += new EventHandler(NavBarGroupManager_Click);
            //        nbg.IsExpanded = false;
            //        navBar.Groups.Add(nbg);
            //    }
            //}
            //else
            //{
            //    //NavBarGroup nbg = new NavBarGroup();
            //    //nbg.Header = "80--107 MHz";
            //    //FreqNavBar fnb = new FreqNavBar();
            //    //fnb.text_FreqStart.Text = "80";
            //    //fnb.text_FreqStop.Text = "107";
            //    //fnb.text_BandWidth.Text = "100";
            //    //fnb.reAnalysisFreqRange.Visibility = System.Windows.Visibility.Collapsed;
            //    //fnb.delete_freqNavBar.Visibility = System.Windows.Visibility.Collapsed;
            //    //fnb.MeasureId = "Measure20160830162310";
            //    //nbg.Items.Add(fnb);
            //    //nbg.Click += new EventHandler(NavBarGroupManager_Click);
            //    //nbg.IsExpanded = false;
            //    //navBar.Groups.Add(nbg);

            //    //NavBarGroup nbg1 = new NavBarGroup();
            //    //nbg1.Header = "80--120 MHz";
            //    //FreqNavBar fnb1 = new FreqNavBar();
            //    //fnb1.text_FreqStart.Text = "80";
            //    //fnb1.text_FreqStop.Text = "107";
            //    //fnb1.text_BandWidth.Text = "100";
            //    //fnb1.reAnalysisFreqRange.Visibility = System.Windows.Visibility.Collapsed;
            //    //fnb1.delete_freqNavBar.Visibility = System.Windows.Visibility.Collapsed;
            //    //fnb1.MeasureId = "Measure20160830162310";
            //    //nbg1.Items.Add(fnb1);
            //    //nbg1.Click += new EventHandler(NavBarGroupManager_Click);
            //    //nbg1.IsExpanded = false;
            //    //navBar.Groups.Add(nbg1);
            //}
            PlaceFreqPlan[] freqPlanList = GetActivityFreqPlanInfoSource(activityPlaceInfo.Guid);
            if (freqPlanList.Length > 0)
            {
                for (int i = 0; i < freqPlanList.Length; i++)
                {
                    PlaceFreqPlan fps = freqPlanList[i];
                    NavBarGroup nbg = new NavBarGroup();
                    //AT_BC.Data.Range<double> freqValue = fps.FreqValue;
                    //nbg.Header = fps.FreqPlanName + "    " + freqValue.Little / 1000000 + "--" + freqValue.Great / 1000000 + "MHz";
                    nbg.Header = fps.Name;
                    FreqNavBar fnb = new FreqNavBar();
                    fnb.FreqStart = fps.MHzFreqFrom.ToString();
                    fnb.FreqStop = fps.MHzFreqTo.ToString();

                    //fnb.text_FreqStart.Text = (freqValue.Little / 1000000).ToString();
                    //fnb.text_FreqStop.Text = (freqValue.Great / 1000000).ToString();
                    //fnb.text_BandWidth.Text = (Convert.ToDouble(fps.FreqBand.Split('/')[0]) / 1000).ToString();

                    fnb.reAnalysisFreqRange.Visibility = System.Windows.Visibility.Collapsed;
                    fnb.delete_freqNavBar.Visibility = System.Windows.Visibility.Collapsed;
                    fnb.BandWidth = fps.kHzBand.ToString();
                    fnb.DataContext = fnb;
                    nbg.Items.Add(fnb);
                    nbg.Click += new EventHandler(NavBarGroupManager_Click);
                    nbg.IsExpanded = false;
                    navBar.Groups.Add(nbg);
                }
            }
            else
            {
                //NavBarGroup nbg = new NavBarGroup();
                //nbg.Header = "80--107 MHz";
                //FreqNavBar fnb = new FreqNavBar();
                //fnb.text_FreqStart.Text = "80";
                //fnb.text_FreqStop.Text = "107";
                //fnb.text_BandWidth.Text = "100";
                //fnb.reAnalysisFreqRange.Visibility = System.Windows.Visibility.Collapsed;
                //fnb.delete_freqNavBar.Visibility = System.Windows.Visibility.Collapsed;
                //fnb.MeasureId = "Measure20160830162310";
                //nbg.Items.Add(fnb);
                //nbg.Click += new EventHandler(NavBarGroupManager_Click);
                //nbg.IsExpanded = false;
                //navBar.Groups.Add(nbg);

                //NavBarGroup nbg1 = new NavBarGroup();
                //nbg1.Header = "80--120 MHz";
                //FreqNavBar fnb1 = new FreqNavBar();
                //fnb1.text_FreqStart.Text = "80";
                //fnb1.text_FreqStop.Text = "107";
                //fnb1.text_BandWidth.Text = "100";
                //fnb1.reAnalysisFreqRange.Visibility = System.Windows.Visibility.Collapsed;
                //fnb1.delete_freqNavBar.Visibility = System.Windows.Visibility.Collapsed;
                //fnb1.MeasureId = "Measure20160830162310";
                //nbg1.Items.Add(fnb1);
                //nbg1.Click += new EventHandler(NavBarGroupManager_Click);
                //nbg1.IsExpanded = false;
                //navBar.Groups.Add(nbg1);
            }
        }

        /// <summary>
        /// 导航控件按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FreqNavBarButtonClick(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            Grid grid = (Grid)btn.Parent;
            FreqNavBar fnr = (FreqNavBar)grid.Parent;
            fnr.PlaceGuid = LoginService.CurrentActivityPlace.Guid;
            busyIndicator.IsBusy = true;
            cds.MeasureID = ((FreqCollectionIndex)lab_collectIndex.Tag).MeasureID;
            AnalysisFreqData(fnr, true);
        }

        /// <summary>
        /// 全部重新分析
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_reAnalysis_Click(object sender, RoutedEventArgs e)
        {
            busyIndicator.IsBusy = true;
            cds.MeasureID = ((FreqCollectionIndex)lab_collectIndex.Tag).MeasureID;
            if (navBar.Groups.Count > 0)
            {
                AnalysisFreqData(navBar.Groups);                              
            }
            else
            {
                MessageBox.Show("请先导入频率规划或手动添加！");
                busyIndicator.IsBusy = false;
            }

            //busyIndicator.IsBusy = true;
            //cds.MeasureID = ((FreqCollectionIndex)lab_collectIndex.Tag).MeasureID;
            //if (navBar.Groups.Count > 0)
            //{
            //    NavBarGroup nbg = navBar.Groups[0];
            //    FreqNavBar fnr = (FreqNavBar)nbg.Items[0];
            //    AnalysisFreqData(fnr, true);
            //}
            //else
            //{
            //    MessageBox.Show("请先导入频率规划或手动添加！");
            //    busyIndicator.IsBusy = false;
            //}
        }
        private void AnalysisFreqData(NavBarGroupCollection NavBars)
        {
            StatisticFreq sf = new StatisticFreq();
            sf.fileNameList = cds.getFileNames();
            sf.saveAanalysisDataDelegate = cds.saveAnalysisTable;
            sf.refreshAnalysisResultDelete = initDataSource;
            sf.refreshChartResultDelete = setChartDataSouce;
            sf.refreshChartFreqStatDelete = setFreqStatDataSouce;
            sf.reminderBoxDelegate = reminderBoxShow;
            sf.ShowUI = false;
            sf.MeasureId = cds.MeasureID;
            sf.freqCount = cds.getFreqCountByMeasureId(cds.MeasureID);
            //sf.freqNavBar = freqNavBar;
            string fileAddr = cds.IsHaveAnalysisData(cds.MeasureID);
            BackgroundWorker worker = new BackgroundWorker();

            List<FreqNavBar> freqnavbarList = new List<FreqNavBar>();
            for (int i = 0; i < NavBars.Count; i++)
            {
                NavBarGroup nbg = NavBars[i];
                FreqNavBar fnr = (FreqNavBar)nbg.Items[0];
                freqnavbarList.Add(fnr);
            }
            worker.DoWork += (o, ea) =>
            {
                if ("".Equals(fileAddr))
                {
                    sf.initStatisticData();
                }
                else
                {
                    //try
                    //{
                    //    sf.bandWidth = Double.Parse(freqNavBar.BandWidth);
                    //}
                    //catch (Exception)
                    //{
                    //    sf.bandWidth = 100;
                    //}
                    sf.readAnalysisFileForAll(fileAddr, freqnavbarList);
                }
            };

            worker.RunWorkerCompleted += (o, ea) =>
            {
                //chart.IsEnabled = false;
                busyIndicator.IsBusy = false;
            };
            worker.RunWorkerAsync();
        }
        /// <summary>
        /// 分析采集数据
        /// </summary>
        /// <param name="freqNavBar"></param>
        private void AnalysisFreqData(FreqNavBar freqNavBar, bool isRefresh)
        {
            StatisticFreq sf = new StatisticFreq();
            sf.fileNameList = cds.getFileNames();
            sf.saveAanalysisDataDelegate = cds.saveAnalysisTable;
            sf.refreshAnalysisResultDelete = initDataSource;
            sf.refreshChartResultDelete = setChartDataSouce;
            sf.refreshChartFreqStatDelete = setFreqStatDataSouce;
            sf.reminderBoxDelegate = reminderBoxShow;
            sf.isRefresh = isRefresh;
            sf.MeasureId = cds.MeasureID;
            sf.freqCount = cds.getFreqCountByMeasureId(cds.MeasureID);
            sf.freqNavBar = freqNavBar;
            string fileAddr = cds.IsHaveAnalysisData(cds.MeasureID);
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                if ("".Equals(fileAddr))
                {
                    sf.initStatisticData();
                }
                else
                {
                    //sf.bandWidth = 100;
                    try {
                       sf.bandWidth = Double.Parse(freqNavBar.BandWidth);
                    }
                    catch(Exception ){
                        sf.bandWidth = 100;
                    }
                    sf.readAnalysisFile(fileAddr);
                }
            };

            worker.RunWorkerCompleted += (o, ea) =>
            {
                //chart.IsEnabled = false;
                busyIndicator.IsBusy = false;
            };
            worker.RunWorkerAsync();
            //chart.IsEnabled = true;
        }

        /// <summary>
        /// GRIDCONTROL右键菜单事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;
            gridControl_freqInfo.View.PostEditor();
            //foreach (AnalysisResult flif in freqList) 
            //{
            //    if (flif.IsCheck == true) 
            //    {
            //        flif.FreqType = (SignalTypeEnum)Enum.Parse(typeof(SignalTypeEnum), mi.Header.ToString());
            //    }
            //}

            foreach (AnalysisResult item in gridControl_freqInfo.SelectedItems)
            {
                item.FreqType = (SignalTypeEnum)Enum.Parse(typeof(SignalTypeEnum), mi.Header.ToString());
            }

            gridControl_freqInfo.RefreshData();
            RefershStyle();
        }

        //绘制占用度和谱图
        private void setFreqStatDataSouce(Dictionary<double, FreqStatModel> dicFreqStatModel, ChartSeriesPoints chartSeriesPoints)
        {
            ObservableDataSource<Point> maxDataSource = new ObservableDataSource<Point>();
            ObservableDataSource<Point> midDataSource = new ObservableDataSource<Point>();
            if (dicFreqStatModel != null)
            {
                double tempFreq;
                foreach (KeyValuePair<double, FreqStatModel> kvp in dicFreqStatModel)
                {
                    Point dpMax = new Point();
                    Point dpMid = new Point();
                    tempFreq = kvp.Key;
                    int overSignalLimit = kvp.Value.DicAmplitudeCount.Where(x => x.Key > chartSeriesPoints.SignalLimit).Select(x => x.Value).Sum();
                    int sum = kvp.Value.DicAmplitudeCount.Values.Sum();
                    double occpy = (double)overSignalLimit / (double)sum;
                    if (occpy > 0.9)
                    {
                    }
                    double maxValue = kvp.Value.DicAmplitudeCount.Keys.Max();
                    dpMax.X = tempFreq / 1000;
                    dpMax.Y = Convert.ToInt16(maxValue);

                    maxDataSource.AppendAsync(plotter.Dispatcher, dpMax);

                    double AverageValue = kvp.Value.DicAmplitudeCount.Select(x => x.Key).Average();
                    dpMid.X = tempFreq / 1000;
                    dpMid.Y = Convert.ToInt16(AverageValue);
                    midDataSource.AppendAsync(plotter.Dispatcher, dpMid);
                }
            }
            plotter.Dispatcher.Invoke(
                new Action(
                    delegate
                    {
                        List<LineGraph> temp = new List<LineGraph>();
                        foreach (var obj in plotter.Children)
                        {
                            if (typeof(LineGraph).IsAssignableFrom(obj.GetType()))
                            {
                                temp.Add((LineGraph)obj);
                            }
                        }
                        if (temp.Count > 0)
                        {
                            foreach (LineGraph rh in temp)
                            {
                                plotter.Children.Remove(rh);
                            }
                        }
                        plotter.AddLineGraph(maxDataSource, Colors.Red, 1, "最大值");
                        plotter.AddLineGraph(midDataSource, Colors.LightBlue, 1, "中值");
                        plotter.Visible = new Microsoft.Research.DynamicDataDisplay.DataRect(new Rect(chartSeriesPoints.MinFreq, 0, chartSeriesPoints.MaxFreq - chartSeriesPoints.MinFreq, 100));
                        //linegraphMax.DataSource = maxDataSource;
                    }
                )
            );
            // linegraphMid.Dispatcher.Invoke(
            //    new Action(
            //        delegate
            //        {
            //            linegraphMid.DataSource = midDataSource;
            //            plotter.Visible = new Microsoft.Research.DynamicDataDisplay.DataRect(new Rect(chartSeriesPoints.MinFreq, 0, chartSeriesPoints.MaxFreq - chartSeriesPoints.MinFreq, 100));
            //        }
            //    )
            //);
            BarPlotter.Dispatcher.Invoke(
               new Action(
                   delegate
                   {
                       List<RectangleHighlight> temp = new List<RectangleHighlight>();
                       foreach (var obj in BarPlotter.Children)
                       {
                           if (typeof(RectangleHighlight).IsAssignableFrom(obj.GetType()))
                           {
                               temp.Add((RectangleHighlight)obj);
                           }
                       }
                       if (temp.Count > 0)
                       {
                           foreach (RectangleHighlight rh in temp)
                           {
                               BarPlotter.Children.Remove(rh);
                           }
                       }
                       foreach (PointPair pp in chartSeriesPoints.OccupyPointPairList)
                       {
                           if (pp.Y >= chartSeriesPoints.OccuDegreeLimit)
                           {
                               RectangleHighlight rh = new RectangleHighlight();
                               rh.Bounds = new Microsoft.Research.DynamicDataDisplay.DataRect(new Rect(pp.X, 0, 0, pp.Y));
                               rh.Stroke = Brushes.Yellow;
                               rh.StrokeThickness = 10;
                               rh.ToolTip = pp.X.ToString("0.0000") + "MHz";
                               BarPlotter.Children.Add(rh);
                           }
                       }
                       BarPlotter.Visible = new Microsoft.Research.DynamicDataDisplay.DataRect(new Rect(chartSeriesPoints.MinFreq, 0, chartSeriesPoints.MaxFreq - chartSeriesPoints.MinFreq, 100));
                       //ObservableDataSource<Point> standDataSource = new ObservableDataSource<Point>();
                       //standDataSource.AppendAsync(BarPlotter.Dispatcher, new Point(chartSeriesPoints.MinFreq, 0));
                       //standDataSource.AppendAsync(BarPlotter.Dispatcher, new Point(chartSeriesPoints.MinFreq, 100));
                       //standDataSource.AppendAsync(BarPlotter.Dispatcher, new Point(chartSeriesPoints.MaxFreq, 100));
                       //standDataSource.AppendAsync(BarPlotter.Dispatcher, new Point(chartSeriesPoints.MaxFreq, 0));
                       //linegraphStand.DataSource = standDataSource;
                   }
               )
           );

            //this.freqDataSeries.Dispatcher.Invoke(
            //    new Action(
            //       delegate
            //       {
            //           freqDataSeries.DataPoints.Clear();
            //           if (dicFreqStatModel != null)
            //           {
            //               double tempFreq;
            //               foreach (KeyValuePair<double, FreqStatModel> kvp in dicFreqStatModel)
            //               {
            //                   Visifire.Charts.DataPoint dpMax = new Visifire.Charts.DataPoint();

            //                   tempFreq = kvp.Key;
            //                   int overSignalLimit = kvp.Value.DicAmplitudeCount.Where(x => x.Key > 45).Select(x => x.Value).Sum();
            //                   int sum = kvp.Value.DicAmplitudeCount.Values.Sum();
            //                   double occpy = (double)overSignalLimit / (double)sum;
            //                   if (occpy > 0.9)
            //                   {
            //                   }
            //                   double maxValue = kvp.Value.DicAmplitudeCount.Keys.Max();
            //                   //AmplitudeMaxValue.Add(Convert.ToInt16(maxValue));
            //                   dpMax.XValue = tempFreq / 1000;
            //                   dpMax.YValue = Convert.ToInt16(maxValue);

            //                   //spMax.X = tempFreq / 1000;
            //                   //spMax.Y = Convert.ToInt16(maxValue);
            //                   //maxValues.Add(spMax);
            //                    freqDataSeries.DataPoints.Add(dpMax);
            //                   double AverageValue = kvp.Value.DicAmplitudeCount.Select(x => x.Key).Average();
            //               }
            //           }
            //       }
            //    )
            //);

        }


        private void removeBar()
        {
            List<RectangleHighlight> temp = new List<RectangleHighlight>();
            foreach (var obj in BarPlotter.Children)
            {
                if (typeof(RectangleHighlight).IsAssignableFrom(obj.GetType()))
                {
                    temp.Add((RectangleHighlight)obj);
                }
            }
            if (temp.Count > 0)
            {
                foreach (RectangleHighlight rh in temp)
                {
                    BarPlotter.Children.Remove(rh);
                }
            }
        }
        private void reminderBoxShow(string message)
        {
            MessageBox.Show(message);
        }

        private void setChartDataSouce(ChartSeriesPoints chartSeriesPoints)
        {
            //zedGraphControl.GraphPane.CurveList.Clear();
            //zedGraphControlOccpy.GraphPane.CurveList.Clear();
            ////****************winform control
            //this.zedGraphControl.GraphPane.XAxis.Scale.Max = chartSeriesPoints.MidPointPairList[chartSeriesPoints.MidPointPairList.Count - 1].X;
            //this.zedGraphControl.GraphPane.XAxis.Scale.MajorStep = 0.0275;
            //this.zedGraphControl.GraphPane.XAxis.Scale.Min = chartSeriesPoints.MidPointPairList[0].X;

            //this.zedGraphControlOccpy.GraphPane.XAxis.Scale.Max = chartSeriesPoints.MidPointPairList[chartSeriesPoints.MidPointPairList.Count - 1].X;
            //this.zedGraphControlOccpy.GraphPane.XAxis.Scale.MajorStep = 0.0275;
            //this.zedGraphControlOccpy.GraphPane.XAxis.Scale.Min = chartSeriesPoints.MidPointPairList[0].X;
            //this.zedGraphControlOccpy.GraphPane.YAxis.Scale.Max = 100;
            //this.zedGraphControlOccpy.GraphPane.YAxis.Scale.MajorStep = 20;
            //this.zedGraphControlOccpy.GraphPane.YAxis.Scale.Min = 0;
            //GraphPane myPane = this.zedGraphControl.GraphPane;

            //GraphPane myPane1 = this.zedGraphControlOccpy.GraphPane;
            //// 创建红色的菱形曲线
            //// 标记, 图中的 "Porsche"             
            //myCurve = myPane.AddCurve("最大值", chartSeriesPoints.MaxPointPairList, System.Drawing.Color.Red, SymbolType.None);

            //// 创建蓝色的圆形曲线
            //// 标记, 图中的 "Piper"　　　 
            //myCurve2 = myPane.AddCurve("平均值", chartSeriesPoints.MidPointPairList, System.Drawing.Color.Blue, SymbolType.None);

            //BarItem myBar = myPane1.AddBar("占用度", chartSeriesPoints.OccupyPointPairList, System.Drawing.Color.Red);
            //myPane1.BarSettings.ClusterScaleWidth = 0.5;
            //// 在数据变化时绘制图形
            //myPane.AxisChange(zedGraphControl.CreateGraphics());
            //myPane1.AxisChange(zedGraphControlOccpy.CreateGraphics());
            //***************************************

            //**********************dev control
            //maxSeriesPointCollection = chartSeriesPoints.MaxValueCollection;
            //midSeriesPointCollection = chartSeriesPoints.MidValueCollection;
            //this.chart.Dispatcher.Invoke(
            //new Action(
            //       delegate
            //       {
            //           maxSeries.Points.Clear();
            //           foreach (SeriesPoint sp in maxSeriesPointCollection) 
            //           {
            //               maxSeries.Points.Add(sp); 
            //           }
            //           //maxSeries.Points.AddRange(maxSeriesPointCollection);
            //           //midSeries.Points.Clear();
            //           //midSeries.Points.AddRange(midSeriesPointCollection);
            //       }
            //    )
            //);

            //this.midSeries.Dispatcher.Invoke(
            //new Action(
            //       delegate
            //       {
            //           try
            //           {

            //           }
            //           catch (Exception e)
            //           {

            //           }
            //       }
            //    )
            //); 
            //************************************
        }

        /// <summary>
        /// 导入频率规划
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ImportFreq_Click(object sender, RoutedEventArgs e)
        {
            //List<FreqPlanSegment> freqPlanList = GetFreqPartPlan();
            //List<FreqPlanActivity> freqPlanList = GetActivityFreqPlanInfoSource(LoginService.CurrentActivityPlace.Guid);

            List<FreqPlanSegment> freqPlanList = SQLiteDataService.QueryFreqPlanByPlaceID(LoginService.CurrentActivityPlace.Guid); //add by michael 17.07.20
            if (freqPlanList != null && freqPlanList.Count > 0)
            {
                navBar.Groups.Clear();//add by michael
                for (int i = 0; i < freqPlanList.Count; i++)
                {
                    FreqPlanSegment fps = freqPlanList[i];
                    NavBarGroup nbg = new NavBarGroup();
                    AT_BC.Data.Range<double> freqValue = fps.FreqValue;
                    //nbg.Header = fps.FreqPlanName + "    " + freqValue.Little / 1000000 + "--" + freqValue.Great / 1000000 + "MHz";
                    nbg.Header = fps.FreqPlanName + "    " + freqValue.Little + "--" + freqValue.Great + "MHz";
                    FreqNavBar fnb = new FreqNavBar();
                    //fnb.FreqStart = (freqValue.Little / 1000000).ToString();
                    //fnb.FreqStop = (freqValue.Great / 1000000).ToString();
                    fnb.FreqStart = (freqValue.Little).ToString();
                    fnb.FreqStop = (freqValue.Great).ToString();
                    fnb.reAnalysisFreqRange.Click += new RoutedEventHandler(FreqNavBarButtonClick);
                    //fnb.BandWidth = (Convert.ToDouble(fps.FreqBand.Split('/')[0]) / 1000).ToString();
                    fnb.BandWidth = (Convert.ToDouble(fps.FreqBand)).ToString();
                    fnb.FreqGuid = fps.FreqId;
                    nbg.Items.Add(fnb);
                    nbg.Click += new EventHandler(NavBarGroup_Click);
                    nbg.IsExpanded = false;
                    navBar.Groups.Add(nbg);
                }
            }
            else
            {
                MessageBox.Show("该活动区域没有频率规划！");
            }
        }

        /// <summary>
        /// 保存分析结果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_saveResult_Click(object sender, RoutedEventArgs e)
        {
           
            var MeasureStartTime = ((FreqCollectionIndex)lab_collectIndex.Tag).StartTime;
            var MeasureEndTime = ((FreqCollectionIndex)lab_collectIndex.Tag).EndTime;

            BackgroundWorker worker = new BackgroundWorker();
            if (navBar.Groups.Count == 0)
            {
                MessageBox.Show("没有需要保存的数据！");
                return;
            }
            List<ObservableCollection<AnalysisResult>> freqnavbarList = new List<ObservableCollection<AnalysisResult>>();
            for (int i = 0; i < navBar.Groups.Count; i++)
            {
                NavBarGroup nbg = navBar.Groups[i];
                FreqNavBar fnr = (FreqNavBar)nbg.Items[0];
                if(fnr.freqList!=null&&fnr.freqList.Count>0)
                    freqnavbarList.Add(fnr.freqList);
            }
            if (freqnavbarList.Count == 0)
            {
                MessageBox.Show("没有需要保存的数据！");
                return;
            }
            busyIndicator.IsBusy = true;
            worker.DoWork += (o, ea) =>
            {
                try
                {
                    SQLiteDataService.Transaction = SQLiteConnect.GetSQLiteTransaction(LoginService.CurrentActivity.Name);
                    for (int i = 0; i < freqnavbarList.Count; i++)
                    {
                        ObservableCollection<AnalysisResult> freqs = freqnavbarList[i];
                        if (freqs != null && freqs.Count > 0)
                        {
                            for (int j = 0; j < freqs.Count; j++)
                            {
                                AnalysisResult temp = freqs[j];
                                temp.MeasureStartTime = MeasureStartTime;
                                temp.MeasureEndTime = MeasureEndTime;
                                temp.PlaceGuid = LoginService.CurrentActivityPlace.Guid;
                                SQLiteDataService.SaveAnalysisResult(temp);
                            }                                
                        }
                    }
                    SQLiteDataService.Transaction.Commit();
                    MessageBox.Show("数据保存成功");
                }
                catch(Exception ex)
                {
                    SQLiteDataService.Transaction.Rollback();
                    MessageBox.Show(ex.Message);
                }
                
               
                //if (freqList != null && freqList.Count > 0)
                //{
                //    try
                //    {
                //        SQLiteDataService.Transaction = SQLiteConnect.GetSQLiteTransaction(LoginService.CurrentActivity.Name);

                //        for (int i = 0; i < freqList.Count; i++)
                //        {
                //            AnalysisResult temp = freqList[i];
                //            temp.MeasureStartTime = MeasureStartTime;
                //            temp.MeasureEndTime = MeasureEndTime;
                //            temp.PlaceGuid = LoginService.CurrentActivityPlace.Guid;
                //            SQLiteDataService.SaveAnalysisResult(temp);
                //        }
                //        SQLiteDataService.Transaction.Commit();
                //        MessageBox.Show("数据保存成功");
                //    }
                //    catch (Exception ex)
                //    {
                //        SQLiteDataService.Transaction.Rollback();
                //        MessageBox.Show(ex.Message);
                //    }
                //}
                //else
                //{
                //    MessageBox.Show("没有需要保存的数据！");
                //}
            };

            worker.RunWorkerCompleted += (o, ea) =>
            {
                busyIndicator.IsBusy = false;
            };
            worker.RunWorkerAsync();
        }

        private void setCollectionLabel(FreqCollectionIndex freqCollectionIndex)
        {
            lab_collectIndex.Content = freqCollectionIndex.CurrentActivityPlaceName + "  " + freqCollectionIndex.DisplayMem;
            lab_collectIndex.Tag = freqCollectionIndex;
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CollectionIndexManage cim = new CollectionIndexManage();
            cim.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            data = cds.getMeasureIds();//add by michael
            cim.initGridCtrlData(data);
            cim.setCollectionLabel = setCollectionLabel;
            cim.initComboxDataDelegate = initComboxData;
            cim.collectionDataSave = cds;
            cim.Height = 450;
            cim.Width = 650;
            cim.ShowDialog(this);
        }

        //private List<FreqPlanSegment> GetFreqPartPlan()
        //{
        //    try
        //    {
        //        return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan, List<FreqPlanSegment>>(channel =>
        //        {
        //            return channel.GetFreqPlanInfo();
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.GetExceptionMessage());
        //        return null;
        //    }
        //}
        //private List<FreqPlanActivity> GetActivityFreqPlanInfoSource(string pPlaceId)
        private PlaceFreqPlan[] GetActivityFreqPlanInfoSource(string pPlaceId)
        {
            try
            {
                //List<FreqPlanActivity> freqPlanActivitys =
                //return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan, List<FreqPlanActivity>>(channel =>                                {
                //    return channel.GetFreqPlanActivitys(pPlaceId);
                //});
                PlaceFreqPlan[] placeFreqPlans = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqStation.I_CO_IA_FreqStation, PlaceFreqPlan[]>(channel =>
                {
                     return channel.GetPlaceFreqPlans(pPlaceId);
                });
                return placeFreqPlans;
                //if (freqPlanActivitys != null)
                //    xfreqPartPlanList.FreqPlanInfoItemsSource = new System.Collections.ObjectModel.ObservableCollection<FreqPlanActivity>(freqPlanActivitys);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetExceptionMessage());
                return null;
            }
        }

        private void cbxUsing_Checked(object sender, RoutedEventArgs e)
        {
            if (cbxFree.IsLoaded && cbxUsing.IsLoaded && cbxUnUsing.IsLoaded)
            {
                List<string> showTypes = new List<string>();
                if (cbxFree.IsChecked == true)
                {
                    showTypes.Add(cbxFree.Tag.ToString());
                }
                if (cbxUsing.IsChecked == true)
                {
                    showTypes.Add(cbxUsing.Tag.ToString());
                }
                if (cbxUnUsing.IsChecked == true)
                {
                    showTypes.Add(cbxUnUsing.Tag.ToString());
                }
                UI.Collection.Converter.RowShowConverter.ShowType = string.Join(",", showTypes);
                RefershStyle();
            }
        }

        private void ComboBoxEditItem_Selected(object sender, RoutedEventArgs e)
        {
            RefershStyle();
        }

        /// <summary>
        /// 刷新样式
        /// </summary>
        private void RefershStyle()
        {
            tbView.RowStyle = null;
            tbView.RowStyle = Resources["RowStyle"] as Style;
        }
        /// <summary>
        /// 点击台站名称超链接，打开台站基本信息界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HyperlinkRoundStation_Click(object sender, RoutedEventArgs e)
        {
            var dataContext = ((sender as Hyperlink).Parent as TextBlock).Tag as AnalysisResult;
           
            if (dataContext!=null && StationHyperlinkClick != null)
            {
                if (string.IsNullOrEmpty(dataContext.StationGuid))
                {
                    MessageBox.Show("没有台站ID，无法查看台站信息。");
                    //StationHyperlinkClick("1004");
                    return;
                }
                else
                {
                    StationHyperlinkClick(dataContext.StationGuid);
                }
            }
        }
    }
}
