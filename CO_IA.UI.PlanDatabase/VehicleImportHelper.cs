using CO_IA.Client;
using CO_IA.Data;

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace CO_IA.UI.PlanDatabase
{
    public class VehicleImportHelper
    {
        static Dictionary<string, string> dicProvinceAreaCodes = CO_IA.Client.Utility.GetProvinceAreaCode();

        public static string ImageFilePath
        {
            get;
            set;
        }

        public static List<DataRow> GetEnableDataRow(DataTable[] tables)
        {
            List<DataRow> rows = new List<DataRow>();
            DataTable vehicletab = tables.FirstOrDefault(r => r.TableName == "车辆信息");
            if (vehicletab != null && vehicletab.Rows.Count > 0)
            {
                for (int i = 0; i < vehicletab.Rows.Count; i++)
                {
                    DataRow row = vehicletab.Rows[i];

                    object[] content = row.ItemArray;
                    object obj = content.FirstOrDefault(r => !string.IsNullOrWhiteSpace(r.ToString())); //是否存在不为空的项

                    if (obj != null)
                    {
                        rows.Add(row);
                    }

                    //int count = vehicletab.Columns.Count;
                    //if (!IsNullRow(row, count))
                    //{
                    //    rows.Add(row);
                    //}
                }
            }
            return rows;
        }

        /// <summary>
        /// 获取设备信息
        /// </summary>
        /// <param name="tables"></param>
        /// <returns></returns>
        public static List<VehicleInfo> GetVehicleFromTable(List<DataRow> rows, out bool success)
        {
            success = true;
            List<VehicleInfo> vehicles = null;
            if (VerifyVehicleNo(rows))
            {
                if (VerifyVehicleProperty(rows))
                {
                    vehicles = GetVehicles(rows);
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
            return vehicles;
        }

        private static bool IsNullRow(DataRow row, int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (!string.IsNullOrWhiteSpace(row[i].ToString()))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool VerifyVehicleNo(List<DataRow> rows)
        {
            StringBuilder msg = new StringBuilder();
            List<string> lstsameno = new List<string>();

            for (int i = 0; i < rows.Count; i++)
            {
                DataRow dr = rows[i];

                string name = dr["车牌号码"].ToString();
                if (string.IsNullOrEmpty(name))
                {
                    MessageBox.Show(string.Format("车牌号码为空的,请重新输入"));
                    return false;
                }
                else
                {
                    if (ExistSameVehicleNo(name, rows))
                    {
                        if (!lstsameno.Contains(name))
                        {
                            lstsameno.Add(name);
                        }
                    }
                }
            }

            if (lstsameno.Count > 0)
            {
                msg.AppendFormat("Excel中存在相同的车牌号码:");
                for (int i = 0; i < lstsameno.Count; i++)
                {
                    msg.AppendFormat("{0}、", lstsameno[i]);
                }
                MessageBox.Show(msg.ToString().TrimEnd('、'));
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 在数据库中,是否存在相同车牌号码
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static bool VerifyVehicleNotInDB(List<VehicleInfo> vehicles, out  List<VehicleInfo> samenolst)
        {
            samenolst = new List<VehicleInfo>();
    
            //foreach (VehicleInfo vehicle in vehicles)
            //{
            //    int count = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_PlanDatabase, int>
            //        (channel =>
            //        {
            //            return channel.GetVehicleNoCount(vehicle.GUID, vehicle.VehicleNo);
            //        });

            //    if (count > 0)
            //    {
            //        samenolst.Add(vehicle);
            //    }
            //}
          
            if (samenolst.Count > 0)
            {
              
                return false;
            }
            else
            {
                return true;
            }
        }

        private static bool VerifyVehicleProperty(List<DataRow> rows)
        {
            StringBuilder msg = new StringBuilder();
            bool verifyresult = true;
            for (int i = 0; i < rows.Count; i++)
            {
                DataRow dr = rows[i];

                StringBuilder error = new StringBuilder();
                if (VerifyVehicleDataRow(dr, out error))
                {

                }
                else
                {
                    verifyresult = false;
                    msg.AppendLine(string.Format("{0}验证失败", dr["车牌号码"]));
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

        private static bool VerifyVehicleDataRow(DataRow dr, out StringBuilder errormsg)
        {
            errormsg = new StringBuilder();
            bool result = true;
            string areacode;

            if (!string.IsNullOrEmpty(dr["车牌号码"].ToString()))
            {
                if (string.IsNullOrEmpty(dr["地区编码"].ToString()))
                {
                    errormsg.Append("地区编码不能为空");
                    result = false;
                }
                else
                {
                    areacode = dr["地区编码"].ToString();
                    if (!dicProvinceAreaCodes.Keys.Contains(areacode))
                    {
                        errormsg.Append(string.Format("地区编码{0}不可以导入,原因可能是,地区编码不存在或者没有导入其他地区的权限", areacode));
                        result = false;
                    }
                }
                if (string.IsNullOrEmpty(dr["监测车"].ToString()))
                {
                    errormsg.Append("请选择是否为监测车");
                    result = false;
                }
            }
            else
            {
                errormsg.Append("存在名称为空的车辆");
                result = false;
            }
            return result;
        }

        private static List<VehicleInfo> GetVehicles(List<DataRow> rows)
        {
            List<VehicleInfo> ants = new List<VehicleInfo>();
            for (int i = 0; i < rows.Count; i++)
            {
                VehicleInfo ant = CreateVehicle(rows[i]);
                if (ant != null)
                {
                    ants.Add(ant);
                }
            }
            return ants;
        }

        private static VehicleInfo CreateVehicle(DataRow dr)
        {
            if (!string.IsNullOrWhiteSpace(dr["车牌号码"].ToString()))
            {
                VehicleInfo vehicle = new VehicleInfo();
                vehicle.AreaCode = (dr["地区编码"].ToString());
                vehicle.VehicleNo = dr["车牌号码"].ToString();
                vehicle.VehicleModel = dr["车辆型号"].ToString();
                vehicle.IsMonitor = dr["监测车"].ToString() == "是" ? true : false; //监测车
                vehicle.Driver = dr["司机"].ToString();//司机
                vehicle.Phone = dr["手机号码"].ToString();//联系电话
                vehicle.Comment = dr["备注"].ToString();//备注

                if (!string.IsNullOrEmpty(dr["照片"].ToString()))
                {
                    string path = ImageFilePath + "\\" + dr["照片"].ToString();
                    if (File.Exists(path))
                    {
                        byte[] imageData = File.ReadAllBytes(path);
                        byte[] imageOut = AT_BC.Common.ImageZipper.ZipAsJpg(imageData, 800, 600);
                        vehicle.Picture = imageOut;
                    }
                    else
                    {
                        //车辆照片不存在
                    }
                }
                return vehicle;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 验证名称是否重复
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dt"></param>
        /// <param name="nameindex">name所在的列.固定站name在第0列。设备及天线在第1列</param>
        /// <returns></returns>
        private static bool ExistSameVehicleNo(string name, List<DataRow> rows)
        {
            int count = 0;
            for (int i = 0; i < rows.Count; i++)
            {
                DataRow dr = rows[i];
                if (dr["车牌号码"].ToString() == name)
                {
                    count++;
                }
            }

            if (count > 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
 
    }
}
