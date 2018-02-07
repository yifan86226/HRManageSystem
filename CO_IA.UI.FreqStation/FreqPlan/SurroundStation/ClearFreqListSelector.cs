using CO_IA.Data;
using CO_IA_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace CO_IA.UI.FreqStation.FreqPlan.SurroundStation
{
    public class ClearFreqListSelector : DataTemplateSelector
    {
        private DataTemplate successfulTemplate = null;

        public DataTemplate SuccessfulTemplate
        {
            get { return successfulTemplate; }
            set { successfulTemplate = value; }
        }

        private DataTemplate failureTemplate = null;
        public DataTemplate FailureTemplate
        {
            get { return failureTemplate; }
            set { failureTemplate = value; }
        }

        private DataTemplate noClearTemplate = null;
        public DataTemplate NoClearTemplate
        {
            get { return noClearTemplate; }
            set { noClearTemplate = value; }
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is StationEmitInfo)
            {
                StationEmitInfo emitInfo = item as StationEmitInfo;
                switch (emitInfo.ClearResult)
                {
                    case ClearResultEnum.ClearSucceed:
                        return SuccessfulTemplate;
                    case ClearResultEnum.ClearFail:
                        return FailureTemplate;
                    default:
                        return NoClearTemplate;
                }
            }

            return base.SelectTemplate(item, container);
        }

    }
}
