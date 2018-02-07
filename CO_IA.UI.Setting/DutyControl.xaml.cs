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

namespace CO_IA.UI.Setting
{
    /// <summary>
    /// DutyControl.xaml 的交互逻辑
    /// </summary>
    public partial class DutyControl : UserControl
    {
        public DutyControl()
        {
            InitializeComponent();
            this.DataContextChanged += DutyControl_DataContextChanged;
        }

        void DutyControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.dataGridDuty.ItemsSource = e.NewValue as IList<PP_DutyInfo>;
        }

        private void dataGridDuty_LayoutUpdated(object sender, EventArgs e)
        {
            this.dataGridDuty.RowHeight = double.NaN;
        }
    }
}
