using CO_IA.Data;
using I_CO_IA.FreqStation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace CO_IA.UI.FreqStation.StationPlan
{
    /// <summary>
    /// EquipmentCheckControl.xaml 的交互逻辑
    /// </summary>
    public partial class EquipmentInspectionManageControl : UserControl, INotifyPropertyChanged
    {
        private EquInspectionQueryCondition querycondition = new EquInspectionQueryCondition();
        private EquInspectionQueryCondition QueryCondition
        {
            get { return querycondition; }
            set { querycondition = value; }
        }

        private EquipmentInspection SelectedEquipmentInspection
        {
            get;
            set;
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

        private ActivityPlaceInfo CurrentActivityPlace
        {
            get;
            set;
        }

        public EquipmentInspectionManageControl(ActivityPlaceInfo pActivityPlaceInfo)
        {
            InitializeComponent();
            CurrentActivityPlace = pActivityPlaceInfo;
            equipmentInspectionListControl.EquInspectionSelectionChanged += equipmentInspectionListControl_EquInspectionSelectionChanged;
            GetEquInspections(QueryCondition);
        }

        private void equipmentInspectionListControl_EquInspectionSelectionChanged(EquipmentInspection obj)
        {
            if (obj != null)
            {
                obj.ApplyArea = this.CurrentActivityPlace.Name;
                SelectedEquipmentInspection = obj;
                DataContextEquipmentInspection = AT_BC.Data.Helpers.DataContractSerializeHelper.Clone<EquipmentInspection>(obj);
                equipmentInspectionDetailControl.DataContext = DataContextEquipmentInspection;
            }
        }

        /// <summary>
        /// 查询设备
        /// </summary> 
        /// <param name="condition"></param>
        private void GetEquInspections(EquInspectionQueryCondition condition)
        {
            condition.ActivityGuid = this.CurrentActivityPlace.ActivityGuid;
            condition.PlaceGuid = this.CurrentActivityPlace.Guid;
            List<EquipmentInspection> sources = FreqStationHelper.GetEquipmentInspections(condition);
            equipmentInspectionListControl.DataContext = sources;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            EquipmentInspectionQueryDialog querydialog = new EquipmentInspectionQueryDialog(QueryCondition);
            querydialog.OnQueryEvent += querydialog_OnQueryEvent;
            querydialog.ShowDialog();
        }

        private void querydialog_OnQueryEvent(EquInspectionQueryCondition obj)
        {
            this.QueryCondition = obj;
            GetEquInspections(obj);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (DataContextEquipmentInspection != null)
            {
                DataContextEquipmentInspection.CheckState = equipmentInspectionDetailControl.CheckState;
                if (ValidEquipmentInspection(DataContextEquipmentInspection))
                {
                    try
                    {
                        PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqStation>(channel =>
                        {
                            channel.SaveEquipmentInspection(DataContextEquipmentInspection);
                        });
                        UpdateUI();
                        MessageBox.Show("检测完成");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.GetExceptionMessage());
                    }

                }
            }

        }

        private void UpdateUI()
        {
            SelectedEquipmentInspection.ApplyPerson = DataContextEquipmentInspection.ApplyPerson;
            SelectedEquipmentInspection.ApplyPersonNo = DataContextEquipmentInspection.ApplyPersonNo;
            SelectedEquipmentInspection.ApplyORG = DataContextEquipmentInspection.ApplyORG;
            SelectedEquipmentInspection.EquManufacturer = DataContextEquipmentInspection.EquManufacturer;
            SelectedEquipmentInspection.FreqUENCYNO = DataContextEquipmentInspection.FreqUENCYNO;
            SelectedEquipmentInspection.RunningFrom = DataContextEquipmentInspection.RunningFrom;
            SelectedEquipmentInspection.RunningTo = DataContextEquipmentInspection.RunningTo;
            SelectedEquipmentInspection.ApplyArea = DataContextEquipmentInspection.ApplyArea;
            SelectedEquipmentInspection.Remark = DataContextEquipmentInspection.Remark;
            SelectedEquipmentInspection.CheckState = DataContextEquipmentInspection.CheckState;
            SelectedEquipmentInspection.InspectionDescription = DataContextEquipmentInspection.InspectionDescription;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedEquipmentInspection != null)
            {
                DataContextEquipmentInspection.ApplyPerson = SelectedEquipmentInspection.ApplyPerson;
                DataContextEquipmentInspection.ApplyPersonNo = SelectedEquipmentInspection.ApplyPersonNo;
                DataContextEquipmentInspection.ApplyORG = SelectedEquipmentInspection.ApplyORG;
                DataContextEquipmentInspection.EquManufacturer = SelectedEquipmentInspection.EquManufacturer;
                DataContextEquipmentInspection.FreqUENCYNO = SelectedEquipmentInspection.FreqUENCYNO;
                DataContextEquipmentInspection.RunningFrom = SelectedEquipmentInspection.RunningFrom;
                DataContextEquipmentInspection.RunningTo = SelectedEquipmentInspection.RunningTo;
                DataContextEquipmentInspection.ApplyArea = SelectedEquipmentInspection.ApplyArea;
                DataContextEquipmentInspection.Remark = SelectedEquipmentInspection.Remark;
                DataContextEquipmentInspection.CheckState = SelectedEquipmentInspection.CheckState;
                DataContextEquipmentInspection.InspectionDescription = SelectedEquipmentInspection.InspectionDescription;
            }
            else
            {
                DataContextEquipmentInspection = new EquipmentInspection();
                equipmentInspectionDetailControl.DataContext = DataContextEquipmentInspection;
            }
        }

        private bool ValidEquipmentInspection(EquipmentInspection equipmentInspection)
        {
            StringBuilder errormsg = new StringBuilder();
            bool result = true;
            if (equipmentInspection != null)
            {
                if (string.IsNullOrWhiteSpace(equipmentInspection.ApplyPerson))
                {
                    errormsg.AppendLine("申请人姓名不能为空");
                    result = false;
                }

                if (string.IsNullOrWhiteSpace(equipmentInspection.ApplyPersonNo))
                {
                    errormsg.AppendLine("申请人姓身份证号不能为空");
                    result = false;
                }

                if (string.IsNullOrWhiteSpace(equipmentInspection.ApplyORG))
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

                if (equipmentInspection.CheckState != CheckStateEnum.Qualified && equipmentInspection.CheckState != CheckStateEnum.UnQualified)
                {
                    errormsg.AppendLine("请选择检测结果");
                    result = false;

                }
                if (equipmentInspection.CheckState == CheckStateEnum.UnQualified)
                {
                    if (string.IsNullOrWhiteSpace(equipmentInspection.InspectionDescription))
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


        private void btnSendLicense_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedEquipmentInspection != null)
            {
                if (SelectedEquipmentInspection.CheckState != CheckStateEnum.Qualified)
                {
                    MessageBox.Show("不可以发放许可证!");
                }
                else
                {
                    LicenseTemplete template = FreqStationHelper.GetLicenseTemplete(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
                    if (template == null)
                    {
                        MessageBox.Show("没有许可证模板,请设置许可证模板");
                    }
                    else
                    {
                        List<EquipmentInspection> equs = new List<EquipmentInspection>();
                        equs.Add(SelectedEquipmentInspection);

                        SendLicense(equs);
                    }
                }
            }
        }

        private void btnBathSendLicense_Click(object sender, RoutedEventArgs e)
        {
            List<EquipmentInspection> selects = equipmentInspectionListControl.EquipmentInspectionItemsSource.Where(r => r.IsChecked == true).ToList();

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
                    LicenseTemplete template = FreqStationHelper.GetLicenseTemplete(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
                    if (template == null)
                    {
                        MessageBox.Show("没有许可证模板,请设置许可证模板");
                    }
                    else
                    {
                        SendLicense(selects);
                    }
                }
                else
                {
                    MessageBox.Show(error.ToString().TrimEnd('、'), "提示");
                }
            }
        }

        private void btnLicenseTemplate_Click(object sender, RoutedEventArgs e)
        {
            LicenseTempleteDialog templatedialog = new LicenseTempleteDialog();
            templatedialog.ShowDialog();
        }

        private void SendLicense(List<EquipmentInspection> equs)
        {
            SendLicenseDialog sendLicensedialog = new SendLicenseDialog(equs);
            sendLicensedialog.AfterSendLicense += sendLicensedialog_AfterSendLicense;
            sendLicensedialog.ShowDialog();
        }

        /// <summary>
        /// 更新
        /// </summary>
        private void sendLicensedialog_AfterSendLicense()
        {
            equipmentInspectionListControl.UnSelectedAll();
            GetEquInspections(QueryCondition);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }
    }
}
