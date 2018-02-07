using Best.VWPlatform.Common.Map;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common
{
    /// <summary>
    /// 系统配置
    /// </summary>
    public class VWPlatformConfig : INotifyPropertyChanged
    {
        private static readonly object LockObj = new object();
        private static VWPlatformConfig _config;
        private readonly List<WMonitorParam> _params;
        private MapConfig _mapConfig;
        private Dictionary<string, string> _platformUsers;

        public VWPlatformConfig()
        {
            _params = new List<WMonitorParam>();
            WMonitorParam param1 = new WMonitorParam();
            param1.ClassParamName = "系统设置\\地图类型";
            param1.ParamValue = ConfigurationManager.AppSettings["MapType"];
            _params.Add(param1);

            WMonitorParam param2 = new WMonitorParam();
            param2.ClassParamName = "系统设置\\地图地址";
            param2.ParamValue = ConfigurationManager.AppSettings["MapAddress"];
            _params.Add(param2);

            WMonitorParam param3 = new WMonitorParam();
            param3.ClassParamName = "系统设置\\默认地图范围";
            param3.ParamValue = ConfigurationManager.AppSettings["MapDefaultExtent"];
            _params.Add(param3);
        }
       
        public Dictionary<string, string> PlatformUsers
        {
            get { return _platformUsers ?? (_platformUsers = new Dictionary<string, string>()); }
        }
        /// <summary>
        /// 地图配置参数
        /// </summary>
        public MapConfig MapConfig
        {
            get { return _mapConfig ?? (_mapConfig = new MapConfig()); }
        }

        /// <summary>
        /// 主题程序集名称
        /// </summary>
        public string ThemeAssemblyName
        {
            get;
            set;
        }
        /// <summary>
        /// Rmtp服务联调IP地址，空值不调试
        /// </summary>
        public string RmtpDebugIp { get; set; }

        /// <summary>
        /// WMonitorConfig 唯一实例
        /// </summary>
        public static VWPlatformConfig Current
        {
            get
            {
                if (_config == null)
                {
                    lock (LockObj)
                    {
                        if (_config == null)
                        {
                            _config = new VWPlatformConfig();
                        }
                    }
                }
                return _config;
            }
        }

        internal void LoadConfig(VWPlatformConfig pConfig)
        {
            _config = pConfig;
        }

        internal void Clear()
        {
            if (_params != null)
                _params.Clear();
        }

        /// <summary>
        /// 查询平台配置参数
        /// </summary>
        /// <param name="pParamName">参数名称或参数分类名（如"子目录\孙目录\参数名称"）</param>
        /// <returns>参数值，未找到返回空字符串</returns>
        public string QueryParamValue(string pParamName)
        {
            string upperName = pParamName.ToUpper();
            WMonitorParam sParam = null;
            foreach (WMonitorParam sp in _params)
            {
                if (sp.ClassParamName.ToUpper().CompareTo(upperName) == 0)
                {
                    sParam = sp;
                    break;
                }
            }

            if (sParam != null)
                return sParam.ParamValue;
            return string.Empty;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string pPropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(pPropertyName));
        }
    }

    public class WMonitorParam
    {
        public string ClassParamName
        {
            get;
            set;
        }

        public string ParamValue
        {
            get;
            set;
        }
    }
}
