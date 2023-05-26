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

namespace Core
{
    /// <summary>
    /// Interaction logic for FiltersUsDateTimeFiltersUserControlerControl.xaml
    /// Peforms the UI filter for the range [FROM, TO]
    /// </summary>
    public partial class DateTimeFiltersUserControl : DisposableUserControlBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the From DateTime
        /// </summary>
        public DateTime? From
        {
            get { return (DateTime?)GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }

        /// <summary>
        /// Gets or sets the To DateTime
        /// </summary>
        public DateTime? To
        {
            get { return (DateTime?)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Is Filter Enabled
        /// </summary>
        public bool AreFiltersEnabled
        {
            get { return (bool)GetValue(AreFiltersEnabledProperty); }
            set { SetValue(AreFiltersEnabledProperty, value); }
        }

        /// <summary>
        /// Get\Set the Filter Changed Command
        /// </summary>
        public ICommand FilterChangedCommand
        {
            get { return (ICommand)GetValue(FilterChangedCommandProperty); }
            set { SetValue(FilterChangedCommandProperty, value); }
        }

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty FromProperty =
           DependencyProperty.Register("From", typeof(DateTime?), typeof(DateTimeFiltersUserControl), new UIPropertyMetadata(null));

        public static readonly DependencyProperty ToProperty =
           DependencyProperty.Register("To", typeof(DateTime?), typeof(DateTimeFiltersUserControl), new UIPropertyMetadata(null));

        public static DependencyProperty FilterChangedCommandProperty =
           DependencyProperty.Register("FilterChangedCommand", typeof(ICommand), typeof(DateTimeFiltersUserControl));

        public static DependencyProperty AreFiltersEnabledProperty =
           DependencyProperty.Register("AreFiltersEnabled", typeof(bool), typeof(DateTimeFiltersUserControl));

        #endregion

        #region Constructor

        public DateTimeFiltersUserControl()
        {
            InitializeComponent();

            // Set the data contex
            LayoutRoot.DataContext = this;
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the FromDatePicker changes its date
        /// </summary>
        private void OnFromDatePickerSelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DatePicker)
            {
                if (fromDatePicker?.SelectedDate > toDatePicker?.SelectedDate)
                {
                    fromDatePicker.SelectedDate = null;
                }
            }
        }

        /// <summary>
        /// Occurs when the ToDatePicker changes its date
        /// </summary>
        private void OnToDatePickerSelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DatePicker)
            {
                if (toDatePicker?.SelectedDate < fromDatePicker?.SelectedDate)
                {
                    toDatePicker.SelectedDate = null;
                }
            }
        }

        /// <summary>
        /// Occurs when the Enter is pressed on the DatePicker
        /// </summary>
        protected void OnDatePickerPreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                FilterChangedCommand.Execute(null);
            }
        }

        #endregion  
    }
}
