using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace BeiLiNu.Ui.Controls.WPF.Controls
{
    public class XProgressBar : ProgressBar
    {
        static XProgressBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XProgressBar), new FrameworkPropertyMetadata(typeof(XProgressBar)));
        }


        #region 属性


        [CategoryAttribute("自定义属性"), DescriptionAttribute("获取或设置控件前景图片")]
        public ImageSource FrontImgae
        {
            get { return (ImageSource)GetValue(FrontImgaeProperty); }
            set { SetValue(FrontImgaeProperty, value); }
        }
        [CategoryAttribute("自定义属性"), DescriptionAttribute("获取或设置控件前景图片")]
        public static readonly DependencyProperty FrontImgaeProperty =
            DependencyProperty.Register("FrontImgae", typeof(ImageSource), typeof(XProgressBar));




        [CategoryAttribute("自定义属性"), DescriptionAttribute("获取或设置控件背景图片")]
        public ImageSource BackImage
        {
            get { return (ImageSource)GetValue(BackImageProperty); }
            set { SetValue(BackImageProperty, value); }
        }
        [CategoryAttribute("自定义属性"), DescriptionAttribute("获取或设置控件背景图片")]
        public static readonly DependencyProperty BackImageProperty =
            DependencyProperty.Register("BackImage", typeof(ImageSource), typeof(XProgressBar));



        [CategoryAttribute("自定义属性"), DescriptionAttribute("获取或设置控件背景图片九宫布局")]
        public Thickness NineGrid
        {
            get { return (Thickness)GetValue(NineGridProperty); }
            set { SetValue(NineGridProperty, value); }
        }
        [CategoryAttribute("自定义属性"), DescriptionAttribute("获取或设置控件背景图片九宫布局")]
        public static readonly DependencyProperty NineGridProperty =
            DependencyProperty.Register("NineGrid", typeof(Thickness), typeof(XProgressBar), new PropertyMetadata(new Thickness(0)));



        [CategoryAttribute("自定义属性"), DescriptionAttribute("获取或设置前景色相对背景位置")]
        public Thickness FrontMargin
        {
            get { return (Thickness)GetValue(FrontMarginProperty); }
            set { SetValue(FrontMarginProperty, value); }
        }

        [CategoryAttribute("自定义属性"), DescriptionAttribute("获取或设置前景色相对背景位置")]
        public static readonly DependencyProperty FrontMarginProperty =
            DependencyProperty.Register("FrontMargin", typeof(Thickness), typeof(XProgressBar), new PropertyMetadata(new Thickness(0)));
        #endregion

        #region 隐藏基类属性

        [Browsable(false)]
        public new Brush Background
        {
            get { return base.Background; }
            set { base.Background = value; }
        }

        [Browsable(false)]
        public new Brush BorderBrush
        {
            get { return base.BorderBrush; }
            set { base.BorderBrush = value; }
        }

        [Browsable(false)]
        public new Brush Foreground
        {
            get { return base.Foreground; }
            set { base.Foreground = value; }

        }

        #endregion


    }
}
