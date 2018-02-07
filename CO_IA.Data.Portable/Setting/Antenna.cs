using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class Antenna : INotifyPropertyChanged
    {
        public Antenna()
        {
            if (string.IsNullOrEmpty(Guid))
            {
                Guid = System.Guid.NewGuid().ToString();
            }
        }

        public string Guid { get; set; }

        private string _antType;
        /// <summary>
        /// 天线类型
        /// </summary>
        public string AntType
        {
            get { return _antType; }
            set
            {
                _antType = value;
                NotifyPropertyChanged("AntType");
            }
        }

        private string _antmodel;
        /// <summary>
        /// 天线型号
        /// </summary>
        public string AntModel
        {
            get { return _antmodel; }
            set
            {
                _antmodel = value;
                NotifyPropertyChanged("AntModel");
            }
        }

        private double? _antelevation;
        /// <summary>
        /// 天线仰角
        /// </summary>
        public double? AntElevation
        {
            get { return _antelevation; }
            set
            {
                _antelevation = value;
                NotifyPropertyChanged("AntUpAng");
            }
        }

        /// <summary>
        /// 天线方位角
        /// </summary>
        private double? _antAzimuth;
        public double? AntAzimuth
        {
            get { return _antAzimuth; }
            set
            {
                _antAzimuth = value;
                NotifyPropertyChanged("AntAzimuth");
            }
        }

        private EMCS.Types.EMCPolarisationEnum _antPolar;
        /// <summary>
        /// 极化方式
        /// </summary>
        public EMCS.Types.EMCPolarisationEnum AntPolar
        {
            get { return _antPolar; }
            set
            {
                _antPolar = value;
                NotifyPropertyChanged("AntPolar");
            }
        }

        public double? _antHight;
        /// <summary>
        ///天线高度 
        /// </summary>
        public double? AntHight
        {
            get { return _antHight; }
            set
            {
                _antHight = value;
                NotifyPropertyChanged("AntHight");
            }
        }

        public double? _antGain;
        /// <summary>
        /// 天线增益
        /// </summary>
        public double? AntGain
        {
            get { return _antGain; }
            set
            {
                _antGain = value;
                NotifyPropertyChanged("AntGain");
            }
        }

        public double? _feedlength;
        /// <summary>
        /// 馈线长度
        /// </summary>
        public double? FeedLength
        {
            get { return _feedlength; }
            set
            {
                _feedlength = value;
                NotifyPropertyChanged("FeedLength");
            }
        }

        public double? _feedLose;
        /// <summary>
        /// 馈线损耗
        /// </summary>
        public double? FeedLose
        {
            get { return _feedLose; }
            set
            {
                _feedLose = value;
                NotifyPropertyChanged("FeedLose");
            }
        }


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
