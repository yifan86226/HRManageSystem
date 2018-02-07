using CO_IA.Data;
using CO_IA.Types;
using CO_IA.UI.FreqStation.FreqPlan;
using CO_IA.UI.FreqStation.FreqPlan.SurroundStation;
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


namespace CO_IA.UI.FreqStation
{
    /// <summary>
    /// FreqPlanControl.xaml 的交互逻辑
    /// </summary>
    public partial class FreqPlanModule : UserControl
    {
        private static readonly FreqPlanningStep[] disposalSteps = new FreqPlanningStep[]{
            FreqPlanningStep.BusinessPlanning, 
            FreqPlanningStep.StationPlanning,
            FreqPlanningStep.SurroundStationAnalyse,
            FreqPlanningStep.EMEAnalyse,
            FreqPlanningStep.EMEClear};

        //private MapLoad_Control _mapLoadControl = new MapLoad_Control();
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
            CO_IA.Data.ActivityPlaceInfo[] activityPlaces = CO_IA.Client.Utility.GetPlacesByActivityId(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
            xComboboxSite.ItemsSource = activityPlaces;
            xComboboxSite.DisplayMemberPath = "Name";
            xComboboxSite.SelectedValuePath = "Guid";
            if (activityPlaces.Length > 0)
                this.xComboboxSite.SelectedIndex = 0;
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

        private void xComboboxSite_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.borderContent.Visibility = System.Windows.Visibility.Collapsed;
            if (e.AddedItems.Count == 0)
            {
                return;
            }
            ActivityPlace place = e.AddedItems[0] as ActivityPlace;
            if (place == null)
            {
                return;
            }

            var stepState = this.listBoxFreqPlanningStep.SelectedValue as FreqPlanningStepState;
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivityManage.I_CO_IA_ActivityManage>(p =>
            {
                var stepStates = p.GetStepStates(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid, place.Guid);
                stepStates = (from step in stepStates where disposalSteps.Contains(step.Step) select step).OrderBy(step => step.Order).ToArray();

                this.listBoxFreqPlanningStep.ItemsSource = null;
                this.listBoxFreqPlanningStep.ItemsSource = stepStates;
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
                ActivityPlaceInfo selectPlace = (ActivityPlaceInfo)xComboboxSite.SelectedItem;
                switch (stepState.Step)
                {
                    case Types.FreqPlanningStep.BusinessPlanning: //频率保障方案
                        FreqPartPlanShow(selectPlace);
                        break;
                    case Types.FreqPlanningStep.StationPlanning: //单位设备管理
                        ORGAndEquipmentManage EquipmentListControl = new ORGAndEquipmentManage(selectPlace);
                        this.borderContent.Child = EquipmentListControl;
                        this.borderContent.Visibility = Visibility.Visible;
                        break;
                    case Types.FreqPlanningStep.SurroundStationAnalyse: //周围台站分析
                        //RoundStatAnalyse_Control SurroundStationAnalyseControl = new RoundStatAnalyse_Control();
                        //this.borderContent.Child = SurroundStationAnalyseControl;
                        //this.borderContent.Visibility = Visibility.Visible;
                        SurroundStationAnalyseManage SurroundStationAnalyseControl = new SurroundStationAnalyseManage(selectPlace);
                        this.borderContent.Child = SurroundStationAnalyseControl;
                        this.borderContent.Visibility = Visibility.Visible;
                        break;
                    //RoundStatAnalyseShow();
                    case Types.FreqPlanningStep.EMEAnalyse:
                        this.borderContent.Child = new EmeMonitorAnalyse_Control((ActivityPlaceInfo)xComboboxSite.SelectedItem);
                        this.borderContent.Visibility = System.Windows.Visibility.Visible;
                        break;
                    case Types.FreqPlanningStep.EMEClear:
                        this.ClearStationShow(selectPlace);
                        break;
                    default:
                        this.borderContent.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                }
            }
        }

        //频率规划
        private void FreqPartPlanShow(ActivityPlaceInfo placeInfo)
        {
            if (placeInfo != null)
            {
                PLaceFreqPlanControl control = new PLaceFreqPlanControl();
                control.DataContext = placeInfo;
                //PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqStation.I_CO_IA_FreqStation>(channel =>
                //    {
                //        var dataSource = channel.GetPlaceFreqPlans(placeInfo.Guid);
                //        if (dataSource != null && dataSource.Length > 0)
                //        {
                //            control.DataContext = new ObservableCollection<PlaceFreqPlan>(dataSource);
                //        }
                //        else
                //        {
                //            control.DataContext = new ObservableCollection<PlaceFreqPlan>();
                //        }
                //    });

                this.borderContent.Child = control;
                this.borderContent.Visibility = System.Windows.Visibility.Visible;
            }
            //_mapLoadControl.UpSubControl = new FreqPartPlan_Control();
            //this.borderContent.Child = _mapLoadControl;
            //this.borderContent.Visibility = System.Windows.Visibility.Visible;
        }

        private void ClearStationShow(ActivityPlaceInfo placeInfo)
        {
            if (placeInfo != null)
            {
                SurroundStationClearResultControl control = new SurroundStationClearResultControl();
                control.DataContext = placeInfo;
                this.borderContent.Child = control;
                this.borderContent.Visibility = System.Windows.Visibility.Visible;
            }
        }        
        //周围台站分析
        private void RoundStatAnalyseShow()
        {
            //_mapLoadControl.UpSubControl = new RoundStatAnalyse_Control();
            //this.borderContent.Child = _mapLoadControl;
            //this.borderContent.Visibility = System.Windows.Visibility.Visible;
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
