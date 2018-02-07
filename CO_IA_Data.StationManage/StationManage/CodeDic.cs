#region 文件描述
/***************************************************************#@#***************************************************************
 * 创建人：xiaguohui
 * 摘 要 ：台站业务类型字典
 * 日 期 ：2016-09-21
 ***************************************************************#@#***************************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA_Data.StationManage.StationManage
{
    public class CodeDic : INotifyPropertyChanged
    {
        private int co;
        /// <summary>
        /// 编号
        /// </summary>
        public int CO
        {
            get { return co; }
            set { co = value; }
        }
        private string cn;
        /// <summary>
        /// 英文描述
        /// </summary>
        public string CN
        {
            get { return cn; }
            set { cn = value; }
        }
        private string code_type_chi_name;
        /// <summary>
        /// 类型描述
        /// </summary>
        public string CODE_TYPE_CHI_NAME
        {
            get { return code_type_chi_name; }
            set { code_type_chi_name = value; }
        }
        private string code_chi_name;
        /// <summary>
        /// 中文描述
        /// </summary>
        public string CODE_CHI_NAME
        {
            get { return code_chi_name; }
            set { code_chi_name = value; }
        }
        private int code_discn;
        /// <summary>
        /// 
        /// </summary>
        public int CODE_DISCN
        {
            get { return code_discn; }
            set { code_discn = value; }
        }
        
        #region 实现INotifyPropertyChanged接口

        /// <summary>
        /// 属性变更通知事件
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 通知属性变更方法
        /// </summary>
        /// <param name="propertyName">发生变更的属性名称</param>
        private void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
        
    }
}
