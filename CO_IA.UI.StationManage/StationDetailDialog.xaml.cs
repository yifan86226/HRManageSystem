using CO_IA_Data;
using I_CO_IA.StationManage;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace CO_IA.UI.StationManage
{
    /// <summary>
    /// StationDetailDialog.xaml 的交互逻辑
    /// </summary>
    /// 
    public partial class StationDetailDialog : Window
    {

        #region   变量
        private ApplyInfoControl ApplyControl = null;
        private StationInfoControl StationControl = null;
        private EquInfoControl EquControl = null;
        private FreqInfoControl FreqControl = null;
        private AntfeedInfoControl AntfeedControl = null;
        private DataSet CurrentResult = new DataSet();
        private string m_statGuid="";
        #endregion
        public StationDetailDialog(string strStatGuid)
        {
            InitializeComponent();
            m_statGuid = strStatGuid;
            GetDetailInfo();
        }
        /// <summary>
        /// 环境分析调用，脱机情况下
        /// </summary>
        public StationDetailDialog( List<ActivitySurroundStation> listStation,List<StationEmitInfo> listEmitInfo)
        {
            InitializeComponent();
            lg_Apply.Visibility = System.Windows.Visibility.Collapsed;
            lg_Equinfo.Visibility = System.Windows.Visibility.Collapsed;
            lg_Antfeed.Visibility = System.Windows.Visibility.Collapsed;

            
            if (listStation != null && listStation.Count > 0)
            {
                StationControl = new StationInfoControl(listStation);
                this.lg_Station.Children.Add(StationControl);
            }
            if (listEmitInfo != null && listEmitInfo.Count > 0)
            {
                FreqControl = new FreqInfoControl(listEmitInfo);
                this.lg_Freq.Children.Add(FreqControl);
            }
        }

        private void GetDetailInfo()
        {
            #region
            //调用台站详细信息服务
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_StationManage>(channel =>
            {
                string strStation = channel.SearchStationByGuid(m_statGuid);
                if (!string.IsNullOrEmpty(strStation))
                {
                    System.IO.StringReader xmlSR = new System.IO.StringReader(strStation);
                    System.Data.DataSet CurrentResult = new System.Data.DataSet();
                    MemoryStream ms = new MemoryStream(Convert.FromBase64String(strStation));
                    CurrentResult.ReadXml(ms, XmlReadMode.Auto);  


                    //StringReader xmlSR = new StringReader(strStationStr);
                    //CurrentResult.ReadXml(xmlSR, XmlReadMode.Auto); //忽视任何内联架构，从数据推断出强类型架构并加载数据。如果无法推断，则解释成字符串数据
                    if (CurrentResult.Tables.Count > 0)
                    {
                        ShowDetail(CurrentResult);
                    }
                    else 
                    {
                        MessageBox.Show("获取不到台站概要信息，请确认数据源是否改变！");
                    }
                    
                }
                else 
                {
                    MessageBox.Show("获取不到台站概要信息，请确认数据源是否改变！");
                }
            });
            #endregion
        }
        private void ShowDetail(DataSet ds) 
        {
            if (ds == null)
            {
                MessageBox.Show("获取不到台站概要信息，请确认数据源是否改变！");
                return;
            }
            else
            {
                #region 弹出对话框
                if (ds.Tables.Count > 0)
                {
                    //申请表
                    ApplyControl = new ApplyInfoControl(ds);
                    this.lg_Apply.Children.Add(ApplyControl);

                    //台站资料表
                    if (ds.Tables["RSBT_STATION"] != null && ds.Tables["RSBT_STATION"].Rows.Count > 0)
                    {
                        StationControl = new StationInfoControl(ds.Tables["RSBT_STATION"]);
                        this.lg_Station.Children.Add(StationControl);
                    }

                    //设备表
                    if (ds.Tables["RSBT_EQU"] != null && ds.Tables["RSBT_EQU"].Rows.Count > 0)
                    {
                        EquControl = new EquInfoControl(ds.Tables["RSBT_EQU"], ds.Tables["RSBT_EQU_T"]);
                        this.lg_Equinfo.Children.Add(EquControl);
                    }

                    //频率表
                    if (ds.Tables["RSBT_FREQ"] != null && ds.Tables["RSBT_FREQ"].Rows.Count > 0)
                    {

                        FreqControl = new FreqInfoControl(ds.Tables["RSBT_FREQ"]);
                        this.lg_Freq.Children.Add(FreqControl);
                    }

                    //天线表
                    if (ds.Tables["RSBT_ANTFEED"] != null && ds.Tables["RSBT_ANTFEED"].Rows.Count > 0)
                    {
                        AntfeedControl = new AntfeedInfoControl(ds.Tables["RSBT_ANTFEED"]);
                        this.lg_Antfeed.Children.Add(AntfeedControl);
                    }
                }
                #endregion
            }
        }
    }
}
