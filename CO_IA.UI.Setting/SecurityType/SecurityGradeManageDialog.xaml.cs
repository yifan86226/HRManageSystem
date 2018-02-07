using CO_IA.Data;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CO_IA.Client;
using System;
using CO_IA.Types;
namespace CO_IA.UI.Setting
{
    /// <summary>
    /// SecurityGradeManageControl.xaml 的交互逻辑
    /// </summary>
    public partial class SecurityGradeManageDialog : Window
    {
        private SecurityGrade SelectedSecurityGrade
        {
            get
            {
                return securitygradegrid.SelectedItem as SecurityGrade;
            }
            set
            {
                securitygradegrid.SelectedItem = value;
            }
        }

        private SecurityGrade[] OriginalSource
        {
            get;
            set;
        }

        public ObservableCollection<SecurityGrade> SecurityGradeItemsSource
        {
            get { return (ObservableCollection<SecurityGrade>)GetValue(SecurityGradeItemsSourcePropertyProperty); }
            set { SetValue(SecurityGradeItemsSourcePropertyProperty, value); }
        }

        public static readonly DependencyProperty SecurityGradeItemsSourcePropertyProperty =
            DependencyProperty.Register("SecurityGradeItemsSource", typeof(ObservableCollection<SecurityGrade>), typeof(SecurityGradeManageDialog),
            new PropertyMetadata(null));

        public SecurityGradeManageDialog()
        {
            InitializeComponent();
            this.DataContext = this;
            GetSecurityGrade();
        }

        private void GetSecurityGrade()
        {
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.Setting.I_CO_IA_Setting>(channel =>
            {
                var result = channel.GetSecurityGrade();
                if (result != null && result.Length > 0)
                {
                    SecurityGradeItemsSource = new ObservableCollection<SecurityGrade>(result);
                }
                else
                {
                    SecurityGradeItemsSource = new ObservableCollection<SecurityGrade>();
                }
                SecurityGradeItemsSource.Add(NewSecurityGrade());
            });
        }

        private SecurityGrade NewSecurityGrade()
        {
            return new SecurityGrade() { Guid = System.Guid.NewGuid().ToString() };
        }

        private void securitygradegrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
            (e.Row.Item as SecurityGrade).Orders = e.Row.GetIndex() + 1;
        }

        private void securitygradegrid_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                SecurityGradeItemsSource.Add(NewSecurityGrade());
            }
        }
        /// <summary>
        /// 上移
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btUp_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(SelectedSecurityGrade.Name))
            {
                int index = SecurityGradeItemsSource.IndexOf(SelectedSecurityGrade);
                if (index > 0)
                {
                    SecurityGrade upsecuritygrade = SecurityGradeItemsSource[index - 1];
                    SecurityGradeItemsSource.Insert(index - 1, SelectedSecurityGrade);
                    SecurityGradeItemsSource.Insert(index, upsecuritygrade);

                    SecurityGradeItemsSource.RemoveAt(index + 1);
                    SecurityGradeItemsSource.RemoveAt(index + 1);

                    SelectedSecurityGrade = SecurityGradeItemsSource[index - 1];
                }
            }
        }

        /// <summary>
        /// 下移
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btDown_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(SelectedSecurityGrade.Name))
            {
                int index = SecurityGradeItemsSource.IndexOf(SelectedSecurityGrade);
                if (index < SecurityGradeItemsSource.Count - 1)
                {
                    SecurityGrade downsecuritygrade = SecurityGradeItemsSource[index + 1];

                    if (downsecuritygrade.Name != null)
                    {
                        SecurityGradeItemsSource.Insert(index + 1, SelectedSecurityGrade);
                        SecurityGradeItemsSource.Insert(index, downsecuritygrade);

                        SecurityGradeItemsSource.RemoveAt(index + 1);
                        SecurityGradeItemsSource.RemoveAt(index + 2);
                    }
                }
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.Setting.I_CO_IA_Setting>(channel =>
                {
                    SecurityGrade[] grades = new SecurityGrade[SecurityGradeItemsSource.Count];
                    SecurityGradeItemsSource.CopyTo(grades, 0);
                    channel.SaveSecurityGrade(grades);
                });
                MessageBox.Show("保存成功!", "提示", MessageBoxButton.OK);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.GetExceptionMessage(), "提示", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("确定要删除选中的保障等级!", "提示", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.Setting.I_CO_IA_Setting>(channel =>
                    {
                        channel.DeleteSecurityGrade(SelectedSecurityGrade.Guid);
                    });

                    GetSecurityGrade();

                    MessageBox.Show("删除成功!", "提示", MessageBoxButton.OK);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.GetExceptionMessage(), "提示", MessageBoxButton.OK);
                }
            }
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            GetSecurityGrade();
        }
    }
}
