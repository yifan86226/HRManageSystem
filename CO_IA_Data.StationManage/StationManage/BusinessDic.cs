using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA_Data
{
    public class BusinessDic : INotifyPropertyChanged
    {
        private string cn;
        /// <summary>
        /// 字典代码
        /// </summary>
        public string CN 
        {
            get { return cn; }
            set { cn = value; }
        }
        private string code_chi_name;
        /// <summary>
        /// 字典码对应的汉字
        /// </summary>
        public string CODE_CHI_NAME 
        {
            get { return code_chi_name; }
            set { code_chi_name = value; }
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
