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
    /// EquList.xaml 的交互逻辑
    /// </summary>
    public partial class EquList : UserControl
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
        public EquList()
        {
            InitializeComponent();
        }

        void LoadData()
        {
            dataGridMonitorEquipment.ItemsSource = null;
            if (!string.IsNullOrEmpty(OrgID))
            {
                dataGridMonitorEquipment.ItemsSource = null;
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                {
                    List<MonitorStationEquInfo> itemEquipList = channel.GetPP_EqupInfos(OrgID);
                    dataGridMonitorEquipment.ItemsSource = itemEquipList;
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
                if (nodes!=null&&nodes.Count > 0)
                {
                    List<MonitorStationEquInfo> itemEquipList = new List<MonitorStationEquInfo>();
                    List<MonitorStationEquInfo> EquipList;
                    foreach (var org in nodes)
                    {
                        PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                        {
                            EquipList = channel.GetPP_EqupInfos(org.GUID);
                            if (EquipList != null&&EquipList.Count>0)
                                itemEquipList.AddRange(EquipList);
                        });
                    }
                    this.dataGridMonitorEquipment.ItemsSource = itemEquipList;
                }
            }
        }
        private void dataGridMonitorEquipment_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DataGridRow dgr = DataGridRow.GetRowContainingElement(e.OriginalSource as FrameworkElement);
                if (dgr != null)
                {
                    var monitorEquipnent = dgr.DataContext as MonitorStationEquInfo;
                    if (monitorEquipnent != null)
                    {
                        //MonitorStationEquDialog dialog = new MonitorStationEquDialog(monitorEquipnent);
                        //dialog.Title = "便携式设备信息";
                        //dialog.IsShowDetail = true;
                        //dialog.ShowDialog(this);
                    }
                }
            }
        }
    }
}
