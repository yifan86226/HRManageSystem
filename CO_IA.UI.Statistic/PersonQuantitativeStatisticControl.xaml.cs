using AT_BC.Data;
using CO_IA.Data;
using CO_IA.Data.PlanDatabase;
using DataManager.Public;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.LayoutControl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace CO_IA.UI.Statistic
{
    /// <summary>
    /// PersonPlanStatisticControl.xaml 的交互逻辑
    /// </summary>
    public partial class PersonQuantitativeStatisticControl : UserControl, IStatisticObject
    {
        private List<PersonPlanStatisticData> personplanstatisticSource;
        public List<PersonPlanStatisticData> PersonPlanStatisticSource
        {
            get { return personplanstatisticSource; }
            set
            {
                personplanstatisticSource = value;
                Statistic(personplanstatisticSource);
            }
        }

        public PersonQuantitativeStatisticControl()
        {
            InitializeComponent();
  
            //初始化代码

          
            this.de_fromdate.Text = DateTime.Now.Year.ToString() + "-01-01";
            this.de_todate.Text = DateTime.Now.Year.ToString() + "-12-31";


            statisticListControl.OnCellClick_RF += statisticListControl_OnCellClick;


            statisticListControl.OnMouseLeftButtonDownnClick += StatisticListControl_OnMouseLeftButtonDownnClick;



            InitSource();

            StatisticModel model = new StatisticModel();

            List<NameValuePair<double>> datalist = model.GetPersonRPScoreByDate(de_fromdate.Text, de_todate.Text);
            List<SeriesNameValue<double>> othersource = new List<SeriesNameValue<double>>();
            foreach (NameValuePair<double> nvp in datalist)
            {
                SeriesNameValue<double> snv = new SeriesNameValue<double>();

                snv.Name = snv.SeriesName = nvp.Name;
                snv.Value = nvp.Value;

                othersource.Add(snv);
            }


            this.statisticChartControl.StatisticItemsSource = othersource;
            
            ShowLegend = true;
        }


        private void GroupBox_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var groupBox = (DevExpress.Xpf.LayoutControl.GroupBox)sender;
            groupBox.State = groupBox.State == GroupBoxState.Normal ? GroupBoxState.Maximized : GroupBoxState.Normal;
        }



        /// <summary>
        /// 委托的左键点击事件
        /// </summary>
        /// <param name="sds"></param>
        /// <param name="nameid"></param>
        private void StatisticListControl_OnMouseLeftButtonDownnClick(StatisticDataSource sds, string nameid)
        {
            //DataList = new StringPairList();



            //DataManager.Public.PersonRewardPunishInfoModel model = new DataManager.Public.PersonRewardPunishInfoModel();

            //List<PersonRewardPunishInfo> ppilist = model.GetPersonRewardPunishInfos("", "", nameid, de_fromdate.DateTime.ToString("yyyy-MM-dd"), de_todate.DateTime.ToString("yyyy-MM-dd"), "order by RPTIME ");

            //double tatolScore = 0;

            //foreach (PersonRewardPunishInfo ppi in ppilist)
            //{
            //    StringPair sp = new StringPair();

            //    string datestr = "";
            //    try
            //    {
            //        datestr = Convert.ToDateTime(ppi.RPTIME).ToString("yyyy-MM-dd");
            //    }
            //    catch
            //    {

            //    }

            //    sp.Key = datestr;
            
            //    tatolScore = tatolScore + ppi.FRACTION;
            //    sp.Value = tatolScore.ToString();


            //    DataList.SPList.Add(sp);

            //}

            ////DataList.SPList.Sort();
            //this.Series.DataSource = null;
            //this.Series.DataSource = DataList.SPList;
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


        //public StringPairList DataList { get; private set; }

        public void InitSource()
        {

            this.statisticChartControl.TitleContent = "人员量化数据统计图";

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
                //if (showlegend)
                //{
                //    row1.Height = new GridLength(1, GridUnitType.Star);
                //    statisticChartControl.Visibility = Visibility.Visible;
                //}
                //else
                //{
                //    row1.Height = new GridLength(0);
                //    statisticChartControl.Visibility = Visibility.Collapsed;
                //}

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

                double totalNum = 0;
                IEnumerable<IGrouping<string, PersonPlanStatisticData>> groups = condition.GroupBy(r => r.Type);
                foreach (IGrouping<string, PersonPlanStatisticData> group in groups)
                {
                    double count = group.Sum(n => n.Count);
                    datasource[group.Key] = count;

                    totalNum = totalNum + count;
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

                datasource["合计"] =Math.Round( totalNum,1);

                

                datasources.Add(datasource);
            }


            NameValuePair<string> hjpair = new NameValuePair<string>() { Name = "合计", Value = "合计" };
            if (!statisticgroups.Contains(hjpair))
            {
                statisticgroups.Add(hjpair);
            }



            this.statisticListControl.statisticGroup = statisticgroups;
            this.statisticListControl.StatisticItemsSource = datasources;
       
        }

        /// <summary>
        /// 查询统计
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Stat_Click(object sender, RoutedEventArgs e)
        {

            DataManager.Public.StatisticModel model = new DataManager.Public.StatisticModel();

            PersonPlanStatisticSource = model.GetPersonRPStatByDate(de_fromdate.DateTime.ToString("yyyy-MM-dd"), de_todate.DateTime.ToString("yyyy-MM-dd")).ToList<PersonPlanStatisticData>(); 

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="datasource"></param>
        /// <param name="incident">类型</param>
        private void statisticListControl_OnCellClick(StatisticDataSource datasource, string incident)
        {
            //string activityguid = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid;

            string useid = datasource.GroupGuid; //取得用户ID
            if (useid == "total" || useid == "合计" || useid == "数据详情")
            {
                useid = string.Empty;
            }

            string type = incident; //要查询的统计类型
                                    //switch (type)
                                    //{

            if (string.IsNullOrEmpty(type))
            {

                #region  delete
                //DataList = new StringPairList();

                //DataManager.Public.PersonRewardPunishInfoModel model = new DataManager.Public.PersonRewardPunishInfoModel();

                //List<PersonRewardPunishInfo> ppilist = model.GetPersonRewardPunishInfos("", "", useid, de_fromdate.DateTime.ToString("yyyy-MM-dd"), de_todate.DateTime.ToString("yyyy-MM-dd"), "order by RPTIME ");

                //double tatolScore = 0;

                //foreach (PersonRewardPunishInfo ppi in ppilist)
                //{

                //    string datestr = "";
                //    try
                //    {
                //        datestr = Convert.ToDateTime(ppi.RPTIME).ToString("yyyy-MM-dd");
                //    }
                //    catch
                //    {

                //    }


                //    foreach (StringPair tempsp in DataList.SPList)
                //    {
                //        if (tempsp.Key == datestr)
                //        {
                //            tempsp.Value = (Convert.ToDouble(tempsp.Value) +ppi.FRACTION).ToString(); 

                //            continue;
                //        }

                //    }

                //    StringPair sp = new StringPair();



                //    sp.Key = datestr;


                //    tatolScore = tatolScore + ppi.FRACTION;
                //    sp.Value = tatolScore.ToString();


                //    DataList.SPList.Add(sp);

                //}
                //this.Series.DataSource = null;
                //this.Series.DataSource = DataList.SPList;
                #endregion

                StatisticLineChartControl statisticLineChartControl = new StatisticLineChartControl();
                statisticLineChartControl.NameID = useid;
               
                if (string.IsNullOrEmpty(de_fromdate.Text) == false)
                {
                    statisticLineChartControl.FromDate = de_fromdate.DateTime.ToString("yyyy-MM-dd");
                }

                if (string.IsNullOrEmpty(de_todate.Text) == false)
                {
                    statisticLineChartControl.ToDate = de_todate.DateTime.ToString("yyyy-MM-dd");
                }
                statisticLineChartControl.LoadData();
                SetStatisticDetailControl(statisticLineChartControl);

            }
            else
            {

                //人员id  //类型 // 时间段

                //default: //病假、购物等

                PersonRPList personrplist = new PersonRPList();
                personrplist.NameID = useid;
                personrplist.Incident = type;
                if (string.IsNullOrEmpty(de_fromdate.Text) == false)
                {
                    personrplist.FromDate = de_fromdate.DateTime.ToString("yyyy-MM-dd");
                }

                if (string.IsNullOrEmpty(de_todate.Text) == false)
                {
                    personrplist.ToDate = de_todate.DateTime.ToString("yyyy-MM-dd");
                }
                personrplist.LoadData();
                SetStatisticDetailControl(personrplist);
                //break;
                
                //}
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
