using CO_IA.Data;
using CO_IA.Data.Setting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace CO_IA.UI.PlanDatabase
{
    public class ExamPlaceImportHelper
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
            DataTable examtab = tables.FirstOrDefault(r => r.TableName.Contains("考点信息"));
            if (examtab != null && examtab.Rows.Count > 1)
            {
                for (int i = 0; i < examtab.Rows.Count; i++)
                {
                    DataRow row = examtab.Rows[i];

                    int count = examtab.Columns.Count;
                    if (!IsNullRow(row, count))
                    {
                        rows.Add(row);
                    }
                }

            }
            return rows;
        }
        /// <summary>
        /// 获取考点信息
        /// </summary>
        /// <param name="tables"></param>
        /// <returns></returns>
        public static List<ExamPlace> GetExamPlaceFromTable(List<DataRow> rows, out bool success)
        {
            success = true;
            List<ExamPlace> examPlaces = null;
            if (VerifyExamPlaceProperty(rows))
            {
                examPlaces = GetExamPlaces(rows);
            }
            else
            {
                success = false;
            }

            return examPlaces;
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
        private static bool VerifyExamPlaceProperty(List<DataRow> rows)
        {
            StringBuilder msg = new StringBuilder();
            bool verifyresult = true;
            for (int i = 0; i < rows.Count; i++)
            {
                DataRow dr = rows[i];

                StringBuilder error = new StringBuilder();
                if (VerifyExamPlaceDataRow(dr, out error))
                {

                }
                else
                {
                    verifyresult = false;
                    msg.AppendLine(string.Format("{0}验证失败", dr[1]));
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
        private static bool VerifyExamPlaceDataRow(DataRow dr, out StringBuilder errormsg)
        {
            errormsg = new StringBuilder();
            bool result = true;
            if (!string.IsNullOrEmpty(dr["考点名称"].ToString()))
            {
            }
            else
            {
                errormsg.Append("考点名称不能为空");
                result = false;
            }
            if (!string.IsNullOrEmpty(dr["考点地址"].ToString()))
            {
            }
            else
            {
                errormsg.Append("考点地址不能为空");
                result = false;
            }
            if (!string.IsNullOrEmpty(dr["联系人"].ToString()))
            {
            }
            else
            {
                errormsg.Append("联系人不能为空");
                result = false;
            }
            if (!string.IsNullOrEmpty(dr["联系电话"].ToString()))
            {
            }
            else
            {
                errormsg.Append("联系电话不能为空");
                result = false;
            }
            if (!string.IsNullOrEmpty(dr["所属地区编码"].ToString()))
            {
                string areacode = dr["所属地区编码"].ToString();
                if (!dicProvinceAreaCodes.Keys.Contains(areacode))
                {
                    errormsg.Append(string.Format("所属地区编码{0}不可以导入,原因可能是,地区编码不存在或者没有导入其他地区的权限", areacode));
                    result = false;
                }
            }
            else
            {
                errormsg.Append("所属地区不能为空");
                result = false;
            }
            if (!string.IsNullOrEmpty(dr["图片名称"].ToString()))
            {
                if (string.IsNullOrEmpty(dr["图片类型"].ToString()))
                {
                    errormsg.Append("图片不为空时，图片类型也不能为空");
                    result = false;
                }
                else
                {
                    string[] imgPaths = dr["图片名称"].ToString().Split('|');
                    string[] imgTypes = dr["图片类型"].ToString().Split('|');
                    if (imgPaths.Length != imgTypes.Length)
                    {
                        errormsg.Append("图片数量与图片类型数量不匹配");
                        result = false;
                    }
                }

            }
            return result;
        }
        private static List<ExamPlace> GetExamPlaces(List<DataRow> rows)
        {
            List<ExamPlace> ants = new List<ExamPlace>();
            for (int i = 0; i < rows.Count; i++)
            {
                ExamPlace ant = CreateExamPlace(rows[i]);
                if (ant != null)
                {
                    ants.Add(ant);
                }
            }
            return ants;
        }

        private static ExamPlace CreateExamPlace(DataRow dr)
        {
            if (!string.IsNullOrWhiteSpace(dr["考点名称"].ToString()))
            {
                ExamPlace examPlace = new ExamPlace();
                examPlace.Guid = Guid.NewGuid().ToString();
                examPlace.Name = dr["考点名称"].ToString();

                if (!string.IsNullOrEmpty(dr["考点地址"].ToString()))
                {
                    examPlace.Address = dr["考点地址"].ToString();
                }
                else
                {
                    examPlace.Address = null;
                }
                if (!string.IsNullOrEmpty(dr["联系人"].ToString()))
                {
                    examPlace.Contact = dr["联系人"].ToString();
                }
                else
                {
                    examPlace.Contact = null;
                }
                if (!string.IsNullOrEmpty(dr["联系电话"].ToString()))
                {
                    examPlace.Phone = dr["联系电话"].ToString();
                }
                else
                {
                    examPlace.Phone = null;
                }
                if (!string.IsNullOrEmpty(dr["经度"].ToString()))
                {
                    examPlace.Location_lg = dr["经度"].ToString();
                }
                else
                {
                    examPlace.Location_lg = null;
                }
                if (!string.IsNullOrEmpty(dr["纬度"].ToString()))
                {
                    examPlace.Location_la = dr["纬度"].ToString();
                }
                else
                {
                    examPlace.Location_la = null;
                }
                if (!string.IsNullOrEmpty(dr["所属地区编码"].ToString()))
                {
                    examPlace.Areacode = dr["所属地区编码"].ToString();
                }
                else
                {
                    examPlace.Location_la = null;
                }
                if (!string.IsNullOrEmpty(dr["备注"].ToString()))
                {
                    examPlace.Remark = dr["备注"].ToString();
                }
                else
                {
                    examPlace.Remark = null;
                }
                if (!string.IsNullOrEmpty(dr["图片名称"].ToString()))
                {
                    string[] imgPaths = dr["图片名称"].ToString().Split('|');
                    for (int i = 0; i < imgPaths.Length; i++)
                    {
                        ActivityPlaceLocationImage activityPlaceLocationImage = new ActivityPlaceLocationImage();
                        string path = ImageFilePath + "\\" + imgPaths[i];
                        byte[] imageData = File.ReadAllBytes(path);
                        byte[] imageOut = AT_BC.Common.ImageZipper.ZipAsJpg(imageData, 800, 600);
                        activityPlaceLocationImage.GUID = Guid.NewGuid().ToString();
                        activityPlaceLocationImage.ACTIVITY_PLACE_LOCATION_GUID = examPlace.Guid;
                        activityPlaceLocationImage.Image = imageOut;
                        activityPlaceLocationImage.TYPE = Convert.ToInt32(dr["图片类型"].ToString().Split('|')[i]);

                        examPlace.Images.Add(activityPlaceLocationImage);
                    }
                }

                return examPlace;
            }
            else
            {
                return null;
            }
        }
        private static string GetAreaCode(string name)
        {
            Dictionary<string, string> dicarea = CO_IA.Client.Utility.GetProvinceAreaCode();
            return dicarea.FirstOrDefault(r => r.Value == name).Key;
        }

    }
}
