using AT_BC.Data;
using CO_IA.Data;
using DevExpress.Xpf.LayoutControl;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace CO_IA.UI.Statistic
{
    /// <summary>
    /// PersonPlanStatisticControl.xaml 的交互逻辑
    /// </summary>
    public partial class PersonPlanStatisticControl : UserControl, IStatisticObject
    {
        private IList<PersonPlanStatisticData> personplanstatisticSource;
        public IList<PersonPlanStatisticData> PersonPlanStatisticSource
        {
            get { return personplanstatisticSource; }
            set
            {
                personplanstatisticSource = value;
                Statistic(personplanstatisticSource);
            }
        }

        public PersonPlanStatisticControl()
        {
            InitializeComponent();
            statisticListControl.OnCellClick += statisticListControl_OnCellClick;

            InitSource();
            ShowLegend = true;
        }

        #region IStatisticObject接口
        public string SelectedPlace
        {
            set;
            get;
        }


        public IList StatisticSource
        {
            get;
            set;
        }


        public bool ShowContainer
        {
            get;
            set;
        }

        public bool ShowStatisticList
        {
            get { return false; }
            set
            {
                if (!value)
                    statisticlstColumn.Width = new GridLength(0);
            }
        }

        public void InitSource()
        {
            this.statisticChartControl.TitleContent = "人员外出统计图";
        }

        private bool showlegend;

        public bool ShowLegend
        {
            get
            {
                return showlegend;
            }
            set
            {
                showlegend = value;
                if (showlegend)
                {
                    row1.Height = new GridLength(1, GridUnitType.Star);
                    statisticChartControl.Visibility = Visibility.Visible;
                }
                else
                {
                    row1.Height = new GridLength(0);
                    statisticChartControl.Visibility = Visibility.Collapsed;
                }

            }
        }

        #endregion

        public void Statistic(IList<PersonPlanStatisticData> source)
        {

            List<StatisticDataSource> datasources = new List<StatisticDataSource>();
            List<NameValuePair<string>> statisticgroups = new List<NameValuePair<string>>();

            IEnumerable<IGrouping<string, PersonPlanStatisticData>> conditions = source.GroupBy(r => r.AddressGuid);
            foreach (IGrouping<string, PersonPlanStatisticData> condition in conditions)
            {
                StatisticDataSource datasource = new StatisticDataSource();
                PersonPlanStatisticData data = condition.FirstOrDefault(r => r.AddressGuid == condition.Key);

                datasource.GroupGuid = condition.Key;
                if (data != null)
                {
                    datasource.Group = data.Address;
                }
                else
                {
                    datasource.Group = "";
                }

                IEnumerable<IGrouping<string, PersonPlanStatisticData>> groups = condition.GroupBy(r => r.Type);
                foreach (IGrouping<string, PersonPlanStatisticData> group in groups)
                {
                    double count = group.Sum(n => n.Count);
                    datasource[group.Key] = count;

                    int groupcount = statisticgroups.Count(r => r.Name == group.Key);
                    if (groupcount == 0)
                    {
                        NameValuePair<string> pair = new NameValuePair<string>() { Name = group.Key, Value = group.Key };
                        if (!statisticgroups.Contains(pair))
                        {
                            statisticgroups.Add(pair);
                        }
                    }
                }
                datasources.Add(datasource);
            }

            this.statisticListControl.statisticGroup = statisticgroups;
            this.statisticListControl.StatisticItemsSource = datasources;
            SetChartItemsSource(source.ToList());
        }

        private void SetChartItemsSource(List<PersonPlanStatisticData> staticsource)
        {
            this.statisticChartControl.StatisticItemsSource = null;
            if (staticsource != null && staticsource.Count > 0)
            {
                List<SeriesNameValue<double>> result =
                    ((from source in staticsource
                      group source by new
                      {
                          SeriesName = source.Type,
                          Name = source.Address
                      } into s
                      select new SeriesNameValue<double>
                      {
                          SeriesName = s.Key.SeriesName,
                          Name = s.Key.Name,
                          Value = s.Sum(p => p.Count)
                      }).ToList()).OrderBy(r => r.SeriesName).ToList();


                List<SeriesNameValue<double>> totalsource = result.Where(r => r.Name == "合计" && r.Value > 0).ToList();
                List<SeriesNameValue<double>> othersource = result.Where(r => r.Name != "合计" && r.Value > 0).ToList();

                this.statisticChartControl.StatisticItemsSource = othersource;
                //this.statisticChartPieControl.StatisticItemsSource = totalsource;
            }
        }


        private void GroupBox_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var groupBox = (DevExpress.Xpf.LayoutControl.GroupBox)sender;
            groupBox.State = groupBox.State == GroupBoxState.Normal ? GroupBoxState.Maximized : GroupBoxState.Normal;
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="datasource"></param>
        /// <param name="placeguid">类型</param>
        private void statisticListControl_OnCellClick(StatisticDataSource datasource, string placeguid)
        {
            //string activityguid = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid;

            string orgguid = datasource.GroupGuid;
            if (orgguid == "total"  || orgguid == "合计")
            {
                orgguid = string.Empty;
            }

            string type = placeguid;
            switch (type)
            {
                case "设备":
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

                default: //病假、购物等


                    PersonLeaveList personLeaveList = new PersonLeaveList();
                    personLeaveList.Name = orgguid;
                    personLeaveList.Leave_Type = placeguid;
                    personLeaveList.LoadLeaveData();
                    SetStatisticDetailControl(personLeaveList);
                    break;


            }
        }
        private void SetStatisticDetailControl(FrameworkElement control)
        {
            this.gridcontainer.Visibility = Visibility.Visible;
            this.bordercontainer.Child = null;
            this.bordercontainer.Child = control;
        }

        private void buttonGoback_Click(object sender, RoutedEventArgs e)
        {
            this.gridcontainer.Visibility = Visibility.Collapsed;
            this.bordercontainer.Child = null;
        }
    }
}
