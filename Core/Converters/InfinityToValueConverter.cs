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
    /// This Converter prevents the error when a value become Infinity
    /// </summary>
    public class InfinityToValueConverter : IValueConverter
    {
        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = value;
            if (value is double doubleValue && double.IsInfinity(doubleValue))
            {
                result = 0;
            }

            return result;
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}