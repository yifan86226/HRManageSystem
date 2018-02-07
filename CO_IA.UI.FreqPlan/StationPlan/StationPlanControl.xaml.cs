using CO_IA.Data;
using CO_IA.UI.Setting;

using Microsoft.Win32;
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
    /// StationPlanControl.xaml 的交互逻辑
    /// </summary>
    public partial class StationPlanControl : UserControl
    {
        private int selectedplaceinde;
        public int SelectedPlaceIndex
        {
            get { return selectedplaceinde; }
            set
            {
                selectedplaceinde = value;
                PlaceSourceSelectionChanged(value);
            }
        }

        public ObservableCollection<EquipmentInfo> EquipmentItemsSource
        {
            get;
            set;
        }
        UpdateCompanyInfo companyControl;
        ObservableCollection<ORGInfo> companies;

        public StationPlanControl()
        {
            InitializeComponent();
            InitData();
        }
        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.ErrorException.Message);
        }

        private void InitData()
        {
            //EquipmentItemsSource = DataBaseHelper.CreateEquipments();
            //equipmentListControl.EquipmentItemsSource = EquipmentItemsSource.ToArray();
        }

        public void PlaceSourceSelectionChanged(int index)
        {
            if (borderContent.Visibility == Visibility.Visible)
            {
                if (companyControl != null)
                {
                    companyControl.UpdateSource(index);
                }
            }
            else
            {
                ObservableCollection<EquipmentInfo> collection = new ObservableCollection<EquipmentInfo>();
                if (index == -1)
                {
                    collection = EquipmentItemsSource;
                }
                else if (index == 0)
                {
                    collection = new ObservableCollection<EquipmentInfo>
                      (EquipmentItemsSource.Where(r => r.GUID == "西安塔").ToList()) { };
                }
                else if (index == 1)
                {
                    collection = new ObservableCollection<EquipmentInfo>(EquipmentItemsSource.Where(r => r.GUID == "创意园").ToList()) { };
                }
                //equipmentListControl.EquipmentItemsSource = collection.ToArray();
            }
        }

        private void StationQuery_Click(object sender, RoutedEventArgs e)
        {
            QueryStationRiasDialog dialog = new QueryStationRiasDialog();
            dialog.ShowDialog(this);
        }

        /// <summary>
        /// 手工录入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManualRegister_Click(object sender, RoutedEventArgs e)
        {
            ActivityEquipmentInfo equapply = new ActivityEquipmentInfo();
            EquipmentDetailDialog dialog = new EquipmentDetailDialog(equapply);
            dialog.WindowTitle = "设备-手工登记";
            dialog.ShowDialog(this);
        }

        private void UpdateCompany_Click(object sender, RoutedEventArgs e)
        {
            companyControl = new UpdateCompanyInfo();
            companyControl.GoBack += () => { this.borderContent.Visibility = Visibility.Collapsed; };
            ShowBorderContent(companyControl);
        }

        /// <summary>
        /// 台站库导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExtractFromStationDB_Click(object sender, RoutedEventArgs e)
        {
            ExtractFromStationDBDialog dialog = new ExtractFromStationDBDialog();
            dialog.Show();
        }

        /// <summary>
        /// 设备库导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExtractFromEquipmentDB_Click(object sender, RoutedEventArgs e)
        {
            ExtractFromEquipmentDBDialog extractfromequipment = new ExtractFromEquipmentDBDialog();
            extractfromequipment.ShowDialog(this);
        }

        /// <summary>
        /// 
        /// </summary>
        private void dialog_OnExtractEvent()
        {
            StationListControl lstcontrol = new StationListControl();
            ShowBorderContent(lstcontrol);
        }

        private void ShowBorderContent(UserControl control)
        {
            borderContent.Child = null;
            borderContent.Child = control;
            borderContent.Visibility = Visibility;
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("确认要删除选择的行", "提示", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                //ObservableCollection<EquipmentInfo> items = new ObservableCollection<EquipmentInfo>(this.equipmentListControl.EquipmentItemsSource);
                //items.Remove(this.equipmentListControl.SelectedEquipment);
            }
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPlaceIndex == 0)
            {
                MessageBox.Show("请选择设备使用地点!", "提示", MessageBoxButton.OK);
                return;
            }
            OpenFileDialog opendialog = new OpenFileDialog();
            opendialog.Filter = "设备文件|*.xls";
            if (opendialog.ShowDialog() == true)
            {
                MessageBox.Show("导入成功!", "提示", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// 保障级别设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSetProtectLevel_Click(object sender, RoutedEventArgs e)
        {
            SetProtectLevelControl protectLevel = new SetProtectLevelControl();
            protectLevel.GoBack += () => { this.borderContent.Visibility = Visibility.Collapsed; };
            ShowBorderContent(protectLevel);
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BunExport_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog savedialog = new SaveFileDialog();
            savedialog.Filter = "导出文件|*.xls";
            if (savedialog.ShowDialog() == true)
            {
                MessageBox.Show("导出成功!", "提示", MessageBoxButton.OK);
            }
        }


    }
}
