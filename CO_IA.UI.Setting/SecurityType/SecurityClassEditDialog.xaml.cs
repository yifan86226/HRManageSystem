#region 文件描述
/**********************************************************************************************************************
 * 创建人：Niext
 * 摘  要：增加/修改保障类别
 * 日  期：2016-08-11
 * *********************************************************************************************************************/
#endregion

using CO_IA.Data;
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

namespace CO_IA.UI.Setting.SecurityType
{
    /// <summary>
    /// SecurityAypeInput.xaml 的交互逻辑
    /// </summary>
    public partial class SecurityClassEditDialog : Window
    {
        /// <summary>
        /// 原始保障类别对象
        /// </summary>
        private SecurityClass OriginalSecurityClass
        {
            get;
            set;
        }

        /// <summary>
        /// 当前操作的保障类别对象
        /// </summary>
        private SecurityClass CurrentSecurityClass
        {
            get;
            set;
        }

        bool allowedite = true;
        public bool AllowEdite
        {
            get { return allowedite; }
            set
            {
                allowedite = value;
                if (allowedite)
                {
                    _rectangle.Visibility = Visibility.Collapsed;
                    btOK.IsEnabled = true;
                    btCancel.IsEnabled = true;
                }
                else
                {
                    _rectangle.Visibility = Visibility.Visible;
                    btOK.IsEnabled = false;
                    btCancel.IsEnabled = false;
                }
            }
        }

        public event Action RefreshSecurityClassEvent;

        public SecurityClassEditDialog(SecurityClass securityClass)
        {
            InitializeComponent();
            OriginalSecurityClass = securityClass;
            CurrentSecurityClass = CopySecurityClass();
            this.DataContext = CurrentSecurityClass;
        }

        public string WindowTitle
        {
            set { this.Title = value; }
        }

        private void btOK_Click(object sender, RoutedEventArgs e)
        {
            if (Validated())
            {

                BeOperationInvoker.Invoke<I_CO_IA_Setting, string>(
                    channel =>
                    {
                        string result = channel.SaveSecurityClass(CurrentSecurityClass);
                        if (string.IsNullOrEmpty(result))
                        {
                            MessageBox.Show("保存成功", "提示", MessageBoxButton.OK);

                            if (RefreshSecurityClassEvent != null)
                            {
                                RefreshSecurityClassEvent();
                            }

                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show(result, "提示", MessageBoxButton.OK);
                        }
                        return result;
                    });
            }
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private bool Validated()
        {
            bool IsSuccess = true;
            StringBuilder strmsg = new StringBuilder();

            if (string.IsNullOrEmpty(CurrentSecurityClass.Name))
            {
                strmsg.Append("保障类别名称不能为空! \r");
                IsSuccess = false;
            }
            if (string.IsNullOrEmpty(CurrentSecurityClass.Comments))
            {
                strmsg.Append("保障类别描述不能为空! ");
                IsSuccess = false;
            }
            if (!string.IsNullOrEmpty(strmsg.ToString()))
            {
                MessageBox.Show(strmsg.ToString(), "提示", MessageBoxButton.OK);
            }
            return IsSuccess;
        }

        private SecurityClass CopySecurityClass()
        {
            SecurityClass securityclass = new SecurityClass();
            securityclass.Guid = OriginalSecurityClass.Guid;
            securityclass.Name = OriginalSecurityClass.Name;
            securityclass.Comments = OriginalSecurityClass.Comments;
            return securityclass;
        }
    }
}
