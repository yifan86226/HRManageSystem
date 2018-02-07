#region 文件描述
/**********************************************************************************
 * 创建人：niext
 * 摘  要：车辆信息
 * 日  期：2015-11-27
 * ********************************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    /// <summary>
    /// 车辆信息
    /// </summary>
    public class VehicleInfo : INotifyPropertyChanged
    {
        public VehicleInfo()
        {
            if (string.IsNullOrEmpty(GUID))
            {
                GUID = System.Guid.NewGuid().ToString();
            }
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

        /// <summary>
        /// Guid
        /// </summary>
        public string GUID { get; set; }

        /// <summary>
        /// 车牌号码
        /// </summary>
        public string VehicleNo { get; set; }

        /// <summary>
        /// 车辆型号
        /// </summary>
        public string VehicleModel { get; set; }

        /// <summary>
        /// 默认司机
        /// </summary>
        public string Driver { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Comment { get; set; }


        private byte[] picture;
        /// <summary>
        /// 图片
        /// </summary>
        public byte[] Picture
        {
            get
            {
                return picture;
            }
            set
            {
                picture = value;
                this.NotifyPropertyChanged("Picture");
            }
        }

        /// <summary>
        /// 是否监测车
        /// </summary>
        public bool IsMonitor { get; set; }

        private string _department;
        /// <summary>
        /// 所属地区
        /// </summary>
        public string AreaCode
        {
            get
            {
                return _department;
            }
            set
            {
                _department = value;
                NotifyPropertyChanged("Department");
            }
        }


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
