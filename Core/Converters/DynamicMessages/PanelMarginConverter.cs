using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Core
{
    /// <summary>
    /// Applyed only on Dynamic Messages
    /// This converter calculate the Margin for expand\collapse the Slider Panel
    /// </summary>
    public class PanelMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is double)
            {
                var width = (double)value;
                return new Thickness((width * -1) -15, 0, 0, 0);
            }

            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
