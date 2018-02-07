using CO_IA_Data;
using I_CO_IA.StationManage;
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

namespace CO_IA.UI.StationManage
{
    /// <summary>
    /// StationListControl.xaml 的交互逻辑
    /// </summary>
    public partial class StationListControl : UserControl
    {
        public StationListControl()
        {
            InitializeComponent();
        }
        public StationListControl(List<QUERY_PARAMS> lsParams)
        {
            InitializeComponent();
            List<StationInfo> statinfoLs = new List<StationInfo>();
            if (lsParams.Count > 0)
            {
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_StationManage>(channel =>
                {
                    statinfoLs = channel.GetStationByParams(lsParams);
                    stationdatagrid.ItemsSource = statinfoLs;
                });
            }
        }
        public List<StationInfo> StationItemsSource
        {
            get
            {
                if (stationdatagrid.ItemsSource == null)
                {
                    return null;
                }
                else
                {
                    return stationdatagrid.ItemsSource as List<StationInfo>;
                }
            }
            set { stationdatagrid.ItemsSource = value; }
        }

        private void Stationdatagrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            StationInfo selectstation = this.stationdatagrid.SelectedItem as StationInfo;
            StationDetailDialog dialog = new StationDetailDialog(selectstation.STATGUID);
            dialog.ShowDialog(this);
        }

        #region  全选/全取消事件  取消该功能
        private void chkAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            bool ischecked = chk.IsChecked.Value;

            foreach (StationInfo item in StationItemsSource)
            {
                item.IsChecked = ischecked;
            }
        }

        private void chkAll_Loaded(object sender, RoutedEventArgs e)
        {
            CheckBox chkAll = sender as CheckBox;
            if (this.StationItemsSource != null)
            {
                chkAll.IsChecked = StationItemsSource.Any(item => item.IsChecked);
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

        }

        #endregion
    }
}
