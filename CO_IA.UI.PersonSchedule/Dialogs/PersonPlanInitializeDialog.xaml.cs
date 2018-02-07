#region 文件描述
/**********************************************************************************
 * 创建人：Liuenliang
 * 摘  要：人员预案初始化弹出窗口
 * 日  期：2016-08-17
 * ********************************************************************************/
#endregion
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

namespace CO_IA.UI.PersonSchedule
{
    /// <summary>
    /// PersonPlanInitializeDialog.xaml 的交互逻辑
    /// </summary>
    public partial class PersonPlanInitializeDialog : Window
    {
        public PersonPlanInitializeDialog()
        {
            InitializeComponent();

            #region 初始结构
            List<TreeNode> nodes = new List<TreeNode>()
            {
                new TreeNode { ID = 1, Name = "重大活动安全保障办公室", Type = 0 },
                new TreeNode { ID = 2, Name = "监测组", Type = 0 , ParentID = 1 },
                new TreeNode { ID = 3, Name = "频率组", Type = 0 , ParentID = 1 },
                new TreeNode { ID = 4, Name = "台站组", Type = 0 , ParentID = 1 },
                new TreeNode { ID = 5, Name = "检测组", Type = 0 , ParentID = 1},
                new TreeNode { ID = 6, Name = "第一小组", Type = 0 , ParentID = 2 },
                new TreeNode { ID = 7, Name = "第二小组", Type = 0 , ParentID = 2 },
                new TreeNode { ID = 8, Name = "第三小组", Type = 0 , ParentID = 2 },
                new TreeNode { ID = 9, Name = "第一小组", Type = 0 , ParentID = 3 },
                new TreeNode { ID = 10, Name = "第一小组", Type = 0 , ParentID = 4 },
                new TreeNode { ID = 11, Name = "第一小组", Type = 0 , ParentID = 5 }
            };

            AddTreeNode(-1, null, nodes, tv_PersonPlan);
            #endregion


            #region  初始化人员
            List<TreeNode> nodes1 = new List<TreeNode>()
            {
                new TreeNode { ID = 1, Name = "陕西无线电管理局", Type = 0 },
                new TreeNode { ID = 2, Name = "监测处", Type = 0 , ParentID = 1 },
                new TreeNode { ID = 3, Name = "综合处", Type = 0 , ParentID = 1 },
                new TreeNode { ID = 4, Name = "西安市无线电管理局", Type = 0 , ParentID = 1 },
                new TreeNode { ID = 5, Name = "咸阳市无线电管理局", Type = 0 , ParentID = 1},
                new TreeNode { ID = 6, Name = "王武", Type = 1 , ParentID = 2 },
                new TreeNode { ID = 7, Name = "刘三", Type = 1 , ParentID = 2 },
                new TreeNode { ID = 8, Name = "李斯", Type = 1 , ParentID = 2 },
                new TreeNode { ID = 9, Name = "刘洋", Type = 1 , ParentID = 3 },
                new TreeNode { ID = 10, Name = "李密", Type = 1 , ParentID = 3},
                new TreeNode { ID = 11, Name = "安国强", Type = 1 , ParentID = 3 },
                new TreeNode { ID = 11, Name = "察补斯", Type = 1 , ParentID = 3 },
                new TreeNode { ID = 11, Name = "刘胜", Type = 1 , ParentID = 4 },
                new TreeNode { ID = 11, Name = "李咸平", Type = 1 , ParentID = 4 },
                new TreeNode { ID = 11, Name = "郭大为", Type = 1 , ParentID = 4 },
                new TreeNode { ID = 11, Name = "邵春华", Type = 1 , ParentID = 5 },
                new TreeNode { ID = 11, Name = "刘想", Type = 1 , ParentID = 5 },
                new TreeNode { ID = 11, Name = "黎明当", Type = 1 , ParentID = 5 },
                new TreeNode { ID = 11, Name = "郭吴", Type = 1 , ParentID = 5 }
            };

            AddTreeNode(-1, null, nodes1, tv_PersonList);
            #endregion



            #region  初始化车辆
            List<TreeNode> vehiclenodes = new List<TreeNode>()
            {
                new TreeNode { ID = 1, Name = "监测车辆1", Type = 3 },
                new TreeNode { ID = 2, Name = "监测车辆2", Type = 3 },
                new TreeNode { ID = 3, Name = "监测车辆3", Type = 3  },
                new TreeNode { ID = 4, Name = "监测车辆4", Type = 3  },
                new TreeNode { ID = 5, Name = "指挥车辆1", Type = 3 },
                new TreeNode { ID = 6, Name = "指挥车辆2", Type = 3  },
                new TreeNode { ID = 7, Name = "指挥车辆3", Type = 3  },
                new TreeNode { ID = 8, Name = "检测车辆1", Type = 3  },
                new TreeNode { ID = 9, Name = "检测车辆2", Type = 3  }
            };

            AddTreeNode(-1, null, vehiclenodes, tv_VehicleList);
            #endregion


            #region  初始化设备
            List<TreeNode> equipnodes = new List<TreeNode>()
            {
                new TreeNode { ID = 1, Name = "监测设备1", Type = 4 },
                new TreeNode { ID = 2, Name = "监测设备2", Type = 4 },
                new TreeNode { ID = 3, Name = "监测设备3", Type = 4  },
                new TreeNode { ID = 4, Name = "监测设备4", Type = 4  },
                new TreeNode { ID = 5, Name = "检测测设1", Type = 4 },
                new TreeNode { ID = 6, Name = "检测测设2", Type = 4  },
                new TreeNode { ID = 7, Name = "检测测设3", Type = 4 }
            };

            AddTreeNode(-1, null, equipnodes, tv_EquipList);
            #endregion


        }

