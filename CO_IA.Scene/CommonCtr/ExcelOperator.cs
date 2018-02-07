using AT_BC.Data;
using CO_IA.Data;
using CO_IA.UI.PlanDatabase;
using CO_IA.UI.PlanDatabase.ORG;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;

namespace CO_IA.Scene.CommonCtr
{
    public class ExcelOperator
    {
        private static Action<ActivityOrganization, List<ActivityEquipment>> ImportEquipmentDelegate;
        public static void Import(Action<ActivityOrganization, List<ActivityEquipment>> p_importEquips)
        {
            ImportEquipmentDelegate = p_importEquips;
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
                        activityorg = LoadActivityORGFromTable(orgtable, SystemLoginService.CurrentActivity.Guid);
                    }
                    if (activityorg != null)
                    {
                        #region 验证单位名称
                        OrgQueryCondition condition = new OrgQueryCondition();
                        condition.ActivityGuid = SystemLoginService.CurrentActivity.Guid;
                        condition.Name = activityorg.Name;
                        ActivityOrganization[] orgs = DataOperator.GetActivityOrgSources(condition);
                        ActivityOrganization sameorg = orgs.FirstOrDefault(r => r.Name == activityorg.Name);
                        #endregion

                        if (sameorg != null)
                        {
                            MessageBoxResult result = MessageBox.Show(string.Format("单位'{0}'已经存在,是否将Excel中的设备导入到现有的单位中?", activityorg.Name), "提示", MessageBoxButton.YesNo);
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
        private static void ImportActivityEquipment(DataTable[] tables, ActivityOrganization activityorg)
        {
            StringBuilder error = new StringBuilder();
            bool validresult = ExcelImportHelper.ValidateEquipmentTables(tables, out error); //验证设备信息
            if (validresult)
            {
                List<ActivityEquipment> lstequ = LoadActivityEquipments(tables, SystemLoginService.CurrentActivity.Guid, SystemLoginService.CurrentActivityPlace.Guid, activityorg.Guid);
                if (lstequ == null || lstequ.Count == 0)
                {
                    MessageBox.Show("请输入要导入的设备", "提示", MessageBoxButton.OK);
                    return;
                }
                else
                {
                    ImportEquipmentDelegate(activityorg, lstequ);
                }
            }
            else
            {
                ErrorDialog errordialog = new ErrorDialog(error.ToString());
                errordialog.ShowDialog();
                return;
            }
        }
        private static List<ActivityEquipment> LoadActivityEquipments(DataTable[] tables, string activityguid, string placeguid, string orgguid)
        {
            StringBuilder errormsg = new StringBuilder();
            List<ActivityEquipment> equipments = new List<ActivityEquipment>();
            //0:单位信息
            //1:业务类型
            for (int i = 0; i < tables.Length; i++)
            {
                DataTable table = tables[i];
                string tablename = table.TableName;
                if (tablename == "单位信息" || tablename == "业务类型")
                {
                    continue;
                }

                string tabname = ExcelImportHelper.TableNameConvert(table.TableName);
                string classId = ExcelImportHelper.GetEquipmentClassId(tabname);
                if (!string.IsNullOrEmpty(classId))
                {
                    List<ActivityEquipment> equs = LoadActivityEquFromTable(table, activityguid, placeguid, orgguid, classId);
                    equipments.AddRange(equs);
                }
            }
            return equipments;
        }
        private static List<ActivityEquipment> LoadActivityEquFromTable(DataTable dt, string activityguid, string placeguid, string orgguid, string classId)
        {
            List<ActivityEquipment> equs = new List<ActivityEquipment>();
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 1; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    if (string.IsNullOrEmpty(dr[0].ToString())) continue;
                    else
                    {
                        ActivityEquipment equ = CreateActivityEquipment(dr, activityguid, placeguid, orgguid, classId);
                        equs.Add(equ);
                    }
                }
            }
            return equs;
        }

        private static ActivityEquipment CreateActivityEquipment(DataRow dr, string activityguid, string placeguid, string orgguid, string classId)
        {
            ActivityEquipment equ = new ActivityEquipment();
            equ.Key = System.Guid.NewGuid().ToString();
            equ.OrgInfo.Guid = orgguid;
            equ.ActivityGuid = activityguid;
            equ.PlaceGuid = placeguid;
            equ.Name = dr[0].ToString();
            equ.EquipmentClassID = classId;
            equ.EQUCount = Int32.Parse(dr[1].ToString()); //设备数量
            equ.EquModel = dr[2].ToString();//设备型号
            equ.SeriesNumber = dr[3].ToString();//设备序列号
            equ.IsMobile = dr[4].ToString() == "是" ? true : false; //移动设备
            if (!equ.IsMobile)
            {
                equ.Longitude = double.Parse(dr[5].ToString());
                equ.Latitude = double.Parse(dr[6].ToString());
                equ.Address = dr[7].ToString();
            }
            equ.IsStation = dr[8].ToString() == "是" ? true : false;
            if (equ.IsStation)
            {
                equ.StationName = dr[9].ToString();
                equ.StationTDI = dr[10].ToString();
            }
            else
            {
                equ.StationName = string.Empty;
            }
            equ.SendFreq = double.Parse(dr[11].ToString()); //发射频率
            if (string.IsNullOrEmpty(dr[12].ToString()))
            {
                equ.ReceiveFreq = null;//接收频率
            }
            else
            {
                equ.ReceiveFreq = double.Parse(dr[12].ToString());//接收频率
            }

            if (string.IsNullOrEmpty(dr[13].ToString()))
            {
                equ.SpareFreq = null;//备用频率
            }
            else
            {
                equ.SpareFreq = double.Parse(dr[13].ToString());//接收频率
            }

            equ.IsTunable = dr[14].ToString() == "是" ? true : false;//频率可调
            if (equ.IsTunable)
            {
                if (string.IsNullOrEmpty(dr[15].ToString()))
                {

                    equ.FreqRange.Little = null;//发射频率范围起始
                }
                else
                {
                    equ.FreqRange.Little = double.Parse(dr[15].ToString());//发射频率范围起始

                }
                if (string.IsNullOrEmpty(dr[16].ToString()))
                {
                    equ.FreqRange.Great = null;//发射频率范围终止
                }
                else
                {
                    equ.FreqRange.Great = double.Parse(dr[16].ToString());//发射频率范围终止
                }
            }
            else
            {
                equ.FreqRange = new Range<double?>();
            }

            equ.Band_kHz = double.Parse(dr[17].ToString());//必要带宽
            equ.Power_W = double.Parse(dr[18].ToString());//发射功率
            EMCS.Types.EMCModulationEnum modulat;
            if (Enum.TryParse(dr[19].ToString(), out modulat))
            {
                equ.Modulation = modulat;
            }
            equ.Remark = dr[20].ToString();
            return equ;
        }

        public static ActivityOrganization LoadActivityORGFromTable(DataTable dt, string activityguid)
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

    }
}
