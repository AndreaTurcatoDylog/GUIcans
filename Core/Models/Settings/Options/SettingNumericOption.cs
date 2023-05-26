using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public interface ISettingNumericOption : ISettingItemOption
    {
        double MinValue { get; }
        double MaxValue { get; }
        double Step { get; }
    }

    public class SettingNumericOption : ISettingNumericOption
    {
        public double MinValue { get; private set; }
        public double MaxValue { get; private set; }
        public double Step { get; private set; }

        public SettingNumericOption(double minValue, double maxValue, double step)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            Step = step;
        }
    }
}
