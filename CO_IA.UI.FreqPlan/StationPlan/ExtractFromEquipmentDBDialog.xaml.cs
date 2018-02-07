#region 文件描述
/**********************************************************************************
 * 创建人：Liuenliang
 * 摘  要：从设备库查询并导入数据
 * 日  期：2016-08-17
 * ********************************************************************************/
#endregion

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
using System.Windows.Shapes;
using AT_BC.Data.Helpers;
namespace CO_IA.UI.FreqPlan.StationPlan
{
    /// <summary>
    /// ExtractFromEquipmentDBDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ExtractFromEquipmentDBDialog : Window
    {
        #region 变量集合

        private EquipmentQueryCondition querycondition = new EquipmentQueryCondition() { IsStation = true, IsTunAble = true, IsMobile = true };
        private EquipmentQueryCondition QueryCondition
        {
            get { return querycondition; }
            set { querycondition = value; }
        }


        ObservableCollection<EquipmentInfo> equipmentItemsSource;
        //private CheckBox chkAll;

        public EquipmentInfo[] EquipmentItemsSource
        {
            get { return equdatagrid.ItemsSource as EquipmentInfo[]; }
            set { equdatagrid.ItemsSource = value; }
        }

        public bool _showCompany;

        public bool ShowCompany
        {
            get
            {
                return _showCompany;
            }
            set
            {
                _showCompany = value;
                if (_showCompany)
                {
                    columnCompany.Visibility = Visibility.Visible;
                }
                else
                {
                    columnCompany.Visibility = Visibility.Collapsed;
                }
            }
        }
        public EquipmentInfo SelectedEquipmentDialog
        {
            get { return (EquipmentInfo)GetValue(SelectedEquipmentProperty); }
            set { SetValue(SelectedEquipmentProperty, value); }
        }

        public static readonly DependencyProperty SelectedEquipmentProperty =
            DependencyProperty.Register("SelectedEquipmentDialog", typeof(EquipmentInfo), typeof(EquipmentListControl),
            new PropertyMetadata(new PropertyChangedCallback(SelectedEquipmentChangedCallback)));

        private static void SelectedEquipmentChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }


        //CO_IA.UI.Setting.Equipment.EquipmentQueryDialog querydialog;
        private ActivityPlace activityPlaceInfo;



        public event Action RefreshEquipmentSource;
#endregion

        /// <summary>
        /// 初始化信息
        /// </summary>
        public ExtractFromEquipmentDBDialog()
        {
            InitializeComponent();
            this.Loaded += ExtractFromEquipmentDBDialog_Loaded;
        
        }

        public ExtractFromEquipmentDBDialog(ActivityPlace activityPlaceInfo)
        {
            InitializeComponent();
            this.Loaded += ExtractFromEquipmentDBDialog_Loaded;
            this.activityPlaceInfo = activityPlaceInfo;
        }
        /// <summary>
        /// Load数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExtractFromEquipmentDBDialog_Loaded(object sender, RoutedEventArgs e)
        {
            //querydialog = new Setting.Equipment.EquipmentQueryDialog(QueryCondition,true);
            //querydialog.OnQueryEvent += querydialog_OnQueryEvent;
            //querydialog.ShowDialog(this);
        }

