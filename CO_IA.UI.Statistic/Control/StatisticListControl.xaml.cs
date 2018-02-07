using AT_BC.Data;
using CO_IA.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Xml;

namespace CO_IA.UI.Statistic
{
    /// <summary>
    /// StatisticListControl.xaml 的交互逻辑
    /// </summary>
    public partial class StatisticListControl : UserControl
    {
        //group、column
        public event Action<StatisticDataSource, string> OnCellClick;

        public event Action<StatisticDataSource, string> OnCellClick_RF;


        public event Action<StatisticDataSource, string> OnCellClick_OUT;


        public event Action<StatisticDataSource, string> OnMouseLeftButtonDownnClick;
        /// <summary>
        /// DataGrid数据源
        /// </summary>
        public List<StatisticDataSource> StatisticItemsSource
        {
            get { return (List<StatisticDataSource>)GetValue(StatisticItemsSourceProperty); }
            set { SetValue(StatisticItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty StatisticItemsSourceProperty =
            DependencyProperty.Register("StatisticItemsSource", typeof(List<StatisticDataSource>), typeof(StatisticListControl),
            new PropertyMetadata(new PropertyChangedCallback(StatisticItemsSourcePropertyChangedCallBack)));

        private static void StatisticItemsSourcePropertyChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as StatisticListControl).OnStatistic();
        }

        private List<NameValuePair<string>> _statisticGroup;
        /// <summary>
        /// 统计分组条件（列）
        /// </summary>
        public List<NameValuePair<string>> statisticGroup
        {
            get { return _statisticGroup; }
            set { _statisticGroup = value; }
        }

        public StatisticListControl()
        {
            InitializeComponent();

            //Series.CrosshairLabelPattern = "时间: {A}\n分数: {V}";



        }

        private void OnStatistic()
        {
            Statistic();
            statisticDataGrid.ItemsSource = StatisticItemsSource;
        }

        private void Statistic()
        {
            statisticDataGrid.Columns.Clear();

            DataGridTemplateColumn conditioncolumn = new DataGridTemplateColumn();
            //conditioncolumn.CellTemplate = GetColumnDataTemplate("Group");
            conditioncolumn.CellTemplate = GetColumnCellTemplate("Group");
            this.statisticDataGrid.Columns.Add(conditioncolumn);

            Style styleRight = new Style(typeof(TextBlock));
            Setter setRight = new Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center);//右对齐
            Setter setVertical = new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);//垂直居中
            styleRight.Setters.Add(setRight);
            styleRight.Setters.Add(setVertical);

            foreach (NameValuePair<string> group in statisticGroup)
            {
                DataGridHyperlinkColumn linkcolumn = new DataGridHyperlinkColumn() { Header = group.Value, MinWidth = 50 };
                linkcolumn.Binding = new Binding { Path = new PropertyPath("[" + group.Name + "]") };
                linkcolumn.TargetName = group.Name;
                linkcolumn.ElementStyle = styleRight;

                //DataGridTextColumn columnname = new DataGridTextColumn { Header = group, MinWidth = 100 };
                //columnname.Binding = new Binding { Path = new PropertyPath("[" + group + "]") };
                //this.statisticDataGrid.Columns.Add(columnname);

                //DataGridTemplateColumn cellcolumn = new DataGridTemplateColumn();
                //cellcolumn.Header = group;
                //cellcolumn.CellTemplate = GetColumnCellTemplate(group);

                this.statisticDataGrid.Columns.Add(linkcolumn);
            }



            DataGridTemplateColumn btncolumn = new DataGridTemplateColumn();
            btncolumn.Header = "详情";
           
            btncolumn.CellTemplate = GetColumnCellButtonTemplate("Group");
            this.statisticDataGrid.Columns.Add(btncolumn);



        }

