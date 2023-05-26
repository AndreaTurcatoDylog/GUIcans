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
    /// This converter returns a value dimension (Width or Height) in according of parameter.
    /// The parameter is the With\Height of the parent component; the result is the Width\Height decreased bya specific value
    /// </summary>
    public class DimensionImageConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var dimension = (double)value;
                if (dimension == 0)
                {
                    return dimension;
                }

                return dimension - 10;
            }
            catch(Exception ex)
            {
                CoreLog.Instance.Append($"[{ex.Source}] - {ex.Message} - {ex.StackTrace}", CoreLogType.Error);

                return 0.0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter;
        }
    }
}
