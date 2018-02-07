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
using CO_IA.Data.TaskManage;

namespace CO.IA.UI.TaskManage.TaskType
{
    /// <summary>
    /// TaskApprove.xaml 的交互逻辑
    /// </summary>
    public partial class TaskApprove : Window
    {
        #region 属性
        public List<FrequencyrangeInfo> freList = new List<FrequencyrangeInfo>();
        public event Action AfterSaveEvent;
        private int frequency = 0;
        public int Frequency
        {
            get { return frequency; }
            set { frequency = value; }  
        }
        private FrequencyrangeInfo OriginalrequencyInfo;
        public FrequencyrangeInfo CurrentrequencyInfo{get;set;}
        public string IsAddorUpdate { get; set; }
        #endregion
        public TaskApprove()
        {
            InitializeComponent();
            InitData();
        }

        public TaskApprove(FrequencyrangeInfo frequency,string isAddorUpdate)
        {
            InitializeComponent();
            IsAddorUpdate = isAddorUpdate;
            OriginalrequencyInfo = frequency;
            InitData();
            if (frequency.FREQUENCYTYPE.ToString() != "")
            {
                if (frequency.FREQUENCYTYPE == 0)
                {
                    frequencyRange.IsChecked = true;
                }
                else
                {
                    frequencyPoint.IsChecked = false;
                }
            }
            
            CurrentrequencyInfo = TaskHelper.Clone<FrequencyrangeInfo>(frequency);
            this.DataContext = CurrentrequencyInfo;
           
        }

        #region 方法
        protected void InitData()
        {
            Mstart.DataContext = OriginalrequencyInfo;
            MEnd.DataContext = OriginalrequencyInfo;
            MWidth.DataContext = OriginalrequencyInfo;
            //List<string> unitList = new List<string>();
            //unitList.Add("MHz");
            //unitList.Add("GHz");
            //unitList.Add("kHz");
            //Mstart.ItemsSource = unitList.ToList();
            //Mstart.SelectedItem = "MHz";
            //MEnd.ItemsSource = unitList.ToList();
            //MEnd.SelectedItem = "MHz";
            //MWidth.ItemsSource = unitList.ToList();
            //MWidth.SelectedItem = "MHz";
        }

        private bool Validate(FrequencyrangeInfo frequency)
        {
            bool result = true;
            StringBuilder errmsg = new StringBuilder();
            if (string.IsNullOrEmpty(frequency.BUSINESSNAME))
            {
                errmsg.Append("业务名称不能为空! \r");
                result = false;
            }
            if (string.IsNullOrEmpty(frequency.FREQUENCYSTART))
            {
                errmsg.Append("频率/频段起始不能为空! \r");
                result = false;

            }
            if (string.IsNullOrEmpty(frequency.FREQUENCYEND))
            {
                errmsg.Append("频率/频段结束不能为空! \r");
                result = false;
            }
            if (!string.IsNullOrEmpty(errmsg.ToString()))
            {
                MessageBox.Show(errmsg.ToString(), "验证失败", MessageBoxButton.OKCancel);
            }
            return result;
        }
        #endregion

        #region 事件
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            //if (IsAddorUpdate == "0")
            //{
               
                if (!Validate(CurrentrequencyInfo)) return;
                CurrentrequencyInfo.STARTUNIT = Mstart.Text.ToString();
                CurrentrequencyInfo.ENDUNIT = MEnd.Text.ToString();
                CurrentrequencyInfo.TAPEWIDTHUNIT = MWidth.Text.ToString();
                CurrentrequencyInfo.FREQUENCYTYPE = Frequency;
              
            //}
           // freList.Add(CurrentrequencyInfo);
            //FrequencyrangeInfo fre = new FrequencyrangeInfo();
            //fre.GUID = CurrentrequencyInfo.GUID;
            //fre.MONITORGUDI = CurrentrequencyInfo.MONITORGUDI;

            //PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.TaskManage.I_CO_IA_Task>
            //   (channel =>
            //   {
            //       CurrentrequencyInfo.STARTUNIT = Mstart.Text;
            //       CurrentrequencyInfo.ENDUNIT = MEnd.Text;
            //       CurrentrequencyInfo.TAPEWIDTHUNIT = MWidth.Text;
            //       if (IsAddorUpdate == "0")
            //       {
            //           channel.SaveFrequencyrange(CurrentrequencyInfo);
            //       }
            //       else
            //       {
            //           channel.UpdateFrequencyrange(CurrentrequencyInfo);
            //       }
            //       MessageBox.Show("保存成功!", "提示", MessageBoxButton.OK);
            //   });

            if (AfterSaveEvent != null)
            {
                AfterSaveEvent();
            }
            this.Close();
        }

        #endregion

        private void frequencyRange_Click_1(object sender, RoutedEventArgs e)
        {
            if (frequencyRange.IsChecked == true)
            {
                Frequency = 0;
            }
            else
            {
                Frequency = 1;
            }
        }

        private void btnGoon_Click(object sender, RoutedEventArgs e)
        {
            if (!Validate(CurrentrequencyInfo)) return;
            CurrentrequencyInfo.STARTUNIT = Mstart.Text;
            CurrentrequencyInfo.ENDUNIT = MEnd.Text;
            CurrentrequencyInfo.TAPEWIDTHUNIT = MWidth.Text;
            CurrentrequencyInfo.FREQUENCYTYPE = Frequency;

            ServiceSelection.Text = "";
            frequencyStart.Text = "";
            frequencyEnd.Text = "";
            tapeWidth.Text = "";
            //if (!Validate(CurrentrequencyInfo)) return;
            //PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.TaskManage.I_CO_IA_Task>
            //   (channel =>
            //   {
            //       CurrentrequencyInfo.STARTUNIT = Mstart.Text;
            //       CurrentrequencyInfo.ENDUNIT = MEnd.Text;
            //       CurrentrequencyInfo.TAPEWIDTHUNIT = MWidth.Text;
            //      bool result= channel.SaveFrequencyrange(CurrentrequencyInfo);
            //      if (result == true)
            //      {
            //          ServiceSelection.Text = "";
            //          frequencyStart.Text = "";
            //          frequencyEnd.Text = "";
            //          tapeWidth.Text = "";
            //          //MessageBox.Show("保存成功!", "提示", MessageBoxButton.OK);
            //      }
            //   });

            if (AfterSaveEvent != null)
            {
                AfterSaveEvent();
            }
        }
    }

}
