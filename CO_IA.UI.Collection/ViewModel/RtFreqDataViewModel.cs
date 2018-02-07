using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.UI.Collection.ViewModel
{
    public class RtFreqDataViewModel : ViewModelBase
    {



        private double _frequencyStep = 0.0;
        /// <summary>
        /// 步进
        /// </summary>
        public double frequencyStep
        {
            get
            {
                return _frequencyStep;
            }
            set
            {
                Set(() => frequencyStep, ref _frequencyStep, value);
            }
        }



        private double _startFrequency = 0.0;
        /// <summary>
        /// 开始频率 （MHz）
        /// </summary>
        public double startFrequency
        {
            get
            {
                return _startFrequency;
            }
            set
            {
                Set(() => startFrequency, ref _startFrequency, value);
            }
        }



        private string _FreqMeasureNO =string.Empty;
        /// <summary>
        /// 测量编号
        /// </summary>
        public string FreqMeasureNO
        {
            get
            {
                return _FreqMeasureNO;
            }
            set
            {
                Set(() => FreqMeasureNO, ref _FreqMeasureNO, value);
            }
        }





        private uint _FreqDataCount =0;
        /// <summary>
        /// 频点数量
        /// </summary>
        public uint FreqDataCount
        {
            get
            {
                return _FreqDataCount;
            }
            set
            {
                Set(() => FreqDataCount, ref _FreqDataCount, value);
            }
        }




        private string _FreqDataUpdateTime = "";
        /// <summary>
        /// 频点数量
        /// </summary>
        public string FreqDataUpdateTime
        {
            get
            {
                return _FreqDataUpdateTime;
            }
            set
            {
                Set(() => FreqDataUpdateTime, ref _FreqDataUpdateTime, value);
            }
        }


    }
}
