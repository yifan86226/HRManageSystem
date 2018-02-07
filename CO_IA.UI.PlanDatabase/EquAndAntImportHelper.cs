using CO_IA.Data;
using CO_IA.Types;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;

namespace CO_IA.UI.PlanDatabase
{
    public class EquAndAntImportHelper
    {
        public static StatModeEnum StaMode { get; set; }

        static Dictionary<string, string> dicProvinceAreaCodes = CO_IA.Client.Utility.GetProvinceAreaCode();

        public static List<DataRow> GetEquEnableDataRow(DataTable[] tables)
        {
            List<DataRow> rows = new List<DataRow>();
            DataTable equtab = tables.FirstOrDefault(r => r.TableName.Contains("设备信息"));

            if (equtab != null && equtab.Rows.Count > 0)
            {
                int count = equtab.Columns.Count;
                for (int i = 0; i < equtab.Rows.Count; i++)
                {
                    DataRow dr = equtab.Rows[i];

                    object[] content = dr.ItemArray;
                    object obj = content.FirstOrDefault(r => !string.IsNullOrWhiteSpace(r.ToString())); //是否存在不为空的项
                    if (obj != null)
                    {
                        rows.Add(dr);
                    }
                }
            }
            return rows;
        }

        public static List<DataRow> GetAntEnableDataRow(DataTable[] tables)
        {
            List<DataRow> rows = new List<DataRow>();
            DataTable anttab = tables.FirstOrDefault(r => r.TableName == "天线信息");
            if (anttab != null && anttab.Rows.Count > 0)
            {
                int count = anttab.Columns.Count;
                for (int i = 0; i < anttab.Rows.Count; i++)
                {
                    DataRow dr = anttab.Rows[i];

                    object[] content = dr.ItemArray;
                    object obj = content.FirstOrDefault(r => !string.IsNullOrWhiteSpace(r.ToString())); //是否存在不为空的项
                    if (obj != null)
                    {
                        rows.Add(dr);
                    }
                }
            }
            return rows;
        }

