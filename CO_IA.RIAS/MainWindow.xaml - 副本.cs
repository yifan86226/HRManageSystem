using AT_BC.Data;
using CO_IA.Client;
using CO_IA.Client.UIBuilderFactory;
using CO_IA.Data;
using CO_IA.Types;
using DevExpress.Xpf.Docking;
 
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

using DevExpress.Xpf.LayoutControl;
using DevExpress.Xpf.Core.Native;

using DevExpress.Xpf.DemoBase.DataClasses;
using System.IO;

namespace CO_IA.RIAS
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, CO_IA.Client.IRiasModuleContainer//,Client.IMessageReceiver
    {

        /// <summary>
        /// 绑定TreeView的数据源
        /// </summary>
        List<PP_OrgInfo> itemList = new List<PP_OrgInfo>();



        public MainWindow()
        {
            InitializeComponent();
            CO_IA.Client.RiasPortal.SetupModuleContainer(this);
            this.Loaded += MainWindow_Loaded;
            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += new EventHandler(SystemEvents_DisplaySettingsChanged);
            //test
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string loginArea = CO_IA.Client.RiasPortal.Current.SystemConfig.LoginArea;
            if (!string.IsNullOrWhiteSpace(loginArea) && loginArea.Length >= 2)
            {
                this.textBlockArea.Text = CO_IA.Client.Utility.GetAreaName(CO_IA.Client.RiasPortal.Current.SystemConfig.LoginArea.Substring(0, 2));
            }
            this.textBlockLoginUser.Text = CO_IA.Client.RiasPortal.Current.UserSetting.UserName;
            RiasPortal.UpdateUserRights();

        }

        internal void LoadActivity(CO_IA.Data.Activity activity)
        {
            if (!Utility.HasActivityManageRight())
            {
                this.buttonPlanDatabase.Visibility = System.Windows.Visibility.Collapsed;
                this.buttonSetting.Visibility = System.Windows.Visibility.Collapsed;
                this.buttonTemplate.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (this.Activity == null)
            {
                this.stackPanelFunction.Visibility = System.Windows.Visibility.Visible;

                activity.ActivityStage = ActivityStage.Execute;
                this.Activity = activity;
                this.DataContext = activity;
                //this.textBlockActivityName.Text = this.Activity.Name;
                //this.documentActivityInfo.Content = new CO_IA.UI.ActivityManage.ActivityManageModule();

                CreateAndSaveDefaultOrgInfos(activity);

                //this.layoutRoot.DataContext = new EmployeesViewModel();


                return;
            }
            throw new Exception("已存在打开的活动,不能重新打开");

         
        }




        /// <summary>
        /// 创建默认的组织结构组
        /// </summary>
        private void CreateAndSaveDefaultOrgInfos(CO_IA.Data.Activity activity)
        {
            

            //重大活动安全保障办公室
            string rootGUID = CO_IA.Client.Utility.NewGuid();

            //频率台站第一小组
            string pinlvSubGUID1 = CO_IA.Client.Utility.NewGuid();
            //频率台站第二小组
            string pinlvSubGUID2 = CO_IA.Client.Utility.NewGuid();
            //监测组第一小组
            string jianceSubGUID1 = CO_IA.Client.Utility.NewGuid();
            //监测组第二小组
            string jianceSubGUID2 = CO_IA.Client.Utility.NewGuid();

            List<PP_OrgInfo> nodes = new List<PP_OrgInfo>()
            {
                new PP_OrgInfo { GUID = rootGUID,  ACTIVITY_GUID=activity.Guid, NAME = "油料股" ,DUTY="01"},
                new PP_OrgInfo { GUID = jianceSubGUID1, ACTIVITY_GUID=activity.Guid, NAME = "干部", PARENT_GUID = rootGUID ,DUTY="02"},
                new PP_OrgInfo { GUID = jianceSubGUID2,ACTIVITY_GUID=activity.Guid,  NAME = "高级士官", PARENT_GUID = rootGUID ,DUTY="03"},
                new PP_OrgInfo { GUID = pinlvSubGUID1,ACTIVITY_GUID=activity.Guid,  NAME = "四级军士长", PARENT_GUID = rootGUID,DUTY="04"},
                new PP_OrgInfo { GUID = pinlvSubGUID2,ACTIVITY_GUID=activity.Guid,  NAME = "上士", PARENT_GUID = rootGUID,DUTY="05"},
                new PP_OrgInfo { GUID = pinlvSubGUID2,ACTIVITY_GUID=activity.Guid,  NAME = "中士", PARENT_GUID = rootGUID,DUTY="06"},
                new PP_OrgInfo { GUID = pinlvSubGUID2,ACTIVITY_GUID=activity.Guid,  NAME = "下士", PARENT_GUID = rootGUID,DUTY="07"},
                new PP_OrgInfo { GUID = pinlvSubGUID2,ACTIVITY_GUID=activity.Guid,  NAME = "上等兵", PARENT_GUID = rootGUID,DUTY="08"},
                new PP_OrgInfo { GUID = pinlvSubGUID2,ACTIVITY_GUID=activity.Guid,  NAME = "新兵", PARENT_GUID = rootGUID,DUTY="09"},

            };




            tv_PersonPlan.Items.Clear();//加载根节点前先清除Treeview控件项

            //PP_OrgInfo node = new PP_OrgInfo { GUID = rootGUID, ACTIVITY_GUID = activity.Guid, NAME = "重大活动安全保障办公室" };

            ForeachPropertyNode(nodes[0], nodes[0].GUID, nodes);
            itemList.Add(nodes[0]);

            this.tv_PersonPlan.ItemsSource = null;
            this.tv_PersonPlan.ItemsSource = itemList;
        }


        //无限接循环子节点添加到根节点下面
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




        public class EmployeeViewModel
        {
            public EmployeeViewModel(Employee employee)
            {
                Model = employee;
                ImageSource = ImageHelper.CreateImageFromStream(new MemoryStream(Model.ImageData));
            }

            public string AddressLine2
            {
                get
                {
                    string result = Model.City;
                    if (!string.IsNullOrEmpty(Model.StateProvinceName))
                        result += ", " + Model.StateProvinceName;
                    if (!string.IsNullOrEmpty(Model.PostalCode))
                        result += ", " + Model.PostalCode;
                    if (!string.IsNullOrEmpty(Model.CountryRegionName))
                        result += ", " + Model.CountryRegionName;
                    return result;
                }
            }
            public ImageSource ImageSource { get; private set; }
            public Employee Model { get; private set; }
        }



        private void tv_PersonPlan_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

            //取得当前的被选择节点     
            PP_OrgInfo itemOrgInfo = ((System.Windows.Controls.TreeView)(sender)).SelectedItem as PP_OrgInfo;


            if (itemOrgInfo == null)
            {
                return;
            }
            else
            {
                //界面赋值

                layoutRoot.DataContext = new EmployeesViewModel(itemOrgInfo.DUTY);
            }



        }

        private void PersonItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }



        public class EmployeesViewModel : List<EmployeeViewModel>
        {
            public EmployeesViewModel() : this(EmployeesWithPhotoData.DataSource) { }


            public EmployeesViewModel(string  type) {

                int ctype = 11-Convert.ToInt32(type);


                for (int i= 0; i < EmployeesWithPhotoData.DataSource.Count;i++)
                {
                    if (i % ctype == 0)
                    {
 

                        if (ctype==2)
                        {
                            EmployeesWithPhotoData.DataSource[i].LastName = "干部";
                        }
                        else   if (ctype == 3)
                        {
                            EmployeesWithPhotoData.DataSource[i].LastName = "高级士官";
                        }
                        else if (ctype == 4)
                        {
                            EmployeesWithPhotoData.DataSource[i].LastName = "四级军士长";
                        }
                        else if (ctype == 5)
                        {
                            EmployeesWithPhotoData.DataSource[i].LastName = "上士";
                        }
                        else if (ctype == 6)
                        {
                            EmployeesWithPhotoData.DataSource[i].LastName = "中士";
                        }
                        else if (ctype == 7)
                        {
                            EmployeesWithPhotoData.DataSource[i].LastName = "下士";
                        }
                        else if (ctype == 8)
                        {
                            EmployeesWithPhotoData.DataSource[i].LastName = "上等兵";
                        }
                        else if (ctype == 9)
                        {
                            EmployeesWithPhotoData.DataSource[i].LastName = "新兵";
                        }
                        


                        Add(new EmployeeViewModel(EmployeesWithPhotoData.DataSource[i]));
                    }

                }
            
                Sort(CompareByLastNameFirstName);
            }

            public EmployeesViewModel(string type , IEnumerable<Employee> model)
            {
                foreach (Employee employee in model)
                    Add(new EmployeeViewModel(employee));

                Sort(CompareByLastNameFirstName);
            }



            public EmployeesViewModel(IEnumerable<Employee> model)
            {
                foreach (Employee employee in model)
                    Add(new EmployeeViewModel(employee));

                Sort(CompareByLastNameFirstName);
            }

            private int CompareByLastNameFirstName(EmployeeViewModel x, EmployeeViewModel y)
            {
                string value1 = x.Model.LastName + x.Model.FirstName;
                string value2 = y.Model.LastName + y.Model.FirstName;
                return string.Compare(value1, value2);
            }
        }



        private void GroupBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var groupBox = (DevExpress.Xpf.LayoutControl.GroupBox)sender;
            groupBox.State = groupBox.State == GroupBoxState.Normal ? GroupBoxState.Maximized : GroupBoxState.Normal;
        }




        internal void PlanDatabaseManage(SettingItem setting)
        {
            this.textBlockActivityName.Text = "系统管理";
            switch (setting)
            {
       
                //case SettingItem.PlanDatabase:
                //    this.buttonPlanDatabase.Visibility = System.Windows.Visibility.Collapsed;
                //    this.documentActivityInfo.Caption = "基础数据管理";
                //    this.documentActivityInfo.Content = this.WrapperModule(new CO_IA.UI.PlanDatabase.PlanDatabaseModule());
                //    break;
                //case SettingItem.Template:
                //    this.buttonTemplate.Visibility = System.Windows.Visibility.Collapsed;
                //    this.documentActivityInfo.Caption = "模版管理";
                //    this.documentActivityInfo.Content = this.WrapperModule(new CO_IA.UI.PlanDatabase.TemplateModule());
                //    break;
            }
        }

        private Object WrapperModule(object module,string dutyCode=null)
        {
            Microsoft.Windows.Controls.BusyIndicator busyIndicator = new Microsoft.Windows.Controls.BusyIndicator();
            busyIndicator.Content = module;
            if (!Utility.HasDuty(dutyCode))
            {
                if (module is AT_BC.Client.Extensions.EditableUserControl)
                {
                    (module as AT_BC.Client.Extensions.EditableUserControl).IsReadOnly = true;
                }
            }
            
            return busyIndicator;
        }

        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            this.AdjustWindowArea();
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("确实要关闭当前页面吗", "关闭提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void buttonMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        public override void EndInit()
        {
            base.EndInit();
            this.AdjustWindowArea();
        }

        private void AdjustWindowArea()
        {
            this.Left = 0;//设置位置
            this.Top = 0;
            Rect rect = SystemParameters.WorkArea;//获取工作区大小
            this.Width = rect.Width;
            this.Height = rect.Height;
        }

        private void buttonStationPlan_Click(object sender, RoutedEventArgs e)
        {
            this.SetupModule(this.GetUIBuilder().BuildStationPlanning, sender, "台站预案");
            //this.SetupModule(this.GetUIBuilder().BuildStationPlanning, sender, "台站预案");
        }

        private IActivityUIBuilder GetUIBuilder()
        {
            return Utility.GetUIFactory().GetUIBuilder(this.Activity.ActivityType);
        }

        private void SetupModule(Func<UIElement> GenerateModule, object sender, string caption,string dutyCode=null)
        {
            if (string.IsNullOrWhiteSpace(caption))
            {
                throw new Exception("标题不能为空!");
            }
            foreach (var layout in this.layoutGroup.Items)
            {
                if (layout is DocumentGroup)
                {
                    var documentGroup = layout as DocumentGroup;
                    foreach (var panel in documentGroup.Items)
                    {
                        if (caption.Equals(panel.Caption) && panel.Tag == sender)
                        {
                            panel.IsActive = true;
                            return;
                        }
                    }
                }
            }
            DevExpress.Xpf.Docking.DocumentPanel documentPanel = new DevExpress.Xpf.Docking.DocumentPanel();

            documentPanel.Content = this.WrapperModule(GenerateModule(),dutyCode);
            documentPanel.ClosingBehavior = DevExpress.Xpf.Docking.ClosingBehavior.ImmediatelyRemove;
            documentPanel.IsActive = true;
            documentPanel.Caption = caption;
            documentPanel.Tag = sender;
            this.documentGroup.Add(documentPanel);
        }

        /// <summary>
        /// 监测预案入口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonMonitorPlan_Click(object sender, RoutedEventArgs e)
        {
            this.SetupModule(this.GetUIBuilder().BuildMonitorPlanning, sender, "监测预案");
        }

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSetting_Click(object sender, RoutedEventArgs e)
        {
            //this.SetupModule(() => { return new CO_IA.UI.Setting.SettingModule(); }, sender, "基础信息设置");
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                //this.SetupModule<CO_IA.UI.ActivityManage.ActivityManageModule>("活动管理");
            }
        }

        /// <summary>
        /// 人员预案入口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonPersonPlan_Click(object sender, RoutedEventArgs e)
        {
            this.SetupModule(this.GetUIBuilder().BuildStaffPlanning, sender, "人员预案");
            //this.SetupModule<CO_IA.UI.PersonPlan.PersonPlanModule>("人员预案");
        }

  

        // 规章文件
        private void buttonFiles_Click(object sender, RoutedEventArgs e)
        {
            this.SetupModule(this.GetUIBuilder().BuildFileManage, sender, "工作文件");
            //this.SetupModule<CO_IA.UI.ActivityManage.TaskManage.OfficeManage>("相关文件");
        }

        //任务管理
        private void buttonTask_Click(object sender, RoutedEventArgs e)
        {
            //this.SetupModule(() => { return new CO_IA.UI.MonitorTask.TaskManageModule(); }, sender, "任务管理");
            //this.SetupModule<CO_IA.UI.ActivityManage.TaskManage.TaskList>("任务管理");
        }

        //频率预案
        private void buttonFreqPlan_Click(object sender, RoutedEventArgs e)
        {
            this.SetupModule(this.GetUIBuilder().BuildFreqPlanning, sender, "频率预案");
     
        }


        //活动总结
        private void buttonActivitySummarize_Click(object sender, RoutedEventArgs e)
        {
            this.SetupModule(this.GetUIBuilder().BuildSummarize, sender, "活动总结");
        }




        //量化考核
        private void buttonStatistic2_Click(object sender, RoutedEventArgs e)
        {
            this.SetupModule(this.GetUIBuilder().BuildStatistic2, sender, "量化考核");
        }




        //统计
        private void buttonStatistic_Click(object sender, RoutedEventArgs e)
        {
            this.SetupModule(this.GetUIBuilder().BuildStatistic, sender, "数据统计");
        }


        /// <summary>
        /// 信息维护
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonPlanBase_Click(object sender, RoutedEventArgs e)
        {
            //this.SetupModule(() => { return new CO_IA.UI.PlanDatabase.TemplateModule(); }, sender, "信息维护");


            this.SetupModule(this.GetUIBuilder().BuildPlanDatabase, sender, "信息维护");



        }





        private void buttonTemplate_Click(object sender, RoutedEventArgs e)
        {
            //this.SetupModule(() => { return new CO_IA.UI.PlanDatabase.TemplateModule(); }, sender, "模版管理");
        }




        public CO_IA.Data.Activity Activity
        {
            get;
            private set;
        }

  

        public GS_MapBase.MapControl CreateMap()
        {
            return new GS_MapBase.MapControl { ServiceUrl = CO_IA.Client.RiasPortal.Current.MapConfig.ElectricUrl };
        }
 
        private void rectangleContainer_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void buttonPlanDatabase_Click(object sender, RoutedEventArgs e)
        {
            //this.SetupModule(() => { return new CO_IA.UI.PlanDatabase.PlanDatabaseModule(); }, sender, "基础数据管理");
        }

    
        private void buttonSchedule_Click(object sender, RoutedEventArgs e)
        {
            //this.SetupModule(this.GetUIBuilder().BuildSchedule, sender, "日程安排");
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            AT_BC.Common.CheckableCheckBoxHelper.CheckAll(sender, e);
        }


        private void CheckableDataCheckedCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

 


        public ExecutorLoginInfo GetExecutorLoginInfo()
        {
            throw new NotImplementedException();
        }

     
    }
}
