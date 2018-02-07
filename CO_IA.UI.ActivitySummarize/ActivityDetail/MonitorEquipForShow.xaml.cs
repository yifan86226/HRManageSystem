using CO_IA.Client;
using CO_IA.Data;
using CO_IA.UI.ActivitySummarize.ActivityDetail;
using DevExpress.Xpf.LayoutControl;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace CO_IA.UI.ActivitySummarize
{
    /// <summary>
    /// MonitorEquipForShow.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorEquipForShow : UserControl
    {
             #region 变量
        /// <summary>
        /// 绑定TreeView的数据源
        /// </summary>
        List<ActivityGuaranteeType> itemList = new List<ActivityGuaranteeType>();

        TextBox tempTextBox;
        TreeViewItem item;

        //当前选中的保障分类
        string type = "";
        private bool isRename;

        private bool flg = true;
        #endregion

        private List<GuaranteeProcess> gpList = new List<GuaranteeProcess>();
        private List<GuaranteeProcessView> gpvList = new List<GuaranteeProcessView>();
        private bool isModify = false;
        private string activityGuid;

        public MonitorEquipForShow()
        {
            InitializeComponent();
            this.isModify = false;
            this.activityGuid = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid;
            getGuaranteeTypeList();
            getGuaranteeProcessList(type);
        }

        //public MonitorEquipForShow(bool _isModify)
        //{
        //    InitializeComponent();
        //    this.isModify = _isModify;
        //    this.activityGuid = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid;
        //    getGuaranteeTypeList();
        //    getGuaranteeProcessList(type);
        //}
        /// <summary>
        /// 获取/初始化保障类型
        /// </summary>
        private void getGuaranteeTypeList()
        {
            Activity activity = CO_IA.Client.RiasPortal.ModuleContainer.Activity;
            if (activity == null || String.IsNullOrEmpty(activity.Guid))
            {
                createGuaranteeType();
            }
            else
            {
                List<ActivityGuaranteeType> nodes = new List<ActivityGuaranteeType>();
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivitySummarize.I_CO_IA_ActivitySummarize>(channel =>
                {
                    //更新当前节点
                    nodes = channel.GetActivityGuaranteeType(activityGuid);
                });
                if (nodes.Count() == 0)
                {
                    createGuaranteeType();
                }
                else
                {
                    tabVategory.ItemsSource = null;
                    //ActivityGuaranteeType temp = new ActivityGuaranteeType();
                    foreach (ActivityGuaranteeType type in nodes)
                    {
                        //if (string.IsNullOrEmpty(type.PARENTGUID))
                        //{
                        //    temp = type;
                        //    break;
                        //}
                        itemList.Add(type);
                    }
                    //ForeachPropertyNode(temp, temp.GUID, nodes);
                    //itemList.Add(temp);
                    this.tabVategory.ItemsSource = itemList;
                    //this.tabVategory.SelectedIndex = 0;
                }
            }
        }
        //无限接循环子节点添加到根节点下面
        private void ForeachPropertyNode(ActivityGuaranteeType node, string pid, List<ActivityGuaranteeType> nodes)
        {
            foreach (ActivityGuaranteeType tempNode in nodes)
            {
                if (tempNode.PARENTGUID == pid)
                {

                    ForeachPropertyNode(tempNode, tempNode.GUID, nodes);
                    node.Children.Add(tempNode);
                }
            }
        }
        private void createGuaranteeType()
        {
            ActivityGuaranteeType entity = new ActivityGuaranteeType();
            entity.GUID = CO_IA.Client.Utility.NewGuid();
            entity.NAME = "保障类型";
            entity.ACTIVITYGUID = activityGuid;

            bool bl = false;
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivitySummarize.I_CO_IA_ActivitySummarize>(channel =>
            {
                bl = channel.SaveActivityGuaranteeType(entity);
            });
            if (!bl)
            {
                MessageBox.Show("系统默认创建类型失败。");
            }
            itemList.Add(entity);
            this.tabVategory.ItemsSource = null;
            this.tabVategory.ItemsSource = itemList;
        }

        private void getGuaranteeProcessList(string type)
        {

            gpList = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivitySummarize.I_CO_IA_ActivitySummarize, List<CO_IA.Data.GuaranteeProcess>>(
                channel =>
                {
                    return channel.GetGuaranteeProcessList(activityGuid, type);
                });
            TranslateData();
            this.flc_ImgList.ItemsSource = gpvList;
            //GuaranteeProcess gp = new GuaranteeProcess();
            //gp.NAME = "111";
            //gp.TASK = "222";
            //gpList.Add(gp);



            //GuaranteeProcess gp1 = new GuaranteeProcess();
            //gp1.NAME = "333";
            //gp1.TASK = "444";
            //gpList.Add(gp1);

            //GuaranteeProcess gp2 = new GuaranteeProcess();
            //gp2.NAME = "111";
            //gp2.TASK = "222";
            //gpList.Add(gp2);



            //GuaranteeProcess gp3 = new GuaranteeProcess();
            //gp3.NAME = "333";
            //gp3.TASK = "444";
            //gpList.Add(gp3);

            //GuaranteeProcess gp4 = new GuaranteeProcess();
            //gp4.NAME = "111";
            //gp4.TASK = "222";
            //gpList.Add(gp4);



            //GuaranteeProcess gp5 = new GuaranteeProcess();
            //gp5.NAME = "333";
            //gp5.TASK = "444";
            //gpList.Add(gp5);

            //GuaranteeProcess gp6 = new GuaranteeProcess();
            //gp6.NAME = "111";
            //gp6.TASK = "222";
            //gpList.Add(gp6);



            //GuaranteeProcess gp7 = new GuaranteeProcess();
            //gp7.NAME = "333";
            //gp7.TASK = "444";
            //gpList.Add(gp7);

            //GuaranteeProcess gp8 = new GuaranteeProcess();
            //gp8.NAME = "111";
            //gp8.TASK = "222";
            //gpList.Add(gp8);



            //GuaranteeProcess gp9 = new GuaranteeProcess();
            //gp9.NAME = "333";
            //gp9.TASK = "444";
            //gpList.Add(gp9);

            //GuaranteeProcess gp10 = new GuaranteeProcess();
            //gp10.NAME = "333";
            //gp10.TASK = "444";
            //gpList.Add(gp10);

            //GuaranteeProcess gp11 = new GuaranteeProcess();
            //gp11.NAME = "111";
            //gp11.TASK = "222";
            //gpList.Add(gp11);



            //GuaranteeProcess gp12 = new GuaranteeProcess();
            //gp12.NAME = "333";
            //gp12.TASK = "444";
            //gpList.Add(gp12);

            //GuaranteeProcess gp13 = new GuaranteeProcess();
            //gp13.NAME = "111";
            //gp13.TASK = "222";
            //gpList.Add(gp8);



            //GuaranteeProcess gp14 = new GuaranteeProcess();
            //gp14.NAME = "333";
            //gp14.TASK = "444";
            //gpList.Add(gp14);
        }
        //public MonitorEquip(int p)
        //{
        //    InitializeComponent();


        //    GuaranteeProcess gp = new GuaranteeProcess();
        //    gp.NAME = "111";
        //    gp.TASK = "222";
        //    gpList.Add(gp);



        //    GuaranteeProcess gp1 = new GuaranteeProcess();
        //    gp1.NAME = "333";
        //    gp1.TASK = "444";
        //    gpList.Add(gp1);
        //    this.flc_ImgList.ItemsSource = gpList;
        //}

        //   public MonitorEquip(int mode)
        //{
        //    InitializeComponent();

        //    tb_Modify.Visibility = System.Windows.Visibility.Visible;
        //}

        private void GroupBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!flg)
            {
                flg = true;
                return;
            }
            var groupBox = (DevExpress.Xpf.LayoutControl.GroupBox)sender;
            GuaranteeProcessView gpv = groupBox.DataContext as GuaranteeProcessView;
            if (gpv.GUID == "add")
            {
                GuaranteeProcess gp = new GuaranteeProcess();
                gp.GUID = Utility.NewGuid();
                gp.ACTIVITY_GUID = this.activityGuid;
                gp.TYPE = type;
                MonitorEquipInput dlg = new MonitorEquipInput(gp);
                dlg.RefreshListEvent += () => { getGuaranteeProcessList(type); };
                dlg.ShowDialog(this);
            }
            else if (gpv.GUID == "import")
            {
                GuaranteeProcess gp = new GuaranteeProcess();
                gp.ACTIVITY_GUID = this.activityGuid;
                gp.TYPE = type;
                ImportTask it = new ImportTask(gp);
                it.RefreshListEvent += () => { getGuaranteeProcessList(type); };
                it.ShowDialog(this);
            }
            else
            {
                if (IsVoide(gpv.NAME))
                {
                    string path = System.Windows.Forms.Application.StartupPath + "\\tempVideo\\";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    string filePath = path + gpv.NAME;
                    if (File.Exists(filePath))
                    {
                        if (MessageBox.Show("当前视频文件已存在，是否重新下载？", "消息提示", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            if (DownLoadVideo(gpv.GUID))
                            {
                                Video video = new Video(filePath);
                                video.ShowDialog();
                            }
                            else
                            {
                                MessageBox.Show("视频下载失败", "消息提示");
                            }
                        }
                        else
                        {
                            Video video = new Video(filePath);
                            video.ShowDialog();
                        }
                    }
                    else
                    {
                        if (DownLoadVideo(gpv.GUID))
                        {
                            Video video = new Video(filePath);
                            video.ShowDialog();
                        }
                        else
                        {
                            MessageBox.Show("视频下载失败", "消息提示");
                        }
                    }

                }
                else
                {
                    groupBox.State = groupBox.State == GroupBoxState.Normal ? GroupBoxState.Maximized : GroupBoxState.Normal;
                }
            }
        }
        private bool DownLoadVideo(string guid)
        {
            GuaranteeProcess gp = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivitySummarize.I_CO_IA_ActivitySummarize, CO_IA.Data.GuaranteeProcess>(
                    channel =>
                    {
                        return channel.GetGuaranteeVideo(guid);
                    });
            if (gp != null)
            {
                try
                {
                    System.IO.FileInfo info = new System.IO.FileInfo(gp.NAME);
                    string path = System.Windows.Forms.Application.StartupPath + "\\tempVideo\\" + gp.NAME;
                    //System.IO.Directory.CreateDirectory(info.Directory.FullName + "\\tempVideo\\");
                    File.WriteAllBytes(path, gp.VIDEO);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void btn_ModifyClick(object sender, RoutedEventArgs e)
        {
            flg = false;

            Image btn = sender as Image;
            GuaranteeProcessView gpv = btn.DataContext as GuaranteeProcessView;
            GuaranteeProcess gp = new GuaranteeProcess();
            gp.GUID = gpv.GUID;
            gp.ACTIVITY_GUID = gpv.ACTIVITY_GUID;
            gp.NAME = gpv.NAME;
            gp.TASK = gpv.TASK;
            gp.PHOTO = gpv.PHOTO;
            gp.TYPE = gpv.TYPE;
            MonitorEquipInput dlg = new MonitorEquipInput(gp);
            dlg.RefreshListEvent += () => { getGuaranteeProcessList(type); };
            dlg.ShowDialog(this);
        }

        private void btn_DelClick(object sender, RoutedEventArgs e)
        {
            flg = false;

            Image btn = sender as Image;
            GuaranteeProcessView gpv = btn.DataContext as GuaranteeProcessView;
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivitySummarize.I_CO_IA_ActivitySummarize>(
                 channel =>
                 {
                     channel.DeleteGuaranteeProcess(gpv.GUID);
                     MessageBox.Show("删除成功", "提示", MessageBoxButton.OK);
                     getGuaranteeProcessList(type);
                 });
        }

        private void TranslateData()
        {
            gpvList = new List<GuaranteeProcessView>();
            foreach (GuaranteeProcess gp in gpList)
            {
                GuaranteeProcessView gpv = new GuaranteeProcessView();
                gpv.GUID = gp.GUID;
                gpv.NAME = gp.NAME;
                gpv.TASK = gp.TASK;
                gpv.ACTIVITY_GUID = gp.ACTIVITY_GUID;
                gpv.PHOTO = gp.PHOTO;
                gpv.TYPE = gp.TYPE;
                if (this.isModify)
                {
                    gpv.IsVisible = Visibility.Visible;
                }
                else
                {
                    gpv.IsVisible = Visibility.Hidden;
                }
                if (gp.PHOTO != null)
                {
                    MemoryStream stream = new MemoryStream(gp.PHOTO);
                    BitmapImage bmp = new BitmapImage();
                    bmp.BeginInit();//初始化
                    bmp.StreamSource = stream;//设置源
                    bmp.EndInit();//初始化结束
                    gpv.ImageSource = bmp;//设置图像Source
                }
                gpvList.Add(gpv);
            }
            if (this.isModify)
            {
                GuaranteeProcessView gpvAdd = new GuaranteeProcessView();
                gpvAdd.GUID = "add";
                gpvAdd.NAME = "添加";
                gpvAdd.TASK = "添加保障过程";
                gpvAdd.ACTIVITY_GUID = "";
                gpvAdd.IsVisible = Visibility.Hidden;
                BitmapImage bmpAdd = new BitmapImage(new Uri("../Images/add.png", UriKind.Relative));
                gpvAdd.ImageSource = bmpAdd;//设置图像Source
                gpvList.Add(gpvAdd);

                GuaranteeProcessView gpvImport = new GuaranteeProcessView();
                gpvImport.GUID = "import";
                gpvImport.NAME = "导入";
                gpvImport.TASK = "导入保障过程";
                gpvImport.ACTIVITY_GUID = "";
                gpvImport.IsVisible = Visibility.Hidden;
                BitmapImage bmpImport = new BitmapImage(new Uri("../Images/Import.png", UriKind.Relative));
                gpvImport.ImageSource = bmpImport;//设置图像Source
                gpvList.Add(gpvImport);
            }
        }

        private void StackPanel_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            StackPanel sp = sender as StackPanel;
            if (sp.ContextMenu != null)
            {
                MenuItem item1 = sp.ContextMenu.Items[0] as MenuItem;
                MenuItem item2 = sp.ContextMenu.Items[1] as MenuItem;
                if (sp.Tag == null || string.IsNullOrEmpty(sp.Tag.ToString()))
                {
                    item1.Visibility = System.Windows.Visibility.Visible;
                    item2.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    item1.Visibility = System.Windows.Visibility.Collapsed;
                    item2.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }
        private void AddTreeViewItem_Click(object sender, RoutedEventArgs e)
        {
            //添加树
            if (tabVategory.Items.Count > 0)
            {
                ActivityGuaranteeType info = tabVategory.Items[0] as ActivityGuaranteeType;
                if (info != null)
                {

                    ActivityGuaranteeType tempInfo = new ActivityGuaranteeType();
                    tempInfo.GUID = CO_IA.Client.Utility.NewGuid();
                    tempInfo.ACTIVITYGUID = info.ACTIVITYGUID;
                    tempInfo.PARENTGUID = info.GUID;
                    tempInfo.NAME = "分类名称";

                    info.Children.Add(tempInfo);


                    //保存到数据库
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivitySummarize.I_CO_IA_ActivitySummarize>(channel =>
                    {
                        //更新当前节点
                        channel.SaveActivityGuaranteeType(tempInfo);
                    });

                    this.tabVategory.ItemsSource = null;
                    this.tabVategory.ItemsSource = itemList;

                    //界面赋值
                    //SetOrgInfoDetails(tempOrgInfo);
                }
            }
        }
        private void DelTreeViewItem_Click(object sender, RoutedEventArgs e)
        {
            if (this.tabVategory.SelectedItem == null)//检查是否有节点选中
            { }
            else
            {//有节点选中,添加当前选中节点的子节点

                ActivityGuaranteeType info = tabVategory.SelectedItem as ActivityGuaranteeType;
                if (info != null)
                {
                    if (info.PARENTGUID != "")
                    {
                        MessageBoxResult result = MessageBox.Show("您确认要删除当前节点么？", "删除当前节点", MessageBoxButton.YesNo);
                        if (result == MessageBoxResult.OK || result == MessageBoxResult.Yes)
                        {
                            RemovePPOrginfoFromeList(info.PARENTGUID, info, this.itemList);
                            bool isAllSuccessfull = true;
                            //从数据库删除数据
                            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivitySummarize.I_CO_IA_ActivitySummarize>(channel =>
                            {
                                //删除当前节点
                                isAllSuccessfull = channel.DeleteActivityGuaranteeType(info.GUID);
                            });

                            if (isAllSuccessfull == true)
                            {
                                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                                {
                                    //删除当前节点子节点
                                    channel.DeletePP_OrgInfoByParentID(info.GUID);
                                });

                                this.tabVategory.ItemsSource = null;
                                this.tabVategory.ItemsSource = itemList;

                                if (this.tabVategory.Items.Count > 0)
                                {
                                    //itemOrgInfo = this.tv_Category.Items[0] as ActivityGuaranteeType;
                                    //界面赋值
                                    //SetOrgInfoDetails(itemOrgInfo);
                                }
                            }
                            else
                            {
                                MessageBox.Show("删除节点信息失败！");
                            }
                        }
                    }
                    else
                    {
                        MessageBoxResult result = MessageBox.Show("不允许删除根节点！", "删除根节点", MessageBoxButton.OK);
                    }
                }
            }
        }
        /// <summary>
        /// 移除特定的节点
        /// </summary>
        /// <param name="PARENT_GUID"></param>
        /// <param name="tarOrgInfo"></param>
        /// <param name="orgList"></param>
        private void RemovePPOrginfoFromeList(string PARENT_GUID, ActivityGuaranteeType tarOrgInfo, List<ActivityGuaranteeType> orgList)
        {
            foreach (ActivityGuaranteeType orginfo in orgList)
            {
                if (orginfo.GUID == PARENT_GUID)
                {
                    orginfo.Children.Remove(tarOrgInfo);
                    break;
                }
                else if (orginfo.Children.Count > 0)
                {
                    RemovePPOrginfoFromeList(PARENT_GUID, tarOrgInfo, orginfo.Children);
                }

            }
        }
        /// <summary>
        /// 当TextBox失去焦点时发生此事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void renametextbox_LostFous(object sender, RoutedEventArgs e)
        {
            tempTextBox.Visibility = Visibility.Collapsed;

            if (this.tabVategory.SelectedItem == null)//检查是否有节点选中
            { }
            else
            {//有节点选中,添加当前选中节点的子节点

                ActivityGuaranteeType orgInfo = tabVategory.SelectedItem as ActivityGuaranteeType;

                if (orgInfo != null)
                {
                    //保存到数据库
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivitySummarize.I_CO_IA_ActivitySummarize>(channel =>
                    {
                        //更新当前节点
                        channel.UpdateActivityGuaranteeType(orgInfo);
                    });
                }
            }
            this.isRename = false;
        }
        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (e.ClickCount == 2)
            {
                item = GetParentObjectEx<TreeViewItem>(e.OriginalSource as DependencyObject) as TreeViewItem;
                tempTextBox = FindVisualChild<TextBox>(item as DependencyObject);
                tempTextBox.Visibility = Visibility.Visible;
                //((TextBlock)e.Source).Visibility = System.Windows.Visibility.Collapsed;
            }
        }
        /// <summary>
        /// 获取当前TreeView的TreeViewItem
        /// </summary>
        /// <typeparam name="TreeViewItem"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public TreeViewItem GetParentObjectEx<TreeViewItem>(DependencyObject obj) where TreeViewItem : FrameworkElement
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);
            while (parent != null)
            {
                if (parent is TreeViewItem)
                {
                    return (TreeViewItem)parent;
                }
                parent = VisualTreeHelper.GetParent(parent);
            }
            return null;
        }

        /// <summary>
        /// 获取ItemTemplate内部的各种控件
        /// </summary>
        /// <typeparam name="childItem"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        private childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        {

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {

                DependencyObject child = VisualTreeHelper.GetChild(obj, i);

                if (child != null && child is childItem)

                    return (childItem)child;

                else
                {

                    childItem childOfChild = FindVisualChild<childItem>(child);

                    if (childOfChild != null)

                        return childOfChild;

                }

            }

            return null;

        }

        private void tv_Category_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ActivityGuaranteeType info = ((System.Windows.Controls.TreeView)(sender)).SelectedItem as ActivityGuaranteeType;
            if (info != null && !string.IsNullOrEmpty(info.PARENTGUID))
            {
                type = info.GUID;
            }
            else
            {
                type = "";
            }
            getGuaranteeProcessList(type);
        }

        /// <summary>
        /// 判断文件是否是视频
        /// </summary>
        /// <param name="sText"></param>
        /// <returns></returns> 
        private bool IsVoide(string fileName)
        {
            string strFilter = ".mp4|.flv|.rmvb|.avi|";
            char[] separtor = { '|' };
            string[] tempFileds = strFilter.Split(separtor);
            foreach (string str in tempFileds)
            {
                if (str.ToUpper() == fileName.Substring(fileName.LastIndexOf("."), fileName.Length - fileName.LastIndexOf(".")).ToUpper())
                {
                    return true;
                }
            }
            return false;
        }

        private void listVategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ActivityGuaranteeType info = ((System.Windows.Controls.ListBox)(sender)).SelectedItem as ActivityGuaranteeType;
            if (info != null && !string.IsNullOrEmpty(info.PARENTGUID))
            {
                type = info.GUID;
            }
            else
            {
                type = "";
            }
            getGuaranteeProcessList(type);

            System.Windows.Controls.ListBox listB = (System.Windows.Controls.ListBox)sender;
            
        }

        private void tabVategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ActivityGuaranteeType info = ((System.Windows.Controls.TabControl)(sender)).SelectedItem as ActivityGuaranteeType;
            if (info != null && !string.IsNullOrEmpty(info.PARENTGUID))
            {
                type = info.GUID;
            }
            else
            {
                type = "";
            }
            getGuaranteeProcessList(type);

            //System.Windows.Controls.ListBox listB = (System.Windows.Controls.ListBox)sender;
        }
    }
}
