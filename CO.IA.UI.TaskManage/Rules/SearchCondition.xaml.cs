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

namespace CO.IA.UI.TaskManage.Rules
{
    /// <summary>
    /// SearchCondition.xaml 的交互逻辑
    /// </summary>
    public partial class SearchCondition : UserControl
    {
        public SearchCondition()
        {
            InitializeComponent();
            //默认 折叠起来
            Grid.SetColumnSpan(this.Tab1, 1);
            this.gridTab2.Width = new GridLength(0, GridUnitType.Pixel);
        }
        #region 伸缩菜单
        private void gridTab1_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void gridTab1_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.gridTab2.Width == new GridLength(240, GridUnitType.Pixel))
            {
                //处于展开状态
                // this.tbTip1.Text = "<<";
                Grid.SetColumnSpan(this.Tab1, 1);
                this.gridTab2.Width = new GridLength(0, GridUnitType.Pixel);
            }
            else
            {
                //处于收缩状态
                // this.tbTip1.Text = ">>";
                Grid.SetColumnSpan(this.Tab1, 2);
                this.gridTab2.Width = new GridLength(240, GridUnitType.Pixel);
            }
        }
        #endregion
    }
}
