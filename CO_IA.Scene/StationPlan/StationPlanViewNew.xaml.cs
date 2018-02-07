using AT_BC.Common;
using CO_IA.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using CO_IA.UI.FreqStation;
using CO_IA.UI.FreqStation.StationPlan;
using I_CO_IA.FreqStation;
using CO_IA_Data;
using CO_IA.UI.PlanDatabase.Equipments;
using DevExpress.Xpf.Editors;

namespace CO_IA.Scene.StationPlan
{
    /// <summary>
    /// StationPlanViewNew.xaml 的交互逻辑
    /// </summary>
    public partial class StationPlanViewNew : UserControl
    {
        CheckBox checkBoxAll;
        public event Action<EquipmentInspection> EquInspectionSelectionChanged;
        private EquipmentInspection equipmentinspection;
        public EquipmentInspection EquipmentInspectionSelected
        {
            get
            {
                return equipmentinspection;
            }
            set
            {
                equipmentinspection = value;
                NotifyPropertyChanged("EquipmentInspectionSelected");
            }
        }
        private EquipmentInspection datacontextequipmentInspection;
        public EquipmentInspection DataContextEquipmentInspection
        {
            get
            {
                return datacontextequipmentInspection;
            }
            set
            {
                datacontextequipmentInspection = value;
                NotifyPropertyChanged("DataContextEquipmentInspection");
            }
        }
        public List<EquipmentInspection> EquipmentInspectionItemsSource
        {
            get { return this.DataContext as List<EquipmentInspection>; }

        }
        public StationPlanViewNew()
        {
            InitializeComponent();
            EquInspectionQueryCondition condition = new EquInspectionQueryCondition()
            {
                 ActivityGuid = SystemLoginService.CurrentActivity.Guid,
                 PlaceGuid = SystemLoginService.CurrentActivityPlace.Guid
                 
            };
            this.DataContext = DataOperator.GetEquipmentInspections(condition);
            this.DataContextChanged += EquipmentInspectionListControl_DataContextChanged;
        }
        private void EquipmentInspectionListControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            List<EquipmentInspection> sources = sender as List<EquipmentInspection>;
            if (sources != null && sources.Count > 0)
            {
                this.equInspectiondatagrid.SelectedIndex = 0;
            }
        }
        private void chkAll_Loaded(object sender, RoutedEventArgs e)
        {
            checkBoxAll = sender as CheckBox;
        }
        private void CheckableDataCheckedCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CheckableCheckBoxHelper.Checked(sender, e, this.checkBoxAll);
        }
        
        private void equInspectiondatagrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (EquipmentInspectionSelected != null && EquipmentInspectionSelected.ActivityEquipment != null)
            {
                ActivityEquipment equip = DataOperator.GetEquipmentByID(EquipmentInspectionSelected.ActivityEquipment.Key);
                EquipmentManageDialog equdialog = new EquipmentManageDialog();
                equdialog.AllowEdit = false;
                equdialog.DataContext = equip;
                equdialog.ShowDialog();
            }
        }
        private void equInspectiondatagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EquipmentInspectionSelected = equInspectiondatagrid.SelectedItem as EquipmentInspection;
            if (EquipmentInspectionSelected != null)
            {
                equipmentInspectionDetailControl.IsEnabled = true;
                DataContextEquipmentInspection = AT_BC.Data.Helpers.DataContractSerializeHelper.Clone<EquipmentInspection>(EquipmentInspectionSelected);
                equipmentInspectionDetailControl.DataContext = DataContextEquipmentInspection;
            }
        }

        /// <summary>
        /// 临时设备入场
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TemporaryDeviceBtn_Click(object sender, RoutedEventArgs e)
        {
            ShowDeviceCheckInfo(false);
            TemporaryDeviceDialog dialog = new TemporaryDeviceDialog();
            dialog.ShowDialog();
        }
        /// <summary>
        /// 更改频率
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemFreqModify_Click(object sender, RoutedEventArgs e)
        {
            ShowDeviceCheckInfo(false);
            ShowChangedFreqColumnVisible(true);

         
        }
        /// <summary>
        /// 保存频率
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemSaveFreq_Click(object sender, RoutedEventArgs e)
        {
            ShowDeviceCheckInfo(false);
            ShowChangedFreqColumnVisible(false);
            List<ActivityEquipment> list = GetGridCheckedEquipments();   
            if (VerifyAssignFreq(list,true))
            {
                DataOperator.SaveEquipFreq(list, callbackRst =>
                {
                    MessageBox.Show("保存成功！");
                });
            }
            else
            {
                ShowChangedFreqColumnVisible(true);
            }
           
        }
        void ShowChangedFreqColumnVisible(bool p_visible)
        {

            xChangedFreqBtn.Visibility = p_visible == true ? Visibility.Collapsed : Visibility.Visible;
            _saveFreqBtn.Visibility = p_visible == true ? Visibility.Visible : Visibility.Collapsed;
            _readOnlyFreqColumn.Visibility = p_visible == true ? Visibility.Collapsed : Visibility.Visible;
            _editFreqColumn.Visibility = p_visible == true ? Visibility.Visible : Visibility.Collapsed; 
            _readOnlyStandbyFreqColumn.Visibility = p_visible == true ? Visibility.Collapsed : Visibility.Visible; 
            _editStandbyFreqColumn.Visibility = p_visible == true ? Visibility.Visible : Visibility.Collapsed; 
        }

        private bool VerifyAssignFreq(List<ActivityEquipment> equs,bool isSaveFreqVerify)
        {
            bool result = true;
            StringBuilder errormsg = new StringBuilder();
            if (equs.Count == 0)
            {
                MessageBox.Show("请勾选要指配频率的设备");
                return false;
            }
            foreach (ActivityEquipment equ in equs)
            {
                EquipmentInspection equipmentInspection = EquipmentInspectionItemsSource.Where(p => p.ActivityEquipment == equ).ToList().FirstOrDefault();
                if (equ.AssignSendFreq == null)
                {
                    result = false;
                    errormsg.AppendFormat(string.Format("{0}指配频率为空 \n", equ.Name), "提示");
                }
            }
            if (!result)
            {
                MessageBox.Show(errormsg.ToString());
            }
            return result;
        }
        private void InterferenceAnalyseButton_Click(object sender, RoutedEventArgs e)
        {
            ShowDeviceCheckInfo(false);
              List<ActivityEquipment> equs = GetGridCheckedEquipments();
                if (equs == null || equs.Count == 0)
                {
                    MessageBox.Show("请选择要进行干扰分析的设备");
                }
                else
                {
                    InterfereAnalyseRealize(equs);
                }
        }
        public void InterfereAnalyseRealize(List<ActivityEquipment> equs)
        {
            if (VerifyAssignFreq(equs,false))
            {
                List<ActivitySurroundStation> surroundstation = DataOperator.GetAroundStations();
                InterfereAnalyseDialog interfdialog = new InterfereAnalyseDialog(equs, surroundstation);
                interfdialog.ShowDialog();
            }
        }

        private  List<ActivityEquipment> GetGridCheckedEquipments()
        {
            List<ActivityEquipment> list = EquipmentInspectionItemsSource.Where(p => p.IsChecked == true).Select(q => q.ActivityEquipment).ToList();
            list.ForEach(p => p.ActivityGuid = SystemLoginService.CurrentActivity.Guid);
            list.ForEach(p => p.PlaceGuid = SystemLoginService.CurrentActivityPlace.Guid);
            return list;
        }
        private void MenuItemEquipmentInspection_Click(object sender, RoutedEventArgs e)
        {
            ShowDeviceCheckInfo(true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_menuItem"></param>
        private void ShowDeviceCheckInfo(bool p_showCheck)
        {
            xDeviceCheckGrid.Visibility = p_showCheck==true ?
                System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }
        private void MenuItemLicenseIssuance_Click(object sender, RoutedEventArgs e)
        {
            ShowDeviceCheckInfo(true);

            List<EquipmentInspection> selects = EquipmentInspectionItemsSource.Where(r => r.IsChecked == true).ToList();
            if (selects.Count == 0)
            {
                MessageBox.Show("请勾选要批量发放许可证的设备");
                return;
            }
            else
            {
                StringBuilder error = new StringBuilder();
                error.AppendLine("请移除不可以发放许可证的设备:");

                bool isok = true;
                foreach (EquipmentInspection equ in selects)
                {
                    if (equ.CheckState != CheckStateEnum.Qualified)
                    {
                        error.AppendFormat("{0}、", equ.ActivityEquipment.Name);
                        isok = false;
                    }
                }
                if (isok)
                {
                    SendLicense(selects);
                }
                else
                {
                    MessageBox.Show(error.ToString().TrimEnd('、'), "提示");
                }
            }

        }
        private void SendLicense(List<EquipmentInspection> equs)
        {
            if(FreqStationHelper.GetLicenseTemplete(SystemLoginService.CurrentActivity.Guid) == null)
            {
                MessageBox.Show("没有许可证模板,请设置许可证模板");
                return;
            }
            SendLicenseDialog sendLicensedialog = new SendLicenseDialog(equs);
            sendLicensedialog.AfterSendLicense+=()=>
            {
               foreach(EquipmentInspection equ in equs)
               {
                   equ.SendLicense = SendLicenseEnum.SendLicense;
                   UpdateUI(equ);
               }
            };
            sendLicensedialog.ShowDialog();
        }

        private void btnBathSendLicense_Click(object sender, RoutedEventArgs e)
        {
            ShowDeviceCheckInfo(true);
            List<EquipmentInspection> selects = EquipmentInspectionItemsSource.Where(r => r.IsChecked == true).ToList();
            if (selects.Count == 0)
            {
                MessageBox.Show("请勾选要批量发放许可证的设备");
                return;
            }
            else
            {
                StringBuilder error = new StringBuilder();
                error.AppendLine("请移除不可以发放许可证的设备:");

                bool isok = true;
                foreach (EquipmentInspection equ in selects)
                {
                    if (equ.CheckState != CheckStateEnum.Qualified)
                    {
                        error.AppendFormat("{0}、", equ.ActivityEquipment.Name);
                        isok = false;
                    }
                }
                if (isok)
                {
                    SendLicense(selects);
                }
                else
                {
                    MessageBox.Show(error.ToString().TrimEnd('、'), "提示");
                }
            }

        }
        private void MenuItemEquipmentPrintTemplate_Click(object sender, RoutedEventArgs e)
        {
            ShowDeviceCheckInfo(true);
            LicenseTempleteDialog templatedialog = new LicenseTempleteDialog();
            templatedialog.ShowDialog();
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (DataContextEquipmentInspection == null) return;
            DataContextEquipmentInspection.CheckState = equipmentInspectionDetailControl.CheckState;
            if (ValidEquipmentInspection(DataContextEquipmentInspection))
            {
                try
                {
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqStation>(channel =>
                    {
                        channel.SaveEquipmentInspection(DataContextEquipmentInspection);
                    });
                    UpdateUI(DataContextEquipmentInspection);
                    MessageBox.Show("检测完成");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.GetExceptionMessage());
                }

            }
        }
        private void UpdateUI(EquipmentInspection equ)
        {
            EquipmentInspectionSelected.ApplyPerson = equ.ApplyPerson;
            EquipmentInspectionSelected.ApplyPersonNo = equ.ApplyPersonNo;
            EquipmentInspectionSelected.ApplyORG = equ.ApplyORG;
            EquipmentInspectionSelected.EquManufacturer = equ.EquManufacturer;
            EquipmentInspectionSelected.FreqUENCYNO = equ.FreqUENCYNO;
            EquipmentInspectionSelected.RunningFrom = equ.RunningFrom;
            EquipmentInspectionSelected.RunningTo = equ.RunningTo;
            EquipmentInspectionSelected.ApplyArea = equ.ApplyArea;
            EquipmentInspectionSelected.Remark = equ.Remark;
            EquipmentInspectionSelected.CheckState = equ.CheckState;
            EquipmentInspectionSelected.InspectionDescription = equ.InspectionDescription;
            EquipmentInspectionSelected.SendLicense = equ.SendLicense;
        }
        private bool ValidEquipmentInspection(EquipmentInspection equipmentInspection)
        {
            StringBuilder errormsg = new StringBuilder();
            bool result = true;
            if (equipmentInspection != null)
            {
                if (string.IsNullOrEmpty(equipmentInspection.ApplyPerson))
                {
                    errormsg.AppendLine("申请人姓名不能为空");
                    result = false;
                }

                if (string.IsNullOrEmpty(equipmentInspection.ApplyPersonNo))
                {
                    errormsg.AppendLine("申请人姓身份证号不能为空");
                    result = false;
                }

                if (string.IsNullOrEmpty(equipmentInspection.ApplyORG))
                {
                    errormsg.AppendLine("申请单位不能为空");
                    result = false;
                }

                if (string.IsNullOrEmpty(equipmentInspection.EquManufacturer))
                {
                    errormsg.AppendLine("设备生产厂商不能为空");
                    result = false;
                }
                if (equipmentInspection.RunningFrom == null)
                {
                    errormsg.AppendLine("申请使用时间开始不能为空");
                    result = false;
                }
                if (equipmentInspection.RunningTo == null)
                {
                    errormsg.AppendLine("申请使用时间结束不能为空");
                    result = false;
                }

                if (equipmentInspection.CheckState == CheckStateEnum.UnQualified)
                {
                    if (string.IsNullOrEmpty(equipmentInspection.InspectionDescription))
                    {
                        errormsg.AppendLine("请输入检测结果描述");
                        result = false;
                    }
                }
                if (!result)
                {
                    MessageBox.Show(errormsg.ToString(), "验证失败");
                }
                return result;
            }
            return false;
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (EquipmentInspectionSelected != null)
            {
                DataContextEquipmentInspection.ApplyPerson = EquipmentInspectionSelected.ApplyPerson;
                DataContextEquipmentInspection.ApplyPersonNo = EquipmentInspectionSelected.ApplyPersonNo;
                DataContextEquipmentInspection.ApplyORG = EquipmentInspectionSelected.ApplyORG;
                DataContextEquipmentInspection.EquManufacturer = EquipmentInspectionSelected.EquManufacturer;
                DataContextEquipmentInspection.FreqUENCYNO = EquipmentInspectionSelected.FreqUENCYNO;
                DataContextEquipmentInspection.RunningFrom = EquipmentInspectionSelected.RunningFrom;
                DataContextEquipmentInspection.RunningTo = EquipmentInspectionSelected.RunningTo;
                DataContextEquipmentInspection.ApplyArea = EquipmentInspectionSelected.ApplyArea;
                DataContextEquipmentInspection.Remark = EquipmentInspectionSelected.Remark;
                DataContextEquipmentInspection.CheckState = EquipmentInspectionSelected.CheckState;
                DataContextEquipmentInspection.InspectionDescription = EquipmentInspectionSelected.InspectionDescription;
            }
        }
        string _oldValue;
        private void TextEdit_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            double d;
            TextEdit tbox = sender as TextEdit;
            if (double.TryParse(tbox.Text, out d))
            {
                _oldValue = tbox.Text;
            }
            else
            {
                tbox.Text = _oldValue.ToString();
            }
        }

        #region IPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }
        #endregion
    }
}
