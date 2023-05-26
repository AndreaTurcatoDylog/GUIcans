using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SpeedDetailsImageVisualizer
{
    public class IntToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Count() == 2 && values[0] != null && values[1] != null)
            {
                int selectedIndex = -1;
                var converted = Int32.TryParse(values[0].ToString(), out selectedIndex);
                if (converted)
                {
                    int value = -1;
                    converted = Int32.TryParse(values[1].ToString(), out value);
                    return (selectedIndex == value) ? Visibility.Visible : Visibility.Collapsed;
                }
            }

            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
