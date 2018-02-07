#region 文件描述
/**********************************************************************************
 * 创建人：Niext
 * 摘  要：参会台站管理
 * 日  期：2016-09-01
 * ********************************************************************************/
#endregion
using CO_IA.Data;
using CO_IA.UI.FreqPlan;
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

namespace CO_IA.UI.FreqQuery
{
    /// <summary>
    /// StationQueryControl.xaml 的交互逻辑
    /// </summary>
    public partial class StationQueryControl : UserControl
    {
        EquipmentCheckQueryCondition condition = new EquipmentCheckQueryCondition() 
        {
            ActivityGuid = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid
        };

        public string PlaceGuid
        {
            get { return (string)GetValue(PlaceGuidProperty); }
            set { SetValue(PlaceGuidProperty, value); }
        }

        public static readonly DependencyProperty PlaceGuidProperty =
            DependencyProperty.Register("PlaceGuid", typeof(string), typeof(StationQueryControl), new PropertyMetadata(new PropertyChangedCallback(PlaceChanged)));

        public StationQueryControl()
        {
            InitializeComponent();
            //GetQualifiedEquipment();
        }
        private static void PlaceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StationQueryControl querycontrol = d as StationQueryControl;
            querycontrol.condition.PlaceGuid = e.NewValue == null ? null : e.NewValue.ToString();
            querycontrol.GetQualifiedEquipment();
        }


        private void xEquQuery_Click(object sender, RoutedEventArgs e)
        {
            QueryEquipListDialog dialog = new QueryEquipListDialog();
            dialog.ShowDialog(this);
            if (dialog.IsSuccuessFull == true)
            {
                EquipmentQueryCondition backcondition = dialog.Condition;
                EquipmentCheckQueryCondition equchkCondition = new EquipmentCheckQueryCondition();
                //equchkCondition.ActivityGuid = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid;
                equchkCondition.PlaceGuid = this.PlaceGuid;

                equchkCondition.ORGguid = backcondition.ORGguid;
                equchkCondition.ORGName = backcondition.ORGName;
                equchkCondition.EuqName = backcondition.EuqName;
                equchkCondition.SendFreqLittle = backcondition.SendFreqLittle;
                equchkCondition.SendFreqGreat = backcondition.SendFreqGreat;
                equchkCondition.BandLittle = dialog.Condition.SendCondition.BandLitte;
                equchkCondition.BandGreat = dialog.Condition.SendCondition.BandGreat;
                equchkCondition.CheckState = CheckStateEnum.Qualified;

                condition = equchkCondition;
            }
            GetQualifiedEquipment();
        }

        private void GetQualifiedEquipment()
        {
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.StationPlan.I_CO_IA_StationPlan>(channel =>
            {
                EquipmentCheck[] equs = channel.GetEquipmentCheck(condition);
                //if (equs == null || equs.Length == 0)
                //{
                //    checkEquListControl.EquipmentCheckItemsSource = new ObservableCollection<EquipmentCheck>();
                //}
                //else
                //{
                //    checkEquListControl.EquipmentCheckItemsSource = new ObservableCollection<EquipmentCheck>(equs);
                //}
            });
        }
    }
}
