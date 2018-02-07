using CO_IA.Data;
using CO_IA.UI.PlanDatabase.Equipments;
using CO_IA_Data;
using I_CO_IA.FreqStation;
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

namespace CO_IA.UI.FreqStation.StationPlan
{
    /// <summary>
    /// FreqAssignManage.xaml 的交互逻辑
    /// </summary>
    public partial class FreqAssignManage : UserControl
    {
        private ActivityPlaceInfo CurrentActivityPlace
        {
            get;
            set;
        }

        EquipmentLoadStrategy eququerycondition = new EquipmentLoadStrategy();

        public FreqAssignManage(ActivityPlaceInfo pActivityPlace)
        {
            InitializeComponent();
            CurrentActivityPlace = pActivityPlace;
            GetActivityEquipments(eququerycondition);
        }

        /// <summary>
        /// 查询设备
        /// </summary>
        /// <param name="condition"></param>
        private void GetActivityEquipments(EquipmentLoadStrategy condition)
        {
            condition.ActivityGuid = this.CurrentActivityPlace.ActivityGuid;
            condition.PlaceGuid = this.CurrentActivityPlace.Guid;
            ActivityEquipment[] sources = FreqStationHelper.GetActivityEquipments(condition);
            freqAssignListControl.DataContext = sources;
        }

        /// <summary>
        /// 查询设备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void xEquQuery_Click(object sender, RoutedEventArgs e)
        {
            EquipmentQueryDialog querydialog = new EquipmentQueryDialog(eququerycondition);
            querydialog.ORGNameIsReadOnly = false;
            querydialog.OnQueryEvent += (condition) =>
            {
                eququerycondition = condition;
                this.GetActivityEquipments(condition);
            };
            querydialog.ShowDialog();
        }

        /// <summary>
        /// 周围台站
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void xAroundStationQuery_Click(object sender, RoutedEventArgs e)
        {
            SurroundStationDialog surstation = new SurroundStationDialog(CurrentActivityPlace);
            surstation.ShowDialog();

        }

        /// <summary>
        /// 干扰分析
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void xinterferenceAnalysis_Click(object sender, RoutedEventArgs e)
        {
            if (this.freqAssignListControl.ActivityEquipmentItemsSource != null)
            {
                List<ActivityEquipment> equs = this.freqAssignListControl.ActivityEquipmentItemsSource.Where(r => r.IsChecked == true).ToList();
                if (equs == null || equs.Count == 0)
                {
                    MessageBox.Show("请选择要进行干扰分析的设备");
                }
                else
                {
                    InterfereAnalyseRealize(equs);
                }
            }
        }

        /// <summary>
        /// 保存指配频率
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void xSaveAssingnFreq_Click(object sender, RoutedEventArgs e)
        {
            if (freqAssignListControl.ActivityEquipmentItemsSource != null && freqAssignListControl.ActivityEquipmentItemsSource.Length > 0)
            {
                List<ActivityEquipment> selectequs = freqAssignListControl.ActivityEquipmentItemsSource.Where(r => r.IsChecked == true).ToList();

                if (selectequs.Count == 0)
                {
                    MessageBox.Show("请勾选要指配频率的设备");
                }
                else
                {
                    if (VerifyAssignFreq(selectequs))
                    {
                        try
                        {
                            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqStation>(channel =>
                            {
                                channel.SaveAssignFreq(selectequs);
                                MessageBox.Show("保存成功");
                                freqAssignListControl.UnSelectAll();
                            });
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.GetExceptionMessage(), "提示");

                        }
                    }
                }
            }
        }

        /// <summary>
        /// 验证指配频率是否为空
        /// </summary>
        /// <param name="equs"></param>
        /// <returns></returns>
        private bool VerifyAssignFreq(List<ActivityEquipment> equs)
        {
            bool result = true;
            StringBuilder errormsg = new StringBuilder();

            foreach (ActivityEquipment equ in equs)
            {
                if (equ.AssignSendFreq == null)
                {
                    result = false;
                    errormsg.AppendFormat(string.Format("{0}指配频率为空 \n", equ.Name), "提示");
                }
            }
            if (!result)
            {
                MessageBox.Show(errormsg.ToString());
            }
            return result;
        }

        public void InterfereAnalyseRealize(List<ActivityEquipment> equs)
        {
            if (VerifyAssignFreq(equs))
            {
                List<ActivitySurroundStation> surroundstation = QuerySurroundStationFromDB();

                InterfereAnalyseDialog interfdialog = new InterfereAnalyseDialog(equs, surroundstation);
                interfdialog.ShowDialog();
            }
        }

        private List<ActivitySurroundStation> QuerySurroundStationFromDB()
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqStation, List<ActivitySurroundStation>>(channel =>
            {
                return channel.GetActivitySurroundStations(this.CurrentActivityPlace.ActivityGuid, this.CurrentActivityPlace.Guid, null);
            });
        }
    }
}
