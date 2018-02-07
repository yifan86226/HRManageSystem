using CO_IA.Data;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Data.OleDb;
using System.Data;
using CO_IA.Client;
using System.Reflection;
using AT_BC.Data;
using I_CO_IA.PlanDatabase;
using CO_IA.UI.PlanDatabase.Equipments;
using PT_BS_Service.Client.Framework;

namespace CO_IA.UI.PlanDatabase.ORG
{
    /// <summary>
    /// EquipmentManageModule.xaml 的交互逻辑
    /// </summary>
    public partial class EquipmentManageModule : UserControl
    {
        public OrgQueryCondition orgquerycondition = new OrgQueryCondition();
        public EquipmentLoadStrategy eququerycondition = new EquipmentLoadStrategy();

        private EquipmentQueryCondition querycondition = new EquipmentQueryCondition() { IsStation = true, IsTunAble = true, IsMobile = true };
        private EquipmentQueryCondition QueryCondition
        {
            get { return querycondition; }
            set { querycondition = value; }
        }

        private Organization[] ORGItemsSource
        {
            get { return orgListControl.ORGItemsSource; }
        }

        /// <summary>
        /// 当前选择的单位
        /// </summary>
        private Organization SelectedORG
        {
            get { return orgListControl.SelectedORG; }
        }

        private Equipment SelectedEquipmentInfo
        {
            get { return equipmentListControl.SelectedEquipment; }
        }

        public EquipmentManageModule()
        {
            InitializeComponent();
            GetORGbyCondition(orgquerycondition);
            InitEvent();
        }

        private void InitEvent()
        {
            this.orgListControl.SelectionChanged += orgListControl_SelectionChanged;
            this.equipmentListControl.DoubleClick += equipmentListControl_DoubleClick;
        }

        #region  单位管理相关方法

        /// <summary>
        /// 查询单位信息
        /// </summary>
        /// <param name="condition"></param>
        private void GetORGbyCondition(OrgQueryCondition condition)
        {
            Organization[] source = GetORGSource(condition);
            orgListControl.DataContext = source;
            if (source == null || source.Length == 0)
            {
                equipmentListControl.DataContext = null;
            }
        }

        private Organization[] GetORGSource(OrgQueryCondition condition)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_PlanDatabase, Organization[]>(channel =>
            {
                return channel.GetOrgByCondition(condition);
            });
        }

        /// <summary>
        /// 单位管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnManageORG_Click(object sender, RoutedEventArgs e)
        {
            SecurityClass[] securityclasses = CO_IA.Client.Utility.GetSecurityClasses();
            if (securityclasses == null || securityclasses.Length == 0)
            {
                MessageBox.Show("请先在基础数据设置中增加保障类别");
            }
            else
            {
                ORGManageControl manage = new ORGManageControl();
                manage.RefreshORGItemSource += () =>
                {
                    GetORGbyCondition(orgquerycondition);
                };
                manage.ShowDialog();
            }
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnQueryORG_Click(object sender, RoutedEventArgs e)
        {
            ORGQueryDialog querydialog = new ORGQueryDialog(orgquerycondition);
            querydialog.OnQueryEvent += (condition) =>
            {
                orgquerycondition = condition;
                this.GetORGbyCondition(condition);
            };
            querydialog.ShowDialog();
        }

        /// <summary>
        /// 单位换行事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void orgdatagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedORG != null)
            {
                eququerycondition = new EquipmentLoadStrategy();
                eququerycondition.OrgName = SelectedORG.Name;

                BeOperationInvoker.Invoke<I_CO_IA_PlanDatabase>(channel =>
                {
                    equipmentListControl.DataContext = channel.GetEquipmentsForOrg(SelectedORG.Guid);
                });
            }
        }

        private void orgListControl_SelectionChanged(Organization obj)
        {
            if (SelectedORG != null)
            {
                eququerycondition = new EquipmentLoadStrategy();
                eququerycondition.OrgName = SelectedORG.Name;

                BeOperationInvoker.Invoke<I_CO_IA_PlanDatabase>(channel =>
                {
                    equipmentListControl.DataContext = channel.GetEquipmentsForOrg(SelectedORG.Guid);
                });
            }
        }

