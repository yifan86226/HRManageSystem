using CO_IA.Data;
using I_CO_IA.FreqPlan;
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
    /// FreqQueryControl.xaml 的交互逻辑
    /// </summary>
    public partial class FreqPlanQueryDialog : Window
    {
        public string ActivityGuid { get; set; }

        public string PlaceGuid { get; set; }

        public List<string> BusinessCodes
        {
            get;
            set;
        }

        public event Action<List<string>> OnQueryEvent;

        public FreqPlanQueryDialog(string activityGuid, string placeGuid)
        {
            InitializeComponent();
            ActivityGuid = activityGuid;
            PlaceGuid = placeGuid;
            _lstbusinesstype.ItemsSource = FreqQueryHelper.GetActivityBusinessType(ActivityGuid, PlaceGuid);
        }

        private void BtnQuery_Click(object sender, RoutedEventArgs e)
        {
            BusinessCodes = new List<string>();
            foreach (BusinessType item in _lstbusinesstype.SelectedItems)
            {
                BusinessCodes.Add(item.Guid);
            }

            if (OnQueryEvent != null)
            {
                OnQueryEvent(BusinessCodes);
            }
            this.Close();
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
