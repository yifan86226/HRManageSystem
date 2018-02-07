using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Best.VWPlatform.Controls.MessageBox
{
    public enum EMessageBoxType
    {
        Alert,Confirm
    }

    public enum EResultType
    {
        OK,Cancel
    }
    public class CMessageBox
    {
        public static EResultType Show(EMessageBoxType pType, string pMsg, params string[] ps)
        {
            EResultType re = EResultType.Cancel;
            
            switch (pType)
            {
                case EMessageBoxType.Alert:
                    CustomAlertDialog cad = new CustomAlertDialog(pMsg, ps);
                    cad.ShowDialog();
                    break;
                case EMessageBoxType.Confirm:
                    CustomConfirmDialog ccd = new CustomConfirmDialog(pMsg, ps);
                    ccd.ShowDialog();
                    re = ccd.ConfirmResult;
                    break;
                default:
                    break;
            }

            return re;
        }
    }


}
