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
    /// ExamSiteManageModule.xaml 的交互逻辑
    /// </summary>
    public partial class ExamSiteManageModule : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector
    {
        public ExamPlace SelectExamPlace
        {
            get
            {
                return dataGridExamPlace.SelectedItem as ExamPlace;
            }
        }
        public ExamSiteManageModule()
        {
            InitializeComponent();
            GetExamPlace();
        }

        private void securityAdd_Click(object sender, RoutedEventArgs e)
        {
            ExamSiteDialog dialog = new ExamSiteDialog();
            dialog.WindowTitle = "新增考点信息";
            dialog.RefreshExamPlaceEvent += () => { GetExamPlace(); };
            dialog.ShowDialog(this);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void securityUpdate_Click(object sender, RoutedEventArgs e)
        {
            ExamSiteDialog dialog = new ExamSiteDialog(SelectExamPlace);
            dialog.WindowTitle = "编辑考点信息";
            dialog.RefreshExamPlaceEvent += () => { GetExamPlace(); };
            dialog.ShowDialog(this);
        }

        private void GetExamPlace()
        {
            dataGridExamPlace.ItemsSource = CO_IA.Client.Utility.GetExamPlace();
        }

        private void securityDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("确定删除选中的考点吗？", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    BeOperationInvoker.Invoke<I_CO_IA_Setting>(channel =>
                    {
                        channel.DeleteExamPlace(SelectExamPlace.Guid);
                        GetExamPlace();
                        MessageBox.Show("删除成功!", "提示", MessageBoxButton.OK);
                    });
                }
                catch
                { 
                
                }
            }
        }
    }
}
