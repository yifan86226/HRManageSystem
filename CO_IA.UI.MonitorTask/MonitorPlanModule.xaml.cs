using CO_IA.Client;
using CO_IA.Data;
using CO_IA.UI.Setting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AT_BC.Data;
using AT_BC.Common;
using CO_IA.Types;

namespace CO_IA.UI.MonitorTask
{
    /// <summary>
    /// MonitorPlanModule.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorPlanModule : UserControl
    {
        private CheckBox checkBoxCheckedAll;
        public MonitorPlanModule()
        {
            InitializeComponent();
            //this.DataContextChanged += MonitorPlanModule_DataContextChanged;
            this.Loaded += MonitorPlanModule_Loaded;
        }

        private void MonitorPlanModule_Loaded(object sender, RoutedEventArgs e)
        {
            this.listBoxPlace.ItemsSource = Utility.GetPlacesByActivityId(RiasPortal.ModuleContainer.Activity.Guid);
            if (RiasPortal.ModuleContainer.Activity.ActivityType == ActivityTypeDef.MajorDisturbance || RiasPortal.ModuleContainer.Activity.ActivityType == ActivityTypeDef.Exam)
            {
                this.buttonLoad.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void listBoxPlace_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.dataGridMonitorPlan.ItemsSource = null;
            if (e.AddedItems.Count > 0)
            {
                var placeInfo = e.AddedItems[0] as ActivityPlaceInfo;
                if (placeInfo != null)
                {
                    MonitorPlanInfo[] monitorPlans =
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MonitorTask.I_CO_IA_MonitorTask, MonitorPlanInfo[]>(channel =>
                    {
                        return channel.GetMonitorPlansByPlace(RiasPortal.ModuleContainer.Activity.Guid, placeInfo.Guid);
                    });
                    if (monitorPlans == null || monitorPlans.Length == 0)
                    {
                        this.dataGridMonitorPlan.ItemsSource = new ObservableCollection<MonitorPlanInfo>();
                    }
                    else
                    {
                        this.dataGridMonitorPlan.ItemsSource = new ObservableCollection<MonitorPlanInfo>(monitorPlans);
                    }
                }
            }
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            var existMonitorPlanList=this.dataGridMonitorPlan.ItemsSource as IList<MonitorPlanInfo>;
            var checkedMonitorPlans = existMonitorPlanList.GetCheckedItems();
            if (checkedMonitorPlans.Length > 0)
            {
                if (MessageBox.Show("确实要删除选中的监测频段吗?", "删除提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var checkedMonitorPlanGuids = (from data in checkedMonitorPlans select data.Key).ToArray();
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MonitorTask.I_CO_IA_MonitorTask>(channel =>
                        {
                            channel.DeleteMonitorPlans(checkedMonitorPlanGuids);
                        });
                    foreach (var monitorPlan in checkedMonitorPlans)
                    {
                        existMonitorPlanList.Remove(monitorPlan);
                    }
                }
            }
            else
            {
                MessageBox.Show("没有需要删除的数据");
            }
        }

        private void buttonLoad_Click(object sender, RoutedEventArgs e)
        {
            ActivityPlaceInfo placeInfo = this.listBoxPlace.SelectedValue as ActivityPlaceInfo;
            if (placeInfo==null)
            {
                MessageBox.Show("请选择要生成监测预案的区域");
                return;
            }

            PlaceFreqPlan[] placeFreqPlans = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqStation.I_CO_IA_FreqStation, PlaceFreqPlan[]>(channel =>
                {
                    return channel.GetPlaceFreqPlans(placeInfo.Guid);
                });
            if (placeFreqPlans != null && placeFreqPlans.Length > 0)
            {
                EquipmentClassFreqPlanningSelectWindow wnd = new EquipmentClassFreqPlanningSelectWindow();
                wnd.DataContext = placeFreqPlans;
                if (wnd.ShowDialog(this) == true)
                {
                    List<MonitorPlanInfo> monitorPlanList = new List<MonitorPlanInfo>();
                    PlaceFreqPlan[] selectedFreqPlans = placeFreqPlans.GetCheckedItems();
                    if (selectedFreqPlans.Length > 0)
                    {
                        if (MessageBox.Show("是否合并交叠及相邻频段", "合并提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            Array.Sort(selectedFreqPlans, (one, other) =>
                            {
                                var result = one.MHzFreqFrom.CompareTo(other.MHzFreqFrom); 
                                if (result == 0)
                                {
                                    return other.MHzFreqTo.CompareTo(one.MHzFreqTo);
                                }
                                return result;
                            });
                            MonitorPlanInfo monitorPlan = this.CreateMonitorPlan(selectedFreqPlans[0], placeInfo);
                            monitorPlanList.Add(monitorPlan);
                            for (int i = 1; i < selectedFreqPlans.Length; i++)
                            {
                                if (selectedFreqPlans[i].MHzFreqFrom > monitorPlan.MHzFreqTo)
                                {
                                    monitorPlan = this.CreateMonitorPlan(selectedFreqPlans[i], placeInfo);
                                    monitorPlanList.Add(monitorPlan);
                                }
                                else
                                {
                                    if (selectedFreqPlans[i].MHzFreqTo > monitorPlan.MHzFreqTo)
                                    {
                                        monitorPlan.MHzFreqTo = selectedFreqPlans[i].MHzFreqTo;
                                    }
                                    if (!string.IsNullOrWhiteSpace(monitorPlan.Comments))
                                    {
                                        monitorPlan.Comments = string.Format("{0},{1}", monitorPlan.Comments, selectedFreqPlans[i].Name);
                                    }
                                    else
                                    {
                                        monitorPlan.Comments = selectedFreqPlans[i].Name;
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < selectedFreqPlans.Length; i++)
                            {
                                monitorPlanList.Add(this.CreateMonitorPlan(selectedFreqPlans[i], placeInfo));
                            }
                        }
                        PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MonitorTask.I_CO_IA_MonitorTask>(channel =>
                        {
                            channel.SaveMonitorPlans(monitorPlanList.ToArray());
                        });
                        var locationFreqPlans=this.dataGridMonitorPlan.ItemsSource as ObservableCollection<MonitorPlanInfo>;
                        foreach(var freqPlan in monitorPlanList)
                        {
                            locationFreqPlans.Add(freqPlan);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("该地区没有频率保障方案,无法生成监测预案");
            }
        }

        private MonitorPlanInfo CreateMonitorPlan(EquipmentClassFreqRange freqRange,ActivityPlaceInfo placeInfo)
        {
            MonitorPlanInfo monitorPlan = new MonitorPlanInfo();
            monitorPlan.MHzFreqFrom = freqRange.MHzFreqFrom;
            monitorPlan.MHzFreqTo = freqRange.MHzFreqTo;
            monitorPlan.Key = Utility.NewGuid();
            monitorPlan.LoggingMode = DataLoggingMode.Created;
            monitorPlan.PlaceGuid = placeInfo.Guid;
            monitorPlan.kHzBand = freqRange.kHzBand;
            monitorPlan.ActivityGuid = placeInfo.ActivityGuid;
            monitorPlan.Creator = RiasPortal.Current.UserSetting.UserName;
            monitorPlan.CreateTime = DateTime.Now;
            monitorPlan.Comments = freqRange.Name;
            return monitorPlan;
        }

        private MonitorPlanInfo CreateMonitorPlan(ActivityPlaceInfo placeInfo)
        {
            MonitorPlanInfo monitorPlan = new MonitorPlanInfo();
            monitorPlan.MHzFreqFrom = 0d;
            monitorPlan.MHzFreqTo = 0d;
            monitorPlan.Key = Utility.NewGuid();
            monitorPlan.LoggingMode = DataLoggingMode.ManualEntry;
            monitorPlan.PlaceGuid = placeInfo.Guid;
            monitorPlan.ActivityGuid = placeInfo.ActivityGuid;
            monitorPlan.Creator = RiasPortal.Current.UserSetting.UserName;
            monitorPlan.CreateTime = DateTime.Now;
            monitorPlan.Comments = string.Empty;
            return monitorPlan;
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            ActivityPlaceInfo placeInfo = this.listBoxPlace.SelectedValue as ActivityPlaceInfo;
            if (placeInfo == null)
            {
                MessageBox.Show("请选择要生成监测预案的区域");
                return;
            }
            var monitorPlan = CreateMonitorPlan(placeInfo);
            var wnd = new Monitor.MonitorPlanEditWindow();
            wnd.DataContext = monitorPlan;
            if (wnd.ShowDialog(this) == true)
            {
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MonitorTask.I_CO_IA_MonitorTask>(channel =>
                    {
                        channel.SaveMonitorPlan(monitorPlan);
                    });
                (this.dataGridMonitorPlan.ItemsSource as ObservableCollection<MonitorPlanInfo>).Add(monitorPlan);
            }
        }

        private void checkBoxAll_Loaded(object sender, RoutedEventArgs e)
        {
            this.checkBoxCheckedAll = sender as CheckBox;
        }

        private void dataGridMonitorPlan_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void CheckableDataCheckedCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CheckableCheckBoxHelper.Checked(sender, e, this.checkBoxCheckedAll);
        }

        public void dataGridMonitorPlan_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton== MouseButton.Left)
            {
                DataGridRow dgr = DataGridRow.GetRowContainingElement(e.OriginalSource as FrameworkElement);
                if (dgr != null)
                {
                    MonitorPlanInfo monitorPlan = dgr.DataContext as MonitorPlanInfo;
                    if (monitorPlan != null)
                    {
                        var wnd = new Monitor.MonitorPlanEditWindow();
                        wnd.DataContext = monitorPlan;
                        if (wnd.ShowDialog(this) == true)
                        {
                            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MonitorTask.I_CO_IA_MonitorTask>(channel =>
                                {
                                    channel.SaveMonitorPlan(monitorPlan);
                                });
                        }
                        //this.OpenTemplateManageModule(activity);
                    }
                }
            }
        }
    }
}
