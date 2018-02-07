using CO_IA_Data;
using I_CO_IA.StationManage;
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
using System.Windows.Shapes;

namespace CO_IA.UI.FreqPlan.StationPlan
{
    /// <summary>
    /// ExtractFreqsDialog.xaml 的交互逻辑
    /// </summary>
    public partial class QueryStationDBDialog : Window
    {
        public event Action<List<QUERY_PARAMS>> OnExtractEvent;
        public QueryStationDBDialog()
        {
            InitializeComponent();
            List<BusinessDic> lisDic = new List<BusinessDic>();
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_StationManage>(channel =>
            {
                lisDic = channel.GetDicByCode("00452006");
                cbNetSvn.ItemsSource = lisDic;
            });
        }

        private void BunCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private List<QUERY_PARAMS> GetQueryParam()
        {
            List<QUERY_PARAMS> lsParams = new List<QUERY_PARAMS>();
            QUERY_PARAMS param = null;
            #region 单位名称
            if(!string.IsNullOrEmpty(txtOrgName.Text))
            {
                param = new QUERY_PARAMS();
                param.PARAMS_NAME = "单位名称";
                param.PARAMS_RELATION = "类似";
                param.PARAMS_VALUE = txtOrgName.Text;
                param.PARAMS_UNIT = "";
                param.PARAMS_TYPE = "varchar";
                lsParams.Add(param);
            }
            #endregion
            #region 申请表编号
            string strAppCode = "";
            if(!string.IsNullOrEmpty(txtAppCode.Text))
            {
                strAppCode = txtAppCode.Text.Replace(" ", ";");
                if (strAppCode.Length <= 5)
                {
                    strAppCode = strAppCode.TrimEnd(';') + ";" + ";" + ";";
                }
                else if (strAppCode.Length <= 10)
                {
                    strAppCode = strAppCode.TrimEnd(';') + ";" + ";";
                }
                else if (strAppCode.Length <= 15)
                {
                    strAppCode = strAppCode.TrimEnd(';') + ";";
                }
                param = new QUERY_PARAMS();
                param.PARAMS_NAME = "申请表号";
                param.PARAMS_RELATION = "类似";
                param.PARAMS_VALUE = strAppCode;// "3500;2014;0001";
                param.PARAMS_UNIT = "";
                param.PARAMS_TYPE = "varchar";
                lsParams.Add(param);
            }
            #endregion
            #region 资料表编号
            if(!string.IsNullOrEmpty(txtStatTdi.Text))
            {
                param = new QUERY_PARAMS();
                param.PARAMS_NAME = "资料表号";
                param.PARAMS_RELATION = "类似";
                param.PARAMS_VALUE = txtStatTdi.Text;
                param.PARAMS_UNIT = "";
                param.PARAMS_TYPE = "varchar";
                lsParams.Add(param);
            }
            #endregion
            #region 频率范围
            string strFreqValue = "", strFreqEFU = "";
            if(!string.IsNullOrEmpty(txtFreq_Efb.Text))
            {
                string strFreqType = "0";//发射和接收
                strFreqValue = txtFreq_Efb.Text.Trim() + ";" + txtFreq_Efe.Text.Trim() + ";" + strFreqType;
            }
            if (!string.IsNullOrEmpty(strFreqValue))
            {
                param = new QUERY_PARAMS();
                param.PARAMS_NAME = "频率范围";
                param.PARAMS_RELATION = "=";
                param.PARAMS_VALUE = strFreqValue;
                param.PARAMS_UNIT = strFreqEFU;
                param.PARAMS_TYPE = "double";
                lsParams.Add(param);
            }
            #endregion
            #region 通信业务
            if (!string.IsNullOrEmpty(cbNetSvn.Text))
            {
                ObservableCollection<object> obj = cbNetSvn.SelectedItems as ObservableCollection<object>;
                param = new QUERY_PARAMS();
                param.PARAMS_NAME = "通信业务";
                param.PARAMS_RELATION = "=";
                param.PARAMS_VALUE = ((BusinessDic)obj[0]).CN.ToString() + ";";
                param.PARAMS_UNIT = "";
                param.PARAMS_TYPE = "varchar";
                lsParams.Add(param);
            }
            #endregion
            return lsParams;
        }
        /// <summary>
        /// 查询事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Query_Click_1(object sender, RoutedEventArgs e)
        {
            if (OnExtractEvent != null)
            {
                OnExtractEvent(GetQueryParam());
            }
            this.Close();
        }
    }
}
