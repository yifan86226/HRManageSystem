using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{

    public class StationBaseInfo : INotifyPropertyChanged
    {
        /// <summary>
        /// 通知单选中标识
        /// </summary>
        private bool isChecked;

        /// <summary>
        /// 获取或设置通知单选中标识
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return this.isChecked;
            }
            set
            {
                this.isChecked = value;
                this.NotifyPropertyChanged("IsChecked");
            }
        }

        /// <summary>
        /// 库所属省编码
        /// </summary>
        public string Province_Code { get; set; }

        /// <summary>
        /// 台站id
        /// </summary>
        public string StationId { get; set; }

        /// <summary>
        /// 技术资料编号
        /// </summary>
        public string Stat_Tdi { get; set; }

        /// <summary>
        /// 申请表编号
        /// </summary>
        public string APP_CODE { get; set; }

        /// <summary>
        /// 技术资料申报表类型
        /// </summary>
        public string STAT_APP_TYPE { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        public string Net_Svn { get; set; }

        /// <summary>
        /// 技术体制
        /// </summary>
        public string NET_TS { get; set; }

        /// <summary>
        /// 系统/行业
        /// </summary>
        public string OrgSysCode { get; set; }

        /// <summary>
        /// 台站名称
        /// </summary>
        public string STAT_NAME { get; set; }

        /// <summary>
        /// 台站地址
        /// </summary>
        public string STAT_ADDR { get; set; }

        /// <summary>
        /// 设台单位名称
        /// </summary>
        public string Org_Name { get; set; }

        /// <summary>
        /// 台站经度
        /// </summary>
        public string STAT_LG { get; set; }

        /// <summary>
        /// 台站纬度
        /// </summary>
        public string STAT_LA { get; set; }

        /// <summary>
        /// 海拔高度(米)
        /// </summary>
        public string STAT_AT { get; set; }

        /// <summary>
        /// 带宽
        /// </summary>
        public double BandWidth { get; set; }

        /// <summary>
        /// 台（站）类别RSBT_STATION(STAT_Type)
        /// </summary>
        public string STAT_Type { get; set; }

        /// <summary>
        ///行政管辖所属区域 
        /// </summary>
        public string ORG_CODE { get; set; }

        /// <summary>
        /// 台站所属区域
        /// </summary>
        public string STAT_AREA_CODE { get; set; }

        /// <summary>
        /// 台站状态
        /// </summary>
        public string STAT_STATUS { get; set; }

        /// <summary>
        /// 建站时间
        /// </summary>
        public string STAT_DATE_START { get; set; }

        /// <summary>
        ///申请频率使用期限（截止日期） RSBT_APPLY 申请表 
        /// </summary>
        public DateTime APP_FTLE { get; set; }

        /// <summary>
        ///边境距离 
        /// </summary>
        public string Distance { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FREQ_LC { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FREQ_UC { get; set; }

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

        public string Power { get; set; }

        public string FeedLose { get; set; }

        public string AntGain { get; set; }

        public string AntHight { get; set; }
    }
}
