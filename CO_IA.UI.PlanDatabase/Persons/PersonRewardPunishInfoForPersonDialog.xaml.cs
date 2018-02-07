using CO_IA.Data;
using CO_IA.Data.PlanDatabase;
using DataManager.Public;
using DevExpress.Xpf.LayoutControl;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

namespace CO_IA.UI.PlanDatabase
{
    /// <summary>
    /// ORGQueryDialog.xaml 的交互逻辑
    /// </summary>
    public partial class PersonRewardPunishInfoForPersonDialog : Window
    {

      



        public PersonRewardPunishInfoForPersonDialog(string nameid)
        {
            InitializeComponent();

            InitData(nameid);
        }

        private void InitData(string nameid)
        {
          
            PersonRewardPunishInfoModel model = new PersonRewardPunishInfoModel();
            List<PersonRewardPunishInfo> list = model.GetPersonRewardPunishInfos("", "", nameid,"", "", "");


            foreach (PersonRewardPunishInfo item in list)
            {
                LayoutItem li = new LayoutItem();
                TextBlock tb = new TextBlock();

                string datestr = "";
                try
                {
                    datestr = Convert.ToDateTime(item.RPTIME).ToString("yyyy年MM月dd天") +" ， ";
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
                tb.Text ="  "+ datestr  + item.INCIDENT + " " + item.RPTYPE + "   " + item.FRACTION + "分。";
                tb.TextWrapping = TextWrapping.Wrap;

                li.Content = tb;

                this.LayoutGroup_Container.Children.Add(li);

            }


        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }



        
    }
}
