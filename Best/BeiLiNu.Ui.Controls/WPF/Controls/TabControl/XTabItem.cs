using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class XTabItem : TabItem
    {
        static XTabItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XTabItem), new FrameworkPropertyMetadata(typeof(XTabItem)));
        }



        [CategoryAttribute("自定义属性"), DescriptionAttribute("获取或设置控件圆角值")]
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        public static readonly DependencyProperty CornerRadiusProperty = 
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(XTabItem), new PropertyMetadata(new CornerRadius(0)));

        public Brush SelectedBrush
        {
            get { return GetValue(SelectedBrushProperty) as Brush; }
            set { SetValue(SelectedBrushProperty, value); }
        }
        public static readonly DependencyProperty SelectedBrushProperty = 
            DependencyProperty.Register("SelectedBrush", typeof(Brush), typeof(XTabItem), new PropertyMetadata(null));

        public Brush MouseOverBrush
        {
            get { return GetValue(MouseOverBrushProperty) as Brush; }
            set { SetValue(MouseOverBrushProperty, value); }
        }
        public static readonly DependencyProperty MouseOverBrushProperty =
            DependencyProperty.Register("MouseOverBrush", typeof(Brush), typeof(XTabItem), new PropertyMetadata(null));
    }
}
