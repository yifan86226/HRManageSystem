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
    /// AntfeedInfoControl.xaml 的交互逻辑
    /// </summary>
    public partial class AntfeedInfoControl : UserControl
    {
        public AntfeedInfoControl()
        {
            InitializeComponent();
        }
        public AntfeedInfoControl(DataTable dataTable)
        {
            InitializeComponent();

            try
            {
                if (dataTable.Columns.Contains("ANT_TYPE"))
                {
                    this.tb_AntType.Text = dataTable.Rows[0]["ANT_TYPE"].ToString();
                }
                else
                {
                    this.tb_AntType.Text = "";
                }
                //this.tb_AntType.Text = Canstant.GetInstance().GetDicNameByCode("00172006", dataTable.Rows[0]["ANT_TYPE"]);//天线类型
                if (dataTable.Columns.Contains("ANT_MODEL"))
                {
                    this.tb_AntModel.Text = dataTable.Rows[0]["ANT_MODEL"].ToString();//天线型号
                }
                else 
                {
                    this.tb_AntModel.Text = "";
                }
                if (dataTable.Columns.Contains("ANT_GAIN"))
                {
                    this.tb_AntGain.Text = dataTable.Rows[0]["ANT_GAIN"].ToString() + " dBi";//天线增益
                }
                else 
                {
                    this.tb_AntGain.Text = "";
                }
                //this.tb_AntGain.Text = dataTable.Rows[0]["ANT_GAIN"].ToString() + " dBi";//天线增益
                //this.tb_AntPole.Text = Canstant.GetInstance().GetDicNameByCode("00202006", dataTable.Rows[0]["ANT_POLE"]);//极化方式
                if (dataTable.Columns.Contains("ANT_POLE"))
                {
                    this.tb_AntPole.Text = dataTable.Rows[0]["ANT_POLE"].ToString();//极化方式
                }
                else 
                {
                    this.tb_AntPole.Text = "";
                }
                if (dataTable.Columns.Contains("ANT_ANGLE"))
                {
                    this.tb_AntAngle.Text = dataTable.Rows[0]["ANT_ANGLE"].ToString();//最大辐射方位角
                }
                else 
                {
                    this.tb_AntAngle.Text = "";
                }
                if (dataTable.Columns.Contains("ANT_MENU"))
                {
                    this.tb_AntMenu.Text = dataTable.Rows[0]["ANT_MENU"].ToString();//天线生产厂家
                }
                else 
                {
                    this.tb_AntMenu.Text = "";
                }
            }
            catch
            { }
        }
    }
}
