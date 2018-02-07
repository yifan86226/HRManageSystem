using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Linq;
using Best.VWPlatform.Common.Types;

namespace Best.VWPlatform.Common.Rmtp.DataFrames
{
    /// <summary>
    /// 从本地XML文件读取射频全景参数信息及设备类型等信息
    /// </summary>
    public class DeviceAbilityDataFrame
    {
        private readonly Dictionary<string, DeviceAbilityItem> _propertyValues = new Dictionary<string, DeviceAbilityItem>();
        /// <summary>
        /// 获取驱动器类型
        /// </summary>
        public string DriverName{ get; private set; }
        /// <summary>
        /// 获取设备类型
        /// </summary>
        public string DeviceType { get; private set; }
        /// <summary>
        /// 获取生产厂商
        /// </summary>
        public string Manufacturer { get; private set; }

        /// <summary>
        /// 获取型号
        /// </summary>
        public string Model { get; private set; }
        /// <summary>
        /// 获取可监测的最大频率,-1表示无效，单位：Hz
        /// </summary>
        public double MaxFrequency
        {
            get
            {
                double v;
                if (double.TryParse(_propertyValues["max frequency"].Value.ToString(), out v))
                    return v;
                return -1;
            }
        }
        public string MaxFrequencyUnit
        {
            get { return _propertyValues["max frequency"].Unit; }
        }
        /// <summary>
        /// 获取可监测的最小频率,-1表示无效，单位：Hz
        /// </summary>
        public double MinFrequency
        {
            get
            {
                double v;
                if (double.TryParse(_propertyValues["min frequency"].Value.ToString(), out v))
                    return v;
                return -1;
            }
        }
        public string MinFrequencyUnit
        {
            get { return _propertyValues["min frequency"].Unit; }
        }
        /// <summary>
        /// 获取最大采样率，单位：Hz
        /// </summary>
        public double MaxSample
        {
            get
            {
                double v;
                if (double.TryParse(_propertyValues["max sample rate"].Value.ToString(), out v))
                    return v;
                return -1;
            }
        }
        public string MaxSampleUnit
        {
            get { return _propertyValues["max sample rate"].Unit; }
        }
        /// <summary>
        /// 获取最大span，单位：Hz
        /// </summary>
        public double MaxSpan
        {
            get
            {
                double v;
                if (double.TryParse(_propertyValues["max span"].Value.ToString(), out v))
                    return v;
                return -1;
            }
        }

        public string MaxSpanUnit
        {
            get { return _propertyValues["max span"].Unit; }
        }

        public double MaxEnergy
        {
            get
            {
                double v;
                if (double.TryParse(_propertyValues["max energy"].Value.ToString(), out v))
                    return v;
                return -1;
            }
        }

        public string MaxEnergyUnit
        {
            get { return _propertyValues["max energy"].Unit; }
        }

        public double MinEnergy
        {
            get
            {
                double v;
                if (double.TryParse(_propertyValues["min energy"].Value.ToString(), out v))
                    return v;
                return -1;
            }
        }

        public string MinEnergyUnit
        {
            get { return _propertyValues["min energy"].Unit; }
        }

        private const int Dbuvc = 107;
        public double MaxDbuv
        {
            get
            {
                if (MaxEnergyUnit == "dbm")
                {
                    return MaxEnergy + Dbuvc;
                }
                return MaxEnergy;
            }
        }

        public double MinDbuv
        {
            get
            {
                if (MinEnergyUnit == "dbm")
                {
                    return MinEnergy + Dbuvc;
                }
                return MinEnergy;
            }
        }

        public double MaxDbuvm
        {
            get
            {
                if (MaxEnergyUnit == "dbm")
                {
                    return MaxEnergy;
                }
                return MaxEnergy - Dbuvc;
            }
        }

        public double MinDbuvm
        {
            get
            {
                if (MinEnergyUnit == "dbm")
                {
                    return MinEnergy;
                }
                return MinEnergy - Dbuvc;
            }
        }

        public double MaxAm
        {
            get
            {
                double v;
                if (double.TryParse(_propertyValues["max am"].Value.ToString(), out v))
                    return v;
                return -1;
            }
        }

        public string MaxAmUnit
        {
            get { return _propertyValues["max am"].Unit; }
        }

        public double MinAm
        {
            get
            {
                double v;
                if (double.TryParse(_propertyValues["min am"].Value.ToString(), out v))
                    return v;
                return -1;
            }
        }

        public string MinAmUnit
        {
            get { return _propertyValues["min am"].Unit; }
        }

        public double MaxFm
        {
            get
            {
                double v;
                if (double.TryParse(_propertyValues["max fm"].Value.ToString(), out v))
                    return v;
                return -1;
            }
        }

        public string MaxFmUnit
        {
            get { return _propertyValues["max fm"].Unit; }
        }

        public double MinFm
        {
            get
            {
                double v;
                if (double.TryParse(_propertyValues["min fm"].Value.ToString(), out v))
                    return v;
                return -1;
            }
        }

        public string MinFmUnit
        {
            get { return _propertyValues["min fm"].Unit; }
        }

