using CO_IA.Client;
using CO_IA.Data;
using CO_IA.UI.FreqStation.StationPlan;
using System;
using System.Collections.Generic;
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
    /// StationPlanModule.xaml 的交互逻辑
    /// </summary>
    public partial class StationPlanModule : UserControl
    {
        private static readonly Types.FreqPlanningStep[] disposalSteps = new Types.FreqPlanningStep[]{
                 Types.FreqPlanningStep.FreqAssign,Types.FreqPlanningStep.EquipmentInspection};

        private string CurrentPlaceGuid;
        public StationPlanModule()
        {
            InitializeComponent();
            spSite.Visibility = Visibility.Visible;
            var places = Utility.GetPlacesByActivityId(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
            xComboboxSite.ItemsSource = places;
            this.xComboboxSite.SelectedIndex = places.Length > 0 ? 0 : -1;
            listBoxStationPlanningStep.SelectedIndex = 0;
        }


        private void listBoxStationPlanningStep_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems == null || e.AddedItems.Count == 0)
            {
                this.borderContent.Visibility = System.Windows.Visibility.Collapsed;
                return;
            }
            this.borderContent.Child = null;
            this.borderContent.Visibility = System.Windows.Visibility.Visible;
            var stepState = e.AddedItems[0] as FreqPlanningStepState;
            if (stepState != null)
            {
                ActivityPlaceInfo selectPlace = (ActivityPlaceInfo)xComboboxSite.SelectedItem;

                switch (stepState.Step)
                {
                    case Types.FreqPlanningStep.FreqAssign:
                        FreqAssignManage freqassign = new FreqAssignManage(selectPlace);
                        this.borderContent.Child = freqassign;
                        break;
                    case Types.FreqPlanningStep.EquipmentInspection:

                        EquipmentInspectionManageControl equInspection = new EquipmentInspectionManageControl(selectPlace);
                        this.borderContent.Child = equInspection;
                        break;
                    default:
                        this.borderContent.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                }
            }


            //this.borderContent.Child = null;
            //this.borderContent.Visibility = System.Windows.Visibility.Visible;
            //switch (listBoxStationPlanningStep.SelectedIndex)
            //{
            //    case 0:
            //        FreqAssignHandleControl freqAssignHandle = new FreqAssignHandleControl();
            //        freqAssignHandle.PlaceGuid = this.CurrentPlaceGuid;
            //        this.borderContent.Child = freqAssignHandle;
            //        break;
            //    case 1:
            //        EquCheckControl equCheckControl = new EquCheckControl();
            //        equCheckControl.PlaceGuid = this.CurrentPlaceGuid;
            //        this.borderContent.Child = equCheckControl;
            //        break;
            //}
        }

        private void xComboboxSite_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.CurrentPlaceGuid = this.xComboboxSite.SelectedValue.ToString();
            //if (e.AddedItems.Count == 0)
            //{
            //    return;
            //}
            //ActivityPlace place = e.AddedItems[0] as ActivityPlace;
            //if (place == null)
            //{
            //    return;
            //}

            var stepState = this.listBoxStationPlanningStep.SelectedValue as FreqPlanningStepState;
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivityManage.I_CO_IA_ActivityManage>(p =>
            {
                var stepStates = p.GetStepStates(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid, this.CurrentPlaceGuid);
                stepStates = (from step in stepStates where disposalSteps.Contains(step.Step) select step).OrderBy(step => step.Order).ToArray();

                this.listBoxStationPlanningStep.ItemsSource = null;
                this.listBoxStationPlanningStep.ItemsSource = stepStates;
                //this.listBoxFreqPlanningStep.SelectedIndex = -1;
                if (stepState != null)
                {
                    foreach (var state in stepStates)
                    {
                        if (state.Step == stepState.Step)
                        {
                            this.listBoxStationPlanningStep.SelectedValue = state;
                            return;
                        }
                    }
                }
                if (stepStates.Length > 0)
                {
                    this.listBoxStationPlanningStep.SelectedIndex = 0;
                    return;
                }
                this.listBoxStationPlanningStep.SelectedIndex = -1;
            });

            //this.CurrentPlaceGuid = this.xComboboxSite.SelectedValue.ToString();
            //if (this.borderContent.Child != null)
            //{
            //    UserControl control = borderContent.Child as UserControl;
            //    if (control.GetType() == typeof(FreqAssignHandleControl))
            //    {
            //        (borderContent.Child as FreqAssignHandleControl).PlaceGuid = CurrentPlaceGuid;
            //    }
            //    else if (control.GetType() == typeof(EquCheckControl))
            //    {
            //        (borderContent.Child as EquCheckControl).PlaceGuid = CurrentPlaceGuid;
            //    }
            //}

        }

        //private List<RoundStationInfo> QueryRoundStations(string Placeguid)
        //{

        //    return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqPlan, List<RoundStationInfo>>(channel =>
        //    {
        //        return channel.QueryRoundStationsByPlace(Placeguid);
        //    });

        //}
 

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
