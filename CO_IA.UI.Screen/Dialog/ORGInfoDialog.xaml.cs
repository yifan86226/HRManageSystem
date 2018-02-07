
using CO_IA.Client;
using CO_IA.Data;
using CO_IA.Types;
using CO_IA.UI.Screen.Control;
using I_CO_IA.FreqStation;
using PT_BS_Service.Client.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace CO_IA.UI.Screen.Dialog
{
    /// <summary>
    /// 参保设备 详细信息
    /// </summary>
    public partial class ORGInfoDialog : Window
    {

        public ORGInfoDialog(ActivityOrganization org,ActivityEquipment equ)
        {
            InitializeComponent();

            orgInfo.ORGDataContent = org;
            orgInfo.IsReadOnly = true;
            EquipmentDetail.DataContext = equ;
           
            EquipmentDetail.IsReadOnly = true;
        }

        
       
       

    
    }
}