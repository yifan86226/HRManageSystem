using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
    /// EquInfoControl.xaml 的交互逻辑
    /// </summary>
    public partial class EquInfoControl : UserControl
    {
        private List<EquInfo> equList = new List<EquInfo>();
        public EquInfoControl()
        {
            InitializeComponent();
        }
        public EquInfoControl(DataTable dataTable, DataTable eDT)
        {
            InitializeComponent();

            foreach (DataRow dr in dataTable.Rows)
            {
                try
                {
                    EquInfo equInfo = new EquInfo();
                    if (dataTable.Columns.Contains("EQU_MODEL"))
                    {
                        equInfo.EQU_MODEL = dr["EQU_MODEL"].ToString();
                    }
                    else 
                    {
                        equInfo.EQU_MODEL = "";
                    }
                    if (dataTable.Columns.Contains("EQU_AUTH"))
                    {
                        equInfo.EQU_AUTH = dr["EQU_AUTH"].ToString();
                    }
                    else 
                    {
                        equInfo.EQU_AUTH = "";
                    }
                    if (dataTable.Columns.Contains("EQU_CODE"))
                    {
                        equInfo.EQU_CODE = dr["EQU_CODE"].ToString();
                    }
                    else 
                    {
                        equInfo.EQU_CODE = "";
                    }
                    if (dataTable.Columns.Contains("EQU_MENU"))
                    {
                        equInfo.EQU_MENU = dr["EQU_MENU"].ToString();
                    }
                    else
                    {
                        equInfo.EQU_MENU = "";
                    }
                    if (dataTable.Columns.Contains("EQU_POW"))
                    {
                        equInfo.EQU_POW = dr["EQU_POW"].ToString();
                    }
                    else
                    {
                        equInfo.EQU_POW ="";
                    }



                    try
                    {
                        if (string.IsNullOrEmpty(dr["EQU_POW"].ToString()) == true && dr["GUID"].ToString() != null)
                        {

                            //var names = from s in eDT.Rows
                            //            where s["GUID"] == dr["GUID"] //调用对象的方法
                            //            select s["ET_EQU_DPOW"];

                            //foreach (string name in names)//循环输出结果
                            //{
                            //    equInfo.EQU_POW = name;
                            //    break;
                            //}
                        }
                    }
                    catch
                    {

                    }
                    equList.Add(equInfo);
                }
                catch
                {

                }
            }
            Dg_EquInfoList.ItemsSource = equList;
        }
    }
    public class EquInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string eQU_Model;

        public string EQU_MODEL
        {
            get { return eQU_Model; }
            set { eQU_Model = value; NotifyPropertyChanged("EQU_MODEL"); }
        }
        private string eQU_AUTH;

        public string EQU_AUTH
        {
            get { return eQU_AUTH; }
            set { eQU_AUTH = value; NotifyPropertyChanged("EQU_AUTH"); }
        }
        private string eQU_Code;

        public string EQU_CODE
        {
            get { return eQU_Code; }
            set { eQU_Code = value; NotifyPropertyChanged("EQU_CODE"); }
        }
        private string eQU_MENU;

        public string EQU_MENU
        {
            get { return eQU_MENU; }
            set { eQU_MENU = value; NotifyPropertyChanged("EQU_MENU"); }
        }
        private string eQU_POW;

        public string EQU_POW
        {
            get { return eQU_POW; }
            set { eQU_POW = value; NotifyPropertyChanged("EQU_POW"); }
        }

        private string eT_EQU_SEN;

        public string ET_EQU_SEN
        {
            get { return eT_EQU_SEN; }
            set { eT_EQU_SEN = value; NotifyPropertyChanged("ET_EQU_SEN"); }
        }
        private string eT_EQU_SENU;

        public string ET_EQU_SENU
        {
            get { return eT_EQU_SENU; }
            set { eT_EQU_SENU = value; NotifyPropertyChanged("ET_EQU_SENU"); }
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
