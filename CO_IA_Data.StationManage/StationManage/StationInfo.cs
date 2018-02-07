using AT_BC.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA_Data
{
    public class StationInfo : CheckableData<string>
    {
        //private bool isChecked = false;

        //public bool IsChecked
        //{
        //    get { return isChecked; }
        //    set
        //    {
        //        isChecked = value;
        //        this.NotifyPropertyChanged("IsChecked");
        //    }
        //}

        private string app_guid;
        /// <summary>
        /// 申请表guid
        /// </summary>
        public string APPGUID
        {
            get { return app_guid; }
            set { app_guid = value; }
        }

        private string stat_guid;
        /// <summary>
        /// 台站id
        /// </summary>
        public string STATGUID
        {
            get { return stat_guid; }
            set { stat_guid = value; }
        }
        private string stat_tdi;

        /// <summary>
        /// 技术资料编号
        /// </summary>
        public string STAT_TDI
        {
            get { return stat_tdi; }
            set { stat_tdi = value; }
        }
        private string app_code;

        /// <summary>
        /// 申请表编号
        /// </summary>
        public string APP_CODE
        {
            get { return app_code; }
            set { app_code = value; }
        }
        private string stat_app_type;

        /// <summary>
        /// 技术资料申报表类型
        /// </summary>
        public string STAT_APP_TYPE
        {
            get { return stat_app_type; }
            set { stat_app_type = value; }
        }

        private string net_svn;
        /// <summary>
        /// 业务类型/通信系统
        /// </summary>
        public string NET_SVN
        {
            get { return net_svn; }
            set { net_svn = value; }
        }

        public string orgsyscode;
        /// <summary>
        /// 系统/行业
        /// </summary>
        public string ORGSYSCODE
        {
            get { return orgsyscode; }
            set { orgsyscode = value; }
        }

        private string stat_name;
        /// <summary>
        /// 台站名称
        /// </summary>
        public string STAT_NAME
        {
            get { return stat_name; }
            set { stat_name = value; }
        }

        private string stat_addr;
        /// <summary>
        /// 台站地址
        /// </summary>
        public string STAT_ADDR
        {
            get { return stat_addr; }
            set { stat_addr = value; }
        }

        private string org_name;
        /// <summary>
        /// 设台单位名称
        /// </summary>
        public string ORG_NAME
        {
            get { return org_name; }
            set { org_name = value; }
        }

        private string freq_lc;
        public string FREQ_LC
        {
            get { return freq_lc; }
            set { freq_lc = value; }
        }

        private string freq_uc;
        /// <summary>
        /// 
        /// </summary>
        public string FREQ_UC
        {
            get { return freq_uc; }
            set { freq_uc = value; }
        }

        private string power;
        public string Power
        {
            get { return power; }
            set { power = value; }
        }

        private double stat_lg;
        public double STAT_LG
        {
            get { return stat_lg; }
            set { stat_lg = value; }
        }

        private double stat_la;
        public double STAT_LA
        {
            get { return stat_la; }
            set { stat_la = value; }
        }

        private string stat_at;
        /// <summary>
        /// 海拔
        /// </summary>
        public string STAT_AT
        {
            get { return stat_at; }
            set { stat_at = value; }
        }
    }
}
