using Best.VWPlatform.Controls.Common;
using System;
using System.Windows.Interactivity;

namespace Best.VWPlatform.Controls.Behaviors
{
    /// <summary>
    /// 附加到NumericEditDX实例的控制其单位转换的行为
    /// </summary>
    public class FreqValueBehavior : Behavior<NumericEditDX>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.UnitChanged -= AssociatedObjectUnitChanged;
            AssociatedObject.UnitChanged += AssociatedObjectUnitChanged;
        }

        private void AssociatedObjectUnitChanged(object arg1, UnitsChangedArgs arg2)
        {
            if (string.IsNullOrWhiteSpace(arg2.NewUnit) ||
                string.IsNullOrWhiteSpace(arg2.OldUnit) ||
                string.Compare(arg2.OldUnit, arg2.NewUnit, StringComparison.InvariantCultureIgnoreCase) == 0)
                return;
            double dMin = 0;
            double dMax = 0;
            double dValue = 0;
            //统一转成Hz单位
            switch (arg2.OldUnit.ToUpper())
            {
                case "GHZ":
                    dMin = AssociatedObject.MinValue * 1000000000;
                    dMax = AssociatedObject.MaxValue * 1000000000;
                    dValue = AssociatedObject.Value * 1000000000;
                    break;
                case "MHZ":
                    dMin = AssociatedObject.MinValue * 1000000;
                    dMax = AssociatedObject.MaxValue * 1000000;
                    dValue = AssociatedObject.Value * 1000000;
                    break;
                case "KHZ":
                    dMin = AssociatedObject.MinValue * 1000;
                    dMax = AssociatedObject.MaxValue * 1000;
                    dValue = AssociatedObject.Value * 1000;
                    break;
                case "HZ":
                    dMin = AssociatedObject.MinValue;
                    dMax = AssociatedObject.MaxValue;
                    dValue = AssociatedObject.Value;
                    break;
            }
            if (dMax <= dMin)
            {
                //XGZ:硬编码不合适，应该根据实际设备的能力动态设置
                dMax = 6 * 1000 * 1000;
            }
            //AssociatedObject.MinValue = dMin;
            //if (dMax <= dMin)
            //{
            //    dMax = 6 * 1000 * 1000;
            //}

            //AssociatedObject.MaxValue = dMax;
            //AssociatedObject.Value = dValue;
            //AssociatedObject.Decimals = 9;
            //将Hz转成新的单位
            switch (arg2.NewUnit.ToUpper())
            {
                case "GHZ":
                    AssociatedObject.MinValue = dMin / 1000000000;
                    AssociatedObject.MaxValue = dMax / 1000000000;
                    AssociatedObject.Decimals = 9;
                    AssociatedObject.Value = dValue / 1000000000;
                    break;
                case "MHZ":
                    AssociatedObject.MinValue = dMin / 1000000;
                    AssociatedObject.MaxValue = dMax / 1000000;
                    AssociatedObject.Decimals = 6;
                    AssociatedObject.Value = dValue / 1000000;
                    break;
                case "KHZ":
                    AssociatedObject.MinValue = dMin / 1000;
                    AssociatedObject.MaxValue = dMax / 1000;
                    AssociatedObject.Decimals = 3;
                    AssociatedObject.Value = dValue / 1000;
                    break;
                case "HZ":
                    AssociatedObject.MinValue = dMin;
                    AssociatedObject.MaxValue = dMax;
                    AssociatedObject.Decimals = 0;
                    AssociatedObject.Value = dValue;
                    break;
            }
        }
    }
}
