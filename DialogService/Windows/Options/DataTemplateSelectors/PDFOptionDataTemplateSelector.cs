using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DialogService
{
    public class PDFOptionDataTemplateSelector: DataTemplateSelector
    {
        public DataTemplate TitlesNormalTemplate { get; set; }
        public DataTemplate TitlesTouchTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var isTouch = item as bool?;

            if (isTouch.HasValue)
            {
                return isTouch.Value ? TitlesTouchTemplate
                                     : TitlesNormalTemplate;
            }

            return null;
        }
    }
}