using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class ActivityEquipment : Equipment
    {
        private string activityGuid;

        public string ActivityGuid
        {
            get
            {
                return this.activityGuid;
            }
            set
            {
                if (value != this.activityGuid)
                {
                    this.activityGuid = value;
                    this.NotifyPropertyChanged("ActivityGuid");
                }
            }
        }

        private string placeGuid;

        public string PlaceGuid
        {
            get
            {
                return this.placeGuid;
            }
            set
            {
                if (value != this.placeGuid)
                {
                    this.placeGuid = value;
                    this.NotifyPropertyChanged("PlaceGuid");
                }
            }
        }

        private double? assignSendFreq;
        public double? AssignSendFreq
        {
            get 
            {
                return assignSendFreq;
            }
            set
            {
                assignSendFreq = value;
                NotifyPropertyChanged("AssignSendFreq");
            }
        }

        private double? assignSpareFreq;
        /// <summary>
        /// 
        /// </summary>
        public double? AssignSpareFreq
        {
            get 
            {
                return assignSpareFreq;
            }
            set
            {
                assignSpareFreq = value;
                NotifyPropertyChanged("AssignSpareFreq");
            }
        }
        public void CopyFrom(Equipment equipment)
        {
            this.Band_kHz = equipment.Band_kHz;
            this.BusinessCode = equipment.BusinessCode;
            this.EQUCount = equipment.EQUCount;
            this.EquipmentClassID = equipment.EquipmentClassID;
            this.EquModel = equipment.EquModel;
            this.FreqRange = equipment.FreqRange;
            this.IsMobile = equipment.IsMobile;
            this.IsStation = equipment.IsStation;
            this.IsTunable = equipment.IsTunable;
            this.Key = equipment.Key;
            this.Modulation = equipment.Modulation;
            this.Name = equipment.Name;
            this.OrgInfo = equipment.OrgInfo;
            this.Power_W = equipment.Power_W;
            this.ReceiveFreq = equipment.ReceiveFreq;
            this.Remark = equipment.Remark;
            this.SendFreq = equipment.SendFreq;
            this.SeriesNumber = equipment.SeriesNumber;
            this.StationName = equipment.StationName;
            this.StationTDI = equipment.StationTDI;
            this.StClassCode = equipment.StClassCode;
            this.Longitude = equipment.Longitude;
            this.Latitude = equipment.Latitude;
            this.Address = equipment.Address;
            this.SpareFreq = equipment.SpareFreq;
        }
    }
}

