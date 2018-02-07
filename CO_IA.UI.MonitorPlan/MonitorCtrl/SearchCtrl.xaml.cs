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
using CO_IA.Data;

namespace CO_IA.UI.MonitorPlan.MonitorCtrl
{
    
    /// <summary>
    /// SearchCtrl.xaml 的交互逻辑
    /// </summary>
    public partial class SearchCtrl : UserControl
    {
        /// <summary>
        /// 活动的主体
        /// </summary>
        private string activityId;
        public event Action<ActivityPlaceInfo> AddressSelectionChanged;

        public SearchCtrl()
        {
            InitializeComponent();
            activityId = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid;
            ActivityPlaceInfo[] getActivityPlace = PrototypeDatas.GetPlacesByActivityId(activityId);
            listPlace.ItemsSource = getActivityPlace.ToList();
            listPlace.SelectedIndex = 0;
        }

        /// <summary>
        /// 列表中的项被点击时的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //当列表中的项被点击时，设置列表选中这行数据
            var item = e.Source as ListBoxItem;
            ActivityPlaceInfo _placeInfo = item.DataContext as ActivityPlaceInfo;
            listPlace.SelectedItem = _placeInfo;
            AddressSelectionChanged(_placeInfo);
        }
 
    }

}
