using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using System.Threading;
using System.Collections.Specialized;

namespace Core
{
    /// <summary>
    /// Interaction logic for DynamicSettings.xaml
    /// </summary>
    public partial class DynamicSettings : DisposableUserControl
    {
        #region Fields

        private ObservableCollection<ISettingItem> _RootItemSource;

        private FrameworkElement _CurrentVisibleElement;
        private FrameworkElement _CurrentInvisibleElement;

        private IActivate _CurrentActiveElement;

        #endregion

        #region Event Handler

        public event EventHandler LinkClicked;
        public event EventHandler ValueUpdated;

        #endregion

        #region Properties

        /// <summary>
        /// Get\Set the Item Source
        /// </summary>
        public ObservableCollection<ISettingItem> ItemSource
        {
            get {return (ObservableCollection<ISettingItem>)GetValue(ItemSourceProperty);}
            set {SetValue(ItemSourceProperty, value);}
        }

        #endregion

        #region Dependency Properties

        private static readonly DependencyProperty ItemSourceProperty =
          DependencyProperty.Register("ItemSource", typeof(ObservableCollection<ISettingItem>), typeof(DynamicSettings));

        #endregion

        #region Constructor

        public DynamicSettings()
            :base()
        {
            InitializeComponent();

            _CurrentVisibleElement = null;
            _CurrentInvisibleElement = null;
            LayoutRoot.DataContext = this;

            Loaded += OnLoaded;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Register\Unregister the events
        /// </summary>
        protected override void RegisterEvents(bool register)
        {
            if (register)
            {
               
            }
            else
            {
                Loaded -= OnLoaded;
            }
        }

        protected override void Load()
        {
            _RootItemSource = ItemSource;
        }

        /// <summary>
        /// Get the parent grid of the sender
        /// </summary>
        private Grid GetParentGrid(object sender)
        {
            // Get the parent grid
            Grid parentGrid = null;
            if (sender is Border)
            {
                var border = (sender as Border);
                parentGrid = border.Child as Grid;
            }
            else if (sender is TextBlock)
            {
                parentGrid = ((sender as TextBlock).Parent as Grid);
            }
            else if (sender is ToggleButton)
            {
                parentGrid = ((sender as ToggleButton).Parent as Grid);
            }

            return parentGrid;
        }

        /// <summary>
        /// Make visible\invisible the components when user choose an ISettingItem
        /// </summary>
        private void SetComponentsVisiblity(object sender)
        {
            bool isAlreadyOpen = false;

            // Get the parent grid
            var parentGrid = GetParentGrid(sender);
            if (parentGrid != null)
            {
                // Get the components that will become visibile and invisible
                var componentMustBeVisible = parentGrid.Children[1];
                var componentMustBeHidden = parentGrid.Children[2];

                isAlreadyOpen = (_CurrentActiveElement == componentMustBeVisible) && (_CurrentActiveElement != null);

                // Deactive the current active element if exists
                _CurrentActiveElement?.Deactivate();

                if (!isAlreadyOpen)
                {
                    // Change the visibility
                    componentMustBeVisible.Visibility = Visibility.Visible;
                    componentMustBeHidden.Visibility = Visibility.Hidden;

                    // Change visibility of current elements (current visible element and current invisible element)
                    if (_CurrentVisibleElement != null)
                    {
                        _CurrentVisibleElement.Visibility = Visibility.Hidden;
                    }

                    if (_CurrentInvisibleElement != null)
                    {
                        _CurrentInvisibleElement.Visibility = Visibility.Visible;
                    }

                    // Set the current elements (current visible element and current invisible element)
                    if (componentMustBeVisible != _CurrentVisibleElement)
                    {
                        var newVisibleComponent = componentMustBeVisible as FrameworkElement;
                        newVisibleComponent.Focus();

                        _CurrentVisibleElement = componentMustBeVisible as FrameworkElement;
                        _CurrentInvisibleElement = componentMustBeHidden as FrameworkElement;
                    }
                    else
                    {
                        _CurrentVisibleElement = null;
                        _CurrentInvisibleElement = null;
                    }

                    // Check if the current visible element is an IActivate component (TextBoxKeyaboardBase, ect...)
                    if (_CurrentVisibleElement is IActivate)
                    {
                        // Get and activate the new element
                        _CurrentActiveElement = _CurrentVisibleElement as IActivate;

                        // Listen for the Deactive event of current element 
                        _CurrentActiveElement.Deactivated += OnCurrentActiveElementDeactivated;

                        // Activate the current element
                        _CurrentActiveElement?.Activate();
                    }
                }
                else
                {
                    // Exchange the components
                    var temp = componentMustBeVisible;
                    componentMustBeVisible = componentMustBeHidden;
                    componentMustBeHidden = temp;

                    // Change the visibility
                    componentMustBeVisible.Visibility = Visibility.Visible;
                    componentMustBeHidden.Visibility = Visibility.Hidden;

                    // Refresh values
                    _CurrentVisibleElement = null;
                    _CurrentInvisibleElement = null;
                }
            }
        }

        /// <summary>
        /// Fire the Value updated event
        /// </summary>
        public void InvokeValueUpdatedEvent(object sender, EventArgs e)
        {
            if (sender is FrameworkElement)
            {
                var frameworkElement = sender as FrameworkElement;
                var dataContext = frameworkElement.DataContext;

                if (dataContext!=null && dataContext is ISettingItem)
                {
                    var settingItem = (ISettingItem)dataContext;
                    if (settingItem != null)
                    {
                        settingItem.OnIsValueChanged(sender, e);

                        if (settingItem.IsUpdateChanged)
                        {
                            ValueUpdated?.Invoke(this, new SettingUpdatedEventArgs(settingItem));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Scroll the list view to the checked in a choise container
        /// </summary>
        private void ScroolToChecked()
        {
            if (ItemSource != null)
            {
                // Get the checked element
                var checkedElement = ItemSource.Where(s => (s is SettingChecked)).SingleOrDefault(p => (p as SettingChecked).Checked);
                if (checkedElement != null)
                {
                    // Scrools the listView if needed
                    MainListView.SelectedItem = checkedElement;
                    MainListView.ScrollIntoView(MainListView.SelectedItem);
                }
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the user click on textBlock that contains label or on the border element.
        /// It gets the element to make visible
        /// </summary>
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetComponentsVisiblity(sender);
            e.Handled = true;
        }

        /// <summary>
        /// Occurs when a link is pressed
        /// </summary>
        private void OnLinkMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string key = string.Empty;
            ISettingItem settingItem = null;

            // Refresh visibility
            SetComponentsVisiblity(sender);

            // Get parent grid
            Grid parentGrid = null;
            if (sender is Border)
            {
                var border = (sender as Border);
                parentGrid = border.Child as Grid;
            }
            else if (sender is TextBlock)
            {
                parentGrid = ((sender as TextBlock).Parent as Grid);
            }

            if (parentGrid != null)
            {
                var textBlock = (parentGrid.Children[0] as TextBlock);
                settingItem = textBlock.DataContext as ISettingItem;
                key = settingItem.Label;
            }

            // Get the next SettingItems of choosen SettingLink (it is stored in Tag)
            var nextSettingItems = (List<ISettingItem>)(sender as FrameworkElement).Tag;
            if (nextSettingItems != null)
            {
                LinkClicked?.Invoke(this, new SettingLinkEventArgs(key, nextSettingItems, settingItem));

                // If page is a choise container must scrool the list view to checked element
                ScroolToChecked();
            }

            e.Handled = true;
        }

        /// <summary>
        /// Occurs whent the current active element ((TextBoxKeyaboardBase, ect...) is deactived
        /// </summary>
        private void OnCurrentActiveElementDeactivated(object sender, EventArgs e)
        {
            // Stop to listen to deactive event
            _CurrentActiveElement.Deactivated -= OnCurrentActiveElementDeactivated;

            // Reverse the visibility
            if (_CurrentInvisibleElement != null)
            {
                _CurrentInvisibleElement.Visibility = Visibility.Visible;
            }

            if (_CurrentVisibleElement != null)
            {
                _CurrentVisibleElement.Visibility = Visibility.Hidden;
            }

            //Refresh the elements
            _CurrentVisibleElement = null;
            _CurrentInvisibleElement = null;
            _CurrentActiveElement = null;
        }


        /// <summary>
        /// Occurs when the source of ISettingItems is updated
        /// </summary>
        private void OnSourceUpdated(object sender, DataTransferEventArgs e)
        {
            // Fire the events
            InvokeValueUpdatedEvent(sender, e);
        }

        /// <summary>
        /// Occurs when an item with boolean value changes is value
        /// </summary>
        private void OnBooleanValueChanged(object sender, RoutedEventArgs e)
        {
            // Fire the events
            InvokeValueUpdatedEvent(sender, e);

            e.Handled = true;
        }

        /// <summary>
        /// Occurs when a radio button is clicked
        /// </summary>
        private void OnSingleChoiceMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var parentGrid = GetParentGrid(sender);
            if (parentGrid != null)
            {
                // Get the Radio Button
                var radioButton = (RadioButton)parentGrid.Children[1];
                if (radioButton != null && !radioButton.IsChecked.Value && radioButton.IsEnabled)
                {
                    radioButton.IsChecked = true;

                    // Fire the events
                    InvokeValueUpdatedEvent(sender, e);
                }
            }
        }

        /// <summary>
        /// Prevent the auto scrool of scrool bar that could make problem with the component that use keyboard
        /// </summary>
        private void OnRequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            //e.Handled = true;
        }

        /// <summary>
        /// Reposition the pop up of the component that own one when scrolling occurs
        /// </summary>
        private void ListViewScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.ExtentHeightChange>0 && _CurrentActiveElement != null && _CurrentActiveElement is IHasPopUp)
            {
                (_CurrentActiveElement as IHasPopUp).ReloadPopUp();
            }
        }

        #endregion

        #region IDisposable  Implementation

        /// <summary>
        /// Dispose object in user control
        /// </summary>
        protected override void DisposeObjects()
        {
            base.DisposeObjects();
            foreach (var item in _RootItemSource)
            {
                item?.Dispose();
            }
        }

        #endregion
    }
}
