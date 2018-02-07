#region 文件描述
/***************************************************************#@#***************************************************************
 * 创建人：xiaguohui
 * 摘 要 ：图层管理列表，控制一类绘制图层是否可见或删除
 * 日 期 ：2016-08-09
 ***************************************************************#@#***************************************************************/
#endregion
using CO_IA.UI.MAP;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CO_IA.UI.Map.Control
{
    /// <summary>
    /// LayerInfo.xaml 的交互逻辑
    /// </summary>
    public partial class LayerList : UserControl
    {
        public MapGIS mapGis = null;
        /// <summary>
        /// 存储类型
        /// </summary>
        List<string> itemList = new List<string>();
        public LayerList()
        {
            InitializeComponent();
        }
        public void Add(string title,string sKey)
        {
            if (itemList.Contains(sKey))
                return;
            itemList.Add(sKey);
            LayerItem item = new LayerItem();
            item.chk.Content = title;
            if (title == "其它")
                item.btnClose.Visibility = System.Windows.Visibility.Visible;
            item.chk.IsChecked = true;
            item.Target = sKey;
            item.Height = 28;
            item.ItemClose += ItemClose;
            item.ChkChecked += setState;
            this.stpList.Children.Add(item);

        }
        private void ItemClose(object sender)
        {            
            LayerItem item = sender as LayerItem;
            if (item != null)
            {
                if (itemList.Contains(item.Target.ToString()))
                {
                    itemList.Remove(item.Target.ToString());
                }
                if (mapGis.DrawList.Count > 0)
                {
                    var obj = mapGis.DrawList.Where(itm => itm.Key.StartsWith(item.Target.ToString() + "_")).ToList();
                    if (obj.Count > 0)
                    {
                        obj.ForEach(g => mapGis.MainMap.DefaultLayer.RemoveSymbolElement(g.Key));
                    }
                }
            }
            
            if (stpList.Children.Count == 0)
            {
                img_MouseLeftButtonUp(null,null);
            }
        }
        private void setState(object sender,bool isChecked)
        {
            LayerItem litem = sender as LayerItem;
            if (litem == null)
                return;
            if (mapGis.DrawList.Count > 0)
            {
                var item = mapGis.DrawList.Where(itm => itm.Key.StartsWith(litem.Target + "_")).ToList();
                if (item.Count > 0)
                {
                    item.ForEach(g => mapGis.MainMap.DefaultLayer.ChangeSymbolElement(g.Key, isChecked));
                }
            }            
        }
        /// <summary>
        /// 伸缩按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void img_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            string flag = img.Tag.ToString();
            if (flag == "0")
            {
                if (stpList.Children.Count == 0)
                    return;
                img.FlowDirection = System.Windows.FlowDirection.RightToLeft;

                ExecStorybord(g, FrameworkElement.WidthProperty, 0, 120, 500);
                img.Tag = "1";
                return;
            }
            if (flag == "1")
            {
                img.FlowDirection = System.Windows.FlowDirection.LeftToRight;

                ExecStorybord(g, FrameworkElement.WidthProperty, 120, 0, 500);
                img.Tag = "0";
            }
            
            
        }
        /// <summary>
        /// 执行动画效果
        /// </summary>
        /// <param name="element"></param>
        /// <param name="PropertyName"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="time"></param>
        public static void ExecStorybord(DependencyObject element, object PropertyName, double from, double to, int time, EventHandler Completed = null)
        {
            PropertyPath pro = new PropertyPath(PropertyName);
            DependencyObject obj = element;
            Storyboard sb = makestoryboard(obj, pro, from, to, time);
            if (Completed != null)
                sb.Completed += Completed;
            sb.Begin();
        }
        public static Storyboard makestoryboard(DependencyObject obj, PropertyPath pro, double from, double to, int time)
        {
            Storyboard sb = new Storyboard();
            DoubleAnimation doubleAnim = new DoubleAnimation();
            doubleAnim.Duration = new TimeSpan(0, 0, 0, 0, time);
            doubleAnim.From = from;
            doubleAnim.To = to;

            Storyboard.SetTarget(doubleAnim, obj);
            Storyboard.SetTargetProperty(doubleAnim, pro);
            sb.Children.Add(doubleAnim);
            return sb;
        }
    }
}
