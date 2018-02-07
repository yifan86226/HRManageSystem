using CO_IA.Client;
using CO_IA.Data;
using CO_IA.InterferenceAnalysis;
using CO_IA.UI.FreqPlan;
using CO_IA.UI.FreqPlan.StationPlan;
using CO_IA.UI.MAP;
using CO_IA_Data;
using EMCS.Types;
using I_CO_IA.FreqPlan;
using I_GS_MapBase.Portal.Types;
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

namespace CO_IA.UI.StationPlan
{
    /// <summary>
    /// FreqAssignHandle_Control.xaml 的交互逻辑
    /// </summary>
    public partial class FreqAssignHandleControl : UserControl
    {
        EquipmentQueryCondition querycondition = new EquipmentQueryCondition()
        {
            ActivityGuid = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid
        };

        /// <summary>
        /// 周围台站
        /// </summary>
        List<RoundStationInfo> RoundStations
        {
            get;
            set;
        }

        public string PlaceGuid
        {
            get { return (string)GetValue(MyPropertyProperty); }
            set { SetValue(MyPropertyProperty, value); }
        }
        public static readonly DependencyProperty MyPropertyProperty =
            DependencyProperty.Register("Placeguid", typeof(string), typeof(FreqAssignHandleControl), new PropertyMetadata(new PropertyChangedCallback(PlaceChangedCallBack)));


        public FreqAssignHandleControl()
        {
            InitializeComponent();
        }

        private static void PlaceChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FreqAssignHandleControl freqassign = d as FreqAssignHandleControl;
            freqassign.querycondition.PlaceGuid = e.NewValue == null ? null : e.NewValue.ToString();
            freqassign.GetEquipmentInfos();
            freqassign.QueryRoundStations(e.NewValue.ToString());
        }

        #region 事件

        /// <summary>
        /// 查询事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void xEquQuery_Click(object sender, RoutedEventArgs e)
        {
            QueryEquipListDialog dialog = new QueryEquipListDialog();
            dialog.ShowDialog(this);
            if (dialog.IsSuccuessFull == true)
            {
                dialog.Condition.ActivityGuid = Client.RiasPortal.ModuleContainer.Activity.Guid;
                dialog.Condition.PlaceGuid = this.PlaceGuid;
                querycondition = dialog.Condition;
                GetEquipmentInfos();
            }
        }

        /// <summary>
        /// 周围台站查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void xAroundStationQuery_Click(object sender, RoutedEventArgs e)
        {
            AroundStationQueryDialog dialog = new AroundStationQueryDialog(RoundStations);
            dialog.Show();
        }

        /// <summary>
        /// 干扰分析
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void xinterferenceAnalysis_Click(object sender, RoutedEventArgs e)
        {
            InterfereAnalyseRealize(_freqAssignListControl.CheckedEquipment);
        }

