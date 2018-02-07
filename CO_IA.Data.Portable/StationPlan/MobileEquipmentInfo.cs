using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class MobileEquipmentInfo : INotifyPropertyChanged
    {
        #region 成员变量
        private bool isChecked;
        private string _name;
        private string _modelNo;
        private string _code;
        private string _manufacturer;
        private string _matchingEqu;
        #endregion
        public MobileEquipmentInfo()
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

        public string ID
        {
            get;
            set;
        }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string Name 
        {
            get { return _name; }
            set 
            {
                _name = value;
                this.NotifyPropertyChanged("Name");
            }
        }

        /// <summary>
        /// 设备型号
        /// </summary>
        public string ModelNo
        {
            get { return _modelNo; }
            set 
            {
                _modelNo = value;
                this.NotifyPropertyChanged("ModelNo");
            }
        }

        /// <summary>
        /// 编号
        /// </summary>
        public string Code
        {
            get { return _code; }
            set 
            {
                _code = value;
                this.NotifyPropertyChanged("Code");
            }
        }

        /// <summary>
        /// 生产厂家
        /// </summary>
        public string Manufacturer 
        {
            get { return _manufacturer; }
            set
            {
                _manufacturer = value;
                this.NotifyPropertyChanged("Manufacturer");
            }
        }

        /// <summary>
        /// 配套设备
        /// </summary>
        public string MatchingEqu 
        {
            get { return _matchingEqu; }
            set
            {
                _matchingEqu = value;
                this.NotifyPropertyChanged("MatchingEqu");
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