        /// <summary>
        /// 双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void equdatagrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //CO_IA.UI.Setting.Equipment.EquipmentDetailDialog dialog = new CO_IA.UI.Setting.Equipment.EquipmentDetailDialog(SelectedEquipmentDialog,false);
            //dialog.IsEnabled = false;
            //dialog.WindowTitle = "设备-详细信息";
            //dialog.ShowDialog(this);
        }

        /// <summary>
        /// 换行选择事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void equdatagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.equdatagrid.SelectedItem != null)
            {
                SelectedEquipmentDialog = this.equdatagrid.SelectedItem as EquipmentInfo;
            }
        }


        private void querydialog_OnQueryEvent(Data.EquipmentQueryCondition obj)
        {
            QueryCondition = obj;
            GetEquipments(QueryCondition);
        }

        private void GetEquipments(EquipmentQueryCondition condition)
        {
            //PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.Setting.I_CO_IA_Setting>(channel =>
            //{
            //    EquipmentItemsSource = channel.GetEquipmentInfos(condition);
            //});
        }

        /// <summary>
        /// 查询按键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnQuery_Click(object sender, RoutedEventArgs e)
        {
            //querydialog = new Setting.Equipment.EquipmentQueryDialog(QueryCondition,true);
            //querydialog.OnQueryEvent += querydialog_OnQueryEvent;
            //querydialog.ShowDialog(this);
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            List<ActivityEquipmentInfo> aEquipList = new List<ActivityEquipmentInfo>();
            if (EquipmentItemsSource == null) return;
            foreach (EquipmentInfo info in EquipmentItemsSource)
            {
                if (info.IsChecked == true)
                {
                    ActivityEquipmentInfo tempEuipInfo = ConvertToActivityEquipmentInfo(info);

                    aEquipList.Add(tempEuipInfo);
                }
            }

            bool result = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan, bool>(channel =>
                {
                    return channel.SaveEquipmentList(aEquipList);
                });
      
            this.Close();

            if (RefreshEquipmentSource != null)
            {
                RefreshEquipmentSource();
            }
        }

        /// <summary>
        /// 转换相关设备类
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private ActivityEquipmentInfo ConvertToActivityEquipmentInfo(EquipmentInfo tempInfo)
        {
            #region  保存组织信息
            ORGInfo orginfo = new ORGInfo();
            //PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.Setting.I_CO_IA_Setting>(channel =>
            //{
            //    orginfo = channel.GetORGInfoByGuid(tempInfo.ORG.Guid);
            //});

            ActivityORGInfo activityOrginfo = ConvertToActivityORGInfo(orginfo);


            ActivityORGInfo tempOrginfo = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan, ActivityORGInfo>(channel =>
            {
                return channel.GetORGInfoByActivityORGInfo(activityOrginfo);
            });


            if (tempOrginfo != null && string.IsNullOrEmpty(tempOrginfo.Activity_Guid) == false)
            {
                activityOrginfo = tempOrginfo;
            }

            bool result = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan, bool>(channel =>
              {
                  return channel.SaveORGInfo(activityOrginfo);
              });
            #endregion


            ActivityEquipmentInfo aeuip = new ActivityEquipmentInfo();
            aeuip.GUID =CO_IA.Client.Utility.NewGuid();
            aeuip.ActivityGuid = Client.RiasPortal.ModuleContainer.Activity.Guid;
            aeuip.PlaceGuid = activityPlaceInfo.Guid;
            aeuip.ORGGuid = tempInfo.ORG.Guid;


            aeuip.ADJChannelInh = tempInfo.RecivePara.ADJChannelInh;
            //aeuip.AssignFreq = tempInfo.RecivePara.Ant.AssignFreq;
            aeuip.Band = tempInfo.SendPara.Band;
            aeuip.ChannelBand = tempInfo.SendPara.ChannelBand;
            aeuip.CoChnPro = tempInfo.RecivePara.CoChnPro;
            aeuip.EQUCount = tempInfo.EQUCount;
            aeuip.EquModel = tempInfo.EquModel;
            aeuip.EquNo = tempInfo.EquNo;
            //aeuip.IsChecked = tempInfo.RecivePara.ADJChannelInh;
            aeuip.IsMobile = tempInfo.IsMobile;
            aeuip.IsStation = tempInfo.IsStation;
            aeuip.IsTunAble = tempInfo.SendPara.IsTunAble;
            aeuip.Leakage = tempInfo.RecivePara.ADJChannelInh;
            aeuip.MaxPower = tempInfo.SendPara.MaxPower;

#warning 需要处理调制方式为空情况
            if (tempInfo.SendPara.ModulateMode.HasValue)
            {
                aeuip.ModulateMode = tempInfo.SendPara.ModulateMode.Value;
            }
            aeuip.Name = tempInfo.Name;
            aeuip.Origin = 1;//设备库录入
            aeuip.ReceiveFreq = tempInfo.RecivePara.ReceiveFreq;
            aeuip.RecvAntAzimuth = tempInfo.RecivePara.Ant.AntAzimuth;
            aeuip.RecvAntElevation = tempInfo.RecivePara.Ant.AntElevation;
            aeuip.RecvAntFeedLength = tempInfo.RecivePara.Ant.FeedLength;
            aeuip.RecvAntFeedLoss = tempInfo.RecivePara.Ant.FeedLose;
            aeuip.RecvAntGain = tempInfo.RecivePara.Ant.AntGain;
            aeuip.RecvAntHeight = tempInfo.RecivePara.Ant.AntHight;
            aeuip.RecvAntModel = tempInfo.RecivePara.Ant.AntModel;
            aeuip.RecvAntPolar = tempInfo.RecivePara.Ant.AntPolar;
            aeuip.RecvFreqEnd = tempInfo.RecivePara.FreqEnd;
            aeuip.RecvFreqStart = tempInfo.RecivePara.FreqStart;
            aeuip.Remark = tempInfo.Remark;
            //aeuip.RunningFrom = tempInfo.RecivePara.ADJChannelInh;
            //aeuip.RunningTo = tempInfo.RecivePara.ADJChannelInh;
            aeuip.SendAntAzimuth = tempInfo.SendPara.Ant.AntAzimuth;
            aeuip.SendAntElevation = tempInfo.SendPara.Ant.AntElevation;
            aeuip.SendAntFeedLength = tempInfo.SendPara.Ant.FeedLength;
            aeuip.SendAntFeedLoss = tempInfo.SendPara.Ant.FeedLose;
            aeuip.SendAntGain = tempInfo.SendPara.Ant.AntGain;
            aeuip.SendAntHeight = tempInfo.SendPara.Ant.AntHight;
            aeuip.SendAntModel = tempInfo.SendPara.Ant.AntModel;
            aeuip.SendAntPolar = tempInfo.SendPara.Ant.AntPolar;
            aeuip.SendFreq = tempInfo.SendPara.SendFreq;
            aeuip.SendFreqEnd = tempInfo.SendPara.FreqEnd;
            aeuip.SendFreqStart = tempInfo.SendPara.FreqStart;
            aeuip.Sensitivity = tempInfo.RecivePara.Sensitivity.GetNullableDouble();
            aeuip.SensitivityUnit = tempInfo.RecivePara.SensitivityUnit.GetNullableInt();
            aeuip.SignalNoise = tempInfo.RecivePara.SignalNoise;
            aeuip.StationName = tempInfo.StationName;
            aeuip.BusinessCode = tempInfo.StClassCode;


            return aeuip;
        }

        /// <summary>
        /// 对组织信息进行转换
        /// </summary>
        /// <param name="orginfo"></param>
        /// <returns></returns>
        private ActivityORGInfo ConvertToActivityORGInfo(ORGInfo orginfo)
        {
            ActivityORGInfo activityOrginfo = new ActivityORGInfo();

            activityOrginfo.Guid = orginfo.Guid;
            activityOrginfo.Activity_Guid = Client.RiasPortal.ModuleContainer.Activity.Guid;
            activityOrginfo.Address = orginfo.Address.Trim();
            activityOrginfo.Contact = orginfo.Contact.Trim();
            activityOrginfo.Name = orginfo.Name.Trim();
            activityOrginfo.Phone = orginfo.Phone.Trim();
            activityOrginfo.Securityclass.Guid = orginfo.Securityclass.Guid.Trim();
            activityOrginfo.Securityclass.Name = orginfo.Securityclass.Name.Trim();
            activityOrginfo.ShortName = orginfo.ShortName.Trim();


            return activityOrginfo;
        }




        #region  全选/全取消事件
        private void chkAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            bool ischecked = chk.IsChecked.Value;

            foreach (EquipmentInfo item in EquipmentItemsSource)
            {
                item.IsChecked = ischecked;
            }
        }

        private void chkAll_Loaded(object sender, RoutedEventArgs e)
        {
            CheckBox chkAll = sender as CheckBox;
            if (this.EquipmentItemsSource != null)
            {
                chkAll.IsChecked = EquipmentItemsSource.Any(item => item.IsChecked);
            }
        }

        private void chkCell_Click(object sender, RoutedEventArgs e)
        {

            //bool? isChecked = (sender as CheckBox).IsChecked;
            //if (!isChecked.HasValue)
            //{
            //    return;
            //}
            //bool checkedState = isChecked.Value;



            //foreach (var item in equdatagrid.Items)
            //{
            //    DataGridTemplateColumn templeColumn = equdatagrid.Columns[0] as DataGridTemplateColumn;
            //    FrameworkElement fwElement = equdatagrid.Columns[0].GetCellContent(item);
            //    if (fwElement != null)
            //    {
            //        CheckBox chkAll = templeColumn.HeaderTemplate.FindName("chkAll", fwElement) as CheckBox;
            //        if (chkAll != null)
            //        {
            //            foreach (EquipmentInfo result in EquipmentItemsSource)
            //            {
            //                if (result.IsChecked != checkedState)
            //                {
            //                    chkAll.IsChecked = null;
            //                    return;
            //                }
            //            }
            //            chkAll.IsChecked = checkedState;
            //            break;
            //        }
            //    }
            //}
        }

        #endregion

    }
}