        #region
        //private void AddTreeNodePeron(int parentId, TreeViewItem treeViewItem, List<TreeNode> nodes)
        //{
        //    List<TreeNode> tree = (from li in nodes
        //                           where li.ParentID == parentId
        //                           select li).ToList<TreeNode>();
        //    if (tree.Count > 0)
        //    {
        //        foreach (TreeNode node in tree)
        //        {
        //            TreeViewItem tvi = new TreeViewItem();
        //            tvi.Header = node.Name;
        //            tvi.DataContext = node;
        //            tvi.IsExpanded = true;
        //            if (treeViewItem == null)
        //            {
        //                this.tv_PersonList.Items.Add(tvi);
        //            }
        //            else
        //            {
        //                treeViewItem.Items.Add(tvi);
        //            }
        //            AddTreeNodePeron(node.ID, tvi, nodes);
        //        }
        //    }
        //}



        //public static void CreatTreeView(TreeView _myTreeView, myEx_TreeViewItem _parentNode, DataTable dt, bool isRoot = true, bool isActivity = true, int Depth = 3)
        //{
        //    string myFilterString = isRoot ? "ParentID = 0" : "ParentID = " + _parentNode.ID.ToString();
        //    if (isActivity)
        //    {
        //        #region 动态加载节点
        //        if (Depth > 0)
        //        {
        //            foreach (DataRow row in dt.Select(myFilterString))
        //            {
        //                myEx_TreeViewItem item = new myEx_TreeViewItem();
        //                item.ID = (int)row["ID"];
        //                item.Depth = (int)row["Depth"];
        //                item.ItemContent = row["NodeName"].ToString();
        //                item.ParentID = (int)row["ParentID"];
        //                item.ItemPath = row["NodePath"].ToString();
        //                item.IconPath = row["IconPath"].ToString();
        //                item.IsExpanded = false;
        //                if (isRoot)
        //                {
        //                    _myTreeView.Items.Clear();  //加载根节点前先清除Treeview控件项
        //                    item.IsExpanded = true;
        //                    _myTreeView.Items.Add(item); //新增根节点
        //                }
        //                else
        //                {
        //                    item.IsExpanded = false;
        //                    _parentNode.Items.Add(item); //新增下级节点
        //                }
        //                CreatTreeView(_myTreeView, item, dt, false, true, Depth - 1);
        //            }
        //        }
        //        #endregion
        //    }
        //    else
        //    {
        //        #region 递归生成树
        //        foreach (DataRow row in dt.Select(myFilterString))
        //        {
        //            myEx_TreeViewItem item = new myEx_TreeViewItem();
        //            item.ID = (int)row["ID"];
        //            item.Depth = (int)row["Depth"];
        //            item.ItemContent = row["NodeName"].ToString();
        //            item.ParentID = (int)row["ParentID"];
        //            item.ItemPath = row["NodePath"].ToString();
        //            item.IconPath = row["IconPath"].ToString();
        //            if (isRoot)
        //            {
        //                _myTreeView.Items.Clear();
        //                item.IsExpanded = true;
        //                _myTreeView.Items.Add(item); //新增根节点
        //            }
        //            else
        //            {
        //                item.IsExpanded = false;
        //                _parentNode.Items.Add(item); //新增下级节点
        //            }
        //            CreatTreeView(_myTreeView, item, dt, false);
        //        }
        //        #endregion
        //    }
        //}





