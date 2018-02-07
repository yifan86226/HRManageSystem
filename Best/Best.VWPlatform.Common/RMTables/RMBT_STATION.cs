using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.RMTables
{
    /// <summary>
    /// 监测执行站表
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public partial class RMBT_STATION : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 监测执行站编号 VARCHAR2(8)
        /// </summary>
        //[TableFieldMap(PrimaryKey = true)]
        public string MSID = "";
        public string _MSID
        {
            get { return MSID; }
            set
            {
                MSID = value;
                NotifyPropertyChange("_MSID");
            }
        }

        private bool _msidIsReadOnly = true;
        public bool MSIDIsReadOnly
        {
            get { return _msidIsReadOnly; }
            set { _msidIsReadOnly = value; }
        }
        /// <summary>
        /// 名称 VARCHAR2(50)
        /// </summary>
        //[TableFieldMap]
        public string Name = "";
        public string _Name
        {
            get { return Name; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    return;
                Name = value;
                NotifyPropertyChange("_Name");
            }
        }
        private bool ValideName(string pName)
        {
            if (pName.Length > 50)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private void NotifyPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// 类型 NUMBER(2)
        /// 1-	固定监测站
        /// 2-	移动监测车
        /// 3-	可搬移监测站
        /// </summary>
        //[TableFieldMap]
        public int Type = 0;
        public int _Type
        {
            get { return Type; }
            set
            {
                Type = value;
                TypeToVisibility = int.Parse(Type.ToString().Substring(0, 1));
                NotifyPropertyChange("_Type");
                NotifyPropertyChange("TypeToVisibility");
            }
        }
        private int _typeToVisibility = 3;
        public int TypeToVisibility
        {
            get { return _typeToVisibility; }
            set
            {
                _typeToVisibility = value;
                NotifyPropertyChange("TypeToVisibility");
            }
        }
        /// <summary>
        /// 经度 NUMBER(12,9) 单位：度
        /// </summary>
        //[TableFieldMap]
        public double Longitude = 0;
        public double _Longitude
        {
            get { return Longitude; }
            set
            {
                Longitude = value;
                NotifyPropertyChange("_Longitude");
            }
        }

        /// <summary>
        /// 纬度 NUMBER(12,9) 单位：度
        /// </summary>
        //[TableFieldMap]
        public double Latitude = 0;
        public double _Latitude
        {
            get { return Latitude; }
            set
            {
                Latitude = value;
                NotifyPropertyChange("_Latitude");
            }
        }
        /// <summary>
        /// 磁偏角 NUMBER(5,2) 单位：度
        /// </summary>
        //[TableFieldMap]
        public double MagAngle = 0;
        public double _MagAngle
        {
            get { return MagAngle; }
            set
            {
                MagAngle = value;
                NotifyPropertyChange("_MagAngle");
            }
        }
        /// <summary>
        /// 备注 VARCHAR2(512)
        /// </summary>
        //[TableFieldMap]
        public string Memo = "";
        public string _Memo
        {
            get { return Memo; }
            set
            {
                Memo = value;
                NotifyPropertyChange("_Memo");
            }
        }


        /// <summary>
        /// 重写tostring
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }
    }//end of class RMBT_STATION
    /// <summary>
    /// RMBT_STATION表列名称定义
    /// </summary>
    public partial class RMBT_STATION
    {
        /// <summary>
        /// 表名称
        /// </summary>
        public const string TableName = "RMBT_STATION";
        /// <summary>
        /// 列名称-站编号
        /// </summary>
        public const string FieldName_MSID = "MSID";
    }//end of class RMBT_STATION
    /// <summary>
    /// RMBT_STATION列表,提供高级检索
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class RMBT_STATIONList : List<RMBT_STATION>
    {
        /// <summary>
        /// 根据监测执行站编号查找项目
        /// </summary>
        /// <param name="statID">监测执行站编号</param>
        /// <returns>项目</returns>
        public RMBT_STATION FindItemByStatID(string statID)
        {
            int i = 0;
            RMBT_STATION ret = null;
            for (i = 0; i < this.Count; i++)
            {
                if (this[i].MSID == statID)
                {
                    ret = this[i];
                    break;
                }
            }//end for i
            return ret;
        }//end of function FindItemByStatID
    } //end of class RMBT_STATIONList
}
