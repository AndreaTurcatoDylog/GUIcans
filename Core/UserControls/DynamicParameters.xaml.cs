using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
    /// Interaction logic for DynamicParameters.xaml
    /// </summary>
    public partial class DynamicParameters : DisposableUserControl
    {
        #region Fields

        private IEnumerable _RootItemSource;

        private FrameworkElement _CurrentHiddenElement;
        private FrameworkElement _CurrentVisibleElement;

        private IActivate _CurrentActiveElement;

        #endregion

        #region Event Handler

        public event EventHandler ValueUpdated;

        #endregion

        #region Properties

        /// <summary>
        /// Get\Set the Item Source
        /// </summary>
        public IEnumerable ItemSource
        {
            get { return (IEnumerable)GetValue(ItemSourceProperty); }
            set { SetValue(ItemSourceProperty, value); }
        }

        /// <summary>
        /// Get\Set Border Thickness
        /// </summary>
        public int SettingBorderThickness
        {
            get { return (int)GetValue(SettingBorderThicknessProperty); }
            set { SetValue(SettingBorderThicknessProperty, value); }
        }

        /// <summary>
        /// Get\Set the border Background
        /// </summary>
        public Brush SettingBorderBackground
        {
            get { return (Brush)GetValue(SettingBorderBackgroundProperty); }
            set { SetValue(SettingBorderBackgroundProperty, value); }
        }

        #endregion

        #region Dependency Properties

        private static readonly DependencyProperty ItemSourceProperty =
          DependencyProperty.Register("ItemSource", typeof(IEnumerable), typeof(DynamicParameters), new PropertyMetadata(OnItemSourceChangedCallBack));

        private static readonly DependencyProperty SettingBorderThicknessProperty =
          DependencyProperty.Register("SettingBorderThickness", typeof(int), typeof(DynamicParameters), new PropertyMetadata(0));

        private static readonly DependencyProperty SettingBorderBackgroundProperty =
         DependencyProperty.Register("SettingBorderBackground", typeof(Brush), typeof(DynamicParameters), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        #endregion

        #region Constructor

        public DynamicParameters()
            : base()
        {
            InitializeComponent();

            LayoutRoot.DataContext = this;
        }

        #endregion

        #region Methods

        private static void OnItemSourceChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var c = sender as DynamicParameters;
            if (c != null)
            {}
        }

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
            }
        }

        /// <summary>
        /// Occurs in OnLoaded event in base class
        /// </summary>
        protected override void Load()
        {
            _RootItemSource = ItemSource;
        }

        /// <summary>
        /// Get the parent grid of the sender
        /// </summary>
        private Grid GetParentGrid(object sender)
        {
            Grid parentGrid = null;

            // Find the root
            FrameworkElement root = null;
            var frameworkElement = (sender as FrameworkElement);

            while (frameworkElement != null)
            {
                var parent = frameworkElement.Parent as FrameworkElement;
                if (parent != null)
                {
                    root = parent;
                }
                else
                {
                    root = frameworkElement;
                }

                frameworkElement = parent;
            }

            if (root != null)
            {
                // The top framework element is a border
                var topBorder = (root as Border).Child;
                parentGrid = topBorder as Grid;
            }

            return parentGrid;
        }

        /// <summary>
        /// Make visible\invisible the components when user choose an IParameter
        /// </summary>
        private void SetComponentsVisiblity(object sender)
        {
            // Get the parent grid if exists.
            // The parent grid has got two children Grid: ActiveElementGrid and TitleElementGrid
            var parentGrid = GetParentGrid(sender);
            if (parentGrid != null)
            {
                // Change visibility and store Visibile and Hidden Grid
                foreach (var child in parentGrid.Children)
                {
                    var frameworkElement = (child as FrameworkElement);
                    if (!(frameworkElement is ToggleButtonActive))
                    {
                        if (frameworkElement.IsVisible)
                        {
                            frameworkElement.Visibility = Visibility.Hidden;
                            _CurrentHiddenElement = frameworkElement;
                        }
                        else
                        {
                            frameworkElement.Visibility = Visibility.Visible;
                            _CurrentVisibleElement = frameworkElement;
                        }
                    }
                }

                // Get the current active element if exists
                var activeElement = GetActiveElement(_CurrentVisibleElement);
                if (activeElement != null)
                {
                    if (_CurrentActiveElement != activeElement)
                    {
                        // Deactivate the previus element if exists
                        _CurrentActiveElement?.Deactivate();

                        // Active the current element if exists
                        _CurrentActiveElement = activeElement;
                        activeElement.Activate();
                        _CurrentActiveElement.Deactivated += OnCurrentActiveElementDeactivated;
                    }
                }
                else
                {
                    _CurrentActiveElement?.Deactivate();
                    _CurrentActiveElement = activeElement;
                }
            }
        }

        /// <summary>
        /// Occurs when the active element is deactive
        /// </summary>
        private void OnCurrentActiveElementDeactivated(object sender, EventArgs e)
        {
            // Stop to listen to deactivated element
            _CurrentActiveElement.Deactivated -= OnCurrentActiveElementDeactivated;

            // Get the previus grid elements
            var previusParentGrid = GetParentGrid(_CurrentActiveElement);
            var activeElementGrid = previusParentGrid.Children[0] as Grid;
            var titleElementGrid = previusParentGrid.Children[1] as Grid;

            // Change visibility of grids
            activeElementGrid.Visibility = Visibility.Hidden;
            titleElementGrid.Visibility = Visibility.Visible;

            _CurrentActiveElement = null;
        }

        /// <summary>
        /// Returns the Active element if exists
        /// </summary>
        private IActivate GetActiveElement(FrameworkElement frameworkElement)
        {
            IActivate result = null;

            if (frameworkElement != null)
            {
                var grid = (frameworkElement as Grid);
                if (grid.Children.Count > 0)
                {
                    var child = grid.Children[0];
                    if (child is Border)
                    {
                        var border = (child as Border);
                        if (border.Child is IActivate)
                        {
                            return border.Child as IActivate;
                        }
                    }
                }
            }

            return result;
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

                if (dataContext != null && dataContext is ISettingItem)
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

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the user click on textBlock that contains label or on the border element.
        /// It gets the element to make visible
        /// </summary>
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ((sender as FrameworkElement).IsEnabled)
            {
                SetComponentsVisiblity(sender);
            }

            e.Handled = true;
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
            if (e.ExtentHeightChange > 0 && _CurrentActiveElement != null && _CurrentActiveElement is IHasPopUp)
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
            if (_RootItemSource != null)
            {
                foreach (var item in _RootItemSource)
                {
                    (item as IParameter)?.Dispose();
                }
            }
        }

        #endregion
    }
}
