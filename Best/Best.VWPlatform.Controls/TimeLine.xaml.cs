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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Best.VWPlatform.Controls
{
    /// <summary>
    /// TimeLine.xaml 的交互逻辑
    /// </summary>
    public partial class TimeLine : UserControl
    {
        public event Action<HisThumbnailItem> ListBoxSelectedHandler;
        public TimeLine()
        {
            InitializeComponent();
            this.Loaded += TimeLine_Loaded;
        }

        private int _selIndex = 0;
        void TimeLine_Loaded(object sender, RoutedEventArgs e)
        {
            xListBox.SelectedIndex = _selIndex;
        }

        public void InitList(List<HisThumbnailItem> re)
        {
            xListBox.Items.Clear();

            if (re == null || re.Count == 0)
            {
                return;
            }

            double t = (re.Max(f => f.TaskTime) - re.Min(f => f.TaskTime)).TotalHours;

            for (int i = 0; i < re.Count; i++)
            {
                ListBoxItem lbi = new ListBoxItem();
                lbi.Tag = re[i];

                if (i > 0)
                {
                    double w = 150 * Math.Abs((re[i].TaskTime - re[i - 1].TaskTime).TotalHours) / t;
                    if (w > 30)
                    {
                        lbi.Width = w;
                    }
                }

                xListBox.Items.Add(lbi);
            }
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void xListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (xListBox.Items.Count == 0)
                return;
            HisThumbnailItem hti = ((ListBoxItem)((ListBox)sender).SelectedItem).Tag as HisThumbnailItem;
            if (ListBoxSelectedHandler != null && hti != null)
            {
                _selIndex = ((ListBox) sender).SelectedIndex;
                ListBoxSelectedHandler(hti);
            }
        }
    }

    public class HisThumbnailItem
    {
        public DateTime TaskTime { set; get; }

        public string TaskType { set; get; }

        public string OperatorName { set; get; }
    }
}
