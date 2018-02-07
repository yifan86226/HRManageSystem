using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class MonitorStationEquInfo : INotifyPropertyChanged
    {
        private bool isChecked;
        private double? startfreq;
        private double? endfreq;
        private double? sensitivity;

        public MonitorStationEquInfo()
        {
            ID = System.Guid.NewGuid().ToString();
        }
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
        /// 地区
        /// </summary>
        public string AreaCode { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string Supplier { get; set; }
        /// <summary>
        /// 起始频率
        /// </summary>
        public double? StartFreq
        {
            get
            {
                return this.startfreq;
            }
            set
            {
                this.startfreq = value;
                this.NotifyPropertyChanged("StartFreq");
            }
        }
        /// <summary>
        /// 终止频率
        /// </summary>
        public double? EndFreq
        {
            get
            {
                return this.endfreq;
            }
            set
            {
                this.endfreq = value;
                this.NotifyPropertyChanged("EndFreq");
            }
        }
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; }
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
        /// 灵敏度
        /// </summary>
        public double? Sensitivity
        {
            get
            {
                return this.sensitivity;
            }
            set
            {
                this.sensitivity = value;
                this.NotifyPropertyChanged("Sensitivity");
            }
        }
        /// <summary>
        /// 站类型(固定0/移动1)
        /// </summary>
        public StatModeEnum StatMode { get; set; }
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
