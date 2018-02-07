using CO_IA.Data;

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

namespace CO_IA.Client
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
            set;
            get;
        }
        public object ElementTag
        {
            get;
            set;
        }
        public event Action<ActivityPlaceLocation> ShowPlaceImageEvent;
        public LocationPoint(bool showImage)
        {
            InitializeComponent();
            if (showImage)
                imgshow.Visibility = System.Windows.Visibility.Visible;
            else
                imgshow.Visibility = System.Windows.Visibility.Collapsed;
        }
        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = false;
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
