using AT_BC.Data;
using CO_IA.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using CO_IA.Client;

 


namespace CO_IA.UI.Statistic
{

    /// <summary>
    /// StatisticModuleControl.xaml 的交互逻辑
    /// </summary>
    public partial class StatisticModuleControl : UserControl
    {
        public event Action OnChangeItem;
        public StatisticTypes StatisticType { get; set; }

        public string TitleContent
        {
            set { this.statisticChartControl.TitleContent = value; }
        }

        public IList TypeSource
        {
            get { return (IList)GetValue(TypeSourceProperty); }
            set { SetValue(TypeSourceProperty, value); }
        }

        public static readonly DependencyProperty TypeSourceProperty =
            DependencyProperty.Register("TypeSource", typeof(IList), typeof(StatisticModuleControl),
            new PropertyMetadata(new PropertyChangedCallback(ItemsSourcePropertyChangedCallBack)));
        private static void ItemsSourcePropertyChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as StatisticModuleControl).comboBoxEditNames.ItemsSource = e.NewValue;
            (d as StatisticModuleControl).comboBoxEditSeries.ItemsSource = e.NewValue;
        }

        public string SelectedPlace { set; get; }

        private bool showcontainer = false;
        public bool ShowContainer
        {
            get
            {
                return showcontainer;
            }
            set
            {
                showcontainer = value;
                if (showcontainer)
                {
                    gridcontainer.Visibility = Visibility.Visible;
                }
                else
                {
                    gridcontainer.Visibility = Visibility.Collapsed;
                }
            }
        }

        private bool showstatisticlist = true;
        public bool ShowStatisticList
        {
            get
            {
                return showstatisticlist;
            }
            set
            {
                showstatisticlist = value;
                if (showstatisticlist)
                {
                    statisticlstColumn.Width = new GridLength(1, GridUnitType.Star);
                    statisticListControl.Visibility = Visibility.Visible;
                }
                else
                {
                    statisticlstColumn.Width = new GridLength(0);
                    statisticListControl.Visibility = Visibility.Collapsed;
                }
            }
        }

        public bool ShowLegend
        {
            get
            {
                return statisticChartControl.LegendVisibility; 
            }
            set
            {
                statisticChartControl.LegendVisibility = value;
            }
        }

        /// <summary>
        /// 数据源
        /// </summary>
        public IList StatisticSource
        {
            get { return (IList)GetValue(StationPlanStatisticControlProperty); }
            set { SetValue(StationPlanStatisticControlProperty, value); }
        }

        public static readonly DependencyProperty StationPlanStatisticControlProperty =
            DependencyProperty.Register("StatisticSource", typeof(IList), typeof(StatisticModuleControl),
            new PropertyMetadata(new PropertyChangedCallback(StatisticSourcePropertyChangedCallBack)));

        public StatisticModuleControl()
        {
            InitializeComponent();


            statisticListControl.OnCellClick += statisticListControl_OnCellClick;
        }

        private void comboBoxEditGroup_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            if (OnChangeItem != null)
            {
                OnChangeItem();
            }
        }
        private static void StatisticSourcePropertyChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((d as StatisticModuleControl).OnChangeItem != null)
                (d as StatisticModuleControl).OnChangeItem();
        }

        /// <summary>
        /// 转换成统计图数据源
        /// </summary>
        /// <returns></returns>
        public void OnStatistic<T, S>(Func<T, Func<S, string>> pGetStatisticGroups)
            where T : struct
            where S : StatisticData
        {
            if (StatisticSource == null)
                return;

            T seriestype;
            if (Enum.TryParse(this.comboBoxEditSeries.SelectedItem.ToString(), out seriestype))
            {

            }
            T nametype;
            if (Enum.TryParse(this.comboBoxEditNames.SelectedItem.ToString(), out nametype))
            {

            }
            Func<S, string> seriesGenerator = pGetStatisticGroups(seriestype);
            Func<S, string> nameGenerator = pGetStatisticGroups(nametype);

            List<SeriesNameValue<double>> chartdatasource = GetChartItemsSource(seriesGenerator, nameGenerator);
            if (nametype.GetType() == FreqAssignStatisticType.活动地点.GetType())
            {
                chartdatasource = chartdatasource.OrderBy(r => r.Name).ToList();
            }
            List<NameValuePair<string>> statisticgroups = new List<NameValuePair<string>>();
            List<StatisticDataSource> ItemsSource = GetDataGridItemsSource(seriestype, nametype, seriesGenerator, nameGenerator,
                out statisticgroups);

            statisticListControl.statisticGroup = statisticgroups;
            statisticListControl.StatisticItemsSource = ItemsSource;
            statisticChartControl.StatisticItemsSource = chartdatasource;
        }

        /// <summary>
        /// 获取统计图数据源
        /// </summary>
        /// <returns></returns>
        private List<SeriesNameValue<double>> GetChartItemsSource<S>(Func<S, string> seriesGenerator, Func<S, string> nameGenerator) where S : StatisticData
        {
            string groupname = string.Empty;
            List<SeriesNameValue<double>> result =
                ((from source in (List<S>)StatisticSource
                  group source by new
                  {
                      SeriesName = seriesGenerator(source),
                      Name = nameGenerator(source),
                  } into s
                  select new SeriesNameValue<double>
                  {
                      SeriesName = s.Key.SeriesName,
                      Name = GetName<S>(s.Key.Name),
                      Value = s.Sum(p => p.Count)
                  }).ToList()).OrderBy(r => r.SeriesName).ToList();
            if (result != null)
            {
                return result.Where(r => r.Value > 0).ToList();
            }
            return result;
        }

        private string GetName<S>(string guid) where S : StatisticData
        {
            List<S> source = (List<S>)StatisticSource;
            string name = source.FirstOrDefault(r => r.AddressGuid == guid).Address;
            return name;
            //StatisticSource.ToList();
        }

        private List<StatisticDataSource> GetDataGridItemsSource<T, S>(T seriestype, T nametype, Func<S, string> seriesGenerator,
            Func<S, string> nameGenerator, out List<NameValuePair<string>> sgroups)
            where T : struct
            where S : StatisticData
        {
            List<StatisticDataSource> datasources = new List<StatisticDataSource>();
            List<NameValuePair<string>> statisticgroups = new List<NameValuePair<string>>();

            #region
            //if (nametype.GetType() == FreqAssignStatisticType.活动地点.GetType())
            //{                
            //    IEnumerable<IGrouping<string, S>> conditions = ((List<S>)StatisticSource).GroupBy(r => seriesGenerator(r));
            //    foreach (IGrouping<string, S> condition in conditions)
            //    {
            //        StatisticDataSource datasource = new StatisticDataSource();
            //        datasource.Group = condition.Key;

            //        IEnumerable<IGrouping<string, S>> groups = condition.GroupBy(r => nameGenerator(r));
            //        groups.OrderBy(r => r.Key);
            //        foreach (IGrouping<string, S> group in groups)
            //        {
            //            int count = group.Sum(n => n.Count);
            //            datasource[group.Key] = count;
            //            if (!statisticgroups.Contains(group.Key))
            //            {
            //                statisticgroups.Add(group.Key);
            //            }
            //        }
            //        statisticgroups = statisticgroups.OrderBy(r => r).ToList();
            //        datasources.Add(datasource);
            //        datasources = datasources.OrderBy(r => r.Group).ToList();
            //    }
            //}
            //else
            //{
            //ActivityPlace[] actPlace = RiasPortal.GetCurrentActivityPlaces();
            #endregion

            ActivityPlaceInfo[] actPlace = Utility.GetPlacesByActivityId(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);

            for (int i = 0; i < actPlace.Length; i++)
            {
                if (this.SelectedPlace == "all")
                    statisticgroups.Add(new NameValuePair<string>() { Name = actPlace[i].Guid, Value = actPlace[i].Name });
                else
                {
                    if (this.SelectedPlace == actPlace[i].Guid)
                        statisticgroups.Add(new NameValuePair<string>() { Name = actPlace[i].Guid, Value = actPlace[i].Name });
                }
            }
            IEnumerable<IGrouping<string, S>> conditions = ((List<S>)StatisticSource).GroupBy(r => seriesGenerator(r));
            foreach (IGrouping<string, S> condition in conditions)
            {
                StatisticDataSource datasource = new StatisticDataSource();
                datasource.Group = condition.Key;
                datasource.GroupGuid = condition.Key;

                IEnumerable<IGrouping<string, S>> groups = condition.GroupBy(r => nameGenerator(r));
                foreach (IGrouping<string, S> group in groups)
                {
                    double count = group.Sum(n => n.Count);
                    datasource[group.Key] = count;
                }
                datasources.Add(datasource);
            }
            //}
            sgroups = statisticgroups;
            return datasources;
        }

        /// <summary>
        /// 单元格单击事件
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="placeguid"></param>
        private void statisticListControl_OnCellClick(StatisticDataSource datasource, string placeguid)
        {
            string activityguid = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid;
            EquipmentLoadStrategy equloadstrategy;
            string condition = datasource.Group;
            switch (StatisticType)
            {
 

                #region  设备和台站
                case StatisticTypes.ORGAndEQUStatisticType:
                    equloadstrategy = new EquipmentLoadStrategy();
                    equloadstrategy.ActivityGuid = activityguid;
                    equloadstrategy.PlaceGuid = placeguid;
                    equloadstrategy.OrgName = condition;
                    QueryEQUStatisticDetail(equloadstrategy);
                    break;
                #endregion

                #region 周围台站
                case StatisticTypes.SurroundStatStatisticType:
                    QuerySurroundStatStatisticDetail(activityguid, placeguid, condition);
                    break;
                #endregion

                #region 频率指配
                case StatisticTypes.FreqAssignStatisticType:
                    equloadstrategy = new EquipmentLoadStrategy();
                    equloadstrategy.ActivityGuid = activityguid;
                    equloadstrategy.PlaceGuid = placeguid;
                    QueryFreqAssignStatisticDetail(equloadstrategy, condition);
                    break;
                #endregion

                #region 设备检测
                case StatisticTypes.EquInspectionSticType:
                    EquInspectionQueryCondition loadcondition = new EquInspectionQueryCondition();
                    loadcondition.ActivityGuid = activityguid;
                    loadcondition.PlaceGuid = placeguid;
                    InspectionStateEnum state;
                    if (Enum.TryParse(condition, out state))
                    {
                        switch (state)
                        {
                            case InspectionStateEnum.总数:
                                loadcondition.CheckState = new List<CheckStateEnum>() { };
                                break;

                            case InspectionStateEnum.检测通过:
                                loadcondition.CheckState = new List<CheckStateEnum>() { CheckStateEnum.Qualified };
                                break;

                            case InspectionStateEnum.检测未通过:
                                loadcondition.CheckState = new List<CheckStateEnum>() { CheckStateEnum.UnQualified };

                                break;
                            case InspectionStateEnum.未检测:
                                loadcondition.CheckState = new List<CheckStateEnum>() { CheckStateEnum.UnCheck };
                                break;
                        }

                        QueryEquInspectionStatisticDetail(loadcondition);
                    }
                    break;
                #endregion

                #region 人员预案
                case StatisticTypes.PersonPlanStatisticType:
                    string orgguid = datasource.GroupGuid;
                    if (orgguid == "total")
                    {
                        orgguid = string.Empty;
                    }

                    string type = placeguid;
                    switch (type)
                    {
                        //case "设备":
                        //    EquList equlst = new EquList();
                        //    equlst.OrgID = orgguid;
                        //    SetStatisticDetailControl(equlst);
                        //    break;
                        //case "人员":
                        //    PersonList personlist = new PersonList();
                        //    personlist.OrgID = orgguid;
                        //    SetStatisticDetailControl(personlist);
                        //    break;
                        //case "车辆":
                        //    VehicleList vehiclelst = new VehicleList();
                        //    vehiclelst.OrgID = orgguid;
                        //    SetStatisticDetailControl(vehiclelst);
                        //    break;
                    }
                    break;
                #endregion

            }
        }

        /// <summary>
        /// 设备统计详情
        /// </summary>
        /// <param name="equloadstrategy"></param>
        private void QueryEQUStatisticDetail(EquipmentLoadStrategy equloadstrategy)
        {
            ActivityEquipment[] activityequs = StatisticHelper.QueryActivityEquipments(equloadstrategy);
            CreateActivityEquipmentListControl(activityequs);
        }

        /// <summary>
        /// 周围台站统计详情
        /// </summary>
        /// <param name="activityguid"></param>
        /// <param name="placeguid"></param>
        /// <param name="apptype">表类型</param>
        private void QuerySurroundStatStatisticDetail(string activityguid, string placeguid, string apptype)
        {
            //List<ActivitySurroundStation> stations = StatisticHelper.QueryActivitySurroundStation(activityguid, placeguid);
            //List<ActivitySurroundStation> surrstation = stations.Where(r => r.STAT_APP_TYPE == apptype).ToList(); ;
            //SurroundStationListControl stationlstcontrol = new SurroundStationListControl();
            //stationlstcontrol.DataContext = surrstation;
            //SetStatisticDetailControl(stationlstcontrol);
        }

        /// <summary>
        /// 频率指配统计详情
        /// </summary>
        /// <param name="equloadstrategy"></param>
        /// <param name="freqtype">发射频率指配数量、备用频率指配数量</param>
        private void QueryFreqAssignStatisticDetail(EquipmentLoadStrategy equloadstrategy, string freqtype)
        {
            //ActivityEquipment[] equsource = StatisticHelper.QueryActivityEquipments(equloadstrategy);
            //if (equsource != null)
            //{
            //    ActivityEquipment[] activityequs = null;
            //    if (freqtype == "发射频率指配数量")
            //    {
            //        activityequs = equsource.Where(r => r.AssignSendFreq != null).ToArray();
            //    }
            //    else if (freqtype == "备用频率指配数量")
            //    {
            //        activityequs = equsource.Where(r => r.AssignSpareFreq != null).ToArray();
            //    }

            //    FreqAssignListControl freqassignlstcontrol = new FreqAssignListControl();
            //    freqassignlstcontrol.DataContext = activityequs;
            //    SetStatisticDetailControl(freqassignlstcontrol);
            //}
            //else
            //{
            //    MessageBox.Show("设备出现异常");
            //}
        }

        private void QueryEquInspectionStatisticDetail(EquInspectionQueryCondition loadcondition)
        {
            //List<EquipmentInspection> equs = StatisticHelper.QueryEquipmentInspections(loadcondition);
            //EquipmentInspectionListControl equInsplstcontrol = new EquipmentInspectionListControl();
            //equInsplstcontrol.DataContext = equs;
            //SetStatisticDetailControl(equInsplstcontrol);
        }

        private void CreateActivityEquipmentListControl(ActivityEquipment[] activityequs)
        {
            //ActivityEquipmentListControl equlstcontrol = new ActivityEquipmentListControl();
            ////equlstcontrol.DoubleClick += (equ) =>
            ////{
            ////    EquipmentManageDialog addequdialog = new EquipmentManageDialog();
            ////    addequdialog.AllowEdit = false;
            ////    addequdialog.DataContext = equ;
            ////    addequdialog.ShowDialog();
            ////};
            //equlstcontrol.DataContext = activityequs;
            //SetStatisticDetailControl(equlstcontrol);
        }


        private void SetStatisticDetailControl(FrameworkElement control)
        {
            this.ShowContainer = true;
            this.bordercontainer.Child = null;
            this.bordercontainer.Child = control;
        }

        private void buttonGoback_Click(object sender, RoutedEventArgs e)
        {
            this.ShowContainer = false;
        }

    }
}