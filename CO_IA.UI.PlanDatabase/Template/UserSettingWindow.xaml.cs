using AT_BC.Common;
using AT_BC.Data;
using CO_IA.Data;
using CO_IA.Data.Template;
using PT.Profile.Business;
using PT.Profile.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace CO_IA.UI.PlanDatabase.Template
{
    /// <summary>
    /// SelectOrgPersonDialog.xaml 的交互逻辑
    /// </summary>
    public partial class UserSettingWindow : Window
    {
        private List<string> ignoreUserIDList=new List<string>();
        private ObservableCollection<StaffInfo> selectedUserInfoCollection = new ObservableCollection<StaffInfo>();
        private Dictionary<string, List<StaffInfo>> dicStaffList = new Dictionary<string, List<StaffInfo>>();
        private UserInfoList userInfoList;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="isMultSelect">是否多选</param>
        /// <param name="isShowAll">是否显示全部</param>
        /// <param name="exsitUserIDList">过滤列表</param>
        public UserSettingWindow(IEnumerable<string> ignoreUserIDs)
        {
            InitializeComponent();
            if (ignoreUserIDs != null)
            {
                this.ignoreUserIDList.AddRange(ignoreUserIDs);
            }
            this.dataGridSelectedStaff.ItemsSource = this.selectedUserInfoCollection;
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<PT.Profile.Interface.IOrganization>(channel =>
                {
                    var result = channel.GetDeptInfosByParentDeptID(PT.Profile.Definition.SpecialIDs.DeptRootID, true);
                    if (result != null)
                    {
                        Department[] depts = new Department[result.Count];
                        for (int i = 0; i < depts.Length; i++)
                        {
                            depts[i] = new Department { Guid = result[i].DeptID, Name = result[i].DeptName, ParentDept = result[i].ParentDeptID, IsOrganization = result[i].IsOrganization };
                        }
                        var treeNodeList = TreeNode<Department>.CreateTreeNodes(depts, dept => PT.Profile.Definition.SpecialIDs.DeptRootID.Equals(dept.ParentDept),
                            (dept, partentNode) => dept.ParentDept == partentNode.Value.Guid);
                        this.treeViewOrg.ItemsSource = treeNodeList;
                    }
                });
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<PT.Profile.Interface.IOrganization>(channel =>
                {
                    this.userInfoList = channel.GetUsers();
                    if (this.userInfoList == null)
                    {
                        this.userInfoList = new UserInfoList();
                    }
                });
            }
        }

        /// <summary>
        /// 人员组织树进行切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeViewOrg_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var treeNode = e.NewValue as TreeNode<Department>;
            if (treeNode != null)
            {
                var currentDept=treeNode.Value;
                string orgName=string.Empty;
                TreeNode<Department> destNode=treeNode;
                while(destNode!=null && !destNode.Value.IsOrganization)
                {
                    destNode=destNode.Parent;
                }
                if (destNode!=null)
                {
                    orgName=destNode.Value.Name;
                }
                List<StaffInfo> staffList;
                if (!this.dicStaffList.TryGetValue(currentDept.Guid,out staffList))
                {
                    staffList = new List<StaffInfo>();
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<PT.Profile.Interface.IOrganization>(channel =>
                    {
                        var userList = channel.GetDeptMembersByDeptID(currentDept.Guid,false);
                        if (userList != null)
                        {
                            var organizationChannel = PT_BS_Service.Client.Framework.BeOperationInvoker.GetInterface<PT.Profile.Interface.IOrganization>();
                            foreach (var user in userList)
                            {
                                if (user.UserStatus == UserStatusEnum.Normal)
                                {
                                    if (this.ignoreUserIDList.Contains(user.UserID))
                                    {
                                        continue;
                                    }
                                    StaffInfo staff = new StaffInfo();
                                    staff.Key = user.UserID;
                                    staff.Name = user.RealName;
                                    staff.Duty = user.UserDuty;
                                    staff.Dept = user.DeptName;
                                    staff.AreaCode = user.DistrictCode;
                                    staff.Sex = user.UserSex;
                                    var userInfo = this.userInfoList.Find(item => item.UserID.Equals(user.UserID));
                                    if (userInfo != null)
                                    {
                                        staff.Phone = userInfo.UserPhone;
                                        staff.Unit = orgName;
                                    }
                                    staff.Photo = organizationChannel.GetUserPicture(user.UserID);
                                    staffList.Add(staff);
                                }
                            }
                        }
                    });
                    dicStaffList.Add(currentDept.Guid, staffList);
                }
                this.dataGridSelectableStaff.ItemsSource = staffList;
                ICollectionView view = CollectionViewSource.GetDefaultView(staffList);
                if (view.CanFilter)
                {
                    view.Filter = current =>
                    {
                        StaffInfo currentStaff = current as StaffInfo;
                        if (currentStaff != null)
                        {
                            return !currentStaff.IsChecked;
                        }
                        return true;
                    };
                }
            }
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        /// <summary>
        /// 选择需要加入的人员
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridSelectableStaff_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DataGridRow dgr = DataGridRow.GetRowContainingElement(e.OriginalSource as FrameworkElement);
                if (dgr != null)
                {
                    var staff = dgr.DataContext as StaffInfo;
                    if (staff != null)
                    { 
                        staff.IsChecked = true;
                        ICollectionView view = CollectionViewSource.GetDefaultView(this.dataGridSelectableStaff.ItemsSource as IList<StaffInfo>);
                        view.Refresh();
                        (this.dataGridSelectedStaff.ItemsSource as ObservableCollection<StaffInfo>).Add(staff);
                    }
                }
            }
        }

        /// <summary>
        /// 取消需要加入的人员
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridSelectedStaff_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DataGridRow dgr = DataGridRow.GetRowContainingElement(e.OriginalSource as FrameworkElement);
                if (dgr != null)
                {
                    var staff = dgr.DataContext as StaffInfo;
                    if (staff != null)
                    {
                        staff.IsChecked = false;
                        ICollectionView view = CollectionViewSource.GetDefaultView(this.dataGridSelectableStaff.ItemsSource as IList<StaffInfo>);
                        view.Refresh();
                        (this.dataGridSelectedStaff.ItemsSource as ObservableCollection<StaffInfo>).Remove(staff);
                    }
                }
            }
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            //if (this.SelectPersoninfolist.Count > 0)
            //{
            //    this.DialogResult = true;
            //}
            //else
            //{
            //    this.DialogResult = false;
            //}
            //this.Close();
        }

        private void treeViewOrg_Selected(object sender, RoutedEventArgs e)
        {
            var treeViewItem = e.OriginalSource as TreeViewItem;
            if (treeViewItem != null)
            {
                var a = treeViewItem.ContainerFromElement(treeViewItem);
            }
        }

        public IList<StaffInfo> GetSelectedStaffList()
        {
            return this.selectedUserInfoCollection;
        }

        private class Department : IdentifiableData<string>
        {
            public string ParentDept
            {
                get;
                set;
            }
            public bool IsOrganization
            {
                get;
                set;
            }
        }
    }
}
