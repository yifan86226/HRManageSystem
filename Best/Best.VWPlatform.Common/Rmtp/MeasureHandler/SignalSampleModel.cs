using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Rmtp.MeasureHandler
{
    public class TSignalSampleModel
    {
        public TSignalSampleModel()
        {
            DicProperties = new Dictionary<string, string>();    
        }

        public Dictionary<string, string> DicProperties { set; get; }

        /// <summary>
        /// 性质
        /// </summary>
        public string SignalProperty
        {
            get
            {
                if (!DicProperties.ContainsKey("PROPERTY"))
                {
                     return null;
                }
                return DicProperties["PROPERTY"];
            }
        }

        /// <summary>
        /// 如果确认名不为空返回确认台站名，否则返回查到的
        /// </summary>
        public string StatName
        {
            get
            {
                if (!DicProperties.ContainsKey("STAT_NAME"))
                {
                    return null;
                }

                return DicProperties["STAT_NAME"];
            }
        }


        public string StatGUID
        {
            get
            {
                if (!DicProperties.ContainsKey("GUID"))
                {
                    return null;
                }

                return DicProperties["GUID"];
            }
        }

        /// <summary>
        /// 信道ID
        /// </summary>
        public string ChannelID
        {
            get
            {
                if (!DicProperties.ContainsKey("CHANNELID"))
                {
                    return null;
                }

                return DicProperties["CHANNELID"];
            }
        }

        /// <summary>
        /// 信道频率
        /// </summary>
        public string ChannelFreq
        {
            get
            {
                if (!DicProperties.ContainsKey("FREQPOINT"))
                {
                    return null;
                }

                return DicProperties["FREQPOINT"];
            }
        }

        /// <summary>
        /// 信道带宽
        /// </summary>
        public string ChannelWidth
        {
            get
            {
                if (!DicProperties.ContainsKey("FREQWIDTH"))
                {
                    return null;
                }

                return DicProperties["FREQWIDTH"];
            }
        }

        public bool IsFromSample
        {
            get
            {
                if (!DicProperties.ContainsKey("DATASOURCE"))
                {
                    return false;
                }

                return DicProperties["DATASOURCE"].Equals("1");
            }
        }
    }

    public class SignalSampleModel
    {
        public SignalSampleModel()
        {
            ListSuggestList = new List<TSignalSampleModel>();
        }

        private int _signalProperty = 0;
        private string _staGuid = "";
        private string _staName = "";
        private string _channelID = "";
        private string _channelFre = "";
        private string _channelWidth = "";

        public int NatureProperty
        {
            get { return TransSignalType(_signalProperty); }
        }

        /// <summary>
        /// 信号性质转换，暂时先这样处理
        /// </summary>
        /// <param name="pType"></param>
        /// <returns></returns>
        public int TransSignalType(int pType)
        {
            int re = 0;
            switch (pType)
            {
                case 0:
                    re = 4;
                    break;
                case 1:
                    re = 1;
                    break;
                case 2:
                    re = 0;
                    break;
                case 3:
                    re = 5;
                    break;
            }

            return re;
        }

        /// <summary>
        /// 性质
        /// </summary>
        public int SignalProperty
        {
            set { _signalProperty = value; }
            get { return _signalProperty; }
        }

        /// <summary>
        /// 台站名
        /// </summary>
        public string StatName 
        { 
            set{ _staName = value; }
            get{ return _staName; }
        }

        /// <summary>
        /// 台站的guid，服务需要
        /// </summary>
        public string StatGUID
        {
            set { _staGuid = value; }
            get { return _staGuid; }
        }

        /// <summary>
        /// 信道ID
        /// </summary>
        public string ChannelID
        {
            set { _channelID = value; }
            get { return _channelID; }
        }

        /// <summary>
        /// 信道频率
        /// </summary>
        public string ChannelFreq
        {
            set { _channelFre = value; }
            get { return _channelFre; }
        }

        /// <summary>
        /// 信道带宽
        /// </summary>
        public string ChannelWidth
        {
            set { _channelWidth = value; }
            get { return _channelWidth; }
        }

        public List<TSignalSampleModel> ListSuggestList { set; get; }

        public void AddSignal(TSignalSampleModel pModel)
        {
            ListSuggestList.Add(pModel);
        }
    }
}
