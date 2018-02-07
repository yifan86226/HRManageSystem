using CO_IA.Data;
using I_CO_IA.FreqStation;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;

namespace CO_IA.UI.FreqStation.StationPlan
{
    /// <summary>
    /// LicenseTempleteDialog.xaml 的交互逻辑
    /// </summary>
    public partial class LicenseTempleteDialog : Window, INotifyPropertyChanged
    {
        private LicenseTemplete licenseTemplete
        { get; set; }

        private Activity CurrentActivity = CO_IA.Client.RiasPortal.ModuleContainer.Activity;
        LicenseItemControl selectLicenseItemControl = new LicenseItemControl();
        private List<string> property = new List<string>();
        private string SelectField;  //选择树节点
        bool isDropLicense = false;
        bool isDropQrCode = false;
        double defaultheight = 300;
        double defaultwidth = 400;
        double qrdefaultheight = 100;
        double qrdefaultwidth = 100;

        bool isqrCodeselect = false;
        public bool IsQRCodeSelect
        {
            get { return isqrCodeselect; }
            set
            {
                isqrCodeselect = value;
                NotifyPropertyChanged("IsQRCodeSelect");
            }
        }

        public bool IsLicenseSelect
        {
            get;
            set;
        }

        private byte[] qrCodeImageSource;
        public byte[] QRCodeImageSource
        {
            get
            {
                return qrCodeImageSource;
            }
            set
            {
                qrCodeImageSource = value;
                NotifyPropertyChanged("QRCodeImageSource");
            }
        }

        private List<string> QRFields
        {
            get;
            set;
        }

        private byte[] backgroundImageSource;
        public byte[] BackgroundImageSource
        {
            get
            {
                return backgroundImageSource;
            }
            set
            {
                backgroundImageSource = value;
                NotifyPropertyChanged("BackgroundImageSource");
            }
        }

        public LicenseTempleteDialog()
        {
            InitializeComponent();
            this.DataContext = this;
            treefiles.ItemsSource = FreqStationHelper.Fields;
            InitLicenseTemplete();
        }

        private void InitLicenseTemplete()
        {
            licenseTemplete = FreqStationHelper.GetLicenseTemplete(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
            if (licenseTemplete != null)
            {
                this._txtHeight.EditValue = licenseTemplete.Height.ToString();
                this._txtWidth.EditValue = licenseTemplete.Width.ToString();
                this.defaultheight = licenseTemplete.Height;
                this.defaultwidth = licenseTemplete.Width;
                this.BackgroundImageSource = licenseTemplete.BackgroundImage;
                this.QRCodeImageSource = licenseTemplete.QRCode;
                this._chkImage.IsChecked = licenseTemplete.IsShowImage;
                this._chkQRCode.IsChecked = licenseTemplete.IsShowQRCode;

                XmlDocument xmldoc = null;
                try
                {
                    xmldoc = new XmlDocument();
                    xmldoc.LoadXml(licenseTemplete.XMLLicenseItems);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.GetExceptionMessage());
                    return;
                }

                XmlNode tempxmlnode = xmldoc.SelectSingleNode(FreqStationHelper.template);
                XmlNode xmlnode = tempxmlnode.SelectSingleNode(FreqStationHelper.licenseItems);
                if (xmlnode != null && xmlnode.HasChildNodes)
                {
                    licenseTemplete.LicenseItems = FreqStationHelper.XMLLicenseItems(xmlnode);
                }
                if (licenseTemplete.IsShowQRCode)
                {
                    XmlNode qrpropertynode = tempxmlnode.SelectSingleNode(FreqStationHelper.qrCodeProperty);
                    if (qrpropertynode != null)
                    {
                        InitInitQRCodeProperty(qrpropertynode);
                    }

                    XmlNode qrfieldsnode = tempxmlnode.SelectSingleNode(FreqStationHelper.qrCodeFields);
                    if (qrfieldsnode != null && qrfieldsnode.HasChildNodes)
                    {
                        this.QRFields = FreqStationHelper.XMLQRCodeFields(qrfieldsnode);
                    }
                }

                foreach (LicenseItem item in licenseTemplete.LicenseItems)
                {
                    LicenseItemControl licenseitem = CreateLicenseItemControl(item.PropertyName);
                    Canvas.SetLeft(licenseitem, item.Left);
                    Canvas.SetTop(licenseitem, item.Top);
                    _canvasPanel.Children.Add(licenseitem);
                    property.Add(item.PropertyName);
                }
            }
            else
            {
                licenseTemplete = new LicenseTemplete();
            }
        }

