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
    /// This Converter Enable\Disable the OK Button.
    /// The Values is an array of tree elements:
    /// [0] = IsTryConnecting: bool -> Specify whether the Navigator is trying to connect to a folder
    /// [1] = NavigatorButtons.Count: int -> It is the number of navigator buttons. If it is equals to 1 so it is the root
    /// [2] = SelectedItems.Count: int -> The number of the selected folder
    /// </summary>
    public class OkButtonEnabledConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Count() == 3 && values[0] is bool isTryingConnecting &&
                 values[1] is int navigatorButtonsCount && values[2] is int selectedItemsCount)
            {
                if (!isTryingConnecting)
                {
                    var isRoot = navigatorButtonsCount == 1;
                    if (isRoot)
                    {
                        // if is in a Root so the OK button is enabled only with a folder selected
                        var result = selectedItemsCount > 0;
                        return selectedItemsCount > 0;
                    }
                    else
                    {
                        // if is not in a Root so the OK button is enabled
                        return true;
                    }
                }
            }

            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
