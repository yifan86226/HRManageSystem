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
using CO_IA.UI.Collection.DataAnalysis;

namespace CO_IA.UI.Collection
{
    /// <summary>
    /// NavBarFreqAdd.xaml 的交互逻辑
    /// </summary>
    public partial class NavBarFreqAdd : Window
    {
        public NavBarClickDelegate navBarClickDelegate;
        public FreqNavBarButtonClickDelegate freqNavBarButtonClickDelegate;
        NavBarControl navBarControl;
        public NavBarFreqAdd()
        {
            InitializeComponent();
        }

        public NavBarFreqAdd(NavBarControl nbc) 
        {
            InitializeComponent();
            navBarControl = nbc;
        }

        /// <summary>
        /// 手动添加频段
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bt_freqAdd_Click(object sender, RoutedEventArgs e)
        {
            NavBarGroup nbg = new NavBarGroup();
            nbg.Click += new EventHandler(navBarClickDelegate);
            nbg.Header = "频率段 " + text_freqStart.Text + "--" + text_freqStop.Text + "MHz";
            FreqNavBar fnb = new FreqNavBar();
            fnb.FreqStart = text_freqStart.Text;
            fnb.FreqStop = text_freqStop.Text;
            fnb.reAnalysisFreqRange.Click += new RoutedEventHandler(freqNavBarButtonClickDelegate); 
            fnb.BandWidth = text_bandWidth.Text;
            nbg.Items.Add(fnb);
            navBarControl.Groups.Add(nbg);
            this.Close();
        }
    }
}
