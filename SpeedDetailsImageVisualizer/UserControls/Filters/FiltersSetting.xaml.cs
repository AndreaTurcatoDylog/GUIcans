using Business;
using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SpeedDetailsImageVisualizer
{
    /// <summary>
    /// Interaction logic for FiltersSetting.xaml
    /// </summary>
    public partial class FiltersSetting : DisposableUserControl
    {
        #region Properties        

        /// <summary>
        /// Get\Set the Filters
        /// </summary>
        public Filters Filters
        {
            get { return (Filters)GetValue(FiltersProperty); }
            set { SetValue(FiltersProperty, value); }
        }

        #endregion

        #region Dependency Properties

        private static readonly DependencyProperty FiltersProperty =
            DependencyProperty.Register("Filters", typeof(Filters), typeof(FiltersSetting), new PropertyMetadata(null));

        #endregion

        #region Constructor

        public FiltersSetting()
        {
            InitializeComponent();
        }

        #endregion
    }
}
