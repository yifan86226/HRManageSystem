#region 文件描述
/**********************************************************************************
 * 创建人：wx
 * 摘  要：活动地点位置信息
 * 日  期：2016-08-12
 * ********************************************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class ActivityPlaceLocation : INotifyPropertyChanged
    {
        public string GUID { get; set; }

        private string _ActivityPlaceGuid;
        /// <summary>
        /// 活动地点GUID
        /// </summary>
        public string ActivityPlaceGuid
        {
            get
            {
                return _ActivityPlaceGuid;
            }
            set
            {
                _ActivityPlaceGuid = value;
                NotifyPropertyChanged("ActivityPlaceGuid");
            }
        }
        private string _LocationName;
        /// <summary>
        /// 位置名称
        /// </summary>
        public string LocationName
        {
            get
            {
                return _LocationName;
            }
            set
            {
                _LocationName = value;
                NotifyPropertyChanged("LocationName");
            }
        }
        private double _LocationLG = 0.0;
        /// <summary>
        /// 经度
        /// </summary>
        public double LocationLG
        {
            get
            {
                return _LocationLG;
            }
            set
            {
                _LocationLG = value;
                NotifyPropertyChanged("LocationLG");
            }
        }
        private double _LocationLA = 0.0;
        /// <summary>
        /// 纬度
        /// </summary>
        public double LocationLA
        {
            get
            {
                return _LocationLA;
            }
            set
            {
                _LocationLA = value;
                NotifyPropertyChanged("LocationLA");
            }
        }
        private string _Remark;
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark
        {
            get
            {
                return _Remark;
            }
            set
            {
                _Remark = value;
                NotifyPropertyChanged("Remark");
            }
        }
        public List<ActivityPlaceLocationImage> activityPlaceLocationImage
        { get; set; }
        /// <summary>
        /// 考点guid
        /// </summary>
        public string RiasExamplaceGuid {get; set; }

        //public ActivityPlaceLocationImage[] activityPlaceLocationImages
        //{ get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(PropertyName));
            }
        }
    }
}
