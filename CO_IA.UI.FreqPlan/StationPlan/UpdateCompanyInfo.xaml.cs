using CO_IA.Data;
using CO_IA.UI.Setting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace CO_IA.UI.FreqPlan.StationPlan
{
    /// <summary>
    /// UpdateCompanyInfo.xaml 的交互逻辑
    /// </summary>
    public partial class UpdateCompanyInfo : UserControl
    {

        ObservableCollection<ORGInfo> companies;
        public event Action GoBack;

        public UpdateCompanyInfo()
        {
            InitializeComponent();
            InitData();
        }

        public void InitData()
        {
            //companies = DataBaseHelper.CreateCompanies();
            companydatagrid.ItemsSource = companies;
        }

        public void UpdateSource(int index)
        {
            companydatagrid.ItemsSource = new List<ORGInfo> { companies[index] };
        }

        private void Companydatagrid_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.companydatagrid.SelectedItem != null)
            {
                ORGInfo selectcompany = this.companydatagrid.SelectedItem as ORGInfo;
                CompanyDetailDialog detail = new CompanyDetailDialog(selectcompany);
                detail.ShowDialog(this);
            }
        }

        private void BtnGoBack_Click(object sender, RoutedEventArgs e)
        {
            if (GoBack != null)
            {
                GoBack();
            }
        }
    }
}
