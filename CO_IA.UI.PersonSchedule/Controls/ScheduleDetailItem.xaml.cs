using CO_IA.Client;
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

namespace CO_IA.UI.PersonSchedule
{
    /// <summary>
    /// ScheduleDetailItem.xaml 的交互逻辑
    /// </summary>
    public partial class ScheduleDetailItem : UserControl
    {
        ScheduleDetail[] schedetailInput;
        ScheduleDetail[] _schedetail;
        public ScheduleDetailItem(ScheduleDetail[] schedetail)
        {
            InitializeComponent();

            this.Loaded += ScheduleDetailItem_Loaded;


            this.dg_ScheduleDetail.RowHeight = double.NaN;
            
            schedetailInput = schedetail;
            
        }

        void ScheduleDetailItem_Loaded(object sender, RoutedEventArgs e)
        {
            string areaid = "";
            if (schedetailInput == null || schedetailInput.Length == 0)
                return;
            List<ScheduleDetail> details = schedetailInput.ToList();
            List<ScheduleDetail> newdetails = new List<ScheduleDetail>();
            for (int i = 0; i < schedetailInput.Length; i++)
            {
                if (schedetailInput[i].ScheduleOrgs != null && schedetailInput[i].ScheduleOrgs.Length != 0)
                {
                    var orgs = schedetailInput[i].ScheduleOrgs.OrderBy(p => p.AREA_GUID).ToArray();
                    ScheduleDetail newitem = null;
                    for (int j = 0; j < orgs.Length; j++)
                    {
                        if (areaid != orgs[j].AREA_GUID)
                        {
                            if (newitem != null)
                            {
                                newitem.GROUPS = newitem.GROUPS.Trim(',');
                                newdetails.Add(newitem);
                            }
                            areaid = orgs[j].AREA_GUID;

                            newitem = new ScheduleDetail();
                            newitem.CONTENT = schedetailInput[i].CONTENT;
                            newitem.TIMEDESC = schedetailInput[i].STARTTIME.ToString("HH:mm") + " - " + schedetailInput[i].STOPTIME.ToString("HH:mm");
                            newitem.AREAS = orgs[j].AREA_GUID;
                            newitem.GROUPS = orgs[j].GROUP_GUID + ",";

                        }
                        else
                        {
                            newitem.GROUPS += orgs[j].GROUP_GUID + ",";
                        }
                        if (j == orgs.Length - 1)
                        {
                            newitem.GROUPS = newitem.GROUPS.Trim(',');
                            newdetails.Add(newitem);
                            areaid = "";
                        }
                    }
                }
            }

            _schedetail = newdetails.ToArray();
            //for(int i=0;i<schedetail.Length;i++)
            //{
            //    ScheduleDetail newitem = new ScheduleDetail();
            //    newitem = AT_BC.Data.Helpers.DataContractSerializeHelper.Clone<ScheduleDetail>(schedetail[i]);
            //    newitem.TIMEDESC =  newitem.STARTTIME.ToString("HH:mm") + " - " + newitem.STOPTIME.ToString("HH:mm");
            //    _schedetail[i] = newitem;
            //}

            ActivityPlaceInfo[] places = Utility.GetPlacesByActivityId(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
            List<PP_OrgInfo> nodes = new List<PP_OrgInfo>();
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
            {
                //更新当前节点
                nodes = channel.GetPP_OrgInfos(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
            });
            foreach (var itm in _schedetail)
            {
                itm.AREAS = getAreasDesc(itm.AREAS, places);
                itm.GROUPS = getGroupsDesc(itm.GROUPS, nodes.ToArray());
            }

            dg_ScheduleDetail.ItemsSource = _schedetail;
        }
        private string getAreasDesc(string sareas, ActivityPlaceInfo[] places)
        {
            if(string.IsNullOrEmpty(sareas))
                return "";
            string areas = "";
            foreach (var itm in places)
            {
                if (sareas.IndexOf(itm.Guid) != -1)
                {
                    areas += itm.Name + ",";
                }
            }
            areas = areas.Trim(',');
            return areas;
        }
        private string getGroupsDesc(string sgroups, PP_OrgInfo[] orginfo)
        {
            if (string.IsNullOrEmpty(sgroups))
                return "";
            string groups = "";
            foreach (var itm in orginfo)
            {
                if (sgroups.IndexOf(itm.GUID)!=-1)
                {
                    groups += itm.NAME + ",";
                }
            }
            groups = groups.Trim(',');
            return groups;
        }

    }
}
