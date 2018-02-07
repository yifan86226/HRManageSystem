using System;
using System.Collections;
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

namespace CO_IA.UI.Statistic
{
    /// <summary>
    /// ObjectStatisticControl.xaml 的交互逻辑
    /// </summary>
    public partial class ObjectStatisticControl : UserControl
    {

        public bool ShowContainer
        {
            get
            {
                return this.xstatisticModuleControl.ShowContainer;
            }
            set
            {
                this.xstatisticModuleControl.ShowContainer = value;
            }
        }

        public string SelectedPlace
        {
            set;
            get;
        }

        public IList StatisticSource
        {
            set { this.xstatisticModuleControl.StatisticSource = value; }
        }

        public ObjectStatisticControl()
        {
            InitializeComponent();
        }
    }
}
