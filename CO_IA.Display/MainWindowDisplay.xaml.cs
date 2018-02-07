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
using DataManager.Public;
using CO_IA.Data.PlanDatabase;
using CO_IA.UI.PlanDatabase;
using System.Threading;


namespace CO_IA.RIAS
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindowDisplay : Window, CO_IA.Client.IRiasModuleContainer//,Client.IMessageReceiver
    {


   


        /// <summary>
        /// 绑定TreeView的数据源
        /// </summary>
        List<PP_OrgInfo> itemList = new List<PP_OrgInfo>();



        public MainWindowDisplay()
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
          

                return;
            }
            throw new Exception("已存在打开的活动,不能重新打开");

         
        }



        private void listBoxActivityType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {



            PersonModel model = new PersonModel();
            List<PersonBasicInfo> personList = new List<PersonBasicInfo>();

            ListBoxItem item = listBoxActivityType.SelectedItem as ListBoxItem;

            string persontype = "";
             if (item.Name == "itemPerson_GB")
            {
                persontype = "干部";

            }
            else if (item.Name == "itemPerson_GJSG")
            {
                persontype = "高级士官";
            }

            else if (item.Name == "itemPerson_SJJSZ")
            {
                persontype = "四级军士长";
            }
            else if (item.Name == "itemPerson_SS")
            {
                persontype = "上士";
            }
            else if (item.Name == "itemPerson_ZS")
            {
                persontype = "中士";
            }
            else if (item.Name == "itemPerson_XS")
            {
                persontype = "下士";
            }
            else if (item.Name == "itemPerson_SDB")
            {
                persontype = "上等兵";
            }
            else if (item.Name == "itemPerson_XB")
            {
                persontype = "新兵";
            }

            personList = model.GetPersonBasicInfos(persontype, "", "");
            //界面赋值

            flc_List.ItemsSource = null;
            flc_List.ItemsSource = personList;





            //foreach (PersonBasicInfo fpb in personList)
            //{
            //    Thread oThread = new Thread(new ParameterizedThreadStart(GetPersonPic));

            //    oThread.IsBackground = true;
            //    oThread.Start(fpb);
            //}





        }


        private static void GetPersonPic(object obj)
        {
            PersonBasicInfo fpb = obj as PersonBasicInfo;

            if (string.IsNullOrEmpty(fpb.A2) == false && File.Exists(AppDomain.CurrentDomain.BaseDirectory+fpb.A2) == true)
            {

                string md5code = DataManager.Public.ImageHelper.GetMD5HashFromFile(AppDomain.CurrentDomain.BaseDirectory+fpb.A2);

                if (md5code.Equals(fpb.A1))
                {
                    fpb.PHOTO = DataManager.Public.ImageHelper.BitmapToBytes(new System.Drawing.Bitmap(AppDomain.CurrentDomain.BaseDirectory+fpb.A2));
                }

            }
            else 
            {
                PersonModel model = new PersonModel();

                fpb= model.GetPersonBasicInfos("", fpb.NAMEID,"")[0];


                //fpb.PHOTO = DataManager.Public.ImageHelper.YaSuo(fpb.PHOTO);

                ////保存图片
                //string filename = AppDomain.CurrentDomain.BaseDirectory + "\\Images\\" + fpb.NAMEID;
                //string fileFullname = DataManager.Public.ImageHelper.CreateImageFromBytes(filename, fpb.PHOTO);

                //string md5Code = DataManager.Public.ImageHelper.GetMD5HashFromByte(fpb.PHOTO);

                //fpb.A1 = md5Code;

                //fpb.A2 = fileFullname.Replace(AppDomain.CurrentDomain.BaseDirectory, "");


                //model.ModifyPersonBasicInfo(fpb);

            }

        }


        /// <summary>
        /// 大事记弹出事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void img_DSJ_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            e.Handled = true;
            Image img = sender as Image;

            string nameid = img.Tag.ToString();
        
            PersonRewardPunishInfoForPersonDialog dialog = new PersonRewardPunishInfoForPersonDialog(nameid);

            dialog.Show();

          
        }

 
     




        private void GroupBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var groupBox = (DevExpress.Xpf.LayoutControl.GroupBox)sender;
            groupBox.State = groupBox.State == GroupBoxState.Normal ? GroupBoxState.Maximized : GroupBoxState.Normal;


            if (groupBox.State == GroupBoxState.Maximized)
            {

                PersonBasicInfo binfo = groupBox.DataContext as PersonBasicInfo;

                string eventList = InitGreatEventData(binfo.NAMEID);

                binfo.GREATEVENT = eventList;

            }


        }



        private string InitGreatEventData(string nameid)
        {

            string str = "";
            PersonRewardPunishInfoModel model = new PersonRewardPunishInfoModel();
            List<PersonRewardPunishInfo> list = model.GetPersonRewardPunishInfos("", "", nameid,"","", "");



            foreach (PersonRewardPunishInfo item in list)
            {
         

                string datestr = "";
                try
                {
                    datestr = Convert.ToDateTime(item.RPTIME).ToString("yyyy年MM月dd天") + " ， ";
                }
                catch
                {

                }

                string rpStr = "奖励";
                if (item.RPTYPE == "0")
                {
                    rpStr = "奖励";
                }
                else if (item.RPTYPE == "1")
                {
                    rpStr = "扣除";
                }

                str += "  " + datestr + item.INCIDENT + " " + rpStr + "   " + item.FRACTION + "分。 \r\n";

            }
            return str;

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

  
 

        //任务管理
        private void buttonTask_Click(object sender, RoutedEventArgs e)
        {
            //this.SetupModule(() => { return new CO_IA.UI.MonitorTask.TaskManageModule(); }, sender, "任务管理");
            //this.SetupModule<CO_IA.UI.ActivityManage.TaskManage.TaskList>("任务管理");
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
