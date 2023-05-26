using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SpeedDetailsImageVisualizer
{
    public class InformationImageDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NormalTextBoxesTemplate { get; set; }
        public DataTemplate TouchTextBoxesTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var isTouch = item as bool?;

            if (isTouch.HasValue)
            {
                return isTouch.Value ? TouchTextBoxesTemplate
                                     : NormalTextBoxesTemplate;
            }

            return null;
        }
    }
}
