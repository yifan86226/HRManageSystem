#region 文件描述
/**********************************************************************************
 * 创建人：wangxin
 * 摘  要：许可证模板页面
 * 日  期：2016-09-06
 * ********************************************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Globalization;
using Microsoft.Win32;
using CO_IA.Data.StationPlan;
using CO_IA.Data;
using System.Xml.Linq;
using System.IO;
using CO_IA.Client;


namespace CO_IA.UI.StationPlan
{
    /// <summary>
    /// LicenseSend.xaml 的交互逻辑
    /// </summary>
    public partial class LicenseSend : UserControl
    {
        #region 常量

        private const double PanelHeight = 300;
        private const double PanelWidth = 400;
        private const double ImageHeight = 0;
        private const double ImageWidth = 0;

        #endregion

        public event Action GoBack;

        #region 成员变量
        //控件集合
        private List<FrameworkElement> _ctrlList = new List<FrameworkElement>();
        //是否为拖拽
        private bool _isDragging;
        //模板信息
        public LicenseTempleteInfo _licenseTempleteInfo;

        //模板实体
        public LicenseTemplete _licenseTemplete;
        
        private Point _mousePoint;
        //被拖动的控件
        private FrameworkElement _selectedCtrl;
        //选中控件外边框
        private Border _border;
        //条形码图片
        //private Image _barCodeImage;


        #endregion

        #region 构造函数
        public LicenseSend()
        {
            InitializeComponent();
            AttachEvents();
            CreateBorder();
            InitPage();
        }
        #endregion

        #region 内部方法

        /// <summary>
        /// 初始化页面
        /// </summary>
        private void InitPage()
        {
            _licenseTempleteInfo = GetLicenseTempleteInfo();
            if (_licenseTempleteInfo == null)
            {
                _licenseTempleteInfo = new LicenseTempleteInfo();
                _licenseTempleteInfo.Guid = Utility.NewGuid();
                _licenseTempleteInfo.ActivityGuid = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid;
                LoadBarCodeFrom(CreateEmptyTemplateXml());
                SetAllBtnUnEnable();
            }
            else
            {
                LoadBarCodeFrom(_licenseTempleteInfo.TempXML);
                if (_licenseTempleteInfo.BGImage != null)
                {
                    MemoryStream stream = new MemoryStream(_licenseTempleteInfo.BGImage);
                    BitmapImage bmp = new BitmapImage();
                    bmp.BeginInit();//初始化
                    bmp.StreamSource = stream;//设置源
                    bmp.EndInit();//初始化结束
                    gbImg.ImageSource = bmp;//设置图像Source
                }
                viewLicense();
            }
            InitUIFromBarCode(_licenseTemplete);
        }

        /// <summary>
        /// 获取当前活动的许可证信息
        /// </summary>
        /// <returns></returns>
        private LicenseTempleteInfo GetLicenseTempleteInfo()
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.StationPlan.I_CO_IA_StationPlan, CO_IA.Data.StationPlan.LicenseTempleteInfo>(
                channel =>
                {
                    return channel.GetLicenseTempleteInfo(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
                });
        }

        /// <summary>
        /// 构造空条码模板XML
        /// </summary>
        /// <returns></returns>
        private string CreateEmptyTemplateXml()
        {
                    //创建xml对象
                    XDocument doc = new XDocument();
                    //创建根节点
                    XElement root = new XElement("BarCodes");
                    doc.Add(root);
                    //创建BarCode
                    XElement barCodeElement = new XElement("BarCode");
                    barCodeElement.SetAttributeValue("Height", "300");
                    barCodeElement.SetAttributeValue("Width", "400");
                    barCodeElement.SetAttributeValue("Name", "默认条码模板");
                    barCodeElement.SetAttributeValue("IsUsed", "False");
                    root.Add(barCodeElement);
                    //创建Control
                    XElement CcontrolElement = new XElement("Control");
                    CcontrolElement.SetAttributeValue("Top", "180");
                    CcontrolElement.SetAttributeValue("Left", "300");
                    CcontrolElement.SetAttributeValue("FontSize", "0");
                    CcontrolElement.SetAttributeValue("Column", "");
                    CcontrolElement.SetAttributeValue("Content", "");
                    CcontrolElement.SetAttributeValue("Type", "BarCode");
                    barCodeElement.Add(CcontrolElement);
                    return doc.ToString();
        }

        /// <summary>
        /// 从xml加载条码模板
        /// </summary>
        /// <param name="xml">条码模板XML</param>
        private void LoadBarCodeFrom(string xml)
        {
            _licenseTemplete = BarCodeFactory.CreateBarCodesFromXml(xml.ToString());
        }

        // <summary>
        /// 根据BarCode(条形码)初始化模板编辑页面
        /// </summary>
        /// <param name="pBarCode">条码模板</param>
        private void InitUIFromBarCode(LicenseTemplete pBarCode)
        {
            if (pBarCode == null)
                return;

            InitPanel(pBarCode);

            foreach (var item in pBarCode.Items)
            {
                switch (item.Type)
                {
                    case "Title":
                    case "Value":
                        TextBlock textBlock = new TextBlock();
                        textBlock.Text = item.Content;
                        textBlock.Tag = item.Column;
                        textBlock.FontSize = item.FontSize;
                        _canvasPanel.Children.Add(textBlock);
                        _ctrlList.Add(textBlock);
                        Canvas.SetTop(textBlock, item.Left);
                        Canvas.SetLeft(textBlock, item.Top);
                        foreach (var i in _lstCol.Items)
                        {
                            CheckBox checkBox = ((ContentControl)(i)).Content as CheckBox;
                            if (checkBox.Tag.ToString() == item.Column)
                            {
                                checkBox.IsChecked = true;
                            }
                        }
                        break;
                    case "BarCode":
                        Canvas.SetTop(_barCodeImage, item.Left);
                        Canvas.SetLeft(_barCodeImage, item.Top);
                        break;
                }
            }
        }

        /// <summary>
        /// 初始化面板
        /// </summary>
        private void InitPanel(LicenseTemplete pBarCode)
        {
            //清空显示字段列表
            foreach (var i in _lstCol.Items)
            {
                CheckBox checkBox = ((ContentControl)(i)).Content as CheckBox;
                checkBox.IsChecked = false;
            }

            //清空控件列表
            _ctrlList = new List<FrameworkElement>();

            //初始化面板
            _canvasPanel.Children.Clear();
            _canvasPanel.Height = pBarCode.Height;
            _borderCanvas.Height = pBarCode.Height + 2;
            _canvasPanel.Width = pBarCode.Width;
            _borderCanvas.Width = pBarCode.Width + 2;
            //初始化边框
            CreateBorder();

            //初始化条码图片
            //_barCodeImage = CreateImage();
            _canvasPanel.Children.Add(_barCodeImage);
            _ctrlList.Add(_barCodeImage);

            //设置页面属性
            _txtHeight.Text = _canvasPanel.Height.ToString(CultureInfo.InvariantCulture);
            _txtWidth.Text = _canvasPanel.Width.ToString(CultureInfo.InvariantCulture);
            _txtTemplateName.Text = pBarCode.Name;
            if (pBarCode.IsUsed)
            {
                _isUsed.SelectedItem = _isUsedItem;
                // _isUsedItem.IsSelected = true;
            }
            else
            {
                _isUsed.SelectedItem = _notUsedItem;
                //_notUsedItem.IsSelected = true;
            }
        }

        /// <summary>
        /// 构造条码图片
        /// </summary>
        private Image CreateImage()
        {
            Image image = new Image();
            //BitmapImage bitmap = new BitmapImage(new Uri("../CO_IA.UI.StationPlan/Images/BarCodeTemplate.png", UriKind.Relative));
            BitmapFrame bitmap = BitmapFrame.Create(new Uri("../CO_IA.UI.StationPlan/Images/BarCodeTemplate.png", UriKind.Relative));
            image.Height = ImageHeight;
            image.Width = ImageWidth;
            image.Stretch = Stretch.Fill;
            image.Source = bitmap;
            return image;
        }

        /// <summary>
        /// 构造xml
        /// </summary>
        /// <param name="_licenseTemplete"></param>
        /// <returns></returns>
        private string GetXmlByLicenseTemplete(LicenseTemplete _licenseTemplete)
        {
            //创建xml对象
            XDocument doc = new XDocument();
            //创建根节点
            XElement root = new XElement("BarCodes");
            doc.Add(root);
            //创建BarCode
            XElement barCodeElement = new XElement("BarCode");
            barCodeElement.SetAttributeValue("Height", _licenseTemplete.Height);
            barCodeElement.SetAttributeValue("Width", _licenseTemplete.Width);
            barCodeElement.SetAttributeValue("Name", _licenseTemplete.Name);
            barCodeElement.SetAttributeValue("IsUsed", _licenseTemplete.IsUsed);
            root.Add(barCodeElement);

            foreach (LicenseTempleteItem item in _licenseTemplete.Items)
            {
                //创建Control
                XElement CcontrolElement = new XElement("Control");
                switch (item.Type)
                {
                    case "Title":
                    case "Value":
                        CcontrolElement.SetAttributeValue("Top", item.Top);
                        CcontrolElement.SetAttributeValue("Left", item.Left);
                        CcontrolElement.SetAttributeValue("FontSize", item.FontSize);
                        CcontrolElement.SetAttributeValue("Column", item.Column);
                        CcontrolElement.SetAttributeValue("Content", item.Content);
                        CcontrolElement.SetAttributeValue("Type", item.Type);
                        break;
                    case "BarCode":
                        CcontrolElement.SetAttributeValue("Top", item.Top);
                        CcontrolElement.SetAttributeValue("Left", item.Left);
                        CcontrolElement.SetAttributeValue("FontSize", "0");
                        CcontrolElement.SetAttributeValue("Column", "");
                        CcontrolElement.SetAttributeValue("Content", "");
                        CcontrolElement.SetAttributeValue("Type", item.Type);
                        break;
                }
                barCodeElement.Add(CcontrolElement);
            }
            return doc.ToString();
        }

        /// <summary>
        /// 设置按钮可用
        /// </summary>
        private void SetAllBtnEnable()
        {
            btnPrintPreview.IsEnabled = true;
            btnPrintDlg.IsEnabled = true;
            btnPrintDirect.IsEnabled = true;
        }

        /// <summary>
        /// 设置按钮不可用
        /// </summary>
        private void SetAllBtnUnEnable()
        {
            btnPrintPreview.IsEnabled = false;
            btnPrintDlg.IsEnabled = false;
            btnPrintDirect.IsEnabled = false;
        }

        /// <summary>
        /// 附加事件
        /// </summary>
        private void AttachEvents()
        {
            this.MouseLeftButtonDown += OnMouseLeftButtonDown;
            this.MouseLeftButtonUp += OnMouseLeftButtonUp;
            this.MouseMove += OnMouseMove;

            this._txtTop.LostFocus += OnXYLostFocus;
            this._txtLeft.LostFocus += OnXYLostFocus;
            this._txtTop.KeyDown += OnXYKeyDown;
            this._txtLeft.KeyDown += OnXYKeyDown;

            this._txtHeight.LostFocus += OnHeightLostFocus;
            this._txtHeight.KeyDown += OnHeightKeyDown;
            this._txtWidth.KeyDown += OnWidthKeyDown;
            this._txtWidth.LostFocus += OnWidthLostFocus;

            this._combFontSize.SelectionChanged += OnFontSizeSelectionChanged;
            //this._combBarCodeTemplate.SelectionChanged += OnBarCodeTemplate_SelectionChanged;
            //this._combBarCodeAccount.SelectionChanged += new SelectionChangedEventHandler(CombBarCodeAccount_SelectionChanged);
            //_btnAdd.Click += OnAddClick;
            //_btnModify.Click += OnModifyClick;
            //_btnDelete.Click += OnDeleteClick;
            //_btnSave.Click += OnSaveClick;
            //_btnCancel.Click += OnCancelClick;
        }

        /// <summary>
        /// 构造控件框架
        /// </summary>
        private void CreateBorder()
        {
            //初始化
            _border = new Border()
            {
                Visibility = Visibility.Collapsed,
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Colors.Red)
            };
            //将选中控件边框加入到Canvas中
            _canvasPanel.Children.Add(_border);
        }

        /// <summary>
        /// 添加控件到Canvas中
        /// </summary>
        /// <param name="chk"></param>
        private void Add(CheckBox chk)
        {
            //标题
            TextBlock tbTitle = new TextBlock();
            tbTitle.Text = chk.Content.ToString();
            tbTitle.Tag = chk.Tag;
            tbTitle.FontSize = 16;

            _canvasPanel.Children.Add(tbTitle);
            _ctrlList.Add(tbTitle);
            Canvas.SetTop(tbTitle, 0);
            Canvas.SetLeft(tbTitle, 0);

            //值
            TextBlock tbValue = new TextBlock();
            tbValue.Text = string.Format("{0}值", chk.Content);
            tbValue.Tag = chk.Tag;
            tbValue.FontSize = 16;

            _canvasPanel.Children.Add(tbValue);
            _ctrlList.Add(tbValue);
            Canvas.SetTop(tbValue, 0);
            Canvas.SetLeft(tbValue, 80);
        }

        /// <summary>
        /// 从Canvas中移除控件
        /// </summary>
        /// <param name="chk"></param>
        private void Delete(CheckBox chk)
        {
            int i = 0;
            while (i < _ctrlList.Count)
            {
                FrameworkElement ctrl = _ctrlList[i];
                if (ctrl != null && ctrl.Tag != null && ctrl.Tag.ToString() == chk.Tag.ToString())
                {
                    _ctrlList.Remove(ctrl);
                }
                else
                    i++;
            }
            i = 0;
            while (i < _canvasPanel.Children.Count)
            {
                FrameworkElement ctrl = _canvasPanel.Children[i] as FrameworkElement;
                if (ctrl != null && ctrl.Tag != null && ctrl.Tag.ToString() == chk.Tag.ToString())
                {
                    _canvasPanel.Children.Remove(ctrl);
                }
                else
                {
                    i++;
                }
            }
        }

        /// <summary>
        /// 移动当前控件
        /// </summary>
        private void MoveCtrl()
        {
            if (_selectedCtrl == null) return;

            double top;
            if (!double.TryParse(_txtTop.Text, out top))
            {
                _txtTop.Text = "0";
                top = 0;
            }
            double left;
            if (!double.TryParse(_txtLeft.Text, out left))
            {
                _txtLeft.Text = "0";
                left = 0;
            }

            Canvas.SetTop(_selectedCtrl, top);
            Canvas.SetLeft(_selectedCtrl, left);
            Canvas.SetTop(_border, top);
            Canvas.SetLeft(_border, left);
        }

        /// <summary>
        /// 预览许可证
        /// </summary>
        private void viewLicense()
        {
            ActivityEquipmentInfo equTemp = getTempEquInfo();
            Canvas _viewCanvas = LicenseViewFactory.CreateLicenseViewCanvas(equTemp, _licenseTempleteInfo,true);
            grpView.Content = _viewCanvas;
            //borderView.Child = _viewCanvas;
            //borderView.Height = _licenseTemplete.Height + 2;
            //borderView.Width = _licenseTemplete.Width + 2;
            //grpView.Width = _licenseTemplete.Width + 20;
            //borderView.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 获取设备信息（例子）
        /// </summary>
        /// <returns></returns>
        private ActivityEquipmentInfo getTempEquInfo()
        {
            ActivityEquipmentInfo equTemp = new ActivityEquipmentInfo();
            equTemp.GUID = "";
            equTemp.EquNo = "设备1";
            equTemp.EquModel = "型号1";
            equTemp.EQUCount = 2;
            equTemp.SendFreq = 50.1234;
            equTemp.Band = 60;
            equTemp.ORGGuid = "orgguid";
            equTemp.RunningFrom = DateTime.Now;
            equTemp.RunningTo = DateTime.Now;
            equTemp.OrgInfo.Name = "西安市消防总队";
            equTemp.Remark = "备注";
            return equTemp;
        }
        #endregion

        #region 内部事件
        /// <summary>
        /// 鼠标按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_border == null)
                return;
            //隐藏控件边框
            _border.Visibility = Visibility.Collapsed;
            //获得事件触发的源，即哪个控件      
            _selectedCtrl = e.OriginalSource as FrameworkElement;
            if (_selectedCtrl == null
                || _selectedCtrl.GetType() == typeof(Canvas)
                || _selectedCtrl.Parent == null
                || _selectedCtrl.Parent.GetType() != typeof(Canvas)
                || _selectedCtrl.Tag == null)
                //|| Mode == AssetOperateMode.View)
            {
                return;
            }

            //标记鼠标按下   
            _isDragging = true;
            //获取鼠标在但前canvas内的位置    
            _mousePoint = e.GetPosition(this);

            //移动边框到当前控件上
            _border.Visibility = Visibility.Visible;
            _border.Height = _selectedCtrl.ActualHeight + 4;
            _border.Width = _selectedCtrl.ActualWidth + 4;
            Canvas.SetLeft(_border, Canvas.GetLeft(_selectedCtrl) - 2);
            Canvas.SetTop(_border, Canvas.GetTop(_selectedCtrl) - 2);

            //控件属性
            _txtLeft.Text = Canvas.GetLeft(_selectedCtrl).ToString(CultureInfo.InvariantCulture);
            _txtTop.Text = Canvas.GetTop(_selectedCtrl).ToString(CultureInfo.InvariantCulture);
            foreach (var item in _combFontSize.Items)
            {
                if (_selectedCtrl is TextBlock && (item as ComboBoxItem).Content.ToString() == (_selectedCtrl as TextBlock).FontSize.ToString())
                {
                    _combFontSize.SelectedItem = item;
                }
            }
        }

        /// <summary>
        /// 鼠标抬起事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
        }

        /// <summary>
        /// 鼠标移动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging) return;
            Point theMousePoint = e.GetPosition(this);
            double left = Canvas.GetLeft(_selectedCtrl) + (theMousePoint.X - _mousePoint.X);
            double top = Canvas.GetTop(_selectedCtrl) + (theMousePoint.Y - _mousePoint.Y);
            //移动当前控件
            Canvas.SetLeft(_selectedCtrl, left);
            Canvas.SetTop(_selectedCtrl, top);
            //移动边框到控件上
            Canvas.SetLeft(_border, left - 2);
            Canvas.SetTop(_border, top - 2);
            //记录当前坐标
            _mousePoint = theMousePoint;
            _txtLeft.Text = Canvas.GetLeft(_selectedCtrl).ToString(CultureInfo.InvariantCulture);
            _txtTop.Text = Canvas.GetTop(_selectedCtrl).ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 页面高度KeyDown事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnHeightKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            _canvasPanel.Height = double.Parse(_txtHeight.Text);
            _borderCanvas.Height = double.Parse(_txtHeight.Text) + 2;
        }

        /// <summary>
        /// 页面高度LostFocus事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnHeightLostFocus(object sender, RoutedEventArgs e)
        {
            _canvasPanel.Height = double.Parse(_txtHeight.Text);
            _borderCanvas.Height = double.Parse(_txtHeight.Text) + 2;
        }

        /// <summary>
        /// 页面宽度KeyDown事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWidthKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            _canvasPanel.Width = double.Parse(_txtWidth.Text);
            _borderCanvas.Width = double.Parse(_txtWidth.Text) + 2;
        }
        /// <summary>
        /// 页面宽度LostFocus事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWidthLostFocus(object sender, RoutedEventArgs e)
        {
            _canvasPanel.Width = double.Parse(_txtWidth.Text);
            _borderCanvas.Width = double.Parse(_txtWidth.Text) + 2;
        }

        /// <summary>
        /// 控件上和做KeyDown事件，移动控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnXYKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            MoveCtrl();
        }

        /// <summary>
        /// 控件上和做LostFocus事件，移动控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnXYLostFocus(object sender, RoutedEventArgs e)
        {
            MoveCtrl();
        }

        /// <summary>
        /// 字体选择改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFontSizeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_selectedCtrl == null || _selectedCtrl.GetType() != typeof(TextBlock))
                return;
            ComboBoxItem comboBoxItem = _combFontSize.SelectedItem as ComboBoxItem;
            if (comboBoxItem != null)
            {
                (_selectedCtrl as TextBlock).FontSize = double.Parse(comboBoxItem.Content.ToString());
                _border.Height = _selectedCtrl.ActualHeight;
                _border.Width = _selectedCtrl.ActualWidth;
            }
        }

        /// <summary>
        /// 打印预览按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrintPreview_Click(object sender, RoutedEventArgs e)
        {
            //PrintPreviewLicense previewWnd = new PrintPreviewLicense();
            //previewWnd.ShowInTaskbar = false;
            //previewWnd.ShowDialog();
        }

        /// <summary>
        /// 批量打印按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrintDlg_Click(object sender, RoutedEventArgs e)
        {
            BatchPrint batchprint = new BatchPrint();
            batchprint.ShowDialog(this);
        }

        /// <summary>
        /// 直接打印按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrintDirect_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog dialog = new PrintDialog();
            if (dialog.ShowDialog() == true)
            {
                //PrintPreviewLicense _test = new PrintPreviewLicense();
                //dialog.PrintVisual(_canvasPanel, "许可证模板");
            }
        }

        /// <summary>
        /// 返回按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGoBack_Click(object sender, RoutedEventArgs e)
        {
            if (GoBack != null)
            {
                GoBack();
            }
        }

        /// <summary>
        /// 显示字段选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            if (chk.IsChecked.Value)
            {
                Add(chk);
            }
            else
            {
                Delete(chk);
            }
        }

        private void _btnSave_Click(object sender, RoutedEventArgs e)
        {
            LicenseTemplete _editingBarCode = new LicenseTemplete();
            _editingBarCode.Name = _txtTemplateName.Text;
            _editingBarCode.Height = double.Parse(_txtHeight.Text);
            _editingBarCode.Width = double.Parse(_txtWidth.Text);
            if ((_isUsed.SelectedItem as ComboBoxItem).Content.ToString() == "在用")
            {
                _editingBarCode.IsUsed = true;
            }
            _editingBarCode.Items = new List<LicenseTempleteItem>();
            foreach (var item in _ctrlList)
            {
                LicenseTempleteItem barCodeItem = new LicenseTempleteItem();
                barCodeItem.Top = Canvas.GetLeft(item);
                barCodeItem.Left = Canvas.GetTop(item);
                if (item is Image)
                {
                    barCodeItem.Type = "BarCode";
                }
                else
                {
                    TextBlock tb = item as TextBlock;
                    barCodeItem.Type = (tb.Text.Substring(tb.Text.Length - 1) == "值" && tb.Text.Substring(tb.Text.Length - 2) != "价值") ? "Value" : "Title";
                    barCodeItem.FontSize = tb.FontSize;
                    barCodeItem.Column = tb.Tag.ToString();
                    barCodeItem.Content = tb.Text;
                }
                _editingBarCode.Items.Add(barCodeItem);
            }
            _licenseTempleteInfo.TempXML = GetXmlByLicenseTemplete(_editingBarCode);
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.StationPlan.I_CO_IA_StationPlan>(
                channel =>
                {
                    channel.SaveLicenseTempleteInfo(_licenseTempleteInfo);
                    MessageBox.Show("保存成功", "提示", MessageBoxButton.OK);
                    viewLicense();
                });
            SetAllBtnEnable();
        }

        /// <summary>
        /// 上传背景图片按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUploadBgImg_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();//打开选择文件窗口
            ofd.Filter = "jpg|*.jpg|png|*.png|bmp|*.bmp|gif|*.gif";//过滤器
            if (ofd.ShowDialog() == true)
            {
                string fileName = ofd.FileName;//获得文件的完整路径
                _licenseTempleteInfo.BGImage = File.ReadAllBytes(fileName);//把图像的二进制数据存储到emp的Photo属性中
                gbImg.ImageSource = new BitmapImage(new Uri(fileName));//将图片显示到Image控件上
            }
        }

        #endregion

    }

    /// <summary>
    /// 条码模板工厂
    /// </summary>
    public class BarCodeFactory
    {
        /// <summary>
        /// 从xml中加载对应帐套的BarCode（条形码)信息
        /// </summary>
        /// <param name="pXml">条码Xml信息</param>
        /// <returns></returns>
        public static LicenseTemplete CreateBarCodesFromXml(string pXml)
        {
            TextReader txtReader = new StringReader(pXml);
            XDocument xDoc = XDocument.Load(txtReader);
            var query = from f in xDoc.Descendants("BarCode")
                        select new LicenseTemplete
                        {
                            Name = f.Attribute("Name").Value,
                            Height = double.Parse(f.Attribute("Height").Value),
                            Width = double.Parse(f.Attribute("Width").Value),
                            IsUsed = bool.Parse(f.Attribute("IsUsed").Value),
                            Items = (from c in f.Descendants("Control")
                                     select new LicenseTempleteItem
                                     {
                                         Top = double.Parse(c.Attribute("Top").Value),
                                         Left = double.Parse(c.Attribute("Left").Value),
                                         FontSize = double.Parse(c.Attribute("FontSize").Value),
                                         Column = c.Attribute("Column").Value,
                                         Content = c.Attribute("Content").Value,
                                         Type = c.Attribute("Type").Value
                                     }).ToList()
                        };
            return new ObservableCollection<LicenseTemplete>(query)[0];
        }
    }

    /// <summary>
    /// 许可证预览工厂
    /// </summary>
    public class LicenseViewFactory
    {
        /// <summary>
        /// 创建许可证预览的Canvas
        /// </summary>
        /// <param name="equInfo">设备信息</param>
        /// <param name="tempInfo">许可证模板信息</param>
        /// <param name="isPrintBgImg">是否打印背景图片</param>
        /// <returns></returns>
        public static Canvas CreateLicenseViewCanvas(ActivityEquipmentInfo equInfo, LicenseTempleteInfo tempInfo, bool isPrintBgImg)
        {
            LicenseTemplete temp = BarCodeFactory.CreateBarCodesFromXml(tempInfo.TempXML);
            LicenseViewInfo license = new LicenseViewInfo(equInfo);
            Canvas _viewCanvas = new Canvas();
            _viewCanvas.Height = temp.Height;
            _viewCanvas.Width = temp.Width;
            if (isPrintBgImg)
            {
                ImageBrush image = new ImageBrush();
                if (tempInfo.BGImage != null)
                {
                    MemoryStream stream = new MemoryStream(tempInfo.BGImage);
                    BitmapImage bmp = new BitmapImage();
                    bmp.BeginInit();//初始化
                    bmp.StreamSource = stream;//设置源
                    bmp.EndInit();//初始化结束
                    image.ImageSource = bmp;//设置图像Source
                }
                _viewCanvas.Background = image;
            }
            LicenseViewInfo viewInfo = new LicenseViewInfo(equInfo);
            _viewCanvas.DataContext = viewInfo;
            foreach (var item in temp.Items)
            {
                switch (item.Type)
                {
                    case "Title":
                        TextBlock textBlock = new TextBlock();
                        textBlock.Text = item.Content + ":";
                        textBlock.FontSize = item.FontSize;
                        _viewCanvas.Children.Add(textBlock);
                        Canvas.SetTop(textBlock, item.Left);
                        Canvas.SetLeft(textBlock, item.Top);
                        break;
                    case "Value":
                        TextBlock textBlockValue = new TextBlock();
                        textBlockValue.FontSize = item.FontSize;
                        textBlockValue.Width = temp.Width - item.Top - 10;
                        textBlockValue.TextWrapping = TextWrapping.Wrap;
                        _viewCanvas.Children.Add(textBlockValue);
                        Canvas.SetTop(textBlockValue, item.Left);
                        Canvas.SetLeft(textBlockValue, item.Top);
                        textBlockValue.SetBinding(TextBlock.TextProperty, new Binding(item.Column));
                        break;
                    case "BarCode":
                        //初始化条码图片
                        //private Image _barCodeImage = CreateImage();
                        //_viewCanvas.Children.Add(_barCodeImage);
                        //Canvas.SetTop(_barCodeImage, item.Left);
                        //Canvas.SetLeft(_barCodeImage, item.Top);
                        break;
                }
            }

            return _viewCanvas;
        }
    }

    #region 实体类
    /// <summary>
    /// 模板实体
    /// </summary>
    public class LicenseTemplete
    {
        public string Name { get; set; }

        public double Height { get; set; }

        public double Width { get; set; }

        public bool IsUsed { get; set; }

        //public string AccountCode { get; set; }

        public List<LicenseTempleteItem> Items { get; set; }
    }

    /// <summary>
    /// 模板字段
    /// </summary>
    public class LicenseTempleteItem
    {
        public double Top { get; set; }

        public double Left { get; set; }

        public double FontSize { get; set; }

        public string Column { get; set; }

        public string Content { get; set; }

        public string Type { get; set; }
    }

    /// <summary>
    /// 许可证中显示的信息
    /// </summary>
    public class LicenseViewInfo
    {
        public LicenseViewInfo(ActivityEquipmentInfo equInfo)
        {
            this._guid = equInfo.GUID;
            this._equCode = equInfo.EquNo;
            this._equNum = equInfo.EQUCount;
            this._equType = equInfo.EquModel;
            if (equInfo.SendFreq != null)
            {
                this._sendFreq = equInfo.SendFreq.Value;
            }
            else
            {
                this._sendFreq = 0;
            }
            
            if (equInfo.Band != null)
            {
                this._band = equInfo.Band.Value;
            }
            else
            {
                this._band = 0;
            }
            this._timeLimit = ((DateTime)equInfo.RunningFrom).ToString("yyyy年MM月dd日") + " - " + ((DateTime)equInfo.RunningTo).ToString("yyyy年MM月dd日");
            this._orgName = equInfo.OrgInfo.Name;
            this._remark = equInfo.Remark;
        }
        private string _guid;
        public string Guid
        {
            get
            {
                return this._guid;
            }
            set
            {
                this._guid = value;
            }
        }

        private string _equName;
        public string EquName
        {
            get
            {
                return this._equName;
            }
            set
            {
                this._equName = value;
            }
        }

        private string _equCode;
        public string EquCode
        {
            get
            {
                return this._equCode;
            }
            set
            {
                this._equCode = value;
            }
        }

        private int _equNum;
        public int EquNum
        {
            get
            {
                return this._equNum;
            }
            set
            {
                this._equNum = value;
            }
        }

        private string _equType;
        public string EquType
        {
            get
            {
                return this._equType;
            }
            set
            {
                this._equType = value;
            }
        }

        private double _sendFreq;
        public double SendFreq
        {
            get
            {
                return this._sendFreq;
            }
            set
            {
                this._sendFreq = value;
            }
        }

        private double _band;
        public double Band
        {
            get
            {
                return this._band;
            }
            set
            {
                this._band = value;
            }
        }

        private string _timeLimit;
        public string TimeLimit
        {
            get
            {
                return this._timeLimit;
            }
            set
            {
                this._timeLimit = value;
            }
        }

        private string _orgName;
        public string OrgName
        {
            get
            {
                return this._orgName;
            }
            set
            {
                this._orgName = value;
            }
        }

        private string _remark;
        public string Remark
        {
            get
            {
                return this._remark;
            }
            set
            {
                this._remark = value;
            }
        }

    }
    #endregion
}
