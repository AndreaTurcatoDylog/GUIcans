using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Core
{
    ///// <summary>
    ///// This converter is used to make Visible\Collapsed some components when the Favorite Folders
    ///// are choosen
    ///// </summary>
    /// <example>
    /// <Application.Resources>
    //    <app:SelectedRootToVisiblityConverter
    //        x:Key="SelectedRootToVisiblityConverter" 
    //        True="Visible" 
    //        False="Collapsed" />
    //</Application.Resources>
    /// </example>
    public sealed class SelectedRootToVisiblityConverter : SelectedRootConverter<Visibility>
    {
        public SelectedRootToVisiblityConverter() :
            base(Visibility.Visible, Visibility.Collapsed)
        { }
    }
}