        #endregion

        #region 设备管理相关方法

        /// <summary>
        /// 查询设备
        /// </summary>
        /// <param name="condition"></param>
        private void GetEquipments(EquipmentLoadStrategy condition)
        {
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PlanDatabase.I_CO_IA_PlanDatabase>(channel =>
            {
                equipmentListControl.DataContext = channel.GetEquipments(condition);
            });
        }

        private void GetEquipmentsForOrg(string orggui)
        {
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PlanDatabase.I_CO_IA_PlanDatabase>(channel =>
            {
                equipmentListControl.DataContext = channel.GetEquipmentsForOrg(orggui);
            });
        }

        private bool SaveEquipment(Equipment equ)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_PlanDatabase, bool>(channel =>
            {
                int count = channel.CountEquName(equ);
                if (count == 0)
                {
                    try
                    {
                        channel.SaveEquipment(equ);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.GetExceptionMessage(), "保存失败");
                        return false;
                    }
                }
                else
                {
                    MessageBox.Show(string.Format("当前单位名称下,存在相同的设备名称+设备编号,请重新输入"));
                    return false;
                }
            });
        }

        #endregion

        /// <summary>
        /// 设备列表双击事件
        /// </summary>
        /// <param name="obj"></param>
        private void equipmentListControl_DoubleClick(Equipment obj)
        {
            if (SelectedEquipmentInfo == null)
            {
                MessageBox.Show("请选择设备!", "提示", MessageBoxButton.OK);
                return;
            }

            EquipmentManageDialog addequdialog = new EquipmentManageDialog();
            addequdialog.AllowEdit = true;
            addequdialog.DataContext = obj;
            addequdialog.OnSaveEvent += (equ) =>
             {
                 bool result = SaveEquipment(equ);
                 if (result)
                 {
                     GetEquipmentsForOrg(SelectedORG.Guid);
                     equipmentListControl.SelectedEquipment = equipmentListControl.EquipmentItemsSource.FirstOrDefault(r => r.Key == equ.Key);
                     MessageBox.Show("保存成功");
                 }
                 return result;
             };
            addequdialog.ShowDialog();
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAddEqu_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedORG == null)
            {
                MessageBox.Show("请选择新增设备所属单位!", "提示", MessageBoxButton.OK);
                return;
            }
            Equipment newequ = new Equipment();
            newequ.OrgInfo.Guid = SelectedORG.Guid;
            newequ.OrgInfo.Name = SelectedORG.Name;
            newequ.Key = System.Guid.NewGuid().ToString();
            newequ.EQUCount = 1;
            EquipmentManageDialog addequdialog = new EquipmentManageDialog();
            addequdialog.AllowEdit = true;
            addequdialog.DataContext = newequ;
            addequdialog.OnSaveEvent += (equ) =>
            {
                bool result = SaveEquipment(equ);
                if (result)
                {
                    MessageBox.Show("保存成功");
                    GetEquipmentsForOrg(SelectedORG.Guid);
                }
                return result;
            };
            addequdialog.ShowDialog();
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnQueryEqu_Click(object sender, RoutedEventArgs e)
        {
            EquipmentQueryDialog querydialog = new EquipmentQueryDialog(eququerycondition);
            querydialog.OnQueryEvent += querydialog_OnQueryEvent;
            querydialog.ShowDialog();
        }

        private void querydialog_OnQueryEvent(EquipmentLoadStrategy obj)
        {
            eququerycondition = obj;
            GetEquipments(eququerycondition);
        }

        /// <summary>
        /// XLS导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnImportEqu_Click(object sender, RoutedEventArgs e)
        {
            SecurityClass[] securityclasses = CO_IA.Client.Utility.GetSecurityClasses();
            if (securityclasses == null || securityclasses.Length == 0)
            {
                MessageBox.Show("请先在基础数据设置中增加保障类别");
            }
            else
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Excel文件(*.xls)|*.xls";
                //dialog.Filter = "(*.xlsx)|*.xlsx|(*.xls)|*.xls";
                //dialog.DefaultExt = "xlsx";

                dialog.CheckFileExists = true;
                if (dialog.ShowDialog() == true)
                {
                    DataTable[] tables = ExcelImportHelper.LoadDataFromExcel(dialog.FileName);
                    if (tables != null && tables.Length > 0)
                    {
                        //单位信息
                        Organization orginfo = null;
                        DataTable orgtable = tables.FirstOrDefault(r => r.TableName == "单位信息");
                        if (orgtable != null && ExcelImportHelper.ValidateORG(orgtable))
                        {
                            orginfo = this.LoadORGFromTable(orgtable);
                        }
                        if (orginfo != null)
                        {
                            #region 验证单位名称

                            OrgQueryCondition condition = new OrgQueryCondition();
                            condition.Name = orginfo.Name;
                            Organization[] orgs = this.GetORGSource(condition);
                            Organization sameorg = orgs.FirstOrDefault(r => r.Name == orginfo.Name);
                            #endregion

                            if (sameorg != null)
                            {
                                MessageBoxResult result = MessageBox.Show(string.Format("'{0}'已经存在,是否将Excel中的设备导入到'{0}'中?", orginfo.Name), "提示", MessageBoxButton.YesNo);
                                if (result == MessageBoxResult.Yes)
                                {
                                    ImportEquipment(tables, sameorg);
                                }
                                else
                                {
                                    MessageBox.Show("请修改单位名称");
                                }
                            }
                            else
                            {
                                ImportEquipment(tables, orginfo);
                            }
                        }
                    }
                }
            }
        }

        private void ImportEquipmentEvent(Organization orginfo, List<Equipment> lstequ)
        {
            List<Equipment> lst = new List<Equipment>();

            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PlanDatabase.I_CO_IA_PlanDatabase>(channel =>
            {
                foreach (Equipment equ in lstequ)
                {
                    int count = channel.CountEquName(equ);
                    if (count > 0)
                    {
                        lst.Add(equ);
                    }
                }

                if (lst.Count == 0)
                {
                    try
                    {
                        channel.ImportEquipment(orginfo, lstequ);
                        GetORGbyCondition(orgquerycondition);
                        Organization selectorg = ORGItemsSource.FirstOrDefault(r => r.Guid == orginfo.Guid);
                        this.orgListControl.SelectedORG = selectorg;
                        MessageBox.Show("导入成功！", "提示", MessageBoxButton.OK);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.GetExceptionMessage(), "导入失败");
                    }
                }
                else
                {
                    StringBuilder errormsg = new StringBuilder();
                    errormsg.AppendLine("当前单位名称下,存在相同的设备名称+编号:");
                    foreach (Equipment item in lst)
                    {
                        errormsg.AppendLine(string.Format("设备名称:{0},设备编号:{1}", item.Name, item.SeriesNumber));
                    }

                    ErrorDialog errordialog = new ErrorDialog(errormsg.ToString());
                    errordialog.ShowDialog();
                }
            });
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDeleteEqu_Click(object sender, RoutedEventArgs e)
        {
            CheckableData<string>[] selectequs = Equipment.GetCheckedItems(equipmentListControl.EquipmentItemsSource);
            if (selectequs == null || selectequs.Length == 0)
            {
                MessageBox.Show("请勾选要删除的设备");
                return;
            }
            else
            {
                MessageBoxResult msresult = MessageBox.Show("确认要删除选择的设备?", "提示", MessageBoxButton.YesNo);
                if (msresult == MessageBoxResult.Yes)
                {
                    List<string> guids = selectequs.Select(r => r.Key).ToList();
                    try
                    {
                        PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_PlanDatabase>(channel =>
                        {
                            channel.DeleteEquipment(guids);
                        });
                        GetEquipments(eququerycondition);
                        equipmentListControl.UpdateCheckAllState();
                        MessageBox.Show("删除成功");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.GetExceptionMessage(), "保存失败");
                    }
                }
            }
        }

        /// <summary>
        /// 导出到Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnExportEqu_Click(object sender, RoutedEventArgs e)
        {
            string path = CO_IA.Client.ExcelHelper.GetPath();
            if (!string.IsNullOrWhiteSpace(path))
            {
                object[,] orgsource = this.ORGSourceToObject();
                object[,] equsource = equipmentListControl.EquSourceToObject();
                bool exportresult = ExcelHelper.ExportToExcel(orgsource, equsource, path);
                if (exportresult)
                {
                    MessageBox.Show("导出成功");
                }
                else
                {
                    MessageBox.Show("导出失败");
                }
            }
        }

        private object[,] ORGSourceToObject()
        {
            object[,] obj = new object[6, 1];
            if (SelectedORG != null)
            {
                obj[0, 0] = SelectedORG.Name;
                obj[1, 0] = SelectedORG.ShortName;
                obj[2, 0] = SelectedORG.SecurityClass.Name;
                obj[3, 0] = SelectedORG.Address;
                obj[4, 0] = SelectedORG.Contact;
                obj[5, 0] = SelectedORG.Phone;
            }
            return obj;
        }

        private Organization LoadORGFromTable(DataTable dt)
        {
            Organization org = new Organization();
            if (dt.Rows != null && dt.Rows.Count > 0)
            {
                org.Guid = System.Guid.NewGuid().ToString();

                org.Name = dt.Columns[1].ColumnName;
                org.ShortName = dt.Rows[0][1].ToString();
                org.Address = dt.Rows[2][1].ToString();
                org.Contact = dt.Rows[3][1].ToString();
                org.Phone = dt.Rows[4][1].ToString();

                string secclass = dt.Rows[1][1].ToString();
                SecurityClass[] securityclasses = CO_IA.Client.Utility.GetSecurityClasses();
                if (secclass != null && securityclasses.Length > 0)
                {
                    SecurityClass securityclass = securityclasses.FirstOrDefault(r => r.Name == secclass);
                    if (securityclass == null)
                    {
                        MessageBoxResult msgres = MessageBox.Show(string.Format("系统的保障类别中,不存在名称为‘{0}’的保障类别,是否将保障类别改为默认保障类别'{1}'", secclass, securityclasses[0].Name), "提示", MessageBoxButton.YesNo);
                        if (msgres == MessageBoxResult.Yes)
                        {
                            org.SecurityClass = securityclasses[0];
                        }
                        else
                        {
                            MessageBox.Show("请修改保障类别");
                            return null;
                        }
                    }
                    else
                    {
                        org.SecurityClass = securityclass;
                    }
                }
                else
                {
                    MessageBox.Show("系统中不存在保障类别");
                }
            }
            return org;
        }

        /// <summary>
        /// 导入设备
        /// </summary>
        /// <param name="tables"></param>
        /// <param name="orginfo"></param>
        private void ImportEquipment(DataTable[] tables, Organization orginfo)
        {
            StringBuilder error = new StringBuilder();
            bool validresult = ExcelImportHelper.ValidateEquipmentTables(tables, out error); //验证设备信息
            if (validresult)
            {
                List<Equipment> lstequ = this.LoadEquipments(tables, orginfo.Guid);
                if (lstequ == null || lstequ.Count == 0)
                {
                    MessageBox.Show("请输入要导入的设备", "提示", MessageBoxButton.OK);
                    return;
                }
                else
                {
                    ImportEquipmentEvent(orginfo, lstequ);
                }
            }
            else
            {
                ErrorDialog errordialog = new ErrorDialog(error.ToString());
                errordialog.ShowDialog();
                return;
            }
        }

        private List<Equipment> LoadEquipments(DataTable[] tables, string orgguid)
        {
            StringBuilder errormsg = new StringBuilder();
            List<Equipment> equipments = new List<Equipment>();

            DataTable table = tables.FirstOrDefault(r => r.TableName == "设备信息");
            List<Equipment> equs = LoadEquipmentFromTable(table, orgguid);
            equipments.AddRange(equs);


            return equipments;
        }


        /// <summary>
        /// 从DataTable中加载EquipmentInfo
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="orginfo"></param>
        /// <returns></returns>
        private List<Equipment> LoadEquipmentFromTable(DataTable dt, string orgguid)
        {
            List<Equipment> equs = new List<Equipment>();
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    if (string.IsNullOrEmpty(dr["设备名称"].ToString())) continue;
                    else
                    {
                        Equipment equ = CreateEquipmentInfoByDataRow(dr, orgguid);
                        equs.Add(equ);
                    }
                }
            }
            return equs;
        }

        /// <summary>
        ///创建 EquipmentInfo对象
        /// </summary>
        /// <param name="dr">行</param>
        /// <param name="ORGGuid">单位Guid</param>
        /// <param name="businesscode">业务类型Code</param>
        /// <returns></returns>
        private Equipment CreateEquipmentInfoByDataRow(DataRow dr, string ORGGuid)
        {
            Equipment equ = new Equipment();
            equ.Key = System.Guid.NewGuid().ToString();
            equ.OrgInfo.Guid = ORGGuid;
            equ.Name = dr["设备名称"].ToString();
            equ.SeriesNumber = dr["设备编号"].ToString();//设备编号
            equ.EquipmentClassID = ExcelImportHelper.GetEquipmentClassId(dr["业务类型"].ToString());
            equ.EQUCount = Int32.Parse(dr["数量"].ToString()); //设备数量
            equ.EquModel = dr["设备型号"].ToString();//设备型号
            equ.IsMobile = dr["移动设备"].ToString() == "是" ? true : false; //移动设备
            if (!equ.IsMobile)
            {
                equ.Longitude = double.Parse(dr["经度"].ToString());
                equ.Latitude = double.Parse(dr["纬度"].ToString());
                equ.Address = dr["地点"].ToString();
            }
            equ.IsStation = dr["已建站"].ToString() == "是" ? true : false;
            if (equ.IsStation)
            {
                equ.StationName = dr["已建站名称"].ToString();
                equ.StationTDI = dr["台站编号"].ToString();
            }
            else
            {
                equ.StationName = string.Empty;
                equ.StationTDI = string.Empty;
            }

            equ.SendFreq = double.Parse(dr["发射频率(MHz)"].ToString()); //发射频率
            if (string.IsNullOrWhiteSpace(dr["接收频率(MHz)"].ToString()))
            {
                equ.ReceiveFreq = null;//接收频率
            }
            else
            {
                equ.ReceiveFreq = double.Parse(dr["接收频率(MHz)"].ToString());//接收频率
            }

            if (string.IsNullOrEmpty(dr["备用频率(MHz)"].ToString()))
            {
                equ.SpareFreq = null;//备用频率
            }
            else
            {
                equ.SpareFreq = double.Parse(dr["备用频率(MHz)"].ToString());//接收频率
            }

            equ.IsTunable = dr["频率可调"].ToString() == "是" ? true : false;//频率可调
            if (equ.IsTunable)
            {
                if (string.IsNullOrEmpty(dr["频率范围起始(MHz)"].ToString()))
                {

                    equ.FreqRange.Little = null;//发射频率范围起始
                }
                else
                {
                    equ.FreqRange.Little = double.Parse(dr["频率范围起始(MHz)"].ToString());//发射频率范围起始

                }
                if (string.IsNullOrEmpty(dr["频率范围终止(MHz)"].ToString()))
                {
                    equ.FreqRange.Great = null;//发射频率范围终止
                }
                else
                {
                    equ.FreqRange.Great = double.Parse(dr["频率范围终止(MHz)"].ToString());//发射频率范围终止
                }
            }
            else
            {
                equ.FreqRange = new Range<double?>();
            }

            equ.Band_kHz = double.Parse(dr["波道带宽(kHz)"].ToString());//必要带宽
            equ.Power_W = double.Parse(dr["发射功率(W)"].ToString());//发射功率
            EMCS.Types.EMCModulationEnum modulat;
            if (Enum.TryParse(dr["调制方式"].ToString(), out modulat))
            {
                equ.Modulation = modulat;
            }
            equ.Remark = dr["备注"].ToString();
            return equ;
        }

        private void btnDownLoad_Click(object sender, RoutedEventArgs e)
        {
            ExcelImportHelper.TemplateDownLoad("设备导入模板.xls", "Template\\Equipment\\");
        }
    }
}

