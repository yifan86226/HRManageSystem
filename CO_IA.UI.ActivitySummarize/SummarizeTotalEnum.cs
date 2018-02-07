using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.UI.ActivitySummarize
{
    public class SummarizeTotalEnum
    {
        public enum SummarizeTotalEnumType
        {
            [Description("保障人员")]
            guaranteePerson = 1,
            [Description("现场保障人次")]
            guaranteeCount = 2,
            [Description("监测车辆")]
            monitorCar = 3,
            [Description("机动监测台次")]
            monitorCount = 4,
            [Description("固定监测站")]
            monitorStand = 5,
            [Description("单站监测时长")]
            monitorTime = 6,
            [Description("指配频率")]
            distribution = 7,
            [Description("审批无线电台（家）")]
            approveRadio = 8,
            [Description("抽检设备")]
            checkEquipment = 9,
            [Description("查处干扰")]
            investigateInterference = 10,
            [Description("审批无线电台总数")]
            approveRadioTotal = 11
        }
        /// <summary>
        /// 返回枚举类型的描述信息
        /// </summary>
        /// <param name="myEnum"></param>
        /// <returns></returns>
        public string GetDiscription(System.Enum myEnum)
        {

            System.Reflection.FieldInfo fieldInfo = myEnum.GetType().GetField(myEnum.ToString());
            object[] attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
            if (attrs != null && attrs.Length > 0)
            {
                DescriptionAttribute desc = attrs[0] as DescriptionAttribute;
                if (desc != null)
                {
                    return desc.Description.ToLower();
                }
            }
            return myEnum.ToString();
        }
    }


}
