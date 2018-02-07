using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using System.Threading;
using CO_IA.UI.Collection.Chart;
using GalaSoft.MvvmLight.Messaging;
using CO_IA.UI.Collection.ViewModel;
using System.ComponentModel;
using System.Windows.Threading;
using CO_IA.Data.Portable.Collection;

namespace CO_IA.UI.Collection.Chart
{
    public class ChartViewModel : ViewModelBase, IDisposable
    {
        private ObservableCollection<LineGraphViewModel> lineGraphs;
        private ICommand addLinesCommand;
        private ICommand editLineCommand;
        private ICommand ucLoadedCommand;

        private HorizontalDateTimeAxis dateAxis;
        private CompositeDataSource editedDs;


        List<int> pakagLength = new List<int>();
        List<double> x = new List<double>();
        List<double> y = new List<double>();
        EnumerableDataSource<double> xs;
        EnumerableDataSource<double> ys;
        FreqDataCollectViewModel _FreqDataCollectViewModel;
        public ChartViewModel(FreqDataCollectViewModel freqVM)
        {
            _uiContext = SynchronizationContext.Current;
            _MyLatestQuote = new MinuteQuoteViewModel();
            _MyLatestQuote.LastPx = 0;
            _FreqDataCollectViewModel = freqVM;
        }

        public ObservableCollection<LineGraphViewModel> LineGraphs
        {
            get
            {
                if (this.lineGraphs == null)
                {
                    this.lineGraphs = new ObservableCollection<LineGraphViewModel>();
                }

                return this.lineGraphs;
            }
        }

        public ICommand WinLoadedCommand
        {
            get
            {
                if (this.ucLoadedCommand == null)
                {
                    this.ucLoadedCommand = new RelayCommand<object>(param => this.InitChart(), param => true);
                }

                return this.ucLoadedCommand;
            }
        }

        public ICommand AddLinesCommand
        {
            get
            {
                if (this.addLinesCommand == null)
                {
                    this.addLinesCommand = new RelayCommand<object>(param => this.AddLineGraphs(), param => true);
                }

                return this.addLinesCommand;
            }
        }

        public ICommand EditLineCommand
        {
            get
            {
                if (this.editLineCommand == null)
                {
                    this.editLineCommand = new RelayCommand<object>(param => this.EditLineGraph(), param => true);
                }

                return this.editLineCommand;
            }
        }

        private int GetPacageIndex(int FreqMeasurePakageId)
        {
            if (FreqMeasurePakageId == 1)
                return 0;

            int baseIndex = 0;

            for (int i = 0; i < FreqMeasurePakageId - 1; i++)
            {
                if (pakagLength.Count > i)
                    baseIndex += pakagLength[i];
            }
            return baseIndex;
        }

        //private Random random = new Random();

        private bool UpdateChartData_bak(FreqLineDataItem item)
        {
            int FreqMeasureId = item.FreqMeasureId;
            int FreqMeasurePakageId = item.FreqMeasurePakageId;
            double startFrequency = item.startFrequency;
            double frequencyStep = item.frequencyStep;
            float[] byteArray = item.byteArray;

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {

                if (FreqMeasureId == 1)
                {
                    pakagLength.Add(byteArray.Length);
                    for (int i = 0; i < byteArray.Length; i++)
                    {
                        x.Add(startFrequency + frequencyStep * i);
                        y.Add(byteArray[i] + 107);
                    }
                }
                else
                {
                    for (int i = 0; i < byteArray.Length; i++)
                    {
                        int baseIndex = GetPacageIndex(FreqMeasurePakageId);
                        x[i + baseIndex] = (startFrequency + frequencyStep * i);
                        y[i + baseIndex] = (byteArray[i] + 107);
                    }
                }
                ys.RaiseDataChanged();
                xs.RaiseDataChanged();
            });

