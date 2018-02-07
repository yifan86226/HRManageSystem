using CO_IA.Data;
using I_CO_IA.PlanDatabase;
using Microsoft.Win32;
using PT_BS_Service.Client.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace CO_IA.UI.PlanDatabase.Vehicle
{
    /// <summary>
    /// CarList.xaml 的交互逻辑
    /// </summary>
    public partial class VehicleManageModule : UserControl
    {
        private CheckBox chkAll;

        public VehicleInfo[] VehicleItemsSource
        {
            get { return (VehicleInfo[])GetValue(VehicleItemsSourceProperty); }
            set { SetValue(VehicleItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty VehicleItemsSourceProperty =
            DependencyProperty.Register("VehicleItemsSource", typeof(VehicleInfo[]), typeof(VehicleManageModule), new PropertyMetadata(null, null));

        public VehicleInfo VehicleSelected
        {
            get
            {
                if (cardatagrid.SelectedItem == null)
                {
                    return null;
                }
                else
                {
                    return cardatagrid.SelectedItem as VehicleInfo;
                }
            }
            set
            {
                cardatagrid.SelectedItem = value;
            }
        }

        private VehicleInfo[] OrginVehicleItemsSource
        {
            get;
            set;
        }

        private VehicleQueryCondition vehiclequerycondition = new VehicleQueryCondition();
        private VehicleQueryCondition VehicleQueryCondition
        {
            get
            {
                return vehiclequerycondition;
            }
            set
            {
                vehiclequerycondition = value;
            }
        }

        public VehicleManageModule()
        {
            InitializeComponent();
            this.DataContext = this;
            OrginVehicleItemsSource = this.GetVehicleInfos();
        }

        private void cardatagrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    int equcount = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_PlanDatabase, int>(channel =>
                    {
                        return channel.GetMonitorEquCount(VehicleSelected.GUID);
                    });

                    VehicleEditDialog dialog = new VehicleEditDialog(VehicleSelected, true);
                    dialog.Title = "修改车辆信息";

                    if (equcount > 0)
                    {
                        MessageBox.Show("车辆在监测实施中,已经存在设备。不可以修改'车牌号码'和是否是'监测车'属性");
                        dialog.ModifyMonitorProperty = false;
                    }
                    else
                    {
                        dialog.ModifyMonitorProperty = true;
                    }
                    dialog.AfterSaveEvent += () =>
                    {
                        GetVehicleInfos();
                    };
                    dialog.ShowDialog(this);
                }
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            VehicleInfo vechicleinfo = new VehicleInfo();

            VehicleEditDialog dialog = new VehicleEditDialog(vechicleinfo, false);
            dialog.ModifyMonitorProperty = true;
            dialog.Title = "添加车辆信息";
            dialog.AfterSaveEvent += () =>
            {
                GetVehicleInfos();
            };
            dialog.ShowDialog(this);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            int checkcount = VehicleItemsSource.Count(r => r.IsChecked == true);
            if (checkcount == 0)
            {
                MessageBox.Show("请勾选要删除的车辆");
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("确认要删除选中的车辆？", "提示", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        List<string> vehiclenos = (VehicleItemsSource.Where(v => v.IsChecked == true)).Select(r => r.GUID).ToList();
                        PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_PlanDatabase>(channel =>
                        {
                            List<string> errorguid = channel.DeleteVehicleInfo(vehiclenos);
                            if (errorguid.Count == 0)
                            {
                                GetVehicleInfos();
                                MessageBox.Show("删除成功", "提示", MessageBoxButton.OK);
                                chkAll.IsChecked = false;//取消选中
                            }
                            else
                            {
                                StringBuilder strmsg = new StringBuilder();
                                foreach (string item in errorguid)
                                {
                                    VehicleInfo vehicle = VehicleItemsSource.FirstOrDefault(r => r.GUID == item);
                                    vehicle.IsChecked = true;
                                    if (vehicle != null)
                                    {
                                        strmsg.AppendLine(string.Format("车牌号码'{0}',已经有设备不能删除 ", vehicle.VehicleNo));
                                    }
                                }
                                MessageBox.Show(strmsg.ToString(), "删除完成", MessageBoxButton.OK);
                                GetVehicleInfos();
                                //未删除的监测车还是选中状态
                                VehicleItemsSource.Where(r => errorguid.Contains(r.GUID)).ToList().ForEach(v => v.IsChecked = true);
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("违反完整约束条件"))
                        {
                            MessageBox.Show("车辆已经有使用,不能删除!");
                        }
                    }

                }
            }
        }

        #region 全选

        private void chkAll_Click(object sender, RoutedEventArgs e)
        {
            if (VehicleItemsSource != null)
            {
                CheckBox chk = sender as CheckBox;
                bool ischecked = chk.IsChecked.Value;

                foreach (VehicleInfo item in VehicleItemsSource)
                {
                    item.IsChecked = ischecked;
                }
            }
        }

        private void chkAll_Loaded(object sender, RoutedEventArgs e)
        {
            chkAll = sender as CheckBox;
            if (this.VehicleItemsSource != null)
            {
                chkAll.IsChecked = VehicleItemsSource.Any(item => item.IsChecked);
            }
        }

        private void chkCell_Click(object sender, RoutedEventArgs e)
        {
            if (VehicleItemsSource != null)
            {
                int checkcount = VehicleItemsSource.Count(r => r.IsChecked == true);
                if (checkcount == VehicleItemsSource.Length)
                {
                    chkAll.IsChecked = true;
                }
                else if (checkcount == 0)
                {
                    chkAll.IsChecked = false;
                }
                else
                {
                    chkAll.IsChecked = null;
                }
            }
        }

        #endregion

        /// <summary>
        /// 查询车辆信息
        /// </summary>
        private VehicleInfo[] GetVehicleInfos()
        {
            VehicleItemsSource = CO_IA.Client.Utility.GetVehicleInfos(this.VehicleQueryCondition);
            return VehicleItemsSource;
        }

        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            VehicleQueryDialog query = new VehicleQueryDialog(VehicleQueryCondition);
            query.OnQueryEvent += (condition) =>
            {
                VehicleQueryCondition = condition;
                GetVehicleInfos();
            };
            query.ShowDialog();
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnXLSImport_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Excel 文件|*.xls;*.xlsx";
            dialog.DefaultExt = "xls";

            dialog.CheckFileExists = true;
            if (dialog.ShowDialog() == true)
            {
                DataTable[] tables = ExcelImportHelper.LoadDataFromExcel(dialog.FileName);
                if (tables != null && tables.Length > 0)
                {
                    string imgPath = dialog.FileName.Substring(0, dialog.FileName.LastIndexOf('\\') + 1) + "车辆照片";
                    if (Directory.Exists(imgPath))
                    {
                        List<DataRow> rows = VehicleImportHelper.GetEnableDataRow(tables);
                        if (rows != null && rows.Count > 0)
                        {
                            VehicleImportHelper.ImageFilePath = imgPath;
                            List<VehicleInfo> vehicles = null;
                            bool result = false;
                            vehicles = VehicleImportHelper.GetVehicleFromTable(rows, out result);
                            //设备获取成功
                            if (result)
                            {
                                if (vehicles != null && vehicles.Count > 0)
                                {

                                    List<VehicleInfo> samenolst = new List<VehicleInfo>();
                                    //验证在数据库中是否存在相同的车辆编号
                                    if (VehicleImportHelper.VerifyVehicleNotInDB(vehicles, out samenolst))
                                    {
                                        try
                                        {
                                             BeOperationInvoker.Invoke<I_CO_IA_PlanDatabase>
                                                 (channel =>
                                                 {
                                                     channel.ImportVehicles(vehicles);
                                                 });
                                            GetVehicleInfos();
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show(ex.GetExceptionMessage(), "导入失败");
                                        }
                                    }
                                    else
                                    {
                                        StringBuilder errstr = new StringBuilder();
                                        errstr.AppendLine("数据库存在相同的车牌号码:");
                                        foreach (VehicleInfo item in samenolst)
                                        {
                                            errstr.AppendLine(item.VehicleNo);
                                        }
                                        errstr.AppendLine("是否进行替换?");
                                        errstr.AppendLine("说明:如果车辆在监测实施中,存在设备。则不替换'监测车'属性");
                                        MessageBoxResult msgresult = MessageBox.Show(errstr.ToString(), "提示", MessageBoxButton.YesNo);
                                        if (msgresult == MessageBoxResult.Yes)
                                        {
                                            BeOperationInvoker.Invoke<I_CO_IA_PlanDatabase>
                                                (channel =>
                                                {
                                                    channel.ImportVehicles(vehicles);
                                                });
                                            GetVehicleInfos();
                                        }
                                        else
                                        {
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("车辆信息为空,请填写车辆信息", "提示", MessageBoxButton.OK);
                        }
                    }
                    else
                    {
                        MessageBox.Show("车辆照片文件夹不存在,无法导入照片");
                    }
                }
                else
                {
                    MessageBox.Show("车辆信息为空,请填写车辆信息", "提示", MessageBoxButton.OK);
                }
            }
        }

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTemplateDownLoad_Click(object sender, RoutedEventArgs e)
        {
            ExcelImportHelper.TemplateDownLoad("车辆导入模板文件夹.zip", "Template\\Vehicle\\");
        }
    }
}

