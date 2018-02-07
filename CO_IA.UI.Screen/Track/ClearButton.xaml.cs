using I_GS_MapBase.Portal;
using System.Windows;
using System.Windows.Controls;

namespace CO_IA.UI.Screen.Track
{
    /// <summary>
    /// 测距关闭按钮
    /// </summary>
    public partial class ClearButton : UserControl,IFrameworkElement
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ClearButton()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public ClearButton(UserControl uc)
        {
            InitializeComponent();
            if (uc != null)
            {
                if (uc.Parent != null)
                {
                    Grid element = uc.Parent as Grid;
                    element.Children.Remove(uc);
                }
                ucType.Children.Add(uc);
            }
        }
        public string ElementId { get; set; }
        public object ElementTag { get; set; }
    }
}