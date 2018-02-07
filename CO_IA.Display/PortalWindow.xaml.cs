using AT_BC.Common.Controls;
using CO_IA.Client;
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
using AT_BC.Types;
using CO_IA.Types;
using PT_BS_Service.Client.Core;
using PT.Toolkit.Portable.Collections;
using PT.Profile.Interface;
using System.IO;

namespace CO_IA.RIAS
{
    /// <summary>
    /// PortalWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PortalWindow : Window
    {

        public PortalWindow()
        {
            InitializeComponent();




            this.Loaded += PortalWindow_Loaded;
        }

        void PortalWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Init();
        }

        private void Init()
        {


            #region 读取用户权限
            //invoker.Add<ISubsystem, StringList>(channel =>
            //    {
            //        return channel.GetUsableAuthoritiesBySubsystemID(RiasPortal.Current.UserSetting.UserID, CO_IA.Public.SubSystemIDs.Rias);
            //    },
            //    result =>
            //    {
            //        if (result.IsValid)
            //        {
            //            var activities = result.Result;
            //            RiasPortal.Current.UserSetting.UserRights = result.Result.ToArray();
            //        }
            //    }, "正在加载用户权限");
            #endregion


        }

        private void buttonMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("确实要关闭系统吗", "关闭系统", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                App.Current.Shutdown();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //if (username.Text == "admin" && password.Text == "1qaz@WSX")
            //{
                Data.Activity activity = new Data.Activity();
                activity.Guid = "66AAEEF3-9257-4A16-A793-E7B83A69FFB9";
                MainWindow mainWindow = new MainWindow();
                mainWindow.LoadActivity(activity);


                mainWindow.Show();

                this.Close();
            //}
            //else
            //{
            //    this.tb_SM.Text = "账号或者密码错误，请确认后再登陆！";
            //}
        }
    }
}
