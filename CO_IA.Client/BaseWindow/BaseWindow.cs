using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CO_IA.Client
{
    public class BaseWindow : Window
    {
        public bool isMax = true;
        public bool IsMax
        {
            get
            {
                return isMax;
                //if (maxBtn.Visibility == System.Windows.Visibility.Visible)
                //    return true;
                //else
                //    return false;
            }
            set
            {
                isMax = value;
                if (maxBtn == null)
                    return;
                if (value)
                    maxBtn.Visibility = System.Windows.Visibility.Visible;
                else
                    maxBtn.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
        private bool isMin = true;
        public bool IsMin
        {
            get
            {
                return isMin;
                //if (minBtn.Visibility == System.Windows.Visibility.Visible)
                //    return true;
                //else
                //    return false;
            }
            set
            {
                isMin = value;
                if (minBtn == null)
                    return;
                if (value)
                    minBtn.Visibility = System.Windows.Visibility.Visible;
                else
                    minBtn.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
        public BaseWindow()
        {
            InitializeStyle();
            this.Loaded += delegate
            {
                InitializeEvent();
            };
        }
        Button minBtn;
        Button maxBtn;
        private void InitializeEvent()
        {
            ControlTemplate baseWindowTemplate = (ControlTemplate)Application.Current.Resources["BaseWindowControlTemplate"];

            minBtn = (Button)baseWindowTemplate.FindName("btnMin", this);
            if (isMin)
                minBtn.Visibility = System.Windows.Visibility.Visible;
            else
                minBtn.Visibility = System.Windows.Visibility.Collapsed;
            minBtn.Click += delegate            {

                this.WindowState = WindowState.Minimized;
            };

            maxBtn = (Button)baseWindowTemplate.FindName("btnMax", this);
            if (isMax)
                maxBtn.Visibility = System.Windows.Visibility.Visible;
            else
                maxBtn.Visibility = System.Windows.Visibility.Collapsed;
            maxBtn.Click += delegate
            {

                this.WindowState = (this.WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal);
            };

            Button closeBtn = (Button)baseWindowTemplate.FindName("btnClose", this);
            closeBtn.Click += delegate
            {
                this.Close();
            };

            Border borderTitle = (Border)baseWindowTemplate.FindName("borderTitle", this);
            borderTitle.MouseMove += delegate(object sender, MouseEventArgs e)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
            };
            borderTitle.MouseLeftButtonDown += delegate(object sender, MouseButtonEventArgs e)
            {
                if (e.ClickCount >= 2)
                {
                    maxBtn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                }
            };
        }

        private void InitializeStyle()
        {
            this.Style = (Style)Application.Current.Resources["BaseWindowStyle"];
        }
    }
}