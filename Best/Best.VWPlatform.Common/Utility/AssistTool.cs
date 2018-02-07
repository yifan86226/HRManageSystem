using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Best.VWPlatform.Assist;

namespace Best.VWPlatform.Common.Utility
{
    public class AssistTool
    {
        private static AssistWnd _wnd = null;

        public static void Init()
        {
            if (ConfigurationManager.AppSettings["ShowDebugWnd"].Equals("1"))
            {
                if (_wnd == null)
                {
                    _wnd = new AssistWnd();
                    _wnd.Show();
                    _wnd.Visibility = Visibility.Hidden;
                }                
            }
        }

        public static void ShowWnd()
        {
            if (_wnd != null)
            {
                _wnd.Visibility = Visibility.Visible;                
            }
        }

        public static void CloseWnd()
        {
            if (_wnd != null)
            {
                _wnd.Close();
            }
        }

        public static void WriteOutput(string pStrOutput)
        {
            if (_wnd != null)
            {
                _wnd.WriteLine(pStrOutput);
            }
        }
    }
}
