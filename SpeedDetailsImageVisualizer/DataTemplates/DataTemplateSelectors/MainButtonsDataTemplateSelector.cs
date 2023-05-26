using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SpeedDetailsImageVisualizer
{
    public class MainButtonsDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NormalButtonsTemplate { get; set; }
        public DataTemplate TouchButtonsTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var isTouch = item as bool?;

            if (isTouch.HasValue)
            {
                return isTouch.Value ? TouchButtonsTemplate
                                     : NormalButtonsTemplate;                
            }

            return null;
        }
    }
}
