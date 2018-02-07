using CO_IA.Data;
using CO_IA.Data.PlanDatabase;
using DevExpress.Xpf.Charts;
using System;
using System.Collections.Generic;
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

namespace CO_IA.UI.Statistic
{
    /// <summary>
    /// Interaction logic for StatisticLineChartControl.xaml
    /// </summary>
    public partial class StatisticLineChartControl : UserControl
    {


        private string nameid;
        public string NameID
        {
            get
            {
                return nameid;
            }
            set
            {
                nameid = value;

            }
        }

        private string incident;
        public string Incident
        {
            get
            {
                return incident;
            }
            set
            {
                if (value != "合计")

                {
                    incident = value;

                }

            }
        }


        private string fromdate;
        public string FromDate
        {
            get
            {
                return fromdate;
            }
            set
            {
                fromdate = value;
            }
        }





        private string todate;
        public string ToDate
        {
            get
            {
                return todate;
            }
            set
            {
                todate = value;
            }
        }




        public StatisticLineChartControl()
        {
            InitializeComponent();
        }





        ConstantLineCollection ConstantLines
        {
            get { return ((XYDiagram2D)cc_CaseStatistics.Diagram).AxisY.ConstantLinesBehind; }
        }

        void chart_BoundDataChanged(object sender, RoutedEventArgs e)
        {

            ConstantLines.Clear();//清理上次显示的线条


            XYDiagram2D diagram = (XYDiagram2D)cc_CaseStatistics.Diagram;
            if (diagram.Series[0].Points.Count == 0)
                return;
            double minPrice = Double.MaxValue;
            double maxPrice = 0;
            double averagePrice = 0;
            foreach (SeriesPoint point in diagram.Series[0].Points)
            {
                double price = point.Value;
                if (price < minPrice)
                    minPrice = price;
                if (price > maxPrice)
                    maxPrice = price;
                averagePrice += price;
            }
            averagePrice /= diagram.Series[0].Points.Count;
            ConstantLine minConstantLine = new ConstantLine(minPrice, "最少");
            minConstantLine.Brush = new SolidColorBrush(Colors.Green);
            minConstantLine.Title.Foreground = new SolidColorBrush(Colors.Green);
            ConstantLine maxConstantLine = new ConstantLine(maxPrice, "最多");
            maxConstantLine.Brush = new SolidColorBrush(Colors.Red);
            maxConstantLine.Title.Foreground = new SolidColorBrush(Colors.Red);
            ConstantLine averageConstantLine = new ConstantLine(averagePrice, "平均");
            averageConstantLine.Brush = new SolidColorBrush(Color.FromArgb(0xFF, 0x9A, 0xCD, 0x32));
            averageConstantLine.Title.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0x9A, 0xCD, 0x32));
            ConstantLines.AddRange(new ConstantLine[] { minConstantLine, maxConstantLine, averageConstantLine
            });
            foreach (ConstantLine constantLine in ConstantLines)
                constantLine.Title.Alignment = ConstantLineTitleAlignment.Far;
        }


        public void LoadData()
        {
            StringPairList DataList = new StringPairList();

            DataManager.Public.PersonRewardPunishInfoModel model = new DataManager.Public.PersonRewardPunishInfoModel();

            List<PersonRewardPunishInfo> ppilist = model.GetPersonRewardPunishInfos("", "", nameid, fromdate, todate, "order by RPTIME ");

            if (ppilist.Count > 0)
            {
                this.chartTitle.Content = ppilist[0].NAME + " 的量化数据走势图";

            }
            double tatolScore = 0;

            foreach (PersonRewardPunishInfo ppi in ppilist)
            {

                string datestr = "";
                try
                {
                    datestr = Convert.ToDateTime(ppi.RPTIME).ToString("yyyy-MM-dd");
                }
                catch
                {

                }


                foreach (StringPair tempsp in DataList.SPList)
                {
                    if (tempsp.Key == datestr)
                    {
                        tempsp.Value = (Convert.ToDouble(tempsp.Value) + ppi.FRACTION).ToString();

                        continue;
                    }

                }

                StringPair sp = new StringPair();



                sp.Key = datestr;


                tatolScore = tatolScore + ppi.FRACTION;
                sp.Value = tatolScore.ToString();


                DataList.SPList.Add(sp);

            }
            this.Series.DataSource = null;
            this.Series.DataSource = DataList.SPList;

        }
    }
}