        private void InitInitQRCodeProperty(XmlNode node)
        {
            double left, top, height, width;
            FreqStationHelper.XMLQRCodeProperty(node, out left, out top, out height, out width);
            Canvas.SetTop(_borderqriamge, top);
            Canvas.SetLeft(_borderqriamge, left);
            this._txtqrHeight.EditValue = height.ToString();
            this._txtqrWidth.EditValue = width.ToString();
            this.qrdefaultheight = height;
            this.qrdefaultwidth = width;
        }

        private void TreeViewItem_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                TreeViewItem selectitem = sender as TreeViewItem;
                SelectField = selectitem.Header.ToString();
                isDropLicense = false;
                string str = SelectField.Replace(" ", "");
                if (!property.Contains(str))
                {
                    DragDropEffects effects = DragDrop.DoDragDrop(selectitem, sender, DragDropEffects.Move);
                }
                else
                {
                    DragDropEffects effects = DragDrop.DoDragDrop(selectitem, sender, DragDropEffects.None);
                }
            }
        }

        private void licenseitem_MouseMove(object sender, MouseEventArgs e)
        {
            int time = e.Timestamp;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                selectLicenseItemControl = sender as LicenseItemControl;
                isDropLicense = true;
                DragDrop.DoDragDrop(selectLicenseItemControl, sender, DragDropEffects.Move);
            }
        }

        private void licenseitem_MouseEnter(object sender, MouseEventArgs e)
        {
            selectLicenseItemControl = sender as LicenseItemControl;
            foreach (FrameworkElement itemcontrol in _canvasPanel.Children)
            {
                if (itemcontrol.GetType() == typeof(LicenseItemControl))
                {
                    LicenseItemControl licenseControl = (LicenseItemControl)itemcontrol;
                    if (licenseControl != selectLicenseItemControl)
                    {
                        licenseControl.IsSelect = false;
                    }
                }
            }
            selectLicenseItemControl.IsSelect = true;
            isDropLicense = false;
        }

        private void licenseitem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //selectLicenseItemControl.IsSelect = true;
            //isDropLicense = false;
        }

        /// <summary>
        /// 二维码移动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _qrCodeImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                isDropQrCode = true;
                DragDrop.DoDragDrop(this, sender, DragDropEffects.Move);
            }
        }

        private void _borderCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.IsQRCodeSelect = false;

            //注释,在此处 IsSelect = false，影响_canvasPanel_Drop
            //if (selectLicenseItemControl != null)
            //{
            //    selectLicenseItemControl.IsSelect = false;
            //}
        }


        /// <summary>
        /// Canvas拖拽
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _canvasPanel_Drop(object sender, DragEventArgs e)
        {
            Point p = e.GetPosition(_canvasPanel);
            if (isDropLicense)
            {
                Canvas.SetLeft(selectLicenseItemControl, p.X);
                Canvas.SetTop(selectLicenseItemControl, p.Y);
            }
            else if (isDropQrCode)
            {
                Canvas.SetLeft(_borderqriamge, p.X);
                Canvas.SetTop(_borderqriamge, p.Y);
            }
            else
            {
                LicenseItemControl licenseitem = CreateLicenseItemControl(SelectField);
                Canvas.SetLeft(licenseitem, p.X);
                Canvas.SetTop(licenseitem, p.Y);
                _canvasPanel.Children.Add(licenseitem);
                property.Add(SelectField);
            }
            isDropLicense = false;
            isDropQrCode = false;
        }

        private LicenseItemControl CreateLicenseItemControl(string propertyname)
        {
            LicenseItemControl licenseitem = new LicenseItemControl();
            licenseitem.AllowDrop = true;
            licenseitem.PropertyName = propertyname.Replace(" ", ""); ;
            licenseitem.PropertyValue = "XX" + propertyname;
            licenseitem.MouseEnter += licenseitem_MouseEnter;
            licenseitem.MouseMove += licenseitem_MouseMove;
            licenseitem.DeleteLicenseItem += licenseitem_DeleteLicenseItem;
            return licenseitem;
        }

        private void licenseitem_DeleteLicenseItem(LicenseItemControl obj)
        {
            _canvasPanel.Children.Remove(obj);
            property.Remove(obj.PropertyName);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _btnSave_Click(object sender, RoutedEventArgs e)
        {
            LicenseTemplete licenseTemplete = new LicenseTemplete();
            licenseTemplete.ActivityGuid = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid;
            licenseTemplete.Height = double.Parse(this._txtHeight.Text);
            licenseTemplete.Width = double.Parse(this._txtWidth.Text);

            licenseTemplete.IsShowImage = _chkImage.IsChecked.Value;
            licenseTemplete.BackgroundImage = this.backgroundImageSource;

            licenseTemplete.IsShowQRCode = _chkQRCode.IsChecked.Value;
            if (this.QRCodeImageSource == null)
            {
                MessageBox.Show("请生成二维码");
                return;
            }
            else
            {
                licenseTemplete.QRCode = this.QRCodeImageSource;
            }

            if (_canvasPanel.Children.Count > 0)
            {
                foreach (FrameworkElement itemcontrol in _canvasPanel.Children)
                {
                    if (itemcontrol.GetType() == typeof(LicenseItemControl))
                    {
                        LicenseItemControl licenseControl = (LicenseItemControl)itemcontrol;

                        LicenseItem licenseitem = new LicenseItem();
                        licenseitem.Left = (double)licenseControl.GetValue(Canvas.LeftProperty);
                        licenseitem.Top = (double)licenseControl.GetValue(Canvas.TopProperty);
                        licenseitem.PropertyName = licenseControl.PropertyName;
                        licenseTemplete.LicenseItems.Add(licenseitem);
                    }
                }
            }
            else
            {
                MessageBox.Show("请添加模板内容");
                return;
            }
            licenseTemplete.XMLLicenseItems = GetXmlByLicenseTemplete(licenseTemplete);
            try
            {
                SaveLicenseTemplete(licenseTemplete);
                MessageBox.Show("保存成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetExceptionMessage());
            }
        }

        /// <summary>
        /// 构造xml
        /// </summary>
        /// <param name="_licenseTemplete"></param>
        /// <returns></returns>
        private string GetXmlByLicenseTemplete(LicenseTemplete licenseTemplete)
        {
            XmlDocument xmldoc = new XmlDocument();
            XmlNode xmlnode = xmldoc.CreateXmlDeclaration("1.0", "gb2312", null);
            xmldoc.AppendChild(xmlnode);

            //加Template元素
            XmlElement rootxmlelem = xmldoc.CreateElement("", FreqStationHelper.template, "");
            XmlText xmltext = xmldoc.CreateTextNode("");
            rootxmlelem.AppendChild(xmltext);
            xmldoc.AppendChild(rootxmlelem);

            //加入LicenseItems元素
            XmlElement licensexmlelem = xmldoc.CreateElement("", FreqStationHelper.licenseItems, "");
            xmltext = xmldoc.CreateTextNode("");
            licensexmlelem.AppendChild(xmltext);
            rootxmlelem.AppendChild(licensexmlelem);

            //加入LicenseItems子元素
            foreach (LicenseItem item in licenseTemplete.LicenseItems)
            {
                XmlElement element = xmldoc.CreateElement("", item.PropertyName, "");
                xmltext = xmldoc.CreateTextNode("");
                element.AppendChild(xmltext);
                element.SetAttribute("Top", item.Top.ToString());
                element.SetAttribute("Left", item.Left.ToString());
                licensexmlelem.AppendChild(element);
            }

            //加入QRCodeProperty元素
            if (licenseTemplete.IsShowQRCode)
            {
                string left = _borderqriamge.GetValue(Canvas.LeftProperty).ToString();
                string top = _borderqriamge.GetValue(Canvas.TopProperty).ToString();

                XmlElement qrelement = xmldoc.CreateElement("", FreqStationHelper.qrCodeProperty, "");
                xmltext = xmldoc.CreateTextNode("");
                qrelement.AppendChild(xmltext);

                qrelement.SetAttribute("Top", top);
                qrelement.SetAttribute("Left", left);
                qrelement.SetAttribute("Height", this._txtqrHeight.EditValue.ToString());
                qrelement.SetAttribute("Width", this._txtqrWidth.EditValue.ToString());
                rootxmlelem.AppendChild(qrelement);
            }

            if (licenseTemplete.IsShowQRCode)
            {
                XmlElement qrfieldselement = xmldoc.CreateElement("", FreqStationHelper.qrCodeFields, "");
                xmltext = xmldoc.CreateTextNode("");
                qrfieldselement.AppendChild(xmltext);

                foreach (string field in QRFields)
                {
                    string f = field.Replace(" ", "");
                    XmlElement fieldelement = xmldoc.CreateElement("", f, "");
                    xmltext = xmldoc.CreateTextNode("");
                    fieldelement.AppendChild(xmltext);
                    qrfieldselement.AppendChild(fieldelement);
                }
                rootxmlelem.AppendChild(qrfieldselement);
            }

            return xmldoc.InnerXml.ToString();
        }

        private void SaveLicenseTemplete(LicenseTemplete licenseTemplete)
        {
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqStation>(channel =>
            {
                channel.SaveLicenseTemplete(licenseTemplete);
            });
        }

        /// <summary>
        /// 背景图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _chkImage_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chkbox = sender as CheckBox;
            if (chkbox.IsChecked.Value)
            {
                if (BackgroundImageSource == null)
                {
                    BackgroundImageSource = CurrentActivity.Icon;
                }
            }
            else
            {
                BackgroundImageSource = null;
            }
        }


        private void _chkImage_Checked(object sender, RoutedEventArgs e)
        {
            if (licenseTemplete.BackgroundImage != null)
            {
                BackgroundImageSource = licenseTemplete.BackgroundImage;
            }
            else
            {
                BackgroundImageSource = CurrentActivity.Icon;
            }
            //if (BackgroundImageSource == null)
            //{
            //    BackgroundImageSource = CurrentActivity.Icon;
            //}
        }

        private void _chkImage_Unchecked(object sender, RoutedEventArgs e)
        {
            BackgroundImageSource = null;
        }

        /// <summary>
        /// 背景图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlinkBGImage_Click(object sender, RoutedEventArgs e)
        {
            _chkImage.IsChecked = true;
            OpenFileDialog ofd = new OpenFileDialog();//打开选择文件窗口
            ofd.Filter = "jpg|*.jpg|png|*.png|bmp|*.bmp";//过滤器
            if (ofd.ShowDialog() == true)
            {
                string fileName = ofd.FileName;//获得文件的完整路径
                byte[] imageData = File.ReadAllBytes(fileName);
                BackgroundImageSource = imageData;

                //MemoryStream stream = new MemoryStream(imageData);
                //ImageSourceConverter imageSourceConverter = new ImageSourceConverter();
                //BackgroundImageSource = (ImageSource)imageSourceConverter.ConvertFrom(stream);
            }
        }

        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _chkQRCode_Checked(object sender, RoutedEventArgs e)
        {
            if (this.QRCodeImageSource == null)
            {
                MessageBox.Show("还未生成二维码,请先生成二维码");
                this._chkQRCode.IsChecked = false;
            }
            else
            {
                _borderqriamge.Visibility = Visibility.Visible;
                _qrCodeImage.Visibility = Visibility.Visible;

                //ImageSourceConverter imageSourceConverter = new ImageSourceConverter();
                //_qrCodeImage.Source = (ImageSource)imageSourceConverter.ConvertFrom(QRCodeImageSource);
                //_qrCodeImage.Source = this.QRCodeImageSource;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _chkQRCode_Unchecked(object sender, RoutedEventArgs e)
        {
            _borderqriamge.Visibility = Visibility.Collapsed;
            _qrCodeImage.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 二维码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlinkQRCode_Click(object sender, RoutedEventArgs e)
        {
            QRCodeManageDialog qrcode = new QRCodeManageDialog(QRFields, QRCodeImageSource);
            qrcode.UpdateQRCodeSource += (fields, imagesource) =>
            {
                this.QRFields = fields;
                this.QRCodeImageSource = imagesource;

                if (imagesource != null)
                {
                    ImageSourceConverter imageSourceConverter = new ImageSourceConverter();
                    imgQRcode.Source = (ImageSource)imageSourceConverter.ConvertFrom(QRCodeImageSource);
                }
                else
                {
                    imgQRcode.Source = null;
                    MessageBox.Show("请选择生成二维码字段");
                }
            };
            qrcode.ShowDialog();
        }


        #region 宽度、高度

        /// <summary>
        /// 高度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _txtHeight_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(_txtHeight.EditValue.ToString()))
            {

            }
            else
            {
                double height = double.Parse(_txtHeight.EditValue.ToString());
                if (height <= 0)
                {
                    this._txtHeight.EditValue = this.defaultheight.ToString();
                    MessageBox.Show("高度不能小于等于0");
                }
                else if (height > 500)
                {
                    this._txtHeight.EditValue = this.defaultheight.ToString();
                    MessageBox.Show("高度不能大于500");
                }

                this._borderCanvas.Height = double.Parse(_txtHeight.EditValue.ToString());
                this.Height = double.Parse(_txtHeight.EditValue.ToString()) + 150;

            }
        }

        /// <summary>
        /// 宽度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _txtWidth_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {

            if (string.IsNullOrEmpty(_txtWidth.EditValue.ToString()))
            {

            }
            else
            {
                double width = double.Parse(_txtWidth.EditValue.ToString());
                if (width <= 0)
                {
                    this._txtWidth.EditValue = defaultwidth.ToString();
                    MessageBox.Show("宽度不能小于等于0");
                }
                else if (width > 600)
                {
                    this._txtWidth.EditValue = defaultwidth.ToString();
                    MessageBox.Show("宽度不能大于600");
                }

                this._borderCanvas.Width = double.Parse(_txtWidth.EditValue.ToString());
                if (_bordertool.MinWidth <= width)
                {
                    //this._borderCanvas.Width = width;
                    this.Width = double.Parse(_txtWidth.EditValue.ToString()) + 250;
                }
            }
        }


        private void _txtqrHeight_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(_txtqrHeight.EditValue.ToString()))
            {

            }
            else
            {
                double qrheight = double.Parse(_txtqrHeight.EditValue.ToString());
                double tempheight = double.Parse(_txtHeight.EditValue.ToString());
                if (qrheight <= 0)
                {
                    _txtqrHeight.EditValue = this.qrdefaultheight.ToString();
                    MessageBox.Show("高度不能小于等于0");
                }
                else if (qrheight > tempheight)
                {
                    _txtqrHeight.EditValue = this.qrdefaultheight.ToString();
                    MessageBox.Show("高度不能大于模板高度");
                }

                this._qrCodeImage.Height = double.Parse(_txtqrHeight.EditValue.ToString());
            }
        }

        private void _txtqrWidth_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(_txtqrWidth.EditValue.ToString()))
            {

            }
            else
            {
                double qrwidth = double.Parse(_txtqrWidth.EditValue.ToString());
                double tempwidth = double.Parse(_txtWidth.EditValue.ToString());
                if (qrwidth <= 0)
                {
                    _txtqrWidth.EditValue = this.qrdefaultheight.ToString();
                    MessageBox.Show("宽度不能小于等于0");
                }
                else if (qrwidth > tempwidth)
                {
                    _txtqrWidth.EditValue = this.qrdefaultheight.ToString();
                    MessageBox.Show("宽度不能大于模板宽度");
                }

                this._qrCodeImage.Width = double.Parse(_txtqrWidth.EditValue.ToString());
            }
        }

        #endregion

        #region 位置调整

        private void ImageUp_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            double topvalue;
            if (IsQRCodeSelect)
            {
                topvalue = (double)_borderqriamge.GetValue(Canvas.TopProperty);
                if (topvalue > 0)
                {
                    Canvas.SetTop(_borderqriamge, topvalue - 1);
                }
            }
            else if (selectLicenseItemControl != null && selectLicenseItemControl.IsSelect)
            {
                topvalue = (double)selectLicenseItemControl.GetValue(Canvas.TopProperty);
                if (topvalue > 0)
                {
                    Canvas.SetTop(selectLicenseItemControl, (double)selectLicenseItemControl.GetValue(Canvas.TopProperty) - 1);
                }
            }
        }

        private void ImageDown_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            double topvalue;
            if (IsQRCodeSelect)
            {
                topvalue = (double)_borderqriamge.GetValue(Canvas.TopProperty);
                if (this._borderCanvas.Height - topvalue - _borderqriamge.Height > 2)
                {
                    Canvas.SetTop(_borderqriamge, topvalue + 1);
                }
            }
            else if (selectLicenseItemControl != null && selectLicenseItemControl.IsSelect)
            {
                topvalue = (double)selectLicenseItemControl.GetValue(Canvas.TopProperty);
                if (this._borderCanvas.Height - topvalue - 40 > 0)
                {
                    Canvas.SetTop(selectLicenseItemControl, topvalue + 1);
                }
            }
        }

        private void ImageLeft_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            double leftvalue;
            if (IsQRCodeSelect)
            {
                leftvalue = (double)_borderqriamge.GetValue(Canvas.LeftProperty);
                if (leftvalue > 0)
                {
                    Canvas.SetLeft(_borderqriamge, leftvalue - 1);
                }
            }
            else if (selectLicenseItemControl != null && selectLicenseItemControl.IsSelect)
            {
                leftvalue = (double)selectLicenseItemControl.GetValue(Canvas.LeftProperty);
                if (leftvalue > 0)
                {
                    Canvas.SetLeft(selectLicenseItemControl, leftvalue - 1);
                }
            }
        }

        private void ImageRight_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            double leftvalue;
            if (IsQRCodeSelect)
            {
                leftvalue = (double)_borderqriamge.GetValue(Canvas.LeftProperty);
                if (this._borderCanvas.Width - leftvalue - _borderqriamge.Width > 2)
                {
                    Canvas.SetLeft(_borderqriamge, leftvalue + 1);
                }
            }
            else if (selectLicenseItemControl != null && selectLicenseItemControl.IsSelect)
            {
                leftvalue = (double)selectLicenseItemControl.GetValue(Canvas.LeftProperty);
                if (this._borderCanvas.Width - leftvalue - 245 > 0)
                {
                    Canvas.SetLeft(selectLicenseItemControl, leftvalue + 1);
                }
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _qrCodeImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.isDropLicense = false;
            this.IsQRCodeSelect = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string proname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(proname));
            }
        }
    }
}
