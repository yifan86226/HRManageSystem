using System;
using System.Windows;
using System.Windows.Controls;

namespace CO_IA.UI.StationPlan
{
    /// <summary>
    /// FreqAssignDetailControl.xaml 的交互逻辑
    /// </summary>
    public partial class FreqAssignDetailControl : UserControl
    {
        private string assingfreq
        {
            get
            {
                return tbAssign.Text;
            }
            set
            {
                tbAssign.Text = value;
            }
        }
        public event Action<string> ConfirmEvent;

        public FreqAssignDetailControl()
        {
            InitializeComponent();
        }


        /// <summary>
        /// 确认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(assingfreq))
            {
                MessageBox.Show("请先选择建议频率");
                return;
            }
            if (ConfirmEvent != null)
            {
                ConfirmEvent(assingfreq);
            }
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            assingfreq = null;
        }
    }
}
