using Best.VWPlatform.Common.StationManagement;
using Best.VWPlatform.Common.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Best.VWPlatform.Controls
{
    /// <summary>
    /// GeneralStationTooltipWindow.xaml 的交互逻辑
    /// </summary>
    public partial class GeneralStationTooltipWindow : UserControl
    {
        public GeneralStationTooltipWindow()
        {
            InitializeComponent();
        }

        public void DoFocus()
        {
            ((Storyboard)this.FindResource("x_storyboard")).Begin();
        }

        public void DoLostFocus()
        {
            ((Storyboard)this.FindResource("x_storyboard")).Stop();
        }
    }
}
