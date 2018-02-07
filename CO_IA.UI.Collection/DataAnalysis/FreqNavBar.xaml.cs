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
using DevExpress.Xpf.NavBar;
using System.Collections.ObjectModel;
using CO_IA.Data.Collection;

namespace CO_IA.UI.Collection.DataAnalysis
{
    public delegate void ShowSingalTypeChangeHandel(ShowSingalType type);
    /// <summary>
    /// 信号显示类别
    /// </summary>
    public enum ShowSingalType
    {
        /// <summary>
        /// 全部显示
        /// </summary>
        All = 0,
        /// <summary>
        /// 只显示占用的
        /// </summary>
        Using = 1,
        /// <summary>
        /// 只显示未占用的
        /// </summary>
        UnUsing = 2,
        /// <summary>
        /// 都不显示
        /// </summary>
        None = 3
    }
    /// <summary>
    /// FreqNavBar.xaml 的交互逻辑
    /// </summary>
    public partial class FreqNavBar : UserControl
    {
        public event ShowSingalTypeChangeHandel ShowSingalTypeChangeEvent;

        public FreqNavBar()
        {
            InitializeComponent();
        }
        private string placeGuid;

        public string PlaceGuid
        {
            get { return placeGuid; }
            set { placeGuid = value; }
        }
        /// <summary>
        /// 存储分析结果
        /// </summary>
        public ObservableCollection<AnalysisResult> freqList;

        //add by michael 2017.08.17
        private string freqGuid;

        public string FreqGuid
        {
            get { return freqGuid; }
            set { freqGuid = value; }
        }
        //end 

        private string measureId;

        public string MeasureId
        {
            get { return measureId; }
            set { measureId = value; }
        }
        private string freqStart;

        public string FreqStart
        {
            get { return freqStart; }
            set { freqStart = value; }
        }

        private string freqStop;

        public string FreqStop
        {
            get { return freqStop; }
            set { freqStop = value; }
        }

        private string bandWidth;

        public string BandWidth
        {
            get { return bandWidth; }
            set { bandWidth = value; }
        }

        private string signalLimit;

        public string SignalLimit
        {
            get { return signalLimit; }
            set { signalLimit = value; }
        }

        private string occuDegreeLimit;

        public string OccuDegreeLimit
        {
            get { return occuDegreeLimit; }
            set { occuDegreeLimit = value; }
        }

        /// <summary>
        /// 是否显示已占用信号
        /// </summary>
        public bool ShowUsingSinaglChecked
        {
            get { return _showUsingSinaglChecked; }
            set { _showUsingSinaglChecked = value; }
        }
        private bool _showUsingSinaglChecked = true;

        /// <summary>
        /// 是否显示未占用信号
        /// </summary>
        public bool ShowUnUsingSinaglChecked
        {
            get { return _showUnUsingSinaglChecked; }
            set { _showUnUsingSinaglChecked = value; }
        }
        private bool _showUnUsingSinaglChecked = true;

        private void delete_freqNavBar_Click(object sender, RoutedEventArgs e)
        {
            FreqNavBar objNavBar = (FreqNavBar)this;
            NavBarItem objNavItem = (NavBarItem)objNavBar.Parent;
            NavBarGroup objNavGroup = (NavBarGroup)objNavItem.Parent;
            NavBarControl nbc = (NavBarControl)objNavGroup.Parent;
            nbc.Groups.Remove(objNavGroup);
        }

        private void reAnalysisFreqRange_Click(object sender, RoutedEventArgs e)
        {
        }

        private void cbxUsing_Checked(object sender, RoutedEventArgs e)
        {
            ShowSingalType type = ShowSingalType.All;
            if (cbxUsing.IsChecked == true && cbxUnUsing.IsChecked == true)
            {
                type = ShowSingalType.All;
            }
            else if (cbxUsing.IsChecked == true && cbxUnUsing.IsChecked != true)
            {
                type = ShowSingalType.Using;
            }
            else if (cbxUsing.IsChecked != true && cbxUnUsing.IsChecked == true)
            {
                type = ShowSingalType.UnUsing;
            }
            else if (cbxUsing.IsChecked != true && cbxUnUsing.IsChecked != true)
            {
                type = ShowSingalType.None;
            }
            if (ShowSingalTypeChangeEvent != null)
            {
                ShowSingalTypeChangeEvent(type);
            }
        }
    }
}
