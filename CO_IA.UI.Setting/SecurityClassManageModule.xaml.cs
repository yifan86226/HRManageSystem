#region 文件描述
/**********************************************************************************************************************
 * 创建人：Niext
 * 摘  要：保障类别列表
 * 日  期：2016-08-11
 * *********************************************************************************************************************/
#endregion

using CO_IA.Data;
using CO_IA.UI.Setting.SecurityType;
using I_CO_IA.Setting;
using PT_BS_Service.Client.Framework;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CO_IA.UI.Setting
{
    /// <summary>
    /// SecurityTypeList.xaml 的交互逻辑
    /// </summary>
    public partial class SecurityClassManageModule : UserControl
    {
        public SecurityClass SelectedSecurityClass
        {
            get
            {

                return securityclassgrid.SelectedItem as SecurityClass;
            }
        }

        public List<SecurityClass> ItemsSource
        {
            get;
            set;
        }

        public SecurityClassManageModule()
        {
            InitializeComponent();
            GetSecurityClass();
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void securityAdd_Click(object sender, RoutedEventArgs e)
        {
            SecurityClass newsecurityClass = new SecurityClass();
            newsecurityClass.Guid = System.Guid.NewGuid().ToString();
            SecurityClassEditDialog dialog = new SecurityClassEditDialog(newsecurityClass);
            dialog.AllowEdite = true;
            dialog.WindowTitle = "新增保障类别";
            dialog.RefreshSecurityClassEvent += () => { GetSecurityClass(); };
            dialog.ShowDialog(this);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void securityUpdate_Click(object sender, RoutedEventArgs e)
        {
            SecurityClassEditDialog dialog = new SecurityClassEditDialog(SelectedSecurityClass);
            dialog.AllowEdite = true;
            dialog.WindowTitle = "修改保障类别";
            dialog.RefreshSecurityClassEvent += () => { GetSecurityClass(); };
            dialog.ShowDialog(this);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void securityDelete_Click(object sender, RoutedEventArgs e)
        {
            if (this.SelectedSecurityClass == null)
            {
                MessageBox.Show("请先选择需要删除的保障类别");
                return;
            }
            else
            {
                MessageBoxResult msresult = MessageBox.Show("确认要删除选中的保障类别？", "提示", MessageBoxButton.YesNo);
                if (msresult == MessageBoxResult.Yes)
                {
                    try
                    {
                        BeOperationInvoker.Invoke<I_CO_IA_Setting>(channel =>
                        {
                            channel.DeleteSecurityClass(SelectedSecurityClass.Guid);
                            GetSecurityClass();
                            MessageBox.Show("删除成功!", "提示", MessageBoxButton.OK);
                        });
                    }
                    catch (System.Exception ex)
                    {
                        if (ex.Message.Contains("FK_SECRETFREQ_SECURITYCLASSES"))
                        {
                            MessageBox.Show("已经设置保密频段,不可以删除!");
                        }
                        else if (ex.Message.Contains("FK_ORGINFO_SECURITYCLASS_CODE"))
                        {
                            MessageBox.Show("已经应用为单位的保障类别,不可以删除!");
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }
        }

        private void rulsgrid_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    SecurityClassEditDialog dialog = new SecurityClassEditDialog(SelectedSecurityClass);
                    dialog.AllowEdite = true;
                    dialog.WindowTitle = "修改保障类别";
                    dialog.RefreshSecurityClassEvent += () => { GetSecurityClass(); };
                    dialog.ShowDialog(this);

                    //SecurityClassEditDialog dialog = new SecurityClassEditDialog(SelectedSecurityClass);
                    //dialog.WindowTitle = "保障类别详细信息";
                    //dialog.AllowEdite = false;
                    //dialog.ShowDialog(this);
                }
            }
        }

        public void GetSecurityClass()
        {
            securityclassgrid.ItemsSource = CO_IA.Client.Utility.GetSecurityClasses();
        }

        private void securitygrade_Click(object sender, RoutedEventArgs e)
        {
            SecurityGradeManageDialog gradedialog = new SecurityGradeManageDialog();
            gradedialog.ShowDialog(this);
        }
    }
}
