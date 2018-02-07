#region 文件描述
/**********************************************************************************
 * 创建人：Liuenliang
 * 摘  要：选择人员弹出窗口
 * 日  期：2016-08-17
 * ********************************************************************************/
#endregion
using Betx.Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CO_IA.UI.PersonSchedule
{
    /// <summary>
    /// SelectPersonDialog.xaml 的交互逻辑
    /// </summary>
    public partial class SelectPersonDialog : Window
    {
        public SelectPersonDialog()
        {
            InitializeComponent();
            OrganizationViewModel vm = new OrganizationViewModel();

            vm.Initialize("6100");

            this.treeView.ItemsSource = vm.DepartmentInfoCollection;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }     /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnUserTreeViewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var element = e.OriginalSource as FrameworkElement;
            if (element == null || element.DataContext == null)
                return;
            var userInfo = element.DataContext as UserInfoSource;
            if (userInfo == null)
                return;
            //xLoginNameLookUpEdit.ClosePopup();
        }
    }
}
