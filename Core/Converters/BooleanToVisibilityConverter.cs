using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Core
{
    /// <summary>
    /// The boolean to visibility converter
    /// </summary>
    /// <example>
    /// <Application.Resources>
    //    <app:BooleanToVisibilityConverter
    //        x:Key="BooleanToVisibilityConverter" 
    //        True="Collapsed" 
    //        False="Visible" />
    //</Application.Resources>
    /// </example>
    public sealed class BooleanToVisibilityConverter : BooleanConverter<Visibility>
    {
        public BooleanToVisibilityConverter() :
            base(Visibility.Visible, Visibility.Collapsed)
        { }
    }
}
