using System.Net.Mime;
using BeiLiNu.Ui.Controls.WPF.Windows;
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
using System.Windows.Shapes;
using System.Xml;
using Best.VWPlatform.Controls.Common;

namespace Best.VWPlatform.Controls.Container
{
    /// <summary>
    /// FreqRangeChoiceWnd.xaml 的交互逻辑
    /// </summary>
    public partial class FreqRangeChoiceWnd : XWindowBase
    {
        private NumericTextBoxBase _tbStart;
        private NumericTextBoxBase _tbStop;
        public FreqRangeChoiceWnd(NumericTextBoxBase pTbStart, NumericTextBoxBase pTbStop)
        {
            InitializeComponent();

            _tbStart = pTbStart;
            _tbStop = pTbStop;
            this.Loaded += FreqRangeChoiceWnd_Loaded;
        }

        Dictionary<string, List<FreqRangeModel>> _dicSource = new Dictionary<string, List<FreqRangeModel>>(); 

        void FreqRangeChoiceWnd_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\ParamConfig\\FreqRangeChoice.xml");
                var root = doc.DocumentElement;
                foreach (XmlNode node in root.SelectNodes("FreqRangeType"))
                {
                    List<FreqRangeModel> ranges = new List<FreqRangeModel>();

                    foreach (XmlNode r in node.SelectNodes("FreqRange"))
                    {
                        ranges.Add(new FreqRangeModel(r.Attributes["Range"].Value));
                    }
                    _dicSource.Add(node.Attributes["Name"].Value, ranges);
                }

               XLbType.ItemsSource = _dicSource.Keys;
            }
            catch (Exception ex)
            {
                
            }
        }

        private void XBtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void XLbType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            XLbFreqRange.ItemsSource = _dicSource[XLbType.SelectedItem.ToString()];
            XLbFreqRange.SelectedIndex = 0;
        }

        private void XBtnOK_OnClick(object sender, RoutedEventArgs e)
        {
            var v = (FreqRangeModel) XLbFreqRange.SelectedItem;
            _tbStart.NumericValue = v.StartFreq;
            _tbStop.NumericValue = v.StopFreq;

            this.Close();
        }
    }

    public class FreqRangeModel
    {
        public FreqRangeModel(string pStr)
        {
            _freqRange = pStr;
        }

        private string _freqRange;

        public string FreqRange
        {
            set { _freqRange = value; }
            get { return _freqRange; }
        }

        public string StartFreq
        {
            get { return _freqRange.Split('-')[0]; }
        }

        public string StopFreq
        {
            get { return _freqRange.Split('-')[1]; }
        }

        public string DisplayRange
        {
            get { return _freqRange + "MHz"; }
        }
    }
}
