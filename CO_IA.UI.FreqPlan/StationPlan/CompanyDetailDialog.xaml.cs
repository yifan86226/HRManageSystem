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

namespace CO_IA.UI.FreqPlan.StationPlan
{
    /// <summary>
    /// CompanyDetailDialog.xaml 的交互逻辑
    /// </summary>
    public partial class CompanyDetailDialog : Window
    {
        public CompanyDetailDialog(ORGInfo company)
        {
            InitializeComponent();
            this.DataContext = company;
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("更新成功!", "提示", MessageBoxButton.OKCancel);
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
