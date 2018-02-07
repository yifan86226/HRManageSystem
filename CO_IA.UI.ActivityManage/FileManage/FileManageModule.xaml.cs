using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using CO_IA.Data.FileManage;
using CO_IA.Data;
using AT_BC.Common;

namespace CO_IA.UI.ActivityManage.FileManage
{
    /// <summary>
    /// OfficeManage.xaml 的交互逻辑
    /// </summary>
    public partial class FileManageModule : UserControl, INotifyPropertyChanged
    {
        #region 变量

        private Activity activity;
        /// <summary>
        /// 名称中不可以保护的特殊字符
        /// </summary>
        string strspecial = @"\ / * ? < > |";

        private CheckBox chkAll;

        private TreeViewItem SelectTreeViewItem
        {
            get;
            set;
        }

        private TreeNode<CatalogInfo> SelectedCatalogItem
        {
            get
            {
                return treeCatalog.SelectedItem as TreeNode<CatalogInfo>;
            }
        }

        ObservableCollection<TreeNode<CatalogInfo>> catalogInfosources = new ObservableCollection<TreeNode<CatalogInfo>>();
        public ObservableCollection<TreeNode<CatalogInfo>> CatalogInfoSources
        {
            get
            {
                return treeCatalog.ItemsSource as ObservableCollection<TreeNode<CatalogInfo>>;
            }
            set
            {
                treeCatalog.ItemsSource = value;
            }
        }

        /// <summary>
        /// 文件数据源
        /// </summary>
        public ObservableCollection<WorkFileInfo> WorkFileItemsSource
        {
            get { return dgList.ItemsSource as ObservableCollection<WorkFileInfo>; }
            set { dgList.ItemsSource = value; }
        }

        /// <summary>
        /// 默认数据源
        /// </summary>
        public List<WorkFileInfo> DefaultWorkFileItemsSource
        {
            get;
            set;
        }

        #endregion

        #region 构造函数

        public FileManageModule()
        {
            InitializeComponent();
            activity = CO_IA.Client.RiasPortal.ModuleContainer.Activity;
            InitCatalogTree();
            dgList.MouseDoubleClick += dgList_MouseDoubleClick;
            this.DataContext = this;
        }

        #endregion

        #region  目录树相关

        /// <summary>
        /// 初始化目录树
        /// </summary>
        /// <param name="activityguid"></param>
        private void InitCatalogTree()
        {
            CatalogInfoSources = new ObservableCollection<TreeNode<CatalogInfo>>();
            string activityguid = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid;
            List<CatalogInfo> catalogs = FileManageHelper.QueryCatalogs(activityguid);
            //存在目录信息
            if (catalogs != null && catalogs.Count > 0)
            {
                ObservableCollection<TreeNode<CatalogInfo>> source = TreeNode<CatalogInfo>.CreateTreeNodes(catalogs, org => string.IsNullOrWhiteSpace(org.Parent_guid), (org, node) =>
                {
                    return org.Parent_guid == node.Value.Guid;
                });

                CatalogInfoSources = source;
            }
            //不存在目录信息
            else
            {
                TreeNode<CatalogInfo> newnode = CreateRootTreeNode();
                string error = string.Empty;
                bool result = FileManageHelper.SaveCatalog(newnode.Value, out error);
                if (result)
                {
                    CatalogInfoSources.Add(newnode);
                }
                else
                {
                    MessageBox.Show(error, "保存失败");
                }
            }

            if (CatalogInfoSources != null && CatalogInfoSources.Count > 0)
            {
                CatalogInfoSources[0].IsSelected = true;
            }
        }

        /// <summary>
        /// 创建目录信息
        /// </summary>
        /// <returns></returns>
        private TreeNode<CatalogInfo> CreateRootTreeNode()
        {
            TreeNode<CatalogInfo> node = new TreeNode<CatalogInfo>();
            CatalogInfo catalog = new CatalogInfo();
            catalog.Guid = CO_IA.Client.Utility.NewGuid();
            catalog.Activity_guid = activity.Guid;
            catalog.Name = string.Format("{0}", activity.Name);
            node.Value = catalog;
            catalog.CreateDate = DateTime.Now;
            return node;
        }

        /// <summary>
        /// Node选择改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void item_Selected(object sender, RoutedEventArgs e)
        {
            SelectTreeViewItem = (e.OriginalSource as TreeViewItem);
        }

        /// <summary>
        /// 树控件SelectedItemChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeCatalog_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (SelectedCatalogItem != null)
            {
                GetWorkFiles();
                txtQuery.EditValue = null;
            }
        }

