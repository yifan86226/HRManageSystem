using CO_IA_Data;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace CO_IA.UI.StationManage
{
    /// <summary>
    /// StationInfoControl.xaml 的交互逻辑
    /// </summary>
    public partial class StationInfoControl : UserControl
    {
        public StationInfoControl(DataTable statTable)
        {
            InitializeComponent();

            try
            {
                if (statTable.Columns.Contains("STAT_NAME"))
                {
                    this.tb_StatName.Text = statTable.Rows[0]["STAT_NAME"].ToString();//台站名称
                }
                else 
                {
                    this.tb_StatName.Text ="";//台站名称
                }
                if (statTable.Columns.Contains("STAT_TYPE"))
                {
                    this.tb_StatType.Text = statTable.Rows[0]["STAT_TYPE"].ToString(); //Canstant.GetInstance().GetDicNameByCode("00052006", statTable.Rows[0]["STAT_TYPE"]);//台（站）类别
                }
                else 
                {
                    this.tb_StatType.Text = "";
                }
                if (statTable.Columns.Contains("APP_CODE"))
                {
                    this.tb_AppCode.Text = statTable.Rows[0]["APP_CODE"].ToString();//申请表编号
                }
                else 
                {
                    this.tb_AppCode.Text ="";//申请表编号
                }
                //this.tb_StatName.Text = statTable.Rows[0]["ORG_CODE"];//资料表编号
                if (statTable.Columns.Contains("STAT_WORK"))
                {
                    this.tb_StatWork.Text = statTable.Rows[0]["STAT_WORK"].ToString();//Canstant.GetInstance().GetDicNameByCode("00062006", statTable.Rows[0]["STAT_WORK"]);//工作方式
                }
                else 
                {
                    this.tb_StatWork.Text = "";
                }
                try
                {
                    if (statTable.Columns.Contains("STAT_DATE_START"))
                    {
                        if (string.IsNullOrEmpty(statTable.Rows[0]["STAT_DATE_START"].ToString()) == false)
                        {
                            DateTime statDate = Convert.ToDateTime(statTable.Rows[0]["STAT_DATE_START"].ToString());

                            this.tb_StatDateStart.Text = statDate.Year + "年" + statDate.Month + "月" + statDate.Day + "日";//启用日期
                        }
                    }
                    else 
                    {
                        this.tb_StatDateStart.Text = "";
                    }

                }
                catch
                { }
                if (statTable.Columns.Contains("STAT_ADDR"))
                {
                    this.tb_StatAddr.Text = statTable.Rows[0]["STAT_ADDR"].ToString();//台站地址
                }
                else 
                {
                    this.tb_StatAddr.Text = "";
                }
                if (statTable.Columns.Contains("STAT_LG"))
                {
                    this.tb_StatLG.Text = statTable.Rows[0]["STAT_LG"].ToString() + " 度";//经度
                }
                else 
                {
                    this.tb_StatLG.Text = "";
                }
                if (statTable.Columns.Contains("STAT_LA"))
                {
                    this.tb_StatLA.Text = statTable.Rows[0]["STAT_LA"].ToString() + " 度";//纬度
                }
                else 
                {
                    this.tb_StatLA.Text = "";
                }
                if (statTable.Columns.Contains("STAT_AT"))
                {
                    this.tb_StatAT.Text = statTable.Rows[0]["STAT_AT"].ToString() + " 米";//海拔
                }
                else
                {
                    this.tb_StatAT.Text ="";
                }
            }
            catch
            { }
        }


        public StationInfoControl(List<ActivitySurroundStation> stations)
        {
            InitializeComponent();
            if (stations != null && stations.Count > 0)
            {

                this.tb_StatName.Text = stations[0].STAT_NAME==null?"":stations[0].STAT_NAME;//台站名称


                this.tb_StatType.Text = stations[0].STAT_APP_TYPE==null?"":stations[0].STAT_APP_TYPE; //Canstant.GetInstance().GetDicNameByCode("00052006", statTable.Rows[0]["STAT_TYPE"]);//台（站）类别


                this.tb_AppCode.Text = stations[0].APP_CODE==null?"":stations[0].APP_CODE;//申请表编号


                this.tb_StatWork.Text = "";//Canstant.GetInstance().GetDicNameByCode("00062006", statTable.Rows[0]["STAT_WORK"]);//工作方式



                this.tb_StatDateStart.Text = "";


                this.tb_StatAddr.Text = stations[0].STAT_ADDR==null?"":stations[0].STAT_ADDR;//台站地址


                this.tb_StatLG.Text = stations[0].STAT_LG==null?"":stations[0].STAT_LG.ToString() + " 度";//经度


                this.tb_StatLA.Text = stations[0].STAT_LA==null?"":stations[0].STAT_LA.ToString() + " 度";//纬度


                this.tb_StatAT.Text = stations[0].STAT_AT==null?"":stations[0].STAT_AT.ToString() + " 米";//海拔
            }    
            
        }
    }
}
