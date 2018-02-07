using CO_IA.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.IO;
using CO_IA.Client;
using DevExpress.Xpf.Editors;

namespace CO_IA.UI.FreqStation.StationPlan
{
    /// <summary>
    /// SendLicenseDialog.xaml 的交互逻辑
    /// </summary>
    public partial class SendLicenseDialog : Window, INotifyPropertyChanged
    {
        List<LicenseItem> LicenseItems;
        XmlNode tempxmlnode = null;
        List<string> QRCodeFields;
        List<EquipmentInspection> lstequs;
        private bool IsSendLicense;
        public event Action AfterSendLicense;

        private LicenseTemplete licenseTemplete;
        public LicenseTemplete LicenseTemplete
        {
            get
            {
                return licenseTemplete;
            }
            set
            {
                licenseTemplete = value;
                NotifyPropertyChanged("LicenseTemplete");
            }
        }
        public SendLicenseDialog(List<EquipmentInspection> equs)
        {
            InitializeComponent();
            this.DataContext = this;
            lstequs = equs;
            //InitLicenseTemplete();
            if(InitLicenseTemplete())
            {
                InitLicenseView(equs);
            }
        }

        private bool InitLicenseTemplete()
        {
            LicenseTemplete = FreqStationHelper.GetLicenseTemplete(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
            if (LicenseTemplete != null)
            {
                LoadXml(LicenseTemplete);
                if (tempxmlnode != null)
                {
                    XmlNode xmlnode = tempxmlnode.SelectSingleNode(FreqStationHelper.licenseItems);
                    this.LicenseItems = FreqStationHelper.XMLLicenseItems(xmlnode);
                    if (this.LicenseTemplete.IsShowQRCode)
                    {
                        XmlNode fieldsnode = tempxmlnode.SelectSingleNode(FreqStationHelper.qrCodeFields);
                        this.QRCodeFields = FreqStationHelper.XMLQRCodeFields(fieldsnode);
                    }
                }
                return true;
            }
           
            return false;
        }


        private void LoadXml(LicenseTemplete licenseTemplete)
        {
            try
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.LoadXml(licenseTemplete.XMLLicenseItems);
                tempxmlnode = xmldoc.SelectSingleNode(FreqStationHelper.template);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetExceptionMessage());
            }
        }

        private void InitLicenseView(List<EquipmentInspection> equs)
        {
            foreach (EquipmentInspection equ in equs)
            {
                LicenseViewControl licenseView = new LicenseViewControl();
                licenseView.Margin = new Thickness(5);
                if (LicenseItems != null)
                {
                    foreach (LicenseItem item in LicenseItems)
                    {
                        item.PropertyValue = GetPropertyValue(item.PropertyName.Trim(), equ);
                        licenseView.AddLicenseItem(item);
                    }
                }
                if (LicenseTemplete.IsShowQRCode)
                {
                    if (tempxmlnode != null)
                    {
                        double left = 0, top = 0;
                        double height = 100, width = 100;
                        XmlNode xmlnode = tempxmlnode.SelectSingleNode(FreqStationHelper.qrCodeProperty);
                        if (xmlnode != null)
                        {
                            FreqStationHelper.XMLQRCodeProperty(xmlnode, out left, out top, out height, out width);
                        }
                        licenseView.InitQRCodeProperty(left, top, height, width);
                    }

                    byte[] qrcode = CreateQRCode(equ);
                    licenseView.CreateQRCode(qrcode);
                    //LicenseTemplete.QRCode = CreateQRCode(equ);
                }
                licenseView.DataContext = LicenseTemplete;
                this._contentPanel.Children.Add(licenseView);
            }
        }

        private byte[] CreateQRCode(EquipmentInspection equ)
        {
            if (this.QRCodeFields != null && QRCodeFields.Count > 0)
            {
                StringBuilder str = new StringBuilder();
                foreach (string field in QRCodeFields)
                {
                    string value = field.Replace(" ", "");
                    str.AppendLine(string.Format("{0}:{1}", field, GetPropertyValue(value, equ)));
                }
                try
                {
                    return FreqStationHelper.CreateQRCode(str.ToString());
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }

        private string GetPropertyValue(string propertyname, EquipmentInspection equ)
        {
            switch (propertyname)
            {
                case "用户信息":
                    return equ.ApplyPerson;
                case "单位名称":
                    return equ.ApplyORG;
                case "设备名称":
                    return equ.ActivityEquipment.Name;
                case "设备编号":
                    return equ.ActivityEquipment.EquModel;
                case "有效期":
                    string str = string.Empty;
                    if (equ.RunningFrom != null)
                    {
                        str += equ.RunningFrom.Value.ToShortDateString();
                    }
                    if (equ.RunningTo != null)
                    {
                        str += " 至 " + equ.RunningTo.Value.ToShortDateString();
                    }
                    return str;
                case "使用区域":
                    return equ.ApplyArea;
            }
            return null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string proname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(proname));
            }
        }

        private void btnSendLicense_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FreqStationHelper.SendLicenses(lstequs);
                MessageBox.Show("许可证发放成功");
                IsSendLicense = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetExceptionMessage());
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (_contentPanel.Children.Count > 0)
            {
                Utility.PrintElement(_contentPanel);
            }
            else
            {
                MessageBox.Show("没有需要打印的许可证");
            }

            //PrintDialog dialog = new PrintDialog();
            //if (dialog.ShowDialog() == true)
            //{
            //    //int pageMargin = 5;
            //    //Size pageSize = new Size(dialog.PrintableAreaWidth-pageMargin*2,dialog.PrintableAreaHeight-20);
            //    //_contentPanel.Measure(pageSize);
            //    //_contentPanel.Arrange(new Rect(pageMargin,pageMargin,pageSize.Width,pageSize.Height));

            //    dialog.PrintVisual(_contentPanel, "许可证模板");
            //    this.Close();
            //}
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (IsSendLicense)
            {
                if (AfterSendLicense != null)
                {
                    AfterSendLicense();
                }
            }
        }

        /// <summary>
        /// 边框设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_contentPanel.Children.Count > 0)
            {
                double left = double.Parse(this.txtleft.EditValue.ToString());
                double top = double.Parse(this.txttop.EditValue.ToString());
                double right = double.Parse(this.txtright.EditValue.ToString());
                double bottom = double.Parse(this.txtbottom.EditValue.ToString());

                foreach (LicenseViewControl item in _contentPanel.Children)
                {
                    item.Margin = new Thickness(left, top, right, bottom);
                }
            }
        }

        private void MarginValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            //if (_contentPanel.Children.Count > 0)
            //{
            //    if (LicenseTemplete != null)
            //    {
            //        TextEdit txtedit = sender as TextEdit;
            //        double marginvalue = double.Parse(txtedit.EditValue.ToString());
            //        string type = txtedit.Tag.ToString();
            //        switch (type)
            //        {
            //            case "Top":
            //            case "Bottom":
            //                if (marginvalue > LicenseTemplete.Height)
            //                {
            //                    MessageBox.Show("");
            //                }
            //                break;
            //            case "Left":
            //            case "Right":
            //                if (marginvalue > LicenseTemplete.Width)
            //                {
            //                    MessageBox.Show("");
            //                }
            //                break;
            //        }
            //    }
            //}
        }
    }
}