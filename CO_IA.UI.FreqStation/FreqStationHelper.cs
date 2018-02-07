using CO_IA.Data;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using I_CO_IA.FreqStation;
using Microsoft.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Xml;

namespace CO_IA.UI.FreqStation
{
    public class FreqStationHelper
    {
        //存、取XML时的节点名称
        public static string template = "Template";
        public static string licenseItems = "LicenseItems";
        public static string qrCodeProperty = "QRCodeProperty";
        public static string qrCodeFields = "QRCodeFields";

        public static List<string> Fields = new List<string>() { "用户信息", "单位名称", "设备名称", "设备编号", "有 效 期", "使用区域" };

        /// <summary>
        /// 查询设备
        /// ActivityGuid,PlaceGuid 
        /// </summary>
        /// <param name="condition"></param>
        public static ActivityEquipment[] GetActivityEquipments(EquipmentLoadStrategy condition)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqStation, ActivityEquipment[]>(channel =>
            {
                return channel.GetActivityEquipments(condition);
            });
        }

        /// <summary>
        ///  查询设备检测
        /// </summary>
        /// <param name="loadcondition"></param>
        /// <returns></returns>
        public static List<EquipmentInspection> GetEquipmentInspections(EquInspectionQueryCondition loadcondition)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqStation, List<EquipmentInspection>>(channel =>
            {
                return channel.GetEquipmentInspections(loadcondition);
            });
        }

        public static LicenseTemplete GetLicenseTemplete(string activityGuid)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqStation, LicenseTemplete>(channel =>
            {
                return channel.GetLicenseTemplete(activityGuid);
            });
        }

        public static void SendLicenses(List<EquipmentInspection> equs)
        {
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqStation>(channel =>
            {
                channel.SendLicenses(equs);
            });
        }

        public static List<LicenseItem> XMLLicenseItems(XmlNode xmlnode)
        {
            List<LicenseItem> items = new List<LicenseItem>();
            if (xmlnode != null && xmlnode.HasChildNodes)
            {
                XmlNodeList licensenodelst = xmlnode.ChildNodes;
                foreach (XmlNode node in licensenodelst)
                {
                    LicenseItem licenseitem = new LicenseItem();
                    XmlElement xe = (XmlElement)node;
                    if (xe != null)
                    {
                        licenseitem.PropertyName = node.Name.ToString();
                        licenseitem.Left = double.Parse(xe.GetAttribute("Left"));
                        licenseitem.Top = double.Parse(xe.GetAttribute("Top"));
                        items.Add(licenseitem);
                    }
                }
            }
            return items;
        }

        public static void XMLQRCodeProperty(XmlNode node, out double left, out double top, out double height, out double width)
        {
            left = 0;
            top = 0;
            height = 100;
            width = 100;

            //((System.Xml.XmlAttribute)((new System.Linq.SystemCore_EnumerableDebugView(((System.Xml.XmlElement)node).Attributes)).Items[0])).Value
            XmlElement xe = (XmlElement)node;
            if (xe != null)
            {
                left = double.Parse(xe.GetAttribute("Left"));
                top = double.Parse(xe.GetAttribute("Top"));
                height = double.Parse(xe.GetAttribute("Height"));
                width = double.Parse(xe.GetAttribute("Width"));
            }
        }

        public static List<string> XMLQRCodeFields(XmlNode xmlnode)
        {
            List<string> fields = new List<string>();
            foreach (XmlNode item in xmlnode.ChildNodes)
            {
                XmlElement xe = (XmlElement)item;
                if (xe != null)
                {
                    string name = xe.Name.Replace(" ", "");
                    fields.Add(name);
                }
            }
            return fields;
        }

        public static byte[] CreateQRCode(string qrcontent)
        {
            QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
            QrCode qrCode = new QrCode();
            qrEncoder.TryEncode(qrcontent, out qrCode);

            GraphicsRenderer render = new GraphicsRenderer(new FixedModuleSize(5, QuietZoneModules.Two), System.Drawing.Brushes.Black, System.Drawing.Brushes.White);
            MemoryStream memoryStream = new MemoryStream();
            render.WriteToStream(qrCode.Matrix, ImageFormat.Png, memoryStream);

            byte[] bytearray = null;
            memoryStream.Position = 0;
            using (BinaryReader br = new BinaryReader(memoryStream))
            {
                bytearray = br.ReadBytes((int)memoryStream.Length);
            }
            return bytearray;
        }
    }
}
