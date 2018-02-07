using AT_BC.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class ActivityPlace : IdentifiableData<string>
    {
    }

    public class PlaceInfo : ActivityPlace
    {
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string Contact { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string Phone { get; set; }

        private byte[] image;
        /// <summary>
        /// 地点图片
        /// </summary>
        public byte[] Image
        {
            get
            {
                return this.image;
            }
            set
            {
                this.image = value;
                this.NotifyPropertyChanged("Image");
            }
        }

        /// <summary>
        /// 地点范围
        /// </summary>
        public string Graphics { get; set; }

        /// <summary>
        /// 位置列表
        /// </summary>
        public ActivityPlaceLocation[] Locations { get; set; }
        private bool _isChecked;
        /// <summary>
        /// check
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                _isChecked = value;
                this.NotifyPropertyChanged("IsChecked");
            }
        }
    }

    public sealed class ActivityPlaceInfo : ActivityPlace
    {
        /// <summary>
        /// 活动GUID
        /// </summary>
        public string ActivityGuid { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string Contact { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string Phone { get; set; }

        private byte[] image;
        /// <summary>
        /// 地点图片
        /// </summary>
        public byte[] Image 
        {
            get
            {
                return this.image;
            }
            set
            {
                this.image = value;
                this.NotifyPropertyChanged("Image");
            }
        }

        /// <summary>
        /// 地点范围
        /// </summary>
        public string Graphics { get; set; }

        /// <summary>
        /// 位置列表
        /// </summary>
        public ActivityPlaceLocation[] Locations { get; set; }
        private bool _isChecked;
        /// <summary>
        /// check
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                _isChecked = value;
                this.NotifyPropertyChanged("IsChecked");
            }
        }

        public string GuidInPlanDatabasePlace
        {
            get;
            set;
        }
    }

    public class ActivityPlaceQueryResult
    {
        /// <summary>
        /// 活动信息
        /// </summary>
        public Activity ActivityInfo { get; set; }

        /// <summary>
        /// 地点信息
        /// </summary>
        public ActivityPlaceInfo ActivityPlaceInfo { get; set; }
    }
}
