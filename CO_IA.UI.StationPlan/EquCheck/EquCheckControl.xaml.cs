#region 文件描述
/**********************************************************************************
 * 创建人：Niext
 * 摘  要：设备检测控件
 * 日  期：2016-09-06
 * ********************************************************************************/
#endregion
using CO_IA.Data;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CO_IA.Client;
using System.Collections.Generic;
using System.Text;

namespace CO_IA.UI.StationPlan
{
    /// <summary>
    /// EquCheck_Control.xaml 的交互逻辑
    /// </summary>
    public partial class EquCheckControl : UserControl
    {

        EquipmentCheckQueryCondition querycondition = new EquipmentCheckQueryCondition()
        {
            ActivityGuid = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid,
        };

        private EquipmentCheck CurrentEquipmentCheck
        {
            get { return checkEquListControl.SelectedEquipmentCheck; }
            set { checkEquListControl.SelectedEquipmentCheck = value; }
        }

        public string PlaceGuid
        {
            get { return (string)GetValue(PlaceGuidProperty); }
            set { SetValue(PlaceGuidProperty, value); }
        }

        public static readonly DependencyProperty PlaceGuidProperty =
            DependencyProperty.Register("PlaceGuid", typeof(string), typeof(EquCheckControl), new PropertyMetadata(new PropertyChangedCallback(PlaceChangeCallBack)));

        public EquCheckControl()
        {
            InitializeComponent();
            this.DataContext = this;
        }


        private static void PlaceChangeCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EquCheckControl currentcontrol = d as EquCheckControl;
            currentcontrol.querycondition.PlaceGuid = e.NewValue == null ? null : e.NewValue.ToString();
            currentcontrol.OnQueryEquCheck();
        }


        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnQuery_Click(object sender, RoutedEventArgs e)
        {
            EquCheckQueryDialog dialog = new EquCheckQueryDialog(querycondition);
            dialog.OnQueryEvent += OnQueryEquCheckEvent;
            dialog.ShowDialog(this);
        }

        private void OnQueryEquCheckEvent(EquipmentCheckQueryCondition obj)
        {
            querycondition = obj;
            OnQueryEquCheck();
        }

        private EquipmentCheck[] OnQueryEquCheck()
        {
            querycondition.PlaceGuid = this.PlaceGuid;
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.StationPlan.I_CO_IA_StationPlan, EquipmentCheck[]>(channel =>
              {
                  EquipmentCheck[] equs = channel.GetEquipmentCheck(querycondition);
                  if (equs == null)
                  {
                      checkEquListControl.EquipmentCheckItemsSource = new ObservableCollection<EquipmentCheck>();
                  }
                  else
                  {
                      checkEquListControl.EquipmentCheckItemsSource = new ObservableCollection<EquipmentCheck>(equs);
                  } return equs;
              });
        }

        /// <summary>
        /// 检测通过
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnQualified_Click(object sender, RoutedEventArgs e)
        {
            List<EquipmentCheck> eques = GetCheckEqus();
            if (eques.Count == 0)
            {
                MessageBox.Show("请选择要进行检测的设备!", "提示", MessageBoxButton.OK);
            }
            else
            {
                foreach (EquipmentCheck item in eques)
                {
                    item.CheckState = CheckStateEnum.Qualified;
                }
                BatchCheckEquipment(eques);
            }
        }

        /// <summary>
        /// 检测不通过
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnUnQualified_Click(object sender, RoutedEventArgs e)
        {
            List<EquipmentCheck> eques = GetCheckEqus();
            if (eques.Count == 0)
            {
                MessageBox.Show("请选择要进行检测的设备!", "提示", MessageBoxButton.OK);
            }
            else
            {
                StringBuilder strmsg = new StringBuilder();
                foreach (EquipmentCheck item in eques)
                {
                    if (string.IsNullOrEmpty(item.Remark))
                    {
                        strmsg.AppendFormat("设备{0},检测不通过理由不能为空! \r", item.Equipment.Name);
                    }
                }

                if (!string.IsNullOrEmpty(strmsg.ToString()))
                {
                    MessageBox.Show(strmsg.ToString(), "提示", MessageBoxButton.OK);
                    return;
                }
                else
                {
                    foreach (EquipmentCheck item in eques)
                    {
                        item.CheckState = CheckStateEnum.UnQualified;
                    }
                    BatchCheckEquipment(eques);
                }
            }
        }

        /// <summary>
        /// 发放许可证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSendLicense_Click(object sender, RoutedEventArgs e)
        {
            List<EquipmentCheck> chkeques = GetCheckEqus();
            if (chkeques.Count > 1 || chkeques.Count == 0)
            {
                MessageBox.Show("请选择一条要发放许可证的设备!", "提示", MessageBoxButton.OK);
                return;
            }
            else
            {
                EquipmentCheck chkequ = chkeques[0];
                if (chkequ.CheckState != CheckStateEnum.Qualified)
                {
                    MessageBox.Show("设备未通过检测,不能发放许可证", "提示", MessageBoxButton.OK);
                    return;
                }
                else
                {
                    //chkequ.SendLicense = SendLicenseEnum.SendLicense;
                    //List<ActivityEquipmentInfo> equs = new List<ActivityEquipmentInfo>();
                    //equs.Add(chkequ.Equipment);

                    //SendLicense(chkequ, equs);
                }
            }
        }

