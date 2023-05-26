using Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SpeedDetailsImageVisualizer
{
    /// <summary>
    /// Get a status enum and convert it into a Status Color
    /// </summary>
    public class ConvertigStatusToStatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (CreateReportStatusEnum)value;
            switch (status)
            {
                case CreateReportStatusEnum.Creating: return new SolidColorBrush(Colors.DarkBlue);
                case CreateReportStatusEnum.Success: return new SolidColorBrush(Colors.DarkGreen);
                case CreateReportStatusEnum.Warning: return new SolidColorBrush(Colors.DarkOrange);
                case CreateReportStatusEnum.Error: return new SolidColorBrush(Colors.DarkRed);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter;
        }
    }
}
