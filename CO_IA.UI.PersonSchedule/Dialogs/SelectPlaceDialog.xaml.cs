using CO_IA.Client;
using CO_IA.Data;
 
using I_GS_MapBase.Portal.Types;
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
using System.Windows.Shapes;

namespace CO_IA.UI.PersonSchedule
{
    /// <summary>
    /// SelectPlaceDialog.xaml 的交互逻辑
    /// </summary>
    public partial class SelectPlaceDialog : Window
    {

        public string SelectedPlace
        {
            get
            {
                if (xComboboxSite.SelectedValue == null)
                {
                    return null;
                }
                else
                {
                    return xComboboxSite.SelectedValue.ToString();
                }
            }
        }

        public event Action<string> SelectedPlaceEvent;

        public event Action<string> SelectedPlaceChangeEvent;

        private string PlaceGuid
        {
            get;
            set;
        }

        public SelectPlaceDialog()
        {
            InitializeComponent();
            xComboboxSite.ItemsSource = Utility.GetPlacesByActivityId(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
            xComboboxSite.SelectedIndex = 0;

            xComboboxSite.SelectionChanged += this.xComboboxSite_SelectionChanged;

        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPlace != null)
            {
                if (SelectedPlaceEvent != null)
                {
                    SelectedPlaceEvent(SelectedPlace);
                }
            }
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void xComboboxSite_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedPlaceChangeEvent != null)
            {
                SelectedPlaceChangeEvent(SelectedPlace);
            }
        }
    }
}
