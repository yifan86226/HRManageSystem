using CO_IA.Client;
using AT_BC.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using AT_BC.Client.Extensions;
namespace CO_IA.RIAS
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {

 



        public App()
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CN");
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
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




            Data.Activity activity = new Data.Activity();
            activity.Guid = "66AAEEF3-9257-4A16-A793-E7B83A69FFB9";
            MainWindowDisplay mainWindow = new MainWindowDisplay();
            mainWindow.LoadActivity(activity);


            mainWindow.Show();


        }

        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (!e.Handled)
            {
                var comException = e.Exception as System.Runtime.InteropServices.COMException;
                if (comException != null && comException.ErrorCode == -2147221040)///OpenClipboard HRESULT:0x800401D0 (CLIPBRD_E_CANT_OPEN))
                {
                    e.Handled = true;
                    return;
                }
                var message = e.Exception.GetExceptionMessage();
                MessageBox.Show(message);
                e.Handled = true;
            }
        }
    }
}
