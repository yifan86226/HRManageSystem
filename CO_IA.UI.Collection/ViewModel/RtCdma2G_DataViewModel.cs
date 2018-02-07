using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.UI.Collection.ViewModel
{
    public class RtCdma2G_DataViewModel : ViewModelBase
    {

        private int _MCC = 0;
        /// <summary>
        /// 
        /// </summary>
        public int MCC
        {
            get
            {
                return _MCC;
            }
            set
            {
                Set(() => MCC, ref _MCC, value);
            }
        }



        private int _BASEIDC = 0;
        /// <summary>
        /// 
        /// </summary>
        public int BASEID
        {
            get
            {
                return _BASEIDC;
            }
            set
            {
                Set(() => BASEID, ref _BASEIDC, value);
            }
        }





        private int _NID = 0;
        /// <summary>
        /// 
        /// </summary>
        public int NID
        {
            get
            {
                return _NID;
            }
            set
            {
                Set(() => NID, ref _NID, value);
            }
        }




        private int _SID = 0;
        /// <summary>
        /// 
        /// </summary>
        public int SID
        {
            get
            {
                return _SID;
            }
            set
            {
                Set(() => SID, ref _SID, value);
            }
        }


        

        private string _UpdateTime = "";
        /// <summary>
        /// 更新时间
        /// </summary>
        public string UpdateTime
        {
            get
            {
                return _UpdateTime;
            }
            set
            {
                Set(() => UpdateTime, ref _UpdateTime, value);
            }
        }


    }
}
