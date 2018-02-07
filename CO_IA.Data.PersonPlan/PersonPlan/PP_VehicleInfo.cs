#region 文件描述
/**********************************************************************************
 * 创建人：Liuenliang
 * 摘  要：人员预案车辆信息实体
 * 日  期：2016-08-17
 * ********************************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CO_IA.Data.PersonPlan
{
    public class PP_VehicleInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        private void NotifyPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                //根据PropertyChanged事件的委托类，实现PropertyChanged事件： 
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        } 



        private string guid;

        public string GUID
        {
            get { return guid; }
            set { guid = value; NotifyPropertyChange("GUID"); }
        }

        private string org_guid;

        public string ORG_GUID
        {
            get { return org_guid; }
            set { org_guid = value; NotifyPropertyChange("ORG_GUID"); }
        }

        private string vehicle_numb;

        public string VEHICLE_NUMB
        {
            get { return vehicle_numb; }
            set { vehicle_numb = value; NotifyPropertyChange("VEHICLE_NUMB"); }
        }


        private string vehicle_model;

        public string VEHICLE_MODEL
        {
            get { return vehicle_model; }
            set { vehicle_model = value; NotifyPropertyChange("VEHICLE_MODEL"); }
        }





        private string vehicle_type;

        public string VEHICLE_TYPE
        {
            get { return vehicle_type; }
            set { vehicle_type = value; NotifyPropertyChange("VEHICLE_TYPE"); }
        }




        private string driver;

        public string DRIVER
        {
            get { return driver; }
            set { driver = value; NotifyPropertyChange("DRIVER"); }
        }

        private string driver_phone;

        public string DRIVER_PHONE
        {
            get { return driver_phone; }
            set { driver_phone = value; NotifyPropertyChange("DRIVER_PHONE"); }
        }

        private string other_numb;

        public string OTHER_NUMB
        {
            get { return other_numb; }
            set { other_numb = value; NotifyPropertyChange("OTHER_NUMB"); }
        }

        private string remarks;

        public string REMARKS
        {
            get { return remarks; }
            set { remarks = value; NotifyPropertyChange("REMARKS"); }
        }


        private int add_type;

        public int ADD_TYPE
        {
            get { return add_type; }
            set { add_type = value; NotifyPropertyChange("ADD_TYPE"); }
        }
    }
}
