#region 文件描述
/***************************************************************#@#***************************************************************
 * 创建人：Lizk
 * 摘 要 ：控制按钮ImageSource键值定义,新的资源文件应按照该键值创建资源,使用者又会按照该键值提取资源
 * 日 期 ：2016-08-12
 ***************************************************************#@#***************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Client
{
    /// <summary>
    /// 控制按钮ImageSource键值定义,新的资源文件应按照该键值创建资源,使用者又会按照该键值提取资源
    /// </summary>
    public static class ControlButtonImageKeys
    {
        /// <summary>
        /// 关闭按钮图片资源键值
        /// </summary>
        public const string Close="ControlImage.Close";

        /// <summary>
        /// 帮助按钮图片资源键值
        /// </summary>
        public const string Help="ControlImage.Help";

        /// <summary>
        /// 最小化按钮图片资源键值
        /// </summary>
        public const string Min="ControlImage.Min";

        /// <summary>
        /// 设置按钮图片资源键值
        /// </summary>
        public const string Setting="ControlImage.Setting";

        /// <summary>
        /// 预案库按钮图片资源键值
        /// </summary>
        public const string PlanDatabase = "ControlImage.PlanDatabase";

        /// <summary>
        /// 模版按钮图片资源键值
        /// </summary>
        public const string Template = "ControlImage.Template";
    }
}