            return true;
        }
        private bool UpdateChartData_bak2(FreqLineDataItem item)
        {
            int FreqMeasureId = item.FreqMeasureId;
            int FreqMeasurePakageId = item.FreqMeasurePakageId;
            double startFrequency = item.startFrequency;
            double frequencyStep = item.frequencyStep;
            float[] byteArray = item.byteArray;

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {

                if (FreqMeasureId == 1)
                {
                    if (FreqMeasurePakageId == 1)
                    {
                        double pointCount = (item.TestFreqEnd * 1000 * 1000.0 - item.TestFreqStart * 1000 * 1000.0) / item.frequencyStep;
                        myCount = Convert.ToInt32(Math.Ceiling(pointCount));
                        if (myCount > 450)
                        {
                            myCount = 450;
                        }
                        myStep = (item.TestFreqEnd * 1000 * 1000.0 - item.TestFreqStart * 1000 * 1000.0) / myCount;
                        myStepCount = Convert.ToInt32(myStep / item.frequencyStep);

                        //for (int i = 0; i < myCount; i++)
                        //{
                        //    x.Add(0);
                        //    y.Add(0);
                        //}
                    }
                    pakagLength.Add(byteArray.Length);


                }
                else
                {
                    for (int i = 0; i < byteArray.Length; i++)
                    {
                        int baseIndex = GetPacageIndex(FreqMeasurePakageId);

                        int tempIndex = (baseIndex + i) / myStepCount;
                        if ((baseIndex + i) % myStepCount == 0)
                        {
                            if (tempIndex < myCount)
                            {
                                if (x.Count <= tempIndex)
                                {
                                    x.Add(startFrequency + frequencyStep * i);
                                    y.Add(byteArray[i] + 107);
                                }
                                else
                                {

                                    x[tempIndex] = (startFrequency + frequencyStep * i);
                                    y[tempIndex] = (byteArray[i] + 107);
                                }

                            }
                            else
                            {
                                break;
                            }
                        }

                    }
                }
                ys.RaiseDataChanged();
                xs.RaiseDataChanged();
            });