        public void InterfereAnalyseRealize(List<ActivityEquipmentInfo> quondamEqus)
        {
            //过滤符合业务类型的台站进行比较
            List<ActivityEquipmentInfo> equs = GetCalcEquipmentInfo(quondamEqus);
            if (equs!=null)
            {
                if (equs.Count == 0)
                {
                    MessageBox.Show("公众移动通信系统的设备不进行干扰计算,请重新选择设备!");
                }
                else
                {
                    if (ValidEquipments(equs))
                    {
                        InterfereAnalyseDialog interfdialog = new InterfereAnalyseDialog(equs, this.RoundStations);
                        interfdialog.ShowDialog(this);
                    }
                }
            }
        }
        /// <summary>
        /// 指配建议
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void xAssignAdvise_Click(object sender, RoutedEventArgs e)
        {
            List<ActivityEquipmentInfo> equs = _freqAssignListControl.CheckedEquipment;
            if (equs.Count == 0)
            {
                MessageBox.Show("请先选择设备", "提示", MessageBoxButton.OK);
                return;
            }
            else if (equs.Count > 1)
            {
                MessageBox.Show("请选择一个设备", "提示", MessageBoxButton.OK);
                return;
            }

            FreqAssignDialog assigndialog = new FreqAssignDialog(_freqAssignListControl.CheckedEquipment[0], this.RoundStations, this.PlaceGuid);

            if (assigndialog.isInitOk)
            {
                assigndialog.ConfirmEvent += (obj) =>
                {
                    _freqAssignListControl.CheckedEquipment[0].AssignFreq = obj;
                };
                assigndialog.ShowDialog(this);
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void xSaveAssingnFreq_Click(object sender, RoutedEventArgs e)
        {
            List<ActivityEquipmentInfo> equs = _freqAssignListControl.xFreqAssignGrid.ItemsSource as List<ActivityEquipmentInfo>;
            string massage = Validated(equs);
            if (string.IsNullOrEmpty(massage))
            {
                try
                {
                    SaveAssingnFreq(equs);
                    MessageBox.Show("保存指配频率成功！", "提示", MessageBoxButton.OK);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("保存指配频率失败！" + ex.Message, "提示", MessageBoxButton.OK);
                }
            }
            else
            {
                MessageBox.Show(massage, "提示", MessageBoxButton.OK);
            }
        }

        #endregion

        #region 方法


        /// <summary>
        /// 查询设备
        /// </summary>
        /// <param name="condition"></param>
        private void GetEquipmentInfos()
        {
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan>(channel =>
            {
                List<ActivityEquipmentInfo> sources = channel.GetEquipmentInfos(querycondition);
                this._freqAssignListControl.EquipmentItemsSource = sources;
            });
        }

        /// <summary>
        /// 保存指配频率前的验证
        /// </summary>
        /// <returns></returns>
        private string Validated(List<ActivityEquipmentInfo> equs)
        {
            string message = "";
            string equName = "";
            foreach (ActivityEquipmentInfo equ in equs)
            {
                if (equ.AssignFreq == null)
                {
                    equName += equ.Name + "、";
                }
            }
            if (!string.IsNullOrEmpty(equName))
            {
                equName = equName.Substring(0, equName.Length - 1);
                message = "设备“" + equName + "”指配频率有误，请修改后再保存！";
            }
            return message;
        }

        /// <summary>
        /// 保存设备指配频率
        /// </summary>
        /// <param name="equList"></param>
        /// <returns></returns>
        private bool SaveAssingnFreq(List<ActivityEquipmentInfo> equList)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan, bool>(channel =>
            {
                return channel.SaveEquipmentList(equList);
            });
        }

        /// <summary>
        /// 计算干扰前的验证
        /// </summary>
        /// <returns></returns>
        private bool ValidEquipments(List<ActivityEquipmentInfo> equs)
        {
            StringBuilder msg = new StringBuilder();
            if (equs == null || equs.Count == 0)
            {
                msg.AppendFormat("干扰分析设备不能为空 \r");
            }
            else
            {
                foreach (ActivityEquipmentInfo equ in equs)
                {
                    if (equ.AssignFreq == null || equ.AssignFreq == 0)
                    {
                        msg.AppendFormat("设备{0}的频率不能为空或者0 \r", equ.Name);
                    }
                }

            }
            if (string.IsNullOrEmpty(msg.ToString()))
            {
                return true;
            }
            else
            {
                MessageBox.Show(msg.ToString(), "不能进行干扰分析", MessageBoxButton.OK);
                return false;
            }
        }

        private List<ActivityEquipmentInfo> GetCalcEquipmentInfo(List<ActivityEquipmentInfo> equs)
        {
            List<ActivityEquipmentInfo> matchequs = new List<ActivityEquipmentInfo>();
            if (equs == null || equs.Count == 0)
            {
                MessageBox.Show("请选择要进行计算的设备");
                return null;
            }
            else
            {
                foreach (ActivityEquipmentInfo equ in equs)
                {
                    //LY01开始的业务类型,不进行干扰计算
                    if (!string.IsNullOrEmpty(equ.BusinessCode) && equ.BusinessCode.Substring(0, 4) != "LY01")
                    {
                        matchequs.Add(equ);
                    }
                }
            }
            return matchequs;
        }

        #endregion

        private void QueryRoundStations(string Placeguid)
        {
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqPlan>(channel =>
           {
               RoundStations = channel.QueryRoundStationsByPlace(Placeguid);
           });
        }
    }
}
