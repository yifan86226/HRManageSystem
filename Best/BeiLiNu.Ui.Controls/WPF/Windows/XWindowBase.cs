using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BeiLiNu.Ui.Controls.WPF.Windows
{
    public class XWindowBase : Window
    {
        static XWindowBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XWindowBase), new FrameworkPropertyMetadata(typeof(XWindowBase)));
        }

        public XWindowBase()
        {
            //this.MouseLeftButtonDown += XeWindows_MouseLeftButtonDown;
        }

        private void XeWindows_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        protected override void OnManipulationBoundaryFeedback(ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            WindowBehaviorHelperEX wh = new WindowBehaviorHelperEX(this);
            wh.RepairBehavior();
        }

        [CategoryAttribute("自定义属性"), DescriptionAttribute("获取或设置窗体圆角值")]
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(XWindowBase), new PropertyMetadata(new CornerRadius(0)));


        [CategoryAttribute("自定义属性"), DescriptionAttribute("获取或设置窗体透明度")]
        public double BackOpacity
        {
            get { return (double)GetValue(BackOpacityProperty); }
            set { SetValue(BackOpacityProperty, value); }
        }

        public static readonly DependencyProperty BackOpacityProperty = DependencyProperty.Register("BackOpacity", typeof(double), typeof(XWindowBase), new UIPropertyMetadata(1d));


        [CategoryAttribute("自定义属性"), DescriptionAttribute("获取或设置窗体阴影大小")]
        public double ShadowSize
        {
            get { return (double)GetValue(ShadowSizeProperty); }
            set { SetValue(ShadowSizeProperty, value); }
        }
        public static readonly DependencyProperty ShadowSizeProperty = DependencyProperty.Register("ShadowSize", typeof(double), typeof(XWindowBase), new UIPropertyMetadata(0d));


        public static readonly DependencyProperty NineGridProperty = DependencyProperty.Register("NineGrid", typeof(Thickness), typeof(XWindowBase), new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.None));

        [CategoryAttribute("自定义属性"), DescriptionAttribute("获取或设置阴影九宫格图片如何拉伸")]
        public Thickness NineGrid
        {
            get { return (Thickness)GetValue(NineGridProperty); }
            set { SetValue(NineGridProperty, value); }
        }

        public static readonly DependencyProperty NineImageProperty = DependencyProperty.Register("NineImage", typeof(ImageSource), typeof(XWindowBase));
        [CategoryAttribute("自定义属性"), DescriptionAttribute("获取或设置阴影九宫格图片")]
        public ImageSource NineImage
        {
            get { return (ImageSource)GetValue(NineImageProperty); }
            set { SetValue(NineImageProperty, value); }
        }
    }
}
