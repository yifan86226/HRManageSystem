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
using System.Windows.Shapes;

namespace CO_IA.UI.StationPlan
{
    /// <summary>
    /// InterfereResetDetailControl.xaml 的交互逻辑
    /// </summary>
    public partial class InterfereResetDetailControl : Window
    {
        public InterfereResetDetailControl()
        {
            InitializeComponent();
            comboxType.ItemsSource = new string[] { "同频", "邻频", "互调" };
            //this.DataContext = pInterfereResult;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("保存成功!", "T提示", MessageBoxButton.OK);
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
