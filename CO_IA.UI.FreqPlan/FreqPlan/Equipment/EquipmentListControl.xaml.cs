using CO_IA.Data;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
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
using CO_IA.Client;
using AT_BC.Data;

namespace CO_IA.UI.FreqPlan
{
    /// <summary>
    /// EquipmentListControl.xaml 的交互逻辑
    /// </summary>
    public partial class EquipmentListControl : UserControl
    {
        #region  变量
        ObservableCollection<ActivityEquipmentInfo> equipmentItemsSource;
        EquipmentQueryCondition condition = new EquipmentQueryCondition();

        public List<ActivityEquipmentInfo> EquipmentItemsSource
        {
            get { return dg_equiplist.ItemsSource as List<ActivityEquipmentInfo>; }
            set { dg_equiplist.ItemsSource = null; dg_equiplist.ItemsSource = value; }
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

        public ActivityEquipmentInfo SelectedEquipment
        {
            get { return (ActivityEquipmentInfo)GetValue(SelectedEquipmentProperty); }
            set { SetValue(SelectedEquipmentProperty, value); }
        }

        public static readonly DependencyProperty SelectedEquipmentProperty =
            DependencyProperty.Register("SelectedEquipment", typeof(ActivityEquipmentInfo), typeof(EquipmentListControl),
            new PropertyMetadata(new PropertyChangedCallback(SelectedEquipmentChangedCallback)));

        private static void SelectedEquipmentChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }


        //public ObservableCollection<ActivityEquipmentInfo> EquipmentItemsSource
        //{
        //    get;
        //    set;
        //}
        //UpdateCompanyInfo companyControl;

        ObservableCollection<ORGInfo> companies;

        /// <summary>
        /// 地点信息
        /// </summary>
        private ActivityPlaceInfo activityPlaceInfo;


        #endregion

        //public ActivityEquipmentInfo SelectedEquipment
        //{
        //    get
        //    {
        //        if (equdatagrid.SelectedItem == null)
        //        {
        //            return null;
        //        }
        //        else
        //        {
        //            return equdatagrid.SelectedItem as ActivityEquipmentInfo;
        //        }
        //    }
        //    //set
        //    //{
        //    //    equdatagrid.SelectedItem = value;
        //    //}
        //}

        #region 构造函数

        public EquipmentListControl(ActivityPlaceInfo placeinfo)
        {
            InitializeComponent();
            ShowCompany = true;
            //this.DataContext = this;
            this.activityPlaceInfo = placeinfo;
           
            if (activityPlaceInfo != null && string.IsNullOrEmpty(activityPlaceInfo.Guid) == false)
            {
                condition.PlaceGuid = activityPlaceInfo.Guid;
            }
            condition.ActivityGuid = Client.RiasPortal.ModuleContainer.Activity.Guid;
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan>(channel =>
                {
                    EquipmentItemsSource = channel.GetEquipmentInfos(condition);
                });

            this.dg_equiplist.ItemsSource = EquipmentItemsSource;
        }
        #endregion

        #region 按键功能
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Modify_Click(object sender, RoutedEventArgs e)
        {
            foreach (ActivityEquipmentInfo info in EquipmentItemsSource)
            {
                if (info.IsChecked == true)
                {
                    EquipmentDetailDialog dialog = new UI.FreqPlan.EquipmentDetailDialog(info);
                    dialog.RefreshEquipmentSource += Dialog_RefreshEquipmentSource;
                    dialog.ShowDialog(this);
                    break;
                }
            }
        }

        /// <summary>
        /// 删除信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("确认要删除选择的行", "提示", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                List<ActivityEquipmentInfo> deleteLsit = new List<ActivityEquipmentInfo>();
                foreach (ActivityEquipmentInfo einfo in EquipmentItemsSource)
                {
                    if (einfo.IsChecked == true)
                    {
                        deleteLsit.Add(einfo);
                    }

                }

