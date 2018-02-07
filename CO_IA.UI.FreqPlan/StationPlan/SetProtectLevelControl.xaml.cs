using CO_IA.Data;
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
using CO_IA.UI.Setting;

namespace CO_IA.UI.FreqPlan.StationPlan
{
    /// <summary>
    /// SetProtectLevelControl.xaml 的交互逻辑
    /// </summary>
    public partial class SetProtectLevelControl : UserControl
    {
        private CheckBox chkAll;
        public ObservableCollection<ProtectLevelData> ProtectLevelDataSource
        {
            get;
            set;
        }

        public event Action GoBack;

        public SetProtectLevelControl()
        {
            InitializeComponent();
            //ProtectLevelDataSource = DataBaseHelper.CreateProtectLevelData();
            this.DataContext = this;
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            //Hyperlink hy = sender as Hyperlink;
            //string type = hy.Tag.ToString();


            //ObservableCollection<EquipmentInfo> equs = new ObservableCollection<EquipmentInfo>(DataBaseHelper.CreateEquipments().Where(r => r.GUID == type));
            //EquipmentListControl equlistcotrol = new EquipmentListControl();

            //equlistcotrol.EquipmentItemsSource = equs.ToArray();
            //Window window = new Window();
            //window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            //window.Content = equlistcotrol;
            //window.ShowDialog();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("保存成功", "提示", MessageBoxButton.OK);
        }

        private void chkAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            bool ischecked = chk.IsChecked.Value;

            foreach (ProtectLevelData item in ProtectLevelDataSource)
            {
                item.IsChecked = ischecked;
            }
        }

        private void chkAll_Loaded(object sender, RoutedEventArgs e)
        {
            chkAll = sender as CheckBox;
            if (this.ProtectLevelDataSource != null)
            {
                chkAll.IsChecked = ProtectLevelDataSource.Any(item => item.IsChecked);
            }
        }

        private void chkCell_Click(object sender, RoutedEventArgs e)
        {
            bool? isChecked = (sender as CheckBox).IsChecked;
            if (!isChecked.HasValue)
            {
                return;
            }
            bool checkedState = isChecked.Value;

            foreach (ProtectLevelData result in ProtectLevelDataSource)
            {
                if (result.IsChecked != checkedState)
                {
                    this.chkAll.IsChecked = null;
                    return;
                }
            }
            chkAll.IsChecked = checkedState;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (GoBack != null)
            {
                GoBack();
            }
        }
    }
}
