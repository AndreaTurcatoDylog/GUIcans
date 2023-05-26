using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SpeedDetailsImageVisualizer
{
    public class DateTimeFilterDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DateTimeFilterNormalTemplate { get; set; }
        public DataTemplate DateTimeFilterTouchTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var isTouch = item as bool?;

            if (isTouch.HasValue)
            {
                return isTouch.Value ? DateTimeFilterTouchTemplate
                                     : DateTimeFilterNormalTemplate;
            }

            return null;
        }
    }
}
