using CO_IA.Data;
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

namespace CO_IA.UI.Screen.Audio
{
    /// <summary>
    /// AudioPanel.xaml 的交互逻辑
    /// </summary>
    public partial class AudioPanel : UserControl
    {
        PP_OrgInfo CurrentOrg;
        List<PP_OrgInfo> itemList = new List<PP_OrgInfo>();
        public AudioPanel()
        {
            InitializeComponent();
            this.Loaded += AudioPanel_Loaded;
        }
        public AudioPanel(PP_OrgInfo orgInfo)
        {
            InitializeComponent();
            CurrentOrg = orgInfo;
            this.Loaded += AudioPanel_Loaded;
        }

        void AudioPanel_Loaded(object sender, RoutedEventArgs e)
        {
            List<PP_OrgInfo> nodes = new List<PP_OrgInfo>();

            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
            {
                //更新当前节点
                nodes = channel.GetPP_OrgInfos(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
            });
            if (nodes == null || nodes.Count == 0)
            {
                MessageBox.Show("没有组信息！");
                return;
            }

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

            if (CurrentOrg != null)
            {
                PP_OrgInfo[] orgs = itemList.Where(item=>item.GUID==CurrentOrg.GUID).ToArray();
                if (orgs != null && orgs.Length == 0)
                {
                    TreeViewItem tv = tv_PersonPlan.ItemContainerGenerator.ContainerFromItem(orgs[0]) as TreeViewItem;
                    tv.IsSelected = true;
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
        private void tv_PersonPlan_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                //取得当前的被选择节点     
                var itemOrgInfo = ((System.Windows.Controls.TreeView)(sender)).SelectedItem as PP_OrgInfo;

                if (itemOrgInfo == null)
                {
                    return;
                }
                else
                {
                    

                }
            }
            catch
            {
                throw;
            }
        }
        private void listBoxPlace_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CO_IA.UI.ActivitySummarize.ListItem item = listBoxPlace.SelectedItem as CO_IA.UI.ActivitySummarize.ListItem;
        }
    }
}
