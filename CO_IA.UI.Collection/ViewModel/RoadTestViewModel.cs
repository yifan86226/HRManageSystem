using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO_IA.Data.Collection;
using System.Collections.ObjectModel;

namespace CO_IA.UI.Collection.ViewModel
{
    public class RoadTestViewModel : ViewModelBase
    {

        private string _RoadTestName = string.Empty;
        public string RoadTestName
        {
            get
            {
                return _RoadTestName;
            }
            set
            {
                Set(() => RoadTestName, ref _RoadTestName, value);
            }
        }




        private string _CarPlate = "ABCD";
        public string CarPlate
        {
            get
            {
                return _CarPlate;
            }
            set
            {
                Set(() => CarPlate, ref _CarPlate, value);
            }
        }

        //private string _ReciverIp = "192.168.6.240";//sensor
        private string _ReciverIp = "192.168.6.23";//rmtp
        //private string _ReciverIp = "192.168.3.114";
        //private string _ReciverIp = "192.168.2.153";
        public string ReciverIp
        {
            get
            {
                return _ReciverIp;
            }
            set
            {
                Set(() => ReciverIp, ref _ReciverIp, value);
            }
        }

        private int _Port = 5150;
        public int Port
        {
            get
            {
                return _Port;
            }
            set
            {
                Set(() => Port, ref _Port, value);
            }
        }

        private string _Username = "rxtest";
        public string Username
        {
            get
            {
                return _Username;
            }
            set
            {
                Set(() => Username, ref _Username, value);
            }
        }

        private string _Password = "rxtest";
        public string Password
        {
            get
            {
                return _Password;
            }
            set
            {
                Set(() => Password, ref _Password, value);
            }
        }

        private string _StationId = "00000001";
        public string StationId
        {
            get
            {
                return _StationId;
            }
            set
            {
                Set(() => StationId, ref _StationId, value);
            }
        }

        private string _CenterId = "00000000";
        public string CenterId
        {
            get
            {
                return _CenterId;
            }
            set
            {
                Set(() => CenterId, ref _CenterId, value);
            }
        }

        private int _TestSample = 1000;
        public int TestSample
        {
            get
            {
                return _TestSample;
            }
            set
            {
                Set(() => TestSample, ref _TestSample, value);
            }
        }


        private string _FreqDataTable = "FreqDataDebug";
        public string FreqDataTable
        {
            get
            {
                return _FreqDataTable;
            }
            set
            {
                Set(() => FreqDataTable, ref _FreqDataTable, value);
            }
        }


        private double _StartFreq = 30;
        public double StartFreq
        {
            get
            {
                return _StartFreq;
            }
            set
            {
                Set(() => StartFreq, ref _StartFreq, value);
            }
        }

        private double _EndFreq = 3000;
        public double EndFreq
        {
            get
            {
                return _EndFreq;
            }
            set
            {
                Set(() => EndFreq, ref _EndFreq, value);
            }
        }



        private double _Bandwidth = 25;
        public double Bandwidth
        {
            get
            {
                return _Bandwidth;
            }
            set
            {
                Set(() => Bandwidth, ref _Bandwidth, value);
            }
        }



        private string _TestStaffName = string.Empty;
        public string TestStaffName
        {
            get
            {
                return _TestStaffName;
            }
            set
            {
                Set(() => TestStaffName, ref _TestStaffName, value);
            }
        }

        /// <summary>
        /// 保存采集数据到oracle库
        /// </summary>
        public bool SaveData
        {
            get
            {
                return _saveData;
            }
            set
            {
                Set(() => SaveData, ref _saveData, value);
            }
        }
        private bool _saveData = true;

        private double _SignalLimit = 30;
        public double SignalLimit
        {
            get
            {
                return _SignalLimit;
            }
            set
            {
                Set(() => SignalLimit, ref _SignalLimit, value);
            }
        }

        private ObservableCollection<AnalysisResult> _AnalysisList;
        /// <summary>
        /// 采集集合
        /// </summary>
        public ObservableCollection<AnalysisResult> AnalysisList
        {
            get
            {
                return _AnalysisList;
            }
            set
            {
                Set(() => AnalysisList, ref _AnalysisList, value);
            }
        }
        
        /// <summary>
        /// 冲突信号集合
        /// </summary>
        public ObservableCollection<AnalysisResult> IllegalAnalysisList
        {
            get
            {
                return _IllegalAnalysisList;
            }
            set
            {
                Set(() => IllegalAnalysisList, ref _IllegalAnalysisList, value);
            }
        }
        private ObservableCollection<AnalysisResult> _IllegalAnalysisList;
    }
}
