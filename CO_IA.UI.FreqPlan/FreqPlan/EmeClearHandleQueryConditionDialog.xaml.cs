using CO_IA.Data;
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

namespace CO_IA.UI.FreqPlan
{
    /// <summary>
    /// EmeClearHandleQueryConditionDialog.xaml 的交互逻辑
    /// </summary>
    public partial class EmeClearHandleQueryConditionDialog : Window
    {

        public bool IsSuccuessFull = false;

        EmeClearQueryCondition condition = new EmeClearQueryCondition();

        /// <summary>
        /// 
        /// </summary>
        public EmeClearQueryCondition Condition
        {
            get
            {
                return condition;
            }

            set
            {
                condition = value;
            }
        }



        public EmeClearHandleQueryConditionDialog()
        {
            InitializeComponent();

            this.DataContext = Condition;
        }



        private void BtnQuery_Click(object sender, RoutedEventArgs e)
        {
            IsSuccuessFull = true;

            //condition.ActivityGuid = Client.RiasPortal.ModuleContainer.Activity.Guid;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
