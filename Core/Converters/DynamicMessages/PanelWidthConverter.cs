using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Core
{
    /// <summary>
    /// Applyed only on Dynamic Messages
    /// This converter calculate the total width of the component.
    /// TotalWidth = SlidePanelWidth + BookmarkWidth + 15
    /// BookmarkWidth = Width of text
    /// SlidePanelWidth = Width of bookmark
    /// 15 = Offset
    /// </summary>
    public class PanelWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double? totalWidth = 0;

            //Combine all the values passed to give a total width
            foreach (object o in values)
            {
                int current;
                bool parsed = int.TryParse(o.ToString(), out current);
                if (parsed)
                {
                    totalWidth += current;
                }
            }

            return totalWidth + 15;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
