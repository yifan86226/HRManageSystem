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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using Office_To_XPS;
using System.Windows.Xps.Packaging;
using CO.IA.UI.TaskManage.Rules;
using CO_IA.Data.FileManage;
using CO_IA.Data;
using CO_IA.Data.TaskManage;

namespace CO.IA.UI.TaskManage
{
    /// <summary>
    /// OfficeManage.xaml 的交互逻辑
    /// </summary>
    public partial class OfficeManage : UserControl
    {
        #region 变量
        //string _index = "";
        //string ActivityID = "";
        private Activity CurrentActivity
        {
            get
            {
                return CO_IA.Client.RiasPortal.ModuleContainer.Activity;
            }
        }
        private CheckBox chkAll;
        RegulationsInfo regulate = new RegulationsInfo();
        List<RegulationsInfo> currentRegulateList = new List<RegulationsInfo>();
        public RegulationsInfo RegluateSelected
        {
            get
            {
                if (dgList.SelectedItem == null)
                {
                    return null;
                }
                else
                {
                    return dgList.SelectedItem as RegulationsInfo;
                }
            }
            set
            {
                dgList.SelectedItem = value;
            }
        }

        public List<RegulationsInfo> OfficeItemsSource
        {
            get;
            set;
        }

        /// <summary>
        /// 活动的主体
        /// </summary>
        private Activity activity;
        private List<RegulationsTreeInfo> itemList = new List<RegulationsTreeInfo>();
        #endregion

        #region 构造函数
        public OfficeManage()
        {
            InitializeComponent();
            activity = CO_IA.Client.RiasPortal.ModuleContainer.Activity;
            //新建还是读取
            if (activity == null || string.IsNullOrEmpty(activity.Guid) == true)
            {
                //创建默认的组织结构组
                CreateAndSaveDefaultTreeInfos();
            }
            else
            {
                List<RegulationsTreeInfo> nodes = new List<RegulationsTreeInfo>();
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.RuleAndFileManage.I_CO_IA_RuleAndFile>(channel =>
                {
                    //更新当前节点
                    nodes = channel.GetTree(activity.Guid);
                });
                if (nodes.Count() == 0)
                {
                    CreateAndSaveDefaultTreeInfos();
                }
                else
                {
                    treePlace.ItemsSource = null;
                    RegulationsTreeInfo tempTreeInfo = new RegulationsTreeInfo();
                    foreach (RegulationsTreeInfo node in nodes)
                    {
                        if (string.IsNullOrEmpty(node.Parent_guid))
                        {
                            tempTreeInfo = node;
                            break;
                        }
                    }
                    ForeachPropertyNode(tempTreeInfo, tempTreeInfo.Guid, nodes);
                    itemList.Add(tempTreeInfo);
                    this.treePlace.ItemsSource = null;
                    this.treePlace.ItemsSource = itemList;
                }
            }
            //InitData();
        }
        #endregion

        #region 方法
        public void InitData()
        {
            //获取树形结构
            List<RegulationsTreeInfo> nodes = new List<RegulationsTreeInfo>();

            OfficeItemsSource = FileManageHelper.GetRegulationsInfo(CurrentActivity.Guid).ToList();
            //dgList.ItemsSource = OfficeItemsSource;
            if (OfficeItemsSource == null || OfficeItemsSource.Count() == 0)
            {
                btnSave.IsEnabled = true;
            }
            //dgList.ItemsSource = OfficeItemsSource.Where(r => r.RULETYPER == this.listBoxPlace.SelectedIndex).ToList();
        }

        private void CreateAndSaveDefaultTreeInfos()
        {
            string rootGUID =CO_IA.Client.Utility.NewGuid();
            RegulationsTreeInfo nodes = new RegulationsTreeInfo { Guid = rootGUID, Activity_guid = CurrentActivity.Guid, Name = "文件管理" };

            bool isSuccesful = false;
            //保存相关数据信息
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.RuleAndFileManage.I_CO_IA_RuleAndFile>(channel =>
            {
                //更新当前节点
                isSuccesful = channel.AddTree(nodes);
            });

            if (isSuccesful == false)
            {
                MessageBox.Show("系统默认创建和保存活动组织结构失败。");
            }
            itemList.Add(nodes);
            this.treePlace.ItemsSource = null;
            this.treePlace.ItemsSource = itemList;
        }
        #endregion

