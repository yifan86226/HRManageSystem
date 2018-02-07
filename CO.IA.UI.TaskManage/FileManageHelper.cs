using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CO_IA.Data.FileManage;
using CO_IA.Data.TaskManage;
using I_CO_IA.RuleAndFileManage;

namespace CO.IA.UI.TaskManage
{
   public class FileManageHelper
   {
       #region 相关文件、制度管理
       /// <summary>
       /// 获取全部，制度和文件信息
       /// </summary>
       /// <returns></returns>
       public static RegulationsInfo[] GetRegulationsInfo(string activityid )
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
           return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.RuleAndFileManage.I_CO_IA_RuleAndFile, CO_IA.Data.TaskManage.RuleFile[]>(
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
