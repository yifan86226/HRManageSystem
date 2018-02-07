using CO_IA.Data;
using CO_IA.UI.Setting;
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
using AT_BC.Data.Helpers;
namespace CO_IA.UI.FreqPlan.StationPlan
{
    /// <summary>
    /// BatchExtractFreqsDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ExtractFromStationDBDialog : Window
    {

        public event Action RefreshEquipmentSource;
        QueryStationDBDialog queryDialog;
        public ExtractFromStationDBDialog()
        {
            InitializeComponent();
            this.Loaded += ExtractStationDBDialog_Loaded;
        }

        public ExtractFromStationDBDialog(ActivityPlace activityPlaceInfo)
        {
            InitializeComponent();
            this.Loaded += ExtractStationDBDialog_Loaded;
            this.activityPlaceInfo = activityPlaceInfo;
        }

        private void ExtractStationDBDialog_Loaded(object sender, RoutedEventArgs e)
        {
            queryDialog = new QueryStationDBDialog();
            queryDialog.OnExtractEvent += queryDialog_OnExtractEvent;
            queryDialog.ShowDialog(this);
        }

        private void ButQuery_Click(object sender, RoutedEventArgs e)
        {
            queryDialog = new QueryStationDBDialog();
            queryDialog.OnExtractEvent += queryDialog_OnExtractEvent;
            queryDialog.ShowDialog(this);
        }

        private void queryDialog_OnExtractEvent(List<QUERY_PARAMS> lsParams)
        {
            List<StationInfo> statinfoLs = new List<StationInfo>();
            if (lsParams.Count > 0)
            {
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_StationManage>(channel =>
                {
                    statinfoLs = channel.GetStationByParams(lsParams);
                    this.stationListControl.StationItemsSource = statinfoLs;
                });
            }
        }

        private void BtnExtract_Click(object sender, RoutedEventArgs e)
        {
            if (stationListControl.StationItemsSource == null)
            {
                return;
            }
            List<ActivityEquipmentInfo> equiplist = new List<ActivityEquipmentInfo>();
            foreach (StationInfo item in stationListControl.StationItemsSource)
            {
                if (item.IsChecked == true)
                {
                    // 解析并且保存设备信息
                    ConvertAndSaveActivityEquipmentInfos(item);
                }
            }
            MessageBox.Show("导入成功!", "提示", MessageBoxButton.OKCancel);
            this.Close();

            if (RefreshEquipmentSource != null)
            {
                RefreshEquipmentSource();
            }
        }
        private DataSet CurrentResult = new DataSet();
        private ActivityPlace activityPlaceInfo;
        /// <summary>
        /// 解析并且保存设备信息
        /// </summary>
        /// <param name="item"></param>
        private void ConvertAndSaveActivityEquipmentInfos(StationInfo item)
        {
            GetDetailInfo(item.STATGUID);
        }

        /// <summary>
        /// 调用台站详细信息服务
        /// </summary>
        /// <param name="m_statGuid"></param>
        private void GetDetailInfo(string m_statGuid)
        {
            #region
            //调用台站详细信息服务
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_StationManage>(channel =>
            {
                string strStationStr = channel.SearchStationByGuid(m_statGuid);
                if (!string.IsNullOrEmpty(strStationStr))
                {
                    StringReader xmlSR = new StringReader(strStationStr);
                    //CurrentResult.ReadXml(xmlSR, XmlReadMode.InferTypedSchema); //忽视任何内联架构，从数据推断出强类型架构并加载数据。如果无法推断，则解释成字符串数据
                    CurrentResult.ReadXml(xmlSR, XmlReadMode.Auto); //忽视任何内联架构，从数据推断出强类型架构并加载数据。如果无法推断，则解释成字符串数据
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
                    //ApplyControl = new ApplyInfoControl(ds);
                    //this.lg_Apply.Children.Add(ApplyControl);

                    ActivityORGInfo orginfo = new ActivityORGInfo();
                    #region  单位信息
                 
                    orginfo.Activity_Guid = Client.RiasPortal.ModuleContainer.Activity.Guid;
                    if (ds.Tables["RSBT_ORG"] != null && ds.Tables["RSBT_ORG"].Rows.Count > 0)
                    {

                     DataTable   orgTable = ds.Tables["RSBT_ORG"];
                        if (orgTable.Columns.Contains("ORG_NAME"))
                        {
                            orginfo.Name = orgTable.Rows[0]["ORG_NAME"].ToString();//单位名称
                        }
                        else
                        {
                            orginfo.Name = "";
                        }
                        //if (orgTable.Columns.Contains("ORG_CODE"))
                        //{
                        //    this.tb_UnitCode.Text = orgTable.Rows[0]["ORG_CODE"].ToString();//组织机构代码
                        //}
                        //else
                        //{
                        //    this.tb_UnitCode.Text = "";
                        //}
                        if (orgTable.Columns.Contains("ORG_ADDR"))
                        {
                            orginfo.Address = orgTable.Rows[0]["ORG_ADDR"].ToString();//单位地址
                        }
                        if (orgTable.Columns.Contains("ORG_LINK_PERSON"))
                        {
                            orginfo.Contact = orgTable.Rows[0]["ORG_LINK_PERSON"].ToString();//单位联系人
                        }
                      
                        if (orgTable.Columns.Contains("ORG_PHONE"))
                        {
                            orginfo.Phone = orgTable.Rows[0]["ORG_PHONE"].ToString();//联系电话
                        }

                    }


                    #endregion

                    List<CO_IA.UI.StationManage.EquInfo> equList = new List<CO_IA.UI.StationManage.EquInfo>();

                    List<CO_IA.UI.StationManage.FreqInfo> freqList = new List<CO_IA.UI.StationManage.FreqInfo>();
                    string STAT_NAME = string.Empty;
                    string NET_TS = string.Empty;
           

                    #region  详细信息

                    List<ActivityEquipmentInfo> equipList = new List<ActivityEquipmentInfo>();


                    //网表
                    if (ds.Tables["RSBT_NET"] != null && ds.Tables["RSBT_NET"].Rows.Count > 0)
                    {
                        if (ds.Tables["RSBT_NET"].Columns.Contains("NET_TS"))
                        {
                            NET_TS = ds.Tables["RSBT_NET"].Rows[0]["NET_TS"].ToString();//技术体制
                        }
                    }



                    //台站资料表
                    if (ds.Tables["RSBT_STATION"] != null && ds.Tables["RSBT_STATION"].Rows.Count > 0)
                    {
                        if (ds.Tables["RSBT_STATION"].Columns.Contains("STAT_NAME"))
                        {
                            STAT_NAME = ds.Tables["RSBT_STATION"].Rows[0]["STAT_NAME"].ToString();//台站名称
                        }
                    }

                    //设备表
                    if (ds.Tables["RSBT_EQU"] != null && ds.Tables["RSBT_EQU"].Rows.Count > 0)
                    {
                        //EquControl = new EquInfoControl(ds.Tables["RSBT_EQU"], ds.Tables["RSBT_EQU_T"]);
                        //this.lg_Equinfo.Children.Add(EquControl);

                        DataTable dataTable = ds.Tables["RSBT_EQU"];
                        DataTable eDT = ds.Tables["RSBT_EQU_T"];
                      
                        #region  设备表

                        foreach (DataRow dr in dataTable.Rows)
                        {
                            try
                            {
                                CO_IA.UI.StationManage.EquInfo equInfo = new CO_IA.UI.StationManage.EquInfo();
                                if (dataTable.Columns.Contains("EQU_MODEL"))
                                {
                                    equInfo.EQU_MODEL = dr["EQU_MODEL"].ToString();
                                }
                                else
                                {
                                    equInfo.EQU_MODEL = "";
                                }
                                if (dataTable.Columns.Contains("EQU_AUTH"))
                                {
                                    equInfo.EQU_AUTH = dr["EQU_AUTH"].ToString();
                                }
                                else
                                {
                                    equInfo.EQU_AUTH = "";
                                }
                                if (dataTable.Columns.Contains("EQU_CODE"))
                                {
                                    equInfo.EQU_CODE = dr["EQU_CODE"].ToString();
                                }
                                else
                                {
                                    equInfo.EQU_CODE = "";
                                }
                                if (dataTable.Columns.Contains("EQU_MENU"))
                                {
                                    equInfo.EQU_MENU = dr["EQU_MENU"].ToString();
                                }
                                else
                                {
                                    equInfo.EQU_MENU = "";
                                }
                                if (dataTable.Columns.Contains("EQU_POW"))
                                {
                                    equInfo.EQU_POW = dr["EQU_POW"].ToString();
                                }
                                else
                                {
                                    equInfo.EQU_POW = "";
                                }

                                try
                                {
                                    if (string.IsNullOrEmpty(dr["EQU_POW"].ToString()) == true && dr["GUID"].ToString() != null)
                                    {



                                        foreach (DataRow edr in eDT.Rows)
                                        {
                                            if (edr["GUID"] == dr["GUID"])
                                            {
                                                if (dataTable.Columns.Contains("ET_EQU_SENU"))
                                                {
                                                    equInfo.ET_EQU_SENU = dr["ET_EQU_SENU"].ToString();
                                                }
                                                else
                                                {
                                                    equInfo.ET_EQU_SENU = "";
                                                }
                                                if (dataTable.Columns.Contains("ET_EQU_SEN"))
                                                {
                                                    equInfo.ET_EQU_SEN = dr["ET_EQU_SEN"].ToString();
                                                }
                                                else
                                                {
                                                    equInfo.ET_EQU_SEN = "";
                                                }

                                                break;
                                            }

                                        }
                                    }
                                }
                                catch
                                {

                                }
                                equList.Add(equInfo);
                            }
                            catch
                            {

                            }
                        }

                        #endregion
                    }

                    //频率表
                    if (ds.Tables["RSBT_FREQ"] != null && ds.Tables["RSBT_FREQ"].Rows.Count > 0)
                    {

                        //FreqControl = new FreqInfoControl(ds.Tables["RSBT_FREQ"]);
                        //this.lg_Freq.Children.Add(FreqControl);

                        DataTable dataTable = ds.Tables["RSBT_FREQ"];
                        #region 频率表
                        foreach (DataRow dr in dataTable.Rows)
                        {
                            try
                            {
                                CO_IA.UI.StationManage.FreqInfo freqInfo = new CO_IA.UI.StationManage.FreqInfo();


                                //freqInfo.FREQ_EFB = dr["FREQ_EFB"];
                                //freqInfo.FREQ_EFE = dr["FREQ_EFE"];
                                //freqInfo.FREQ_RFB = dr["FREQ_RFB"];
                                //freqInfo.FREQ_RFE = dr["FREQ_RFE"];
                                //freqInfo.FREQ_UC = dr["FREQ_UC"];

                                if (dataTable.Columns.Contains("GUID"))
                                {
                                    freqInfo.GUID = dr["GUID"].ToString();
                                }
                                else
                                {
                                    freqInfo.GUID = Guid.NewGuid().ToString();
                                
                                }

                                if (dataTable.Columns.Contains("FREQ_TYPE"))
                                {
                                    freqInfo.FREQ_TYPE = dr["FREQ_TYPE"].ToString();
                                }
                                else
                                {
                                    freqInfo.FREQ_TYPE = "";
                                }
                                double dbUC = 0;
                                if (dataTable.Columns.Contains("FREQ_UC"))
                                {
                                    if (dr["FREQ_UC"].ToString() != null && dr["FREQ_UC"].ToString() != string.Empty && double.TryParse(dr["FREQ_UC"].ToString(), out  dbUC) == true)
                                    {
                                        freqInfo.FREQ_UC = dbUC.ToString();

                                        //if (dbUC >= 3)
                                        //{
                                        //    freqInfo.FREQ_UC = dbUC + " M";
                                        //}
                                        //else
                                        //{
                                        //    freqInfo.FREQ_UC = dbUC * 1000 + " kHz";

                                        //}

                                    }
                                    else
                                    {
                                        freqInfo.FREQ_UC = dr["FREQ_UC"].ToString();
                                    }
                                }
                                else
                                {
                                    freqInfo.FREQ_UC = "";
                                }


                                double dbLC = 0;
                                if (dataTable.Columns.Contains("FREQ_LC"))
                                {
                                    if (dr["FREQ_LC"].ToString() != null && dr["FREQ_LC"].ToString() != string.Empty && double.TryParse(dr["FREQ_LC"].ToString(), out  dbLC) == true)
                                    {
                                        freqInfo.FREQ_LC = dbLC.ToString();

                                        //if (dbUC >= 3)
                                        //{
                                        //    freqInfo.FREQ_UC = dbUC + " M";
                                        //}
                                        //else
                                        //{
                                        //    freqInfo.FREQ_UC = dbUC * 1000 + " kHz";

                                        //}

                                    }
                                    else
                                    {
                                        freqInfo.FREQ_LC = dr["FREQ_LC"].ToString();
                                    }
                                }
                                else
                                {
                                    freqInfo.FREQ_LC = "";
                                }



                                double dbEFB = 0;
                                if (dataTable.Columns.Contains("FREQ_EFB"))
                                {
                                    if (dr["FREQ_EFB"].ToString() != null && dr["FREQ_EFB"].ToString() != string.Empty && double.TryParse(dr["FREQ_EFB"].ToString(), out  dbEFB) == true)
                                    {
                                        freqInfo.FREQ_EFB = dbEFB.ToString();
                                        //if (dbEFB >= 3)
                                        //{
                                        //    freqInfo.FREQ_EFB = dbEFB + " M";
                                        //}
                                        //else
                                        //{
                                        //    freqInfo.FREQ_EFB = dbEFB * 1000 + " kHz";

                                        //}
                                    }
                                    else
                                    {
                                        freqInfo.FREQ_EFB = dr["FREQ_EFB"].ToString();
                                    }
                                }
                                else
                                {
                                    freqInfo.FREQ_EFB = "";
                                }



                                double dbEFE = 0;
                                if (dataTable.Columns.Contains("FREQ_EFE"))
                                {
                                    if (dr["FREQ_EFE"].ToString() != null && dr["FREQ_EFE"].ToString() != string.Empty && double.TryParse(dr["FREQ_EFE"].ToString(), out  dbEFE) == true)
                                    {
                                        freqInfo.FREQ_EFE = dbEFE.ToString();
                                        //if (dbEFE >= 3)
                                        //{
                                        //    freqInfo.FREQ_EFE = dbEFE + " M";
                                        //}
                                        //else
                                        //{
                                        //    freqInfo.FREQ_EFE = dbEFE * 1000 + " kHz";

                                        //}
                                    }
                                    else
                                    {
                                        freqInfo.FREQ_EFE = dr["FREQ_EFE"].ToString();
                                    }
                                }
                                else
                                {
                                    freqInfo.FREQ_EFE = "";
                                }

                                double dbRFB = 0;
                                if (dataTable.Columns.Contains("FREQ_RFB"))
                                {
                                    if (dr["FREQ_RFB"].ToString() != null && dr["FREQ_RFB"].ToString() != string.Empty && double.TryParse(dr["FREQ_RFB"].ToString(), out  dbRFB) == true)
                                    {
                                        freqInfo.FREQ_RFB = dbRFB.ToString();
                                        //if (dbRFB >= 3)
                                        //{
                                        //    freqInfo.FREQ_RFB = dbRFB + " M";
                                        //}
                                        //else
                                        //{
                                        //    freqInfo.FREQ_RFB = dbRFB * 1000 + " kHz";

                                        //}

                                    }
                                    else
                                    {
                                        freqInfo.FREQ_RFB = dr["FREQ_RFB"].ToString();
                                    }
                                }
                                else
                                {
                                    freqInfo.FREQ_RFB = "";
                                }

                                double dbRFE = 0;
                                if (dataTable.Columns.Contains("FREQ_RFE"))
                                {
                                    if (dr["FREQ_RFE"].ToString() != null && dr["FREQ_RFE"].ToString() != string.Empty && double.TryParse(dr["FREQ_RFE"].ToString(), out  dbRFE) == true)
                                    {

                                        freqInfo.FREQ_RFE = dbRFE.ToString();
                                        //if (dbRFE >= 3)
                                        //{
                                        //    freqInfo.FREQ_RFE = dbRFE + " M";
                                        //}
                                        //else
                                        //{
                                        //    freqInfo.FREQ_RFE = dbRFE * 1000 + " kHz";

                                        //}
                                    }
                                    else
                                    {
                                        freqInfo.FREQ_RFE = dr["FREQ_RFE"].ToString();
                                    }
                                }
                                else
                                {
                                    freqInfo.FREQ_RFE = "";
                                }
                                //freqInfo.FREQ_E_BAND = dr["FREQ_E_BAND"];
                                //freqInfo.FREQ_R_BAND = dr["FREQ_R_BAND"];

                                double db_E_BAND = 0;
                                if (dataTable.Columns.Contains("FREQ_E_BAND"))
                                {
                                    if (dr["FREQ_E_BAND"].ToString() != null && dr["FREQ_E_BAND"].ToString() != string.Empty && double.TryParse(dr["FREQ_E_BAND"].ToString(), out  db_E_BAND) == true)
                                    {

                                        freqInfo.FREQ_E_BAND = db_E_BAND.ToString();

                                        //if (db_E_BAND >= 3)
                                        //{
                                        //    freqInfo.FREQ_E_BAND = db_E_BAND + " M";
                                        //}
                                        //else
                                        //{
                                        //    freqInfo.FREQ_E_BAND = db_E_BAND * 1000 + " kHz";

                                        //}

                                    }
                                    else
                                    {
                                        freqInfo.FREQ_E_BAND = dr["FREQ_E_BAND"].ToString();
                                    }
                                }
                                else
                                {
                                    freqInfo.FREQ_E_BAND = "";
                                }


                                double db_R_BAND = 0;
                                if (dataTable.Columns.Contains("FREQ_R_BAND"))
                                {
                                    if (dr["FREQ_R_BAND"].ToString() != null && dr["FREQ_R_BAND"].ToString() != string.Empty && double.TryParse(dr["FREQ_R_BAND"].ToString(), out  db_R_BAND) == true)
                                    {

                                        freqInfo.FREQ_R_BAND = db_R_BAND.ToString();


                                        //if (db_R_BAND >= 3)
                                        //{
                                        //    freqInfo.FREQ_R_BAND = db_R_BAND + " M";
                                        //}
                                        //else
                                        //{
                                        //    freqInfo.FREQ_R_BAND = db_R_BAND * 1000 + " kHz";

                                        //}
                                    }
                                    else
                                    {
                                        freqInfo.FREQ_R_BAND = dr["FREQ_R_BAND"].ToString();
                                    }
                                }
                                else
                                {
                                    freqInfo.FREQ_R_BAND = "";
                                }
                                //freqInfo.FT_FREQ_TimeB = dr["FT_FREQ_TimeB"];
                                //freqInfo.FT_FREQ_TimeE = dr["FT_FREQ_TimeE"];

                                //freqInfo.FT_FREQ_INFO_Type = dr["FT_FREQ_INFO_Type"];
                                if (dataTable.Columns.Contains("FREQ_MOD"))
                                {
                                    freqInfo.FREQ_MOD = dr["FREQ_MOD"].ToString();
                                }
                                else
                                {
                                    freqInfo.FREQ_MOD = "";
                                }
                                //freqInfo.FT_FREQ_HCL = dr["FT_FREQ_HCL"];

                                #region  频率冗余表


                                if (dr["GUID"].ToString() != null && ds.Tables["RSBT_FREQ_T"] != null && ds.Tables["RSBT_FREQ_T"].Rows.Count > 0)
                                {

                                    DataTable eDT = ds.Tables["RSBT_FREQ_T"];

                                    foreach (DataRow edr in eDT.Rows)
                                    {
                                        if (edr["GUID"] == dr["GUID"])
                                        {
                                            if (dataTable.Columns.Contains("FT_FREQ_POW_MAX"))
                                            {
                                                freqInfo.FT_FREQ_POW_MAX = dr["FT_FREQ_POW_MAX"].ToString();
                                            }
                                            else
                                            {
                                                freqInfo.FT_FREQ_POW_MAX = "";
                                            }

                                            break;
                                        }

                                    }
                                #endregion

                                }
                                freqList.Add(freqInfo);
                                //break;
                            }
                            catch
                            {

                            }
                        }
                        #endregion
                    }
   
                    #endregion 细节

                    #region  保存组织信息
                    if (freqList.Count == 0)
                    { return; }


                    ActivityORGInfo  tempOrginfo = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan, ActivityORGInfo>(channel =>
                    {
                        return channel.GetORGInfoByActivityORGInfo(orginfo);
                    });


                    if (tempOrginfo != null && string.IsNullOrEmpty(tempOrginfo.Activity_Guid) == false)
                    {
                        orginfo = tempOrginfo;                    
                    }

                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan>(channel =>
                    {
                        channel.SaveORGInfo(orginfo);
                    });
                    #endregion


                    foreach (CO_IA.UI.StationManage.FreqInfo freqInfo in freqList)
                    {
                        ActivityEquipmentInfo equipinfo = new ActivityEquipmentInfo();
                        equipinfo.GUID = freqInfo.GUID;
                        equipinfo.OrgInfo = orginfo;
                        equipinfo.ORGGuid = orginfo.Guid;
                        //equipinfo.ORG.Guid = orginfo.Guid;
                        //equipinfo.ORG.Value = orginfo.Name;
                        equipinfo.ActivityGuid = orginfo.Activity_Guid;
                        equipinfo.PlaceGuid = activityPlaceInfo.Guid;
                        equipinfo.StationName = STAT_NAME;
                        equipinfo.EQUCount = equList.Count;
                        equipinfo.IsStation = true;
                        equipinfo.BusinessCode = NET_TS;
                        equipinfo.Origin = 1;
                      

                        if (equList.Count > 0)
                        {
                            CO_IA.UI.StationManage.EquInfo   tempInfo = equList[0];
                            equipinfo.IsMobile = false;
                            equipinfo.Name = tempInfo.EQU_MODEL;
                            equipinfo.EquModel = tempInfo.EQU_MODEL;
                            equipinfo.EquNo = tempInfo.EQU_CODE;
                            equipinfo.Sensitivity = tempInfo.ET_EQU_SEN.GetNullableDouble();
                            equipinfo.SensitivityUnit = tempInfo.ET_EQU_SEN.GetNullableInt();

                        }

                            //equipinfo.ADJChannelInh = 1;
                        
                            equipinfo.RecvFreqEnd = freqInfo.FREQ_RFB.GetNullableDouble();
                            equipinfo.RecvFreqStart = freqInfo.FREQ_RFE.GetNullableDouble();
                            equipinfo.SendFreqEnd = freqInfo.FREQ_EFB.GetNullableDouble();
                            equipinfo.SendFreqStart = freqInfo.FREQ_EFB.GetNullableDouble();
                            equipinfo.MaxPower =freqInfo.FT_FREQ_POW_MAX.GetNullableDouble();

                            equipinfo.ReceiveFreq = freqInfo.FREQ_UC.GetNullableDouble();//接收频率
                            equipinfo.SendFreq = freqInfo.FREQ_LC.GetNullableDouble(); //发射频率

                            //equipinfo.Band = 1;
                            //equipinfo.ChannelBand = 1;

                            //equipinfo.SignalNoise = 1;
                            //equipinfo.Remark = "1";
                            //equipinfo.RunningFrom = DateTime.Now;
                            //equipinfo.RunningTo = DateTime.Now;

                            equipinfo.IsTunAble = true;
                            //equipinfo.Leakage = 1;


                            try
                            {
                                EMCS.Types.EMCModulationEnum ModulateModeEnum = (EMCS.Types.EMCModulationEnum)freqInfo.FREQ_MOD.GetNullableInt();

                                if (Enum.IsDefined(typeof(EMCS.Types.EMCModulationEnum), ModulateModeEnum))
                                {
                                    equipinfo.ModulateMode = ModulateModeEnum;
                                }
                            }
                            catch
                            { }
                            //天线表
                            if (ds.Tables["RSBT_ANTFEED"] != null && ds.Tables["RSBT_ANTFEED"].Rows.Count > 0)
                            {
                                DataTable dataTable = ds.Tables["RSBT_ANTFEED"];
                                #region  天线表

                                try
                                {
                                    if (dataTable.Columns.Contains("ANT_MODEL"))
                                    {
                                        equipinfo.SendAntModel = equipinfo.RecvAntModel = dataTable.Rows[0]["ANT_MODEL"].ToString();
                                    }
                                  
                            
                                    if (dataTable.Columns.Contains("ANT_GAIN"))
                                    {
                                        equipinfo.SendAntGain =  equipinfo.RecvAntGain = dataTable.Rows[0]["ANT_GAIN"].ToString().GetNullableDouble();
                                    }

                                    if (dataTable.Columns.Contains("FEED_LENGTH"))
                                    {
                                        equipinfo.RecvAntFeedLength = equipinfo.SendAntFeedLength = dataTable.Rows[0]["FEED_LENGTH"].ToString().GetNullableDouble();//馈线长度
                                    }
                                    if (dataTable.Columns.Contains("FEED_LOSE"))
                                    {
                                        equipinfo.RecvAntFeedLoss = equipinfo.SendAntFeedLoss = dataTable.Rows[0]["FEED_LOSE"].ToString().GetNullableDouble();//馈线系统总损耗
                                    }


                                    if (dataTable.Columns.Contains("ANT_HIGHT"))
                                    {
                                        equipinfo.RecvAntHeight = equipinfo.SendAntHeight = dataTable.Rows[0]["ANT_HIGHT"].ToString().GetNullableDouble();//馈线系统总损耗
                                    }


                                 
                                  
                                }
                                catch
                                { }

                                #endregion
                            }
                            //天线表冗余表
                            if (ds.Tables["RSBT_ANTFEED_T"] != null && ds.Tables["RSBT_ANTFEED_T"].Rows.Count > 0)
                            {
                                DataTable dataTable = ds.Tables["RSBT_ANTFEED_T"];
                                #region  天线冗余表

                                try
                                {
                              
                                    if (dataTable.Columns.Contains("AT_ANT_UPANG"))
                                    {
                                        equipinfo.RecvAntElevation = equipinfo.SendAntElevation = dataTable.Rows[0]["AT_ANT_UPANG"].ToString().GetNullableDouble();
                                    }


                                    if (dataTable.Columns.Contains("ANT_POLE"))
                                    {
                                        //ANT_POLE = dataTable.Rows[0]["ANT_POLE"].ToString();//极化方式

                                        try
                                        {
                                            EMCS.Types.EMCPolarisationEnum EMCPolarisationEnum = (EMCS.Types.EMCPolarisationEnum)freqInfo.FREQ_MOD.GetIntOrDefault();

                                            if (Enum.IsDefined(typeof(EMCS.Types.EMCPolarisationEnum), EMCPolarisationEnum))
                                            {
                                                equipinfo.RecvAntPolar = equipinfo.SendAntPolar = EMCPolarisationEnum;
                                            }
                                        }
                                        catch
                                        { }

                                    }
                                   

                                    if (dataTable.Columns.Contains("ANT_ANGLE"))
                                    {
                                        equipinfo.RecvAntAzimuth = equipinfo.SendAntAzimuth = dataTable.Rows[0]["ANT_ANGLE"].ToString().GetNullableDouble();//最大辐射方位角
                                    }

                                    if (dataTable.Columns.Contains("FEED_LENGTH"))
                                    {
                                        equipinfo.RecvAntFeedLength = equipinfo.SendAntFeedLength =dataTable.Rows[0]["FEED_LENGTH"].ToString().GetNullableDouble();//馈线长度
                                    }
                                    if (dataTable.Columns.Contains("FEED_LOSE"))
                                    {
                                        equipinfo.RecvAntFeedLoss = equipinfo.SendAntFeedLoss = dataTable.Rows[0]["FEED_LOSE"].ToString().GetNullableDouble();//馈线系统总损耗
                                    }


                                    if (dataTable.Columns.Contains("ANT_HIGHT"))
                                    {
                                        equipinfo.RecvAntHeight = equipinfo.SendAntHeight = dataTable.Rows[0]["ANT_HIGHT"].ToString().GetNullableDouble();//馈线系统总损耗
                                    }


                                

                                }
                                catch
                                { }

                                #endregion
                            }
                       

                        equipList.Add(equipinfo);
                    }



                  PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan, bool>(channel =>
                    {
                        return channel.SaveEquipmentList(equipList);
                    });
      
                }
                #endregion
            }
        }
    }
}
