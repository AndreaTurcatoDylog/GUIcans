using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Core
{
    public class SettingsDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var frameworkElement = container as FrameworkElement;
            var settingItem = item as ISettingItem;

            if (settingItem != null)
            {
                switch (settingItem.Type)
                {
                    case (int)Setting.EMPTY: return frameworkElement.FindResource("EmptyDataTemplate") as DataTemplate;
                    case (int)Setting.TEXT: return frameworkElement.FindResource("TextDataTemplate") as DataTemplate;
                    case (int)Setting.BOOLEAN: return frameworkElement.FindResource("BooleanDataTemplate") as DataTemplate;
                    case (int)Setting.NUMERIC: return frameworkElement.FindResource("NumericDataTemplate") as DataTemplate;
                    case (int)Setting.CHECKED: return frameworkElement.FindResource("RadioDataTemplate") as DataTemplate;
                    case (int)Setting.BIT: return frameworkElement.FindResource("BitDataTemplate") as DataTemplate;
                    case (int)Setting.LINK_UP: return frameworkElement.FindResource("LinkUpDataTemplate") as DataTemplate;
                    case (int)Setting.LINK_WITH_VALUE: return frameworkElement.FindResource("LinkWithValueDataTemplate") as DataTemplate;
                    case (int)Setting.LINK_WITH_BIT_RESULT_VALUE: return frameworkElement.FindResource("LinkWithBitResultValueDataTemplate") as DataTemplate;
                    case (int)Setting.LINK_WITH_SINGLE_RESULT_VALUE: return frameworkElement.FindResource("LinkWithSingleChoiseResultValueDataTemplate") as DataTemplate;

                    case (int)Setting.LINK:
                    case (int)Setting.LINK_WITH_COMMON_SINGLE_RESULT_VALUE:
                        return frameworkElement.FindResource("LinkDataTemplate") as DataTemplate;
                }
            }
            
            return null;
        }
    }
}