        /// <summary>
        /// 添加目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddTreeViewItem_Click(object sender, RoutedEventArgs e)
        {
            if (treeCatalog.SelectedItem != null)
            {
                TreeNode<CatalogInfo> selectnode = treeCatalog.SelectedItem as TreeNode<CatalogInfo>;
                if (selectnode.Level < 2)
                {
                    TreeNode<CatalogInfo> newcatalog = new TreeNode<CatalogInfo>();
                    CatalogInfo catalog = new CatalogInfo();
                    catalog.Guid = Guid.NewGuid().ToString();
                    catalog.Activity_guid = activity.Guid;
                    catalog.Parent_guid = selectnode.Value.Guid;
                    catalog.Name = CreateCatalogName();
                    catalog.CreateDate = DateTime.Now;
                    newcatalog.Value = catalog;

                    string error = string.Empty;
                    bool result = FileManageHelper.SaveCatalog(newcatalog.Value, out error);
                    if (result)
                    {
                        selectnode.SubTreeNodes.Add(newcatalog);
                    }
                    else
                    {
                        MessageBox.Show(error, "目录添加失败");
                    }
                }
                else
                {
                    MessageBox.Show("只可以创建三层目录");
                }
            }
        }


        private string CreateCatalogName()
        {
            string defname = "新建目录";
            if (CatalogInfoSources != null && CatalogInfoSources[0] != null)
            {
                List<CatalogInfo> nodes = CatalogInfoSources[0].GetNodeDataList();
                List<string> names = nodes.Where(r => r.Name.Contains(defname)).Select(r => r.Name).ToList();
                if (names != null && names.Count > 0)
                {
                    if (names.Contains(defname))
                    {
                        string newname = string.Empty;
                        for (int i = 1; i < names.Count + 1; )
                        {
                            newname = defname + i.ToString();

                            if (names.Contains(newname))
                            {
                                i++;
                            }
                            else
                            {
                                return newname;
                            }
                        }
                    }
                    else
                    {
                        return defname;
                    }
                }
                return defname;

            }
            return defname;
        }

        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DelTreeItem_Click(object sender, RoutedEventArgs e)
        {
            TreeNode<CatalogInfo> selectnode = treeCatalog.SelectedItem as TreeNode<CatalogInfo>;
            if (selectnode != null)
            {
                if (selectnode.Level == 0)
                {
                    MessageBox.Show("不可以删除根目录");
                }
                else
                {
                    List<string> guids = GetDeleteItemGuid(selectnode);
                    string result = FileManageHelper.DeleteCatalog(guids);
                    if (string.IsNullOrEmpty(result))
                    {

                        InitCatalogTree();
                        //MessageBox.Show("删除成功");
                        CatalogInfoSources.Remove(selectnode);
                    }
                    else
                    {
                        MessageBox.Show(result, "删除失败");
                    }
                }
            }
        }

        /// <summary>
        /// 重命名目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RNameTreeItem_Click(object sender, RoutedEventArgs e)
        {
            TextBox tempTextBox = AT_BC.Common.VisualTreeHelperExtension.GetChildObject<TextBox>(SelectTreeViewItem, "renametextbox");
            tempTextBox.Visibility = Visibility.Visible;
            tempTextBox.Focus();
            tempTextBox.SelectAll();
        }

        private void treeCatalog_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Type t = sender.GetType();
            if (t == typeof(TreeViewItem))
            {
                TreeViewItem item = sender as TreeViewItem;

                TextBox temp = AT_BC.Common.VisualTreeHelperExtension.GetChildObject<TextBox>(item, "renametextbox");
                if (temp.IsFocused)
                {
                    return;
                }
            }