        private DataTemplate GetColumnDataTemplate(string group)
        {
            StringBuilder cellTemp = new StringBuilder();
            cellTemp.Append(@"<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
              xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
              xmlns:local='clr-namespace:CO_IA.UI.Statistic.Converter;assembly=CO_IA.UI.Statistic'>");
            cellTemp.Append("<Grid>");
            cellTemp.Append("<Grid.Resources>");
            cellTemp.Append("<local:ColorConverter x:Key='colorConvert' />");
            cellTemp.Append("</Grid.Resources>");
            cellTemp.Append("<TextBlock MinWidth=\"100\" Text=\"{Binding " + group + "}\"  Foreground=\"{Binding Path=" + group + ",Converter={StaticResource colorConvert}}\"  />");
            cellTemp.Append("</Grid>");
            cellTemp.Append("</DataTemplate>");

            byte[] bytes = Encoding.ASCII.GetBytes(cellTemp.ToString());
            MemoryStream stream = new MemoryStream(bytes);
            return (DataTemplate)System.Windows.Markup.XamlReader.Load(stream);
        }

        private DataTemplate GetColumnCellTemplate(string group)
        {
            StringBuilder cellTemp = new StringBuilder();
            cellTemp.Append(@"<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
              xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
              xmlns:local='clr-namespace:CO_IA.UI.Statistic;assembly=CO_IA.UI.Statistic'
              xmlns:converter='clr-namespace:CO_IA.UI.Statistic.Converter;assembly=CO_IA.UI.Statistic'>");
            cellTemp.Append("<Grid>");
            cellTemp.Append("<Grid.Resources>");
            cellTemp.Append("<converter:DataConverter x:Key='KeyDataConverter' />");
            cellTemp.Append("</Grid.Resources>");
            cellTemp.Append("<TextBlock MinWidth=\"100\"  HorizontalAlignment=\"Right\" >");
            cellTemp.Append("<Hyperlink x:Name=\"hylink\" Tag=\"" + group + "\" DataContext=\"{Binding }\">");
            cellTemp.Append("<TextBlock Text=\"{Binding " + group + "}\"   />");
            cellTemp.Append("</Hyperlink>");
            cellTemp.Append("</TextBlock>");
            cellTemp.Append("</Grid>");
            cellTemp.Append("</DataTemplate>");

            StringReader strreader = new StringReader(cellTemp.ToString());
            XmlTextReader xmlreader = new XmlTextReader(strreader);
            object obj = XamlReader.Load(xmlreader);
            DataTemplate temp = (DataTemplate)obj;

            //DependencyObject rootElement = temp.LoadContent();
            //FrameworkElement frameworkElement = (FrameworkElement)rootElement;
            //Hyperlink link = (Hyperlink)frameworkElement.FindName("hylink");
            //link.Click += Hyperlink_Click;
            ////Hyperlink link1 = (Hyperlink)LogicalTreeHelper.FindLogicalNode(rootElement, "hylink");

            return (DataTemplate)obj;
        }


        private DataTemplate GetColumnCellButtonTemplate(string group)
        {
            StringBuilder cellTemp = new StringBuilder();
            cellTemp.Append(@"<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
              xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
              xmlns:local='clr-namespace:CO_IA.UI.Statistic;assembly=CO_IA.UI.Statistic'
              >");
            cellTemp.Append("<Grid>");


            cellTemp.Append("<TextBlock MinWidth=\"75\"  Margin=\"5, 2\" HorizontalAlignment=\"Right\" >");
            cellTemp.Append("<Hyperlink   TargetName=\"数据详情\" >");
            cellTemp.Append("<TextBlock Text=\"数据详情\"   />");
            cellTemp.Append("</Hyperlink>");
            cellTemp.Append("</TextBlock>");
 
            cellTemp.Append("</Grid>");
            cellTemp.Append("</DataTemplate>");

            StringReader strreader = new StringReader(cellTemp.ToString());
            XmlTextReader xmlreader = new XmlTextReader(strreader);
            object obj = XamlReader.Load(xmlreader);
            DataTemplate temp = (DataTemplate)obj;

 

            return (DataTemplate)obj;
        }




        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            if (statisticDataGrid.SelectedItem != null)
            {
                Hyperlink hylink = e.OriginalSource as Hyperlink;
                if (hylink != null)
                {
                    string column = hylink.TargetName;
                    StatisticDataSource data = (StatisticDataSource)statisticDataGrid.SelectedItem;
                    if (data != null)
                    {
                        double count = data[column];
                        //if (count > 0)
                        //{
                            string group = data.Group;
                            if (OnCellClick != null)
                            {
                                OnCellClick(data, column);
                            }

                            else if (OnCellClick_RF != null)
                            {
                                OnCellClick_RF(data, column);
                            }


                        else if (OnCellClick_OUT != null)
                        {
                            OnCellClick_OUT(data, column);
                        }
                        //}
                    }
                }
            }
        }

        private void statisticDataGrid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

           
            if (statisticDataGrid.SelectedItem != null)
            {
                StatisticDataSource data = (StatisticDataSource)statisticDataGrid.SelectedItem;
                if (data != null)
                {
                    string group = data.Group;
                    if (OnMouseLeftButtonDownnClick != null)
                    {
                        OnMouseLeftButtonDownnClick(data, data.GroupGuid);
                    }
                }
            }
        }
    }
}
