using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data.Setting
{
    public class ExamPlace : INotifyPropertyChanged
    {
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

        public string Guid { get; set; }

        private string _name;
        /// <summary>
        /// 考点名称
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }
        private string _Address;
        /// <summary>
        /// 考点地址
        /// </summary>
        public string Address
        {
            get { return _Address; }
            set
            {
                _Address = value;
                NotifyPropertyChanged("Address");
            }
        }

        private string _Contact;
        /// <summary>
        /// 考点联系人
        /// </summary>
        public string Contact
        {
            get { return _Contact; }
            set
            {
                _Contact = value;
                NotifyPropertyChanged("Contact");
            }
        }

        private string _Phone;
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Phone
        {
            get { return _Phone; }
            set
            {
                _Phone = value;
                NotifyPropertyChanged("Phone");
            }
        }
        private string _Location_lg;
        /// <summary>
        /// 经度
        /// </summary>
        public string Location_lg
        {
            get { return _Location_lg; }
            set
            {
                _Location_lg = value;
                NotifyPropertyChanged("Location_lg");
            }
        }
        private string _Location_la;
        /// <summary>
        /// 纬度
        /// </summary>
        public string Location_la
        {
            get { return _Location_la; }
            set
            {
                _Location_la = value;
                NotifyPropertyChanged("Location_la");
            }
        }
        private string _Remark;
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark
        {
            get { return _Remark; }
            set
            {
                _Remark = value;
                NotifyPropertyChanged("Remark");
            }
        }

        private string _Areacode;
        /// <summary>
        /// 所属地区
        /// </summary>
        public string Areacode
        {
            get { return _Areacode; }
            set
            {
                _Areacode = value;
                NotifyPropertyChanged("Areacode");
            }
        }

        private List<ActivityPlaceLocationImage> _Images = new List<ActivityPlaceLocationImage>();
        public List<ActivityPlaceLocationImage> Images
        {
            get { return _Images; }
            set
            {
                _Images = value;
            }
        }
        /// <summary>
        /// 图片
        /// </summary>
        public byte[] Picture { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string pPropertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(pPropertyName));
            }
        }
    }
}