        public double MaxPm
        {
            get
            {
                double v;
                if (double.TryParse(_propertyValues["max pm"].Value.ToString(), out v))
                    return v;
                return -1;
            }
        }

        public string MaxPmUnit
        {
            get { return _propertyValues["max pm"].Unit; }
        }

        public double MinPm
        {
            get
            {
                double v;
                if (double.TryParse(_propertyValues["min pm"].Value.ToString(), out v))
                    return v;
                return -1;
            }
        }

        public string MinPmUnit
        {
            get { return _propertyValues["min pm"].Unit; }
        }
        /// <summary>
        /// 获取极化方式Peak参数
        /// </summary>
        public IEnumerable<string> PolarizationPeakParameters
        {
            get
            {
                return GetItemParameters("polarization", "peak");
            }
        }
        /// <summary>
        /// 获取极化方式Rms参数
        /// </summary>
        public IEnumerable<string> PolarizationRmsParameters
        {
            get { return GetItemParameters("polarization", "rms"); }
        }
        /// <summary>
        /// 获取分辨带宽视窗名称
        /// </summary>
        public IEnumerable<string> RbwWindows
        {
            get
            {
                var polarization = _propertyValues["window"].Value as XElement;
                if (polarization == null)
                    return null;
                return polarization.Elements().Select(r => r.Name.LocalName).ToList();
            }
        }
        /// <summary>
        /// 获取指定视窗的分辨带宽
        /// </summary>
        /// <param name="pWindowName">视窗</param>
        /// <returns>分辨带宽集合</returns>
        public IEnumerable<string> GetRbws(string pWindowName)
        {
            return GetItemParameters("window", pWindowName);
        }

        private IEnumerable<string> GetItemParameters(string pItemName, string pChildItemName)
        {
            var polarization = _propertyValues[pItemName].Value as XElement;
            if (polarization == null)
                return null;
            var foundItems = from i in polarization.Elements() where i.Name.LocalName.ToLower() == pChildItemName select i;
            if (!foundItems.Any())
                return null;
            var rms = foundItems.First();
            return rms.Elements().Select(r => r.Value).ToList();
        }

        private string GetPropertyValue(XElement pElement, string pPropertyName)
        {
            return pElement.Attribute(pPropertyName) == null ? string.Empty : pElement.Attribute(pPropertyName).Value;
        }
        /// <summary>
        /// 获取手动增益模式参数
        /// </summary>
        /// <param name="pWindowName">手动增益</param>
        /// <returns>手动增益模式参数</returns>
        public IEnumerable<string> GetManualGainModeParam(string pGainMode)
        {
            return GetItemParameters("gainmode", pGainMode);
        }

        /// <summary>
        /// 获取频段和中频测量中频带宽参数
        /// </summary>
        /// <param name="pWindowName">扫描名称</param>
        /// <returns>中频带宽参数</returns>
        public IEnumerable<string> GetIfbwParam(string pIfbw)
        {
            return GetItemParameters("ifbw", pIfbw);
        }

        /// <summary>
        /// 获取中频测量跨距参数
        /// </summary>
        /// <param name="pWindowName">扫描名称</param>
        /// <returns>中频跨距参数</returns>
        public IEnumerable<string> GetSpanParam(string pSpan)
        {
            return GetItemParameters("span", pSpan);
        }
        
        /// <summary>
        /// XML文件解析
        /// </summary>
        public void XmlfileParsePara(XElement element)
        {
            try
            {
                if (element.Name.LocalName == "driver")
                {
                    //驱动名称
                    DriverName = GetPropertyValue(element, "name");
                }

                foreach (var ex in element.Elements())
                {
                    //query xml Elment
                    if (ex.Name.LocalName == "item")
                    {
                        var name = GetPropertyValue(ex, "name");
                        if (string.IsNullOrWhiteSpace(name))
                            continue;
                        var daItem = new DeviceAbilityItem { Name = name, Unit = GetPropertyValue(ex, "unit") };
                        switch (daItem.Name)
                        {
                            case "polarization":
                            case "window":
                            case "gainmode":
                            case "ifbw":
                            case "span":
                                daItem.Value = ex;
                                break;
                            default:
                                daItem.Value = ex.Value;
                                break;
                        }
                        _propertyValues[name] = daItem;
                    }
                    else if (ex.Name.LocalName == "meter")
                    {
                        //设备种类（一体机MDF；测向机DF；接收机receiver）、厂商、型号
                        switch (GetPropertyValue(ex, "type"))
                        {
                            case "mdf":
                                DeviceType = DeviceTypes.一体机;
                                break;
                            case "df":
                                DeviceType = DeviceTypes.测向机;
                                break;
                            case "receiver":
                                DeviceType = DeviceTypes.接收机;
                                break;
                        }
                        Manufacturer = GetPropertyValue(ex, "man");
                        Model = GetPropertyValue(ex, "model");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("参数信息查询，XML解析失败！{0}", ex.Message);
            }
        }
    }

    internal struct DeviceAbilityItem
    {
        public string Name { get; set; }
        public string Unit { get; set; }
        public object Value { get; set; }
    }
}
