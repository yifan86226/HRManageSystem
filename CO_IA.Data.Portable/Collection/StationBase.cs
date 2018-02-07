using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data.Collection
{
    /// <summary>
    /// 台站基础信息
    /// </summary>
    public class StationBase
    {
        private string _guid;
        private string _placeGuid;
        private string _appCode;
        private string _statTdi;
        private string _statAppType;
        private string _orgGuid;
        private string _orgName;
        private string _orgLinkPerson;
        private string _orgPhone;
        private string _statName;
        private string _statAddr;
        private string _netSvn;
        private double? _statLg;
        private double? _statLa;
        private string _activityGuid;
        private string _activityFreqGuid;
        private string _appGuid;

        public string Guid
        {
            get { return _guid; }
            set { _guid = value; }
        }
        /// <summary>
        /// 活动地点ID
        /// </summary>
        public string PlaceGuid
        {
            get { return _placeGuid; }
            set { _placeGuid = value; }
        }
         
        /// <summary>
        /// 申请表编号
        /// </summary>
        public string AppCode
        {
            get { return _appCode; }
            set { _appCode = value; }
        }
        /// <summary>
        /// 资料表编号
        /// </summary>
        public string StatTdi
        {
            get { return _statTdi; }
            set { _statTdi = value; }
        }
        /// <summary>
        /// 资料表类型
        /// </summary>
        public string StatAppType
        {
            get { return _statAppType; }
            set { _statAppType = value; }
        }
        /// <summary>
        /// 组织单位ID
        /// </summary>
        public string OrgGuid
        {
            get { return _orgGuid; }
            set { _orgGuid = value; }
        }
        /// <summary>
        /// 组织单位名称
        /// </summary>
        public string OrgName
        {
            get { return _orgName; }
            set { _orgName = value; }
        }
        /// <summary>
        /// 单位联系人
        /// </summary>
        public string OrgLinkPerson
        {
            get { return _orgLinkPerson; }
            set { _orgLinkPerson = value; }
        }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string OrgPhone
        {
            get { return _orgPhone; }
            set { _orgPhone = value; }
        }
        /// <summary>
        /// 台站名称
        /// </summary>
        public string StatName
        {
            get { return _statName; }
            set { _statName = value; }
        }
        /// <summary>
        /// 台站地址
        /// </summary>
        public string StatAddr
        {
            get { return _statAddr; }
            set { _statAddr = value; }
        }
        /// <summary>
        /// 通信系统(字典表)
        /// </summary>
        public string NetSvn
        {
            get { return _netSvn; }
            set { _netSvn = value; }
        }
        /// <summary>
        /// 经度
        /// </summary>
        public double? StatLg
        {
            get { return _statLg; }
            set { _statLg = value; }
        }
        /// <summary>
        /// 纬度
        /// </summary>
        public double? StatLa
        {
            get { return _statLa; }
            set { _statLa = value; }
        }
        /// <summary>
        /// 活动的guid
        /// </summary>
        public string ActivityGuid
        {
            get { return _activityGuid; }
            set { _activityGuid = value; }
        }
        /// <summary>
        /// 活动规划频率Guid（不是频率Guid(发射设备GUID)）
        /// </summary>
        public string ActivityFreqGuid
        {
            get { return _activityFreqGuid; }
            set { _activityFreqGuid = value; }
        }
        /// <summary>
        /// 申请表Guid，而AppCode是申请表编号，二者不同
        /// </summary>
        public string AppGuid
        {
            get { return _appGuid; }
            set { _appGuid = value; }
        }

    }
    public class StationBaseExt : StationBase
    {
        public StationBaseExt(StationBase p_stat)
        {
            AppCode = p_stat.AppCode;
            Guid = p_stat.Guid;
            NetSvn = p_stat.NetSvn;
            OrgGuid = p_stat.OrgGuid;
            OrgLinkPerson = p_stat.OrgLinkPerson;
            OrgName = p_stat.OrgName;
            OrgPhone = p_stat.OrgPhone;
            PlaceGuid = p_stat.PlaceGuid;
            StatAddr = p_stat.StatAddr;
            StatAppType = p_stat.StatAppType;
            StatLa = p_stat.StatLa;
            StatLg = p_stat.StatLg;
            StatName = p_stat.StatName;
            StatTdi = p_stat.StatTdi;
        }
        private List<EmitInfo> _emitInfoList = new List<EmitInfo>();

        public List<EmitInfo> EmitInfoList
        {
            get { return _emitInfoList; }
            set { _emitInfoList = value; }
        }
       
    }
}
