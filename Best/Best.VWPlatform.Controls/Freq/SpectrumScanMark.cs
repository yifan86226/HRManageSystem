using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Best.VWPlatform.Controls.Freq
{
    [TemplatePart(Name = "xScanMark", Type = typeof(Grid))]
    public sealed class SpectrumScanMark : Control
    {
        #region 变量
        public static readonly DependencyProperty TranslateXProperty = DependencyProperty.Register("TranslateX", typeof(double), typeof(SpectrumScanMark), null);

        public static readonly DependencyProperty TranslateYProperty = DependencyProperty.Register("TranslateY", typeof(double), typeof(SpectrumScanMark), null);

        public static readonly DependencyProperty MarkFreValueProperty = DependencyProperty.Register("MarkFreValue", typeof(double), typeof(SpectrumScanMark), null);

        public static readonly DependencyProperty MarkFreValueConverterProperty = DependencyProperty.Register("MarkFreValueConverter", typeof(IValueConverter), typeof(SpectrumScanMark), null);

        public static readonly DependencyProperty MarkOffsetProperty = DependencyProperty.Register("MarkOffset", typeof(double), typeof(SpectrumScanMark), null);

        public static readonly DependencyProperty MarkVisibilityProperty = DependencyProperty.Register("MarkVisibility", typeof(Visibility), typeof(SpectrumScanMark), new PropertyMetadata(Visibility.Visible));

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(SolidColorBrush), typeof(SpectrumScanMark), new PropertyMetadata(new SolidColorBrush(Colors.White)));

        private bool _isMouseCaptured = false;
        private Grid _xScanMark;
        private Grid _xScanMarkFreq;
        private TextBlock _xFreqMarkValue;
        #endregion

        #region 构造函数

        static SpectrumScanMark()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SpectrumScanMark), new FrameworkPropertyMetadata(typeof(SpectrumScanMark)));
        }

        public SpectrumScanMark()
        {
            Id = Guid.NewGuid().ToString();
            AttachEvents();
        }
        #endregion

        #region 内部方法
        private void AttachEvents()
        {
            SizeChanged += SpectrumMarkSizeChanged;
        }

        void SpectrumMarkSizeChanged(object sender, SizeChangedEventArgs e)
        {
            double ws = e.NewSize.Width / 2;
            Margin = new Thickness(-ws, 0, 0, 0);
        }

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _xScanMark = GetTemplateChild("xScanMark") as Grid;
            _xScanMarkFreq = GetTemplateChild("xScanMarkFreq") as Grid;
            _xFreqMarkValue = GetTemplateChild("xMarkFreqValue") as TextBlock;

            SetMarkTransform();
        }

        /// <summary>
        /// 绑定mark上的值和mark的位置
        /// </summary>
        private void SetMarkTransform()
        {
            var MarktextBind = new Binding("MarkFreValue") { StringFormat = "0.000MHz", Converter = MarkFreValueConverter, Source = this };

            var transform = new TranslateTransform();
            var ctBinding = new Binding("TranslateX") { Source = this };
            BindingOperations.SetBinding(transform, TranslateTransform.XProperty, ctBinding);
            RenderTransform = transform;

            _xFreqMarkValue.SetBinding(TextBlock.TextProperty, MarktextBind);
        }

        /// <summary>
        /// 获取Mark唯一标识
        /// </summary>
        public string Id { get; set; }

        public SolidColorBrush Color
        {
            get { return (SolidColorBrush)GetValue(ColorProperty); }
            set
            {
                SetValue(ColorProperty, value);
            }
        }

        /// <summary>
        /// 获取或设置 Mark 的值
        /// </summary>
        public double MarkFreValue
        {
            get
            {
                return (double)GetValue(MarkFreValueProperty);
            }
            set
            {
                SetValue(MarkFreValueProperty, value);
            }
        }

        /// <summary>
        /// Mark值转换
        /// </summary>
        public IValueConverter MarkFreValueConverter
        {
            get { return (IValueConverter)GetValue(MarkFreValueConverterProperty); }
            set { SetValue(MarkFreValueConverterProperty, value); }
        }

        /// <summary>
        /// Mark中心点 X偏移
        /// </summary>
        public double TranslateX
        {
            get
            {
                return (double)GetValue(TranslateXProperty);
            }
            set
            {
                SetValue(TranslateXProperty, value);
            }
        }

        public double TranslateY
        {
            get { return (double)GetValue(TranslateYProperty); }
            set
            {
                SetValue(TranslateYProperty, value);
            }
        }

        /// <summary>
        /// 浮标偏移量
        /// </summary>
        public double MarkOffset
        {
            get { return (double)GetValue(MarkOffsetProperty); }
            set
            {
                if (MarkVisibility == Visibility.Visible)
                    SetValue(MarkOffsetProperty, value);
            }
        }

        /// <summary>
        /// 获取或设置是否显Mark上的浮标
        /// </summary>
        public Visibility MarkVisibility
        {
            get { return (Visibility)GetValue(MarkVisibilityProperty); }
            set { SetValue(MarkVisibilityProperty, value); }
        }

        /// <summary>
        /// Mark上方显示的频率显示方框是否可见
        /// </summary>
        public Visibility FreqVisibility { get; set; }
    }
}
