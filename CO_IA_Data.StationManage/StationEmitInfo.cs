using CO_IA.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA_Data
{
    /// <summary>
    /// 台站发射信息
    /// </summary>
    public class StationEmitInfo : INotifyPropertyChanged
    {
        /// <summary>
        /// 频率guid
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// 台站guid
        /// </summary>
        public string StationGuid { get; set; }

        /// <summary>
        /// 地点
        /// </summary>
        public string PlaceGuid { get; set; }

        /// <summary>
        /// 天线高度
        /// </summary>
        public double AntHight { get; set; }

        /// <summary>
        /// 设备功率
        /// </summary>
        public double EquPow { get; set; }

        /// <summary>
        /// 频率类型(0：频点 1：频段)
        /// </summary>
        public FreqType FreqType { get; set; }

        /// <summary>
        /// 频段:发射频率
        /// </summary>
        public double? FreqEC { get; set; }

        /// <summary>
        /// 频段:接收频率
        /// </summary>
        public double? FreqRC { get; set; }

        /// <summary>
        /// 频段:发射频率开始
        /// </summary>
        public double? FreqEFB { get; set; }

        /// <summary>
        /// 频段:发射频率结束
        /// </summary>
        public double? FreqEFE { get; set; }

        /// <summary>
        /// 频段:接收频率开始
        /// </summary>
        public double? FreqRFB { get; set; }

        /// <summary>
        /// 频段:接收频率结束
        /// </summary>
        public double? FreqRFE { get; set; }

        /// <summary>
        /// 带宽
        /// </summary>
        public double FreqBand { get; set; }

        /// <summary>
        /// 天线增益
        /// </summary>
        public double AntEgain { get; set; }

        /// <summary>
        ///馈线损耗 
        /// </summary>
        public double FeedLose { get; set; }

        /// <summary>
        /// 调制方式
        /// </summary>
        public string FreqMod { get; set; }

        /// <summary>
        /// 极化方式
        /// </summary>
        public string AntPole { get; set; }

        /// <summary>
        /// 海拔高度
        /// </summary>
        public double StatAT { get; set; }

        /// <summary>
        /// 接收天线高度
        /// </summary>
        public double RCVAntHight { get; set; }

        /// <summary>
        /// 接收天线增益
        /// </summary>
        public double RCVAntEgain { get; set; }

        /// <summary>
        /// 接收天线馈线损耗
        /// </summary>
        public double RCVFeedLose { get; set; }

        private NeedClearEunm needclear;
        /// <summary>
        /// 是否需要清理
        /// </summary>
        public NeedClearEunm NeedClear
        {
            get
            {
                return needclear;
            }
            set
            {
                needclear = value;
                NotifyPropertyChanged("NeedClear");
            }
        }

        private ClearResultEnum clearResult;
        /// <summary>
        /// 清理结果
        /// </summary>
        public ClearResultEnum ClearResult 
        {
            get
            {
                return this.clearResult;
            }
            set
            {
                this.clearResult = value;
                this.NotifyPropertyChanged("ClearResult");
            }
        }

        /// <summary>
        /// TF及C表序列号
        /// </summary>
        public string Ccode { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 通知属性变更方法
        /// </summary>
        /// <param name="propertyName">发生变更的属性名称</param>
        public void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
