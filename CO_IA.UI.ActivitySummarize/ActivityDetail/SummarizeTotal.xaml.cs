using CO_IA.Client;
using CO_IA.Data;
using CO_IA.Data.ActivitySummarize;
using I_CO_IA.ActivitySummarize;
using Microsoft.Win32;
using PT_BS_Service.Client.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CO_IA.UI.ActivitySummarize
{
    /// <summary>
    /// SummarizeTotal.xaml 的交互逻辑
    /// </summary>
    public partial class SummarizeTotal : Window
    {
        string activityId = "";
        private Dictionary<string, string> dicKey = new Dictionary<string, string>();
        SummarizeTotalEnum stEnum = new SummarizeTotalEnum();
        SummarizeDoc currentsummarize = new SummarizeDoc();
        public SummarizeTotal()
        {
            InitializeComponent();
            activityId = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid;
            //GetlstboxKey(null);
            //bindDataGrid();
            GetSummarizeTotal();
            
            this.DataContext = this;
            InitTemplate();
        }
        private void GetlstboxKey(List<int> totalKey)
        {
            dicKey.Clear();
            lstboxKey.ItemsSource = null;
            foreach (SummarizeTotalEnum.SummarizeTotalEnumType myCode in Enum.GetValues(typeof(SummarizeTotalEnum.SummarizeTotalEnumType)))
            {
                bool flg = false;
                string name = "";
                int values;
                string description = "";
                name = myCode.ToString();
                values = (int)myCode;

                if (totalKey != null)
                {
                    for (int i = 0; i < totalKey.Count(); i++)
                    {
                        if (values == totalKey[i])
                        {
                            flg = true;
                            break;
                        }
                    }
                }
                if (flg)
                {
                    continue;
                }

                description = stEnum.GetDiscription(myCode);
                dicKey.Add(values.ToString(), description);
            }
            lstboxKey.ItemsSource = dicKey;
            
        }
        public SummarizeTotalData[] SummarizeTotalItemsSource
        {
            get { return (SummarizeTotalData[])GetValue(SummarizeTotalItemsSourceProperty); }
            set { SetValue(SummarizeTotalItemsSourceProperty, value); }
        }
        public static readonly DependencyProperty SummarizeTotalItemsSourceProperty =
    DependencyProperty.Register("SummarizeTotalItemsSource", typeof(SummarizeTotalData[]), typeof(SummarizeTotal), new PropertyMetadata(null, null));

        private SummarizeTotalData[] GetSummarizeTotal()
        {
            List<int> totalKey = new List<int>();

            List<SummarizeTotalData> list = new List<SummarizeTotalData>();
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivitySummarize.I_CO_IA_ActivitySummarize>
                        (channel =>
                        {
                            list = channel.GetSummarizeTotal(activityId);
                        });
            for (int i = 0; i < list.Count(); i++)
            {
                SummarizeTotalEnum.SummarizeTotalEnumType temp = (SummarizeTotalEnum.SummarizeTotalEnumType)Convert.ToInt32(list[i].KEY);
                string description = stEnum.GetDiscription(temp);
                list[i].DESCRIPTION = description;

                totalKey.Add(Convert.ToInt32(list[i].KEY));
            }

            SummarizeTotalItemsSource = list.ToArray();

            GetlstboxKey(totalKey);

            return SummarizeTotalItemsSource;
        }
        //private void bindDataGrid()
        //{
        //    GetSummarizeTotal();
        //}

        private List<string> GetKey()
        {
            List<string> keys = new List<string>();
            if (lstboxKey.SelectedItems.Count > 0)
            {
                foreach (KeyValuePair<string, string> item in lstboxKey.SelectedItems)
                {
                    keys.Add(item.Key);
                }
            }
            return keys;
        }

        private void btnTotal_Click(object sender, RoutedEventArgs e)
        {
            List<string> keys = GetKey();
            for (int i = 0; i < keys.Count(); i++)
            {
                if (!saveTotal(keys[i].ToString()))
                {
                    MessageBox.Show("统计时出现失败", "消息提示", MessageBoxButton.OK);
                    break;
                }
            }
            //bindDataGrid();
            GetSummarizeTotal();
            //this.DataContext = this;
        }
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            string guid = ((Button)sender).Uid.ToString();
            for (int i = 0; i < SummarizeTotalItemsSource.Length; i++)
            {
                if (SummarizeTotalItemsSource[i].GUID == guid)
                {
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivitySummarize.I_CO_IA_ActivitySummarize>
                        (channel =>
                        {
                            bool flg = channel.UpdateSummarizeTotal(SummarizeTotalItemsSource[i]);
                            if (flg)
                            {
                                MessageBox.Show("更新成功", "消息提示", MessageBoxButton.OK);
                                GetSummarizeTotal();
                            }
                            else
                            {
                                MessageBox.Show("更新失败", "消息提示", MessageBoxButton.OK);
                            }
                        });
                }
            }
        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            string guid = ((Button)sender).Uid.ToString();
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivitySummarize.I_CO_IA_ActivitySummarize>
                        (channel =>
                        {
                            bool flg = channel.DeleteSummarizeTotal(guid);
                            if (flg)
                            {
                                MessageBox.Show("删除成功", "消息提示", MessageBoxButton.OK);
                                GetSummarizeTotal();
                            }
                            else
                            {
                                MessageBox.Show("删除失败", "消息提示", MessageBoxButton.OK);
                            }
                        });
        }

        private bool saveTotal(string key)
        {
            bool ret = false;
            SummarizeTotalData stData = new SummarizeTotalData();
            switch (key)
            {
                case "1":
                    stData = GetTotalForGuaranteePerson();
                    break;
                case "2":
                    stData = GetTotalForGuaranteeCount();
                    break;
                case "3":
                    stData = GetTotalForMonitorCar();
                    break;
                case "4":
                    stData = GetTotalForMonitorCount();
                    break;
                case "5":
                    stData = GetTotalForMonitorStand();
                    break;
                case "6":
                    stData = GetTotalForMonitorTime();
                    break;
                case "7":
                    stData = GetTotalForDistribution();
                    break;
                case "8":
                    stData = GetTotalForApproveRadio();
                    break;
                case "9":
                    stData = GetTotalForCheckEquipment();
                    break;
                case "10":
                    stData = GetTotalForInvestigateInterference();
                    break;
                case "11":
                    stData = GetTotalapproveRadioTotal();
                    break;
                default:
                    break;

            }
            List<SummarizeTotalData> listStData = new List<SummarizeTotalData>();
            if (stData != null)
            {
                listStData.Add(stData);
            }
            if (listStData != null && listStData.Count() > 0)
            {
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivitySummarize.I_CO_IA_ActivitySummarize>
                            (channel =>
                            {
                                ret = channel.SaveSummarizeTotal(listStData);

                            });
            }
            return ret;
        }
        /// <summary>
        /// 获取人员保障
        /// </summary>
        /// <returns></returns>
        private SummarizeTotalData GetTotalForGuaranteePerson()
        {
            SummarizeTotalData stData = new SummarizeTotalData();
            stData.GUID = CO_IA.Client.Utility.NewGuid();
            stData.ACTIVITY_GUID = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid;

            stData.KEY = "1";

            int PersonCount = 0;

            List<PP_OrgInfo> nodes = new List<PP_OrgInfo>();
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
            {
                //更新当前节点
                nodes = channel.GetPP_OrgInfos(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
            });
            if (nodes != null && nodes.Count() > 0)
            {
                for (int i = 0; i < nodes.Count(); i++)
                {
                    List<PP_PersonInfo> itemPersonList = new List<PP_PersonInfo>();
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                    {
                        //更新当前节点
                        itemPersonList = channel.GetPP_PersonInfos(nodes[i].GUID);

                        if (itemPersonList != null)
                        {
                            var itemPersonListNew = itemPersonList.Where(o => o.NAME != null);
                            PersonCount += itemPersonListNew.Count();
                            //PersonCount += itemPersonList.Count();
                        }
                    });
                }
            }

            //ScheduleDetail[] Details = Utility.getOrgGroupsBySchedule(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid, CO_IA.Client.RiasPortal.ModuleContainer.Activity.ActivityStage);
            //if (Details != null && Details.Length > 0)
            //{
            //    for (int i = 0; i < Details.Length; i++)
            //    {
            //        ScheduleOrg[] scheduleOrgs = Details[i].ScheduleOrgs;
            //        if (scheduleOrgs != null && scheduleOrgs.Length > 0)
            //        {
            //            for (int j = 0; j < scheduleOrgs.Length; j++)
            //            {
            //                PP_OrgInfo orgInfo = scheduleOrgs[j].OrgInfo;
            //                if (orgInfo != null)
            //                {
            //                    string orgId = orgInfo.GUID;
            //                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
            //                    {
            //                        //更新当前节点
            //                       List<PP_PersonInfo> itemPersonList = channel.GetPP_PersonInfos(orgId);
            //                       if (itemPersonList != null)
            //                       {
            //                           var infos = itemPersonList.Where(item=>!string.IsNullOrEmpty(item.NAME)).ToArray();
            //                           PersonCount += infos.Length;
            //                       }
            //                    });
            //                }
            //            }
            //        }
            //    }
            //}

            stData.SUMMARIZEVALUE = PersonCount;
            stData.UPDATEVALUE = PersonCount;

            return stData;
        }
        /// <summary>
        /// 获取现场保障人次
        /// </summary>
        /// <returns></returns>
        private SummarizeTotalData GetTotalForGuaranteeCount()
        {
            SummarizeTotalData stData = new SummarizeTotalData();
            stData.GUID = CO_IA.Client.Utility.NewGuid();
            stData.ACTIVITY_GUID = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid;
            //测试
            stData.KEY = "2";
          

            stData.SUMMARIZEVALUE = 0;
            stData.UPDATEVALUE = 0;

            return stData;
        }
        /// <summary>
        /// 获取监测车辆
        /// </summary>
        /// <returns></returns>
        private SummarizeTotalData GetTotalForMonitorCar()
        {
            SummarizeTotalData stData = new SummarizeTotalData();
            stData.GUID = CO_IA.Client.Utility.NewGuid();
            stData.ACTIVITY_GUID = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid;

            stData.KEY = "3";

            int VehicleCount = 0;

            List<PP_OrgInfo> nodes = new List<PP_OrgInfo>();
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
            {
                //更新当前节点
                nodes = channel.GetPP_OrgInfos(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
            });
            if (nodes != null && nodes.Count() > 0)
            {
                for (int i = 0; i < nodes.Count(); i++)
                {
                    PP_VehicleInfo itemVehicle = new PP_VehicleInfo();
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                    {
                        //更新当前节点
                        itemVehicle = channel.GetPP_VehicleInfo(nodes[i].GUID);
                        if (itemVehicle != null && !string.IsNullOrEmpty(itemVehicle.GUID))
                        {
                            VehicleCount++;
                        }
                    });
                }
            }
            //ScheduleDetail[] Details = Utility.getOrgGroupsBySchedule(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid, CO_IA.Client.RiasPortal.ModuleContainer.Activity.ActivityStage);
            //if (Details != null && Details.Length > 0)
            //{
            //    for (int i = 0; i < Details.Length; i++)
            //    {
            //        ScheduleOrg[] scheduleOrgs = Details[i].ScheduleOrgs;
            //        if (scheduleOrgs != null && scheduleOrgs.Length > 0)
            //        {
            //            for (int j = 0; j < scheduleOrgs.Length; j++)
            //            {
            //                PP_OrgInfo orgInfo = scheduleOrgs[j].OrgInfo;
            //                if (orgInfo != null)
            //                {
            //                    string orgId = orgInfo.GUID;
            //                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
            //                    {
            //                        //更新当前节点
            //                        PP_VehicleInfo itemVehicle = channel.GetPP_VehicleInfo(orgId);
            //                        if (itemVehicle != null && !string.IsNullOrEmpty(itemVehicle.GUID))
            //                        {
            //                            VehicleCount += 1;
            //                        }
            //                    });
            //                }
            //            }
            //        }
            //    }
            //}

            stData.SUMMARIZEVALUE = VehicleCount;
            stData.UPDATEVALUE = VehicleCount;

            return stData;
        }
        /// <summary>
        /// 获取机动监测台次
        /// </summary>
        /// <returns></returns>
        private SummarizeTotalData GetTotalForMonitorCount()
        {
            SummarizeTotalData stData = new SummarizeTotalData();
            stData.GUID = CO_IA.Client.Utility.NewGuid();
            stData.ACTIVITY_GUID = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid;
            //测试
            stData.KEY = "4";
            stData.SUMMARIZEVALUE = 0;
            stData.UPDATEVALUE = 0;

            return stData;
        }
        /// <summary>
        /// 获取固定监测站
        /// </summary>
        /// <returns></returns>
        private SummarizeTotalData GetTotalForMonitorStand()
        {
            SummarizeTotalData stData = new SummarizeTotalData();
            stData.GUID = CO_IA.Client.Utility.NewGuid();
            stData.ACTIVITY_GUID = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid;
            //测试
            stData.KEY = "5";

            int count = 0;
            //FixedStationQueryCondition con = new FixedStationQueryCondition();

            //List<FixedStationInfo> fsData = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PlanDatabase.I_CO_IA_PlanDatabase, List<FixedStationInfo>>(
            //            channel =>
            //            {
            //                return channel.SelectMonitorStation(con);
            //            });
            //if (fsData != null)
            //{
            //    count = fsData.Count();
            //}
            stData.SUMMARIZEVALUE = count;
            stData.UPDATEVALUE = count;

            return stData;
        }
        /// <summary>
        /// 获取单站监测时长
        /// </summary>
        /// <returns></returns>
        private SummarizeTotalData GetTotalForMonitorTime()
        {
            SummarizeTotalData stData = new SummarizeTotalData();
            stData.GUID = CO_IA.Client.Utility.NewGuid();
            stData.ACTIVITY_GUID = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid;
            //测试
            stData.KEY = "6";
            stData.SUMMARIZEVALUE = 0;
            stData.UPDATEVALUE = 0;

            return stData;
        }
        /// <summary>
        /// 获取指配频率
        /// </summary>
        /// <returns></returns>
        private SummarizeTotalData GetTotalForDistribution()
        {
            SummarizeTotalData stData = new SummarizeTotalData();
            stData.GUID = CO_IA.Client.Utility.NewGuid();
            stData.ACTIVITY_GUID = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid;

            stData.KEY = "7";
            int count = 0;
            FreqAssignStatisticData[] fsData = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.Statistic.I_CO_IA_Statistic, FreqAssignStatisticData[]>(
                        channel =>
                        {
                            return channel.StatisticFreqAssign(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
                        });
            if (fsData != null && fsData.Length > 0)
            {
                for (int i = 0; i < fsData.Length; i++)
                {
                    count += fsData[i].Count;
                }
            }

            stData.SUMMARIZEVALUE = count;
            stData.UPDATEVALUE = count;

            return stData;
        }
        /// <summary>
        /// 获取审批无线电台
        /// </summary>
        /// <returns></returns>
        private SummarizeTotalData GetTotalForApproveRadio()
        {
            SummarizeTotalData stData = new SummarizeTotalData();
            stData.GUID = CO_IA.Client.Utility.NewGuid();
            stData.ACTIVITY_GUID = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid;
            //测试
            stData.KEY = "8";
            stData.SUMMARIZEVALUE = 0;
            stData.UPDATEVALUE = 0;
            return stData;
        }
        /// <summary>
        /// 获取抽检设备
        /// </summary>
        /// <returns></returns>
        private SummarizeTotalData GetTotalForCheckEquipment()
        {
            SummarizeTotalData stData = new SummarizeTotalData();
            stData.GUID = CO_IA.Client.Utility.NewGuid();
            stData.ACTIVITY_GUID = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid;
            //测试
            stData.KEY = "9";
            int count = 0;
            EquInspectionStatisticData[] eisData = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.Statistic.I_CO_IA_Statistic, EquInspectionStatisticData[]>(
            channel =>
            {
                return channel.GetEquInspectionStats(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
            });
            if (eisData != null)
            {
               var list = (eisData.ToList()).Where(o => (int)o.InspectionState == 1 || (int)o.InspectionState == 2);
               if (list != null)
               {
                   foreach (var li in list)
                   {
                       count += li.Count;
                   }
               }
            }

            stData.SUMMARIZEVALUE = count;
            stData.UPDATEVALUE = count;
            return stData;
        }
        /// <summary>
        /// 获取查处干扰
        /// </summary>
        /// <returns></returns>
        private SummarizeTotalData GetTotalForInvestigateInterference()
        {
            SummarizeTotalData stData = new SummarizeTotalData();
            stData.GUID = CO_IA.Client.Utility.NewGuid();
            stData.ACTIVITY_GUID = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid;
            //测试
            stData.KEY = "10";
            stData.SUMMARIZEVALUE = 0;
            stData.UPDATEVALUE = 0;
            return stData;
        }
        /// <summary>
        /// 审批无线电台总数
        /// </summary>
        /// <returns></returns>
        private SummarizeTotalData GetTotalapproveRadioTotal()
        {
            SummarizeTotalData stData = new SummarizeTotalData();
            stData.GUID = CO_IA.Client.Utility.NewGuid();
            stData.ACTIVITY_GUID = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid;
            //测试
            stData.KEY = "11";
            stData.SUMMARIZEVALUE = 0;
            stData.UPDATEVALUE = 0;
            return stData;
        }
        
        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            if (combTemplate.SelectedItemValue == null)
            {
                MessageBox.Show("请选择总结模板", "消息提示", MessageBoxButton.OK);
                return;
            }
            string guid = ((ItemsData)combTemplate.SelectedItem).Key;

            string path = downLoadTemplate(guid);
            if (path == null || path == "")
            {
                MessageBox.Show("模板下载出现问题，请稍后尝试", "消息提示", MessageBoxButton.OK);
                return;
            }
            AT_BC.Offices.Word.BeWord helper = AT_BC.Offices.Word.BeWord.LoadFromTemplate(path);
            //WordHelperCS helper = new WordHelperCS();
            //string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Template\\总结模板.doc";
            //helper.CreateNewWordDocument(path);

            List<SummarizeTotalData> list = new List<SummarizeTotalData>();
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivitySummarize.I_CO_IA_ActivitySummarize>
                        (channel =>
                        {
                            list = channel.GetSummarizeTotal(activityId);
                        });
            if (list != null && list.Count() > 0)
            {
                for (int i = 0; i < list.Count(); i++)
                {
                    if (list[i].KEY == ((int)SummarizeTotalEnum.SummarizeTotalEnumType.guaranteePerson).ToString())
                    {
                        helper.ReplaceBookmark("保障人员", list[i].UPDATEVALUE.ToString());
                    }
                    if (list[i].KEY == ((int)SummarizeTotalEnum.SummarizeTotalEnumType.guaranteeCount).ToString())
                    {
                        helper.ReplaceBookmark("现场保障人次", list[i].UPDATEVALUE.ToString());
                    }
                    if (list[i].KEY == ((int)SummarizeTotalEnum.SummarizeTotalEnumType.monitorCar).ToString())
                    {
                        helper.ReplaceBookmark("监测车辆", list[i].UPDATEVALUE.ToString());
                    }
                    if (list[i].KEY == ((int)SummarizeTotalEnum.SummarizeTotalEnumType.monitorCount).ToString())
                    {
                        helper.ReplaceBookmark("机动监测台次", list[i].UPDATEVALUE.ToString());
                    }
                    if (list[i].KEY == ((int)SummarizeTotalEnum.SummarizeTotalEnumType.monitorStand).ToString())
                    {
                        helper.ReplaceBookmark("固定监测站", list[i].UPDATEVALUE.ToString());
                    }
                    if (list[i].KEY == ((int)SummarizeTotalEnum.SummarizeTotalEnumType.monitorTime).ToString())
                    {
                        helper.ReplaceBookmark("单站监测时长", list[i].UPDATEVALUE.ToString());
                    }
                    if (list[i].KEY == ((int)SummarizeTotalEnum.SummarizeTotalEnumType.distribution).ToString())
                    {
                        helper.ReplaceBookmark("指配频率", list[i].UPDATEVALUE.ToString());
                    }
                    if (list[i].KEY == ((int)SummarizeTotalEnum.SummarizeTotalEnumType.approveRadio).ToString())
                    {
                        helper.ReplaceBookmark("审批无线电台（家）", list[i].UPDATEVALUE.ToString());
                    }
                    if (list[i].KEY == ((int)SummarizeTotalEnum.SummarizeTotalEnumType.checkEquipment).ToString())
                    {
                        helper.ReplaceBookmark("抽检设备", list[i].UPDATEVALUE.ToString());
                    }
                    if (list[i].KEY == ((int)SummarizeTotalEnum.SummarizeTotalEnumType.investigateInterference).ToString())
                    {
                        helper.ReplaceBookmark("查处干扰", list[i].UPDATEVALUE.ToString());
                    }
                    if (list[i].KEY == ((int)SummarizeTotalEnum.SummarizeTotalEnumType.approveRadioTotal).ToString())
                    {
                        helper.ReplaceBookmark("审批无线电台总数", list[i].UPDATEVALUE.ToString());
                    }
                }
            }
            else
            {
                MessageBox.Show("没有统计数据，请统计之后再生成报告。", "消息提示", MessageBoxButton.OK);
                return;
            }

            string savePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "DOC\\";
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            string filePath = savePath + DateTime.Now.ToString("yyyyMMddhhmmss") + "活动总结.doc";
            helper.SaveAs(filePath);
            //helper.Close();

            if (UpLodeEnclouseFolder(filePath) == true)
            {
                currentsummarize.GUID = CO_IA.Client.Utility.NewGuid();
                currentsummarize.ACTIVITY_GUID = activityId;//活动id
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivitySummarize.I_CO_IA_SummarizeDoc>
                       (channel =>
                       {
                           bool monitorResult = channel.SaveSummarizeDoc(currentsummarize);
                           if (monitorResult == true)
                           {
                               MessageBox.Show("保存成功！");
                               this.Close();
                           }
                           else
                           {
                               MessageBox.Show("保存失败!");
                           }
                       });
            }
        }

        private string downLoadTemplate(string guid)
        {
            try
            {
                STTemplate sTTemplateFile = new STTemplate();
                BeOperationInvoker.Invoke<I_CO_IA_ActivitySummarize>(channel =>
                {
                    sTTemplateFile = channel.GetSummarizeTemplateFile(guid);
                });

                byte[] file = sTTemplateFile.FILEDOC;
                string docstr = sTTemplateFile.NAME.Substring(sTTemplateFile.NAME.LastIndexOf('.'));
                string serviceDocPath = ByteConvertDocService(file, sTTemplateFile.NAME);
                WebClient webClient = new WebClient();
                //SaveFileDialog dlg = new SaveFileDialog();
                //dlg.FileName = sTTemplateFile.NAME;
                //dlg.Reset();
                //if (docstr == ".doc" || docstr == ".docx")
                //{
                //    dlg.Filter += "Word 文件|*.doc;*.docx";
                //}
                //else if (docstr == ".xls" || docstr == ".xlsx")
                //{
                //    dlg.Filter += "Excel 文件|*.xls;*.xlsx";
                //}
                // dlg.Filter = "Office Files|*.doc;*.docx;*.xls;*.xlsx";

                webClient.DownloadFile(serviceDocPath, sTTemplateFile.NAME);
                //if (dlg.ShowDialog() == true)
                //{
                //    try
                //    {
                //        webClient.DownloadFile(serviceDocPath, dlg.FileName);
                //    }
                //    catch (Exception)
                //    {
                //        return "";
                //    }
                //}
                return serviceDocPath;
            }
            catch (Exception ex)
            {
                return "";
                //MessageBox.Show(ex.GetExceptionMessage());
            }
        }
        /// <summary>
        /// 二进制数据转换为word文件(缓存到服务器)
        /// </summary>
        /// <param name="data">二进制数据</param>
        /// <param name="fileName">word文件名</param>
        /// <returns>文件保存的相对路径</returns>
        public string ByteConvertDocService(byte[] data, string fileName)
        {
            FileStream fs;
            //string savePath = GetPath();
            string savePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Template\\summarize\\";
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            savePath += fileName;
            if (System.IO.File.Exists(savePath))
            {
                //文件已经存在
                fs = new FileStream(savePath, FileMode.Truncate);
            }
            else
            {
                //先删除文件
                System.IO.File.Delete(savePath);
                //重新创建
                fs = new FileStream(savePath, FileMode.CreateNew);
            }
            BinaryWriter br = new BinaryWriter(fs);
            br.Write(data, 0, data.Length);
            br.Flush();
            br.Close();
            fs.Close();
            return savePath;
        }
        /// <summary>
        /// 上传功能
        /// </summary>
        /// <param name="ServerPath"></param>
        /// <param name="EnclouseFolderPath"></param>
        /// <returns></returns>
        private bool UpLodeEnclouseFolder(string ServerPath)
        {
            bool isresult = false;
            string SereverAndEnclousePath = "";
            //获取文件名
            string fileName = ServerPath.Substring(ServerPath.LastIndexOf("\\") + 1);
            string path = this.GetPath();
            SereverAndEnclousePath = System.IO.Path.Combine(path, fileName);
            FileStream fs = null;
            try
            {
                WebClient web = new WebClient();
                web.Credentials = CredentialCache.DefaultCredentials;
                //初始化上传文件  打开读取
                fs = new FileStream(ServerPath, FileMode.Open, FileAccess.Read);
                if (fs.Length / 1024 / 1024 > 20)
                {
                    MessageBox.Show("上传附件不支持超过20M大小的文件。");
                    isresult = false;
                }
                else
                {
                    BinaryReader br = new BinaryReader(fs);
                    byte[] btArray = br.ReadBytes((int)fs.Length);
                    Stream uplodeStream = web.OpenWrite(SereverAndEnclousePath, "PUT");
                    if (uplodeStream.CanWrite)
                    {
                        uplodeStream.Write(btArray, 0, btArray.Length);
                        uplodeStream.Flush();
                        uplodeStream.Close();
                        //将文件以二进制形式存储到数据库中
                        currentsummarize.FILEPATH = btArray;
                        currentsummarize.FILENAME = fileName.Substring(0, fileName.LastIndexOf(".")).ToString();
                        //显示文件格式
                        currentsummarize.FILEFORM = fileName;
                        currentsummarize.FILESIZE = (fs.Length).ToString();
                        currentsummarize.FILETYPE = "";
                        currentsummarize.FILEDOC = btArray;
                        isresult = true;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                fs.Close();//上传成功后，关闭流
            }

            return isresult;
        }
        /// <summary>
        /// 项目所在路径
        /// </summary>
        /// <returns></returns>
        public string GetPath()
        {
            string savePath = @"SysDoc\" + FormatNowTime(2) + @"\";
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, savePath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }
        /// <summary>
        /// 格式化当前时间: 
        /// 1:yyMMddHHmmss; 2:yyyy-MM\dd\
        /// </summary>
        /// <returns></returns>
        public string FormatNowTime(int num)
        {
            if (num == 1)
            {
                return DateTime.Now.ToString("yyMM");
            }
            else if (num == 2)
            {
                return DateTime.Now.ToString("yyyy-MM");
            }
            return "";
        }

        private void btnTemplate_Click(object sender, RoutedEventArgs e)
        {
            SummarizeTotalTemplate summarizeTotalTemplate = new SummarizeTotalTemplate();
            summarizeTotalTemplate.ShowDialog();
            InitTemplate();
        }

        private void InitTemplate()
        {
            List<ItemsData> listID = new List<ItemsData>();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            List<STTemplate> list = new List<STTemplate>();
            BeOperationInvoker.Invoke<I_CO_IA_ActivitySummarize>(channel =>
            {
                list = channel.GetSummarizeTemplateAllName();
            });
            if (list != null)
            {
                foreach (var template in list)
                {
                    ItemsData itemsData = new ItemsData();
                    itemsData.Key = template.GUID;
                    itemsData.Value = template.NAME;
                    listID.Add(itemsData);
                    //dic.Add(template.GUID, template.NAME);
                }
            }
            combTemplate.ItemsSource = listID;
        }
    }
    public class ItemsData
    {
        public string Key
        {
            get;
            set;
        }
        public string Value
        {
            get;
            set;
        }
    }
}
