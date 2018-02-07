using CO_IA.Data;
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

namespace CO_IA.UI.Screen.Audio
{
    /// <summary>
    /// AudioControl.xaml 的交互逻辑
    /// </summary>
    public partial class AudioControl : UserControl
    {
        public Action<bool,string> Call = null;
        private PP_PersonInfo personData;
        public PP_PersonInfo PersonData
        {
            get { return personData; }
            set { personData = value; g.DataContext = PersonData; }
        }
        public bool Called = false;
        //Action<bool> RedButtonClick=null;
        //Action<bool> GreenButtonClick = null;
        //private Person_Info_Ext personInfo = null;
        //public Person_Info_Ext PersonInfo
        //{
        //    get
        //    {
        //        return personInfo;
        //    }
        //    set
        //    {
        //        personInfo = value;
        //        g.DataContext = PersonInfo;
        //    }
        //}
        public AudioControl()
        {
            InitializeComponent();
            //PersonInfo = pInfo;
            
        }

        //bool Flag
        //{
        //    get;
        //    set;
        //}
        private void RedEllipse_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //PersonInfo.Flag = true;
            //if (RedButtonClick != null)
            //{
            //    RedButtonClick(PersonInfo.Flag);
            //}
        }

        private void GreenEllipse_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //PersonInfo.Flag = false;
            //if (GreenButtonClick != null)
            //{
            //    GreenButtonClick(PersonInfo.Flag);
            //}
        }

        private void OnImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (imgOn.Opacity != 1)
                return;
            if (Call != null&&PersonData!=null)
                Call(true,PersonData.VOICEID);
        }
        public void ChangeBtnState(bool flag, bool state)
        {
            if (flag)//是否成功
            {
                if (state)
                {
                    imgOff.Visibility = System.Windows.Visibility.Visible;
                    imgOn.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    imgOff.Visibility = System.Windows.Visibility.Collapsed;
                    imgOn.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }
        //public void ChangeBtnState(bool flag)
        //{
        //    if (flag)
        //    {
        //        imgOff.Visibility = System.Windows.Visibility.Visible;
        //        imgOn.Visibility = System.Windows.Visibility.Collapsed;
        //    }
        //    else
        //    {
        //        imgOff.Visibility = System.Windows.Visibility.Collapsed;
        //        imgOn.Visibility = System.Windows.Visibility.Visible;
        //    }
        //}
        private void OffImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            imgOn.Visibility = System.Windows.Visibility.Visible;
            imgOff.Visibility = System.Windows.Visibility.Collapsed;
            if (Call != null)
                Call(false,PersonData.VOICEID);
        }

        private void g_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            imgOn.Opacity = 0.5;
            if(Called)
                return;
            imgOn.Opacity = 0.5;
            if (PersonData == null)
                return;
            if (string.IsNullOrEmpty(PersonData.VOICEID))
            {
                imgOn.Opacity = 0.5;
            }
            else
            {
                imgOn.Opacity = 1;
            }
        }
    }
    public class Person_Info_Ext:PP_PersonInfo
    {
        private bool flag;
        public bool Flag
        {
            get
            {
                return flag;
            }
            set {
                flag = value;
                NotifyPropertyChanged("ISCHECKED"); 
            }
        }
    }
    public class AudioFlagConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double v = 0.2;
            string para = (string)parameter;
            var person = (Person_Info_Ext)value;
            if (person != null)
            {
                if (para == "0")//红
                {
                    v = person.Flag == true ? 0.2 : 1;
                }
                if (para == "1")//绿
                {
                    v = person.Flag == true ? 1 : 0.2;
                }
                if (string.IsNullOrEmpty(person.VOICEID))
                    v = 0.2;
            }
            return v;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
