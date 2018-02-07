using CO_IA.Data;
using CO_IA.UI.StationManage;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CO_IA.UI.Setting.Equipment
{
    /// <summary>
    /// EquImportFromStationDB.xaml 的交互逻辑
    /// </summary>
    public partial class EquImportFromStationDB : Window
    {
        QueryStationDBDialog queryDialog;
        private DataSet CurrentResult = new DataSet();

        public EquImportFromStationDB()
        {
            InitializeComponent();
            this.Loaded += EquImportFromStationDB_Loaded;
            queryDialog = new QueryStationDBDialog();
            queryDialog.OnExtractEvent += queryDialog_OnExtractEvent;
        }


        private void EquImportFromStationDB_Loaded(object sender, RoutedEventArgs e)
        {
            queryDialog.ShowDialog();
        }

        private void ButQuery_Click(object sender, RoutedEventArgs e)
        {
            queryDialog.ShowDialog();
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
            List<ActivityEquipmentInfo> equiplist = new List<ActivityEquipmentInfo>();

            if (stationListControl.StationItemsSource != null)
            {
                List<StationInfo> stations = stationListControl.StationItemsSource.Where(r => r.IsChecked == true).ToList();

                foreach (StationInfo item in stations)
                {
                    //ConvertAndSaveEquipmentInfos(item);
                }
            }


            MessageBox.Show("导入成功!", "提示", MessageBoxButton.OKCancel);
            this.Close();
        }

        /// <summary>
        /// 解析并且保存设备信息
        /// </summary>
        /// <param name="item"></param>
        private void ConvertAndSaveEquipmentInfos(StationInfo item)
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
                    CurrentResult.ReadXml(xmlSR, XmlReadMode.InferTypedSchema); //忽视任何内联架构，从数据推断出强类型架构并加载数据。如果无法推断，则解释成字符串数据
                    if (CurrentResult.Tables.Count > 0)
                    {
                       // ShowDetail(CurrentResult);
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
     
        private ORGInfo GetORGInfos(DataTable orgTable)
        {
            #region  单位信息

            ORGInfo orginfo = new ORGInfo();
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
            return orginfo;

            #endregion
        }

        private List<EquInfo> GetStationEquInfo(DataSet ds)
        {
            List<CO_IA.UI.StationManage.EquInfo> equList = new List<CO_IA.UI.StationManage.EquInfo>();
            
            string STAT_NAME = string.Empty;
            string NET_TS = string.Empty;

            #region  详细信息
            List<EquipmentInfo> equipList = new List<EquipmentInfo>();
            
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

                DataTable equTable = ds.Tables["RSBT_EQU"];
                DataTable eDT = ds.Tables["RSBT_EQU_T"];

                #region  设备表

                foreach (DataRow dr in equTable.Rows)
                {
                    try
                    {
                        CO_IA.UI.StationManage.EquInfo equInfo = new CO_IA.UI.StationManage.EquInfo();
                        if (equTable.Columns.Contains("EQU_MODEL"))
                        {
                            equInfo.EQU_MODEL = dr["EQU_MODEL"].ToString();
                        }
                        else
                        {
                            equInfo.EQU_MODEL = "";
                        }
                        if (equTable.Columns.Contains("EQU_AUTH"))
                        {
                            equInfo.EQU_AUTH = dr["EQU_AUTH"].ToString();
                        }
                        else
                        {
                            equInfo.EQU_AUTH = "";
                        }
                        if (equTable.Columns.Contains("EQU_CODE"))
                        {
                            equInfo.EQU_CODE = dr["EQU_CODE"].ToString();
                        }
                        else
                        {
                            equInfo.EQU_CODE = "";
                        }
                        if (equTable.Columns.Contains("EQU_MENU"))
                        {
                            equInfo.EQU_MENU = dr["EQU_MENU"].ToString();
                        }
                        else
                        {
                            equInfo.EQU_MENU = "";
                        }
                        if (equTable.Columns.Contains("EQU_POW"))
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
                                        if (equTable.Columns.Contains("ET_EQU_SENU"))
                                        {
                                            equInfo.ET_EQU_SENU = dr["ET_EQU_SENU"].ToString();
                                        }
                                        else
                                        {
                                            equInfo.ET_EQU_SENU = "";
                                        }
                                        if (equTable.Columns.Contains("ET_EQU_SEN"))
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

            return equList;

            #endregion
        }
    }
}
