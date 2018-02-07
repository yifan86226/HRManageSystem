using CO_IA.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace CO_IA.MonitoringCollecting
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private MainWindow _thisMainWindow = null;
        public App()
        {

        }
        //public App()
        //{
        //    _thisMainWindow = new MainWindow(LoadMainWindow);
        //}
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            try
            {
                SystemConfigLoader.RegisterSystemConfig();
              
            }
            catch (Exception ex)
            {
                LoadMainWindow(false);
                return;
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
            wnd.InvokeActivity += new Action<CO_IA.Data.Activity>(activity =>
            {
                LoadMainWindow(activity,SystemLoginService.CurrentActivityPlace);
            });
            wnd.ShowDialog();
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        }
        void LoadMainWindow(CO_IA.Data.Activity p_activity , CO_IA.Data.ActivityPlace p_activityPlace)
        {
            if (_thisMainWindow != null)
                _thisMainWindow.Hide();

            _thisMainWindow = new MainWindow(p_activityPlace, LoadMainWindow);      
            //MainWindow thisWindow = new MainWindow(LoadMainWindow);
            this.MainWindow = _thisMainWindow;
            App.Current.ShutdownMode = System.Windows.ShutdownMode.OnLastWindowClose;
            bool a = _thisMainWindow.IsInitialized;
            _thisMainWindow.Show();
            _thisMainWindow.LoadActivity(p_activity);
        }
    }
}
