using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SpeedDetailsImageVisualizer
{
    /// <summary>
    ///  Modify the Path Image with ">" character
    /// </summary>
    public class ConvertPathToViewFormatPathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string sourcePath && !string.IsNullOrEmpty(sourcePath))
            {
                try
                {
                    var tempString = sourcePath;

                    // Check whether it is a net address
                    var isNetAddress = sourcePath.ElementAt(0) == '\\';

                    // Modify the shown string if it is a net address
                    if (isNetAddress)
                    {
                        tempString = sourcePath.Substring(2, sourcePath.Count() - 2);
                    }

                    // Modify the path in view Format (replace the '\' in '>')
                    return tempString.Replace(@"\", " > ");
                }
                catch(Exception)
                {
                    return null;
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter;
        }
    }
}
