using CO_IA.Client;
using CO_IA.Data;
using CO_IA.UI.PlanDatabase;
using CO_IA.UI.PlanDatabase.Equipments;
using CO_IA.UI.PlanDatabase.ORG;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System;
using AT_BC.Data;
using PT_BS_Service.Client.Framework;
using I_CO_IA.FreqStation;

namespace CO_IA.UI.FreqStation.FreqPlan
{
    /// <summary>
    /// ORGAndEquipmentManage.xaml 的交互逻辑
    /// </summary>
    public partial class ORGAndEquipmentManage : UserControl
    {
        private OrgQueryCondition orgquerycondition = new OrgQueryCondition();
        private EquipmentLoadStrategy eququerycondition = new EquipmentLoadStrategy()
        {
            ActivityGuid = RiasPortal.ModuleContainer.Activity.Guid,
        };

        /// <summary>
        /// 当前活动
        /// </summary>
        private Activity CurrentActivity { get; set; }

        public ActivityPlaceInfo CurrentActivityPlace
        {
            get;
            set;
        }

        public ActivityOrganization[] ActivityOrgSource
        {
            get { return orgdatagrid.ItemsSource as ActivityOrganization[]; }
        }

        public ActivityOrganization SelectActivityORG
        {
            get { return orgdatagrid.SelectedItem as ActivityOrganization; }
        }

        public ORGAndEquipmentManage(ActivityPlaceInfo avtivityplace)
        {
            InitializeComponent();
            this.CurrentActivityPlace = avtivityplace;
            this.CurrentActivity = RiasPortal.ModuleContainer.Activity;
            orgquerycondition.ActivityGuid = CurrentActivity.Guid;
            InitQueryCondition(eququerycondition);
            GetActivityOrgs(orgquerycondition);
            InitEvent();
        }

        private void InitEvent()
        {
            //单位
            btnOrgManage.Click += this.btnORGManage_Click;
            btnOrgQuery.Click += this.btnOrgQuery_Click;

            //设备
            btnEquQuery.Click += this.btnEquQuery_Click;
            btnEquAdd.Click += this.btnEquAdd_Click;
            btnEquDelete.Click += this.btnEquDelete_Click;
            btnImport.Click += this.btnImport_Click;
            btnOutput.Click += this.btnOutput_Click;
            btnFromRias.Click += this.btnFromRias_Click;

            equipmentListControl.DoubleClick += equipmentListControl_DoubleClick;
        }



        private void InitQueryCondition(EquipmentLoadStrategy condition)
        {
            condition.ActivityGuid = CurrentActivity.Guid;
            condition.PlaceGuid = CurrentActivityPlace.Guid;
            if (SelectActivityORG != null)
            {
                condition.OrgName = SelectActivityORG.Name;
            }
        }

        #region 单位相关方法


        /// <summary>
        /// 查询单位信息
        /// </summary>
        /// <param name="condition"></param>
        private void GetActivityOrgs(OrgQueryCondition condition)
        {
            ActivityOrganization[] source = GetActivityOrgSources(condition);

            orgdatagrid.ItemsSource = source;

            if (source != null && source.Length > 0)
            {
                orgdatagrid.SelectedIndex = 0;
            }
            else
            {
                equipmentListControl.DataContext = null;
            }
        }

        private ActivityOrganization[] GetActivityOrgSources(OrgQueryCondition condition)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqStation, ActivityOrganization[]>(channel =>
            {
                return channel.GetActivityOrgs(condition);
            });
        }

        private void btnORGManage_Click(object sender, RoutedEventArgs e)
        {
            SecurityClass[] securityclasses = CO_IA.Client.Utility.GetSecurityClasses();
            if (securityclasses == null || securityclasses.Length == 0)
            {
                MessageBox.Show("请现在基础数据设置中增加保障级别");
            }
            else
            {
                ORGManageDialog orgmanage = new ORGManageDialog();
                orgmanage.RefreshORGItemSource += () =>
                {
                    GetActivityOrgs(orgquerycondition);
                };
                orgmanage.ShowDialog();
            }
        }

        private void btnOrgQuery_Click(object sender, RoutedEventArgs e)
        {
            ORGQueryDialog querydialog = new ORGQueryDialog(orgquerycondition);
            querydialog.OnQueryEvent += (condition) =>
            {
                orgquerycondition = condition;

                this.GetActivityOrgs(condition);
            };
            querydialog.ShowDialog();
        }

