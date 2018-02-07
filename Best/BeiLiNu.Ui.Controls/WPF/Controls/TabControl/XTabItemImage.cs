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
    public class XTabItemImage : TabItem
    {
        static XTabItemImage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XTabItemImage), new FrameworkPropertyMetadata(typeof(XTabItemImage)));
        }


        #region 属性


        public Brush SelectedBrush
        {
            get { return GetValue(SelectedBrushProperty) as Brush; }
            set { SetValue(SelectedBrushProperty, value); }
        }

        public static readonly DependencyProperty SelectedBrushProperty =
            DependencyProperty.Register("SelectedBrush", typeof(Brush), typeof(XTabItemImage), new PropertyMetadata(null));



        public Brush MouseOverBrush
        {
            get { return GetValue(MouseOverBrushProperty) as Brush; }
            set { SetValue(MouseOverBrushProperty, value); }
        }

        public static readonly DependencyProperty MouseOverBrushProperty =
            DependencyProperty.Register("MouseOverBrush", typeof(Brush), typeof(XTabItemImage), new PropertyMetadata(null));


        #endregion
    }
}
