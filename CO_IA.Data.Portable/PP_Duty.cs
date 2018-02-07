#region 文件描述
/**********************************************************************************
 * 创建人：Xiaguohui
 * 摘  要：职责
 * 日  期：2017-05-16
 * ********************************************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class PP_Duty : AT_BC.Data.CheckableData<string>
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; this.NotifyPropertyChanged("Name"); }
        }

        private string description;

        public string Description
        {
            get { return description; }
            set { description = value; NotifyPropertyChanged("Description"); }
        }
                
    }

    public class PP_DutyInfo : PP_Duty
    {
        private string[] queryRights;

        public string[] QueryRights
        {
            get
            {
                return this.queryRights;
            }
            set
            {
                this.queryRights = value;
                this.NotifyPropertyChanged("QueryRights");
            }
        }

        private string[] manageRights;

        public string[] ManageRights
        {
            get
            {
                return this.manageRights;
            }
            set
            {
                this.manageRights = value;
                this.NotifyPropertyChanged("ManageRights");
            }
        }
    }
}
