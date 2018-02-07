using AT_BC.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.UI.Screen
{
    public class StateData:NotifyPropertyChangedObject
    {
        #region one;
        int planFreqPointNum;
        /// <summary>
        /// 指配
        /// </summary>
        public int PlanFreqPointNum
        {
            get { return planFreqPointNum; }
            set { planFreqPointNum = value; NotifyPropertyChanged("PlanFreqPointNum"); }
        }

        int applyFreqPointNum;
        /// <summary>
        /// 备用
        /// </summary>
        public int ApplyFreqPointNum
        {
            get { return applyFreqPointNum; }
            set { applyFreqPointNum = value; NotifyPropertyChanged("ApplyFreqPointNum"); }
        }

        int clearFreqCount;
        /// <summary>
        /// 清理频点数
        /// </summary>
        public int ClearFreqCount
        {
            get { return clearFreqCount; }
            set { clearFreqCount = value; NotifyPropertyChanged("ClearFreqCount"); }
        }
        #endregion

        #region two

        int knowSignalCount;
        /// <summary>
        /// 已知信号
        /// </summary>
        public int KnowSignalCount
        {
            get { return knowSignalCount; }
            set { knowSignalCount = value; NotifyPropertyChanged("KnowSignalCount"); }
        }

        int unKnowSignalcount;
        /// <summary>
        /// 未知信号
        /// </summary>
        public int UnKnowSignalcount
        {
            get { return unKnowSignalcount; }
            set { unKnowSignalcount = value; NotifyPropertyChanged("UnKnowSignalcount"); }
        }

        int distCount;
        /// <summary>
        /// 干扰数量
        /// </summary>
        public int DistCount
        {
            get { return distCount; }
            set { distCount = value; NotifyPropertyChanged("DistCount"); }
        }
        #endregion


        int personNum;
        /// <summary>
        /// 人员
        /// </summary>
        public int PersonNum
        {
            get { return personNum; }
            set { personNum = value; NotifyPropertyChanged("PersonNum"); }
        }

        int vehicleNum;
        /// <summary>
        /// 车辆
        /// </summary>
        public int VehicleNum
        {
            get { return vehicleNum; }
            set { vehicleNum = value; NotifyPropertyChanged("VehicleNum"); }
        }

        int quipmentNum;
        /// <summary>
        /// 设备
        /// </summary>
        public int EquipmentNum
        {
            get { return quipmentNum; }
            set { quipmentNum = value; NotifyPropertyChanged("EquipmentNum"); }
        }

        //private List<KeyValuePair<string, double>> _listone;
        //public List<KeyValuePair<string, double>> DsListOne
        //{
        //    get {
        //        if (_listone == null || _listone.Count == 0)
        //        {
        //            _listone = new List<KeyValuePair<string, double>>() {    
        //                new KeyValuePair<string,double>("指配频率",10),
        //                new KeyValuePair<string,double>("备用频率",0),
        //                new KeyValuePair<string,double>("清理频率",0),
        //            };
        //        }
        //        return _listone; 
        //    }
        //    set
        //    {
        //        _listone = value;                
        //        NotifyPropertyChanged("DsListOne");
        //    }
        //}

        //private List<KeyValuePair<string, double>> _listtwo;
        //public List<KeyValuePair<string, double>> DsListTwo
        //{
        //    get { return _listtwo; }
        //    set
        //    {
        //        _listtwo = value;
        //        if (_listtwo == null || _listtwo.Count == 0)
        //        {
        //            _listtwo = null;
        //            _listtwo = new List<KeyValuePair<string, double>>() {    
        //                new KeyValuePair<string,double>("已知信号",0),
        //                new KeyValuePair<string,double>("未知信号",0),
        //                new KeyValuePair<string,double>("干扰数量",0),
        //            };
        //        }
        //        NotifyPropertyChanged("DsListTwo");
        //    }
        //}

        //private List<KeyValuePair<string, double>> _listthree;
        //public List<KeyValuePair<string, double>> DsListThree
        //{
        //    get { return _listthree; }
        //    set
        //    {
        //        _listthree = value;
        //        if (_listthree == null || _listthree.Count == 0)
        //        {
        //            _listthree = null;
        //            _listthree = new List<KeyValuePair<string, double>>() {    
        //                new KeyValuePair<string,double>("保障人员",0),
        //                new KeyValuePair<string,double>("监测车数量",0),
        //                new KeyValuePair<string,double>("设备数量",0),
        //            };
        //        }
        //        NotifyPropertyChanged("DsListThree");
        //    }
        //}
    }
}
