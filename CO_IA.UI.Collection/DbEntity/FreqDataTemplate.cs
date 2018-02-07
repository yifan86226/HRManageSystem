using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
//using PWMIS.Common;
//using PWMIS.DataMap.Entity;

namespace CO_IA.UI.Collection
{
    [Serializable()]
    public partial class FreqDataTemplate
    {
        public FreqDataTemplate()
        {
            //TableName = "FreqDataTemplate";
            //Schema = "dbo";
            //EntityMap = EntityMapType.Table;
            //IdentityName = "标识字段名";

            //PrimaryKeys.Add("主键字段名");


        }


        /// <summary>
        /// 
        /// </summary>
        public System.Int32 DataIndex
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public System.Int32 MeasureId
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public System.Int32 FreqMeasurePakageId
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public System.DateTime MeasureTime
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public System.Double LongitudeValue
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public System.Double LatitudeValue
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public System.Double AltitudeValue
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public System.Double CarSpeed
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public System.Int32 SatelliteCount
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public System.Double StartFreq
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public System.Double StepValue
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public System.Int32 FreqCount
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public System.Byte[] FreqData
        {
            get;
            set;
        }
    }
}
