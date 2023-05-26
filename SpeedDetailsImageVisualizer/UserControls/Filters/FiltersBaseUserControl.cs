using Business;
using Core;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SpeedDetailsImageVisualizer
{
    // <summary>
    /// The base class for the Filters User Control
    /// </summary>
    public abstract class FiltersBaseUserControl : DisposableUserControl
    {       
        #region Events Handlers

        public event EventHandler IsFiltersVisibilityChanged;
        //public event EventHandler ChangePageLostFocus;
        //public event EventHandler<ChangePageEventArgs> CurrenPageChanged;

        #endregion       

        #region Methods        

        /// <summary>
        /// Trigger the IsFiltersVisibilityChanged Event
        /// </summary>
        protected void TriggerIsFiltersVisibilityChangedEvent(object sender, EventArgs eventArgs)
        {
            IsFiltersVisibilityChanged?.Invoke(sender, eventArgs);
        }

        ///// <summary>
        ///// Trigger the ChangePageLostFocus Event
        ///// </summary>
        //protected void TriggerChangePageLostFocusEvent(object sender, EventArgs eventArgs)
        //{
        //    ChangePageLostFocus?.Invoke(sender, eventArgs);
        //}

        ///// <summary>
        ///// Rais the CurrentPage Event
        ///// </summary>
        //protected void TriggerCurrenPageEvent(object sender, ChangePageEventArgs changePageEventArgs)
        //{
        //    CurrenPageChanged?.Invoke(sender, changePageEventArgs);
        //}

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the Visibility of the filters changes
        /// </summary>
        protected void OnFiltersVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            TriggerIsFiltersVisibilityChangedEvent(sender, EventArgs.Empty);
        }

        ///// <summary>
        ///// Occurs when the Change Page TextBox lost the focus
        ///// </summary>
        //protected void OnChangePageTextBoxLostFocus(object sender, RoutedEventArgs e)
        //{
        //    TriggerChangePageLostFocusEvent(sender, e);
        //}

        ///// <summary>
        ///// Occurs when a key is pressed on the ChangePageTextBox 
        ///// </summary>
        //protected void OnChangePageKeyDown(object sender, KeyEventArgs e)
        //{
        //    if (sender is TextBox changePageTextBox && e.Key == Key.Enter)
        //    {
        //        var converted = int.TryParse(changePageTextBox.Text, out int currentPage);
        //        if (converted)
        //        {                   
        //            // Trigger the CurrentPage Event
        //            TriggerCurrenPageEvent(sender, new ChangePageEventArgs(currentPage));
        //        }
        //    }
        //}

        #endregion        
    }
}
