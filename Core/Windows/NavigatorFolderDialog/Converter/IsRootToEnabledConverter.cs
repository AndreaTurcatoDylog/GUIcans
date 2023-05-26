using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Core
{
    /// <summary>
    /// This converter get a Count of Navigator button to make Enabled\Disabled a component.
    /// Count > 1: Enable
    /// Count == 1: Disable
    /// </summary>
    public class IsRootToEnabledConverter : IValueConverter
    {

        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //var pippo = (int)value == 1;
            return value is int && ((int)value >1);
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
