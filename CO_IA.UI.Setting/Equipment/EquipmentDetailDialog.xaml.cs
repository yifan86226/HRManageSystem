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

namespace CO_IA.UI.Setting.Equipment
{
    /// <summary>
    /// EquipmentDetailDialog.xaml 的交互逻辑
    /// </summary>
    public partial class EquipmentDetailDialog : Window
    {
        public EquipmentInfo CurrentEquipment
        {
            get { return (EquipmentInfo)GetValue(CurrentEquipmentProperty); }
            set { SetValue(CurrentEquipmentProperty, value); }
        }

        public static readonly DependencyProperty CurrentEquipmentProperty =
            DependencyProperty.Register("MyProperty", typeof(EquipmentInfo), typeof(EquipmentDetailDialog), null);

        public string WindowTitle
        {
            set { this.Title = value; }
        }

        public event Action RefreshEquipmentSource;

        bool allowedite = true;
        public bool AllowEdite
        {
            get { return allowedite; }
            set
            {
                allowedite = value;
                if (allowedite)
                {
                    _rectangle.Visibility = Visibility.Collapsed;
                    btnSave.IsEnabled = true;
                    btnCancel.IsEnabled = true;
                }
                else
                {
                    _rectangle.Visibility = Visibility.Visible;
                    btnSave.IsEnabled = false;
                    btnCancel.IsEnabled = false;
                }
            }
        }

        public EquipmentDetailDialog(EquipmentInfo equipment, bool isAdd)
        {
            InitializeComponent();
            if (!isAdd)
            {
                CurrentEquipment = GetEquipmentInfo(equipment);
            }
            else
            {
                CurrentEquipment = equipment;
            }

            this.DataContext = this;
        }

