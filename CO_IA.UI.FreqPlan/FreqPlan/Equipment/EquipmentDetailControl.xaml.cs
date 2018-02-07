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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CO_IA.UI.FreqPlan
{
    /// <summary>
    /// EquipmentDetailControl.xaml 的交互逻辑
    /// </summary>
    public partial class EquipmentDetailControl : UserControl
    {
        public ActivityEquipmentInfo CurrentEquipmentInfo { get; set; }

        public ActivityEquipmentInfo OriginalEquipment { get; set; }

        private static void OriginalEquipmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as EquipmentDetailControl).CurrentEquipmentInfo = SettingHelper.Clone<ActivityEquipmentInfo>(e.NewValue as ActivityEquipmentInfo);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public EquipmentDetailControl()
        {
            InitializeComponent();

            this.DataContextChanged += EquipmentDetailControl_DataContextChanged;

            List<ActivityORGInfo> cmb_OrgList = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan, List<ActivityORGInfo>>(channel =>
            {
                return channel.GetORGInfos(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
            });

            this.cmb_OrgList.ItemsSource = cmb_OrgList;
            this.cmb_OrgList.ValueMember = "Guid";
            this.cmb_OrgList.DisplayMember = "Name";
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

         

        }

 

        public EquipmentDetailControl(ActivityEquipmentInfo  equipInfo)
        {
            InitializeComponent();

            List<ActivityORGInfo> cmb_OrgList = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan, List<ActivityORGInfo>>(channel =>
            {
                return channel.GetORGInfos(OriginalEquipment.ActivityGuid);
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

            this.DataContextChanged += EquipmentDetailControl_DataContextChanged;
            this.DataContext = equipInfo;

        }

        private void EquipmentDetailControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            OriginalEquipment = e.NewValue as ActivityEquipmentInfo;
            CurrentEquipmentInfo = SettingHelper.Clone<ActivityEquipmentInfo>(OriginalEquipment);
        }

        /// <summary>
        /// 选择单位改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmb_OrgList_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            //if (cmb_OrgList.SelectedItem != null)
            //{

            //    ActivityORGInfo orginfo = cmb_OrgList.SelectedItem as ActivityORGInfo;

            //    if (OriginalEquipment != null)
            //    {
            //        OriginalEquipment.OrgInfo = orginfo;
            //    }
            //    if (CurrentEquipmentInfo != null)
            //    {
            //        CurrentEquipmentInfo.OrgInfo = orginfo;
            //    }
            //}
        }

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
                this.txt_UnitName.Visibility = System.Windows.Visibility.Collapsed;            
            }
            else
            {
                cmb_OrgList.Visibility = System.Windows.Visibility.Collapsed;
                this.txt_UnitName.Visibility = System.Windows.Visibility.Hidden;
            }
        }
    }
}
