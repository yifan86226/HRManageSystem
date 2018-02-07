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
    /// ApplyInfoControl.xaml 的交互逻辑
    /// </summary>
    public partial class ApplyInfoControl : UserControl
    {

        private DataTable orgTable;
        private DataTable netTable;
        public ApplyInfoControl()
        {
            InitializeComponent();
        }
        public ApplyInfoControl(DataSet ds) 
        {
            InitializeComponent();


            if (ds.Tables["RSBT_ORG"] != null && ds.Tables["RSBT_ORG"].Rows.Count > 0)
            {

                orgTable = ds.Tables["RSBT_ORG"];
                if (orgTable.Columns.Contains("ORG_NAME"))
                {
                    this.tb_UnitName.Text = orgTable.Rows[0]["ORG_NAME"].ToString();//单位名称
                }
                else 
                {
                    this.tb_UnitName.Text = "";
                }
                if (orgTable.Columns.Contains("ORG_CODE"))
                {
                    this.tb_UnitCode.Text = orgTable.Rows[0]["ORG_CODE"].ToString();//组织机构代码
                }
                else 
                {
                    this.tb_UnitCode.Text = "";
                }
                if (orgTable.Columns.Contains("ORG_ADDR"))
                {
                    this.tb_UnitAddress.Text = orgTable.Rows[0]["ORG_ADDR"].ToString();//单位地址
                }
                if (orgTable.Columns.Contains("ORG_LINK_PERSON"))
                {
                    this.tb_UnitPerson.Text = orgTable.Rows[0]["ORG_LINK_PERSON"].ToString();//单位联系人
                }
                else 
                {
                    this.tb_UnitPerson.Text = "";
                }
                if (orgTable.Columns.Contains("ORG_PHONE"))
                {
                    this.tb_UnitPhone.Text = orgTable.Rows[0]["ORG_PHONE"].ToString();//联系电话
                }
                else 
                {
                    this.tb_UnitPhone.Text = "";
                }
            }


            if (ds.Tables["RSBT_NET"] != null && ds.Tables["RSBT_NET"].Rows.Count > 0)
            {

                netTable = ds.Tables["RSBT_NET"];

                if (netTable.Columns.Contains("NET_NAME"))
                {
                    this.tb_NetName.Text = netTable.Rows[0]["NET_NAME"].ToString();//无线电系统/网络名称
                }
                else
                {
                    this.tb_NetName.Text = "";
                }
                if (netTable.Columns.Contains("NET_BAND"))
                {
                    this.tb_NetBand.Text = netTable.Rows[0]["NET_BAND"].ToString() + " MHz";//信道带宽/波道间隔
                }
                else 
                {
                    this.tb_NetBand.Text = "";
                }
                if (netTable.Columns.Contains("NET_SVN"))
                {
                    this.tb_NetSvn.Text = netTable.Rows[0]["NET_SVN"].ToString();
                }
                else 
                {
                    this.tb_NetSvn.Text = "";
                }
                if (netTable.Columns.Contains("NET_SP"))
                {
                    this.tb_NetSP.Text = netTable.Rows[0]["NET_SP"].ToString();
                }
                else 
                {
                    this.tb_NetSP.Text = "";
                }
                if (netTable.Columns.Contains("NET_TS"))
                {
                    this.tb_NetTS.Text = netTable.Rows[0]["NET_TS"].ToString();
                }
                else 
                {
                    this.tb_NetTS.Text = "";
                }
                if (netTable.Columns.Contains("NET_AREA"))
                {
                    this.tb_NetArea.Text = netTable.Rows[0]["NET_AREA"].ToString();
                }
                else 
                {
                    this.tb_NetArea.Text = "";
                }
                //this.tb_NetSvn.Text = Canstant.GetInstance().GetDicNameByCode("00452006", netTable.Rows[0]["NET_SVN"]);//通信业务/系统类型
                //this.tb_NetSP.Text = Canstant.GetInstance().GetDicNameByCode("00002006", netTable.Rows[0]["NET_SP"]);//业务性质
                //this.tb_NetTS.Text = Canstant.GetInstance().GetDicNameByCode("00012006", netTable.Rows[0]["NET_TS"]);//技术体制
                //this.tb_NetArea.Text = Canstant.GetInstance().GetDicNameByCode("00022006", netTable.Rows[0]["NET_AREA"]);//使用范围
                if (netTable.Columns.Contains("NET_USE"))
                {
                    this.tb_NetUse.Text = netTable.Rows[0]["NET_USE"].ToString();//网络用途
                }
                else 
                {
                    this.tb_NetUse.Text = "";
                }
            }
        }
    }
}
