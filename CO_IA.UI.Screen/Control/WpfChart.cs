#region 文件描述
/***************************************************************#@#***************************************************************
 * 创建人：xiaguohui
 * 摘 要 ：chart控件，用于地点统计
 * 日 期 ：2016-08-09
 ***************************************************************#@#***************************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CO_IA.UI.Screen.Control
{
    class WpfChart : Grid
    {
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource",
            typeof (IEnumerable<KeyValuePair<string, double>>), typeof (WpfChart),
            new PropertyMetadata(new List<KeyValuePair<string, double>>(), OnItemsSourcePropertyChanged));

        private readonly Brush _maxValueBrush =
            new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FF90c68a"));

        private readonly Brush _defaultBrush = new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FF22b6c4"));
        private readonly Brush _enterBrush = new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FF3bcfdd"));

        
        private readonly Grid _gridContent = new Grid();
        private Brush[] BrushItems = null;
        private readonly string itemColors= "#FF3B75BE,#FFD78227,#FFBA3B35,#FF98BF48,#FF75539D,#FF32A1BF,#FF93B5D0,#FF00797A,#FF749B00,#FFD4A00C";
        public WpfChart()
        {
            InitializeComponent();

        }

        public IEnumerable<KeyValuePair<string, double>> ItemsSource
        {
            get { return (List<KeyValuePair<string, double>>) GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        protected static void OnItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chart = (WpfChart) d;
            // 只要变化就全部重新绘制

            if (chart.ItemsSource == null) return;
            if (chart.ItemsSource.Count() == 0) return;
            if (chart.ActualHeight == 0) return;
            chart.GetVTextCollection();
            chart.DrawCoordinate();
        }

        void InitializeComponent()
        {
            string[] colors = itemColors.Split(','); 
            BrushItems = new Brush[colors.Length];
            for(int i=0;i<BrushItems.Length;i++)
            {
                BrushItems[i] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colors[i]));
            }

           
            this.Children.Add(_gridContent);
            this.SizeChanged += WpfChart_SizeChanged;

            if (DesignerProperties.GetIsInDesignMode(this))
            {
                Random rand = new Random();
                List<KeyValuePair<string, double>> list = new List<KeyValuePair<string, double>>();
                for (var i = 0; i < 4; i++)
                    list.Add(new KeyValuePair<string, double>("项目"+i.ToString(), rand.NextDouble()*100));
                ItemsSource = list;
            }

        }

        private void WpfChart_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            if (ItemsSource == null) return;
            if (ItemsSource.Count() == 0) return;
            if (this.ActualHeight == 0) return;
            
            GetVTextCollection();
            DrawCoordinate();
        }

        private double _maxValue;

        int GetMaxNumber()
        {
            _maxValue = ItemsSource.Select(v => v.Value).Concat(new[] {double.MinValue}).Max();
            return (int) _maxValue;
        }

        IEnumerable<string> GetVTextCollection()
        {
            int maxNumber = GetMaxNumber(); // 获取队列里的最大值。
            int offset = 1;
            if (maxNumber > 1)
            {
                //var tempOffset = int.Parse((maxNumber.ToString()[0].ToString())) < 5 ? "5" : "10";
                //tempOffset += "".PadRight((maxNumber/10).ToString().Length - 1, '0');
                //offset = int.Parse(tempOffset);
                offset = GetReportStepValue(maxNumber);
            }
            List<string> list = new List<string>();
            this._chartMaxNumber = offset*11;
            for (int i = 10; i >= 0; i--)
            {
                list.Add((i*offset).ToString());
            }

            return list;
        }

        int GetReportStepValue(int maxDataValue)
        {
            int maxValue = 0;
            int i = 2;
            while (maxValue < maxDataValue)
            {
                maxValue = 5*i;
                i += 2;
            }
            return maxValue/10;
        } 

        readonly Thickness _viewPortPad = new Thickness(0, 0, 0, 0);

        private int _chartMaxNumber = 0;

        void DrawCoordinate()
        {
            Rectangle rectangle;
            TextBlock tb;
            TextBlock tbTxt;
            _gridContent.ColumnDefinitions.Clear();
            _gridContent.RowDefinitions.Clear();
            _gridContent.Children.Clear();
            
            _gridContent.ColumnDefinitions.Add(new ColumnDefinition()
            {
                Width = new GridLength(1, GridUnitType.Star)
            });
           
            var maxHeight = _gridContent.ActualHeight - _viewPortPad.Top - _viewPortPad.Bottom;
            var maxWidth = _gridContent.ActualWidth - _viewPortPad.Left;

            var list = ItemsSource;
            int index = 0;
            foreach (var v in list)
            {
                var xx = maxWidth * (v.Value / _chartMaxNumber);
                _gridContent.Children.Add(
                    rectangle =
                        new Rectangle()
                        {
                            Fill = BrushItems[index],
                            Width = xx,
                            //Height = maxHeight / list.Count() * 0.8,
                            //VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                            Margin = new Thickness(0,2,0,2)
                        });
                var doubleAnimation = new DoubleAnimation(0, xx,
                    new Duration(new TimeSpan(0, 0, 0, 0, 500)));

                rectangle.BeginAnimation(Rectangle.WidthProperty, doubleAnimation);

                rectangle.Tag = v.Value;

                //if (v.Value == this._maxValue)
                //    rectangle.Fill = _maxValueBrush;

                _gridContent.Children.Add(
                    tbTxt =
                        new TextBlock()
                        {
                            Text = v.Key,
                            FontSize = 14,
                            Foreground=new SolidColorBrush(Colors.White),
                            //Margin = new Thickness(0, 0, 0, maxHeight*(v.Value/_chartMaxNumber) + 5),
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Left,

                        }
                    );
                _gridContent.Children.Add(
                    tb =
                        new TextBlock()
                        {
                            Text = v.Value.ToString("f0"),
                            FontSize = 14,
                            FontWeight = FontWeights.Bold,
                            Foreground = new SolidColorBrush(Colors.White),
                            //Foreground = new SolidColorBrush(Color.FromArgb(255, 249, 179, 179)),
                            //Margin = new Thickness(0, 0, 0, maxHeight*(v.Value/_chartMaxNumber) + 5),
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Right,

                        }
                    );


                var thicknessAnimation = new ThicknessAnimation(new Thickness(0, 0, maxHeight * (v.Value / _chartMaxNumber) + 5, 0),
                    new Thickness(0, 0, 0, 0),
                    new Duration(new TimeSpan(0, 0, 0, 0, 1000)));
                tb.BeginAnimation(TextBlock.MarginProperty, thicknessAnimation);

                //rectangle.MouseEnter += Rectangle_MouseEnter;
                //rectangle.MouseLeave += Rectangle_MouseLeave;

                _gridContent.RowDefinitions.Add(new RowDefinition());
                Grid.SetRow(rectangle, _gridContent.RowDefinitions.Count - 1);
                Grid.SetRow(tb, _gridContent.RowDefinitions.Count - 1);
                Grid.SetRow(tbTxt, _gridContent.RowDefinitions.Count - 1);
                index++;
                if (index >= BrushItems.Length)
                {
                    index = 0;
                }
            }
        }

        private void Rectangle_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Rectangle rectangle = sender as Rectangle;
            if (rectangle == null) return;
            if ((double) rectangle.Tag == this._maxValue)
                rectangle.Fill = _maxValueBrush;
            else
                rectangle.Fill = _defaultBrush;
        }

        private void Rectangle_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Rectangle rectangle = sender as Rectangle;
            if (rectangle != null) rectangle.Fill = _enterBrush;
        }
    }
}