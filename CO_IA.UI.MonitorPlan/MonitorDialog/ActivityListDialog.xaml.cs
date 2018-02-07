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
using System.Windows.Shapes;
using CO_IA.Data;

namespace CO_IA.UI.MonitorPlan.MonitorDialog
{
    /// <summary>
    /// ActivityListDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ActivityListDialog : Window
    {
        private ActivityPlaceInfo _activityPlace = new ActivityPlaceInfo();
        public event Action<ActivityPlaceLocation> LocationSelectionChanged;
        public ActivityListDialog(ActivityPlaceInfo p_activityPlace)
        {
            InitializeComponent();
            _activityPlace = p_activityPlace;
            //获取对应地点的活动位置信息
            ActivityPlaceLocation[] getLocations = _activityPlace.Locations;
            listPlace.ItemsSource = getLocations.ToList();
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
            ActivityPlaceLocation _placeInfo = item.DataContext as ActivityPlaceLocation;
            listPlace.SelectedItem = _placeInfo;
            LocationSelectionChanged(_placeInfo);
        }

        private void listPlace_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            LocationSelectionChanged(listPlace.SelectedItem as ActivityPlaceLocation);
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            ActivityPlaceLocation _placeinfo = listPlace.SelectedItem as ActivityPlaceLocation;
            LocationSelectionChanged(_placeinfo);
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        
    }
}
