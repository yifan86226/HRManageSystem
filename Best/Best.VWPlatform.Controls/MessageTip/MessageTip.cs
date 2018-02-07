using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Best.VWPlatform.Controls.MessageTip
{
    public class MessageTip
    {
        public static void ShowTip(string pStrMess)
        {
            MsgTip mt = new MsgTip(pStrMess);
            mt.Owner = Application.Current.MainWindow;
            mt.Show();
        }
    }
}
