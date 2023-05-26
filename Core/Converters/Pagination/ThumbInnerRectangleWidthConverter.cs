using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Core
{    
    public class ThumbInnerRectangleWidthConverter : IValueConverter
    {
        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double innerRectnagleWidth && innerRectnagleWidth - 10 > 5)
            {
                return innerRectnagleWidth = innerRectnagleWidth - 10;

            }

            return 5;
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
