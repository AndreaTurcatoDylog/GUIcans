using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Core
{
    public class AttachedProperties : DependencyObject
    {

        #region Buttons

        /// <summary>
        /// ButtonPath
        /// </summary>
        public static readonly DependencyProperty ButtonPathProperty = DependencyProperty.RegisterAttached("ButtonPath", typeof(Geometry), typeof(AttachedProperties));
        public static void SetButtonPath(UIElement element, Geometry value)
        {
            element.SetValue(ButtonPathProperty, value);
        }
        public static Geometry GetButtonPath(UIElement element)
        {
            return (Geometry)element.GetValue(ButtonPathProperty);
        }

        /// <summary>
        /// ButtonContentMargin 
        /// </summary>
        public static readonly DependencyProperty ButtonContentMarginProperty = DependencyProperty.RegisterAttached(
            "ButtonContentMargin",
            typeof(string),
            typeof(AttachedProperties));
            //,new UIPropertyMetadata(OnMarginRightPropertyChanged));

        public static string GetButtonContentMargin(FrameworkElement element)
        {
            return (string)element.GetValue(ButtonContentMarginProperty);
        }

        public static void SetButtonContentMargin(FrameworkElement element, string value)
        {
            element.SetValue(ButtonContentMarginProperty, value);
        }

        #endregion       
    }
}