        //public static void CreatTreeViewItemChild(TreeView _myTreeView, myEx_TreeViewItem _SelectedParentItem, DataTable dt, int Depth = 2)
        //{
        //    _SelectedParentItem.Items.Clear();

        //    if (Depth > 0)  //判断加载的深度，默认只加载两级
        //    {
        //        //设定过滤条件
        //        string myFilterString;
        //        myFilterString = "ParentID = " + _SelectedParentItem.ID.ToString();

        //        foreach (DataRow row in dt.Select(myFilterString))
        //        {
        //            myEx_TreeViewItem item = new myEx_TreeViewItem();
        //            item.ID = (int)row["ID"];
        //            item.Depth = (int)row["Depth"];
        //            item.ItemContent = row["NodeName"].ToString();
        //            item.ParentID = (int)row["ParentID"];
        //            item.ItemPath = row["NodePath"].ToString();
        //            item.IconPath = row["IconPath"].ToString();
        //            item.IsExpanded = false;
        //            _SelectedParentItem.Items.Add(item); //新增下级节点
        //            CreatTreeViewItemChild(_myTreeView, item, dt, (Depth - 1));
        //        }
        //    }
        //}

        #endregion


        private void AddTreeNode(int parentId, TreeViewItem treeViewItem, List<TreeNode> nodes, TreeView treeView)
        {
            List<TreeNode> tree = (from li in nodes
                                   where li.ParentID == parentId
                                   select li).ToList<TreeNode>();
            if (tree.Count > 0)
            {
                foreach (TreeNode node in tree)
                {
                    TreeViewItem tvi = new TreeViewItem();
                    tvi.Header = node.Name;
                    tvi.DataContext = node;
                    tvi.IsExpanded = true;
                    if (treeViewItem == null)
                    {
                        treeView.Items.Add(tvi);
                    }
                    else
                    {
                        treeViewItem.Items.Add(tvi);
                    }
                    AddTreeNode(node.ID, tvi, nodes, treeView);
                }
            }
        }


        private TreeViewItem selectTreeItme = new TreeViewItem();

        private int tempid = 100;

        private void btnAddItem_Click(object sender, RoutedEventArgs e)
        {
            //TreeNode treenode = new TreeNode { ID = tempid, Name = "第二小组", Type = 0, ParentID = 5 };
            tempid++;
            //selectTreeItme.Items.Add(treenode);

            TreeNode treenode = new TreeNode { ID = tempid, Name = "待重命名" + tempid, Type = 0, ParentID = 5 };//实例化节点对象

            if (this.tv_PersonPlan.SelectedItem == null)//检查是否有节点选中
            {
                TreeViewItem tvi = new TreeViewItem();
                tvi.Header = treenode.Name;
                tvi.DataContext = treenode;
                tvi.IsExpanded = true;
                treenode.ParentID = -1;
                //nodeName = nodeName + this.tv_PersonPlan.Items.Count;

                this.tv_PersonPlan.Items.Add(tvi);//向treeview中添加根节点
            }
            else
            {//有节点选中,添加当前选中节点的子节点


                TreeViewItem tvi = new TreeViewItem();
                tvi.Header = treenode.Name;
                tvi.DataContext = treenode;
                tvi.IsExpanded = true;

                //treenode.ParentID = tv_PersonPlan.SelectedItem

                ((TreeViewItem)this.tv_PersonPlan.SelectedItem).Items.Add(tvi);//向当前选中节点的节点集合中添加node
            }


        }

        private void btnMove_Click(object sender, RoutedEventArgs e)
        {
            if (this.tv_PersonPlan.SelectedItem == null)//检查是否有节点选中
            {

            }
            else
            {//有节点选中,添加当前选中节点的子节点




                if (((TreeViewItem)this.tv_PersonPlan.SelectedItem).Parent.GetType() == typeof(TreeViewItem))
                {
                    MessageBoxResult result = MessageBox.Show("您确认要删除当前节点么？", "删除当前节点", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.OK || result == MessageBoxResult.Yes)
                    {
                        TreeViewItem aa = ((TreeViewItem)this.tv_PersonPlan.SelectedItem).Parent as TreeViewItem;

                        aa.Items.Remove(this.tv_PersonPlan.SelectedItem);
                    }
                }
                else if (((TreeViewItem)this.tv_PersonPlan.SelectedItem).Parent.GetType() == typeof(TreeView))
                {
                    MessageBoxResult result = MessageBox.Show("不允许删除根节点！", "删除根节点", MessageBoxButton.YesNo);


                }

            }

        }

