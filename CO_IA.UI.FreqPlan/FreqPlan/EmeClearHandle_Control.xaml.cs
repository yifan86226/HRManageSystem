using CO_IA.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace CO_IA.UI.FreqPlan.FreqPlan
{
    /// <summary>
    /// EmeClearHandle_Control.xaml 的交互逻辑
    /// </summary>
    public partial class EmeClearHandle_Control : UserControl
    {
        private Dictionary<string, string> SaveStateDic = new Dictionary<string, string>();

        private List<EmeClearState> _emeClearStateList = new List<EmeClearState>();


        public List<EmeClearState> EmeClearStateList
        {

            get { return _emeClearStateList; }
            set { _emeClearStateList = value; }
        }

        public List<EmeClearInfo> EmeClearInfoList
        {

            get { return xEMEClearGrid.ItemsSource as List<EmeClearInfo>; }
            set { xEMEClearGrid.ItemsSource = null; xEMEClearGrid.ItemsSource = value; }
        }

        private EmeClearQueryCondition condition = new EmeClearQueryCondition();
        private ActivityPlaceInfo activityPlaceInfo;

        public EmeClearHandle_Control()
        {
            InitializeComponent();

        }

        public EmeClearHandle_Control(ActivityPlaceInfo activityPlaceInfo)
        {
            InitializeComponent();


            EmeClearState cs1 = new EmeClearState();

            cs1.Name = "未作处理";
            cs1.Value = "0";

            _emeClearStateList.Add(cs1);


            EmeClearState cs2 = new EmeClearState();

            cs2.Name = "清理成功";
            cs2.Value = "1";

            _emeClearStateList.Add(cs2);



            EmeClearState cs3 = new EmeClearState();

            cs3.Name = "清理失败";
            cs3.Value = "2";

            _emeClearStateList.Add(cs3);

            this.activityPlaceInfo = activityPlaceInfo;

            condition.PlaceGuid = activityPlaceInfo.Guid;
            condition.NeedClear = "1";
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan>(channel =>
            {
                EmeClearInfoList = channel.GetEmeClearHandleInfoList(condition);
            });
        }

        private void xbtnQuery_Click(object sender, RoutedEventArgs e)
        {
            EmeClearHandleQueryConditionDialog dialog = new EmeClearHandleQueryConditionDialog();

            dialog.ShowDialog(this);
            if (dialog.IsSuccuessFull == true)
            {
                if (activityPlaceInfo != null && string.IsNullOrEmpty(activityPlaceInfo.Guid) == false)
                {
                    dialog.Condition.PlaceGuid = activityPlaceInfo.Guid;
                }
                dialog.Condition.NeedClear = "1";
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan>(channel =>
                {
                    EmeClearInfoList = channel.GetEmeClearHandleInfoList(dialog.Condition);
                });
            }

        }


        private void xbtnSave_Click(object sender, RoutedEventArgs e)
        {
            List<EmeClearState> statelist = new List<EmeClearState>();

            foreach (string guid in SaveStateDic.Keys)
            {

                EmeClearState cs = new EmeClearState();

                cs.Name = guid;
                cs.Value = SaveStateDic[guid];

                statelist.Add(cs);
            }

            bool isSuccessfule = false;
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan>(channel =>
            {
                isSuccessfule = channel.UpdateEmeClearStateInfoList(statelist);
            });

            condition = new EmeClearQueryCondition();
            condition.PlaceGuid = activityPlaceInfo.Guid;
            condition.NeedClear = "1";
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan>(channel =>
            {
                EmeClearInfoList = channel.GetEmeClearHandleInfoList(condition);
            });

            MessageBox.Show("保存完毕！");
        }



        /// <summary>
        /// 插入新的数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void xbtnAdd_Click(object sender, RoutedEventArgs e)
        {
            QueryEmeClear_Dialog dialog = new QueryEmeClear_Dialog(activityPlaceInfo);

            dialog.Closed += dialog_Closed;
            dialog.ShowDialog(this);
        }


        private void dialog_Closed(object sender, EventArgs e)
        {
            condition.PlaceGuid = activityPlaceInfo.Guid;
            condition.NeedClear = "1";
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan>(channel =>
            {
                EmeClearInfoList = channel.GetEmeClearHandleInfoList(condition);
            });
        }



        private void xfinishClear_Click(object sender, RoutedEventArgs e)
        {
            if (this.xEMEClearGrid.SelectedItem != null)
                ((EMEEnvironment)this.xEMEClearGrid.SelectedItem).ResultIsClear = "完成";
        }

        private ObservableCollection<EMEEnvironment> CreateEMEEnvironmentSource()
        {
            ObservableCollection<EMEEnvironment> eEmEnvironments = new ObservableCollection<EMEEnvironment>();
            EMEEnvironment eMEEnvironment = new EMEEnvironment();
            eMEEnvironment.Freq = 138.25;
            eMEEnvironment.SignalSource = "已建台站";
            eMEEnvironment.Department = "设台单位";
            eMEEnvironment.Address = "设台单位地址";
            eMEEnvironment.RelationMan = "王某某";
            eMEEnvironment.Phone = "13080000001";
            eMEEnvironment.IsLegal = "是";
            eMEEnvironment.IsClear = "是";
            eMEEnvironment.ResultIsClear = "完成";
            eEmEnvironments.Add(eMEEnvironment);

            eMEEnvironment = new EMEEnvironment();
            eMEEnvironment.Freq = 143.25;
            eMEEnvironment.SignalSource = "未知";
            eMEEnvironment.Department = "";
            eMEEnvironment.Address = "";
            eMEEnvironment.RelationMan = "";
            eMEEnvironment.Phone = "";
            eMEEnvironment.IsLegal = "否";
            eMEEnvironment.IsClear = "是";
            eMEEnvironment.ResultIsClear = "完成";
            eEmEnvironments.Add(eMEEnvironment);

            eMEEnvironment = new EMEEnvironment();
            eMEEnvironment.Freq = 425.25;
            eMEEnvironment.SignalSource = "已建台站";
            eMEEnvironment.Department = "某某某单位";
            eMEEnvironment.Address = "某某某单位地址";
            eMEEnvironment.RelationMan = "李某某";
            eMEEnvironment.Phone = "13080000002";
            eMEEnvironment.IsLegal = "是";
            eMEEnvironment.IsClear = "否";
            eMEEnvironment.ResultIsClear = "未完成";
            eEmEnvironments.Add(eMEEnvironment);

            eMEEnvironment = new EMEEnvironment();
            eMEEnvironment.Freq = 430.25;
            eMEEnvironment.SignalSource = "已建台站";
            eMEEnvironment.Department = "某某某单位";
            eMEEnvironment.Address = "某某某单位地址";
            eMEEnvironment.RelationMan = "张某某";
            eMEEnvironment.Phone = "13080000003";
            eMEEnvironment.IsLegal = "是";
            eMEEnvironment.IsClear = "是";
            eMEEnvironment.ResultIsClear = "未完成";
            eEmEnvironments.Add(eMEEnvironment);

            eMEEnvironment = new EMEEnvironment();
            eMEEnvironment.Freq = 434.25;
            eMEEnvironment.SignalSource = "未知";
            eMEEnvironment.Department = "";
            eMEEnvironment.Address = "";
            eMEEnvironment.RelationMan = "";
            eMEEnvironment.Phone = "";
            eMEEnvironment.IsLegal = "否";
            eMEEnvironment.IsClear = "否";
            eMEEnvironment.ResultIsClear = "完成";

            eEmEnvironments.Add(eMEEnvironment);
            return eEmEnvironments;
        }

        private void cbxList_DropDownClosed(object sender, EventArgs e)
        {

            ComboBox cbx = sender as ComboBox;
            if (cbx != null)
            {
                if (cbx.SelectedItem != null)
                {
                    EmeClearState cs = (EmeClearState)cbx.SelectedItem;   //check(cbx.SelectedItem.ToString());
                    string key = cbx.Tag as string;
                    if (cs != null)
                    {
                        if (SaveStateDic.ContainsKey(key) == true)
                        {
                            SaveStateDic[key] = cs.Value;
                        }
                        else
                        {
                            SaveStateDic.Add(key, cs.Value);
                        }
                    }

                }

            }
        }
    }


}