        /// <summary>
        /// 批量发放许可证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBatchSendLicense_Click(object sender, RoutedEventArgs e)
        {
            List<EquipmentCheck> items = GetCheckEqus();
            if (items.Count == 0)
            {
                MessageBox.Show("请选择要批量发放许可证的设备!", "提示", MessageBoxButton.OK);
                return;
            }
            else
            {
                StringBuilder strmsg = new StringBuilder();
                foreach (EquipmentCheck item in items)
                {
                    if (item.CheckState != CheckStateEnum.Qualified)
                    {
                        strmsg.AppendFormat("设备{0}未通过检测,不可以发放许可证 \r", item.Equipment.Name);
                    }
                    else
                    {
                        item.SendLicense = SendLicenseEnum.SendLicense;
                    }
                }

                if (!string.IsNullOrEmpty(strmsg.ToString()))
                {
                    MessageBox.Show(strmsg.ToString(), "提示");
                    return;
                }
                else
                {
                    List<ActivityEquipmentInfo> equs = new List<ActivityEquipmentInfo>();
                    //foreach (EquipmentCheck item in items)
                    //{
                    //    item.SendLicense = SendLicenseEnum.SendLicense;
                    //    equs.Add(item.Equipment);
                    //}
                    //BatchSendLicense(items, equs);
                }
            }
        }

        /// <summary>
        /// 许可证模板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLicenseTemplate_Click(object sender, RoutedEventArgs e)
        {
            //CO_IA.UI.StationPlan.LicenseSend licesend = new CO_IA.UI.StationPlan.LicenseSend();
            //licesend.GoBack += () => { this.borderContent.Visibility = Visibility.Collapsed; };
            //this.borderContent.Visibility = Visibility.Visible;
            //this.borderContent.Child = null;
            //this.borderContent.Child = licesend;
            LicenseTempleteDialog dialog = new LicenseTempleteDialog();
            dialog.Show();
        }

        private List<EquipmentCheck> GetCheckEqus()
        {
            return checkEquListControl.EquipmentCheckItemsSource.Where(r => r.IsChecked == true).ToList();
        }

        /// <summary>
        /// 批量检测
        /// </summary>
        private void BatchCheckEquipment(List<EquipmentCheck> equs)
        {
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.StationPlan.I_CO_IA_StationPlan>(channel =>
            {
                try
                {
                    channel.BatchSaveEquipmentCheck(equs);
                    MessageBox.Show("设备检测完成!", "提示", MessageBoxButton.OK);
                    OnQueryEquCheck();
                    checkEquListControl.CancelChKAll();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.GetExceptionMessage());
                }
            });
        }

        /// <summary>
        /// 发放许可证
        /// </summary>
        private void SendLicense(EquipmentCheck chkequ, List<ActivityEquipmentInfo> equs)
        {
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.StationPlan.I_CO_IA_StationPlan>(channel =>
            {
                try
                {
                    channel.SaveEquipmentCheck(chkequ);
                    MessageBox.Show("许可证发放成功!", "提示", MessageBoxButton.OK);
                    OnQueryEquCheck();
                    checkEquListControl.CancelChKAll();
                    PrintLicense(equs);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.GetExceptionMessage());
                }
            });
        }

        /// <summary>
        /// 批量发放许可证
        /// </summary>
        private void BatchSendLicense(List<EquipmentCheck> equchks, List<ActivityEquipmentInfo> equs)
        {
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.StationPlan.I_CO_IA_StationPlan>(channel =>
            {
                try
                {
                    channel.BatchSaveEquipmentCheck(equchks);
                    MessageBox.Show("许可证发放成功!", "提示", MessageBoxButton.OK);
                    OnQueryEquCheck();
                    checkEquListControl.CancelChKAll();
                    PrintLicense(equs);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.GetExceptionMessage());
                }
            });
        }

        /// <summary>
        /// 打印许可证
        /// </summary>
        /// <param name="equs"></param>
        private void PrintLicense(List<ActivityEquipmentInfo> equs)
        {
            PrintPreviewLicense printlicense = new PrintPreviewLicense(equs);
            if (printlicense._licenseTempleteInfo != null)
            {
                printlicense.ShowInTaskbar = false;
                printlicense.ShowDialog(this);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.StationPlan.I_CO_IA_StationPlan>(channel =>
            {
                Dictionary<string, int> equs = channel.StatisticEquipmentCheck(querycondition.ActivityGuid, querycondition.PlaceGuid);
                if (equs != null)
                {
 
                }
             });
        }
    }
}