        private void btn_SavePlan_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void MouseDoubleClick(TreeView personPlan, TreeView singleTreeView)
        {

            if (this.tv_PersonPlan.SelectedItem == null)//检查是否有节点选中
            {

            }
            else
            {//有节点选中,添加当前选中节点的子节点

                try
                {
                    if (((TreeViewItem)singleTreeView.SelectedItem).DataContext != null)
                    {
                        TreeNode tempTreeNode = ((TreeViewItem)singleTreeView.SelectedItem).DataContext as TreeNode;



                        if (tempTreeNode.Type == 0)
                        {
                            //MessageBox.Show("不能操作组织结构！");
                            return;
                        }

                    }
                }
                catch
                { }

                //if (((TreeViewItem)singleTreeView.SelectedItem).Parent.GetType() == typeof(TreeViewItem))
                //{


                TreeNode treenode = ((TreeViewItem)singleTreeView.SelectedItem).DataContext as TreeNode;

                if (((TreeViewItem)singleTreeView.SelectedItem).Parent.GetType() == typeof(TreeViewItem))
                {
                    TreeViewItem aa = ((TreeViewItem)singleTreeView.SelectedItem).Parent as TreeViewItem;

                    aa.Items.Remove(singleTreeView.SelectedItem);

                }
                else if (((TreeViewItem)singleTreeView.SelectedItem).Parent.GetType() == typeof(TreeView))
                {
                    TreeView aa = ((TreeViewItem)singleTreeView.SelectedItem).Parent as TreeView;

                    aa.Items.Remove(singleTreeView.SelectedItem);
                }
                if (personPlan.SelectedItem == null)//检查是否有节点选中
                {
                    TreeViewItem tvi = new TreeViewItem();
                    tvi.Header = treenode.Name;
                    tvi.DataContext = treenode;
                    tvi.IsExpanded = true;
                    treenode.ParentID = -1;

                    personPlan.Items.Add(tvi);//向treeview中添加根节点
                }
                else
                {//有节点选中,添加当前选中节点的子节点


                    TreeViewItem tvi = new TreeViewItem();
                    tvi.Header = treenode.Name;
                    tvi.DataContext = treenode;
                    tvi.IsExpanded = true;



                    ((TreeViewItem)personPlan.SelectedItem).Items.Add(tvi);//向当前选中节点的节点集合中添加node
                }

            }
            //}
        }

        /// <summary>
        /// 人员列表双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tv_PersonList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            MouseDoubleClick(tv_PersonPlan, tv_PersonList);
            //if (this.tv_PersonPlan.SelectedItem == null)//检查是否有节点选中
            //{

            //}
            //else
            //{//有节点选中,添加当前选中节点的子节点

            //    try
            //    {
            //        if (((TreeViewItem)this.tv_PersonList.SelectedItem).DataContext != null)
            //        {
            //            TreeNode tempTreeNode = ((TreeViewItem)this.tv_PersonList.SelectedItem).DataContext as TreeNode;



            //            if (tempTreeNode.Type == 0)
            //            {
            //                //MessageBox.Show("不能操作组织结构！");
            //                return;
            //            }

            //        }
            //    }
            //    catch
            //    { }

            //    if (((TreeViewItem)this.tv_PersonList.SelectedItem).Parent.GetType() == typeof(TreeViewItem))
            //    {


            //        TreeNode treenode = ((TreeViewItem)this.tv_PersonList.SelectedItem).DataContext as TreeNode;


            //        TreeViewItem aa = ((TreeViewItem)this.tv_PersonList.SelectedItem).Parent as TreeViewItem;

            //        aa.Items.Remove(this.tv_PersonList.SelectedItem);

            //        if (this.tv_PersonPlan.SelectedItem == null)//检查是否有节点选中
            //        {
            //            TreeViewItem tvi = new TreeViewItem();
            //            tvi.Header = treenode.Name;
            //            tvi.DataContext = treenode;
            //            tvi.IsExpanded = true;
            //            treenode.ParentID = -1;
            //            //nodeName = nodeName + this.tv_PersonPlan.Items.Count;

            //            this.tv_PersonPlan.Items.Add(tvi);//向treeview中添加根节点
            //        }
            //        else
            //        {//有节点选中,添加当前选中节点的子节点


            //            TreeViewItem tvi = new TreeViewItem();
            //            tvi.Header = treenode.Name;
            //            tvi.DataContext = treenode;
            //            tvi.IsExpanded = true;

            //            //treenode.ParentID = tv_PersonPlan.SelectedItem

            //            ((TreeViewItem)this.tv_PersonPlan.SelectedItem).Items.Add(tvi);//向当前选中节点的节点集合中添加node
            //        }

