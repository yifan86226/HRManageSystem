using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CO_IA.Data.FileManage;
using CO_IA.Data.TaskManage;
using I_CO_IA.RuleAndFileManage;
using I_CO_IA.ActivityManage;
using PT_BS_Service.Client.Framework;
using System.Collections.ObjectModel;

namespace CO_IA.UI.ActivityManage.FileManage
{
    public class FileManageHelper
    {
        /// <summary>
        /// 查询目录集合
        /// </summary>
        /// <param name="activityid"></param>
        /// <returns></returns>
        public static List<CatalogInfo> QueryCatalogs(string activityid)
        {
            return BeOperationInvoker.Invoke<I_CO_IA_ActivityManage, List<CatalogInfo>>(
                channel =>
                {
                    return channel.GetCatalogs(activityid);
                });
        }

        /// <summary>
        /// 保存目录信息
        /// </summary>
        /// <param name="catalog"></param>
        public static bool SaveCatalog(CatalogInfo catalog, out string errormsg)
        {
            errormsg = null;
            try
            {
                BeOperationInvoker.Invoke<I_CO_IA_ActivityManage>(channel =>
                {
                    channel.SaveCatalog(catalog);
                });
                return true;
            }
            catch (Exception ex)
            {
                errormsg = ex.GetExceptionMessage();
                return false;
            }
        }

        /// <summary>
        /// 删除目录信息
        /// </summary>
        /// <param name="guids"></param>
        /// <returns></returns>
        public static string DeleteCatalog(List<string> guids)
        {
            try
            {
                return BeOperationInvoker.Invoke<I_CO_IA_ActivityManage, string>(channel =>
                  {
                      return channel.DeleteCatalog(guids);
                  });

            }
            catch (Exception ex)
            {
                return ex.GetExceptionMessage();
            }
        }

        /// <summary>
        /// 查询工作文件
        /// </summary>
        /// <param name="catalog"></param>
        /// <returns></returns>
        public static List<WorkFileInfo> GetWorkFiles(string activityguid, string catalogguid)
        {
            return BeOperationInvoker.Invoke<I_CO_IA_ActivityManage, List<WorkFileInfo>>(channel =>
            {
                return channel.GetWorkFile(activityguid, catalogguid);
            });
        }

        /// <summary>
        /// 保存工作文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="errormsg"></param>
        /// <returns></returns>
        public static bool SaveWorkFiles(WorkFileInfo file, out string errormsg)
        {
            errormsg = null;
            try
            {
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_ActivityManage>(channel =>
                {
                    channel.SaveWorkFile(file);
                });
                return true;
            }
            catch (Exception ex)
            {

                errormsg = ex.GetExceptionMessage();
                return false;
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="guids"></param>
        /// <param name="errormsg"></param>
        /// <returns></returns>
        public static bool DeleteWorkFile(List<string> guids, out string errormsg)
        {
            errormsg = null;
            try
            {
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_ActivityManage>(channel =>
                {
                    channel.DeleteWorkFile(guids);
                });
                return true;
            }
            catch (Exception ex)
            {

                errormsg = ex.GetExceptionMessage();
                return false;
            }
        }

        /// <summary>
        /// 查询附件
        /// </summary>
        /// <param name="fileguid"></param>
        /// <returns></returns>
        public static ObservableCollection<FileAttachment> QueryFileAttachments(string fileguid)
        {
            List<FileAttachment> atts =
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_ActivityManage, List<FileAttachment>>(channel =>
                {
                    return channel.GetFileAttachment(fileguid);
                });
            return new ObservableCollection<FileAttachment>(atts);
        }

        public static bool SaveFileAttachment(List<FileAttachment> atts, out string errormsg)
        {
            errormsg = string.Empty;
            try
            {
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_ActivityManage>(channel =>
                {
                    channel.SaveFileAttachment(atts);
                });
                return true;
            }
            catch (Exception ex)
            {
                errormsg = ex.GetExceptionMessage();
                return false;
            }
        }



        #region 相关文件、制度管理
        /// <summary>
        /// 获取全部，制度和文件信息
        /// </summary>
        /// <returns></returns>
        public static RegulationsInfo[] GetRegulationsInfo(string activityid)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.RuleAndFileManage.I_CO_IA_RuleAndFile, CO_IA.Data.FileManage.RegulationsInfo[]>(
               channel =>
               {
                   return channel.GetRegulationsInfo(activityid);
               });
        }
        #endregion

        #region 附件相关
        /// <summary>
        /// 根据外键获取附件列表
        /// </summary>
        /// <param name="mainGuid"></param>
        /// <returns></returns>
        public static RuleFile[] GetRuleFiles(string mainGuid)
        {

            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.RuleAndFileManage.I_CO_IA_RuleAndFile, RuleFile[]>(
                channel =>
                {
                    return channel.GetRulesFile(mainGuid);
                });
        }
        #endregion

        #region 上传路径
        /// <summary>
        /// 格式化当前时间: 
        /// 1:yyMMddHHmmss; 2:yyyy-MM\dd\
        /// </summary>
        /// <returns></returns>
        public static string FormatNowTime(int num)
        {
            if (num == 1)
            {
                return DateTime.Now.ToString("yyMM");
            }
            else if (num == 2)
            {
                return DateTime.Now.ToString("yyyy-MM");
            }
            return "";
        }

        /// <summary>
        /// 项目所在路径
        /// </summary>
        /// <returns></returns>
        //public static string GetPath()
        //{
        //    string savePath = @"\SysDoc\" + FormatNowTime(2) + @"\";
        //    string allPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        //    int proPathLength = allPath.Substring(allPath.LastIndexOf("Output") - 1).Length;
        //    string proPath = allPath.Substring(0, allPath.Length - proPathLength);
        //    string path = proPath + @"\Output\" + "textoffice" + savePath;
        //    if (!File.Exists(path))
        //    {
        //        Directory.CreateDirectory(path);
        //    }
        //    return path;
        //}
        public static string GetPath()
        {
            string savePath = @"FileManage\" + FormatNowTime(2) + @"\";
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, savePath);// proPath + @"\Output\" + "textoffice" + savePath;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }
        /// <summary>
        /// 二进制数据转换为word文件(缓存到服务器)
        /// </summary>
        /// <param name="data">二进制数据</param>
        /// <param name="fileName">word文件名</param>
        /// <returns>文件保存的相对路径</returns>
        public static string ByteConvertDocService(byte[] data, string fileName)
        {
            FileStream fs;
            string savePath = GetPath();
            if (!System.IO.Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            savePath += fileName;
            if (System.IO.File.Exists(savePath))
            {
                //文件已经存在
                fs = new FileStream(savePath, FileMode.Truncate);
            }
            else
            {
                //先删除文件
                System.IO.File.Delete(savePath);
                //重新创建
                fs = new FileStream(savePath, FileMode.CreateNew);
            }
            BinaryWriter br = new BinaryWriter(fs);
            br.Write(data, 0, data.Length);
            br.Flush();
            br.Close();
            fs.Close();
            return savePath;
        }
        #endregion
    }
}
