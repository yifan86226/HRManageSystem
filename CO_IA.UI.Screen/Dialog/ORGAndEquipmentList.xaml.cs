using CO_IA.Client;
using CO_IA.Data;
using CO_IA.UI.PlanDatabase;
using CO_IA.UI.PlanDatabase.Equipments;
using CO_IA.UI.PlanDatabase.ORG;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System;
using AT_BC.Data;
using PT_BS_Service.Client.Framework;
using I_CO_IA.FreqStation;
using CO_IA.UI.FreqStation.FreqPlan;

namespace CO_IA.UI.Screen.Dialog
{
    /// <summary>
    /// ORGAndEquipmentManage.xaml 的交互逻辑
    /// </summary>
    public partial class ORGAndEquipmentList : Window
    {
        private OrgQueryCondition orgquerycondition = new OrgQueryCondition();
        private EquipmentLoadStrategy eququerycondition = new EquipmentLoadStrategy()
        {
            ActivityGuid = RiasPortal.ModuleContainer.Activity.Guid,
        };

        /// <summary>
        /// 当前活动
        /// </summary>
        private Activity CurrentActivity { get; set; }

        public string CurrentActivityPlaceID
        {
            get;
            set;
        }

        public ActivityOrganization[] ActivityOrgSource
        {
            get { return orgdatagrid.ItemsSource as ActivityOrganization[]; }
        }

        public ActivityOrganization SelectActivityORG
        {
            get { return orgdatagrid.SelectedItem as ActivityOrganization; }
        }

        public ORGAndEquipmentList(string avtivityplaceid)
        {
            InitializeComponent();
            this.CurrentActivityPlaceID = avtivityplaceid;
            this.CurrentActivity = RiasPortal.ModuleContainer.Activity;
            orgquerycondition.ActivityGuid = CurrentActivity.Guid;
            InitQueryCondition(eququerycondition);
            GetActivityOrgs(orgquerycondition);
            InitEvent();
        }

        private void InitEvent()
        {
            //单位      

            equipmentListControl.DoubleClick += equipmentListControl_DoubleClick;
        }



        private void InitQueryCondition(EquipmentLoadStrategy condition)
        {
            condition.ActivityGuid = CurrentActivity.Guid;
            condition.PlaceGuid = CurrentActivityPlaceID;
            if (SelectActivityORG != null)
            {
                condition.OrgName = SelectActivityORG.Name;
            }
        }

        #region 单位相关方法


        /// <summary>
        /// 查询单位信息
        /// </summary>
        /// <param name="condition"></param>
        private void GetActivityOrgs(OrgQueryCondition condition)
        {
            ActivityOrganization[] source = GetActivityOrgSources(condition);

            orgdatagrid.ItemsSource = source;

            if (source != null && source.Length > 0)
            {
                orgdatagrid.SelectedIndex = 0;
            }
            else
            {
                equipmentListControl.DataContext = null;
            }
        }

        private ActivityOrganization[] GetActivityOrgSources(OrgQueryCondition condition)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqStation, ActivityOrganization[]>(channel =>
            {
                return channel.GetActivityOrgs(condition);
            });
        }
        private void orgdatagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectActivityORG != null)
            {
                eququerycondition = new EquipmentLoadStrategy();
                InitQueryCondition(eququerycondition);
                GetActivityEquipments(eququerycondition);
            }
        }

        #endregion

        #region 设备相关方法
        

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="obj"></param>
        private void equipmentListControl_DoubleClick(ActivityEquipment obj)
        {
            EquipmentManageDialog addequdialog = new EquipmentManageDialog();
            addequdialog.AllowEdit = false;
            addequdialog.DataContext = obj;
            addequdialog.OnSaveEvent += (equ) =>
            {
                ActivityEquipment actequ = new ActivityEquipment();
                actequ.ActivityGuid = this.CurrentActivity.Guid;
                actequ.PlaceGuid = this.CurrentActivityPlaceID;
                actequ.CopyFrom(equ);
                bool result = SaveActivityEquipment(actequ);
                if (result)
                {
                    InitQueryCondition(eququerycondition);
                    GetActivityEquipments(eququerycondition);
                    MessageBox.Show("保存成功");
                }
                return result;
            };
            addequdialog.ShowDialog();
        }

       

      
      
       
        #endregion

        /// <summary>
        /// 查询设备
        /// </summary>
        /// <param name="condition"></param>
        private void GetActivityEquipments(EquipmentLoadStrategy condition)
        {
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqStation>(channel =>
            {
                ActivityEquipment[] sources = channel.GetActivityEquipments(condition);
                equipmentListControl.DataContext = sources;
            });
        }

        /// <summary>
        /// 保存设备方法
        /// </summary>
        /// <param name="equ"></param>
        /// <returns></returns>
        private bool SaveActivityEquipment(ActivityEquipment equ)
        {
            try
            {
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqStation>(channel =>
                {
                    channel.SaveActivityEquipment(equ);
                });
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetExceptionMessage(), "保存失败");
                return false;
            }
        }

       
      
    }
}