            //    }
            //}

        }



        /// <summary>
        /// 人员预案双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tv_PersonPlan_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            try
            {
                if (((TreeViewItem)this.tv_PersonPlan.SelectedItem).DataContext != null)
                {
                    TreeNode tempTreeNode = ((TreeViewItem)this.tv_PersonPlan.SelectedItem).DataContext as TreeNode;


                    if (tempTreeNode.Type == 0)
                    {
                        //MessageBox.Show("不能操作组织结构！");
                        return;
                    }

                }
            }
            catch
            { }

            if (((TreeViewItem)this.tv_PersonPlan.SelectedItem).Parent.GetType() == typeof(TreeViewItem))
            {


                TreeNode treenode = ((TreeViewItem)this.tv_PersonPlan.SelectedItem).DataContext as TreeNode;


                TreeViewItem aa = ((TreeViewItem)this.tv_PersonPlan.SelectedItem).Parent as TreeViewItem;

                aa.Items.Remove(this.tv_PersonPlan.SelectedItem);

                if (treenode.Type == 1)
                {
                    //if (this.tv_PersonList.SelectedItem == null)//检查是否有节点选中
                    //{

                        TreeViewItem parentItem = GetParentTreeViewItem(tv_PersonList, treenode);

                        TreeViewItem tvi = new TreeViewItem();
                        tvi.Header = treenode.Name;
                        tvi.DataContext = treenode;
                        tvi.IsExpanded = true;
                        parentItem.Items.Add(tvi);//向当前选中节点的节点集合中添加node
                    //}
                }
                else if (treenode.Type == 3)
                {

                    TreeViewItem tvi = new TreeViewItem();
                    tvi.Header = treenode.Name;
                    tvi.DataContext = treenode;
                    tvi.IsExpanded = true;
                    tv_VehicleList.Items.Add(tvi);//向当前选中节点的节点集合中添加node
                }
                else if (treenode.Type == 4)
                {

                    TreeViewItem tvi = new TreeViewItem();
                    tvi.Header = treenode.Name;
                    tvi.DataContext = treenode;
                    tvi.IsExpanded = true;
                    this.tv_EquipList.Items.Add(tvi);//向当前选中节点的节点集合中添加node
                }

            }
        }

        private TreeViewItem GetParentTreeViewItem(TreeView treeView, TreeNode treenode)
        {
            TreeViewItem parentItem = new TreeViewItem();
            foreach (TreeViewItem item in treeView.Items)
            {
                TreeNode tempnode = item.DataContext as TreeNode;
                if (tempnode.ID == treenode.ParentID)
                {
                    return item;
                }
                else
                {
                    if (item.Items.Count > 0)
                    {
                        parentItem = GetParentTreeViewItemByViewItem(item, treenode);
                    }
                }
            }

            return parentItem;
        }

        private TreeViewItem GetParentTreeViewItemByViewItem(TreeViewItem inItem, TreeNode treenode)
        {
            TreeViewItem parentItem = new TreeViewItem();
            foreach (TreeViewItem item in inItem.Items)
            {
                TreeNode tempnode = item.DataContext as TreeNode;
                if (tempnode.ID == treenode.ParentID)
                {
                    return item;
                }
                else
                {
                    if (item.Items.Count > 0)
                    {
                        parentItem = GetParentTreeViewItemByViewItem(item, treenode);
                    }
                }
            }

            return parentItem;
        }

        private void btn_SPersonList_Click(object sender, RoutedEventArgs e)
        {
            tv_PersonList.Visibility = System.Windows.Visibility.Visible;
            tv_VehicleList.Visibility = System.Windows.Visibility.Hidden;
            tv_EquipList.Visibility = System.Windows.Visibility.Hidden;
        }

        private void btn_SVehicleList_Click(object sender, RoutedEventArgs e)
        {
            tv_PersonList.Visibility = System.Windows.Visibility.Hidden;
            tv_VehicleList.Visibility = System.Windows.Visibility.Visible;
            tv_EquipList.Visibility = System.Windows.Visibility.Hidden;
        }

        private void btn_SEquipList_Click(object sender, RoutedEventArgs e)
        {
            tv_PersonList.Visibility = System.Windows.Visibility.Hidden;
            tv_VehicleList.Visibility = System.Windows.Visibility.Hidden;
            tv_EquipList.Visibility = System.Windows.Visibility.Visible;
        }

        private void tv_VehicleList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MouseDoubleClick(tv_PersonPlan, tv_VehicleList);
        }

        private void tv_EquipList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MouseDoubleClick(tv_PersonPlan, tv_EquipList);

        }

   
    }
}
