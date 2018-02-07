using CO_IA_Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
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

namespace CO_IA.UI.StationManage
{
    /// <summary>
    /// FreqInfoControl.xaml 的交互逻辑
    /// </summary>
    public partial class FreqInfoControl : UserControl
    {
        private List<FreqInfo> freqList = new List<FreqInfo>();
        public FreqInfoControl()
        {
            InitializeComponent();
        }
        public FreqInfoControl(List<StationEmitInfo> listEmitInfo)
        {
            InitializeComponent();
            foreach (StationEmitInfo dr in listEmitInfo)
            {
                try
                {
                    FreqInfo freqInfo = new FreqInfo();

                    freqInfo.FREQ_TYPE =dr.FreqType.ToString();
                    
                    if (dr.FreqEC!=null)
                    {
                        if (dr.FreqEC >= 3)
                        {
                            freqInfo.FREQ_UC = dr.FreqEC + " M";
                        }
                        else
                        {
                            freqInfo.FREQ_UC = dr.FreqEC * 1000 + " kHz";
                        }
                    }
                    else
                    {
                        freqInfo.FREQ_UC = "";
                    }
                                       
                    if (dr.FreqEFB!=null)
                    {
                        if (dr.FreqEFB >= 3)
                        {
                            freqInfo.FREQ_EFB = dr.FreqEFB + " M";
                        }
                        else
                        {
                            freqInfo.FREQ_EFB = dr.FreqEFB * 1000 + " kHz";
                        }
                    }
                    else
                    {
                        freqInfo.FREQ_EFB = "";
                    }

                    if (dr.FreqEFE!= null)
                    {
                        if (dr.FreqEFE >= 3)
                        {
                            freqInfo.FREQ_EFE = dr.FreqEFE + " M";
                        }
                        else
                        {
                            freqInfo.FREQ_EFE = dr.FreqEFE * 1000 + " kHz";
                        }
                    }
                    else
                    {
                        freqInfo.FREQ_EFE = "";
                    }
                   
                    if (dr.FreqRFB != null)
                    {
                        if (dr.FreqRFB >= 3)
                        {
                            freqInfo.FREQ_RFB = dr.FreqRFB + " M";
                        }
                        else
                        {
                            freqInfo.FREQ_RFB = dr.FreqRFB * 1000 + " kHz";
                        }

                    }
                    else
                    {
                        freqInfo.FREQ_RFB = "";
                    }
                   
                    if (dr.FreqRFE != null)
                    {
                        if (dr.FreqRFE >= 3)
                        {
                            freqInfo.FREQ_RFE = dr.FreqRFE + " M";
                        }
                        else
                        {
                            freqInfo.FREQ_RFE = dr.FreqRFE * 1000 + " kHz";
                        }
                    }
                    else
                    {
                        freqInfo.FREQ_RFE = "";
                    }
                   
                   
                    if (dr.FreqBand != null)
                    {
                        if (dr.FreqBand >= 3)
                        {
                            freqInfo.FREQ_E_BAND = dr.FreqBand + " M";
                        }
                        else
                        {
                            freqInfo.FREQ_E_BAND = dr.FreqBand * 1000 + " kHz";
                        }
                    }
                    else
                    {
                        freqInfo.FREQ_E_BAND = "";
                    }
                    freqInfo.FREQ_R_BAND = "";
                    freqInfo.FREQ_MOD = dr.FreqMod.ToString();                    
                    freqList.Add(freqInfo);
                    //break;
                }
                catch
                {

                }
            }
            Dg_FreqInfoList.ItemsSource = freqList;
        }
        public FreqInfoControl(DataTable dataTable)
        {
            InitializeComponent();

            foreach (DataRow dr in dataTable.Rows)
            {
                try
                {
                    FreqInfo freqInfo = new FreqInfo();

                    //freqInfo.FREQ_EFB = dr["FREQ_EFB"];
                    //freqInfo.FREQ_EFE = dr["FREQ_EFE"];
                    //freqInfo.FREQ_RFB = dr["FREQ_RFB"];
                    //freqInfo.FREQ_RFE = dr["FREQ_RFE"];
                    //freqInfo.FREQ_UC = dr["FREQ_UC"];
                    if (dataTable.Columns.Contains("FREQ_TYPE"))
                    {
                        freqInfo.FREQ_TYPE = dr["FREQ_TYPE"].ToString();
                    }
                    else 
                    {
                        freqInfo.FREQ_TYPE = "";
                    }
                    double dbUC = 0;
                    if (dataTable.Columns.Contains("FREQ_UC"))
                    {
                        if (dr["FREQ_UC"].ToString() != null && dr["FREQ_UC"].ToString() != string.Empty && double.TryParse(dr["FREQ_UC"].ToString(), out  dbUC) == true)
                        {
                            if (dbUC >= 3)
                            {
                                freqInfo.FREQ_UC = dbUC + " M";
                            }
                            else
                            {
                                freqInfo.FREQ_UC = dbUC * 1000 + " kHz";

                            }

                        }
                        else
                        {
                            freqInfo.FREQ_UC = dr["FREQ_UC"].ToString();
                        }
                    }
                    else 
                    {
                        freqInfo.FREQ_UC = "";
                    }

                    double dbEFB = 0;
                    if (dataTable.Columns.Contains("FREQ_EFB"))
                    {
                        if (dr["FREQ_EFB"].ToString() != null && dr["FREQ_EFB"].ToString() != string.Empty && double.TryParse(dr["FREQ_EFB"].ToString(), out  dbEFB) == true)
                        {
                            if (dbEFB >= 3)
                            {
                                freqInfo.FREQ_EFB = dbEFB + " M";
                            }
                            else
                            {
                                freqInfo.FREQ_EFB = dbEFB * 1000 + " kHz";

                            }

                        }
                        else
                        {
                            freqInfo.FREQ_EFB = dr["FREQ_EFB"].ToString();
                        }
                    }
                    else 
                    {
                        freqInfo.FREQ_EFB = "";
                    }



                    double dbEFE = 0;
                    if (dataTable.Columns.Contains("FREQ_EFE"))
                    {
                        if (dr["FREQ_EFE"].ToString() != null && dr["FREQ_EFE"].ToString() != string.Empty && double.TryParse(dr["FREQ_EFE"].ToString(), out  dbEFE) == true)
                        {
                            if (dbEFE >= 3)
                            {
                                freqInfo.FREQ_EFE = dbEFE + " M";
                            }
                            else
                            {
                                freqInfo.FREQ_EFE = dbEFE * 1000 + " kHz";

                            }

                        }
                        else
                        {
                            freqInfo.FREQ_EFE = dr["FREQ_EFE"].ToString();
                        }
                    }
                    else
                    {
                        freqInfo.FREQ_EFE = "";
                    }

                    double dbRFB = 0;
                    if (dataTable.Columns.Contains("FREQ_RFB"))
                    {
                        if (dr["FREQ_RFB"].ToString() != null && dr["FREQ_RFB"].ToString() != string.Empty && double.TryParse(dr["FREQ_RFB"].ToString(), out  dbRFB) == true)
                        {
                            if (dbRFB >= 3)
                            {
                                freqInfo.FREQ_RFB = dbRFB + " M";
                            }
                            else
                            {
                                freqInfo.FREQ_RFB = dbRFB * 1000 + " kHz";

                            }

                        }
                        else
                        {
                            freqInfo.FREQ_RFB = dr["FREQ_RFB"].ToString();
                        }
                    }
                    else 
                    {
                        freqInfo.FREQ_RFB = "";
                    }

                    double dbRFE = 0;
                    if (dataTable.Columns.Contains("FREQ_RFE"))
                    {
                        if (dr["FREQ_RFE"].ToString() != null && dr["FREQ_RFE"].ToString() != string.Empty && double.TryParse(dr["FREQ_RFE"].ToString(), out  dbRFE) == true)
                        {
                            if (dbRFE >= 3)
                            {
                                freqInfo.FREQ_RFE = dbRFE + " M";
                            }
                            else
                            {
                                freqInfo.FREQ_RFE = dbRFE * 1000 + " kHz";

                            }
                        }
                        else
                        {
                            freqInfo.FREQ_RFE = dr["FREQ_RFE"].ToString();
                        }
                    }
                    else 
                    {
                        freqInfo.FREQ_RFE = "";
                    }
                    //freqInfo.FREQ_E_BAND = dr["FREQ_E_BAND"];
                    //freqInfo.FREQ_R_BAND = dr["FREQ_R_BAND"];

                    double db_E_BAND = 0;
                    if (dataTable.Columns.Contains("FREQ_E_BAND"))
                    {
                        if (dr["FREQ_E_BAND"].ToString() != null && dr["FREQ_E_BAND"].ToString() != string.Empty && double.TryParse(dr["FREQ_E_BAND"].ToString(), out  db_E_BAND) == true)
                        {
                            if (db_E_BAND >= 3)
                            {
                                freqInfo.FREQ_E_BAND = db_E_BAND + " M";
                            }
                            else
                            {
                                freqInfo.FREQ_E_BAND = db_E_BAND * 1000 + " kHz";

                            }

                        }
                        else
                        {
                            freqInfo.FREQ_E_BAND = dr["FREQ_E_BAND"].ToString();
                        }
                    }
                    else 
                    {
                        freqInfo.FREQ_E_BAND = "";
                    }


                    double db_R_BAND = 0;
                    if (dataTable.Columns.Contains("FREQ_R_BAND"))
                    {
                        if (dr["FREQ_R_BAND"].ToString() != null && dr["FREQ_R_BAND"].ToString() != string.Empty && double.TryParse(dr["FREQ_R_BAND"].ToString(), out  db_R_BAND) == true)
                        {
                            if (db_R_BAND >= 3)
                            {
                                freqInfo.FREQ_R_BAND = db_R_BAND + " M";
                            }
                            else
                            {
                                freqInfo.FREQ_R_BAND = db_R_BAND * 1000 + " kHz";

                            }
                        }
                        else
                        {
                            freqInfo.FREQ_R_BAND = dr["FREQ_R_BAND"].ToString();
                        }
                    }
                    else 
                    {
                        freqInfo.FREQ_R_BAND = "";
                    }
                    //freqInfo.FT_FREQ_TimeB = dr["FT_FREQ_TimeB"];
                    //freqInfo.FT_FREQ_TimeE = dr["FT_FREQ_TimeE"];

                    //freqInfo.FT_FREQ_INFO_Type = dr["FT_FREQ_INFO_Type"];
                    if (dataTable.Columns.Contains("FREQ_MOD"))
                    {
                        freqInfo.FREQ_MOD = dr["FREQ_MOD"].ToString();
                    }
                    else
                    {
                        freqInfo.FREQ_MOD ="";
                    }
                    //freqInfo.FT_FREQ_HCL = dr["FT_FREQ_HCL"];
                    freqList.Add(freqInfo);
                    //break;
                }
                catch
                {

                }
            }
            Dg_FreqInfoList.ItemsSource = freqList;

        }
    }
    public class FreqInfoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = value as string;

