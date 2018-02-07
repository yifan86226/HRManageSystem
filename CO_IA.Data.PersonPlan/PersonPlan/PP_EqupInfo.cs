#region 文件描述
/**********************************************************************************
 * 创建人：Liuenliang
 * 摘  要：人员预案装备实体
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
    public class PP_EquipInfo: INotifyPropertyChanged 
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


        private bool isChecked = false;
        /// <summary>
        /// 是否被选则
        /// </summary>
        public bool ISCHECKED
        {
            get { return isChecked; }
            set { isChecked = value; NotifyPropertyChange("ISCHECKED"); }
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

        private string name;

        public string NAME
        {
            get { return name; }
            set { name = value; NotifyPropertyChange("NAME"); }
        }


        private string model;

        public string MODEL
        {
            get { return model; }
            set { model = value; NotifyPropertyChange("MODEL"); }
        }



        private string equip_numb;

        public string EQUIP_NUMB
        {
            get { return equip_numb; }
            set { equip_numb = value; NotifyPropertyChange("EQUIP_NUMB"); }
        }

        private int add_type;

        public int ADD_TYPE
        {
            get { return add_type; }
            set { add_type = value; NotifyPropertyChange("ADD_TYPE"); }
        }


    }
}
