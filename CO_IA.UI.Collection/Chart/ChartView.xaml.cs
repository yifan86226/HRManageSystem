using System;
using System.Collections.Generic;
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
using Best.VWPlatform.Common.Rmtp.DataFrames;
using Best.VWPlatform.Controls.Freq;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Common;
using Microsoft.Research.DynamicDataDisplay.ViewportRestrictions;
using CO_IA.UI.Collection.Chart;

namespace CO_IA.UI.Collection.Chart
{
    /// <summary>
    /// ChartView.xaml 的交互逻辑
    /// </summary>
    public partial class ChartView : UserControl
    {
        public ChartView()
        {
            InitializeComponent();
            Loaded += ChartView_Loaded;
            this.Unloaded += ChartView_Unloaded;
        }


        private void GenericMessageAction(GenericMessage<BestFreqDataItem> obj)
        {
            if (obj.Content.IsHandle)
                return;
            obj.Content.IsHandle = true;

            if (obj.Content.pointNums == 0)
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    x_widebandFreq.Clear();
                    x_widebandFreq.MeasureUnit = "dBμV";
                    x_widebandFreq.InitSpectrumProperty(null, null, obj.Content.TestFreqStart, obj.Content.TestFreqEnd);
                    x_widebandFreq.Update();
                    x_widebandFreq.Initializers(obj.Content.TestFreqStart, obj.Content.FreqStep, (int)obj.Content.FreqTotolCount);
                }), System.Windows.Threading.DispatcherPriority.SystemIdle);
            }
            else
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    x_widebandFreq.DrawLine(obj.Content.TestFreqStart, obj.Content.FreqStep, obj.Content.SecFreqStart, obj.Content.FreqStep, obj.Content.data, (uint)obj.Content.SecDataIndex, Colors.Green, SpectrumLineType.Wave);
                }), System.Windows.Threading.DispatcherPriority.SystemIdle);
            }
        }

        void ChartView_Loaded(object sender, RoutedEventArgs e)
        {

            x_widebandFreq.Clear();

            Messenger.Default.Register<GenericMessage<BestFreqDataItem>>(this, GenericMessageAction);

        }


        private void ChartView_Unloaded(object sender, RoutedEventArgs e)
        {
            x_widebandFreq.Clear();
            Messenger.Default.Unregister<GenericMessage<BestFreqDataItem>>(this, GenericMessageAction);
        }
    }
}
