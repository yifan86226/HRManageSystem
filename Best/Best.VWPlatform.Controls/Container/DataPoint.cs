using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Best.VWPlatform.Controls.Container
{
    public class DataPoint : Control
    {
        private double _yAxisHeight;
        private double _yAxisUnitLength = 1;
        private double _yAxisValue;
        private string _formatText = "#0.##%";

        public static readonly DependencyProperty FormattedValueProperty =
            DependencyProperty.Register("FormattedValue", typeof(string), typeof(DataPoint), new PropertyMetadata(default(string)));

        /// <summary>
        /// 格式化后的字符串
        /// </summary>
        public string FormattedValue
        {
            get { return (string)GetValue(FormattedValueProperty); }
            set { SetValue(FormattedValueProperty, value); }
        }

        public string Label { get; set; }

        /// <summary>
        /// 格式化字符串
        /// </summary>
        public string FormatText
        {
            get { return _formatText; }
            set
            {
                _formatText = value;
                FormattedValue = string.Format(FormatText, XAxisValue, YAxisValue);
            }
        }

        /// <summary>
        /// y 轴高度
        /// </summary>
        public double YAxisHeight
        {
            get { return _yAxisHeight; }
            set
            {
                //FIX:Y unitlength 改变后高度不改变
                _yAxisHeight = value;
                Height = value * YAxisUnitLength;
            }
        }

        /// <summary>
        /// x 轴值
        /// </summary>
        public double XAxisValue { get; set; }

        /// <summary>
        /// Y轴值
        /// </summary>
        public double YAxisValue
        {
            get { return _yAxisValue; }
            set
            {
                _yAxisValue = value;
                FormattedValue = string.Format(FormatText, XAxisValue, YAxisValue);
            }
        }

        /// <summary>
        /// y 轴单位长度
        /// </summary>
        public double YAxisUnitLength
        {
            get { return _yAxisUnitLength; }
            set
            {
                if (!_yAxisUnitLength.Equals(value))
                    _yAxisUnitLength = value;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="xvalue">x 轴值</param>
        /// <param name="yvalue">测向次数</param>
        public DataPoint(double xvalue, double yvalue)
        {
            DefaultStyleKey = typeof(DataPoint);

            XAxisValue = xvalue;
            YAxisValue = yvalue;
            VerticalAlignment = VerticalAlignment.Bottom;
            HorizontalAlignment = HorizontalAlignment.Center;
        }

        #region 重写

        public bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (DataPoint)obj;
            return XAxisValue.Equals(other.XAxisValue);
        }

        public int GetHashCode()
        {
            return 1;
        }

        #endregion
    }
}
