using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Best.VWPlatform.Common.Rmtp.DataFrames
{
    public class DeviceAbilityInfoFrame
    {
        private static Dictionary<string, DeviceAbilityDataFrame> _driverProperyValues = null;

        private static DeviceAbilityInfoFrame _instance = null;

        private DeviceAbilityInfoFrame()
        {
            _driverProperyValues = new Dictionary<string, DeviceAbilityDataFrame>();
            XmlDevicefileParse();
        }
        public static DeviceAbilityInfoFrame GetInstance()
        {
            if (_instance == null)
            {
                _instance = new DeviceAbilityInfoFrame();
            }

            return _instance;
        }

        private void XmlDevicefileParse()
        {
            try
            {
                XDocument doc = new XDocument();
                doc = XDocument.Load(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "/ParamConfig/" + "DeviceAbilityInfo.xml");//加载本地XML文件
                string strpara = doc.ToString();//将xml文件转换成字符串
                var xml = XDocument.Parse(strpara);//解析xml字符串
                var xmlElement = xml.Root;
                if (xmlElement.HasElements)
                {
                    foreach (var route in xmlElement.Elements())
                    {
                        //route xml Elment
                        if (!route.HasElements)
                            continue;
                        foreach (var iq in route.Elements())
                        {
                            //iq xml Elment
                            if (!iq.HasElements)
                                continue;
                            foreach (var query in iq.Elements())
                            {
                                //query xml Elment
                                if (!query.HasElements)
                                    continue;
                                
                                DeviceAbilityDataFrame devicedataframe = new DeviceAbilityDataFrame();

                                if (query != null && xmlElement.HasElements)
                                {
                                    devicedataframe.XmlfileParsePara(query);
                                    _driverProperyValues[devicedataframe.DriverName] = devicedataframe;
                                }  
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("参数信息查询，XML解析失败！{0}", ex.Message);
            }
        }

        public DeviceAbilityDataFrame GetCurrentDeviceInfo()
        {
            try
            {
                string path = ConfigurationManager.AppSettings["SensorXmlAddress"];
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                var root = doc.DocumentElement;
                string drivername = root.SelectSingleNode("meter/driver").InnerText;
                if (_driverProperyValues.ContainsKey(drivername))
                {
                    return _driverProperyValues[drivername];
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