            TextBox tempTextBox = AT_BC.Common.VisualTreeHelperExtension.GetChildObject<TextBox>(SelectTreeViewItem, "renametextbox");
            tempTextBox.Visibility = Visibility.Collapsed;
            tempTextBox.Focusable = false;
        }

        /// <summary>
        /// 重命名文本框失去焦点时,保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void renametextbox_LostFocus(object sender, RoutedEventArgs e)
        {
            //TextBox tempTextBox = AT_BC.Common.VisualTreeHelperExtension.GetChildObject<TextBox>(SelectTreeViewItem, "renametextbox");
            //tempTextBox.Visibility = Visibility.Collapsed;

            TextBox txb = sender as TextBox;
            if (ValidName(txb.Text))
            {
                txb.Visibility = Visibility.Collapsed;
                this.SelectedCatalogItem.Value.Name = txb.Text;
                string error = null;
                FileManageHelper.SaveCatalog(SelectedCatalogItem.Value, out error);
            }
            else
            {
                txb.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// 按下回车键表示重命名完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void renametextbox_KeyDown(object sender, KeyEventArgs e)
        {
            //按下回车键表示输入完成
            if (e.Key == Key.Enter)
            {
                TextBox txb = sender as TextBox;
                if (ValidName(txb.Text))
                {
                    txb.Visibility = Visibility.Collapsed;
                    this.SelectedCatalogItem.Value.Name = txb.Text;
                }
                else
                {
                    txb.Visibility = Visibility.Visible;
                    txb.Text = this.SelectedCatalogItem.Value.Name;
                    txb.Focus();
                }
            }
        }

        /// <summary>
        /// 获取删除节点Guid 
        /// </summary>
        /// <returns></returns>
        private List<string> GetDeleteItemGuid(TreeNode<CatalogInfo> selectnode)
        {
            List<string> guids = new List<string>();
            guids.Add(selectnode.Value.Guid);

            foreach (TreeNode<CatalogInfo> item in selectnode.SubTreeNodes)
            {
                guids.Add(item.Value.Guid);
                if (item.SubTreeNodes.Count > 0)
                {
                    GetDeleteItemGuid(item);
                }
            }
            return guids;
        }

        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="selectnode"></param>
        /// <param name="nodes"></param>
        private void DeleteTreeItem(TreeNode<CatalogInfo> selectnode, ObservableCollection<TreeNode<CatalogInfo>> nodes)
        {
            foreach (TreeNode<CatalogInfo> item in nodes)
            {
                if (selectnode.Value.Guid == item.Value.Parent_guid)
                {
                    selectnode.SubTreeNodes.Remove(item);
                }
                else if (item.SubTreeNodes.Count > 0)
                {
                    DeleteTreeItem(item, item.SubTreeNodes);
                }
            }
        }

        /// <summary>
        /// 验证名称
        /// </summary>
        private bool ValidName(string rname)
        {
            bool result = true;
            if (string.IsNullOrWhiteSpace(rname))
            {
                MessageBox.Show("名称不可以为空");
                result = false;
            }
            else
            {
                foreach (char item in rname.ToCharArray())
                {
                    if (strspecial.Contains(item))
                    {
                        MessageBox.Show(string.Format("文件名不能包含下来任何字符 \n {0}", strspecial));
                        result = false;
                        return false;
                    }
                }
            }
            if (result) //验证名称是否存在
            {
                bool isexist = NameIsExist(rname);
                if (isexist)
                {
                    MessageBox.Show("名称已经存在请重新输入");
                    result = false;
                }
            }
            return result;
        }

        /// <summary>
        /// 验证名称是否存在
        /// </summary>
        /// <param name="name"></param>
        private bool NameIsExist(string name)
        {
            if (CatalogInfoSources != null && CatalogInfoSources[0] != null)
            {
                List<CatalogInfo> nodes = CatalogInfoSources[0].GetNodeDataList();

                int count = nodes.Count(r => r.Name == name && r.Guid != SelectedCatalogItem.Value.Guid);
                if (count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        ////无限接循环子节点添加到根节点下面
        //private void SetSubCatalog(List<CatalogInfo> catalogs, CatalogTreeNode catalognode)
        //{
        //    List<CatalogInfo> subcatalogs = catalogs.Where(c => c.Parent_guid == catalognode.Catalog.Guid).ToList();
        //    foreach (CatalogInfo catalog in subcatalogs)
        //    {
        //        CatalogTreeNode node = new CatalogTreeNode();
        //        node.Catalog = catalog;
        //        SetSubCatalog(catalogs, node);
        //        catalognode.Children.Add(node);
        //    }
        //}


        #endregion

        #region 文件相关操作

        /// <summary>
        /// 查询工作文件
        /// </summary>
        /// <returns></returns>
        private void GetWorkFiles()
        {
            if (this.SelectedCatalogItem != null)
            {
                string catalogguid = this.SelectedCatalogItem.Value.Guid;
                List<WorkFileInfo> files = FileManageHelper.GetWorkFiles(this.activity.Guid, catalogguid);
                DefaultWorkFileItemsSource = files;
                WorkFileItemsSource = new ObservableCollection<WorkFileInfo>(files);
            }
        }

        /// <summary>
        /// 添加工作文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCatalogItem != null)
            {
                WorkFileInfo file = new WorkFileInfo();
                file.IssuePerson = CO_IA.Client.RiasPortal.Current.UserSetting.UserName;
                file.GUID = System.Guid.NewGuid().ToString();
                file.Catalog.Guid = SelectedCatalogItem.Value.Guid;
                file.Catalog.Name = SelectedCatalogItem.Value.Name;
                file.SendState = SendStateEnum.草拟;
                WorkFileManage dialog = new WorkFileManage(file);
                dialog.WindowTitle = "添加工作文件";
                dialog.AfterSaveEvent += (newfile) =>
                {
                    MessageBox.Show("保存成功");
                    GetWorkFiles();
                };
                dialog.ShowDialog(this);
            }
            else
            {
                MessageBox.Show("请先选择目录");
            }
        }

        /// <summary>
        /// 修改文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgList.SelectedItem != null)
            {
                WorkFileInfo selectfile = dgList.SelectedItem as WorkFileInfo;
                if (!string.IsNullOrEmpty(selectfile.GUID))
                {
                    int selectindex = dgList.SelectedIndex;

                    WorkFileManage modifyfile = new WorkFileManage(selectfile);
                    modifyfile.WindowTitle = "修改工作文件";
                    modifyfile.AfterSaveEvent += (file) =>
                    {
                        MessageBox.Show("保存成功");
                        GetWorkFiles();
                        dgList.SelectedIndex = selectindex;
                    };
                    modifyfile.ShowDialog();
                }
                else
                {
                    MessageBox.Show(string.Format("{0}不包含文件,请先添加文件", SelectedCatalogItem.Value.Name));
                }
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (WorkFileItemsSource != null || WorkFileItemsSource.Count > 0)
            {
                List<WorkFileInfo> files = WorkFileItemsSource.Where(r => r.IsChecked == true).ToList();
                if (files.Count == 0)
                {
                    MessageBox.Show("请勾选要删除的文件");
                    return;
                }

                List<string> guids = files.Select(r => r.GUID).ToList();
                string error;
                bool result = FileManageHelper.DeleteWorkFile(guids, out error);
                if (result)
                {
                    MessageBox.Show("删除成功");
                    GetWorkFiles();
                    chkAll.IsChecked = false;
                }
                else
                {
                    MessageBox.Show(error, "删除失败");
                }
            }
        }

        private void txtQuery_EditValueChanging(object sender, DevExpress.Xpf.Editors.EditValueChangingEventArgs e)
        {
            if (txtQuery.EditValue != null)
            {
                string condition = txtQuery.EditValue.ToString();
                if (!string.IsNullOrWhiteSpace(condition))
                {
                    List<WorkFileInfo> subSource = WorkFileItemsSource.Where(r => r.FileName.Contains(condition)).ToList();
                    WorkFileItemsSource = new ObservableCollection<WorkFileInfo>(subSource);
                }
                else
                {
                    WorkFileItemsSource = new ObservableCollection<WorkFileInfo>(DefaultWorkFileItemsSource);
                }
            }
            else
            {
                WorkFileItemsSource = new ObservableCollection<WorkFileInfo>(DefaultWorkFileItemsSource);
            }
        }


        /// <summary>
        /// 搜索文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            txtQuery.EditValue = string.Empty;
            //WorkFileItemsSource = new ObservableCollection<WorkFileInfo>(DefaultWorkFileItemsSource);
        }

        #region 列表Check相关

        /// <summary>
        /// 初始加载选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkAll_Loaded(object sender, RoutedEventArgs e)
        {
            chkAll = sender as CheckBox;
            if (WorkFileItemsSource != null)
            {
                chkAll.IsChecked = WorkFileItemsSource.Any(item => item.IsChecked);
            }
        }

        /// <summary>
        /// 全选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            bool ischecked = chk.IsChecked.Value;

            foreach (WorkFileInfo item in WorkFileItemsSource)
            {
                item.IsChecked = ischecked;
            }
        }

        /// <summary>
        /// 单选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkCell_Click(object sender, RoutedEventArgs e)
        {
            CheckBox box = sender as CheckBox;
            bool? isChecked = box.IsChecked;
            if (!isChecked.HasValue)
            {
                return;
            }

            int ischeckCount = (WorkFileItemsSource).Count(p => p.IsChecked == true);
            if (ischeckCount == 0)
            {
                chkAll.IsChecked = false;
            }
            else if (ischeckCount == WorkFileItemsSource.Count)
            {
                chkAll.IsChecked = true;
            }
            else
            {
                chkAll.IsChecked = null;
            }
        }

        #endregion

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

