using Best.VWPlatform.Common.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace Best.VWPlatform.Common.Types
{
    /// <summary>
    /// 站点基类
    /// </summary>
    public abstract class Station : Control, INotifyPropertyChanged, ITimer
    {
        #region 变量
        public static readonly DependencyProperty StationImageProperty = DependencyProperty.Register("StationImage", typeof(Brush), typeof(Station), null);
        protected static object LockObj = new object();
        private static readonly Dictionary<string, ImageBrush> StationImageCache = new Dictionary<string, ImageBrush>();

        private StationInfo _stationInfo;
        private double _scale = 0.9;
        private bool _selected;
        private bool _isShowName;
        private bool _isShowFactory;
        private double _translateY;
        private double _panelTranslateY;
        private const double TRANSLATE_Y = -14;
        private const double PANEL_TRANSLATE_Y = -18;
        #endregion

        private readonly string _iconNamePrefix;

        private Dictionary<string, string> _stationImage;

        #region 构造函数

        protected Station(StationInfo pStationInfo)
        {
            StationInfo = pStationInfo;
            _iconNamePrefix = string.Format("pack://application:,,,/Best.VWPlatform.Resources;component/Images/Map/{{0}}{0}_{1}_{{1}}.png", 5, 0);
            Attributes = new Dictionary<string, object>();

            if(_stationImage == null)
            {
                StationCommSystemXmlRead();
            }           
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取或设置站图片
        /// </summary>
        public ImageBrush StationImage
        {
            get { return (ImageBrush)GetValue(StationImageProperty); }
            set { SetValue(StationImageProperty, value); }
        }
        /// <summary>
        /// 获取站图标名称前缀格式
        /// {0}{1}_{2}_{3}.png ：{0} - 表示站类型, {1} - 站类型，{2} - 平台类型，{3} - 图片索引
        /// </summary>
        protected string IconNamePrefix
        {
            get { return _iconNamePrefix; }
        }
        /// <summary>
        /// 获取或设置站信息
        /// </summary>
        public StationInfo StationInfo
        {
            get
            {
                return _stationInfo;
            }
            set
            {
                _stationInfo = value;
                OnPropertyChanged("StationInfo");
            }
        }
        /// <summary>
        /// 选择状态
        /// </summary>
        public bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                OnPropertyChanged("Selected");
            }
        }
        /// <summary>
        /// 缩放值
        /// </summary>
        public double Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                OnPropertyChanged("Scale");
                TranslateY = TRANSLATE_Y * Scale;
                PanelTranslateY = TranslateY + PANEL_TRANSLATE_Y * Scale;
            }
        }

        /// <summary>
        /// 站Y轴偏移值
        /// </summary>
        public double TranslateY
        {
            get { return _translateY; }
            set
            {
                _translateY = value;
                OnPropertyChanged("TranslateY");
            }
        }

        /// <summary>
        /// 面板Y轴偏移值
        /// </summary>
        public double PanelTranslateY
        {
            get { return _panelTranslateY; }
            set
            {
                _panelTranslateY = value;
                OnPropertyChanged("PanelTranslateY");
            }
        }

        /// <summary>
        /// 显示名称标识
        /// </summary>
        public bool IsShowName
        {
            get { return _isShowName; }
            set
            {
                _isShowName = value;
                OnPropertyChanged("IsShowName");
            }
        }
        /// <summary>
        /// 显示集成厂商标识
        /// </summary>
        public bool IsShowFactory
        {
            get { return _isShowFactory; }
            set
            {
                _isShowFactory = value;
                OnPropertyChanged("IsShowFactory");
            }
        }
        /// <summary>
        /// 获取站控件模版
        /// </summary>
        public virtual ControlTemplate ControlTemplate
        {
            get { return null; }
        }
        /// <summary>
        /// 获取或设置开启 Tick 的时间周期的调用
        /// </summary>
        protected bool PlayTick { get; set; }
        /// <summary>
        /// 获取属性表
        /// </summary>
        public Dictionary<string, object> Attributes { get; private set; }
        #endregion

        #region 方法
        protected string GetImageKey(string pImageKey)
        {
            return string.Format(IconNamePrefix, "S", pImageKey);
        }
        /// <summary>
        /// 添加图片到缓存中
        /// </summary>
        /// <param name="pImageKey">图片索引</param>
        /// <param name="pStationTag">站标志</param>
        /// <returns></returns>
        protected ImageBrush AddImage(string pImageKey)
        {
            string key = GetImageKey(pImageKey); 
            if (StationImageCache.ContainsKey(key))
                return StationImageCache[key];
            var stationBrush = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(key, UriKind.RelativeOrAbsolute))
            };
            StationImageCache[key] = stationBrush;
            return stationBrush;
        }

        /// <summary>
        /// 获取缓存中的图片
        /// </summary>
        /// <param name="pImageKey">图片索引</param>
        /// <param name="pGetFailedOutput">获取图片失败，在右侧输出窗口输出失败原因，默认：true</param>
        /// <returns></returns>
        protected ImageBrush GetImage(string pImageKey, bool pGetFailedOutput = true)
        {
            string key = GetImageKey(pImageKey); 
            if (StationImageCache.ContainsKey(key))
                return StationImageCache[key];
            if (pGetFailedOutput)
            {
                Debug.WriteLine("目前没有该类型站点的“图标”！", Colors.Red);
            }
            return null;
        }
        /// <summary>
        /// 用于解析台站图片文件，根据台站通信系统查找对应显示图片ID
        /// </summary>
        private void StationCommSystemXmlRead()
        {
            _stationImage = new Dictionary<string, string>();
            // 创建一个XmlDocument类的对象 
            XDocument doc = new XDocument();
            doc = XDocument.Load(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "/ParamConfig/" + "StationImage.xml");
            string strpara = doc.ToString();
            var xml = XDocument.Parse(strpara);
            var xmlElement = xml.Root;
            if (xmlElement.HasElements)
            {
                foreach (var e in xmlElement.Elements())
                {
                    if (!e.HasElements)
                        continue;
                    if (e.Name.LocalName == "Images")
                    {
                        var id = GetPropertyValue(e, "ImageID");
                        foreach (var item in e.Elements())
                        {
                            if (!e.HasElements)
                                continue;
                            if (item.Name.LocalName == "CommSystem")
                            {
                                if (!_stationImage.ContainsKey(item.Value))
                                {
                                    _stationImage[item.Value] = id;
                                }
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 根据通信系统获取图片对应id
        /// </summary>
        /// <returns></returns>
        protected string GetStationImageID(string commsystem)
        {
            if (commsystem == null)
            {
                return "0";
            }

            if (_stationImage.ContainsKey(commsystem))
            {
                return _stationImage[commsystem];
            }

            return "0";
        }

        private string GetPropertyValue(XElement pElement, string pPropertyName)
        {
            return pElement.Attribute(pPropertyName) == null ? string.Empty : pElement.Attribute(pPropertyName).Value.ToLower();
        }

        /// <summary>
        /// 刷新站状态
        /// </summary>
        /// <param name="pStationInfo"></param>
        public abstract void UpdateState(StationInfo pStationInfo);
        #endregion

        #region INotifyPropertyChanged
        protected void OnPropertyChanged(string pPropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(pPropertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        public virtual void Tick(TimeSpan pTime)
        {

        }

        #region events
        public event MouseEventHandler StationMouseEnter;
        public event MouseEventHandler StationMouseLeave;
        public event MouseButtonEventHandler StationClick;
        public event MouseButtonEventHandler StationRightClick;

        protected virtual void OnStationMouseEnter(MouseEventArgs e)
        {
            MouseEventHandler handler = StationMouseEnter;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnStationMouseLeave(MouseEventArgs e)
        {
            MouseEventHandler handler = StationMouseLeave;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnStationClick(MouseButtonEventArgs e)
        {
            MouseButtonEventHandler handler = StationClick;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnStationRightClick(MouseButtonEventArgs e)
        {
            MouseButtonEventHandler handler = StationRightClick;
            if (handler != null) handler(this, e);
        }

        #endregion
    }
}
