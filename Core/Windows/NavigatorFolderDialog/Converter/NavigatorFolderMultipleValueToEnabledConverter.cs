using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Core
{
    /// <summary>
    /// This is a multiple value to enabled converter     
    /// value[0]: Count of Selected Folders
    /// value[1]: Trying connection
    /// Enabled: if it is not trying connection AND almost one folder is selected
    /// </summary>
    public class NavigatorFolderMultipleValueToEnabledConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Count() == 2)
            {
                if (values[0] is int && values[1] is bool)
                {
                    // Get the Is Try connecting
                    var tryConnecting = (bool)values[1];
                    if (!tryConnecting)
                    {                       
                        // Get the number of selected folder
                        return (int)values[0] >0;
                    }

                    return false;
                }
            }

            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
