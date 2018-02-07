using CO_IA.Data;
using I_GS_MapBase.Portal.Types;
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

namespace CO_IA.UI.Screen.Areas
{
    /// <summary>
    /// AreaInfo.xaml 的交互逻辑
    /// </summary>
    public partial class AreaInfo : UserControl
    {
        public AreaInfo()
        {
            InitializeComponent();
        }
        public void IniData(ActivityPlaceInfo placeinfo)
        {
            this.DataContext = placeinfo; 
        }
        //private void LocationFix(object sender, RoutedEventArgs e)
        //{
        //    ActivityPlaceLocation delLocation = dg_LocationList.SelectedItem as ActivityPlaceLocation;
        //    if (delLocation != null)
        //    {
        //        Obj.screenMap.setExtent(new AT_BC.Data.GeoPoint() { Longitude = delLocation.LocationLG, Latitude = delLocation.LocationLA });
        //    }
        //}
        private void DataGridRow_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            ActivityPlaceLocation delLocation = dg_LocationList.SelectedItem as ActivityPlaceLocation;
            if (delLocation != null)
            {
                MapPointEx p = Obj.screenMap.MainMap.MapPointFactory.Create(delLocation.LocationLG, delLocation.LocationLA);
                Obj.screenMap.setExtent(p);
                Obj.screenMap.SelectPointShine(p);
            }
        }
    }
}
