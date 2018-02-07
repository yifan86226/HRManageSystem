using CO_IA.Client;
using CO_IA.Data;
using CO_IA.UI.FreqQuery.FreqAnalyse;
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

namespace CO_IA.UI.FreqQuery
{
    /// <summary>
    /// FreqQueryModule.xaml 的交互逻辑
    /// </summary>
    public partial class FreqQueryModule : UserControl
    {
        private Activity CurrentActivity = CO_IA.Client.RiasPortal.ModuleContainer.Activity;
        private string CurrentPalceGuid
        {
            get { return xComboboxPlace.SelectedValue as string; }
        }


        public FreqQueryModule()
        {
            InitializeComponent();
            xComboboxPlace.ItemsSource = Utility.GetPlaces(CurrentActivity.Guid);
            this.xComboboxPlace.SelectedIndex = 0;
            this.listBoxMenu.SelectedIndex = 0;
        }

        private void listBoxMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.borderContent.Child = null;
            switch (listBoxMenu.SelectedIndex)
            {
                //频谱规划表
                case 0:
                    this.borderContent.Visibility = System.Windows.Visibility.Visible;
                    FreqPlanMangeControl freqplan = new FreqPlanMangeControl();
                    freqplan.PlaceGuid = this.CurrentPalceGuid;
                    this.borderContent.Child = freqplan;
                    break;

                //频率占用
                case 1:
                    this.borderContent.Visibility = System.Windows.Visibility.Visible;
                    FreqAnalyseControl freqanalyse = new FreqAnalyseControl();
                    freqanalyse.PlaceGuid = this.CurrentPalceGuid;
                    this.borderContent.Child = freqanalyse;
                    break;
                //参会台站
                case 2:
                    this.borderContent.Visibility = System.Windows.Visibility.Visible;
                    StationQueryControl stationquerycontrol = new StationQueryControl();
                    stationquerycontrol.PlaceGuid = this.CurrentPalceGuid;
                    this.borderContent.Child = stationquerycontrol;
                    break;
            }
        }

        /// <summary>
        /// 切换地点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void xComboboxPlace_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.borderContent.Child != null)
            {
                UserControl content = borderContent.Child as UserControl;

                if (content.GetType() == typeof(FreqPlanMangeControl))
                {
                    (borderContent.Child as FreqPlanMangeControl).PlaceGuid = CurrentPalceGuid;
                }
                else if (content.GetType() == typeof(FreqAnalyseControl))
                {
                    (borderContent.Child as FreqAnalyseControl).PlaceGuid = CurrentPalceGuid;
                }

                else if (content.GetType() == typeof(StationQueryControl))
                {
                    (borderContent.Child as StationQueryControl).PlaceGuid = CurrentPalceGuid;
                }
            }
        }
    }
}
