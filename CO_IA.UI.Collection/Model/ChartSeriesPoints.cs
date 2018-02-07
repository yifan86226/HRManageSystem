using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpf.Charts;
using ZedGraph;
using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;



namespace CO_IA.UI.Collection.Model
{
    public class ChartSeriesPoints
    {
        public ChartSeriesPoints()
        {
        }

        /// <summary>
        /// dev控件数据格式，加载速度超级慢
        /// </summary>
        private SeriesPointCollection midValueCollection;

        public SeriesPointCollection MidValueCollection
        {
            get { return midValueCollection; }
            set { midValueCollection = value; }
        }
        private SeriesPointCollection maxValueCollection;

        public SeriesPointCollection MaxValueCollection
        {
            get { return maxValueCollection; }
            set { maxValueCollection = value; }
        }

        /// <summary>
        /// ZedGraph数据格式
        /// </summary>
        private PointPairList midPointPairList;

        public PointPairList MidPointPairList
        {
            get { return midPointPairList; }
            set { midPointPairList = value; }
        }
        private PointPairList maxPointPairList;

        public PointPairList MaxPointPairList
        {
            get { return maxPointPairList; }
            set { maxPointPairList = value; }
        }

        private PointPairList occupyPointPairList;

        public PointPairList OccupyPointPairList
        {
            get { return occupyPointPairList; }
            set { occupyPointPairList = value; }
        }

        /// <summary>
        /// DynamicDataDisplay数据格式
        /// </summary>
        private ObservableDataSource<Point> maxObservableDataSource;

        public ObservableDataSource<Point> MaxObservableDataSource
        {
            get { return maxObservableDataSource; }
            set { maxObservableDataSource = value; }
        }
        private ObservableDataSource<Point> midObservableDataSource;

        public ObservableDataSource<Point> MidObservableDataSource
        {
            get { return midObservableDataSource; }
            set { midObservableDataSource = value; }
        }

        private double minFreq;

        public double MinFreq
        {
            get { return minFreq; }
            set { minFreq = value; }
        }

        private double maxFreq;

        public double MaxFreq
        {
            get { return maxFreq; }
            set { maxFreq = value; }
        }

        private double signalLimit;

        public double SignalLimit
        {
            get { return signalLimit; }
            set { signalLimit = value; }
        }

        private double occuDegreeLimit;

        public double OccuDegreeLimit
        {
            get { return occuDegreeLimit; }
            set { occuDegreeLimit = value; }
        }
    }
}