        /// <summary>
        /// 查询设备详细信息
        /// </summary>
        /// <param name="equipment"></param>
        /// <returns></returns>
        private EquipmentInfo GetEquipmentInfo(EquipmentInfo equipment)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.Setting.I_CO_IA_Setting, EquipmentInfo>(channel =>
              {
                  return channel.GetEquipmentInfo(equipment);
              });
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            EquipmentInfo equipment = _equipmentDetailControl.CurrentEquipmentInfo;
            if (equipment != null && Validate(equipment))
            {
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.Setting.I_CO_IA_Setting>(channel =>
                {
                    channel.SaveEquipment(equipment);
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
        private bool Validate(EquipmentInfo equipment)
        {
            bool isSuccess = true;
            StringBuilder errormsg = new StringBuilder();
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


            if (equipment.SendPara == null && equipment.RecivePara == null)
            {
                errormsg.Append("发射参数和接收参数不同时为空! \r");
                isSuccess = false;
            }
            else
            {
                #region 发射

                //如果有发射信息
                if (equipment.SendPara != null)
                {
                    if (string.IsNullOrEmpty(equipment.SendPara.SendFreq.ToString()))
                    {
                        errormsg.Append("发射频率不能为空 \r");
                        isSuccess = false;
                    }
                    else if (string.IsNullOrEmpty(equipment.SendPara.FreqStart.ToString()))
                    {
                        errormsg.Append("发射频率开始不能为空 \r");
                        isSuccess = false;
                    }
                    else if (string.IsNullOrEmpty(equipment.SendPara.FreqEnd.ToString()))
                    {
                        errormsg.Append("发射频率结束不能为空 \r");
                        isSuccess = false;
                    }
                    else if (equipment.SendPara.SendFreq < 0 || equipment.SendPara.SendFreq > 99999.99999)
                    {
                        errormsg.Append("发射频率需要在(0.00001-99999.99999)之间 \r");
                        isSuccess = false;
                    }
                    if (string.IsNullOrEmpty(equipment.SendPara.Band.ToString()))
                    {
                        errormsg.Append("带宽不能为空 \r");
                        isSuccess = false;
                    }
                    else if (equipment.SendPara.Band < 0 || equipment.SendPara.Band > 99999.99999)
                    {
                        errormsg.Append("带宽需要在(0.01-99999.99)之间 \r");
                        isSuccess = false;
                    }

                    if (string.IsNullOrEmpty(equipment.SendPara.MaxPower.ToString()))
                    {
                        errormsg.Append("最大功率不能为空 \r");
                        isSuccess = false;
                    }
                    else if (equipment.SendPara.MaxPower < 0 || equipment.SendPara.MaxPower > 9999.9999)
                    {
                        errormsg.Append("功率需要在(0.0001-9999.9999)之间 \r");
                        isSuccess = false;
                    }

                    #region 发射天线

                    if (string.IsNullOrEmpty(equipment.SendPara.MaxPower.ToString()))
                    {
                        errormsg.Append("调制方式不能为空 \r");
                        isSuccess = false;
                    }

                    if (equipment.SendPara.Ant != null)
                    {
                        if (string.IsNullOrEmpty(equipment.SendPara.Ant.AntGain.ToString()))
                        {
                            errormsg.Append("发射天线增益不能为空 \r");
                            isSuccess = false;
                        }
                        else if (equipment.SendPara.Ant.AntGain < 0 || equipment.SendPara.Ant.AntGain > 99.99)
                        {
                            errormsg.Append("发射天线增益需要在(0.01-99.99) \r");
                            isSuccess = false;
                        }
                        if (equipment.SendPara.Ant.AntElevation != null && (equipment.SendPara.Ant.AntElevation < 0 || equipment.SendPara.Ant.AntElevation > 99.99))
                        {
                            errormsg.Append("发射天线仰角需要在(0.01-99.99) \r");
                            isSuccess = false;
                        }
                        if (equipment.SendPara.Ant.AntAzimuth != null && (equipment.SendPara.Ant.AntAzimuth < 0 || equipment.SendPara.Ant.AntAzimuth > 999.99))
                        {
                            errormsg.Append("发射天线方位角需要在(0.01-999.99) \r");
                            isSuccess = false;
                        }

                        if (string.IsNullOrEmpty(equipment.SendPara.Ant.AntPolar.ToString()))
                        {
                            errormsg.Append("发射天线极化方式不能为空 \r");
                            isSuccess = false;
                        }

                        if (string.IsNullOrEmpty(equipment.SendPara.Ant.AntHight.ToString()))
                        {
                            errormsg.Append("发射天线高度不能为空 \r");
                            isSuccess = false;
                        }
                        else if (equipment.SendPara.Ant.AntGain < 0 || equipment.SendPara.Ant.AntGain > 9999.99)
                        {
                            errormsg.Append("发射天线高度需要在(0.01-9999.99) \r");
                            isSuccess = false;
                        }
                        if (equipment.SendPara.Ant.FeedLength != null && (equipment.SendPara.Ant.FeedLength < 0 || equipment.SendPara.Ant.FeedLength > 9999.99))
                        {
                            errormsg.Append("发射天线馈线长度需要在(0.01-9999.99) \r");
                            isSuccess = false;
                        }
                        if (string.IsNullOrEmpty(equipment.SendPara.Ant.FeedLose.ToString()))
                        {
                            errormsg.Append("发射天线馈线损耗不能为空 \r");
                            isSuccess = false;
                        }
                        else if (equipment.SendPara.Ant.FeedLose < 0 || equipment.SendPara.Ant.FeedLose > 999.99)
                        {
                            errormsg.Append("发射天线馈线损耗需要在(0.01-999.99) \r");
                            isSuccess = false;
                        }

                    #endregion

                    }
                }

                #endregion

                #region 接收

                if (equipment.RecivePara != null)
                {
                    if (equipment.RecivePara.ReceiveFreq < 0 || equipment.RecivePara.ReceiveFreq > 99999.99999)
                    {
                        errormsg.Append("接收频率起始需要在(0.00001-99999.99999)之间 \r");
                        isSuccess = false;
                    }
                    if (equipment.RecivePara.FreqStart < 0 || equipment.RecivePara.FreqStart > 99999.99999)
                    {
                        errormsg.Append("接收频率起始需要在(0.00001-99999.99999)之间 \r");
                        isSuccess = false;
                    }
                    if (equipment.RecivePara.FreqEnd < 0 || equipment.RecivePara.FreqEnd > 99999.99999)
                    {
                        errormsg.Append("接收频率终止需要在(0.00001-99999.99999)之间 \r");
                        isSuccess = false;
                    }

                    //if (string.IsNullOrEmpty(equipment.SendPara.Band.ToString()))
                    //{
                    //    errormsg.Append("带宽不能为空 \r");
                    //    isSuccess = false;
                    //}
                    //else
                    //{
                    //    if (equipment.SendPara.Band < 0 || equipment.SendPara.Band > 99999.99999)
                    //    {
                    //        errormsg.Append("带宽需要在(0.01-99999.99)之间 \r");
                    //        isSuccess = false;
                    //    }
                    //}
                    //if (string.IsNullOrEmpty(equipment.SendPara.MaxPower.ToString()))
                    //{
                    //    errormsg.Append("最大功率不能为空 \r");
                    //    isSuccess = false;
                    //}
                    //else if (equipment.SendPara.MaxPower < 0 || equipment.SendPara.MaxPower > 9999.9999)
                    //{
                    //    errormsg.Append("功率需要在(0.0001-9999.9999)之间 \r");
                    //    isSuccess = false;
                    //}
                    //if (string.IsNullOrEmpty(equipment.SendPara.MaxPower.ToString()))
                    //{
                    //    errormsg.Append("调制方式不能为空 \r");
                    //    isSuccess = false;
                    //}

                    #region 天线信息

                    if (string.IsNullOrEmpty(equipment.RecivePara.Ant.AntGain.ToString()))
                    {
                        errormsg.Append("接收天线增益不能为空 \r");
                        isSuccess = false;
                    }
                    else if (equipment.RecivePara.Ant.AntGain < 0 || equipment.RecivePara.Ant.AntGain > 99.99)
                    {
                        errormsg.Append("接收天线增益需要在(0.01-99.99) \r");
                        isSuccess = false;
                    }

                    if (equipment.RecivePara.Ant.AntElevation != null && (equipment.RecivePara.Ant.AntElevation < 0 || equipment.RecivePara.Ant.AntElevation > 99.99))
                    {
                        errormsg.Append("接收天线仰角需要在(0.01-99.99) \r");
                        isSuccess = false;
                    }
                    if (equipment.RecivePara.Ant.AntAzimuth != null && (equipment.RecivePara.Ant.AntAzimuth < 0 || equipment.RecivePara.Ant.AntAzimuth > 999.99))
                    {
                        errormsg.Append("接收天线方位角需要在(0.01-999.99) \r");
                        isSuccess = false;
                    }

                    if (string.IsNullOrEmpty(equipment.RecivePara.Ant.AntPolar.ToString()))
                    {
                        errormsg.Append("接收天线极化方式不能为空 \r");
                        isSuccess = false;
                    }
                    if (string.IsNullOrEmpty(equipment.RecivePara.Ant.AntHight.ToString()))
                    {
                        errormsg.Append("接收天线高度不能为空 \r");
                        isSuccess = false;
                    }
                    else if (equipment.RecivePara.Ant.AntHight < 0 || equipment.RecivePara.Ant.AntHight > 9999.99)
                    {
                        errormsg.Append("接收天线高度需要在(0.01-9999.99) \r");
                        isSuccess = false;
                    }
                    if (equipment.RecivePara.Ant.FeedLength != null && (equipment.RecivePara.Ant.FeedLength < 0 || equipment.RecivePara.Ant.FeedLength > 9999.99))
                    {
                        errormsg.Append("接收天线馈线长度需要在(0.01-9999.99) \r");
                        isSuccess = false;
                    }

                    if (string.IsNullOrEmpty(equipment.RecivePara.Ant.FeedLose.ToString()))
                    {
                        errormsg.Append("接收天线馈线损耗不能为空 \r");
                        isSuccess = false;
                    }
                    else if (equipment.RecivePara.Ant.FeedLose < 0 || equipment.RecivePara.Ant.FeedLose > 999.99)
                    {
                        errormsg.Append("接收天线馈线损耗需要在(0.01-999.99) \r");
                        isSuccess = false;
                    }

                    #endregion
                }

                #endregion

            }
            if (!isSuccess)
            {
                MessageBox.Show(errormsg.ToString(), "提示", MessageBoxButton.OK);
            }

            return isSuccess;
        }
    }
}
