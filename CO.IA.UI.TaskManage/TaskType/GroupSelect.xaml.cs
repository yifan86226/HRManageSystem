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
using CO_IA.Data;

namespace CO.IA.UI.TaskManage.TaskType
{
    /// <summary>
    /// GroupSelect.xaml 的交互逻辑
    /// </summary>
    public partial class GroupSelect : UserControl
    {
        /// <summary>
        /// 活动的主体
        /// </summary>
        private Activity activity;
        string ActivityID = "";
        /// <summary>
        /// 绑定TreeView的数据源
        /// </summary>
        List<PP_OrgInfo> itemList = new List<PP_OrgInfo>();
        /// <summary>
        /// 当前组织ID
        /// </summary>
        private PP_OrgInfo itemOrgInfo = new PP_OrgInfo();
        public event Action<PP_OrgInfo> OKButtonClick;
        public GroupSelect()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded()
        {
            activity = CO_IA.Client.RiasPortal.ModuleContainer.Activity;
            ActivityID = activity.Guid;
            if (activity == null || string.IsNullOrEmpty(activity.Guid) == true)
            {
                MessageBox.Show("请先设置活动信息！");
            }
            else
            {
                //所得所有已存的人员组织结构
                List<PP_OrgInfo> nodes = new List<PP_OrgInfo>();
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                {
                    //更新当前节点
                    nodes = channel.GetPP_OrgInfos(activity.Guid);
                });
                if (nodes.Count > 0)
                {
                    tv_PersonPlan.Items.Clear();//加载根节点前先清除Treeview控件项
                    PP_OrgInfo tempOrgInfo = new PP_OrgInfo();
                    foreach (PP_OrgInfo oinfo in nodes)
                    {
                        if (string.IsNullOrEmpty(oinfo.PARENT_GUID))
                        {
                            tempOrgInfo = oinfo;
                            break;
                        }
                    }
                    ForeachPropertyNode(tempOrgInfo, tempOrgInfo.GUID, nodes);
                    itemList.Add(tempOrgInfo);
                    this.tv_PersonPlan.ItemsSource = null;
                    this.tv_PersonPlan.ItemsSource = itemList;

                }
            }
        }
        private void ForeachPropertyNode(PP_OrgInfo node, string pid, List<PP_OrgInfo> nodes)
        {
            foreach (PP_OrgInfo tempNode in nodes)
            {
                if (tempNode.PARENT_GUID == pid)
                {
                    ForeachPropertyNode(tempNode, tempNode.GUID, nodes);
                    node.Children.Add(tempNode);
                }
            }
        }
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {

        }

        private void tv_PersonPlan_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            //取得当前的被选择节点     
            itemOrgInfo = ((System.Windows.Controls.TreeView)(sender)).SelectedItem as PP_OrgInfo;

            if (itemOrgInfo == null)
            {
                return;
            }
            else
            {
                if (itemOrgInfo.PARENT_GUID == "")
                {
                   
                }
                else
                {
                    if (itemOrgInfo.Children.Count() == 0)
                    {
                        if (OKButtonClick != null)
                        {
                            OKButtonClick(itemOrgInfo);
                        }
                    }
                    else
                    {
                        
                    }
                   
                }
            }
        }
    }
}
