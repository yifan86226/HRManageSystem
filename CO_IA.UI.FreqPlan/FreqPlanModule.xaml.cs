using CO_IA.Data;
using CO_IA.UI.FreqPlan.FreqPlan;
using CO_IA.UI.FreqPlan.StationPlan;
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


namespace CO_IA.UI.FreqPlan
{
    /// <summary>
    /// FreqPlanControl.xaml 的交互逻辑
    /// </summary>
    public partial class FreqPlanModule : UserControl
    {
        private static readonly Types.FreqPlanningStep[] disposalSteps = new Types.FreqPlanningStep[]{
                 Types.FreqPlanningStep.BusinessPlanning,Types.FreqPlanningStep.StationPlanning,Types.FreqPlanningStep.SurroundStationAnalyse,
                 Types.FreqPlanningStep.EMEAnalyse,Types.FreqPlanningStep.EMEClear};

        private MapLoad_Control _mapLoadControl =  new MapLoad_Control();
        public FreqPlanModule()
        {
            InitializeComponent();
            InitActivityPlace();
            //this.listBoxFreqPlanningStep.SelectedIndex = 0;
        }
        /// <summary>
        /// 现场端
        /// </summary>
        /// <param name="p_activity"></param>
        /// <param name="p_place"></param>
        public FreqPlanModule(Activity p_activity, ActivityPlace p_place)
        {
            InitializeComponent();
        }
        private void InitActivityPlace()
        {
            CO_IA.Data.ActivityPlaceInfo[] activityPlaces = GetPlacesByActivityId();
            xComboboxSite.ItemsSource = activityPlaces;
            xComboboxSite.DisplayMemberPath = "Name";
            xComboboxSite.SelectedValuePath = "Guid";
            if (activityPlaces.Length >0)
                this.xComboboxSite.SelectedIndex = 0; 
        }

        private int getint<T>(T pp) where T:struct
        {
            return Convert.ToInt32(pp);
        }
        /// <summary>
        /// 获取活动地点列表
        /// </summary>
        /// <param name="_activityGuid">活动guid</param>
        /// <returns></returns>
        private static CO_IA.Data.ActivityPlaceInfo[] GetPlacesByActivityId()
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivityManage.I_CO_IA_ActivityManage, CO_IA.Data.ActivityPlaceInfo[]>(
                channel =>
                {
                    return channel.GetPlaceInfosByActivityId(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
                });
         }

        public string Title
        {
            get 
            {
                return "频率规划";
            }
        }

        public string Guid
        {
            get 
            {
                return "FreqPlanning";
            }
        }

        public void Active()
        {
        }

        private void listBoxFreqPlanningStep_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems == null || e.AddedItems.Count == 0)
            {
                this.borderContent.Visibility = System.Windows.Visibility.Collapsed;
                return;
            }
            var stepState = e.AddedItems[0] as FreqPlanningStepState;
            if (stepState != null)
            {
                switch (stepState.Step)
                {
                    case Types.FreqPlanningStep.BusinessPlanning:
                        FreqPartPlanShow();
                        break;
                    case Types.FreqPlanningStep.StationPlanning:
                        EquipmentListControl EquipmentListControl = new UI.FreqPlan.EquipmentListControl((ActivityPlaceInfo)xComboboxSite.SelectedItem);
                        //EquipmentListControl.SelectedPlaceIndex = xComboboxSite.SelectedIndex - 1;
                        this.borderContent.Child = EquipmentListControl;
                        this.borderContent.Visibility = System.Windows.Visibility.Visible;
                        break;
                    case Types.FreqPlanningStep.SurroundStationAnalyse:
                        RoundStatAnalyseShow();
                        break;
                    case Types.FreqPlanningStep.EMEAnalyse:
                        this.borderContent.Child = new EmeMonitorAnalyse_Control((ActivityPlaceInfo)xComboboxSite.SelectedItem);
                        this.borderContent.Visibility = System.Windows.Visibility.Visible;
                        break;
                    case Types.FreqPlanningStep.EMEClear:
                        this.borderContent.Child = new EmeClearHandle_Control((ActivityPlaceInfo)xComboboxSite.SelectedItem);
                        this.borderContent.Visibility = System.Windows.Visibility.Visible;
                        break;
                    default:
                        this.borderContent.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                }
            }
        }
        //频率规划
        private void FreqPartPlanShow()
        {
            _mapLoadControl.UpSubControl = new FreqPartPlan_Control();
            this.borderContent.Child = _mapLoadControl;
            this.borderContent.Visibility = System.Windows.Visibility.Visible;
        }
        //周围台站分析
        private void RoundStatAnalyseShow()
        {
            _mapLoadControl.UpSubControl = new RoundStatAnalyse_Control();
            this.borderContent.Child = _mapLoadControl;
            this.borderContent.Visibility = System.Windows.Visibility.Visible;
        }
        private void xComboboxSite_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count ==0)
            {
                return;
            }
            ActivityPlace place = e.AddedItems[0] as ActivityPlace;
            if (place==null)
            {
                return;
            }
            _mapLoadControl.LoadActivityPlace = (CO_IA.Data.ActivityPlaceInfo)xComboboxSite.SelectedItem;
            var stepState = this.listBoxFreqPlanningStep.SelectedValue as FreqPlanningStepState;
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivityManage.I_CO_IA_ActivityManage>(p=>
            {
                var stepStates = p.GetStepStates(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid, place.Guid);
                stepStates = (from step in stepStates where disposalSteps.Contains(step.Step) select step).OrderBy(step=>step.Order).ToArray();

                this.listBoxFreqPlanningStep.ItemsSource = null;
                this.listBoxFreqPlanningStep.ItemsSource = stepStates;
                //this.listBoxFreqPlanningStep.SelectedIndex = -1;
                if (stepState != null)
                {
                    foreach (var state in stepStates)
                    {
                        if (state.Step == stepState.Step)
                        {
                            this.listBoxFreqPlanningStep.SelectedValue = state;
                            return;
                        }
                    }
                }
                if (stepStates.Length > 0)
                {
                    this.listBoxFreqPlanningStep.SelectedIndex = 0;
                    return;
                }
                this.listBoxFreqPlanningStep.SelectedIndex = -1;
            });
        }

        private void ListBoxItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
#warning 在这里判断节点状态,如果没有开始则为true,否则为false
            //判断当前选中项是否为已经完成或正在进行步骤,如果不是将取消对当前项目的选择
            bool noStart = false;
            if (noStart)
            {
                e.Handled = true;
            }
        }

        private void buttonCompleteStep_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("确实要完成该环节吗", "完成提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var stepState = (sender as Image).DataContext as FreqPlanningStepState;
                if (stepState != null)
                {
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivityManage.I_CO_IA_ActivityManage>
                        (channel =>
                            {
                                channel.CompleteFreqPlanningStep(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid, xComboboxSite.SelectedValue as string, stepState.Step);
                            });
                    stepState.IsCompleted = true;
                }
            }
        }

    }
}
