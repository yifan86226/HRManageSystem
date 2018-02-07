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
    public class XCheckBox : CheckBox
    {
        static XCheckBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XCheckBox), new FrameworkPropertyMetadata(typeof(XCheckBox)));
        }


        #region 属性


        [CategoryAttribute("自定义属性"), DescriptionAttribute("获取或设置图片宽度")]
        public double ImageWidth
        {
            get { return (double)GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
        }
        [CategoryAttribute("自定义属性"), DescriptionAttribute("获取或设置图片宽度")]
        public static readonly DependencyProperty ImageWidthProperty =
            DependencyProperty.Register("ImageWidth", typeof(double), typeof(XCheckBox), new PropertyMetadata(0D));



        [CategoryAttribute("自定义属性"), DescriptionAttribute("获取或设置图片高度")]
        public double ImageHeight
        {
            get { return (double)GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }
        [CategoryAttribute("自定义属性"), DescriptionAttribute("获取或设置图片高度")]
        public static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.Register("ImageHeight", typeof(double), typeof(XCheckBox), new PropertyMetadata(0D));



        [CategoryAttribute("自定义属性"), DescriptionAttribute("获取或设置控件默认背景图片")]
        public ImageSource NormalImgae
        {
            get { return ( ImageSource)GetValue(NormalImgaeProperty); }
            set { SetValue(NormalImgaeProperty, value); }
        }
        [CategoryAttribute("自定义属性"), DescriptionAttribute("获取或设置控件默认背景图片")]
        public static readonly DependencyProperty NormalImgaeProperty =
            DependencyProperty.Register("NormalImgae", typeof(ImageSource), typeof(XCheckBox));


        [CategoryAttribute("自定义属性"), DescriptionAttribute("获取或设置控件鼠标悬停时背景图片")]
        public ImageSource MouseOverImage
        {
            get { return (ImageSource)GetValue(MouseOverImageProperty); }
            set { SetValue(MouseOverImageProperty, value); }
        }
        [CategoryAttribute("自定义属性"), DescriptionAttribute("获取或设置控件鼠标悬停时背景图片")]
        public static readonly DependencyProperty MouseOverImageProperty =
            DependencyProperty.Register("MouseOverImage", typeof(ImageSource), typeof(XCheckBox));



        [CategoryAttribute("自定义属性"), DescriptionAttribute("获取或设置控件按下时背景图片")]
        public ImageSource PressedImage
        {
            get { return (ImageSource)GetValue(PressedImageProperty); }
            set { SetValue(PressedImageProperty, value); }
        }

        [CategoryAttribute("自定义属性"), DescriptionAttribute("获取或设置控件按下时背景图片")]
        public static readonly DependencyProperty PressedImageProperty =
            DependencyProperty.Register("PressedImage", typeof(ImageSource), typeof(XCheckBox));



        [CategoryAttribute("自定义属性"), DescriptionAttribute("获取或设置控件选中时背景图片")]
        public ImageSource CheckImage
        {
            get { return ( ImageSource)GetValue(CheckImageProperty); }
            set { SetValue(CheckImageProperty, value); }
        }

        [CategoryAttribute("自定义属性"), DescriptionAttribute("获取或设置控件选中时背景图片")]
        public static readonly DependencyProperty CheckImageProperty =
            DependencyProperty.Register("CheckImage", typeof( ImageSource), typeof(XCheckBox));



        [CategoryAttribute("自定义属性"), DescriptionAttribute("获取或设置控件选中时背景图片")]
        public ImageSource CheckMouseOver
        {
            get { return (ImageSource)GetValue(CheckMouseOverProperty); }
            set { SetValue(CheckMouseOverProperty, value); }
        }

        [CategoryAttribute("自定义属性"), DescriptionAttribute("获取或设置控件选中时背景图片")]
        public static readonly DependencyProperty CheckMouseOverProperty =
            DependencyProperty.Register("CheckMouseOver", typeof(ImageSource),typeof(XCheckBox));


        [CategoryAttribute("自定义属性"), DescriptionAttribute("获取或设置控件选中时背景图片")]
        public ImageSource CheckPressed
        {
            get { return (ImageSource)GetValue(CheckPressedProperty); }
            set { SetValue(CheckPressedProperty, value); }
        }

       [CategoryAttribute("自定义属性"), DescriptionAttribute("获取或设置控件选中时背景图片")]
        public static readonly DependencyProperty CheckPressedProperty =
            DependencyProperty.Register("CheckPressed", typeof(ImageSource), typeof(XCheckBox));




        [CategoryAttribute("自定义属性"), DescriptionAttribute("获取或设置控件选中时背景图片")]
        public ImageSource ThreeState
        {
            get { return (ImageSource)GetValue(ThreeStateProperty); }
            set { SetValue(ThreeStateProperty, value); }
        }

        [CategoryAttribute("自定义属性"), DescriptionAttribute("获取或设置控件选中时背景图片")]
        public static readonly DependencyProperty ThreeStateProperty =
            DependencyProperty.Register("ThreeState", typeof(ImageSource),typeof(XCheckBox));



        [CategoryAttribute("自定义属性"), DescriptionAttribute("获取或设置控件选中时背景图片")]
        public ImageSource ThreeMouseOver
        {
            get { return (ImageSource)GetValue(ThreeMouseOverProperty); }
            set { SetValue(ThreeMouseOverProperty, value); }
        }

        [CategoryAttribute("自定义属性"), DescriptionAttribute("获取或设置控件选中时背景图片")]
        public static readonly DependencyProperty ThreeMouseOverProperty =
            DependencyProperty.Register("ThreeMouseOver", typeof(ImageSource), typeof(XCheckBox));


        [CategoryAttribute("自定义属性"), DescriptionAttribute("获取或设置控件选中时背景图片")]
        public ImageSource ThreePressed
        {
            get { return (ImageSource)GetValue(ThreePressedProperty); }
            set { SetValue(ThreePressedProperty, value); }
        }

        [CategoryAttribute("自定义属性"), DescriptionAttribute("获取或设置控件选中时背景图片")]
        public static readonly DependencyProperty ThreePressedProperty =
            DependencyProperty.Register("ThreePressed", typeof(ImageSource), typeof(XCheckBox));

        

        



        #endregion
    }
}
