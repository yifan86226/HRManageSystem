using CO_IA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace CO_IA.UI.ActivitySummarize
{
    public delegate void ChangeUCDelegate(object sender, MouseButtonEventArgs e);

    public delegate void PromptMessageDelegate(string text);


    public class Canstant
    {

        /// <summary>
        /// 更换页面
        /// </summary>
        public event ChangeUCDelegate ChangeUCHandler;

        public void ChangeUC(object sender, MouseButtonEventArgs e)
        {
            ChangeUCHandler(sender, e);
        }


        #region 常量
        private static Canstant canstant;

        public static Canstant GetInstance()
        {
            if (canstant == null)
            {
                canstant = new Canstant();
            }
            return canstant;
        }
        private Canstant()
        {


            activity = CO_IA.Client.RiasPortal.ModuleContainer.Activity;
        }
        #endregion

        /// <summary>
        /// 台站地址数目
        /// </summary>
        private int stationPositionCount = 0;

        public int StationPositionCount
        {

            get { return stationPositionCount; }
            set { stationPositionCount = value; }
        }



        public List<SummarizeChart> SummarizeChartList
        {
            get
            {

                List<SummarizeChart> SummarizeChartlist = new List<SummarizeChart>();

                #region A
                SummarizeChart SummarizeChart = new SummarizeChart();
                SummarizeChart.Type = "A类站";


                List<BasicSummarizeChart> list = new List<BasicSummarizeChart>();


                BasicSummarizeChart item2006 = new BasicSummarizeChart();
                item2006.Name = "2006";
                item2006.Value = 3;

                list.Add(item2006);

                BasicSummarizeChart item12007 = new BasicSummarizeChart();
                item12007.Name = "2007";
                item12007.Value = 5;

                list.Add(item12007);

                BasicSummarizeChart item22008 = new BasicSummarizeChart();
                item22008.Name = "2008";
                item22008.Value = 9;

                list.Add(item22008);

                BasicSummarizeChart item32009 = new BasicSummarizeChart();
                item32009.Name = "2009";
                item32009.Value = 11;
                list.Add(item32009);


                BasicSummarizeChart item = new BasicSummarizeChart();
                item.Name = "2010";
                item.Value = 26;

                list.Add(item);

                BasicSummarizeChart item1 = new BasicSummarizeChart();
                item1.Name = "2011";
                item1.Value = 32;

                list.Add(item1);

                BasicSummarizeChart item2 = new BasicSummarizeChart();
                item2.Name = "2012";
                item2.Value = 40;

                list.Add(item2);

                BasicSummarizeChart item3 = new BasicSummarizeChart();
                item3.Name = "2013";
                item3.Value = 46;

                list.Add(item3);


                SummarizeChart.List = list;

                SummarizeChartlist.Add(SummarizeChart);

                #endregion

                #region B

                SummarizeChart SummarizeChart1 = new SummarizeChart();
                SummarizeChart1.Type = "B类站";

                List<BasicSummarizeChart> list1 = new List<BasicSummarizeChart>();


                BasicSummarizeChart item42006 = new BasicSummarizeChart();
                item42006.Name = "2006";
                item42006.Value = 3;

                list1.Add(item42006);

                BasicSummarizeChart item52007 = new BasicSummarizeChart();
                item52007.Name = "2007";
                item52007.Value = 5;

                list1.Add(item52007);

                BasicSummarizeChart item62008 = new BasicSummarizeChart();
                item62008.Name = "2008";
                item62008.Value = 9;

                list1.Add(item62008);

                BasicSummarizeChart item72009 = new BasicSummarizeChart();
                item72009.Name = "2009";
                item72009.Value = 17;
                list1.Add(item72009);


                BasicSummarizeChart item4 = new BasicSummarizeChart();
                item4.Name = "2010";
                item4.Value = 21;

                list1.Add(item4);

                BasicSummarizeChart item5 = new BasicSummarizeChart();
                item5.Name = "2011";
                item5.Value = 24;

                list1.Add(item5);

                BasicSummarizeChart item6 = new BasicSummarizeChart();
                item6.Name = "2012";
                item6.Value = 32;

                list1.Add(item6);

                BasicSummarizeChart item7 = new BasicSummarizeChart();
                item7.Name = "2013";
                item7.Value = 36;

                list1.Add(item7);


                SummarizeChart1.List = list1;

                SummarizeChartlist.Add(SummarizeChart1);
                #endregion

                #region C
                SummarizeChart SummarizeChart2 = new SummarizeChart();
                SummarizeChart2.Type = "C类站";

                List<BasicSummarizeChart> list2 = new List<BasicSummarizeChart>();


                BasicSummarizeChart item82006 = new BasicSummarizeChart();
                item82006.Name = "2006";
                item82006.Value = 5;

                list2.Add(item82006);

                BasicSummarizeChart item92007 = new BasicSummarizeChart();
                item92007.Name = "2007";
                item92007.Value = 8;

                list2.Add(item92007);

                BasicSummarizeChart item112008 = new BasicSummarizeChart();
                item112008.Name = "2008";
                item112008.Value = 14;

                list2.Add(item112008);

                BasicSummarizeChart item122009 = new BasicSummarizeChart();
                item122009.Name = "2009";
                item122009.Value = 23;
                list2.Add(item122009);

                BasicSummarizeChart item8 = new BasicSummarizeChart();
                item8.Name = "2010";
                item8.Value = 31;

                list2.Add(item8);

                BasicSummarizeChart item9 = new BasicSummarizeChart();
                item9.Name = "2011";
                item9.Value = 35;

                list2.Add(item9);

                BasicSummarizeChart item10 = new BasicSummarizeChart();
                item10.Name = "2012";
                item10.Value = 37;

                list2.Add(item10);

                BasicSummarizeChart item11 = new BasicSummarizeChart();
                item11.Name = "2013";
                item11.Value = 41;

                list2.Add(item11);

                SummarizeChart2.List = list2;

                SummarizeChartlist.Add(SummarizeChart2);

                #endregion


                return SummarizeChartlist;
            }

        }


        /// <summary>
        /// 活动的主体
        /// </summary>
        private Activity activity;

        public List<BasicSummarizeChart> BasicSummarizeChartListForWorkType
        {
            get
            {
                List<BasicSummarizeChart> list = new List<BasicSummarizeChart>();

                BasicSummarizeChart item = new BasicSummarizeChart();
                item.Name = "已清理";
                item.Value = 13;

                list.Add(item);

                BasicSummarizeChart item1 = new BasicSummarizeChart();
                item1.Name = "未清理";
                item1.Value = 35;

                list.Add(item1);

                BasicSummarizeChart item2 = new BasicSummarizeChart();
                item2.Name = "待确认";
                item2.Value = 54;

                list.Add(item2);

                return list;
            }

        }

        public List<BasicSummarizeChart> BasicSummarizeChartListForCallResource
        {
            get
            {

                List<BasicSummarizeChart> list = new List<BasicSummarizeChart>();

                BasicSummarizeChart item = new BasicSummarizeChart();
                item.Name = "已占用";
                item.Value = 46;

                list.Add(item);

                BasicSummarizeChart item1 = new BasicSummarizeChart();
                item1.Name = "未占用";
                item1.Value = 25;

                list.Add(item1);

                BasicSummarizeChart item2 = new BasicSummarizeChart();
                item2.Name = "预留";
                item2.Value = 23;

                list.Add(item2);

                BasicSummarizeChart item3 = new BasicSummarizeChart();
                item3.Name = "注销";
                item3.Value = 14;

                list.Add(item3);


                return list;
            }

        }

        /// <summary>
        /// 人员地区分布
        /// </summary>
        public List<BasicSummarizeChart> PersonAreaStatistics
        {
            get
            {
                try
                {
                    List<BasicSummarizeChart> list = new List<BasicSummarizeChart>();

                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                    {
                        //更新当前节点
                        list = channel.GetPersonAreaStatistics(activity.Guid);
                    });

                    return list;
                }
                catch
                { }
                return null;
            }
        }

        /// <summary>
        /// 人员组织构成
        /// </summary>
        public List<BasicSummarizeChart> StaffOrgStatistics
        {
            get
            {
                try
                {
                    List<BasicSummarizeChart> list = new List<BasicSummarizeChart>();

                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                    {
                        //更新当前节点
                        list = channel.GetStaffOrgStatistics(activity.Guid);
                    });

                    return list;
                }
                catch
                { }
                return null;
            }

        }

        /// <summary>
        /// 人员分布统计图
        /// </summary>
        public List<BasicSummarizeChart> PersonDistributedStatistics
        {
            get
            {
                try
                {
                    List<BasicSummarizeChart> list = new List<BasicSummarizeChart>();


                    List<BasicSummarizeChart> reflist = new List<BasicSummarizeChart>();



                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                    {
                        //更新当前节点
                        reflist = channel.GetPersonDistributedStatistics(activity.Guid);
                    });
                    double wwm = 0;
                    double qtm = 0;
                    foreach (BasicSummarizeChart cartItem in reflist)
                    {

                        if (cartItem.Name == "3")
                        {
                            wwm = cartItem.Value;
                        }
                        else
                        {
                            qtm += cartItem.Value;
                        }
                    }

                    BasicSummarizeChart wwitem = new BasicSummarizeChart();
                    wwitem.Name = "无委人员";
                    wwitem.Value = wwm;

                    list.Add(wwitem);


                    BasicSummarizeChart qtitem = new BasicSummarizeChart();
                    qtitem.Name = "非无委人员";
                    qtitem.Value = qtm;

                    list.Add(qtitem);

                    return list;
                }
                catch
                { }
                return null;
            }
        }



        /// <summary>
        /// 人员车辆设备分布统计
        /// </summary>
        public List<BasicSummarizeChart> ActivityPEVStatistics
        {
            get
            {
                try
                {
                    List<BasicSummarizeChart> list = new List<BasicSummarizeChart>();

                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                    {
                        //更新当前节点
                        list = channel.GetActivityPEVStatistics(activity.Guid);
                    });

                    return list;
                }
                catch
                { }
                return null;
            }
        }



        /// <summary>
        /// 台站清理结果统计
        /// </summary>
        public List<BasicSummarizeChart> StationClearStatistics
        {
            get
            {
                try
                {
                    List<BasicSummarizeChart> list = new List<BasicSummarizeChart>();

                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivitySummarize.I_CO_IA_ActivitySummarize>(channel =>
                    {
                        //更新当前节点
                        list = channel.GetStationClearStatistics(activity.Guid);
                    });

                    foreach (BasicSummarizeChart cartItem in list)
                    {
                        if (cartItem.Name == "0")
                        {
                            cartItem.Name = "未作处理";
                        }
                        else if (cartItem.Name == "1")
                        {
                            cartItem.Name = "清理成功";
                        }
                        else if (cartItem.Name == "2")
                        {
                            cartItem.Name = "清理失败";
                        }
                    }

                    if (list.Count == 0)
                    {
                        BasicSummarizeChart item0 = new BasicSummarizeChart();
                        item0.Name = "未作处理";
                        item0.Value = 0;
                        list.Add(item0);

                        BasicSummarizeChart item1 = new BasicSummarizeChart();
                        item0.Name = "清理成功";
                        item0.Value = 0;
                        list.Add(item0);

                        BasicSummarizeChart item2 = new BasicSummarizeChart();
                        item0.Name = "清理失败";
                        item0.Value = 0;
                        list.Add(item0);
                    }

                    return list;
                }
                catch
                { }
                return null;
            }
        }


    }
}