        #region 事件
        //private void listBoxPlace_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (listBoxPlace.SelectedIndex == 0)
        //    {
        //        //规章制度管理
        //        //_index = "0";
        //        dgList.ItemsSource = OfficeItemsSource.Where(r => r.RULETYPER == 0).ToList();
        //        //Image image = new Image();
        //        //image.Width = 100;
        //        //image.Height = 90;

        //    }
        //    else
        //    {
        //        //文件管理
        //        //_index = "1";
        //        dgList.ItemsSource = OfficeItemsSource.Where(r => r.RULETYPER == 1).ToList();
        //        //Image image = new Image();
        //        //image.Width = 100;
        //        //image.Height = 90;
        //    }
        //}

        private void rulsgrid_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (RegluateSelected.RULETYPER == 0)
            {
                RegluateSelected.RuleFiles = FileManageHelper.GetRuleFiles(RegluateSelected.GUID).ToList();
                RulesAndRegulationsDialog dialog = new RulesAndRegulationsDialog(RegluateSelected);
                dialog.WindowTitle = "修规章制度管理";
                dialog.AfterSaveEvent += () =>
                {
                    InitData();
                };
                dialog.ShowDialog(this);
            }
            else
            {
                RegluateSelected.RuleFiles = FileManageHelper.GetRuleFiles(RegluateSelected.GUID).ToList();
                FileManagementDialog dialog = new FileManagementDialog(RegluateSelected);
                dialog.WindowTitle = "修改文件管理";
                dialog.AfterSaveEvent += () =>
                {
                    InitData();
                };
                dialog.ShowDialog(this);
            }

        }
        /// <summary>
        /// 录入--规章制度、文件管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //if (this.listBoxPlace.SelectedIndex == 0)
            //{
            //    RulesAndRegulationsDialog dialog = new RulesAndRegulationsDialog(CurrentActivity.Guid);
            //    dialog.WindowTitle = "规章制度手工登记";
            //    dialog.AfterSaveEvent += () =>
            //   {
            //       InitData();
            //   };
            //    dialog.ShowDialog(this);
            //}
            //else
            //{
            //    FileManagementDialog dialog = new FileManagementDialog(CurrentActivity.Guid);
            //    dialog.WindowTitle = "文件管理手工登记";
            //    dialog.AfterSaveEvent += () =>
            //    {
            //        InitData();
            //    };
            //    dialog.ShowDialog(this);
            //}
        }
        /// <summary>
        /// 删除操作（支持批量删除）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool isdeleteOk = true;
                List<RegulationsInfo> checkFreq = (dgList.ItemsSource as List<RegulationsInfo>).Where(p => p.IsChecked == true).ToList();
                if (checkFreq.Count == 0)
                {
                    MessageBox.Show("请选择要删除的数据！"); return;
                }

                if (checkFreq.Count() > 0)
                {
                    foreach (RegulationsInfo item in checkFreq)
                    {
                        PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.RuleAndFileManage.I_CO_IA_RuleAndFile>
                        (channel =>
                        {

                            List<RuleFile> fileRules = item.RuleFiles;
                            //删除附件列表
                            if (fileRules != null && fileRules.Count() > 0)
                            {
                                bool monitorResult = channel.DeleteRulesFileMainID(item.GUID);//删除附件
                                if (monitorResult == false) return;
                                foreach (RuleFile file in fileRules)
                                {
                                    //删除客户端缓存的附件
                                    string savePath = FileManageHelper.GetPath() + file.FILEFORM.ToString();
                                    string xpsPath = savePath.Substring(0, savePath.LastIndexOf(".")).ToString() + ".xps";
                                    if (System.IO.File.Exists(savePath))
                                    {
                                        System.IO.File.Delete(savePath);
                                        System.IO.File.Delete(xpsPath);
                                    }

                                }
                            }
                            else
                            {
                                // 当前附件实体列表为空，从数据中查附件，然后删除本地缓存的附件
                                RuleFile[] getRuleFiles = FileManageHelper.GetRuleFiles(item.GUID);
                                if (getRuleFiles.Count() > 0)
                                {
                                    foreach (RuleFile file in getRuleFiles)
                                    {
                                        //删除客户端缓存的附件
                                        string savePath = FileManageHelper.GetPath() + file.FILEFORM.ToString();
                                        string xpsPath = savePath.Substring(0, savePath.LastIndexOf(".")).ToString() + ".xps";
                                        if (System.IO.File.Exists(savePath))
                                        {
                                            System.IO.File.Delete(savePath);
                                            System.IO.File.Delete(xpsPath);
                                        }

                                    }
                                }
                            }
                            bool deleteRegulate = channel.DeleteRegulationsInfo(item.GUID);//删除主表
                            if (deleteRegulate == false)
                            {
                                isdeleteOk = false;
                            }
                        });
                        if (isdeleteOk == false)
                            break;//主表删除失败
                    }
                    if (isdeleteOk == true)
                    {
                        MessageBox.Show("删除成功!", "提示", MessageBoxButton.OK);
                        InitData();
                    }
                    else
                    {
                        MessageBox.Show("删除失败!", "提示", MessageBoxButton.OK);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region 列表相关
        /// <summary>
        /// 初始加载选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkAll_Loaded(object sender, RoutedEventArgs e)
        {
            chkAll = sender as CheckBox;
            if (dgList.ItemsSource != null)
            {
                chkAll.IsChecked = (dgList.ItemsSource as List<RegulationsInfo>).Any(item => item.IsChecked);
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
            if (ischecked == true)
            {
                btnSave.IsEnabled = false;//不可用
                foreach (RegulationsInfo item in dgList.ItemsSource)
                {
                    item.IsChecked = ischecked;
                    currentRegulateList.Add(item);
                }
            }
            else
            {
                btnSave.IsEnabled = true;//可用
                foreach (RegulationsInfo item in dgList.ItemsSource)
                {
                    item.IsChecked = ischecked;
                    currentRegulateList.Remove(item);
                }
            }
        }

        private void dgList_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            regulate = dgList.SelectedItem as RegulationsInfo;
            if (regulate == null)
                return;//防止单击空白或标题等触发该事件
            if (regulate.IsChecked == true)
            {
                regulate.IsChecked = false;
                currentRegulateList.Remove(regulate);
            }
            else
            {
                regulate.IsChecked = true;
                currentRegulateList.Add(regulate);
            }
            #region 判断全选和保存状态
            bool checkedState = regulate.IsChecked;
            int ischeckCount = (dgList.ItemsSource as List<RegulationsInfo>).Count(p => p.IsChecked == true);
            if (ischeckCount == 1 || ischeckCount == 0)
            {
                btnSave.IsEnabled = true;
            }
            else
            {
                btnSave.IsEnabled = false;
            }
            foreach (RegulationsInfo result in dgList.ItemsSource)
            {
                if (result.IsChecked != checkedState)
                {
                    this.chkAll.IsChecked = null;
                    return;
                }

            }
            chkAll.IsChecked = checkedState;
            #endregion

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
            regulate = box.DataContext as RegulationsInfo;
            bool checkedState = isChecked.Value;
            regulate.IsChecked = isChecked.Value;
            if (regulate.IsChecked == true)
            {
                currentRegulateList.Add(regulate);
            }
            else
            {
                currentRegulateList.Remove(regulate);
            }

            #region 判断全选和保存状态
            int ischeckCount = (dgList.ItemsSource as List<RegulationsInfo>).Count(p => p.IsChecked == true);
            if (ischeckCount == 1 || ischeckCount == 0)
            {
                btnSave.IsEnabled = true;
            }
            else
            {
                btnSave.IsEnabled = false;
            }
            foreach (RegulationsInfo result in dgList.ItemsSource)
            {
                if (result.IsChecked != checkedState)
                {
                    this.chkAll.IsChecked = null;
                    return;
                }

            }
            chkAll.IsChecked = checkedState;
            #endregion
        }
        #endregion

        //无限接循环子节点添加到根节点下面
        private void ForeachPropertyNode(RegulationsTreeInfo node, string pid, List<RegulationsTreeInfo> nodes)
        {
            foreach (RegulationsTreeInfo tempNode in nodes)
            {
                if (tempNode.Parent_guid == pid)
                {

                    ForeachPropertyNode(tempNode, tempNode.Guid, nodes);
                    node.Children.Add(tempNode);
                }
            }
        }
    }
}