            return true;
        }

        private int pointTotleCount = 0;


        public bool UpdateChartDataRelaod(FreqLineDataItem item)
        {
            int FreqMeasureId = item.FreqMeasureId;
            int FreqMeasurePakageId = item.FreqMeasurePakageId;
            double startFrequency = item.startFrequency;
            double frequencyStep = item.frequencyStep;
            float[] byteArray = item.byteArray;

            if (FreqMeasureId == 1)
            {
                pakagLength.Add(byteArray.Length);
                pointTotleCount += byteArray.Length;
            }
            else if (FreqMeasureId == 2)
            {
                if (FreqMeasurePakageId == 1)
                {
                    //初始化谱图
                    BestFreqDataItem bestItem = new BestFreqDataItem();
                    bestItem.FreqStep = item.frequencyStep;
                    bestItem.FreqTotolCount = pointTotleCount;
                    bestItem.TestFreqStart = item.TestFreqStart * 1000 * 1000.0;
                    bestItem.TestFreqEnd = item.TestFreqEnd * 1000 * 1000.0;

                    Messenger.Default.Send<GenericMessage<BestFreqDataItem>>(new GenericMessage<BestFreqDataItem>(this, bestItem));
                }
            }
            else
            {
                int baseIndex = GetPacageIndex(FreqMeasurePakageId);
                BestFreqDataItem bestItem = new BestFreqDataItem();
                bestItem.FreqStep = item.frequencyStep;
                bestItem.FreqTotolCount = pointTotleCount;
                bestItem.TestFreqStart = item.TestFreqStart * 1000 * 1000.0;
                bestItem.TestFreqEnd = item.TestFreqEnd * 1000 * 1000.0;
                bestItem.SecFreqStart = item.startFrequency;
                bestItem.SecDataIndex = baseIndex;

                bestItem.pointNums = item.byteArray.Length;
                bestItem.data = new short[byteArray.Length];

                for (int i = 0; i < byteArray.Length; i++)
                {
                    float a = byteArray[i] + 107;
                    bestItem.data[i] = (short)((a));
                }
                Messenger.Default.Send<GenericMessage<BestFreqDataItem>>(new GenericMessage<BestFreqDataItem>(this, bestItem));
            }

            return true;
        }



        public bool UpdateChartData(FreqLineDataItem item)
        {
            int FreqMeasureId = item.FreqMeasureId;
            int FreqMeasurePakageId = item.FreqMeasurePakageId;
            double startFrequency = item.startFrequency;
            double frequencyStep = item.frequencyStep;
            float[] byteArray = item.byteArray;

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {

                if (FreqMeasureId == 1)
                {
                    pakagLength.Add(byteArray.Length);
                    pointTotleCount += byteArray.Length;
                }
                else if (FreqMeasureId == 2)
                {
                    if (FreqMeasurePakageId == 1)
                    {
                        //初始化谱图
                        BestFreqDataItem bestItem = new BestFreqDataItem();
                        bestItem.FreqStep = item.frequencyStep;
                        bestItem.FreqTotolCount = pointTotleCount;
                        bestItem.TestFreqStart = item.TestFreqStart * 1000 * 1000.0;
                        bestItem.TestFreqEnd = item.TestFreqEnd * 1000 * 1000.0;

                        Messenger.Default.Send<GenericMessage<BestFreqDataItem>>(new GenericMessage<BestFreqDataItem>(this, bestItem));
                    }
                }

                else
                {

                    int baseIndex = GetPacageIndex(FreqMeasurePakageId);
                    BestFreqDataItem bestItem = new BestFreqDataItem();
                    bestItem.FreqStep = item.frequencyStep;
                    bestItem.FreqTotolCount = pointTotleCount;
                    bestItem.TestFreqStart = item.TestFreqStart * 1000 * 1000.0;
                    bestItem.TestFreqEnd = item.TestFreqEnd * 1000 * 1000.0;
                    bestItem.SecFreqStart = item.startFrequency;
                    bestItem.SecDataIndex = baseIndex;

                    bestItem.pointNums = item.byteArray.Length;
                    bestItem.data = new short[byteArray.Length];

                    for (int i = 0; i < byteArray.Length; i++)
                    {
                        float a = byteArray[i] + 107;
                        bestItem.data[i] = (short)((a));
                    }

                    Messenger.Default.Send<GenericMessage<BestFreqDataItem>>(new GenericMessage<BestFreqDataItem>(this, bestItem));
                }
            });

            return true;
        }


        private int myCount = 0;
        private double myStep = 0;
        private int myStepCount = 0;


        private object lockObj = new object();

        private Queue<FreqLineDataItem> FreqLineDataItemBuffer = new Queue<FreqLineDataItem>();
        public void InsertShowData(FreqLineDataItem data)
        {
            //lock (lockObj)
            //{
            //    FreqLineDataItemBuffer.Enqueue(data);
            //}

            lock (FreqLineDataItemBuffer)
            {
                FreqLineDataItemBuffer.Enqueue(data);
                Monitor.Pulse(FreqLineDataItemBuffer);
            }


        }


        private readonly SynchronizationContext _uiContext;

        private void MySendOrPostCallback(object obj)
        {
            FreqLineDataItem item = obj as FreqLineDataItem;
            UpdateChartData(item);
            if (_FreqDataCollectViewModel != null)
                _FreqDataCollectViewModel.UpdateRtFreqDataModel(item);
        }
        public void InitChart()
        {
            xs = new EnumerableDataSource<double>(y);
            xs.SetYMapping(_y => _y);
            ys = new EnumerableDataSource<double>(x);
            this.dateAxis = new HorizontalDateTimeAxis();
            ys.SetXMapping(t_y => t_y);
            CompositeDataSource ds = new CompositeDataSource(xs, ys);
            LineGraphViewModel lineGraphViewModel = new LineGraphViewModel();
            lineGraphViewModel.PointDataSource = ds;
            this.editedDs = ds;
            lineGraphViewModel.Name = "频谱曲线";
            lineGraphViewModel.Color = Color.FromRgb(255, 0, 0);
            lineGraphViewModel.EntityId = Guid.NewGuid();
            lineGraphViewModel.LineAndMarker = false;
            lineGraphViewModel.Thickness = 1;

            this.LineGraphs.Add(lineGraphViewModel);

            _uiContext.Send(o =>
            {

                ThreadShowOperator = new Thread(new ThreadStart(() =>
                {
                    while (true)
                    {
                        lock (FreqLineDataItemBuffer)
                        {
                            if (FreqLineDataItemBuffer.Count > 0)
                            {
                                FreqLineDataItem item = FreqLineDataItemBuffer.Dequeue();
                                _uiContext.Post(MySendOrPostCallback, item);
                                Monitor.Pulse(FreqLineDataItemBuffer);
                            }
                            else
                            {
                                Monitor.Wait(FreqLineDataItemBuffer);
                            }
                        }
                    }
                }))
                { IsBackground = true };
                ThreadShowOperator.Start();
            }, null);
            //BackgroundWorker backgroundWorker = new BackgroundWorker();
            //backgroundWorker.DoWork +=(o,ea)=>
            //{
            //    try{
            //        while (true)
            //        {
            //            lock (FreqLineDataItemBuffer)
            //            {
            //                if (FreqLineDataItemBuffer.Count > 0)
            //                {
            //                    FreqLineDataItem item = FreqLineDataItemBuffer.Dequeue();
            //                    //UpdateChartData(item);
            //                    _uiContext.Send(MySendOrPostCallback, item);
            //                    Monitor.Pulse(FreqLineDataItemBuffer);
            //                }
            //                else
            //                {
            //                    Monitor.Wait(FreqLineDataItemBuffer);
            //                }
            //            }
            //        }
            //    }
            //    catch(Exception e)
            //    {

            //    }
            //};

            //backgroundWorker.RunWorkerCompleted += (o, ea) =>
            //{

            //};

            //backgroundWorker.RunWorkerAsync();
            //ThreadPool.QueueUserWorkItem(
            //  o =>
            //  {
            //      while (true)
            //      {
            //          lock (FreqLineDataItemBuffer)
            //          {
            //              if (FreqLineDataItemBuffer.Count > 0)
            //              {
            //                  FreqLineDataItem item = FreqLineDataItemBuffer.Dequeue();
            //                  UpdateChartData(item);
            //                  Monitor.Pulse(FreqLineDataItemBuffer);
            //              }
            //              else
            //              {
            //                  Monitor.Wait(FreqLineDataItemBuffer);
            //              }
            //          }
            //      }
            //  });
        }
        Random random = new Random();
        private void EditLineGraph()
        {

        }


        private void AddLineGraphs()
        {


        }


        private MinuteQuoteViewModel _MyLatestQuote;


        /// <summary>
        /// 
        /// </summary>
        public MinuteQuoteViewModel MyLatestQuote
        {
            get
            {
                return _MyLatestQuote;
            }
            set
            {
                Set(() => MyLatestQuote, ref _MyLatestQuote, value);
            }
        }


        public void Clear()
        {
            pakagLength.Clear();
            pointTotleCount = 0;

        }

        private Thread ThreadShowOperator;

        public void Dispose()
        {
            if (ThreadShowOperator != null)
                ThreadShowOperator.Abort();
        }


        public override void Cleanup()
        {
            // Clean up if needed
            this.Dispose();
            base.Cleanup();
        }
    }

    public class BestFreqDataItem
    {
        public bool IsHandle = false;


        public double TestFreqStart;
        public double TestFreqEnd;
        public int FreqTotolCount;
        public double FreqStep;



        public int SecDataIndex;
        public double SecFreqStart;
        public int pointNums;
        public short[] data;

    }
}
