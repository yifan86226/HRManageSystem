using Betx.Portal.Models;
using CO_IA.Data;
using PT.Profile.Business;
using PT.Profile.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// SelectOrgPersonDialog.xaml 的交互逻辑
    /// </summary>
    public partial class SelectOrgPersonDialog : Window
    {
        private List<PP_DepartmentInfo> _pp_DepartmentInfoList = new List<PP_DepartmentInfo>();

        private string _areaCode;

        private List<PP_PersonInfo> personinfolist = new List<PP_PersonInfo>();
        public List<PP_PersonInfo> SelectPersoninfolist = new List<PP_PersonInfo>();

        /// <summary>
        /// 当前选择的组织信息
        /// </summary>
        private PP_DepartmentInfo itemOrgInfo = new PP_DepartmentInfo();
        private List<string> exsitUserIDList;
        private bool isMultSelect;
        private bool isShowAll;

        private UserInfoList userInfoList;
        /// <summary>
        /// 组织机构集合
        /// </summary>
        public List<PP_DepartmentInfo> PP_DepartmentInfoList
        {
            get
            {
                return _pp_DepartmentInfoList;
            }
            set
            {
                _pp_DepartmentInfoList = value;
                //RaisePropertyChanged("DepartmentInfoCollection");
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="isMultSelect">是否多选</param>
        /// <param name="isShowAll">是否显示全部</param>
        /// <param name="exsitUserIDList">过滤列表</param>
        public SelectOrgPersonDialog(bool isMultSelect, bool isShowAll, List<string> exsitUserIDList)
        {
            InitializeComponent();

            string privinceCode = CO_IA.Client.RiasPortal.Current.SystemConfig.LoginArea.Substring(0, 2) + "00";
            Initialize(privinceCode);

            this.exsitUserIDList = exsitUserIDList;
            this.isMultSelect = isMultSelect;
            this.isShowAll = isShowAll;

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        public void Initialize(string pAreaCode = "")
        {
            PT.Profile.Types.DeptInfoList deptLsit = new PT.Profile.Types.DeptInfoList();


            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<PT.Profile.Interface.IOrganization, PT.Profile.Types.DeptInfoList>(p =>
            {
                deptLsit = p.GetDeptInfos();
                return deptLsit;
            });
            //获取所有用户信息
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<PT.Profile.Interface.IOrganization>(channel =>
            {
                this.userInfoList = channel.GetUsers();
                if (this.userInfoList == null)
                {
                    this.userInfoList = new UserInfoList();
                }
            });

            PP_DepartmentInfoList.Clear();
            _areaCode = string.IsNullOrWhiteSpace(pAreaCode)
                            ? ParameterInformation.Current.GlobalParameter.Code
                            : pAreaCode;
            if (string.IsNullOrWhiteSpace(_areaCode) || _areaCode.Equals("0"))
                return;
            //添加服务引用方式调用服务

            OrganizationClientOnGetDeptInfos(deptLsit);
        }

        private void OrganizationClientOnGetDeptInfos(DeptInfoList pDeptInfos)
        {
            try
            {
                var deptinfos = pDeptInfos;
                if (deptinfos == null)
                    throw new Exception("组织结构初始化失败。");
                if (deptinfos.Count == 0)
                    return;
                DeptInfo foundRoot = (from i in deptinfos where i.DistrictCode == _areaCode && i.IsOrganization select i).FirstOrDefault();
                if (foundRoot == null)
                    return;

                PP_DepartmentInfoList.Clear();
                PP_DepartmentInfo rootdepartment = CreateDepartmentInfoSource(foundRoot);

                ForeachPropertyNode(rootdepartment, rootdepartment.GUID, pDeptInfos);
                PP_DepartmentInfoList.Add(rootdepartment);

                this.tv_PersonPlan.ItemsSource = null;
                this.tv_PersonPlan.ItemsSource = PP_DepartmentInfoList;


            }
            catch 
            {
                ////LogHelper.WriteLog("初始化组织结构", Utile.GetExceptionContext(ex));
                //OnInitializeCompleted(false);
            }
            finally
            {
                //if (!completed)
                //    OnInitializeCompleted(false);
            }
        }
        //无限接循环子节点添加到根节点下面
        private void ForeachPropertyNode(PP_DepartmentInfo node, string pid, DeptInfoList nodes)
        {
            foreach (DeptInfo tempNode in nodes)
            {
                if (tempNode.ParentDeptID == pid)
                {
                    PP_DepartmentInfo pp_department = CreateDepartmentInfoSource(tempNode);

                    ForeachPropertyNode(pp_department, pp_department.GUID, nodes);

                    node.Children.Add(pp_department);
                }
            }
        }

        /// <summary>
        /// 人员组织树进行切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tv_PersonPlan_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                //取得当前的被选择节点     
                itemOrgInfo = ((System.Windows.Controls.TreeView)(sender)).SelectedItem as PP_DepartmentInfo;

                if (itemOrgInfo == null)
                {
                    return;
                }
                else
                {
                    //界面赋值

                    PT.Profile.Types.DeptMemberList deptList = new PT.Profile.Types.DeptMemberList();

                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<PT.Profile.Interface.IOrganization, PT.Profile.Types.DeptMemberList>(p =>
                    {
                        deptList = p.GetDeptMembersByDeptID(itemOrgInfo.GUID, false);

                        return deptList;
                    });

                    personinfolist.Clear();

                    foreach (DeptMember dm in deptList)
                    {

                        if (dm.UserStatus == UserStatusEnum.Out
                            || dm.UserStatus == UserStatusEnum.Forbid
                            || dm.UserStatus == UserStatusEnum.Leave
                            || dm.UserStatus == UserStatusEnum.Background
                            || exsitUserIDList.Contains(dm.UserID))
                        {
                            continue;
                        }

                        PP_PersonInfo tempPersonInfo = CreatePersonInfoSource(dm);
                        personinfolist.Add(tempPersonInfo);

                    }


                    this.dg_GrouperList.ItemsSource = null;
                    this.dg_GrouperList.ItemsSource = personinfolist;

                }
            }
            catch
            {
                throw;
            }
        }

        private PP_PersonInfo CreatePersonInfoSource(DeptMember dm)
        {
            PP_PersonInfo tempPersonInfo = new PP_PersonInfo();
            tempPersonInfo.GUID = dm.UserID;
            tempPersonInfo.DISTRICT_CODE = dm.DistrictCode;

            tempPersonInfo.NAME = dm.RealName;
            tempPersonInfo.SEX = dm.UserSex;
            tempPersonInfo.DUTY = dm.UserDuty;

            DistrictInfo rootDistrict = PT.Profile.Business.DistrictHelper.DistrictInfos.FindItemByCode(dm.DistrictCode);
            if (rootDistrict != null)
            {
                tempPersonInfo.UNIT = rootDistrict.Name + "无委";
            }

            tempPersonInfo.DEPT = dm.DeptName;
            tempPersonInfo.DEPT_ID = dm.DeptID;
            //tempPersonInfo.TASK = dm.UserID;
            //tempPersonInfo.PERSON_TYPE = dm.UserID;
            tempPersonInfo.ADD_TYPE = 3;//选择加入

            try
            {
                var organizationChannel = PT_BS_Service.Client.Framework.BeOperationInvoker.GetInterface<PT.Profile.Interface.IOrganization>();
                var userInfo = this.userInfoList.Find(item => item.UserID.Equals(dm.UserID));
                if (userInfo != null)
                {
                    tempPersonInfo.PHONE = userInfo.UserPhone;
                    tempPersonInfo.PHOTO = organizationChannel.GetUserPicture(dm.UserID);
                }
               

                //PT.Profile.Types.UserInfo ui = new PT.Profile.Types.UserInfo();

                //PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<PT.Profile.Interface.IOrganization, PT.Profile.Types.UserInfo>(p =>
                //{
                //    ui = p.GetUser(dm.UserID);
                //    return ui;
                //});

                //if (ui != null)
                //{
                //    tempPersonInfo.PHONE = ui.UserPhone;
                //    //tempPersonInfo.PHOTO = ui.UserPicture;
                //    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<PT.Profile.Interface.IOrganization>(p =>
                //    {
                //        tempPersonInfo.PHOTO = p.GetUserPicture(dm.UserID);

                //    });
                //}
            }
            catch
            { }

            return tempPersonInfo;
        }

        private PP_DepartmentInfo CreateDepartmentInfoSource(DeptInfo pDeptInfo)
        {
            var dis = new PP_DepartmentInfo
            {
                ORG_TYPE = 0,
                GUID = pDeptInfo.DeptID,
                NAME = pDeptInfo.DeptName,
                DeptFax = pDeptInfo.DeptFax,
                DeptPhone = pDeptInfo.DeptPhone,
                DisplaySequence = pDeptInfo.DisplaySequence,
                DistrictCode = pDeptInfo.DistrictCode,
                IsOrganization = pDeptInfo.IsOrganization,
                PARENT_GUID = pDeptInfo.ParentDeptID,
                //XGZ:用户在线数量
                //OnlineUserCount = "(1/10)"
            };
            return dis;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        /// <summary>
        /// 选择需要加入的人员
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dg_GrouperList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            PP_PersonInfo tempPerson = dg_GrouperList.SelectedItem as PP_PersonInfo;
            if (tempPerson != null)
            {
                if (isMultSelect == true)
                {
                    personinfolist.Remove(tempPerson);
                    exsitUserIDList.Add(tempPerson.GUID);
                    SelectPersoninfolist.Add(tempPerson);

                    this.dg_SelectGrouperList.ItemsSource = null;
                    this.dg_SelectGrouperList.ItemsSource = SelectPersoninfolist;

                    this.dg_GrouperList.ItemsSource = null;
                    this.dg_GrouperList.ItemsSource = personinfolist;
                }
                else
                {
                    SelectPersoninfolist.Clear();
                    SelectPersoninfolist.Add(tempPerson);

                    this.dg_SelectGrouperList.ItemsSource = null;
                    this.dg_SelectGrouperList.ItemsSource = SelectPersoninfolist;

                }

            }
        }

        /// <summary>
        /// 取消需要加入的人员
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dg_SelectGrouperList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            PP_PersonInfo tempPerson = this.dg_SelectGrouperList.SelectedItem as PP_PersonInfo;
            if (tempPerson != null)
            {
                exsitUserIDList.Remove(tempPerson.GUID);
                SelectPersoninfolist.Remove(tempPerson);
                this.dg_SelectGrouperList.ItemsSource = null;
                this.dg_SelectGrouperList.ItemsSource = SelectPersoninfolist;

                if (tempPerson.DEPT_ID == itemOrgInfo.GUID)
                {
                    personinfolist.Add(tempPerson);
                    this.dg_GrouperList.ItemsSource = null;
                    this.dg_GrouperList.ItemsSource = personinfolist;
                }
            }
        }

        private void btn_OK_Click(object sender, RoutedEventArgs e)
        {
            if (this.SelectPersoninfolist.Count > 0)
            {
                this.DialogResult = true;
            }
            else
            {
                this.DialogResult = false;
            }
            this.Close();
        }
    }
}
