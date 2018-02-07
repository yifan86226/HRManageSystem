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
    /// ProcessingCtl.xaml 的交互逻辑
    /// </summary>
    public partial class ProcessingCtl : UserControl
    {
        public ProcessingCtl()
        {
            InitializeComponent();
            this.Loaded += ProcessingCtl_Loaded;
        }

        void ProcessingCtl_Loaded(object sender, RoutedEventArgs e)
        {
            ((Storyboard)this.FindResource("StoryboardPoint")).Begin();
        }
        public void BeginStoryboard()
        {
            ((Storyboard)this.FindResource("StoryboardPoint")).Begin();
            this.Visibility = Visibility.Visible;
        }

        public void StopStoryboard()
        {
            ((Storyboard)this.FindResource("StoryboardPoint")).Stop();
            this.Visibility = Visibility.Collapsed;
        }
    }
}