        private void orgdatagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectActivityORG != null)
            {
                eququerycondition = new EquipmentLoadStrategy();
                InitQueryCondition(eququerycondition);
                GetActivityEquipments(eququerycondition);
            }
        }

        #endregion

        #region 设备相关方法

        /// <summary>
        /// 查询设备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEquQuery_Click(object sender, RoutedEventArgs e)
        {
            EquipmentQueryDialog querydialog = new EquipmentQueryDialog(eququerycondition);
            querydialog.ORGNameIsReadOnly = true;
            querydialog.OnQueryEvent += (condition) =>
            {
                eququerycondition = condition;
                eququerycondition.ActivityGuid = CurrentActivity.Guid;
                eququerycondition.PlaceGuid = CurrentActivityPlace.Guid;

                this.GetActivityEquipments(condition);
            };
            querydialog.ShowDialog();
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEquAdd_Click(object sender, RoutedEventArgs e)
        {
            if (SelectActivityORG == null)
            {
                MessageBox.Show("请选择新增设备所属单位!", "提示", MessageBoxButton.OK);
                return;
            }
            Equipment newequ = new Equipment();
            newequ.OrgInfo.Guid = SelectActivityORG.Guid;
            newequ.OrgInfo.Name = SelectActivityORG.Name;
            newequ.Key = System.Guid.NewGuid().ToString();
            newequ.EQUCount = 1;
            EquipmentManageDialog addequdialog = new EquipmentManageDialog();
            addequdialog.AllowEdit = true;
            addequdialog.DataContext = newequ;
            addequdialog.OnSaveEvent += (equ) =>
            {
                ActivityEquipment actequ = new ActivityEquipment();
                actequ.ActivityGuid = this.CurrentActivity.Guid;
                actequ.PlaceGuid = this.CurrentActivityPlace.Guid;
                actequ.CopyFrom(equ);

                bool result = this.SaveActivityEquipment(actequ);
                if (result)
                {
                    InitQueryCondition(eququerycondition);
                    GetActivityEquipments(eququerycondition);
                    MessageBox.Show("保存成功");
                }
                return result;
            };
            addequdialog.ShowDialog();
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="obj"></param>
        private void equipmentListControl_DoubleClick(ActivityEquipment obj)
        {
            EquipmentManageDialog addequdialog = new EquipmentManageDialog();
            addequdialog.AllowEdit = true;
            addequdialog.DataContext = obj;
            addequdialog.OnSaveEvent += (equ) =>
            {
                ActivityEquipment actequ = new ActivityEquipment();
                actequ.ActivityGuid = this.CurrentActivity.Guid;
                actequ.PlaceGuid = this.CurrentActivityPlace.Guid;
                actequ.CopyFrom(equ);
                bool result = SaveActivityEquipment(actequ);
                if (result)
                {
                    InitQueryCondition(eququerycondition);
                    GetActivityEquipments(eququerycondition);
                    MessageBox.Show("保存成功");
                }
                return result;
            };
            addequdialog.ShowDialog();
        }

        /// <summary>
        /// 设备库导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFromRias_Click(object sender, RoutedEventArgs e)
        {
            EquipmentSelectorDialog equselectordialog = new EquipmentSelectorDialog();
            equselectordialog.OnConfirmEvent += ImportFromRiasEvent;
            equselectordialog.ShowDialog();
        }

        private void ImportFromRiasEvent(Organization arg1, List<Equipment> arg2)
        {
            ActivityOrganization importorg = new ActivityOrganization();
            importorg.CopyFrom(arg1);
            importorg.ActivityGuid = this.CurrentActivity.Guid;

            if (arg2 != null && arg2.Count > 0)
            {
                List<ActivityEquipment> actequs = new List<ActivityEquipment>();
                foreach (Equipment item in arg2)
                {
                    ActivityEquipment equ = new ActivityEquipment();
                    equ.CopyFrom(item);
                    equ.ActivityGuid = this.CurrentActivity.Guid;
                    equ.PlaceGuid = this.CurrentActivityPlace.Guid;
                    equ.OrgInfo.Guid = importorg.Guid;
                    actequs.Add(equ);
                }

                this.ImportActivityEquipmentEvent(importorg, actequs);
            }
            else
            {
                MessageBox.Show("请选择要导入的设备");
            }
        }

        /// <summary>
        /// Excel导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            Import();
        }
        public void Import()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Excel文件(*.xls)|*.xls";
            dialog.DefaultExt = "xls";

            dialog.CheckFileExists = true;
            if (dialog.ShowDialog() == true)
            {
                DataTable[] tables = ExcelImportHelper.LoadDataFromExcel(dialog.FileName);
                if (tables != null && tables.Length > 0)
                {
                    //单位信息
                    ActivityOrganization activityorg = null;
                    DataTable orgtable = tables.FirstOrDefault(r => r.TableName == "单位信息");
                    if (orgtable != null && ExcelImportHelper.ValidateORG(orgtable))
                    {
                        activityorg = this.LoadActivityORGFromTable(orgtable, CurrentActivity.Guid);
                    }
                    if (activityorg != null)
                    {
                        #region 验证单位名称
                        OrgQueryCondition condition = new OrgQueryCondition();
                        condition.ActivityGuid = CurrentActivity.Guid;
                        condition.Name = activityorg.Name;
                        ActivityOrganization[] orgs = this.GetActivityOrgSources(condition);
                        ActivityOrganization sameorg = orgs.FirstOrDefault(r => r.Name == activityorg.Name);
                        #endregion

                        if (sameorg != null)
                        {
                            MessageBoxResult result = MessageBox.Show(string.Format("'{0}'已经存在,是否将Excel中的设备导入到'{0}'中?", activityorg.Name), "提示", MessageBoxButton.YesNo);

                            if (result == MessageBoxResult.Yes)
                            {
                                ImportActivityEquipment(tables, sameorg);
                            }
                            else
                            {
                                MessageBox.Show("请修改单位名称");
                            }
                        }
                        else
                        {
                            ImportActivityEquipment(tables, activityorg);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// XLS导入
        /// </summary>
        /// <param name="orginfo"></param>
        /// <param name="lstequ"></param>
        private void ImportActivityEquipmentEvent(ActivityOrganization orginfo, List<ActivityEquipment> lstequ)
        {
            List<ActivityEquipment> lst = new List<ActivityEquipment>();

            BeOperationInvoker.Invoke<I_CO_IA_FreqStation>(channel =>
            {
                foreach (ActivityEquipment equ in lstequ)
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
                        channel.ImportActivityEquipment(orginfo, lstequ);
                        GetActivityOrgs(orgquerycondition);
                        ActivityOrganization selectorg = this.ActivityOrgSource.FirstOrDefault(r => r.Guid == orginfo.Guid);
                        this.orgdatagrid.SelectedItem = selectorg;
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
        private void btnEquDelete_Click(object sender, RoutedEventArgs e)
        {
            List<ActivityEquipment> equs = equipmentListControl.EquipmentItemsSource.Where(r => r.IsChecked == true).ToList();

            if (equs.Count == 0)
            {
                MessageBox.Show("请勾选要删除的设备");
                return;
            }
            List<string> guids = equs.Select(r => r.Key).ToList();

            MessageBoxResult result = MessageBox.Show("确认要删除勾选的单位", "提示", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    BeOperationInvoker.Invoke<I_CO_IA_FreqStation>(channel =>
                    {
                        channel.DeleteActivityEquipment(guids);
                    });

                    InitQueryCondition(eququerycondition);
                    GetActivityEquipments(eququerycondition);

                    MessageBox.Show("删除成功");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.GetExceptionMessage(), "删除失败");
                }
            }
        }

        /// <summary>
        /// XLS导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOutput_Click(object sender, RoutedEventArgs e)
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

        #endregion


        private object[,] ORGSourceToObject()
        {
            object[,] obj = new object[6, 1];
            if (SelectActivityORG != null)
            {
                obj[0, 0] = SelectActivityORG.Name;
                obj[1, 0] = SelectActivityORG.ShortName;
                obj[2, 0] = SelectActivityORG.SecurityClass.Name;
                obj[3, 0] = SelectActivityORG.Address;
                obj[4, 0] = SelectActivityORG.Contact;
                obj[5, 0] = SelectActivityORG.Phone;
            }
            return obj;
        }
        /// <summary>
        /// 查询设备
        /// </summary>
        /// <param name="condition"></param>
        private void GetActivityEquipments(EquipmentLoadStrategy condition)
        {
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqStation>(channel =>
            {
                ActivityEquipment[] sources = channel.GetActivityEquipments(condition);
                equipmentListControl.DataContext = sources;
            });
        }

        /// <summary>
        /// 保存设备方法
        /// </summary>
        /// <param name="equ"></param>
        /// <returns></returns>
        private bool SaveActivityEquipment(ActivityEquipment equ)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqStation, bool>(channel =>
            {
                //编号是否重复
                int count = channel.CountEquName(equ);
                if (count == 0)
                {
                    try
                    {
                        channel.SaveActivityEquipment(equ);
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
                    MessageBox.Show("当前单位名称下,存在相同的设备名称+设备编号,请重新输入", "保存失败");
                    return false;
                }
            });
        }

        #region XLS导入方法

        /// <summary>
        /// 加载单位信息
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="activityguid"></param>
        /// <returns></returns>
        public ActivityOrganization LoadActivityORGFromTable(DataTable dt, string activityguid)
        {
            ActivityOrganization org = null;
            if (dt.Rows != null && dt.Rows.Count > 0)
            {
                org = new ActivityOrganization();
                org.ActivityGuid = activityguid;
                org.Guid = System.Guid.NewGuid().ToString();
                if (dt.Columns[1] != null)
                {
                    org.Name = dt.Columns[1].ColumnName;
                }

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
        /// <param name="activityorg"></param>
        private void ImportActivityEquipment(DataTable[] tables, ActivityOrganization activityorg)
        {
            StringBuilder error = new StringBuilder();
            bool validresult = ExcelImportHelper.ValidateEquipmentTables(tables, out error); //验证设备信息
            if (validresult)
            {
                List<ActivityEquipment> lstequ = this.LoadActivityEquipments(tables, CurrentActivity.Guid, CurrentActivityPlace.Guid, activityorg.Guid);
                if (lstequ == null || lstequ.Count == 0)
                {
                    MessageBox.Show("请输入要导入的设备", "提示", MessageBoxButton.OK);
                    return;
                }
                else
                {
                    ImportActivityEquipmentEvent(activityorg, lstequ);
                }
            }
            else
            {
                ErrorDialog errordialog = new ErrorDialog(error.ToString());
                errordialog.ShowDialog();
                return;
            }
        }

        /// <summary>
        /// 加载设备信息
        /// </summary>
        /// <param name="tables"></param>
        /// <param name="activityguid"></param>
        /// <param name="placeguid"></param>
        /// <param name="orgguid"></param>
        /// <returns></returns>
        private List<ActivityEquipment> LoadActivityEquipments(DataTable[] tables, string activityguid, string placeguid, string orgguid)
        {
            StringBuilder errormsg = new StringBuilder();
            List<ActivityEquipment> equipments = new List<ActivityEquipment>();
            DataTable table = tables.FirstOrDefault(r => r.TableName == "设备信息");
            if (table != null && table.Rows.Count > 0)
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    DataRow dr = table.Rows[i];
                    if (string.IsNullOrEmpty(dr["设备名称"].ToString())) continue;
                    else
                    {
                        ActivityEquipment equ = CreateActivityEquipment(dr, activityguid, placeguid, orgguid);
                        equipments.Add(equ);
                    }

                }
            }
            return equipments;
        }

        /// <summary>
        /// 创建ActivityEquipment对象
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="activityguid"></param>
        /// <param name="placeguid"></param>
        /// <param name="orgguid"></param>
        /// <param name="classId"></param>
        /// <returns></returns>
        private ActivityEquipment CreateActivityEquipment(DataRow dr, string activityguid, string placeguid, string orgguid)
        {
            ActivityEquipment equ = new ActivityEquipment();
            equ.Key = System.Guid.NewGuid().ToString();
            equ.OrgInfo.Guid = orgguid;
            equ.ActivityGuid = activityguid;
            equ.PlaceGuid = placeguid;
            equ.Name = dr["设备名称"].ToString();
            equ.EquipmentClassID = ExcelImportHelper.GetEquipmentClassId(dr["业务类型"].ToString());
            equ.EQUCount = Int32.Parse(dr["数量"].ToString()); //设备数量
            equ.EquModel = dr["设备型号"].ToString();//设备型号
            equ.SeriesNumber = dr["设备编号"].ToString();//设备编号
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

        #endregion
    }
}
