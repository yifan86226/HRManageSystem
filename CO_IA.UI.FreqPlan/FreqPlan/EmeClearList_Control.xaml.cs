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

namespace CO_IA.UI.FreqPlan.FreqPlan
{
    /// <summary>
    /// EmeClearList_Control.xaml 的交互逻辑
    /// </summary>
    public partial class EmeClearList_Control : UserControl
    {
        public EmeClearList_Control()
        {
            InitializeComponent();
            EMEEnvironmentSource = CreateEMEEnvironmentSource();
        }
        public ObservableCollection<EMEEnvironment> EMEEnvironmentSource
        {
            get { return xEMEClearGrid.ItemsSource as ObservableCollection<EMEEnvironment>; }
            set { xEMEClearGrid.ItemsSource = value; }
        }
        private ObservableCollection<EMEEnvironment> CreateEMEEnvironmentSource()
        {
            ObservableCollection<EMEEnvironment> eEmEnvironments = new ObservableCollection<EMEEnvironment>();
            EMEEnvironment eMEEnvironment = new EMEEnvironment();
            eMEEnvironment.Freq = 138.25;
            eMEEnvironment.SignalSource = "已建台站";
            eMEEnvironment.Department = "设台单位";
            eMEEnvironment.Address = "设台单位地址";
            eMEEnvironment.RelationMan = "王某某";
            eMEEnvironment.Phone = "13080000001";
            eMEEnvironment.IsLegal = "是";
            eMEEnvironment.IsClear = "是";
            eMEEnvironment.ResultIsClear = "完成";
            eEmEnvironments.Add(eMEEnvironment);

            eMEEnvironment = new EMEEnvironment();
            eMEEnvironment.Freq = 143.25;
            eMEEnvironment.SignalSource = "未知";
            eMEEnvironment.Department = "";
            eMEEnvironment.Address = "";
            eMEEnvironment.RelationMan = "";
            eMEEnvironment.Phone = "";
            eMEEnvironment.IsLegal = "否";
            eMEEnvironment.IsClear = "是";
            eMEEnvironment.ResultIsClear = "完成";
            eEmEnvironments.Add(eMEEnvironment);

            eMEEnvironment = new EMEEnvironment();
            eMEEnvironment.Freq = 425.25;
            eMEEnvironment.SignalSource = "已建台站";
            eMEEnvironment.Department = "某某某单位";
            eMEEnvironment.Address = "某某某单位地址";
            eMEEnvironment.RelationMan = "李某某";
            eMEEnvironment.Phone = "13080000002";
            eMEEnvironment.IsLegal = "是";
            eMEEnvironment.IsClear = "否";
            eMEEnvironment.ResultIsClear = "未完成";
            eEmEnvironments.Add(eMEEnvironment);

            eMEEnvironment = new EMEEnvironment();
            eMEEnvironment.Freq = 430.25;
            eMEEnvironment.SignalSource = "已建台站";
            eMEEnvironment.Department = "某某某单位";
            eMEEnvironment.Address = "某某某单位地址";
            eMEEnvironment.RelationMan = "张某某";
            eMEEnvironment.Phone = "13080000003";
            eMEEnvironment.IsLegal = "是";
            eMEEnvironment.IsClear = "是";
            eMEEnvironment.ResultIsClear = "未完成";
            eEmEnvironments.Add(eMEEnvironment);

            eMEEnvironment = new EMEEnvironment();
            eMEEnvironment.Freq = 434.25;
            eMEEnvironment.SignalSource = "未知";
            eMEEnvironment.Department = "";
            eMEEnvironment.Address = "";
            eMEEnvironment.RelationMan = "";
            eMEEnvironment.Phone = "";
            eMEEnvironment.IsLegal = "否";
            eMEEnvironment.IsClear = "否";
            eMEEnvironment.ResultIsClear = "完成";

            eEmEnvironments.Add(eMEEnvironment);
            return eEmEnvironments;
        }
        private CheckBox _isAllCheckBox;
        private void ckbSelectedAll_Loaded(object sender, RoutedEventArgs e)
        {
            this._isAllCheckBox = (CheckBox)sender;
            if (this.EMEEnvironmentSource != null)
            {
                _isAllCheckBox.IsChecked = EMEEnvironmentSource.Any(item => item.IsSelected);
            }
        }

        private void ckbSelectedAll_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            bool ischecked = chk.IsChecked.Value;

            foreach (EMEEnvironment item in EMEEnvironmentSource)
            {
                item.IsSelected = ischecked;
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

            foreach (EMEEnvironment result in EMEEnvironmentSource)
            {
                if (result.IsSelected != checkedState)
                {
                    this._isAllCheckBox.IsChecked = null;
                    return;
                }
            }
            _isAllCheckBox.IsChecked = checkedState;
        }
    }
}
