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

namespace CO_IA.UI.PersonSchedule.Foreign
{
    /// <summary>
    /// VehicleList.xaml 的交互逻辑
    /// </summary>
    public partial class VehicleList : UserControl
    {
        private string orgId;
        public string OrgID
        {
            get
            {
                return orgId;
            }
            set
            {
                orgId = value;
                LoadData();
            }
        }
        public VehicleList()
        {
            InitializeComponent();
        }

        void LoadData()
        {
            dg_VehicleList.ItemsSource = null;
            if (!string.IsNullOrEmpty(OrgID))
            {
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                {
                    //更新当前节点
                    PP_VehicleInfo itemVehicle = channel.GetPP_VehicleInfo(OrgID);
                    this.dg_VehicleList.ItemsSource = new List<PP_VehicleInfo>() { itemVehicle };
                });
            }
            else
            {
                List<PP_OrgInfo> nodes = new List<PP_OrgInfo>();
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                {
                    //更新当前节点
                    nodes = channel.GetPP_OrgInfos(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
                });
                if (nodes != null && nodes.Count > 0)
                {
                    List<PP_VehicleInfo> itemVehicleList = new List<PP_VehicleInfo>();
                    PP_VehicleInfo VehicleList;
                    foreach (var org in nodes)
                    {
                        PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                        {
                            VehicleList = channel.GetPP_VehicleInfo(org.GUID);
                            if (VehicleList != null && !string.IsNullOrEmpty(VehicleList.VEHICLE_NUMB))
                                itemVehicleList.Add(VehicleList);
                        });
                    }
                    this.dg_VehicleList.ItemsSource = itemVehicleList;
                }


            }
        }
        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DataGridRow dgr = DataGridRow.GetRowContainingElement(e.OriginalSource as FrameworkElement);
                if (dgr != null)
                {
                    var VehicleInfo = dgr.DataContext as PP_VehicleInfo;
                    if (VehicleInfo != null)
                    {
                        VehicleInfoDialog dialog = new VehicleInfoDialog(VehicleInfo);
                        dialog.Title = "便携式设备信息";
                        dialog.ShowDialog(this);
                    }
                }
            }
        }
    }
}