            string result = ""; //Canstant.GetInstance().GetDicNameByCode("00272006", str);
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FreqTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = value as string;

            if (str == "0")
            {
                return "频点";
            }
            else
            {
                return "频段";

            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class FreqInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string fREQ_EFB;

        public string FREQ_EFB
        {
            get { return fREQ_EFB; }
            set { fREQ_EFB = value; NotifyPropertyChanged("FREQ_EFB"); }
        }
        private string guid;

        public string GUID
        {
            get { return guid; }
            set { guid = value; NotifyPropertyChanged("GUID"); }
        }
        private string fREQ_EFE;

        public string FREQ_EFE
        {
            get { return fREQ_EFE; }
            set { fREQ_EFE = value; NotifyPropertyChanged("FREQ_EFE"); }
        }
        private string fREQ_RFB;

        public string FREQ_RFB
        {
            get { return fREQ_RFB; }
            set { fREQ_RFB = value; NotifyPropertyChanged("FREQ_RFB"); }
        }
        private string fREQ_RFE;

        public string FREQ_RFE
        {
            get { return fREQ_RFE; }
            set { fREQ_RFE = value; NotifyPropertyChanged("FREQ_RFE"); }
        }
        private string fT_FREQ_TimeB;

        public string FT_FREQ_TimeB
        {
            get { return fT_FREQ_TimeB; }
            set { fT_FREQ_TimeB = value; NotifyPropertyChanged("FT_FREQ_TimeB"); }
        }
        private string fT_FREQ_TimeE;

        public string FT_FREQ_TimeE
        {
            get { return fT_FREQ_TimeE; }
            set { fT_FREQ_TimeE = value; NotifyPropertyChanged("FT_FREQ_TimeE"); }
        }
        private string fREQ_E_Band;

        public string FREQ_E_BAND
        {
            get { return fREQ_E_Band; }
            set { fREQ_E_Band = value; NotifyPropertyChanged("FREQ_E_BAND"); }
        }
        private string fREQ_R_Band;

        public string FREQ_R_BAND
        {
            get { return fREQ_R_Band; }
            set { fREQ_R_Band = value; NotifyPropertyChanged("FREQ_R_BAND"); }
        }
        private string fT_FREQ_INFO_Type;

        public string FT_FREQ_INFO_Type
        {
            get { return fT_FREQ_INFO_Type; }
            set { fT_FREQ_INFO_Type = value; NotifyPropertyChanged("FT_FREQ_INFO_Type"); }
        }
        private string fREQ_MOD;

        public string FREQ_MOD
        {
            get { return fREQ_MOD; }
            set { fREQ_MOD = value; NotifyPropertyChanged("FREQ_MOD"); }
        }
        private string fT_FREQ_HCL;

        public string FT_FREQ_HCL
        {
            get { return fT_FREQ_HCL; }
            set { fT_FREQ_HCL = value; NotifyPropertyChanged("FT_FREQ_HCL"); }
        }


        private string fT_FREQ_UC;

        public string FREQ_UC
        {
            get { return fT_FREQ_UC; }
            set { fT_FREQ_UC = value; NotifyPropertyChanged("FREQ_UC"); }
        }

        private string fREQ_LC;

        public string FREQ_LC
        {
            get { return fREQ_LC; }
            set { fREQ_LC = value; NotifyPropertyChanged("FREQ_LC"); }
        }


        private string fT_FREQ_TYPE;

        public string FREQ_TYPE
        {
            get { return fT_FREQ_TYPE; }
            set { fT_FREQ_TYPE = value; NotifyPropertyChanged("FREQ_TYPE"); }
        }


        private string fT_FREQ_POW_MAX;

        public string FT_FREQ_POW_MAX
        {
            get { return fT_FREQ_POW_MAX; }
            set { fT_FREQ_POW_MAX = value; NotifyPropertyChanged("FT_FREQ_POW_MAX"); }
        }


        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,
                new PropertyChangedEventArgs(propertyName));
            }
        }


    }

}
