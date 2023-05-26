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
    /// This class manage a selected root value to make visible\invisible some components
    /// based on Favorite Folders panel
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SelectedRootConverter<T> : IValueConverter
    {
        public T True { get; set; }
        public T False { get; set; }

        public int RootID { get; set; }

        public SelectedRootConverter(T trueValue, T falseValue)
        {
            True = trueValue;
            False = falseValue;
        }

        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //return value is int && ((NavigatorFolderType)value == NavigatorFolderType.Favorite) ? True : False;
            return value is int && ((int)value == RootID) ? True : False;
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is T && EqualityComparer<T>.Default.Equals((T)value, True);
        }
    }
}
