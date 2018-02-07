using CO_IA.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CO_IA.Data
{
    public class FixedStationInfo : INotifyPropertyChanged
    {
        public FixedStationInfo()
        {
            Guid = System.Guid.NewGuid().ToString();
        }

        #region 变量
        private bool _ischecked;
        private string _name;
        private string _areacode;
        private string _address;
        private double? _long;
        private double? _lat;
        private FixedStationTypeEnums _type;
        private DataStateEnum _datastate;
        #endregion

        /// <summary>
        /// 选择
        /// </summary>
        public bool IsChecked
        {
            get { return _ischecked; }
            set
            {
                _ischecked = value;
                NotifyPropertyChanged("IsChecked");
            }
        }

        public string Guid { get; set; }

        /// <summary>
        /// 名称
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

        public string AreaCode
        {
            get { return _areacode; }
            set
            {
                _areacode = value;
                NotifyPropertyChanged("AreaCode");
            }
        }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address
        {
            get { return _address; }
            set
            {
                _address = value;
                NotifyPropertyChanged("Address");
            }
        }

        /// <summary>
        /// 经度
        /// </summary>
        public double? LONG
        {
            get { return _long; }
            set
            {
                _long = value;
                NotifyPropertyChanged("LONG");
            }
        }

        /// <summary>
        /// 纬度
        /// </summary>
        public double? LAT
        {
            get { return _lat; }
            set
            {
                _lat = value;
                NotifyPropertyChanged("LAT");
            }
        }

        /// <summary>
        /// 类别
        /// </summary>
        public FixedStationTypeEnums Type
        {
            get { return _type; }
            set
            {
                _type = value;
                NotifyPropertyChanged("Type");
            }
        }

        public DataStateEnum DataState
        {

            get { return _datastate; }
            set
            {
                _datastate = value;
                NotifyPropertyChanged("DataState");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {


                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public bool CompareTo(FixedStationInfo newstation)
        {
            FixedStationInfo originstation = this;

            PropertyInfo[] orgprops = originstation.GetType().GetProperties();
            PropertyInfo[] newprops = newstation.GetType().GetProperties();

            foreach (PropertyInfo op in orgprops)
            {
                if (op.Name == "IsChecked" || op.Name == "DataState")
                    continue;
                PropertyInfo sp = newprops.FirstOrDefault(r => r.Name == op.Name && r.GetType() == op.GetType());

                object ovalue = op.GetValue(originstation, null);
                object svalue = sp.GetValue(newstation, null);

                if (ovalue != null && svalue != null && !ovalue.Equals(svalue))
                {
                    return true;
                }
                else if (ovalue == null)
                {
                    if (svalue == null)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