                if (deleteLsit.Count > 0)
                {
                    DeleteEquipList(deleteLsit);

                    condition = new EquipmentQueryCondition();

                    if (activityPlaceInfo != null && string.IsNullOrEmpty(activityPlaceInfo.Guid) == false)
                    {
                        condition.PlaceGuid = activityPlaceInfo.Guid;
                    }
                    condition.ActivityGuid = Client.RiasPortal.ModuleContainer.Activity.Guid;


                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan>(channel =>
                    {
                        EquipmentItemsSource = channel.GetEquipmentInfos(condition);
                    });

                }
            }
        }


        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void btn_Export_Click(object sender, RoutedEventArgs e)
        //{
        //    SaveFileDialog savedialog = new SaveFileDialog();
        //    savedialog.Filter = "导出文件|*.xls";
        //    if (savedialog.ShowDialog() == true)
        //    {
        //        MessageBox.Show("导出成功!", "提示", MessageBoxButton.OK);
        //    }
        //}


        private void btn_Export_Click(object sender, RoutedEventArgs e)
        {
            string path = CO_IA.Client.ExcelHelper.GetPath();

            if (path != null)
            {
                object[,] source = SourceToObject(EquipmentItemsSource);
                if (source != null)
                {
                    bool exportresult = CO_IA.Client.ExcelHelper.ExportToExcel(source, path, "设备列表");
                    if (exportresult)
                    {
                        MessageBox.Show("导出成功");
                    }
                    else
                    {
                        MessageBox.Show("导出失败");
                    }
                }
                else
                {
                    MessageBox.Show("没有需要导出的设备");
                }
            }
        }


        private object[,] SourceToObject(List<ActivityEquipmentInfo> equs)
        {
            object[,] obj = null;
            int rows = equs.Count;
            ObservableCollection<DataGridColumn> columns = dg_equiplist.Columns;
            int cols = columns.Count;
            obj = new object[rows + 1, cols];

            for (int c = 1; c < cols; c++)
            {
                DataGridColumn column = columns[c];
                obj[0, c - 1] = column.Header;
            }
            for (int r = 0; r < rows; r++)
            {
                ActivityEquipmentInfo equ = equs[r];
                obj[r + 1, 0] = equ.ORG.Name;//单位
                obj[r + 1, 1] = equ.Name;//设备名称
                obj[r + 1, 2] = GetBusinessName(equ.BusinessCode);//业务类型
                obj[r + 1, 3] = equ.EQUCount; //设备数量
                obj[r + 1, 4] = equ.SendFreq;//发射频率
                obj[r + 1, 5] = equ.ReceiveFreq;//接收频率
                obj[r + 1, 6] = equ.IsTunAble == true ? "是" : "否";//频率可调
                obj[r + 1, 7] = equ.Band;//带宽
                obj[r + 1, 8] = equ.MaxPower;//最大功率
                //obj[r + 1, 9] = equ.BusinessCode;//技术体制

            }

            return obj;
        }

        private string GetBusinessName(string businesstype)
        {
            BusinessType type = Utility.BusinessTypes.FirstOrDefault(r => r.Guid == businesstype);
            if (type == null)
            {
                return null;
            }
            else
            {
                return type.Name;
            }
        }


        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_EquipnQuery_Click(object sender, RoutedEventArgs e)
        {
            QueryEquipListDialog dialog = new QueryEquipListDialog();
            dialog.ShowDialog(this);
            if (dialog.IsSuccuessFull == true)
            {
                if (activityPlaceInfo != null && string.IsNullOrEmpty(activityPlaceInfo.Guid) == false)
                {
                    dialog.Condition.PlaceGuid = activityPlaceInfo.Guid;
                }
                dialog.Condition.ActivityGuid = Client.RiasPortal.ModuleContainer.Activity.Guid;
                condition = dialog.Condition;
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan>(channel =>
                {
                    EquipmentItemsSource = channel.GetEquipmentInfos(dialog.Condition);
                });
            }
        }

        /// <summary>
        /// 手工录入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ManualRegister_Click(object sender, RoutedEventArgs e)
        {

            if (activityPlaceInfo != null && string.IsNullOrEmpty(activityPlaceInfo.Guid) == false)
            {
                ActivityEquipmentInfo equapply = new ActivityEquipmentInfo();
                equapply.PlaceGuid = activityPlaceInfo.Guid;
                EquipmentDetailDialog dialog = new EquipmentDetailDialog(equapply);
                dialog.WindowTitle = "设备-手工登记";
                dialog.RefreshEquipmentSource += Dialog_RefreshEquipmentSource;

                dialog.ShowDialog(this);
            }
            else
            {
                MessageBox.Show("请选择设备使用地点!", "提示", MessageBoxButton.OK);
            }
        }
        private void Dialog_RefreshEquipmentSource()
        {
            EquipmentQueryCondition condition = new EquipmentQueryCondition();
            condition.ActivityGuid = Client.RiasPortal.ModuleContainer.Activity.Guid;
            if (activityPlaceInfo != null && string.IsNullOrEmpty(activityPlaceInfo.Guid) == false)
            {
                condition.PlaceGuid = activityPlaceInfo.Guid;
                //condition.ActivityGuid = activityPlaceInfo.Guid;
            }
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan>(channel =>
            {
                EquipmentItemsSource = channel.GetEquipmentInfos(condition);
            });
        }

        /// <summary>
        /// 保障级别设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_SetProtectLevel_Click(object sender, RoutedEventArgs e)
        {
            SetProtectLevelDialog dialog = new SetProtectLevelDialog();
            dialog.Show();
        }

        /// <summary>
        /// 台站库导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ExtractFromStationDB_Click(object sender, RoutedEventArgs e)
        {
            CO_IA.UI.FreqPlan.StationPlan.ExtractFromStationDBDialog dialog = new CO_IA.UI.FreqPlan.StationPlan.ExtractFromStationDBDialog(activityPlaceInfo);
           
            dialog.RefreshEquipmentSource += Dialog_RefreshEquipmentSource;

            dialog.ShowDialog(this);
        }

        /// <summary>
        /// 设备库导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ExtractFromEquipmentDB_Click(object sender, RoutedEventArgs e)
        {
            CO_IA.UI.FreqPlan.StationPlan.ExtractFromEquipmentDBDialog dialog = new CO_IA.UI.FreqPlan.StationPlan.ExtractFromEquipmentDBDialog(activityPlaceInfo);

            dialog.RefreshEquipmentSource += Dialog_RefreshEquipmentSource;
            dialog.ShowDialog(this);
        }



        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Import_Click(object sender, RoutedEventArgs e)
        {
            if (activityPlaceInfo == null || string.IsNullOrEmpty(activityPlaceInfo.Guid) == true)
            {
                MessageBox.Show("请选择设备使用地点!", "提示", MessageBoxButton.OK);
                return;
            }
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Excel文件(*.xls)|*.xls";
            dialog.DefaultExt = "xls";

            dialog.CheckFileExists = true;
            if (dialog.ShowDialog() == true)
            {
                DataTable[] tables = LoadDataFromExcel(dialog.FileName);
                if (tables != null && tables.Length > 0)
                {
                    //单位信息
                    ActivityORGInfo orginfo = null;
                    DataTable orgtable = tables.FirstOrDefault(r => r.TableName == "单位信息");
                    if (orgtable != null && ValidateORG(orgtable))
                    {
                        orginfo = LoadORGFromTable(orgtable);


                        ActivityORGInfo tempOrginfo = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan, ActivityORGInfo>(channel =>
                        {
                            return channel.GetORGInfoByActivityORGInfo(orginfo);
                        });


                        if (tempOrginfo != null && string.IsNullOrEmpty(tempOrginfo.Activity_Guid) == false)
                        {
                            orginfo = tempOrginfo;
                        }


                    }
                    if (orginfo != null)
                    {
                        List<ActivityEquipmentInfo> lstequ = LoadActivityEquipmentFromTable(tables, orginfo);

                        if (lstequ != null)
                        {
                            if (lstequ.Count == 0)
                            {
                                MessageBox.Show("请输入要导入的设备", "提示", MessageBoxButton.OK);
                                return;
                            }
                            else
                            {
                                ImportEquipmentEvent(orginfo, lstequ);
                            }
                        }

                    }
                }
            }
        }

        private void ImportEquipmentEvent(ActivityORGInfo orginfo, List<ActivityEquipmentInfo> lstequ)
        {
            try
            {
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan>(channel =>
                {
                    channel.SaveORGInfo(orginfo);
                });
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan>(channel =>
                {
                    channel.SaveEquipmentList(lstequ);
                });

                condition = new EquipmentQueryCondition();

                if (activityPlaceInfo != null && string.IsNullOrEmpty(activityPlaceInfo.Guid) == false)
                {
                    condition.PlaceGuid = activityPlaceInfo.Guid;
                }
                condition.ActivityGuid = Client.RiasPortal.ModuleContainer.Activity.Guid;


                //重新查询设备
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan>(channel =>
                {
                    EquipmentItemsSource = channel.GetEquipmentInfos(condition);
                });

                MessageBox.Show("导入成功！", "提示", MessageBoxButton.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetExceptionMessage());
            }
        }


        private DataTable[] LoadDataFromExcel(string FileName)
        {
            string SheetName = string.Empty;
            string ConnStr;
            //ConnStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=\"" + FileName + "\";Extended Properties=\"Excel 8.0;HDR=YES\"";
            ConnStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\"" + FileName + "\";Extended Properties=\"Excel 12.0;HDR=YES\"";

            OleDbConnection conn_excel = new OleDbConnection();
            conn_excel.ConnectionString = ConnStr;
            conn_excel.Open();
            //读取Excel的sheetNames
            DataTable dt = conn_excel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
            conn_excel.Close();

            DataTable[] dataTables = new DataTable[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                SheetName = dt.Rows[i]["Table_Name"].ToString();
                OleDbDataAdapter da_excel = new OleDbDataAdapter("Select * From [" + SheetName + "]", conn_excel);
                DataTable dt_excel = new DataTable();
                dt_excel.TableName = SheetName.TrimEnd('$');
                da_excel.Fill(dt_excel);
                dataTables[i] = dt_excel;
            }
            return dataTables;
        }

        /// <summary>
        /// 验证单位信息
        /// </summary>
        /// <returns></returns>
        private bool ValidateORG(DataTable dt)
        {
            bool isSuccess = true;
            StringBuilder errormsg = new StringBuilder();
            if (dt != null)
            {
                string error = string.Empty;
                int index = 1;

                if (!ValidateNull("单位名称", dt.Rows[0][1].ToString(), out error))
                {
                    errormsg.AppendFormat("{0}:" + error, index);
                    isSuccess = false;
                    index++;
                }
                if (dt.Rows[0][1].ToString().Length > 100)
                {
                    errormsg.AppendFormat("{0}:单位名称不能超过100个字符串 \r", index);
                    isSuccess = false;
                    index++;
                }
                if (!ValidateNull("单位简称", dt.Rows[1][1].ToString(), out error))
                {
                    errormsg.AppendFormat("{0}:单位简称不能为空 \r", index);
                    isSuccess = false;
                    index++;
                }
                if (dt.Rows[1][1].ToString().Length > 20)
                {
                    errormsg.AppendFormat("{0}:单位简称不能超过20个字符串 \r", index);
                    isSuccess = false;
                    index++;
                }
                if (dt.Rows[3][1].ToString().Length > 100)
                {
                    errormsg.AppendFormat("{0}:单位地址不能超过100个字符 \r", index);
                    isSuccess = false;
                    index++;

                }
                if (!ValidateNull("联系人", dt.Rows[4][1].ToString(), out error))
                {
                    errormsg.AppendFormat("{0}:联系人不能为空 \r", index);
                    isSuccess = false;
                    index++;

                }
                if (dt.Rows[4][1].ToString().Length > 10)
                {
                    errormsg.AppendFormat("{0}:联系人不能超过10个字符串 \r", index);
                    isSuccess = false;
                    index++;
                }
                if (!ValidateNull("联系方式", dt.Rows[5][1].ToString(), out error))
                {
                    errormsg.AppendFormat("{0}:联系方式不能为空 \r", index);
                    isSuccess = false;
                    index++;
                }
                if (dt.Rows[5][1].ToString().Length > 16)
                {
                    errormsg.AppendFormat("{0}:联系人不能超过16个字符串 \r", index);
                    isSuccess = false;
                    index++;
                }
            }
            else
            {
                errormsg.AppendFormat("导入单位信息为空,请先填写单位信息!");
            }
            if (!isSuccess)
            {
                string title = string.Format("单位{0}错误信息如下:\r", dt.Rows[0][1].ToString());
                StringBuilder msg = new StringBuilder();
                msg.Append(title);
                msg.Append(errormsg.ToString());
                ShowErrorDialog(msg.ToString());
            }
            return isSuccess;
        }

        /// <summary>
        /// 验证设备
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private StringBuilder ValidateEquipments(DataTable dt)
        {
            StringBuilder errormsg = new StringBuilder();
            //第1行为列头
            if (dt != null && dt.Rows.Count > 1)
            {
                for (int i = 1; i < dt.Rows.Count; i++)
                {
                    DataRow row = dt.Rows[i];

                    //名称不为空
                    if (!string.IsNullOrEmpty(row[0].ToString()))
                    {

                        string error = string.Empty; ;
                        if (!ValidateEquipment(row, out  error))
                        {
                            errormsg.AppendFormat("设备{0}:\r", row[0].ToString());
                            errormsg.Append(error);
                        }
                    }
                }
            }
            return errormsg;
        }

        private ActivityORGInfo LoadORGFromTable(DataTable dt)
        {
            ActivityORGInfo org = new ActivityORGInfo();
            if (dt.Rows != null && dt.Rows.Count > 0)
            {
                org.Activity_Guid = Client.RiasPortal.ModuleContainer.Activity.Guid;
                org.Name = dt.Rows[0][1].ToString().Trim();
                org.ShortName = dt.Rows[1][1].ToString().Trim();
                org.Address = dt.Rows[3][1].ToString().Trim();
                org.Contact = dt.Rows[4][1].ToString().Trim();
                org.Phone = dt.Rows[5][1].ToString().Trim();
            }
            return org;
        }

        /// <summary>
        /// 从DataTable中加载EquipmentInfo
        /// </summary>
        /// <param name="tables"></param>
        /// <param name="orginfo"></param>
        /// <returns></returns>
        private List<ActivityEquipmentInfo> LoadActivityEquipmentFromTable(DataTable[] tables, ActivityORGInfo orginfo)
        {
            StringBuilder errormsg = new StringBuilder();

            List<ActivityEquipmentInfo> equipments = new List<ActivityEquipmentInfo>();

            //0:参数信息
            //1:单位信息
            //2:业务类型
            for (int i = 0; i < tables.Length; i++)
            {
                DataTable table = tables[i];
                string tablename = table.TableName;
                if (tablename == "参数设置" || tablename == "单位信息" || tablename == "业务类型")
                {
                    continue;
                }

                string tabname = SettingHelper.TableNameConvert(table.TableName);
                string businesscode = SettingHelper.GetBusinessCode(tabname);
                if (!string.IsNullOrEmpty(businesscode))
                {
                    StringBuilder error = ValidateEquipments(table);
                    if (!string.IsNullOrEmpty(error.ToString()))
                    {
                        errormsg.AppendFormat("{0}错误信息:\r", tabname);
                        errormsg.Append(error);
                        errormsg.Append("\r");
                    }
                    else
                    {
                        List<ActivityEquipmentInfo> equs = CreateActivityEquipments(table, orginfo, businesscode);
                        equipments.AddRange(equs);
                    }
                }
            }
            if (string.IsNullOrEmpty(errormsg.ToString()))
            {
                return equipments;
            }
            else
            {
                ShowErrorDialog(errormsg.ToString());
                return null;
            }
        }

        /// <summary>
        /// 创建ActivityEquipmentInfo对象
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="orginfo"></param>
        /// <param name="businesscode"></param>
        /// <returns></returns>
        private List<ActivityEquipmentInfo> CreateActivityEquipments(DataTable dt, ActivityORGInfo orginfo, string businesscode)
        {
            List<ActivityEquipmentInfo> equs = new List<ActivityEquipmentInfo>();
            if (dt != null && dt.Rows.Count > 0)
            {
                StringBuilder errorMsg = new StringBuilder();

                #region

                for (int i = 1; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    if (string.IsNullOrEmpty(dr[0].ToString())) continue;

                    ActivityEquipmentInfo equ = new ActivityEquipmentInfo();
                    equ.GUID = System.Guid.NewGuid().ToString();
                    equ.ActivityGuid = Client.RiasPortal.ModuleContainer.Activity.Guid;
                    equ.PlaceGuid = activityPlaceInfo.Guid; ;
                    equ.ORGGuid = orginfo.Guid;
                    equ.Origin = 3;
                    IdentifiableData<string> org = new IdentifiableData<string>();
                    org.Guid = orginfo.Guid;
                    org.Name = orginfo.Name;
                    equ.ORG = org;
                    equ.Name = dr[0].ToString();
                    equ.BusinessCode = businesscode;
                    equ.EQUCount = Int32.Parse(dr[1].ToString()); //设备数量
                    equ.EquModel = dr[2].ToString();//设备型号
                    equ.EquNo = dr[3].ToString();//设备编号
                    equ.Remark = dr[4].ToString();//备注
                    equ.IsStation = false;
                    equ.StationName = string.Empty;
                    equ.IsMobile = false;

                    equ.SendFreq = string.IsNullOrEmpty(dr[5].ToString()) ? double.NaN : double.Parse(dr[5].ToString());
                    equ.ReceiveFreq = string.IsNullOrEmpty(dr[6].ToString()) ? double.NaN : double.Parse(dr[6].ToString());//接收频率
                    equ.IsTunAble = dr[7].ToString() == "是" ? true : false;//频率可调
                    equ.SendFreqStart = string.IsNullOrEmpty(dr[8].ToString()) ? double.NaN : double.Parse(dr[8].ToString());//发射频率范围起始
                    equ.SendFreqEnd = string.IsNullOrEmpty(dr[9].ToString()) ? double.NaN : double.Parse(dr[9].ToString());//发射频率范围终止
                    equ.Band = string.IsNullOrEmpty(dr[10].ToString()) ? double.NaN : double.Parse(dr[10].ToString());//带宽
                    equ.MaxPower = string.IsNullOrEmpty(dr[11].ToString()) ? double.NaN : double.Parse(dr[11].ToString());//发射功率

                    EMCS.Types.EMCModulationEnum modulat;
                    if (Enum.TryParse(dr[12].ToString(), out modulat))
                    {
                        equ.ModulateMode = modulat;
                    }

                    //发射天线    
                    //equ.SendPara.Ant.Guid = System.Guid.NewGuid().ToString();

                    //equ.SendPara.Ant.AntType = dr[13].ToString();//天线类型
                    equ.SendAntModel = dr[14].ToString();//天线型号
                    equ.SendAntGain = string.IsNullOrEmpty(dr[15].ToString()) ? double.NaN : double.Parse(dr[15].ToString());//天线增益
                    EMCS.Types.EMCPolarisationEnum sendpolar;
                    if (Enum.TryParse(dr[16].ToString(), out sendpolar)) //极化方式
                    {
                        equ.SendAntPolar = sendpolar;
                    }
                    equ.SendAntHeight = double.Parse(dr[17].ToString());//天线高度
                    equ.SendAntFeedLoss = double.Parse(dr[18].ToString());//馈线损耗


                    //接收
                    equ.RecvFreqStart = string.IsNullOrEmpty(dr[19].ToString()) ? double.NaN : double.Parse(dr[19].ToString());//接收频率范围起始
                    equ.RecvFreqEnd = string.IsNullOrEmpty(dr[20].ToString()) ? double.NaN : double.Parse(dr[20].ToString());//接收频率范围终止
                    //equ.RecivePara.Ant.Guid = System.Guid.NewGuid().ToString();
                    //equ.RecivePara.Ant.AntType = dr[21].ToString();//天线类型
                    equ.RecvAntModel = dr[22].ToString();//天线型号
                    equ.RecvAntGain = string.IsNullOrEmpty(dr[23].ToString()) ? double.NaN : double.Parse(dr[23].ToString());//天线增益
                    EMCS.Types.EMCPolarisationEnum recpolar;
                    if (Enum.TryParse(dr[24].ToString(), out recpolar)) //极化方式
                    {
                        equ.RecvAntPolar = recpolar;

                    }
                    equ.RecvAntHeight = string.IsNullOrEmpty(dr[25].ToString()) ? double.NaN : double.Parse(dr[25].ToString());//天线高度
                    equ.RecvAntFeedLoss = string.IsNullOrEmpty(dr[26].ToString()) ? double.NaN : double.Parse(dr[26].ToString());//馈线损耗

                    equs.Add(equ);
                }

                #endregion

            }
            return equs;
        }

        /// <summary>
        /// 验证设备信息
        /// </summary>
        /// <returns></returns>
        private bool ValidateEquipment(DataRow row, out string outerror)
        {
            outerror = string.Empty;
            StringBuilder errormsg = new StringBuilder();
            bool isSuccess = true;
            int index = 1;
            string error = string.Empty;


            if (!ValidateNull("设备名称", row[0].ToString(), out error))
            {
                errormsg.AppendFormat("{0}:" + error, index);
                index++;
                isSuccess = false;
            }

            //if (!isSuccess)
            //{
            //    errormsg.AppendFormat("{0}:" + error, index);
            //    index++;
            //    isSuccess = false;
            //}

            if (!ValidateDouble("设备数量", row[1].ToString(), out error))
            {
                errormsg.AppendFormat("{0}:" + error, index);
                index++;
                isSuccess = false;
            }


            if (!ValidateDouble("发射频率", row[5].ToString(), out error))
            {
                errormsg.AppendFormat("{0}:" + error, index);
                index++;
                isSuccess = false;
            }

            //接收频率可以为空
            //if (!ValidateDouble("接收频率", row[6].ToString(), out error))
            //{
            //    errormsg.AppendFormat("{0}:" + error, index);
            //    index++;
            //    isSuccess = false;
            //}

            if (!ValidateNull("频率可调", row[7].ToString(), out error))
            {
                errormsg.AppendFormat("{0}:" + error, index);
                index++;
                isSuccess = false;
            }

            if (!ValidateDouble("发射频率范围起始", row[8].ToString(), out error))
            {
                errormsg.AppendFormat("{0}:" + error, index);
                index++;
                isSuccess = false;
            }

            if (!ValidateDouble("发射频率范围终止", row[9].ToString(), out error))
            {
                errormsg.AppendFormat("{0}:" + error, index);
                index++;
                isSuccess = false;
            }

            if (!ValidateDouble("发射频率带宽", row[10].ToString(), out error))
            {
                errormsg.AppendFormat("{0}:" + error, index);
                index++;
                isSuccess = false;
            }

            if (!ValidateDouble("发射功率", row[11].ToString(), out error))
            {
                errormsg.AppendFormat("{0}:" + error, index);
                index++;
                isSuccess = false;
            }

            if (!ValidateNull("调制方式", row[12].ToString(), out error))
            {
                errormsg.AppendFormat("{0}:" + error, index);
                index++;
                isSuccess = false;
            }

            if (!ValidateDouble("发射天线增益", row[15].ToString(), out error))
            {
                errormsg.AppendFormat("{0}:" + error, index);
                index++;
                isSuccess = false;
            }

            if (!ValidateDouble("发射天线高度", row[17].ToString(), out error))
            {
                errormsg.AppendFormat("{0}:" + error, index);
                index++;
                isSuccess = false;
            }

            if (!ValidateDouble("发射天线馈线损耗", row[18].ToString(), out error))
            {
                errormsg.AppendFormat("{0}:" + error, index);
                index++;
                isSuccess = false;
            }

            //接收频率为空,则不保存接收信息
            if (!string.IsNullOrEmpty(row[6].ToString()))
            {
                //接收
                if (!ValidateDouble("接收频率范围起始", row[19].ToString(), out error))
                {
                    errormsg.AppendFormat("{0}:" + error, index);
                    index++;
                    isSuccess = false;

                }

                if (!ValidateDouble("接收频率范围终止", row[20].ToString(), out error))
                {
                    errormsg.AppendFormat("{0}:" + error, index);
                    index++;
                    isSuccess = false;
                }

                if (!ValidateDouble("接收天线增益", row[23].ToString(), out error))
                {
                    errormsg.AppendFormat("{0}:" + error, index);
                    index++;
                    isSuccess = false;
                }

                if (!ValidateDouble("接收天线高度", row[25].ToString(), out error))
                {
                    errormsg.AppendFormat("{0}:" + error, index);
                    index++;
                    isSuccess = false;
                }

                if (!ValidateDouble("接收天线馈线损耗", row[26].ToString(), out error))
                {
                    errormsg.AppendFormat("{0}:" + error, index);
                    index++;
                    isSuccess = false;
                }
            }

            outerror = errormsg.ToString();
            return isSuccess;
        }

        /// <summary>
        /// 验证是否为空值
        /// </summary>
        /// <param name="name">验证的列明，用于提示</param>
        /// <param name="value">验证值</param>
        /// <param name="isSuccess">验证结果的布尔值</param>
        /// <returns></returns>
        private bool ValidateNull(string name, string value, out string error)
        {
            bool isSuccess = true;
            error = string.Empty;
            if (string.IsNullOrEmpty(value))
            {
                error = string.Format("{0}不能为空 \r", name);
                isSuccess = false;
            }
            return isSuccess;
        }

        /// <summary>
        /// 验证是否是Double值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        private bool ValidateDouble(string name, string value, out string error)
        {
            bool isSuccess = true;
            error = string.Empty;
            StringBuilder strerror = new StringBuilder();
            string nullerror = string.Empty;

            //先验证是否为空
            if (!ValidateNull(name, value, out nullerror))
            {
                strerror.Append(nullerror);
                isSuccess = false;
            }
            else
            {
                if (!IsDouble(value))
                {
                    strerror.AppendFormat("{0}应为为数字 \r", name);
                    isSuccess = false;
                }
            }
            error = strerror.ToString();
            return isSuccess;
        }

        private bool IsDouble(string value)
        {
            double dresult = 0;
            if (double.TryParse(value, out dresult))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        private void ShowErrorDialog(string content)
        {
            //CO_IA.UI.Setting.Equipment.ErrorDialog errordialog = new CO_IA.UI.Setting.Equipment.ErrorDialog(content);
            //errordialog.ShowDialog(this);
        }

        #endregion


        #region DataGrid 按键方法
        /// <summary>
        /// 弹出设备详情
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void equdatagrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EquipmentDetailDialog dialog = new EquipmentDetailDialog(SelectedEquipment);
            dialog.IsEnabled = false;
            dialog.WindowTitle = "设备-详细信息";
            dialog.ShowDialog(this);
        }

        private void chkAll_Click(object sender, RoutedEventArgs e)
        {
            //CheckBox chk = sender as CheckBox;
            //bool ischecked = chk.IsChecked.Value;

            //foreach (ActivityEquipmentInfo item in EquipmentItemsSource)
            //{
            //    item.IsChecked = ischecked;
            //}
        }

        private void chkAll_Loaded(object sender, RoutedEventArgs e)
        {
            //chkAll = sender as CheckBox;
            //if (this.EquipmentItemsSource != null)
            //{
            //    chkAll.IsChecked = EquipmentItemsSource.Any(item => item.IsChecked);
            //}
        }

        /// <summary>
        /// 列点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkCell_Click(object sender, RoutedEventArgs e)
        {
            int i = 0;
            foreach (ActivityEquipmentInfo info in EquipmentItemsSource)
            {
                if (info.IsChecked == true)
                {
                    i++;
                }

            }


            if (i == 1)
            {
                btn_Modify.IsEnabled = true;
                btn_Delete.IsEnabled = true;
            }
            else if (i > 1)
            {
                btn_Modify.IsEnabled = false;
                btn_Delete.IsEnabled = true;
            }
            else
            {
                btn_Modify.IsEnabled = false;
                btn_Delete.IsEnabled = false;
            }

        }



        private void equdatagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.dg_equiplist.SelectedItem != null)
            {
                SelectedEquipment = this.dg_equiplist.SelectedItem as ActivityEquipmentInfo;
            }
        }


        #endregion

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.ErrorException.Message);
        }

        public void PlaceSourceSelectionChanged(int index)
        {
            //if (borderContent.Visibility == Visibility.Visible)
            //{
            //    if (companyControl != null)
            //    {
            //        companyControl.UpdateSource(index);
            //    }
            //}
            //else
            //{
            //    ObservableCollection<ActivityEquipmentInfo> collection = new ObservableCollection<ActivityEquipmentInfo>();
            //    if (index == -1)
            //    {
            //        collection = EquipmentItemsSource;
            //    }
            //    else if (index == 0)
            //    {
            //        collection = new ObservableCollection<ActivityEquipmentInfo>
            //          (EquipmentItemsSource.Where(r => r.GUID == "西安塔").ToList())
            //        { };
            //    }
            //    else if (index == 1)
            //    {
            //        collection = new ObservableCollection<ActivityEquipmentInfo>(EquipmentItemsSource.Where(r => r.GUID == "创意园").ToList()) { };
            //    }
            //    equipmentListControl.EquipmentItemsSource = collection.ToArray();
            //}
        }


        /// <summary>
        /// 删除相关设备信息
        /// </summary>
        /// <param name="deleteLsit"></param>
        private void DeleteEquipList(List<ActivityEquipmentInfo> deleteLsit)
        {
            bool result = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan, bool>(channel =>
            {
                return channel.DeleteEquipmentList(deleteLsit);
            });

            //if (result == true)
            //{
            //    MessageBox.Show("删除信息成功！");
            //}
            //else
            //{
            //    MessageBox.Show("删除信息失败，请联系管理员查看！");

            //}
        }

        /// <summary>
        /// 设置功能按钮条是否显示  by xiaguohui
        /// </summary>
        /// <param name="Show"></param>
        public void SetToolBarVisibility(bool Show)
        {
            if (Show)
            {
                toolbar.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                toolbar.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}