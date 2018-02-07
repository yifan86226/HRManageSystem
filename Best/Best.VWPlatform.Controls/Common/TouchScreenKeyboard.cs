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
using System.Windows.Input.Test;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using DevExpress.Xpf.Core.HandleDecorator;

namespace Best.VWPlatform.Controls.Common
{
    public class TouchScreenKeyboard : Window
    {
        static TouchScreenKeyboard()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TouchScreenKeyboard), new FrameworkPropertyMetadata(typeof(TouchScreenKeyboard)));
            
            SetCommandBinding();
        }

        private static Window _InstanceObject;
        private static Control _CurrentControl;

        public static RoutedUICommand Cmd1 = new RoutedUICommand();
        public static RoutedUICommand Cmd2 = new RoutedUICommand();
        public static RoutedUICommand Cmd3 = new RoutedUICommand();
        public static RoutedUICommand Cmd4 = new RoutedUICommand();
        public static RoutedUICommand Cmd5 = new RoutedUICommand();
        public static RoutedUICommand Cmd6 = new RoutedUICommand();
        public static RoutedUICommand Cmd7 = new RoutedUICommand();
        public static RoutedUICommand Cmd8 = new RoutedUICommand();
        public static RoutedUICommand Cmd9 = new RoutedUICommand();
        public static RoutedUICommand Cmd0 = new RoutedUICommand();
        public static RoutedUICommand CmdPoint = new RoutedUICommand();
        public static RoutedUICommand CmdBackspace = new RoutedUICommand();
        public static RoutedUICommand CmdEnter = new RoutedUICommand();
        public static RoutedUICommand CmdSpaceBar = new RoutedUICommand();
        public static RoutedUICommand CmdClear = new RoutedUICommand();
        public static RoutedUICommand CmdClose = new RoutedUICommand();
        public static RoutedUICommand CmdMinus = new RoutedUICommand();

        private static void SetCommandBinding()
        {
            CommandBinding Cb1 = new CommandBinding(Cmd1, RunCommand);
            CommandBinding Cb2 = new CommandBinding(Cmd2, RunCommand);
            CommandBinding Cb3 = new CommandBinding(Cmd3, RunCommand);
            CommandBinding Cb4 = new CommandBinding(Cmd4, RunCommand);
            CommandBinding Cb5 = new CommandBinding(Cmd5, RunCommand);
            CommandBinding Cb6 = new CommandBinding(Cmd6, RunCommand);
            CommandBinding Cb7 = new CommandBinding(Cmd7, RunCommand);
            CommandBinding Cb8 = new CommandBinding(Cmd8, RunCommand);
            CommandBinding Cb9 = new CommandBinding(Cmd9, RunCommand);
            CommandBinding Cb0 = new CommandBinding(Cmd0, RunCommand);
            CommandBinding CbPoint = new CommandBinding(CmdPoint, RunCommand);
            CommandBinding CbBackspace = new CommandBinding(CmdBackspace, RunCommand);
            CommandBinding CbEnter = new CommandBinding(CmdEnter, RunCommand);
            CommandBinding CbSpaceBar = new CommandBinding(CmdSpaceBar, RunCommand);
            CommandBinding CbClear = new CommandBinding(CmdClear, RunCommand);
            CommandBinding CbClose = new CommandBinding(CmdClose, RunCommand);
            CommandBinding CbMinus = new CommandBinding(CmdMinus, RunCommand);

            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), Cb1);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), Cb2);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), Cb3);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), Cb4);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), Cb5);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), Cb6);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), Cb7);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), Cb8);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), Cb9);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), Cb0);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), CbPoint);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), CbBackspace);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), CbEnter);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), CbSpaceBar);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), CbClear);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), CbClose);
            CommandManager.RegisterClassCommandBinding(typeof (TouchScreenKeyboard), CbMinus);
        }

        static void SendToUIThread(UIElement element, string text)
        {
            element.Dispatcher.BeginInvoke(new Action(() =>
            {
                SendKeys.Send(element, text);
            }), DispatcherPriority.Input);
        }

        static void RunCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == Cmd1)
            {
                SendToUIThread(_CurrentControl, "1");
            }
            else if (e.Command == Cmd2)
            {
                SendToUIThread(_CurrentControl, "2");
            }
            else if (e.Command == Cmd3)
            {
                SendToUIThread(_CurrentControl, "3");
            }
            else if (e.Command == Cmd4)
            {
                SendToUIThread(_CurrentControl, "4");
            }
            else if (e.Command == Cmd5)
            {
                SendToUIThread(_CurrentControl, "5");
            }
            else if (e.Command == Cmd6)
            {
                SendToUIThread(_CurrentControl, "6");
            }
            else if (e.Command == Cmd7)
            {
                SendToUIThread(_CurrentControl, "7");
            }
            else if (e.Command == Cmd8)
            {
                SendToUIThread(_CurrentControl, "8");
            }
            else if (e.Command == Cmd9)
            {
                SendToUIThread(_CurrentControl, "9");
            }
            else if (e.Command == Cmd0)
            {
                SendToUIThread(_CurrentControl, "0");
            }
            else if (e.Command == CmdBackspace)
            {
                SendToUIThread(_CurrentControl, "{BS}");
            }
            else if (e.Command == CmdEnter)
            {
                SendToUIThread(_CurrentControl, "{ENTER}");
            }
            else if (e.Command == CmdPoint)
            {
                SendToUIThread(_CurrentControl, ".");
            }
            else if (e.Command == CmdClear)
            {
                ((TextBox)_CurrentControl).Clear();
            }
            else if (e.Command == CmdClose)
            {
                if (_InstanceObject != null)
                {
                    _InstanceObject.Visibility = System.Windows.Visibility.Hidden;
                }
            }
            else if (e.Command == CmdMinus)
            {
                SendToUIThread(_CurrentControl, "-");
            }
        }

        public static bool GetIsShowKeyboard(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsShowKeyboardProperty);
        }

        public static void SetIsShowKeyboard(DependencyObject obj, bool value)
        {
            obj.SetValue(IsShowKeyboardProperty, value);
        }

        public static readonly DependencyProperty IsShowKeyboardProperty = DependencyProperty.RegisterAttached(
            "IsShowKeyboard", typeof(bool), typeof(TouchScreenKeyboard), new UIPropertyMetadata(default(bool), IsShowKeyboardPropertyChanged));


        private static void IsShowKeyboardPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var host = d as FrameworkElement;
            if (bool.Parse(e.NewValue.ToString()))
            {
                if (host != null)
                {
                    host.GotFocus += new RoutedEventHandler(OnGotFocus);
                    host.LostFocus += new RoutedEventHandler(OnLostFocus);
                }
            }
            else
            {
                if (host != null)
                {
                    host.GotFocus -= new RoutedEventHandler(OnGotFocus);
                    host.LostFocus -= new RoutedEventHandler(OnLostFocus);
                }
            }
        }

        private static Brush _previewBorderBrush;

        private static void OnLostFocus(object sender, RoutedEventArgs e)
        {
            Control host = sender as Control;
            host.BorderBrush = _previewBorderBrush;

            if (_InstanceObject != null)
            {
                _InstanceObject.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private static void OnGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox host = sender as TextBox;

            _previewBorderBrush = host.BorderBrush;
            host.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7F14B1FB"));

            _CurrentControl = host;

            if (_InstanceObject == null)
            {
                _InstanceObject = new TouchScreenKeyboard();
                _InstanceObject.Width = 450;
                _InstanceObject.Height = 350;
                _InstanceObject.AllowsTransparency = true;
                _InstanceObject.WindowStyle = WindowStyle.None;
                _InstanceObject.ShowInTaskbar = false;
                _InstanceObject.ResizeMode = ResizeMode.NoResize;
                _InstanceObject.MouseLeftButtonDown += new MouseButtonEventHandler(NonRectangularWindow_MouseLeftButtonDown);
                _InstanceObject.Topmost = true;
                _InstanceObject.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                _InstanceObject.ShowActivated = false;
                _InstanceObject.Show();
            }
            else
            {
                _InstanceObject.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private static void NonRectangularWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _InstanceObject.DragMove();
        }
    }
}
