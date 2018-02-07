#region 文件描述
/***************************************************************#@#***************************************************************
 * 创建人：xiaguohui
 * 摘 要 ：存储地点、监测组、位置关联信息
 * 日 期 ：2016-09-20
 ***************************************************************#@#***************************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data.MonitorPlan
{
    public class OrgDetail : PP_OrgInfo
    {
        /// <summary>
        /// 子集合
        /// </summary>
        public List<OrgDetail> Children { get; set; }
        public OrgDetail()
        {
            Children = new List<OrgDetail>();
        }      
      

        private string placeID;
        /// <summary>
        /// 地点ID
        /// </summary>
        public string PlaceID
        {
            get
            {
                return placeID;
            }

            set
            {
                placeID = value; NotifyPropertyChange("PlaceID"); 
            }
        }

        //private string orgID;
        ///// <summary>
        ///// 监测组ID
        ///// </summary>
        //public string OrgID
        //{
        //    get
        //    {
        //        return orgID;
        //    }

        //    set
        //    {
        //        orgID = value; NotifyPropertyChange("OrgID");
        //    }
        //}

        private string locationID;
        /// <summary>
        /// 位置ID
        /// </summary>
        public string LocationID
        {
            get
            {
                return locationID;
            }

            set
            {
                locationID = value; NotifyPropertyChange("LocationID");
            }
        }

        //private string orgName;
        ///// <summary>
        ///// 监测组名称
        ///// </summary>
        //public string OrgName
        //{
        //    get
        //    {
        //        return orgName;
        //    }

        //    set
        //    {
        //        orgName = value; NotifyPropertyChange("OrgName");
        //    }
        //}
        private int orgType;
        /// <summary>
        /// 监测组类别 车、人；   0:人；1:车;  -1:非监测组 2：人在线；3：车在线；4：人离线；5：车离线
        /// </summary>
        public int OrgType
        {
            get
            {
                return orgType;
            }

            set
            {
                orgType = value; NotifyPropertyChange("OrgType");
            }
        }

        private string locationName;
        /// <summary>
        /// 地点名称
        /// </summary>
        public string LocationName
        {
            get
            {
                return locationName;
            }

            set
            {
                locationName = value; NotifyPropertyChange("LocationName");
            }
        }

        private double locationLG;
        /// <summary>
        /// 位置经度
        /// </summary>
        public double LocationLG
        {
            get
            {
                return locationLG;
            }

            set
            {
                locationLG = value; NotifyPropertyChange("LocationLG");
            }
        }

        private double locationLA;
        /// <summary>
        /// 位置纬度
        /// </summary>
        public double LocationLA
        {
            get
            {
                return locationLA;
            }

            set
            {
                locationLA = value; NotifyPropertyChange("LocationLA");
            }
        }

        private string vehicleId;
        /// <summary>
        /// 车ID
        /// </summary>
        public string VehicleId
        {
            get
            {
                return vehicleId;
            }

            set
            {
                vehicleId = value; NotifyPropertyChange("VehicleId");
            }
        }

        private string vehicleNum;
        /// <summary>
        /// 车牌
        /// </summary>
        public string VehicleNum
        {
            get
            {
                return vehicleNum;
            }

            set
            {
                vehicleNum = value; NotifyPropertyChange("VehicleNum");
            }
        }
    }

    public class GroupAndLocation : ActivityPlaceLocation, INotifyPropertyChanged
    {
        private bool isChecked;
        /// <summary>
        ///  选择
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return this.isChecked;
            }
            set
            {
                this.isChecked = value;
                this.NotifyPropertyChanged("IsChecked");
            }
        }
        /// <summary>
        /// 监测组id
        /// </summary>
        public string GroupGuid { get; set; }
        /// <summary>
        /// 监测组名称
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// 任务开始时间
        /// </summary>
        public DateTime BeginTime { get; set; }
        /// <summary>
        /// 任务结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 监测id列表
        /// </summary>
        public List<string> GroupIDs { get; set; }
        /// <summary>
        /// 位置列表
        /// </summary>
        public ActivityPlaceLocation[] Locations { get; set; }

         /// <summary>
        /// 组织结构下人员信息
        /// </summary>
        public List<PP_PersonInfo> Persons { get; set; }
        public GroupAndLocation()
        {
            Persons = new List<PP_PersonInfo>();
        }
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 通知属性变更方法
        /// </summary>
        /// <param name="propertyName">发生变更的属性名称</param>
        private void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
