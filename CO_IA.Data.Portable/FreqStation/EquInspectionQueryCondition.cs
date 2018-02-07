using AT_BC.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class EquInspectionQueryCondition : INotifyPropertyChanged
    {
        public EquInspectionQueryCondition()
        {
            CheckState = new List<CheckStateEnum>();
            SendLicense = new List<SendLicenseEnum>();
        }

        public string ActivityGuid { get; set; }

        public string PlaceGuid { get; set; }

        private string orgname;
        public string ORGName
        {
            get 
            { 
                return orgname; 
            }
            set
            {
                orgname = value;
                NotifyPropertyChanged("ORGName");
            }
        }

        private string equname;
        public string EuqName
        {
            get
            {
                return equname;
            }
            set
            {
                equname = value;
                NotifyPropertyChanged("EuqName");
            }
        }

        private double? sendfreqlittle;
        public double? SendFreqLittle
        {
            get
            {
                return sendfreqlittle;
            }
            set
            {
                sendfreqlittle = value;
                NotifyPropertyChanged("SendFreqLittle");
            }
        }

        private double? sendfreqgreat;
        public double? SendFreqGreat
        {
            get
            {
                return sendfreqgreat;
            }
            set
            {
                sendfreqgreat = value;
                NotifyPropertyChanged("SendFreqGreat");
            }
        }

        private double? bandlittle;
        public double? BandLittle
        {
            get
            {
                return bandlittle;
            }
            set
            {
                bandlittle = value;
                NotifyPropertyChanged("BandLittle");
            }
        }

        private double? bandgreat;
        public double? BandGreat
        {
            get
            {
                return bandgreat;
            }
            set
            {
                bandgreat = value;
                NotifyPropertyChanged("BandGreat");
            }
        }

        private List<CheckStateEnum> checkstate;
        /// <summary>
        /// 检测状态
        /// </summary>
        public List<CheckStateEnum> CheckState
        {
            get
            {
                return checkstate;
            }
            set
            {
                checkstate = value;
                NotifyPropertyChanged("CheckState");
            }
        }

        private List<SendLicenseEnum> sendlicense;
        /// <summary>
        /// 许可证发放状态
        /// </summary>
        public List<SendLicenseEnum> SendLicense
        {
            get
            {
                return sendlicense;
            }
            set
            {
                sendlicense = value;
                NotifyPropertyChanged("SendLicense");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }
    }
}
