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

namespace CO_IA.UI.Setting.Equipment
{
    /// <summary>
    /// ErrorDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ErrorDialog : Window
    {
        public ErrorDialog(string error)
        {
            InitializeComponent();

            this.txtError.Text = error;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
