using CO_IA.Client;
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

namespace CO_IA.UI.FreqPlan
{
    /// <summary>
    /// EquipmentQueryDialog.xaml 的交互逻辑
    /// </summary>
    public partial class EquipmentQueryDialog : Window
    {
        private EquipmentQueryCondition QueryCondition;

        public event Action<EquipmentQueryCondition> OnQueryEvent;

        public EquipmentQueryDialog(EquipmentQueryCondition queryCondition)
        {
            InitializeComponent(); 
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            comboxORG.ItemsSource = Utility.GetORGInfos();
            QueryCondition = queryCondition;
            this.DataContext = QueryCondition;
        }

        private void BtnQuery_Click(object sender, RoutedEventArgs e)
        {
            if (OnQueryEvent != null)
            {
                OnQueryEvent(QueryCondition);
            }
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
