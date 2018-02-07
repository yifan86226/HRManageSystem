using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CO_IA.UI.Collection.DbEntity
{
    [Serializable()]
    public partial class RoadTest 
    {
        public RoadTest()
        {
            //TableName = "RoadTest";
            //Schema = "dbo";
            //EntityMap = EntityMapType.Table;
            //IdentityName = "标识字段名";

            //PrimaryKeys.Add("主键字段名");


        }

        /// <summary>
        /// 
        /// </summary>
        public System.Int32 RoadTestId
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public System.String RoadTestName
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public System.String CarPlate
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public System.String ReciverIp
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public System.Int32 ReciverPort
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public System.Int32 TestSample
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public System.DateTime TestDateTime
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public System.String FreqDataTable
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public System.Int64 StartFreq
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public System.Int64 EndFreq
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public System.Int32 Bandwidth
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public System.String TestStaffName
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public System.String Bz
        {
            get;
            set;
        }
    }
}
