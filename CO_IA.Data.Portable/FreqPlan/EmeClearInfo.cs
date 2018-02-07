using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class EmeClearInfo : INotifyPropertyChanged
    {


        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                this.NotifyPropertyChanged("IsSelected");
            }
        }


        private string guid;

        /// <summary>
        ///   主键
        /// </summary>
        public string GUID
        {
            get
            {
                return guid;
            }

            set
            {
                guid = value;
            }
        }


        private double? fREQ_RC;
        /// <summary>
        /// 接收频率
        /// </summary>
        public double? FREQ_RC
        {
            get
            {
                return fREQ_RC;
            }

            set
            {
                fREQ_RC = value;
            }
        }

        private double? fREQ_EC;

        /// <summary>
        /// 发射频率
        /// </summary>
        public double? FREQ_EC
        {
            get
            {
                return fREQ_EC;
            }

            set
            {
                fREQ_EC = value;
            }
        }

        private double? fREQ_BAND;
        /// <summary>
        /// 发射频率
        /// </summary>
        public double? FREQ_BAND
        {
            get
            {
                return fREQ_BAND;
            }

            set
            {
                fREQ_BAND = value;
            }
        }


        private string signalSource;

        /// <summary>
        ///   信号来源
        /// </summary>
        public string SignalSource
        {
            get
            {
                return signalSource;
            }

            set
            {
                signalSource = value;
            }
        }

        private string department;

        /// <summary>
        /// 使用单位
        /// </summary>
        public string Department
        {
            get
            {
                return department;
            }

            set
            {
                department = value;
            }
        }


        private string statName;

        /// <summary>
        /// 台站名称
        /// </summary>
        public string StationName
        {
            get
            {
                return statName;
            }

            set
            {
                statName = value;
            }
        }


        private string address;

        /// <summary>
        /// 台站地址
        /// </summary>
        public string Address
        {
            get
            {
                return address;
            }

            set
            {
                address = value;
            }
        }
        private string relationMan;

        /// <summary>
        /// 联系人
        /// </summary>
        public string RelationMan
        {
            get
            {
                return relationMan;
            }

            set
            {
                relationMan = value;
            }
        }

        private string phone;

        /// <summary>
        /// 联系方式
        /// </summary>
        public string Phone
        {
            get
            {
                return phone;
            }

            set
            {
                phone = value;
            }
        }

        private string _resultIsClear;
        /// <summary>
        /// 清理结果标识
        /// </summary>
        public string ResultIsClear
        {
            get
            {
                return _resultIsClear;
            }
            set
            {
                _resultIsClear = value;

                this.NotifyPropertyChanged("ResultIsClear");
            }
        }



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
