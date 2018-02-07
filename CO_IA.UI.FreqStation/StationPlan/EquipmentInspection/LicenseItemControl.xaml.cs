using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace CO_IA.UI.FreqStation.StationPlan
{
    /// <summary>
    /// LicenseItemControl.xaml 的交互逻辑
    /// </summary>
    public partial class LicenseItemControl : UserControl, INotifyPropertyChanged
    {
        public event Action<LicenseItemControl> DeleteLicenseItem;

        public string PropertyName
        {
            get;
            set;
        }

        public string PropertyValue
        {
            get;
            set;
        }

        private bool isselect;
        public bool IsSelect
        {
            get
            {
                return isselect;
            }
            set
            {
                isselect = value;
                NotifyPropertyChanged("IsSelect");
            }
        }

        public LicenseItemControl()
        {
            InitializeComponent();
            this.DataContext = this;
        }


        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (DeleteLicenseItem != null)
            {
                DeleteLicenseItem(this);
            }
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IsSelect = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string proname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(proname));
            }
        }

    }
}
