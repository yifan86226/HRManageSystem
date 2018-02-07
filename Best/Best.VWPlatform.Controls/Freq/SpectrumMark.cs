using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Best.VWPlatform.Controls.Freq
{
    /// <summary>
    /// 频谱容器上的 Mark 控件
    /// </summary>
    [TemplatePart(Name = "xMarkLeft", Type = typeof(Grid))]
    [TemplatePart(Name = "xMarkBottom", Type = typeof(Grid))]
    public sealed class SpectrumMark : Control, INotifyPropertyChanged
    {
        #region 变量
        public static readonly DependencyProperty TranslateXProperty = DependencyProperty.Register("TranslateX", typeof(double), typeof(SpectrumMark), null);

        public static readonly DependencyProperty TranslateYProperty = DependencyProperty.Register("TranslateY", typeof(double), typeof(SpectrumMark), null);

        public static readonly DependencyProperty MarkValueProperty = DependencyProperty.Register("MarkValue", typeof(double), typeof(SpectrumMark), null);

        public static readonly DependencyProperty MarkValueConverterProperty = DependencyProperty.Register("MarkValueConverter", typeof(IValueConverter), typeof(SpectrumMark), null);

        public static readonly DependencyProperty BuoyOffsetProperty = DependencyProperty.Register("BuoyOffset", typeof(double), typeof(SpectrumMark), null);

        public static readonly DependencyProperty BuoyVisibilityProperty = DependencyProperty.Register("BuoyVisibility", typeof(Visibility), typeof(SpectrumMark), new PropertyMetadata(Visibility.Visible));

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(SolidColorBrush), typeof(SpectrumMark), new PropertyMetadata(new SolidColorBrush(Colors.White)));

        private bool _isMouseCaptured = false;
        private Grid _xMarkLeft;
        private Grid _xMarkBottom;
        private TextBlock _xButtomMarkText;
        private TextBox _xBottomMarkTextHide;
        private TextBlock _xLeftMarkText;
        private Visibility _markArrowVisibility;
        private Rectangle _xHorizontalLine;
        private Rectangle _xVerticalLine;
        private Grid _xMarkTooltip;
        private TextBlock _xTooltipMarkValue;
        #endregion

        #region 构造函数

        static SpectrumMark()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SpectrumMark), new FrameworkPropertyMetadata(typeof(SpectrumMark)));
        }

        public SpectrumMark()
        {
            Id = Guid.NewGuid().ToString();
            AttachEvents();
            TooltipVisibility = Visibility.Collapsed;
            MarkArrowVisibility = Visibility.Collapsed;
            VerticalLineVisibility = Visibility.Collapsed;
            HorizontalLineVisibility = Visibility.Collapsed;
        }
        #endregion

        #region 内部方法
        private void AttachEvents()
        {
            SizeChanged += SpectrumMarkSizeChanged;
        }

        void SpectrumMarkSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Direction == MarkDirection.Bottom)
            {
                double ws = e.NewSize.Width / 2;
                Margin = new Thickness(-ws, 0, 0, 0);
            }
            else
            {
                double tp = e.NewSize.Height / 2;
            }
        }

        /// <summary>
        /// 确定mark类型
        /// </summary>
        private void ChangeMarkDirection()
        {
            _xMarkLeft.Visibility = Direction == MarkDirection.Left ? Visibility.Visible : Visibility.Collapsed;
            _xMarkBottom.Visibility = Direction == MarkDirection.Bottom ? Visibility.Visible : Visibility.Collapsed;
        }
        #endregion

        #region 重写
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _xMarkLeft = GetTemplateChild("xMarkLeft") as Grid;
            _xMarkBottom = GetTemplateChild("xMarkBottom") as Grid;
            _xButtomMarkText = GetTemplateChild("xButtomMarkText") as TextBlock;
            _xLeftMarkText = GetTemplateChild("xLeftMarkText") as TextBlock;
            _xBottomMarkTextHide = GetTemplateChild("xBottomMarkTextHide") as TextBox;
            _xHorizontalLine = GetTemplateChild("xHorizontalLine") as Rectangle;
            _xVerticalLine = GetTemplateChild("xVerticalLine") as Rectangle;
            _xMarkTooltip = GetTemplateChild("xMarkTooltip") as Grid;
            _xTooltipMarkValue = GetTemplateChild("xTooltipMarkValue") as TextBlock;

            ChangeMarkDirection();

            if (_xBottomMarkTextHide != null)
            {
                _xBottomMarkTextHide.KeyDown += OnBottomMarkTextKeyDown;
            }

            SetMarkTransform();
            //SetMarkClip();
            //SetMarkArrowClickEvent();
            SetMarkMouseEvents();
        }

        /// <summary>
        /// 添加鼠标滑入时显示
        /// </summary>
        private void SetMarkMouseEvents()
        {
            var hMark = GetTemplateChild("xMark") as Grid;
            if (hMark != null)
            {
                hMark.MouseEnter += (sender, e) =>
                {
                    _xHorizontalLine.Visibility = Visibility.Visible;
                };
                hMark.MouseLeave += (sender, e) =>
                {
                    if (HorizontalLineVisibility == Visibility.Collapsed)
                        _xHorizontalLine.Visibility = Visibility.Collapsed;
                };
            }

            var hand = GetTemplateChild("xHand") as Grid;
            if (hand == null)
                return;
            hand.MouseEnter += (sender, e) =>
            {
                _xVerticalLine.Visibility = Visibility.Visible;
                _xMarkTooltip.Visibility = Visibility.Visible;
            };
            hand.MouseLeave += (sender, e) =>
            {
                if (VerticalLineVisibility == Visibility.Collapsed)
                    _xVerticalLine.Visibility = Visibility.Collapsed;
                if (TooltipVisibility == Visibility.Collapsed)
                    _xMarkTooltip.Visibility = Visibility.Collapsed;
            };
        }

        /// <summary>
        /// 左右箭头
        /// </summary>
        private void SetMarkArrowClickEvent()
        {
            var leftArrow = GetTemplateChild("xLeftArrow") as Border;
            if (leftArrow != null)
            {
                leftArrow.MouseLeftButtonDown += (sender, e) =>
                {
                    e.Handled = true;
                    OnMarkArrowButtonClick(this, true);
                };
            }
            var rightArrow = GetTemplateChild("xRightArrow") as Border;
            if (rightArrow != null)
            {
                rightArrow.MouseLeftButtonDown += (sender, e) =>
                {
                    e.Handled = true;
                    OnMarkArrowButtonClick(this, false);
                };
            }
        }

        /// <summary>
        /// @?
        /// </summary>
        private void SetMarkClip()
        {
            var markBottomChild = GetTemplateChild("xMarkBottomChild") as Grid;
            if (markBottomChild != null)
            {
                markBottomChild.SizeChanged +=
                    (sender, e) =>
                    {
                        markBottomChild.Clip = new RectangleGeometry { Rect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height) };
                    };
            }
        }

        /// <summary>
        /// 绑定mark上的值和mark的位置
        /// </summary>
        private void SetMarkTransform()
        {
            var textBind = new Binding("MarkValue") {StringFormat = "0", Converter = MarkValueConverter, Source = this };
            var MarktextBind = new Binding("MarkValue") { StringFormat = "0.000MHz", Converter = MarkValueConverter, Source = this };
            if (Direction == MarkDirection.Bottom)
            {
                _xButtomMarkText.SetBinding(TextBlock.TextProperty, textBind);
                var transform = new TranslateTransform();
                var ctBinding = new Binding("TranslateX") { Source = this };
                BindingOperations.SetBinding(transform, TranslateTransform.XProperty, ctBinding);
                RenderTransform = transform;

                _xTooltipMarkValue.SetBinding(TextBlock.TextProperty, MarktextBind);
            }
            else
            {
                _xLeftMarkText.SetBinding(TextBlock.TextProperty, textBind);
                var transform = new TranslateTransform();
                var ctBinding = new Binding("TranslateY") { Source = this };
                BindingOperations.SetBinding(transform, TranslateTransform.YProperty, ctBinding);
                RenderTransform = transform;
            }
        }

        private void OnBottomMarkTextKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Left || e.Key == Key.Right) && MarkMoveChanged != null)
            {
                MarkMoveChanged(this, e.Key == Key.Left);
            }
        }

        private Point _startHitPoint;
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            _xBottomMarkTextHide.Focus();
            _isMouseCaptured = true;
            BuoyVisibility = Visibility.Collapsed;

            var p = Parent as FrameworkElement;
            if (p == null)
                return;
            _startHitPoint = e.GetPosition(p);
            
            if (MouseLeftButtonDown != null)
                MouseLeftButtonDown(this, e); 
            
            CaptureMouse();
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            _isMouseCaptured = false;
            ReleaseMouseCapture();
            BuoyVisibility = Visibility.Visible;
            if (MouseLeftButtonUp != null)
                MouseLeftButtonUp(this, e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!_isMouseCaptured)
                return;
            var p = Parent as FrameworkElement;
            if (p == null)
                return;

            Point pt = e.GetPosition(p);
            if (Direction == MarkDirection.Bottom)
            {
                if (pt.X < 0 || pt.X > p.ActualWidth + 1)
                    return;
                TranslateX += pt.X - _startHitPoint.X;
            }
            else
            {
                if (pt.Y < 0 || pt.Y > p.ActualHeight + 1)
                    return;
                TranslateY += pt.Y - _startHitPoint.Y;
            }

            _startHitPoint = pt;

            if (MouseMove != null)
                MouseMove(this, e);
        }
        #endregion

        #region 属性

        /// <summary>
        /// 获取Mark唯一标识
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// 获取或设置Mark组名称
        /// </summary>
        public string GroupName { get; set; }

        public SolidColorBrush Color
        {
            get { return (SolidColorBrush)GetValue(ColorProperty); }
            set
            {
                SetValue(ColorProperty, value);
                Foreground = value.Color.Equals(Colors.White)
                                 ? new SolidColorBrush(Colors.Black)
                                 : new SolidColorBrush(Colors.White);
            }
        }

        /// <summary>
        /// 获取或设置 Mark 的值
        /// </summary>
        public double MarkValue
        {
            get
            {
                return (double)GetValue(MarkValueProperty);
            }
            set
            {
                SetValue(MarkValueProperty, value);
                if (MarkValueChanged != null)
                    MarkValueChanged(value);
            }
        }

        /// <summary>
        /// Mark值转换
        /// </summary>
        public IValueConverter MarkValueConverter
        {
            get { return (IValueConverter)GetValue(MarkValueConverterProperty); }
            set { SetValue(MarkValueConverterProperty, value); }
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

                if (TranslateXChanged != null)
                    TranslateXChanged(value);
            }
        }

        public double TranslateY
        {
            get { return (double)GetValue(TranslateYProperty); }
            set
            {
                SetValue(TranslateYProperty, value);

                if (TranslateYChanged != null)
                    TranslateYChanged(value);
            }
        }
        
        /// <summary>
        /// Mark显示位置
        /// </summary>
        public MarkDirection Direction { get; set; }
        
        /// <summary>
        /// 浮标偏移量
        /// </summary>
        public double BuoyOffset
        {
            get { return (double)GetValue(BuoyOffsetProperty); }
            set
            {
                if (BuoyVisibility == Visibility.Visible)
                    SetValue(BuoyOffsetProperty, value);
            }
        }
        
        /// <summary>
        /// 获取或设置是否显Mark上的浮标
        /// </summary>
        public Visibility BuoyVisibility
        {
            get { return (Visibility)GetValue(BuoyVisibilityProperty); }
            set { SetValue(BuoyVisibilityProperty, value); }
        }
        
        /// <summary>
        /// Mark横线是否可见
        /// </summary>
        public Visibility HorizontalLineVisibility { get; set; }
        
        /// <summary>
        /// Mark竖线是否可见
        /// </summary>
        public Visibility VerticalLineVisibility { get; set; }
        
        /// <summary>
        /// Mark上方显示的提示方框是否可见
        /// </summary>
        public Visibility TooltipVisibility { get; set; }
        
        /// <summary>
        /// Mark上的浮标是否可见
        /// </summary>
        public Visibility MarkArrowVisibility
        {
            get { return _markArrowVisibility; }
            set
            {
                _markArrowVisibility = value;
                OnPropertyChanged("MarkArrowVisibility");
            }
        }

        #endregion

        #region 事件
        public new event MouseButtonEventHandler MouseLeftButtonDown;
        public new event MouseEventHandler MouseMove;
        public new event MouseButtonEventHandler MouseLeftButtonUp;
        public event Action<double> MarkValueChanged;
        public event Action<double> TranslateXChanged;
        public event Action<double> TranslateYChanged;
        
        /// <summary>
        ///  Mark左右键盘移动事件
        /// </summary>
        /// <param name="object">Mark对象</param>
        /// <param name="bool">true - 左移动，flase - 右移动</param>
        public event Action<object, bool> MarkMoveChanged;
        
        /// <summary>
        ///  Mark左右箭头按钮单击事件
        /// </summary>
        /// <param name="object">Mark对象</param>
        /// <param name="bool">true - 左箭头，flase - 右箭头</param>
        public event Action<object, bool> MarkArrowButtonClick;

        public void OnMarkArrowButtonClick(object arg1, bool arg2)
        {
            Action<object, bool> handler = MarkArrowButtonClick;
            if (handler != null) handler(arg1, arg2);
        }

        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string pPropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(pPropertyName));
        }
        #endregion
    }

    /// <summary>
    /// Mark显示位置
    /// </summary>
    public enum MarkDirection
    {
        Bottom,
        Left
    }
}
