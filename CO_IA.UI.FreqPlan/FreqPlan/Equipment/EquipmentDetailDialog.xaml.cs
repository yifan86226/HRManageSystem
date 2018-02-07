using CO_IA.Data;
using EMCS.Types;
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
    /// EquipmentDetailDialog.xaml 的交互逻辑
    /// </summary>
    public partial class EquipmentDetailDialog : Window
    {
        private ActivityEquipmentInfo _currentEquipment;

        /// <summary>
        /// 活动的主体
        /// </summary>
        private Activity activity;

        public ActivityEquipmentInfo CurrentEquipment
        {
            get { return _currentEquipment; }
            set { _currentEquipment = value; }
        }

        public string WindowTitle
        {
            set { this.Title = value; }
        }

        public event Action RefreshEquipmentSource;


        /// <summary>
        /// 初始化事件
        /// </summary>
        /// <param name="equipment"></param>
        public EquipmentDetailDialog(ActivityEquipmentInfo equipment)
        {
            InitializeComponent();
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            activity = CO_IA.Client.RiasPortal.ModuleContainer.Activity; ;

            CurrentEquipment = GetEquipmentInfo(equipment);
            if (CurrentEquipment == null)
            {
                CurrentEquipment = new ActivityEquipmentInfo();

                CurrentEquipment.ActivityGuid = activity.Guid;
                CurrentEquipment.OrgInfo.Activity_Guid = activity.Guid;
                if (equipment != null && string.IsNullOrEmpty(equipment.PlaceGuid) == false)
                {
                    CurrentEquipment.PlaceGuid = equipment.PlaceGuid;
                }
                else
                {
                    CurrentEquipment.PlaceGuid =CO_IA.Client.Utility.NewGuid();
                }
                CurrentEquipment.ORGGuid = CurrentEquipment.OrgInfo.Guid = Guid.NewGuid().ToString();
                CurrentEquipment.Origin = 0;
                #region  初始化测试数据

                //CurrentEquipment.EQUCount = 1;
                //CurrentEquipment.IsMobile = true;
                //CurrentEquipment.IsStation = true;
                //CurrentEquipment.Name = "test";

                //CurrentEquipment.ADJChannelInh = 1;
                //CurrentEquipment.RecvAntAzimuth = 1;
                //CurrentEquipment.RecvAntElevation = 1;
                //CurrentEquipment.RecvAntGain = 1;
                //CurrentEquipment.RecvAntHeight = 1;
                //CurrentEquipment.RecvAntModel = "1";
                //CurrentEquipment.RecvAntPolar = EMCS.Types.EMCPolarisationEnum.CR;

                //CurrentEquipment.RecvAntFeedLength = 1;
                //CurrentEquipment.RecvAntFeedLoss = 1;

                //CurrentEquipment.RecvFreqEnd = 1;
                //CurrentEquipment.RecvFreqStart = 1;
                //CurrentEquipment.ReceiveFreq = 1;
                //CurrentEquipment.Sensitivity = 1;
                //CurrentEquipment.SensitivityUnit = 1;
                //CurrentEquipment.SignalNoise = 1;
                //CurrentEquipment.Remark = "1";
                //CurrentEquipment.RunningFrom = DateTime.Now;
                //CurrentEquipment.RunningTo = DateTime.Now;

                //CurrentEquipment.Band = 1;
                //CurrentEquipment.SendAntAzimuth = 1;
                //CurrentEquipment.SendAntElevation = 1;
                //CurrentEquipment.SendAntGain = 1;
                //CurrentEquipment.SendAntHeight = 1;
                //CurrentEquipment.SendAntModel = "1";
                //CurrentEquipment.SendAntPolar = EMCS.Types.EMCPolarisationEnum.CR;

                //CurrentEquipment.SendAntFeedLength = 1;
                //CurrentEquipment.SendAntFeedLoss = 1;

                //CurrentEquipment.SendFreqEnd = 1;
                //CurrentEquipment.SendFreqStart = 1;
                //CurrentEquipment.ChannelBand = 1;
                //CurrentEquipment.IsTunAble = true;
                //CurrentEquipment.Leakage = 1;
                //CurrentEquipment.MaxPower = 1;
                //CurrentEquipment.ModulateMode = EMCS.Types.EMCModulationEnum.ASK;
                //CurrentEquipment.SendFreq = 1;
                //CurrentEquipment.StationName = "StationName";

                #endregion

            }
            this.DataContext = this;

            #region  填充combobox
            List<ActivityORGInfo> cmb_OrgList = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan, List<ActivityORGInfo>>(channel =>
            {
                return channel.GetORGInfos(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
            });

            this.cmb_OrgList.ItemsSource = cmb_OrgList;
            List<EMCModulationEnum> modulation = new List<EMCModulationEnum>();
            foreach (string item in Enum.GetNames(typeof(EMCModulationEnum)))
            {
                EMCModulationEnum modulate = new EMCModulationEnum();
                if (Enum.TryParse(item, out modulate))
                {
                    modulation.Add(modulate);
                }
            }
            combModulate.ItemsSource = modulation;


            List<EMCPolarisationEnum> Polars = new List<EMCPolarisationEnum>();
            foreach (string item in Enum.GetNames(typeof(EMCPolarisationEnum)))
            {
                EMCPolarisationEnum polar;
                if (Enum.TryParse(item, out polar))
                {
                    Polars.Add(polar);
                }

            }
            combAntPolar.ItemsSource = Polars;
            combRecvAntPolar.ItemsSource = Polars;

            combClass.ItemsSource = CO_IA.Client.Utility.GetSecurityClasses();
            combGB.ItemsSource = CO_IA.Client.Utility.BusinessTypes;

            #endregion

        }

        /// <summary>
        /// 查询设备详细信息
        /// </summary>
        /// <param name="equipment"></param>
        /// <returns></returns>
        private ActivityEquipmentInfo GetEquipmentInfo(ActivityEquipmentInfo equipment)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan, ActivityEquipmentInfo>(channel =>
              {
                  return channel.GetEquipmentInfo(equipment);
              });
        }



        /// <summary>
        /// 选择单位改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmb_OrgList_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            if (cmb_OrgList.SelectedItem != null)
            {

                ActivityORGInfo orginfo = cmb_OrgList.SelectedItem as ActivityORGInfo;

                if (CurrentEquipment != null && CurrentEquipment.OrgInfo != null)
                {
                    CurrentEquipment.OrgInfo = orginfo;
                    CurrentEquipment.OrgInfo.Activity_Guid = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid;
                    CurrentEquipment.ORGGuid = orginfo.Guid;
                }
            }
        }

        #region  按键及校验
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {

            if (CurrentEquipment != null && Validate(CurrentEquipment))
            {

                //ActivityORGInfo orginfo = new ActivityORGInfo();
                //if (cb_AddNewUnit.IsChecked == true)
                //{
                //    orginfo.Guid =CO_IA.Client.Utility.NewGuid();
                //    orginfo.Activity_Guid = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid;
                //    orginfo.Address = this.txtAddress.Text.Trim();
                //    orginfo.Contact = this.txtContact.Text.Trim();
                //    orginfo.Name = this.txt_UnitName.Text.Trim();
                //    orginfo.Phone = this.txtUnitPhone.Text.Trim();
                //    orginfo.Securityclass.Guid = this.combClass.SelectedValue as string;
                //    orginfo.Securityclass.Value = this.combClass.SelectedValue as string;
                //    orginfo.ShortName = this.txtShortName.Text.Trim();
                //}
                //else
                //{
                //    orginfo = CurrentEquipment.OrgInfo;
                //}

                ActivityORGInfo tempOrginfo = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan, ActivityORGInfo>(channel =>
                {
                    return channel.GetORGInfoByActivityORGInfo(CurrentEquipment.OrgInfo);
                });


                if (tempOrginfo != null && string.IsNullOrEmpty(tempOrginfo.Activity_Guid) == false)
                {
                    CurrentEquipment.OrgInfo = tempOrginfo;
                }


                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan>(channel =>
                {
                    channel.SaveORGInfo(CurrentEquipment.OrgInfo);
                });

                //CurrentEquipment.OrgInfo = orginfo;
                //CurrentEquipment.OrgInfo.Activity_Guid = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid;
                CurrentEquipment.ORGGuid = CurrentEquipment.OrgInfo.Guid;

                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan>(channel =>
                {
                    channel.SaveEquipment(CurrentEquipment);
                    MessageBox.Show("保存成功!", "提示", MessageBoxButton.OK);
                    this.Close();
                    if (RefreshEquipmentSource != null)
                    {
                        RefreshEquipmentSource();
                    }
                });
            }
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool Validate(ActivityEquipmentInfo equipment)
        {
            bool isSuccess = true;
            StringBuilder errormsg = new StringBuilder();


            if (combClass.SelectedValue == null)
            {
                errormsg.Append("单位的业务类别不能为空 \r");
                isSuccess = false;
            }


            if (string.IsNullOrEmpty(equipment.Name))
            {
                errormsg.Append("名称不能为空 \r");
                isSuccess = false;
            }


            if (string.IsNullOrEmpty(equipment.EQUCount.ToString()))
            {
                errormsg.Append("数量不能为空 \r");
                isSuccess = false;
            }
            else if (equipment.EQUCount == 0)
            {
                errormsg.Append("数量需要大于0 \r");
                isSuccess = false;
            }
            if (string.IsNullOrEmpty(equipment.SendFreq.ToString()))
            {
                errormsg.Append("发射频率不能为空 \r");

                isSuccess = false;
            }
            if (string.IsNullOrEmpty(equipment.SendFreqStart.ToString()))
            {
                errormsg.Append("发射频率开始不能为空 \r");
                isSuccess = false;
            }
            if (string.IsNullOrEmpty(equipment.SendFreqEnd.ToString()))
            {
                errormsg.Append("发射频率结束不能为空 \r");
                isSuccess = false;
            }
            if (string.IsNullOrEmpty(equipment.Band.ToString()))
            {
                errormsg.Append("带宽不能为空 \r");
                isSuccess = false;
            }
            if (string.IsNullOrEmpty(equipment.MaxPower.ToString()))
            {
                errormsg.Append("最大功率不能为空 \r");
                isSuccess = false;
            }
            if (string.IsNullOrEmpty(equipment.ModulateMode.ToString()))
            {
                errormsg.Append("调制方式不能为空 \r");
                isSuccess = false;
            }

            if (string.IsNullOrEmpty(equipment.SendAntGain.ToString()))
            {
                errormsg.Append("发射天线增益不能为空 \r");
                isSuccess = false;
            }
            if (string.IsNullOrEmpty(equipment.SendAntPolar.ToString()))
            {
                errormsg.Append("发射天线极化方式不能为空 \r");
                isSuccess = false;
            }
            if (string.IsNullOrEmpty(equipment.SendAntHeight.ToString()))
            {
                errormsg.Append("发射天线高度不能为空 \r");
                isSuccess = false;
            }
            if (string.IsNullOrEmpty(equipment.SendAntFeedLoss.ToString()))
            {
                errormsg.Append("发射天线馈线损耗不能为空 \r");
                isSuccess = false;
            }

           //接收频率为空。不需要判断接收天线信息
            if (equipment.ReceiveFreq != null)
            {
                if (string.IsNullOrEmpty(equipment.RecvAntGain.ToString()))
                {
                    errormsg.Append("接收天线增益不能为空 \r");
                    isSuccess = false;
                }
                if (string.IsNullOrEmpty(equipment.RecvAntPolar.ToString()))
                {
                    errormsg.Append("接收天线极化方式不能为空 \r");
                    isSuccess = false;
                }
                if (string.IsNullOrEmpty(equipment.RecvAntHeight.ToString()))
                {
                    errormsg.Append("接收天线高度不能为空 \r");
                    isSuccess = false;
                }
                if (string.IsNullOrEmpty(equipment.RecvAntFeedLoss.ToString()))
                {
                    errormsg.Append("接收天线馈线损耗不能为空 \r");
                    isSuccess = false;
                }
            }

            if (!isSuccess)
            {
                MessageBox.Show(errormsg.ToString(), "提示", MessageBoxButton.OK);
            }

            return isSuccess;
        }

        #endregion


        /// <summary>
        /// 单位信息保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cb_AddNewUnit_Click(object sender, RoutedEventArgs e)
        {
            if (cb_AddNewUnit.IsChecked == true)
            {
                cmb_OrgList.Visibility = System.Windows.Visibility.Hidden;
                this.txt_UnitName.Visibility = System.Windows.Visibility.Visible;

                ActivityORGInfo orginfo = new ActivityORGInfo();
                orginfo.Activity_Guid = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid;
                CurrentEquipment.OrgInfo = orginfo;
                CurrentEquipment.ORGGuid = orginfo.Guid;
            }
            else
            {
                cmb_OrgList.Visibility = System.Windows.Visibility.Visible;
                this.txt_UnitName.Visibility = System.Windows.Visibility.Hidden;

                ActivityORGInfo orginfo = cmb_OrgList.SelectedItem as ActivityORGInfo;

                CurrentEquipment.OrgInfo = orginfo;
                CurrentEquipment.ORGGuid = orginfo.Guid;

            }
        }
    }
}
