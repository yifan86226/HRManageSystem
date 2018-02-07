using CO_IA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.UI.StationPlan
{
    public class EquFreqPlanSegmentHelper
    {
        public static FreqPlanSegment GetFreqPlanSegment(ActivityEquipmentInfo EquInfo)
        {
            FreqPlanSegment segment = null;
            List<CO_IA.Data.FreqPlanSegment> segmentList = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan, List<CO_IA.Data.FreqPlanSegment>>(
                channel =>
                {
                    return channel.GetFreqPlanInfo();
                });
            foreach (FreqPlanSegment freqPlanSegment in segmentList)
            {
                //if (freqPlanSegment.FreqValue.Little / (double)1000000 <= EquInfo.SendFreqStart.Value
                //    && freqPlanSegment.FreqValue.Great / (double)1000000 >= EquInfo.SendFreqEnd.Value)
                if (freqPlanSegment.ClassCode == EquInfo.BusinessCode)
                {
                    segment = freqPlanSegment;
                    break;
                }
            }
            return segment;
        }
    }
}
