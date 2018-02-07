using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Best.VWPlatform.Controls.MessageTip
{
    public class BackgroundPanelEx : ContentControl, IWaitIndicatorEx
    {
        static BackgroundPanelEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BackgroundPanelEx), new FrameworkPropertyMetadata(typeof(BackgroundPanelEx)));
        }

        private Grid _waitIndicator;
        private Storyboard _storyboard;

        public static readonly DependencyProperty WaitMessageProperty = DependencyProperty.Register("WaitMessage",
                                                                                           typeof(string),
                                                                                           typeof(BackgroundPanelEx),
                                                                                           null);

        public static readonly DependencyProperty WaitMessageTemplateProperty =
            DependencyProperty.Register("WaitMessageTemplate",
                                        typeof(ControlTemplate),
                                        typeof(BackgroundPanelEx), null);

        public static readonly DependencyProperty CanCancelWaitMessageProperty = DependencyProperty.Register("CanCancelWaitMessage",
                                                                                                  typeof(bool),
                                                                                                  typeof(BackgroundPanelEx), null);
        public BackgroundPanelEx()
        {
            CancelWaitMessageCommand = new CancelWaitMessageCommand(this);
            CanCancelWaitMessage = false;
        }

        public string WaitMessage
        {
            get { return (string)GetValue(WaitMessageProperty); }
            set
            {
                bool isShow = !string.IsNullOrWhiteSpace(value);
                if (isShow)
                    SetValue(WaitMessageProperty, value);
                ShowWaitMessage(isShow);
            }
        }

        public ControlTemplate WaitMessageTemplate
        {
            get { return (ControlTemplate)GetValue(WaitMessageTemplateProperty); }
            set
            {
                SetValue(WaitMessageTemplateProperty, value);
            }
        }
        /// <summary>
        /// 是否可以取消提示进度
        /// </summary>
        public bool CanCancelWaitMessage
        {
            get { return (bool)GetValue(CanCancelWaitMessageProperty); }
            set { SetValue(CanCancelWaitMessageProperty, value); }
        }

        public ICommand CancelWaitMessageCommand { get; set; }

        private void ShowWaitMessage(bool pShow = true)
        {
            if (_waitIndicator != null && _storyboard != null)
            {
                _waitIndicator.Visibility = pShow ? Visibility.Visible : Visibility.Collapsed;

                if (pShow)
                    _storyboard.Begin();
                else
                    _storyboard.Stop();
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _waitIndicator = GetTemplateChild("xWaitIndicator") as Grid;
            var layoutRoot = GetTemplateChild("LayoutRoot") as Grid;
            if (layoutRoot != null)
            {
                _storyboard = layoutRoot.Resources["Storyboard1"] as Storyboard;
            }

            ShowWaitMessage(!string.IsNullOrWhiteSpace(WaitMessage));
        }

        internal void OnCancelWaitMessage()
        {
            _waitIndicator.Visibility = Visibility.Collapsed;
            ShowWaitMessage(false);
            if (CancelWaitMessage != null)
            {
                CancelWaitMessage();
            }
        }

        /// <summary>
        /// 点击取消按钮后触发事件
        /// </summary>
        public event Action CancelWaitMessage;
    }

    /// <summary>
    /// 进度提示接口
    /// </summary>
    public interface IWaitIndicatorEx
    {
        /// <summary>
        /// 获取或设置等待信息提示内容
        /// </summary>
        string WaitMessage { get; set; }
        /// <summary>
        /// 是否支持取消等待，（增加取消按钮）
        /// </summary>
        bool CanCancelWaitMessage { get; set; }
        /// <summary>
        /// 进度提示取消事件
        /// </summary>
        event Action CancelWaitMessage;
    }

    public class CancelWaitMessageCommand : ICommand
    {
        private readonly BackgroundPanelEx _backgroundPanelEx;
        public CancelWaitMessageCommand(BackgroundPanelEx pBackgroundPanelEx)
        {
            _backgroundPanelEx = pBackgroundPanelEx;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (_backgroundPanelEx == null)
                return;
            _backgroundPanelEx.OnCancelWaitMessage();
        }
    }
}
