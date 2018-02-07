using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;

namespace CO_IA.UI.Collection.helper
{
    /// <summary>
    /// ActualWidth、ActualHeight依赖属性值变更通知代理
    /// </summary>
    public class ActualSizePropertyProxy : FrameworkElement, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public FrameworkElement Element
        {
            get { return (FrameworkElement)GetValue(ElementProperty); }
            set { SetValue(ElementProperty, value); }
        }

        public double ActualHeightValue
        {
            get { return Element == null ? 0 : Element.ActualHeight; }
        }

        public double ActualWidthValue
        {
            get { return Element == null ? 0 : Element.ActualWidth; }
        }

        public Rect RectValue
        {
            get { return new Rect(0, 0, ActualWidthValue, ActualHeightValue); }
        }

        public static readonly DependencyProperty ElementProperty = DependencyProperty.Register("Element", typeof(FrameworkElement), typeof(ActualSizePropertyProxy), new PropertyMetadata(null, OnElementPropertyChanged));

        private static void OnElementPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ActualSizePropertyProxy)d).OnElementChanged(e);
        }

        private void OnElementChanged(DependencyPropertyChangedEventArgs e)
        {
            var oldElement = (FrameworkElement)e.OldValue;
            var newElement = (FrameworkElement)e.NewValue;

            newElement.SizeChanged += OnElementSizeChanged;
            if (oldElement != null)
            {
                oldElement.SizeChanged -= OnElementSizeChanged;
            }
            NotifyPropChange();
        }

        private void OnElementSizeChanged(object sender, SizeChangedEventArgs e)
        {
            NotifyPropChange();
        }

        private void NotifyPropChange()
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("ActualWidthValue"));
                PropertyChanged(this, new PropertyChangedEventArgs("ActualHeightValue"));
                PropertyChanged(this, new PropertyChangedEventArgs("RectValue"));
            }
        }
    }
}
