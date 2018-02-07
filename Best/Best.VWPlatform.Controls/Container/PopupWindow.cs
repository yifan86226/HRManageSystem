using Best.VWPlatform.Controls.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Best.VWPlatform.Controls.Container
{
    public class PopupWindow : ContentControl
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(object), typeof(PopupWindow), null);
        public static readonly DependencyProperty LittleTitleProperty = DependencyProperty.Register("LittleTitle", typeof(string), typeof(PopupWindow), null);

        private ContentPresenter _content;
        private Popup _popup;
        private FrameworkElement _chrome;
        private Button _closeButton;
        private FrameworkElement _contentRoot;
        private readonly Border _background = new Border();
        private readonly Canvas _backCanvas = new Canvas();

        public event Action Closed;

        public PopupWindow()
        {
            DefaultStyleKey = typeof(PopupWindow);
            _background.Background = new SolidColorBrush(Colors.Transparent);
            _background.MouseLeftButtonUp += OnCloseButtonClick;
            _backCanvas.HorizontalAlignment = HorizontalAlignment.Left;
            _backCanvas.VerticalAlignment = VerticalAlignment.Top;
            _background.Child = _backCanvas;
            MouseLeftButtonUp += OnPopupWindowMouseLeftButtonUp;
        }

        public PopupWindow(string pTitle, string pLittleTitle, UserControl pWin)
            : this()
        {
            Title = pTitle;
            LittleTitle = pLittleTitle;
            Content = pWin;
        }

        #region 公有方法
        public void Show(double pLeft, double pTop)
        {
            if (_popup == null)
            {
                _popup = new Popup();
                _popup.Child = _background;
                _popup.Placement = PlacementMode.Center;
                _popup.HorizontalAlignment = HorizontalAlignment.Left;
                _popup.VerticalAlignment = VerticalAlignment.Top;
                _popup.AllowsTransparency = true;
                var rootVisual = Application.Current.MainWindow as FrameworkElement;
                rootVisual.SizeChanged += OnSizeChanged;
                _background.Width = rootVisual.ActualWidth;
                _background.Height = rootVisual.ActualHeight;
            }
            _backCanvas.Children.Clear();
            _backCanvas.Children.Add(this);
            Canvas.SetLeft(this, pLeft);
            Canvas.SetTop(this, pTop);
            _popup.IsOpen = true;
        }

        public void Close()
        {
            ((FrameworkElement)Application.Current.MainWindow).SizeChanged -= OnSizeChanged;
            if (_popup != null)
                _popup.IsOpen = false;
            if (Closed != null)
                Closed();
        }
        #endregion

        #region 属性
        public object Title
        {
            get
            {
                return GetValue(TitleProperty);
            }
            set
            {
                SetValue(TitleProperty, value);
            }
        }
        public string LittleTitle
        {
            get
            {
                return (string)GetValue(LittleTitleProperty);
            }
            set
            {
                SetValue(LittleTitleProperty, value);
            }
        }
        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_closeButton != null)
                _closeButton.Click -= OnCloseButtonClick;

            _contentRoot = GetTemplateChild("ContentRoot") as FrameworkElement;
            _content = GetTemplateChild("ContentPresenter") as ContentPresenter;
            _closeButton = GetTemplateChild("CloseButton") as Button;
            _chrome = GetTemplateChild("Chrome") as FrameworkElement;

            if (_content != null) _content.Content = Content;
            if (_closeButton != null) _closeButton.Click += OnCloseButtonClick;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _background.Width = e.NewSize.Width;
            _background.Height = e.NewSize.Height;
        }

        private void OnPopupWindowMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
