using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO_IA.Types;

namespace CO_IA.Data
{
    public class TaskDisturbInfo : AT_BC.Data.NotifyPropertyChangedObject
    {

        private string disturbedOrg;
        public string DisturbedOrg
        {
            get
            {
                return this.disturbedOrg;
            }
            set
            {
                if (this.disturbedOrg != value)
                {
                    this.disturbedOrg = value;
                    this.NotifyPropertyChanged("DisturbedOrg");
                }
            }
        }

        private string contact;
        /// <summary>
        /// 单位联系人
        /// </summary>
        public string Contact
        {
            get
            {
                return contact;
            }
            set
            {
                contact = value;
                NotifyPropertyChanged("Contact");
            }
        }

        private string phone;

        /// <summary>
        /// 联系人电话
        /// </summary>
        public string Phone
        {
            get
            {
                return phone;
            }
            set
            {
                phone = value;
                NotifyPropertyChanged("Phone");
            }
        }

        private double disturbedMHzFreq;

        public double DisturbedMHzFreq
        {
            get
            {
                return this.disturbedMHzFreq;
            }
            set
            {
                if (this.disturbedMHzFreq != value)
                {
                    this.disturbedMHzFreq = value;
                    this.NotifyPropertyChanged("DisturbedMHzFreq");
                }
            }
        }

        /// <summary>
        /// 任务描述
        /// </summary>
        private string equipmentModel;

        /// <summary>
        /// 获取或设置任务描述
        /// </summary>
        public string EquipmentModel
        {
            get
            {
                return this.equipmentModel;
            }
            set
            {
                if (value != this.equipmentModel)
                {
                    this.equipmentModel = value;
                    this.NotifyPropertyChanged("EquipmentModel");
                }
            }
        }

        private DisturbType disturbType;

        public DisturbType DisturbType
        {
            get
            {
                return this.disturbType;
            }
            set
            {
                this.disturbType = value;
                this.NotifyPropertyChanged("DisturbType");
            }
        }

        private DisturbLevel disturbLevel;

        public DisturbLevel DisturbLevel
        {
            get
            {
                return this.disturbLevel;
            }
            set
            {
                this.disturbLevel = value;
                this.NotifyPropertyChanged("DisturbLevel");
            }
        }
    }
}
