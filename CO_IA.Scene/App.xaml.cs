using CO_IA.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using CO_IA.Client;

namespace CO_IA.Scene
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CN");
            this.DispatcherUnhandledException+=App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            DevExpress.Xpf.Core.ThemeManager.ApplicationThemeName = DevExpress.Xpf.Core.Theme.MetropolisLightName;
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            if (exception != null)
            {
                var message = exception.GetExceptionMessage();
                MessageBox.Show(message);
            }
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            try
            {
                CO_IA.Client.SystemConfigLoader.RegisterSystemConfig();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                App.Current.Shutdown();
            }
            LoadMainWindow(true);

        }
        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (!e.Handled)
            {
                MessageBox.Show(e.Exception.Message);
                e.Handled = true;
            }
        }
        void LoadMainWindow(bool p_isLoadUserInfo)
        {
            LoadActivity wnd = new LoadActivity(p_isLoadUserInfo);
            wnd.InvokeActivityPlace += new Action<CO_IA.Data.Activity,ActivityPlace,PP_OrgInfo>((activity,place,orgInfo) =>
            {
                MainWindow thisWindow = new MainWindow();
                this.MainWindow = thisWindow;
                //App.Current.ShutdownMode = System.Windows.ShutdownMode.OnLastWindowClose;
                thisWindow.Show();

                orgInfo = new PP_OrgInfo();
                thisWindow.LoadActivityPlace(activity, place,orgInfo);
            });
            wnd.ShowDialog();
        }

    }
}
