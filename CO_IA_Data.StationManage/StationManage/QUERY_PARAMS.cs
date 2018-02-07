using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA_Data
{
    public class QUERY_PARAMS : INotifyPropertyChanged
    {
        /// <summary>
        /// 参数名
        /// </summary>
        private string m_strPARAMS_NAME;
        /// <summary>
        /// 参数名
        /// </summary>
        public string PARAMS_NAME
        {
            get
            {
                return m_strPARAMS_NAME;
            }
            set
            { 
                m_strPARAMS_NAME = value;    
            }
        }
        /// <summary>
        /// 运算关系
        /// </summary>
        private string m_strPARAMS_RELATION;
        /// <summary>
        /// 运算关系
        /// </summary>
        public string PARAMS_RELATION
        {
            get
            {
                return m_strPARAMS_RELATION;
            }
            set
            { 
                m_strPARAMS_RELATION = value;
            }
        }
        /// <summary>
        /// 参数值
        /// </summary>
        private string m_strPARAMS_VALUE { get; set; }
        /// <summary>
        /// 参数值
        /// </summary>
        public string PARAMS_VALUE
        {
            get
            {
                return m_strPARAMS_VALUE;
            }
            set
            {
                m_strPARAMS_VALUE = value;
            }
        }
        /// <summary>
        /// 参数值单位
        /// </summary>
        private string m_strPARAMS_UNIT;
        /// <summary>
        /// 参数值单位
        /// </summary>
        public string PARAMS_UNIT
        {
            get
            {
                return m_strPARAMS_UNIT;
            }
            set
            { 
                m_strPARAMS_UNIT = value;
            }
        }
        /// <summary>
        /// 参数类型
        /// </summary>
        private string m_strPARAMS_TYPE;
        /// <summary>
        /// 参数类型
        /// </summary>
        public string PARAMS_TYPE
        {
            get
            {
                return m_strPARAMS_TYPE;
            }
            set
            {
                m_strPARAMS_TYPE = value;  
            }
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
