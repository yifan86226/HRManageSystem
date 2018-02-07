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
using System.Windows.Shapes;

namespace CO_IA.UI.Screen.Audio
{
    /// <summary>
    /// AudioDialog.xaml 的交互逻辑
    /// </summary>
    public partial class AudioDialog : Window
    {
        CommunicationServiceConfig config = null;


        PP_OrgInfo CurrentOrg;
        List<PP_OrgInfo> itemList = new List<PP_OrgInfo>();
        public AudioDialog(PP_OrgInfo orgInfo)
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
                tv_PersonPlan.UpdateLayout();
                SetSelectNode(itemList);
                //PP_OrgInfo[] orgs = itemList.Where(item => item.GUID == CurrentOrg.GUID).ToArray();
                //if (orgs != null && orgs.Length == 0)
                //{
                //    TreeViewItem tv = tv_PersonPlan.ItemContainerGenerator.ContainerFromItem(orgs[0]) as TreeViewItem;
                //    tv.IsSelected = true;
                //}
            }

            IniService();
            topAudio.Call += VoiceCall;
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
        private void SetSelectNode(List<PP_OrgInfo> itemList)
        {
            foreach (PP_OrgInfo tempNode in itemList)
            {
                if (tempNode.GUID == CurrentOrg.GUID)
                {
                    TreeViewItem tv = tv_PersonPlan.ItemContainerGenerator.ContainerFromItem(tv_PersonPlan.Items[0]) as TreeViewItem;
                    tv.IsSelected = true;
                    break;
                }
                if (tempNode.Children.Count > 0)
                {
                    PP_OrgInfo node = null;
                    TreeViewItem tvNode = tv_PersonPlan.ItemContainerGenerator.ContainerFromItem(tv_PersonPlan.Items[0]) as TreeViewItem;
                    for (int i = 0; i < tvNode.Items.Count;i++ )
                    {
                        node = tvNode.Items[i] as PP_OrgInfo;
                        if (node != null)
                        {
                            if (node.GUID == CurrentOrg.GUID)
                            {
                                TreeViewItem tv = tvNode.ItemContainerGenerator.ContainerFromItem(tvNode.Items[i]) as TreeViewItem;
                                if (tv != null)
                                    tv.IsSelected = true;
                            }
                        }
                    }
                    //foreach (PP_OrgInfo node in tempNode.Children)
                    //{
                    //    if (node.GUID == CurrentOrg.GUID)
                    //    {
                    //        TreeViewItem tvNode = tv_PersonPlan.ItemContainerGenerator.ContainerFromItem(tv_PersonPlan.Items[0]) as TreeViewItem;
                    //        TreeViewItem tv = tvNode.ItemContainerGenerator.ContainerFromItem(node) as TreeViewItem;
                    //        tv.IsSelected = true;
                    //        break;
                    //    }
                    //}
                }
            }
        }
        private void tv_PersonPlan_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                if (topAudio.Called)
                {
                    return;
                }
                topAudio.PersonData = null;
                //取得当前的被选择节点     
                var itemOrgInfo = ((System.Windows.Controls.TreeView)(sender)).SelectedItem as PP_OrgInfo;

                if (itemOrgInfo == null)
                {
                    return;
                }
                else
                {
                    //stpPanel.Children.Clear();
                    List<PP_PersonInfo> personList = null;
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                    {
                        //更新当前节点
                        personList = channel.GetPP_PersonInfos(itemOrgInfo.GUID);
                    });
                    personlistbox.ItemsSource = null;
                    personlistbox.ItemsSource = personList;                   

                }
            }
            catch
            {
                throw;
            }
        }
        private Person_Info_Ext getPersonExt(PP_PersonInfo person)
        {
            Person_Info_Ext p = new Person_Info_Ext();
            p.GUID = person.GUID;
            p.ISCHECKED = person.ISCHECKED;
            p.NAME = person.NAME;
            p.ORG_GUID = person.ORG_GUID;
            p.PERSON_TYPE = person.PERSON_TYPE;
            p.PHONE = person.PHONE;
            p.PHOTO = person.PHOTO;
            p.TASK = person.TASK;
            p.UNIT = person.UNIT;
            p.ADD_TYPE = person.ADD_TYPE;
            p.DUTY = person.DUTY;
            p.Flag = false;
            return p;
        }
        private void splitText_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (splitText.Text == "︿")
            {
                splitText.Text = "﹀";
                splitText.ToolTip = "点击展开";
                bottomGrid.Visibility = System.Windows.Visibility.Collapsed;
                this.Height = 120;
            }
            else
            {
                splitText.Text = "︿";
                splitText.ToolTip = "点击收回";
                bottomGrid.Visibility = System.Windows.Visibility.Visible;
                this.Height = 450;

            }
        }

        private void personlistbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (personlistbox.SelectedItem == null)
                return;
            if (topAudio.Called)
            {
                return;
            }
            PP_PersonInfo person = personlistbox.SelectedItem as PP_PersonInfo;
            topAudio.PersonData = person;

        }

        private void IniService()
        {
            statusInfo.Text = "初始化服务";
            
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.Setting.I_CO_IA_Setting>(
            channel =>
            {
                config = channel.GetCommunicationServiceProgram();
            });
            if (config == null || string.IsNullOrEmpty(config.ServiceUrl) || string.IsNullOrEmpty(config.Tel_Left))
            {
                statusInfo.Text = "缺少服务信息！";
                MessageBox.Show("缺少服务信息！");
                return;
            }
            ScheduleCommandAgent.ServiceUrl = config.ServiceUrl;
            statusInfo.Text = "初始化服务信息完成。  服务地址：" +config.ServiceUrl + "。  左手电话："+config.Tel_Left;
        }

        private void VoiceCall(bool flag,string callnum)
        {
            CallAsync(callnum, new CompleteScheduleCommand((succ) =>
            {
                if (succ)
                {
                    if (flag)
                    {
                        topAudio.Called = true;
                    }
                    else
                    {
                        topAudio.Called = false;
                    }
                    MessageBox.Show("操作成功！");
                    //topAudio.ChangeBtnState(true, flag);
                    //statusInfo2.Text = "呼叫成功！";
                }
                else
                {
                    MessageBox.Show("呼叫失败！");
                    //topAudio.ChangeBtnState(false, flag);
                }
            }));
            //CallAsync(callnum, new CompleteScheduleCommand((succ) => {
            //    if (succ)
            //    {
            //        MessageBox.Show("操作成功！");
            //        topAudio.ChangeBtnState(true);
            //        //statusInfo2.Text = "呼叫成功！";
            //    }
            //    else
            //    {
            //        MessageBox.Show("呼叫失败！");
            //        topAudio.ChangeBtnState(false);
            //    }
            //}));
        }
        private bool CallAsync(string callNum,CompleteScheduleCommand callback)
        {
            if (string.IsNullOrEmpty(callNum))
            {
                MessageBox.Show("没有呼叫号码！");
                return false; 
            }
            if (string.IsNullOrEmpty(ScheduleCommandAgent.ServiceUrl))
            {
                MessageBox.Show("缺少服务信息！");
                return false;
            }
            ScheduleCommandAgent.CallAsync(config.Tel_Left, callNum, callback);
            return true;
        }
    }
}
