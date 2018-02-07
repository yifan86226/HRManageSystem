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
using CO_IA.Data.Monitor;
using CO_IA.Data.MonitorPlan;

namespace CO_IA.UI.MonitorPlan.MonitorDialog
{
    /// <summary>
    /// FreqSelectDialog.xaml 的交互逻辑
    /// </summary>
    public partial class FreqSelectDialog : Window
    {
        public event Action<List<FreqRange>> AfterOKButtonClick;
       // public event Action<DetailMonitorPlan> AfterOKButtonClick;
        List<FreqRange> selectedRangeList = new List<FreqRange>();
        List<double> selectedPointList = new List<double>();
        List<FreqRange> _rangeListSource = new List<FreqRange>();
        List<double> pointListSource = new List<double>();
        private bool _isRangeFreq = true;
        public List<string> ExsitRangeFreqs { get; set; }
        public List<double> ExsitPointFreqs { get; set; }

        DetailMonitorPlan rangeListSource = new DetailMonitorPlan();

        /// <summary>
        /// 加载，频段范围
        /// </summary>
        /// <param name="frequency"></param>
        /// <param name="p_exstFreqs">传过来的频段范围值</param>
        public FreqSelectDialog(DetailMonitorPlan frequency,List<string> p_exstFreqs)
        {
            InitializeComponent();
            this.rangeListSource = frequency;
            this._isRangeFreq = true;
            ExsitRangeFreqs = p_exstFreqs;
            if (ExsitRangeFreqs.Count > 0)
            {
                foreach (var strfreq in ExsitRangeFreqs)
                {
                    FreqRange _freq = new FreqRange();
                    _freq.FreqFrom = Convert.ToDouble(strfreq.Split('-')[0]);
                    _freq.FreqTo = Convert.ToDouble(strfreq.Split('-')[1]);
                    _rangeListSource.Add(_freq);
                }
            }
            _freqRangeLBox.ItemsSource = _rangeListSource;
           // _freqRangeLBox.ItemsSource = rangeListSource.FrequencyRange;
            _freqRangeGrid.Visibility = System.Windows.Visibility.Visible;
            _freqRangeGrid.DataContext = this;
        }
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (AfterOKButtonClick != null && _isRangeFreq == true )
            {
                AfterOKButtonClick(selectedRangeList);
            }
            this.Close();
        }

        private void RangeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            FreqRange range = (sender as CheckBox).Tag as FreqRange;
            selectedRangeList.Add(range);
        }

        private void RangeCheckBox_UnChecked(object sender, RoutedEventArgs e)
        {
            FreqRange range = (sender as CheckBox).Tag as FreqRange;
            if (selectedRangeList.Contains(range))
            {
                selectedRangeList.Remove(range);
            }
        }

        private void PointCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            double range = (double)(sender as CheckBox).Tag;
            selectedPointList.Add(range);
        }

        private void PointCheckBox_UnChecked(object sender, RoutedEventArgs e)
        {
            double range = (double)(sender as CheckBox).Tag;
            if (selectedPointList.Contains(range))
            {
                selectedPointList.Remove(range);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RangeCheckBox_Loaded(object sender, RoutedEventArgs e)
        {
            CheckBox cBox = sender as CheckBox;
           // string freqfrom = cBox.Tag as string;
            FreqRange freqRange = cBox.Tag as FreqRange;
           
           //FreqRange freqRange = cBox.Tag as FreqRange;
           //if (ExsitRangeFreqs != null)
           //{
           //    foreach (FreqRange freq in ExsitRangeFreqs)
           //    {
           //        if (freq.FreqFrom == freqRange.FreqFrom && freq.FreqTo == freqRange.FreqTo)
           //        {
           //            cBox.IsChecked = true;
           //        }
           //    }
           //}
        }

        private void PLaceCheckBox_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void PlaceCheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void PlaceCheckBox_UnChecked(object sender, RoutedEventArgs e)
        {

        }
    }
}
