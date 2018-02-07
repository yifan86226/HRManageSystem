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
using CO_IA.Data.TaskManage;


namespace CO.IA.UI.TaskManage.TaskType
{
    /// <summary>
    /// SameAsTask.xaml 的交互逻辑
    /// </summary>
    public partial class SameAsTask : UserControl
    {
        /// <summary>
        /// 原始保障类别对象
        /// </summary>
        private TaskInfo OriginalTaskInfo
        {
            get;
            set;
        }

        /// <summary>
        /// 当前操作的保障类别对象
        /// </summary>
        private TaskInfo CurrentTaskInfo
        {
            get;
            set;
        }
        public SameAsTask(TaskInfo taskInfo)
        {
            InitializeComponent();
            
        }

       

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
