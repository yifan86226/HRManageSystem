using CO_IA.Data;
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
using System.Windows.Shapes;

namespace CO_IA.UI.FreqPlan.FreqPlan
{
    /// <summary>
    /// QueryEmeClear_Dialog.xaml 的交互逻辑
    /// </summary>
    public partial class QueryEmeClear_Dialog : Window
    {
        private CO_IA.Data.ActivityPlaceInfo activityPlaceInfo;

        //public QueryEmeClear_Dialog()
        //{
        //    InitializeComponent();

        //    EMEEnvironmentSource = CreateEMEEnvironmentSource();
        //}

        public List<EmeClearInfo> EmeClearInfoList
        {

            get { return xEMEClearGrid.ItemsSource as List<EmeClearInfo>; }
            set { xEMEClearGrid.ItemsSource = null; xEMEClearGrid.ItemsSource = value; }
        }

        private EmeClearQueryCondition condition = new EmeClearQueryCondition();


        public QueryEmeClear_Dialog(CO_IA.Data.ActivityPlaceInfo activityPlaceInfo)
        {
            InitializeComponent();


            this.activityPlaceInfo = activityPlaceInfo;


            condition.PlaceGuid = activityPlaceInfo.Guid;
            condition.NeedClear = "0";
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan>(channel =>
            {
                EmeClearInfoList = channel.GetEmeClearHandleInfoList(condition);
            });


        }

        //private CheckBox _isAllCheckBox;
        private void ckbSelectedAll_Loaded(object sender, RoutedEventArgs e)
        {
            //chkAll = (CheckBox)sender;
            //if (this.EMEEnvironmentSource != null)
            //{
            //    chkAll.IsChecked = EMEEnvironmentSource.Any(item => item.IsSelected);
            //}
        }

        private void ckbSelectedAll_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            bool ischecked = chk.IsChecked.Value;

            foreach (EmeClearInfo item in EmeClearInfoList)
            {
                item.IsSelected = ischecked;
            }
        }

        private void chkCell_Click(object sender, RoutedEventArgs e)
        {
            //bool? isChecked = (sender as CheckBox).IsChecked;
            //if (!isChecked.HasValue)
            //{
            //    return;
            //}
            //bool checkedState = isChecked.Value;

            //foreach (EMEEnvironment result in EMEEnvironmentSource)
            //{
            //    if (result.IsSelected != checkedState)
            //    {
            //        chkAll.IsChecked = null;
            //        return;
            //    }
            //}
            //chkAll.IsChecked = checkedState;
        }
        private void xbtnOk_Click(object sender, RoutedEventArgs e)
        {
            List<string> guidList = new List<string>();
            foreach (EmeClearInfo eme in EmeClearInfoList)
            {

                if (eme.IsSelected == true)
                {
                    guidList.Add(eme.GUID);
                }

            }

            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan>(channel =>
            {
                channel.UpdateEmeClearHandleInfoList(guidList);
            });


            this.Close();
        }

        private void xbtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
