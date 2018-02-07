using Best.VWPlatform.Common.Rmtp;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Best.VWPlatform.Common.Types
{
    /// <summary>
    /// 地图上所有站点信息基类
    /// </summary>
    public class StationInfo : StationItem
    {
        private string _guid;
        private string _name;
        /// <summary>
        /// 全局唯一标识符
        /// </summary>
        public string Guid
        {
            get
            {
                if (DicProperties.ContainsKey("RSBT_STATION_CACHE.GUID"))
                {
                    _guid = DicProperties["RSBT_STATION_CACHE.GUID"];
                }

                if (string.IsNullOrEmpty(_guid))
                    return this.Name;
                return _guid;
            }
            set
            {
                _guid = value;
                OnPropertyChanged("Guid");
            }
        }

        /// <summary>
        /// 站点名称
        /// </summary>
        public string Name
        {
            get
            {
                if (DicProperties.ContainsKey("RSBT_STATION_CACHE.STAT_NAME"))
                {
                    _name = DicProperties["RSBT_STATION_CACHE.STAT_NAME"];
                }
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        /// <summary>
        /// 台站是否聚合，大于1表示聚合显示的个数，等于1表示台站
        /// </summary>
        public double StationsCount
        {
            get
            {
                if (!DicProperties.ContainsKey("RSBT_STATION_CACHE.COUNT"))
                {
                    return 0;
                }

                return double.Parse(DicProperties["RSBT_STATION_CACHE.COUNT"]);
            }
        }


        /// <summary>
        /// 台站是否活跃，等于0表示台站不活跃，否则活跃
        /// </summary>
        public double StationsActiveDegree
        {
            get
            {
                if (!DicProperties.ContainsKey("RSBT_STATION_CACHE.ACTIVE_DEGEE"))
                {
                    return 0;
                }

                return double.Parse(DicProperties["RSBT_STATION_CACHE.ACTIVE_DEGEE"]);
            }
        }

        /// <summary>
        /// 台站通信系统类型
        /// </summary>
        public string CommSystemType
        {
            get
            {
                if (!DicProperties.ContainsKey("RSBT_STATION_CACHE.NET_TS"))
                {
                    return null;
                }

                if (string.IsNullOrEmpty(DicProperties["RSBT_STATION_CACHE.NET_TS"]))
                {
                    return DicProperties["RSBT_STATION_CACHE.NET_SVN"];
                }
                return DicProperties["RSBT_STATION_CACHE.NET_TS"];
            }
        }

        /// <summary>
        /// 台站地址
        /// </summary>
        public string Address
        {
            get
            {
                if (!DicProperties.ContainsKey("RSBT_STATION_CACHE.STAT_ADDR"))
                {
                    return null;
                }

                return DicProperties["RSBT_STATION_CACHE.STAT_ADDR"];
            }
        }

        /// <summary>
        /// 设台单位
        /// </summary>
        public string OrgName
        {
            get
            {
                if (!DicProperties.ContainsKey("RSBT_STATION_CACHE.ORG_NAME"))
                {
                    return null;
                }

                return DicProperties["RSBT_STATION_CACHE.ORG_NAME"];
            }
        }

        /// <summary>
        /// 业务系统
        /// </summary>
        public string BusinessSystem
        {
            get
            {
                if (!DicProperties.ContainsKey("RSBT_STATION_CACHE.NET_SVN"))
                {
                    return null;
                }

                return DicProperties["RSBT_STATION_CACHE.NET_SVN"];
            }
        }

        /// <summary>
        /// 技术体制
        /// </summary>
        public string TechSystem
        {
            get
            {
                if (!DicProperties.ContainsKey("RSBT_STATION_CACHE.NET_TS"))
                {
                    return null;
                }

                return DicProperties["RSBT_STATION_CACHE.NET_TS"];
            }
        }

        /// <summary>
        /// 申请编号
        /// </summary>
        public string ApplyCode
        {
            get
            {
                if (!DicProperties.ContainsKey("RSBT_STATION_CACHE.APP_CODE"))
                {
                    return null;
                }

                return DicProperties["RSBT_STATION_CACHE.APP_CODE"];
            }
        }

        /// <summary>
        /// 资料编号
        /// </summary>
        public string DataTypeCode
        {
            get
            {
                if (!DicProperties.ContainsKey("RSBT_STATION_CACHE.STAT_APP_TYPE") || !DicProperties.ContainsKey("RSBT_STATION_CACHE.STAT_TDI"))
                {
                    return null;
                }

                return DicProperties["RSBT_STATION_CACHE.STAT_APP_TYPE"] + DicProperties["RSBT_STATION_CACHE.STAT_TDI"];
            }
        }
        
        
        /// <summary>
        /// 发射频率
        /// </summary>
        public double StationFreq
        {
            get
            {
                if (!DicProperties.ContainsKey("RSBT_STATION_CACHE.FREQPOINT"))
                {
                    return 0;
                }

                return double.Parse(DicProperties["RSBT_STATION_CACHE.FREQPOINT"]);
            }
        }

        /// <summary>
        /// 发射功率
        /// </summary>
        public double StationPower
        {
            get
            {
                if (!DicProperties.ContainsKey("RSBT_STATION_CACHE.EQU_POW"))
                {
                    return 0;
                }

                return double.Parse(DicProperties["RSBT_STATION_CACHE.EQU_POW"]);
            }
        }



        ///////////////////////////////////////////////////////////////////////////////////////////////////////

        private readonly Dictionary<string, string> _propertyValueCache = new Dictionary<string, string>();

        /// <summary>
        /// 附加信息，如：TDO定位出的信号源，这里可存入相关的监测站列表
        /// </summary>
        public object Tag
        {
            get;
            set;
        }
        /// <summary>
        /// 获取StationInfo上附加的属性信息
        /// </summary>
        /// <param name="pPropertyName">属性名称</param>
        /// <returns>值</returns>
        public string this[string pPropertyName]
        {
            get
            {
                return GetPropertyValue(pPropertyName);
            }
            set
            {
                SetPropertyValue(pPropertyName, value);
            }
        }
        /// <summary>
        /// 获取StationInfo上附加的属性信息
        /// </summary>
        /// <param name="pPropertyName">属性名称</param>
        /// <returns>值</returns>
        internal string GetPropertyValue(string pPropertyName)
        {
            string propertyValue = string.Empty;

            var dataFrame = Tag as RmtpDataFrame;
            if (dataFrame != null)
            {
                propertyValue = dataFrame[pPropertyName];
                _propertyValueCache[pPropertyName] = propertyValue;
            }

            return propertyValue;
        }

        internal void SetPropertyValue(string pPropertyName, string pValue)
        {
            var dataFrame = Tag as RmtpDataFrame;
            if (dataFrame != null)
                dataFrame[pPropertyName] = pValue;

            _propertyValueCache[pPropertyName] = pValue;
        }
    }
}
