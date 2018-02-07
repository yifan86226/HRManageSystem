using CO_IA.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class MonitorStationAntInfo : INotifyPropertyChanged
    {
        public MonitorStationAntInfo()
        {
            ID = System.Guid.NewGuid().ToString();
        }
        private bool isChecked;
        /// <summary>
        ///  选择
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
        public string ID { get; set; }
        /// <summary>
        /// 监测站ID
        /// </summary>
        public string MonitorStationID { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 供应商
        /// </summary>
        public string Supplier { get; set; }
        /// <summary>
        /// 起始频率
        /// </summary>
        public double? StartFreq { get; set; }
        /// <summary>
        /// 终止频率
        /// </summary>
        public double? EndFreq { get; set; }
        /// <summary>
        /// 极化方式
        /// </summary>
        public PolarizationEnum Polarization { get; set; }
        /// <summary>
        /// 型号
        /// </summary>
        public string ModelNo { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 设备串号
        /// </summary>
        public string SerialNumber { get; set; }
        /// <summary>
        /// 站类型
        /// </summary>
        public StatModeEnum StatMode { get; set; }
        private bool isDirectional;
        /// <summary>
        ///  定向天线
        /// </summary>
        public bool IsDirectional
        {
            get
            {
                return this.isDirectional;
            }
            set
            {
                this.isDirectional = value;
                this.NotifyPropertyChanged("IsDirectional");
            }
        }

        private bool isActive;
        /// <summary>
        ///  有源天线
        /// </summary>
        public bool IsActive 
        {
            get
            {
                return this.isActive;
            }
            set
            {
                this.isActive = value;
                this.NotifyPropertyChanged("IsActive ");
            }
        }
        /// <summary>
        /// 海拔
        /// </summary>
        public double? Altitude { get; set; }
        /// <summary>
        /// 接入损耗
        /// </summary>
        public double? AccessLoss { get; set; }
        /// <summary>
        /// 天线增益
        /// </summary>
        public double? AntennaGain { get; set; }
        /// <summary>
        /// 挂高
        /// </summary>
        public double? AntennaHeight { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Memo { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

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
    }
}