        /// <summary>
        /// 获取设备信息
        /// </summary>
        /// <param name="tables"></param>
        /// <returns></returns>
        public static List<MonitorStationEquInfo> GetStationEquFromTable(List<DataRow> rows, out bool success)
        {
            success = true;
            List<MonitorStationEquInfo> stations = null;

            if (rows != null && rows.Count > 0)
            {
                if (VerifyEquAndAntName(rows, "设备"))
                {
                    if (VerifyEquProperty(rows))
                    {
                        stations = GetStationEqus(rows);
                    }
                    else
                    {
                        success = false;
                    }
                }
                else
                {
                    success = false;
                }
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("设备信息为空,请填写设备信息", "提示", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {

                }
                else
                {

                }
            }
            return stations;
        }

        /// <summary>
        /// 获取天线信息
        /// </summary>
        /// <param name="tables"></param>
        /// <returns></returns>
        public static List<MonitorStationAntInfo> GetStationAntFromTable(List<DataRow> rows, out bool success)
        {
            success = true;
            List<MonitorStationAntInfo> ants = null;

            if (rows != null && rows.Count > 0)
            {
                if (VerifyEquAndAntName(rows, "天线"))
                {
                    if (VerifyAntProperty(rows))
                    {
                        ants = GetStationAnts(rows);
                    }
                    else
                    {
                        success = false;
                    }
                }
                else
                {
                    success = false;
                }
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("天线信息为空,请填写天线信息?", "提示", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {

                }
                else
                {

                }
            }
            return ants;
        }

        private static List<MonitorStationEquInfo> GetStationEqus(List<DataRow> rows)
        {
            List<MonitorStationEquInfo> equs = new List<MonitorStationEquInfo>();
            for (int i = 0; i < rows.Count; i++)
            {
                MonitorStationEquInfo equ = CreateStationEqu(rows[i]);
                equs.Add(equ);
            }
            return equs;
        }

        private static List<MonitorStationAntInfo> GetStationAnts(List<DataRow> rows)
        {
            List<MonitorStationAntInfo> ants = new List<MonitorStationAntInfo>();
            for (int i = 0; i < rows.Count; i++)
            {
                MonitorStationAntInfo ant = CreateStationAnt(rows[i]);
                ants.Add(ant);
            }
            return ants;
        }

        private static MonitorStationEquInfo CreateStationEqu(DataRow dr)
        {
            MonitorStationEquInfo equ = new MonitorStationEquInfo();
            equ.ID = Guid.NewGuid().ToString();
            equ.StatMode = StaMode;
            equ.AreaCode = dr["地区编码"].ToString();
            equ.Name = dr["设备名称"].ToString();
            equ.Code = dr["设备编号"].ToString();//编号
            equ.ModelNo = dr["设备型号"].ToString(); //型号
            equ.Type = dr["设备类型"].ToString(); //类型
            equ.StartFreq = double.Parse(dr["起始频率(MHz)"].ToString());//起始频率
            equ.EndFreq = double.Parse(dr["终止频率(MHz)"].ToString());//终止频率

            if (string.IsNullOrWhiteSpace(dr["灵敏度(dBm)"].ToString()))
            {
                equ.Sensitivity = null; //灵敏度
            }
            else
            {
                equ.Sensitivity = double.Parse(dr["灵敏度(dBm)"].ToString()); //灵敏度
            }
            equ.SerialNumber = dr["设备串号"].ToString(); //串号
            equ.Supplier = dr["供应商"].ToString();//供应商
            equ.Memo = dr["备注"].ToString(); //备注
            return equ;
        }

        private static MonitorStationAntInfo CreateStationAnt(DataRow dr)
        {
            MonitorStationAntInfo ant = new MonitorStationAntInfo();
            ant.StatMode = StatModeEnum.Fixed;
            ant.ID = Guid.NewGuid().ToString();
            ant.Name = dr["天线名称"].ToString(); //名称
            ant.Code = dr["天线编号"].ToString(); //编号
            ant.ModelNo = dr["天线型号"].ToString();//型号
            ant.IsDirectional = dr["定向天线"].ToString() == "是"; //定向天线
            ant.IsActive = dr["有源天线"].ToString() == "是"; //定向天线
            PolarizationEnum polar;
            if (Enum.TryParse(dr["极化方式"].ToString(), out polar))
            {
                ant.Polarization = polar;
            }
            ant.StartFreq = double.Parse(dr["起始频率(MHz)"].ToString()); //起始频率
            ant.EndFreq = double.Parse(dr["终止频率(MHz)"].ToString()); //终止频率
            if (string.IsNullOrWhiteSpace(dr["天线高度(m)"].ToString()))
            {
                ant.Altitude = null;//天线高度(海拔)
            }
            else
            {
                ant.Altitude = double.Parse(dr["天线高度(m)"].ToString());//天线高度(海拔)
            }
            if (string.IsNullOrWhiteSpace(dr["接入损耗(dB)"].ToString()))
            {
                ant.AccessLoss = null; //接入损耗
            }
            else
            {
                ant.AccessLoss = double.Parse(dr["接入损耗(dB)"].ToString()); //接入损耗
            }
            if (string.IsNullOrWhiteSpace(dr["天线增益(dBi)"].ToString()))
            {
                ant.AntennaGain = null;
            }
            else
            {
                ant.AntennaGain = double.Parse(dr["天线增益(dBi)"].ToString()); //天线增益
            }
            if (string.IsNullOrWhiteSpace(dr["挂高(m)"].ToString()))
            {
                ant.AntennaHeight = null; //挂高
            }
            else
            {
                ant.AntennaHeight = double.Parse(dr["挂高(m)"].ToString()); //挂高
            }
            ant.SerialNumber = dr["串号"].ToString();//串号
            ant.Supplier = dr["供应商"].ToString(); //供应商
            ant.Memo = dr["备注"].ToString();//备注
            return ant;
        }

        private static bool VerifyEquAndAntName(List<DataRow> rows, string type)
        {
            for (int i = 0; i < rows.Count; i++)
            {
                DataRow dr = rows[i];
                string name;
                if (type == "设备")
                {
                    name = dr["设备名称"].ToString();
                }
                else
                {
                    name = dr["天线名称"].ToString();
                }

                if (string.IsNullOrWhiteSpace(name))
                {
                    MessageBox.Show(string.Format("存在名称为空的{0},请重新输入", type));
                    return false;
                }

                //StringBuilder msg = new StringBuilder();
                //List<string> lstsamename = new List<string>();
                //else
                //{
                //    if (ExistSameName(name, rows))
                //    {
                //        if (!lstsamename.Contains(name))
                //        {
                //            lstsamename.Add(name);
                //        }
                //    }
                //}
            }
            return true;

            //if (lstsamename.Count > 0)
            //{
            //    msg.AppendFormat("存在相同名称的{0}:", type);
            //    for (int i = 0; i < lstsamename.Count; i++)
            //    {
            //        msg.AppendFormat("{0}、", lstsamename[i]);
            //    }
            //    MessageBox.Show(msg.ToString().TrimEnd('、'));
            //    return false;
            //}
            //else
            //{
            //    return true;
            //}
        }

        private static bool VerifyEquProperty(List<DataRow> rows)
        {
            StringBuilder msg = new StringBuilder();
            bool verifyresult = true;
            for (int i = 0; i < rows.Count; i++)
            {
                DataRow dr = rows[i];

                StringBuilder error = new StringBuilder();
                if (VerifyEquDataRow(dr, out error))
                {

                }
                else
                {
                    verifyresult = false;
                    msg.AppendLine(string.Format("{0}验证失败", dr["设备名称"]));
                    msg.AppendLine(error.ToString().TrimEnd('、'));
                }
            }
            if (!verifyresult)
            {
                ErrorDialog error = new ErrorDialog(msg.ToString());
                error.ShowDialog();
            }
            return verifyresult;
        }

        private static bool VerifyAntProperty(List<DataRow> rows)
        {
            StringBuilder msg = new StringBuilder();
            bool verifyresult = true;
            for (int i = 0; i < rows.Count; i++)
            {
                DataRow dr = rows[i];
                StringBuilder error = new StringBuilder();
                if (VerifyAntDataRow(dr, out error))
                {

                }
                else
                {
                    verifyresult = false;
                    msg.AppendLine(string.Format("{0}验证失败", dr["天线名称"]));
                    msg.AppendLine(error.ToString().TrimEnd('、'));
                }
            }
            if (!verifyresult)
            {
                ErrorDialog error = new ErrorDialog(msg.ToString());
                error.ShowDialog();
            }
            return verifyresult;
        }
        private static bool VerifyEquDataRow(DataRow dr, out StringBuilder errormsg)
        {
            errormsg = new StringBuilder();
            bool result = true;
            if (!string.IsNullOrWhiteSpace(dr["设备名称"].ToString()))
            {
                //只有便携设备验证地区
                if (EquAndAntImportHelper.StaMode == StatModeEnum.Portable)
                {
                    if (string.IsNullOrWhiteSpace(dr["地区编码"].ToString()))
                    {
                        errormsg.Append("地区不能为空、");
                        result = false;
                    }
                    else
                    {
                        string areacode = dr["地区编码"].ToString();
                        if (!dicProvinceAreaCodes.Keys.Contains(areacode))
                        {
                            errormsg.Append(string.Format("地区编码{0}不可以导入,原因可能是,地区编码不存在或者没有导入其他地区的权限", areacode));
                            result = false;
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(dr["设备编号"].ToString()))
                {
                    errormsg.Append("设备编号不能为空、");
                    result = false;
                }
                if (string.IsNullOrWhiteSpace(dr["设备型号"].ToString()))
                {
                    errormsg.Append("设备型号不能为空、");
                    result = false;
                }
                if (string.IsNullOrWhiteSpace(dr["设备类型"].ToString()))
                {
                    errormsg.Append("设备类型不能为空、");
                    result = false;
                }
                if (string.IsNullOrWhiteSpace(dr["起始频率(MHz)"].ToString()))
                {
                    errormsg.Append("起始频率不能为空、");
                    result = false;
                }
                if (string.IsNullOrWhiteSpace(dr["终止频率(MHz)"].ToString()))
                {
                    errormsg.Append("终止频率不能为空、");
                    result = false;
                }
                if (!string.IsNullOrWhiteSpace(dr["起始频率(MHz)"].ToString()) && !string.IsNullOrWhiteSpace(dr["终止频率(MHz)"].ToString()))
                {
                    if (double.Parse(dr["起始频率(MHz)"].ToString()) > double.Parse(dr["终止频率(MHz)"].ToString()))
                    {
                        errormsg.Append(string.Format("起始频率{0}不能大于终止频率{1}、", dr["起始频率(MHz)"].ToString(), dr["终止频率(MHz)"].ToString()));
                        result = false;
                    }
                }
                if (string.IsNullOrWhiteSpace(dr["灵敏度(dBm)"].ToString()))
                {
                    errormsg.Append("灵敏度不能为空、");
                    result = false;
                }
            }
            else
            {
                errormsg.Append("存在名称为空的设备");
                result = false;
            }
            return result;
        }

        private static bool VerifyAntDataRow(DataRow dr, out StringBuilder errormsg)
        {
            errormsg = new StringBuilder();
            bool result = true;
            if (!string.IsNullOrWhiteSpace(dr["天线名称"].ToString()))
            {
                if (string.IsNullOrWhiteSpace(dr["天线编号"].ToString()))
                {
                    errormsg.Append("天线编号不能为空、");
                    result = false;
                }
                if (string.IsNullOrWhiteSpace(dr["天线型号"].ToString()))
                {
                    errormsg.Append("天线型号不能为空、");
                    result = false;
                }
                if (string.IsNullOrWhiteSpace(dr["定向天线"].ToString()))
                {
                    errormsg.Append("请选择是否定向天线、");
                    result = false;
                }
                if (string.IsNullOrWhiteSpace(dr["有源天线"].ToString()))
                {
                    errormsg.Append("请选择是否有源天线、");
                    result = false;
                }

                if (string.IsNullOrWhiteSpace(dr["极化方式"].ToString()))
                {
                    errormsg.Append("极化方式不能为空、");
                    result = false;
                }
                if (string.IsNullOrWhiteSpace(dr["起始频率(MHz)"].ToString()))
                {
                    errormsg.Append("起始频率不能为空、");
                    result = false;
                }
                if (string.IsNullOrWhiteSpace(dr["终止频率(MHz)"].ToString()))
                {
                    errormsg.Append("终止频率不能为空、");
                    result = false;
                }
                if (!string.IsNullOrWhiteSpace(dr["起始频率(MHz)"].ToString()) && !string.IsNullOrWhiteSpace(dr["终止频率(MHz)"].ToString()))
                {
                    if (double.Parse(dr["起始频率(MHz)"].ToString()) > double.Parse(dr["终止频率(MHz)"].ToString()))
                    {
                        errormsg.Append(string.Format("起始频率{0}不能大于终止频率{1}、", dr["起始频率(MHz)"].ToString(), dr["终止频率(MHz)"].ToString()));
                        result = false;
                    }
                }
                if (string.IsNullOrWhiteSpace(dr["天线高度(m)"].ToString()))
                {
                    errormsg.Append("天线高度不能为空、");
                    result = false;
                }
            }
            else
            {
                errormsg.Append("存在名称为空的天线");
                result = false;
            }
            return result;
        }

        public static bool ImportEquipments(List<MonitorStationEquInfo> equs)
        {
            List<MonitorStationEquInfo> samenameequ = GetEqusInDB(equs);
            if (samenameequ.Count == 0)
            {
                return ImportEquEvent(equs);
            }
            else
            {
                StringBuilder error = new StringBuilder();
                error.AppendLine("数据库中存在以下设备名称和编号:");
                foreach (MonitorStationEquInfo equ in samenameequ)
                {
                    error.AppendLine(string.Format("设备名称:{0},设备编号:{1}", equ.Name, equ.Code));
                }
                error.AppendLine("是否进行替换?");

                MessageBoxResult msgresult = MessageBox.Show(error.ToString(), "提示", MessageBoxButton.YesNo);

                if (msgresult == MessageBoxResult.Yes)
                {
                    return ImportEquEvent(equs);
                }
                return false;
                //ErrorDialog dialog = new ErrorDialog(error.ToString());
                //dialog.ShowDialog();
            }
        }

        public static bool ImportAnts(List<MonitorStationAntInfo> ants)
        {
            List<MonitorStationAntInfo> sameants = GetAntsInDB(ants);

            if (sameants.Count == 0)
            {
                return ImportAntEvent(ants);
            }
            else
            {
                StringBuilder error = new StringBuilder();
                error.AppendLine("数据库中存在以下天线名称和编号:");
                foreach (MonitorStationAntInfo ant in sameants)
                {
                    error.AppendLine(string.Format("天线名称:{0},天线编号:{1}", ant.Name, ant.Code));
                }
                error.AppendLine("是否进行替换?");

                MessageBoxResult msgresult = MessageBox.Show(error.ToString(), "提示", MessageBoxButton.YesNo);

                if (msgresult == MessageBoxResult.Yes)
                {
                    return ImportAntEvent(ants);
                }
                return false;
                //ErrorDialog errordialog = new ErrorDialog(error.ToString());
                //errordialog.ShowDialog();
            }
        }

        private static List<MonitorStationEquInfo> GetEqusInDB(List<MonitorStationEquInfo> equs)
        {
            List<MonitorStationEquInfo> sameequs = new List<MonitorStationEquInfo>();

            //foreach (MonitorStationEquInfo equ in equs)
            //{
            //    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_PlanDatabase>(channel =>
            //    {
            //        int count = channel.CountMonitorEQUName(equ);
            //        if (count > 0)
            //        {
            //            sameequs.Add(equ);
            //        }
            //    });
            //}
            return sameequs;
        }

        private static List<MonitorStationAntInfo> GetAntsInDB(List<MonitorStationAntInfo> ants)
        {
            List<MonitorStationAntInfo> sameants = new List<MonitorStationAntInfo>();
            //foreach (MonitorStationAntInfo ant in ants)
            //{
            //    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_PlanDatabase>(channel =>
            //    {
            //        int count = channel.CountMonitorANTName(ant);
            //        if (count > 0)
            //        {
            //            sameants.Add(ant);
            //        }
            //    });
            //}
            return sameants;
        }

        private static bool ImportEquEvent(List<MonitorStationEquInfo> equs)
        {
            try
            {
                //PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_PlanDatabase>(channel =>
                //{
                //    channel.ImportStationEqus(equs);
                //});
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetExceptionMessage());
                return false;
            }
        }

        private static bool ImportAntEvent(List<MonitorStationAntInfo> ants)
        {
            try
            {
                //PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_PlanDatabase>(channel =>
                //{
                //    channel.ImportStationAnts(ants);
                //});
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetExceptionMessage());
                return false;
            }
        }
    }
}
