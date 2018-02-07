#region 文件描述
/**********************************************************************************
 * 创建人：niext
 * 摘  要：车辆查条件
 * 日  期：2017-05-11
 * ********************************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class VehicleQueryCondition : INotifyPropertyChanged
    {
        public VehicleQueryCondition()
        {
            AreaCodes = new List<string>();
            VehicleTypes = new List<int>() { };
        }


        private string vehicleno;
        public string VehicleNo
        {
            get
            {
                return vehicleno;
            }
            set
            {
                vehicleno = value;
                NotifyPropertyChanged("VehicleNo");
            }
        }

        private string vehiclemodel;
        public string VehicleModel
        {
            get
            {
                return vehiclemodel;
            }
            set
            {
                vehiclemodel = value;
                NotifyPropertyChanged("VehicleModel");
            }
        }

        /// <summary>
        /// 监测,非监测车
        /// </summary>
        public List<int> VehicleTypes
        {
            get;
            set;
        }

        public List<string> AreaCodes { get; set; }

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
