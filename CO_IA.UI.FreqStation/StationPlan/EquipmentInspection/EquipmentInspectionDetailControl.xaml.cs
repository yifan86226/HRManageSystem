using CO_IA.Client;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CO_IA.UI.FreqStation.StationPlan
{
    /// <summary>
    /// EquipmentCheckDetailControl.xaml 的交互逻辑
    /// </summary>
    public partial class EquipmentInspectionDetailControl : UserControl
    {
        public CheckStateEnum CheckState
        {
            get
            {
                if (rabInspection.IsChecked.Value)
                {
                    return CheckStateEnum.Qualified;
                }
                else if (rabUnInspection.IsChecked.Value)
                {
                    return CheckStateEnum.UnQualified;
                }
                return CheckStateEnum.UnCheck;
            }
        }

        public EquipmentInspectionDetailControl()
        {
            InitializeComponent();
            DataContextChanged += EquipmentInspectionDetailControl_DataContextChanged;
            InitData();
            this.txtlable.Text = string.Format("{0}活动无线电发射设备检测申请表", RiasPortal.ModuleContainer.Activity.Name);
        }

        private void InitData()
        {
            //Dictionary<CheckStateEnum, string> source = new Dictionary<CheckStateEnum, string>();
            //source.Add(CheckStateEnum.Qualified, "检测通过");
            //source.Add(CheckStateEnum.UnQualified, "检测未通过");
            //comInspectionResult.ItemsSource = source;

        }

        void EquipmentInspectionDetailControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            EquipmentInspection equinspection = this.DataContext as EquipmentInspection;
            if (equinspection != null)
            {
                if (equinspection.CheckState == CheckStateEnum.Qualified)
                {
                    this.rabInspection.IsChecked = true;
                    this.rabUnInspection.IsChecked = false;
                }
                else if (equinspection.CheckState == CheckStateEnum.UnQualified)
                {
                    this.rabUnInspection.IsChecked = true;
                    this.rabInspection.IsChecked = false;
                }
                else
                {
                    this.rabInspection.IsChecked = false;
                    this.rabUnInspection.IsChecked = false;
                }
            }
        }
    }
}
