using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Core
{
    /// <summary>
    /// Applyed only on Circular Progress bar
    /// </summary>
    public class ProgressToAngleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double result = 0;

            try
            {
                double progress = (double)values[0];
                System.Windows.Controls.ProgressBar bar = values[1] as System.Windows.Controls.ProgressBar;

                result = 359.999 * (progress / (bar.Maximum - bar.Minimum));
            }
            catch(Exception ex)
            {
                CoreLog.Instance.Append($"[{ex.Source}] - {ex.Message} - {ex.StackTrace}", CoreLogType.Error);
            }

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
