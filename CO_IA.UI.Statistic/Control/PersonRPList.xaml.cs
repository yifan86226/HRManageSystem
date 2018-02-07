using CO_IA.Data;
using CO_IA.Data.PlanDatabase;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CO_IA.UI.Statistic
{
    /// <summary>
    /// PersonList.xaml 的交互逻辑
    /// </summary>
    public partial class PersonRPList : UserControl
    {
        private string nameid;
        public string NameID
        {
            get
            {
                return nameid;
            }
            set
            {
                nameid = value;
            
            }
        } 

        private string incident;
        public string Incident
        {
            get
            {
                return incident;
            }
            set
            {
                if (value != "合计" && value!="数据详情")

                {
                    incident = value;

                }

            }
        }


        private string fromdate;
        public string FromDate
        {
            get
            {
                return fromdate;
            }
            set
            {
                fromdate = value;
            }
        }





        private string todate;
        public string ToDate
        {
            get
            {
                return todate;
            }
            set
            {
                todate = value;
            }
        }








        public PersonRPList()
        {
            InitializeComponent();
        }




        public void LoadData()
        {
            this.dg_GrouperList.ItemsSource = null;
            
            List<PersonRewardPunishInfo> itemPersonList;


            DataManager.Public.PersonRewardPunishInfoModel model = new DataManager.Public.PersonRewardPunishInfoModel();
            itemPersonList = model.GetPersonRewardPunishInfos("", incident, nameid, fromdate, todate, "");

            if (itemPersonList != null && itemPersonList.Count > 0)
            {
                this.dg_GrouperList.ItemsSource = itemPersonList.Where(item => !string.IsNullOrEmpty(item.NAME));

            }
             
        }
    }
}
