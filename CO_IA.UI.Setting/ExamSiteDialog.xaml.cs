using CO_IA.Client;
using CO_IA.Data.Setting;
using I_CO_IA.Setting;
using PT_BS_Service.Client.Framework;
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

namespace CO_IA.UI.Setting
{
    /// <summary>
    /// ExamSiteDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ExamSiteDialog : Window
    {
        /// <summary>
        /// 原始考点
        /// </summary>
        public ExamPlace ExamPlaceClass
        {
            get;
            set;
        }
        /// <summary>
        /// 当前考点
        /// </summary>
        public ExamPlace CurrentExamPlaceClass
        {
            get;
            set;
        }
        public ExamSiteDialog()
        {
            InitializeComponent();
            type = 1;
            guid = "";
        }

        public string guid = "";
        public int type = 0;//标记类型（0：插入；1：更新）
        public ExamSiteDialog(ExamPlace examPlace)
        {
            InitializeComponent();
            ExamPlaceClass = examPlace;
            //CurrentExamPlaceClass = CopyExamPlaceClass();
            this.DataContext = examPlace;
            guid = examPlace.Guid;
            type = 0;
        }
        public string WindowTitle
        {
            set { this.Title = value; }
        }
        public event Action RefreshExamPlaceEvent;
        private void btOK_Click(object sender, RoutedEventArgs e)
        {
            if (!Validated())
            {
                return;
            }
            ExamPlace examPlace = new ExamPlace();
            examPlace = GetValue();
            BeOperationInvoker.Invoke<I_CO_IA_Setting, string>(
                channel =>
                {
                    string result = channel.SaveExamPlace(examPlace);
                    if (string.IsNullOrEmpty(result))
                    {
                        MessageBox.Show("保存成功", "提示", MessageBoxButton.OK);
                        if (RefreshExamPlaceEvent != null)
                        {
                            RefreshExamPlaceEvent();
                        }
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show(result, "提示", MessageBoxButton.OK);
                    }
                    return "";
                });
        }

        private bool Validated()
        {
            bool IsSuccess = true;
            StringBuilder strmsg = new StringBuilder();
            if (string.IsNullOrEmpty(this.tbName.Text))
            {
                strmsg.Append("考点名称不能为空! \r");
                IsSuccess = false;
            }
            if (string.IsNullOrEmpty(this.tbAddress.Text))
            {
                strmsg.Append("考点地址不能为空! \r");
                IsSuccess = false;
            }
            if (string.IsNullOrEmpty(this.tbContact.Text))
            {
                strmsg.Append("联系人不能为空! \r");
                IsSuccess = false;
            }
            if (string.IsNullOrEmpty(this.tbPhone.Text))
            {
                strmsg.Append("联系电话不能为空! \r");
                IsSuccess = false;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(this.tbPhone.Text, @"^[1]+\d{10}"))
            {
                strmsg.Append("联系电话格式不正确! \r");
                IsSuccess = false;
            }
            return IsSuccess;
        }

        private ExamPlace GetValue()
        {
            ExamPlace temp = new ExamPlace();
            if (guid == "")
            {
                temp.Guid = Utility.NewGuid();
            }
            else
            {
                temp.Guid = guid;
            }
            temp.Name = this.tbName.Text;
            temp.Address = this.tbAddress.Text;
            temp.Contact = this.tbContact.Text;
            temp.Phone = this.tbPhone.Text;
            temp.Location_la = this.tbLocationLA.Text;
            temp.Location_lg = this.tbLocationLG.Text;
            temp.Remark = this.tbRemark.Text;

            return temp;
        }
    }
}
