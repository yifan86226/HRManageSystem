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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CO_IA.UI.PersonSchedule.Foreign
{
    /// <summary>
    /// PersonList.xaml 的交互逻辑
    /// </summary>
    public partial class PersonLeaveList : UserControl
    {
        private string orgId;
        public string OrgID
        {
            get
            {
                return orgId;
            }
            set
            {
                orgId = value;
                //LoadData();
            }
        }


        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;

            }
        }


        private string leavetype;
        public string Leave_Type
        {
            get
            {
                return leavetype;
            }
            set
            {
                leavetype = value;
            }
        }






        public PersonLeaveList()
        {
            InitializeComponent();
        }

        //void LoadData()
        //{
        //    this.dg_GrouperList.ItemsSource = null;
        //    if (!string.IsNullOrEmpty(OrgID))
        //    {
        //        List<PP_PersonInfo> itemPersonList;
        //        PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
        //        {
        //            itemPersonList = channel.GetPP_PersonInfos(OrgID);
        //            if(itemPersonList!=null&&itemPersonList.Count>0)
        //            this.dg_GrouperList.ItemsSource = itemPersonList.Where(item => !string.IsNullOrEmpty(item.NAME));
        //        });

        //    }
        //    else
        //    {
        //        List<PP_OrgInfo> nodes = new List<PP_OrgInfo>();
        //        PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
        //        {
        //            //更新当前节点
        //            nodes = channel.GetPP_OrgInfos(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
        //        });
        //        if (nodes!=null&&nodes.Count > 0)
        //        {
        //            List<PP_PersonInfo> itemPersonList = new List<PP_PersonInfo>();
        //            List<PP_PersonInfo> PersonList;
        //            foreach (var org in nodes)
        //            {                       
        //                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
        //                {
        //                    PersonList = channel.GetPP_PersonInfos(org.GUID);   
        //                    if(PersonList!=null&&PersonList.Count>0)
        //                        itemPersonList.AddRange(PersonList.Where(item=>!string.IsNullOrEmpty(item.NAME)));
        //                });
        //            }
        //            this.dg_GrouperList.ItemsSource = itemPersonList;
        //        }
        //    }
        //}



        public void LoadLeaveData()
        {
            this.dg_GrouperList.ItemsSource = null;
            //if (!string.IsNullOrEmpty(Name))
            //{
            List<PP_PersonLeaveInfo> itemPersonList;


            DataManager.Public.StatisticModel model = new DataManager.Public.StatisticModel();
            itemPersonList = model.GetPP_PersonInfos(Name, Leave_Type);

            if (itemPersonList != null && itemPersonList.Count > 0)
            {
                this.dg_GrouperList.ItemsSource = itemPersonList.Where(item => !string.IsNullOrEmpty(item.NAME));

            }
            //}
        }
    }
}
