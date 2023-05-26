using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DialogService
{
    class NumberOfImagesDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NumberOfImagesNormalTemplate { get; set; }
        public DataTemplate NumberOfImagesTouchTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var isTouch = item as bool?;

            if (isTouch.HasValue)
            {
                return isTouch.Value ? NumberOfImagesTouchTemplate
                                     : NumberOfImagesNormalTemplate;
            }

            return null;
        }
    }
}