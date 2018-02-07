#region 文件描述
/**********************************************************************************
 * 创建人：niext
 * 摘  要：车辆查询窗体
 * 日  期：2017-5-11
 * ********************************************************************************/
#endregion

using CO_IA.Client;
using CO_IA.Data;
using PT.Profile.Definition;
using PT.Profile.Interface;
using PT.Profile.Types;
using PT_BS_Service.Client.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;

namespace CO_IA.UI.PlanDatabase.Vehicle
{
    /// <summary>
    /// VehicleQueryDialog.xaml 的交互逻辑
    /// </summary>
    public partial class VehicleQueryDialog : Window
    {
        public event Action<VehicleQueryCondition> OnQueryEvent;

        public bool IsMonitorEnable
        {
            get
            {
                return lstboxType.IsEnabled;
            }
            set
            {
                lstboxType.IsEnabled = value;
            }
        }

        private VehicleQueryCondition CurrentVehicleQueryCondition
        {
            get;
            set;
        }

        private VehicleQueryCondition OriginalVehicleQueryCondition
        {
            get;
            set;
        }

        public VehicleQueryDialog(VehicleQueryCondition condition)
        {
            InitializeComponent();
            CurrentVehicleQueryCondition = condition;
            OriginalVehicleQueryCondition = AT_BC.Data.Helpers.DataContractSerializeHelper.Clone<VehicleQueryCondition>(CurrentVehicleQueryCondition);
            this.DataContext = CurrentVehicleQueryCondition;
            InitData();
            InitCondition(CurrentVehicleQueryCondition);
        }

        private void InitData()
        {
            lstboxArea.ItemsSource = Utility.GetProvinceAreaCode();
            Dictionary<int, string> typesource = new Dictionary<int, string>();
            typesource.Add(1, "监测车");
            typesource.Add(0, "非监测车");
            lstboxType.ItemsSource = typesource;
        }

        /// <summary>
        /// 初始化查询条件
        /// </summary>
        private void InitCondition(VehicleQueryCondition queryCondition)
        {
            lstboxArea.UnSelectAll();
            lstboxType.UnSelectAll();
            if (queryCondition.AreaCodes.Count > 0)
            {
                List<string> areacode = queryCondition.AreaCodes;
                foreach (KeyValuePair<string, string> item in lstboxArea.ItemsSource as Dictionary<string, string>)
                {
                    if (areacode.Contains(item.Key))
                    {
                        lstboxArea.SelectedItems.Add(item);
                    }
                }
            }
            if (queryCondition.VehicleTypes.Count > 0)
            {
                List<int> types = queryCondition.VehicleTypes;
                foreach (KeyValuePair<int, string> item in lstboxType.ItemsSource as Dictionary<int, string>)
                {
                    if (types.Contains(item.Key))
                    {
                        lstboxType.SelectedItems.Add(item);
                    }
                }
            }
        }

        private List<string> GetAreas()
        {
            List<string> areas = new List<string>();
            if (lstboxArea.SelectedItems.Count > 0)
            {
                foreach (KeyValuePair<string, string> item in lstboxArea.SelectedItems)
                {
                    areas.Add(item.Key);
                }
            }
            return areas;
        }
        
        private List<int> GetMonitorType()
        {
            List<int> types = new List<int>();
            if (lstboxType.SelectedItems.Count > 0)
            {
                foreach (KeyValuePair<int, string> item in lstboxType.SelectedItems)
                {
                    types.Add(item.Key);
                }
            }
            return types;
        }

        /// <summary>
        /// 重置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            CurrentVehicleQueryCondition.VehicleNo = null;
            CurrentVehicleQueryCondition.VehicleModel = null;
            
            lstboxArea.UnSelectAll();
            lstboxType.UnSelectAll();
            //InitCondition(OriginalVehicleQueryCondition);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            CurrentVehicleQueryCondition.AreaCodes = GetAreas();
            CurrentVehicleQueryCondition.VehicleTypes = GetMonitorType();
            if (OnQueryEvent != null)
            {
                OnQueryEvent(CurrentVehicleQueryCondition);
            }
            this.Close();
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
