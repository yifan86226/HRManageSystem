using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CO_IA.Data;
using CO_IA.Data.Monitor;
using CO_IA.Data.MonitorPlan;

namespace CO_IA.UI.MonitorPlan.MonitorDialog
{
    /// <summary>
    /// PersonSelectedDialog.xaml 的交互逻辑
    /// </summary>
    public partial class WorkerSelectedDialog : Window
    {
         List<PP_PersonInfo> pp_orgInfoList = new List<PP_PersonInfo>();
         List<Sealed_PP_OrgInfo> pp_List = new List<Sealed_PP_OrgInfo>();
        /// <summary>
        /// 绑定TreeView的数据源
        /// </summary>
        private List<Sealed_PP_OrgInfo> _personGroupList = null;
       // public event Action<List<PP_PersonInfo>> OKButtonClick;
        public event Action<List<Sealed_PP_OrgInfo>> OKButtonClick;
        public WorkerSelectedDialog(List<Sealed_PP_OrgInfo> p_personGroupList)
        {
            InitializeComponent();
            _personGroupList = p_personGroupList;
            LoadedTreeView();
        }
        public void LoadedTreeView()
        {
            _treeView.Items.Clear();
            var groupList = _personGroupList.GroupBy(p => p.NAME).ToList();//进行分组操作
            foreach (var group in groupList)
            {
                var personList = group.ToList();
                var groupTree = new TreeViewItem();
                if (group == groupList[0])
                {
                    groupTree.IsExpanded = true;
                }
                groupTree.DataContext = personList;
                var groupCBox = new RadioButton();
                groupCBox.Content = personList[0].NAME;
                groupCBox.DataContext = groupTree;
                groupCBox.Checked += GroupCBox_Checked;
                groupCBox.Unchecked += GroupCBox_UnChecked;
                groupTree.Header = groupCBox;
                _treeView.Items.Add(groupTree);
                foreach (PP_PersonInfo personInfo in personList[0].Persons)
                {
                    var personTree = new TreeViewItem();
                    personTree.DataContext = personInfo;
                    TextBlock tb = new TextBlock();
                    tb.Text = personInfo.NAME;
                    tb.DataContext = personTree;
                    personTree.Header = tb;
                    groupTree.Items.Add(personTree);
                }
            }
           
        }

        private void ForeachPropertyNode(Sealed_PP_OrgInfo node, string pid, List<Sealed_PP_OrgInfo> nodes)
        {
            foreach (Sealed_PP_OrgInfo tempNode in nodes)
            {
                if (tempNode.PARENT_GUID == pid)
                {
                    ForeachPropertyNode(tempNode, tempNode.GUID, nodes);
                    node.Children.Add(tempNode);
                }
            }
        }
        /// <summary>
        /// 取消选择中小组
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GroupCBox_UnChecked(object sender, RoutedEventArgs e)
        {
            var cBox = sender as RadioButton;
            var item = cBox.DataContext as TreeViewItem;
            if ((item.Header as RadioButton).IsChecked == true)
            {

                var pplist = item.DataContext as List<Sealed_PP_OrgInfo>;

                foreach (Sealed_PP_OrgInfo parentitem in pplist)
                {
                    pp_List.Remove(parentitem);
                }
            }
        }
        /// <summary>
        /// 选择中小组
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GroupCBox_Checked(object sender, RoutedEventArgs e)
        {
            pp_List.Clear();//单选按钮，先清空下
            var cBox = sender as RadioButton;
            var item = cBox.DataContext as TreeViewItem;
            if ((item.Header as RadioButton).IsChecked == true)
            {
                var pplist = item.DataContext as List<Sealed_PP_OrgInfo>;
                foreach (Sealed_PP_OrgInfo parentitem in pplist)
                {
                    pp_List.Add(parentitem);
                }
            }
        }
        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (OKButtonClick != null)
            {
                OKButtonClick(pp_List);
            }
            Close();
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void tv_PersonPlan_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

        }

    }
}
