using CO_IA.Data;
using I_GS_MapBase.Portal;
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

namespace CO_IA.UI.ActivityManage
{
    /// <summary>
    /// Location.xaml 的交互逻辑
    /// </summary>
    public partial class LocationPoint : UserControl, IFrameworkElement
    {
        private ActivityPlaceLocation locationInfo;

        public ActivityPlaceLocation LocationInfo
        {
            get
            {
                return locationInfo;
            }
            set
            {
                locationInfo = value;
                this.ToolTip = locationInfo.LocationName;
            }
        }
        public string ElementId
        {
            get { return "location_" + locationInfo.GUID; }
        }
        public event Action<ActivityPlaceLocation> ShowPlaceImageEvent;
        public LocationPoint()
        {
            InitializeComponent();
        }
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (ShowPlaceImageEvent != null)
            {
                ShowPlaceImageEvent(LocationInfo);
            }
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            showBorder.BorderThickness = new Thickness(1);
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            showBorder.BorderThickness = new Thickness(0);
        }
    }
}
