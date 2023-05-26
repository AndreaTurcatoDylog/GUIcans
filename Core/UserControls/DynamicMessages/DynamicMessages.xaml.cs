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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Core
{
    /// <summary>
    /// Interaction logic for DynamicMessages.xaml
    /// </summary>
    public partial class DynamicMessages : UserControl
    {
        #region Constants

        private const double ImageZoomInValue = 0.7;
        private const double ImageZoomOutValue = 1;
        private const double DifferentCoordinatesPosition = 70;

        #endregion

        #region Event Handler

        public event EventHandler MessageDeleted;

        #endregion

        #region Fields

        // Prevent to delete an item if the mouse (touch) is pressed to expand it.
        // If an user expand a dynamic message and continue to press with mouse (touch) and release when the message is expanded,
        // so the message will be deleted becouse the begin of touch and the end of touch coordinates will trigger the delete procedure.
        // This variable prevent this bad scenario
        private bool _JustExpanded;

        // It is the index of deleted item
        private int _DeletedItemIndex;

        private ListViewItem _CurrentMessageItemMouseDown;

        // The point coordinate when mouse (touch) is pressed
        private Point _BeginPoint;

        // The  point coordinate when mouse (touch) is released
        private Point _EndPoint;

        #endregion

        #region Properties

        /// <summary>
        /// Get\Set the IsMessageDeleted.
        /// Specify whether the message is deleted or not
        /// </summary>
        public bool IsMessageDeleted
        {
            get { return (bool)GetValue(IsMessageDeletedProperty); }
            set { SetValue(IsMessageDeletedProperty, value); }
        }

        /// <summary>
        /// Get\Set The image zoom value. 
        /// Used in animation:
        /// Panel expanded: zoom out (large image)
        /// Panel collapsed: zoom in (small image)
        /// </summary>
        public double ImageZoomValue
        {
            get { return (double)GetValue(ImageZoomValueProperty); }
            set { SetValue(ImageZoomValueProperty, value); }
        }

        /// <summary>
        /// Get\Set IsMessagesExpanded
        /// Indicates whether the messages are expanded or not
        /// </summary>
        public bool IsMessagesExpanded
        {
            get { return (bool)GetValue(IsMessagesExpandedProperty); }
            set { SetValue(IsMessagesExpandedProperty, value); }
        }

        /// <summary>
        /// Get\Set the Item Source
        /// </summary>
        public ObservableCollection<IMessageItem> Messages
        {
            get { return (ObservableCollection<IMessageItem>)GetValue(MessagesProperty); }
            set { SetValue(MessagesProperty, value); }
        }

        /// <summary>
        /// Get\Set Slide Panel Width. It is the width of panel
        /// </summary>
        public double SlidePanelWidth
        {
            get { return (double)GetValue(SlidePanelWidthProperty); }
            set { SetValue(SlidePanelWidthProperty, value); }
        }

        /// <summary>
        /// Get\Set the BookmarkWidth
        /// When the panel minimize this property rappresents the value of visible bookmark.
        /// </summary>
        public double BookmarkWidth
        {
            get { return (double)GetValue(BookmarkWidthProperty); }
            set { SetValue(BookmarkWidthProperty, value); }
        }

        #endregion

        #region Dependency Properties

        private static readonly DependencyProperty IsMessageDeletedProperty =
        DependencyProperty.Register("IsMessageDeleted", typeof(bool), typeof(DynamicMessages));

        private static readonly DependencyProperty ImageZoomValueProperty =
        DependencyProperty.Register("ImageZoomValue", typeof(double), typeof(DynamicMessages));

        private static readonly DependencyProperty IsMessagesExpandedProperty =
         DependencyProperty.Register("IsMessagesExpanded", typeof(bool), typeof(DynamicMessages), new PropertyMetadata(OnIsMessagesExpandedPropertyChangedCallBack));

        private static readonly DependencyProperty MessagesProperty =
         DependencyProperty.Register("Messages", typeof(ObservableCollection<IMessageItem>), typeof(DynamicMessages));

        private static readonly DependencyProperty SlidePanelWidthProperty =
          DependencyProperty.Register("SlidePanelWidth", typeof(double), typeof(DynamicMessages));

        private static readonly DependencyProperty BookmarkWidthProperty =
         DependencyProperty.Register("BookmarkWidth", typeof(double), typeof(DynamicMessages));

        #endregion

        #region Constructor

        public DynamicMessages()
        {
            InitializeComponent();

            // Set properties
            ImageZoomValue = ImageZoomInValue;
            IsMessagesExpanded = false;
            ImageZoomValue = ImageZoomOutValue;

            // Set DataContext
            LayoutRoot.DataContext = this;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called when the IsMessagesExpanded property changes
        /// </summary>
        private static void OnIsMessagesExpandedPropertyChangedCallBack(
        DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            DynamicMessages c = sender as DynamicMessages;
            if (c != null)
            {
                c.PerformSlider(c.IsMessagesExpanded);
            }
        }

        /// <summary>
        /// Expand\Collapse the Panel
        /// </summary>
        private void PerformSlider(bool expand)
        {
            if (expand)
            {
                ImageZoomValue = ImageZoomOutValue;
                ((Storyboard)this.Resources["expandStoryBoard"]).Begin(this);
            }
            else
            {
                ImageZoomValue = ImageZoomInValue;
                ((Storyboard)this.Resources["collapseStoryBoard"]).Begin(this);
            }
        }

        #endregion

        /// <summary>
        /// Make visible\collaps the Slider Panel
        /// </summary>
        private void OnTabButtonMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Handle single leftbutton mouse clicks
            if (e.ClickCount < 2 && e.LeftButton == MouseButtonState.Pressed)
            {
                IsMessagesExpanded = !IsMessagesExpanded;

                // Prevent to delete an item if the mouse (touch) is pressed to expand it.
                // If an user expand a dynamic message and continue to press with mouse (touch) and release when the message is expanded,
                // so the message will be deleted becouse the begin of touch and the end of touch coordinates will trigger the delete procedure.
                // This variable prevent this bad scenario.
                _JustExpanded = IsMessagesExpanded;
            }
        }

        #region Events

        /// <summary>
        /// Occurs when the delete animation terminates.
        /// The method will delete the current dynamic message from the list
        /// </summary>
        private void OnDeleteItemAnimationCompleted(object sender, EventArgs e)
        {
            IsMessageDeleted = false;
            if (_DeletedItemIndex > -1)
            {
                Messages.RemoveAt(_DeletedItemIndex);

                // Fire the event
                MessageDeleted?.Invoke(this, e);

                if (_CurrentMessageItemMouseDown != null)
                {
                    _CurrentMessageItemMouseDown.IsSelected = false;
                    _CurrentMessageItemMouseDown = null;
                }

                _DeletedItemIndex = -1;
            }
        }

        /// <summary>
        /// Occurs when the button mouse is down on a Dynamic Message.
        /// It is store the begin position
        /// </summary>
        private void OnListViewItemMouseDown(object sender, EventArgs e)
        {
            _CurrentMessageItemMouseDown = (ListViewItem)sender;
            _BeginPoint = Mouse.GetPosition(_CurrentMessageItemMouseDown);
            _CurrentMessageItemMouseDown.IsSelected = false;
        }

        /// <summary>
        /// Occurs when the button mouse is up on a Dynamic Message.
        /// It is store the end position
        /// </summary>
        private void OnListViewItemMouseUp(object sender, EventArgs e)
        {
            if (!_JustExpanded)
            {
                var obj = (ListViewItem)sender;
                if (obj == _CurrentMessageItemMouseDown)
                {
                    _EndPoint = Mouse.GetPosition((ListViewItem)sender);
                    var difference = _BeginPoint.X - _EndPoint.X;
                    if (difference > DifferentCoordinatesPosition)
                    {
                        IsMessageDeleted = true;
                        _DeletedItemIndex = propertiesPanel.SelectedIndex;
                        var selectedItems = propertiesPanel.SelectedItems;
                    }
                    else
                    {
                        IsMessageDeleted = false;
                        obj.IsSelected = false;
                        _CurrentMessageItemMouseDown = null;
                    }
                }
                else
                {
                    IsMessageDeleted = false;
                    obj.IsSelected = false;
                    _CurrentMessageItemMouseDown = null;
                }
            }

            _JustExpanded = false;
        }

        /// <summary>
        /// Occurs when the touch is down on a Dynamic Message.
        /// It is store the begin position
        /// </summary>
        private void OnListViewItemTouchDown(object sender, TouchEventArgs e)
        {
            _CurrentMessageItemMouseDown = (ListViewItem)sender;
            _BeginPoint = e.GetTouchPoint(sender as IInputElement).Position;
            _CurrentMessageItemMouseDown.IsSelected = false;
        }

        /// <summary>
        /// Occurs when the touch is up on a Dynamic Message.
        /// It is store the end position
        /// </summary>
        private void OnListViewItemTouchUp(object sender, TouchEventArgs e)
        {
            if (!_JustExpanded)
            {
                var obj = (ListViewItem)sender;
                if (obj == _CurrentMessageItemMouseDown)
                {
                    _EndPoint = e.GetTouchPoint(sender as IInputElement).Position;
                    var difference = _BeginPoint.X - _EndPoint.X;
                    if (difference > DifferentCoordinatesPosition)
                    {
                        IsMessageDeleted = true;
                        obj.IsSelected = true;
                        _DeletedItemIndex = propertiesPanel.SelectedIndex;

                        var selectedItems = propertiesPanel.SelectedItems;
                    }
                    else
                    {
                        IsMessageDeleted = false;
                        obj.IsSelected = false;
                        _CurrentMessageItemMouseDown = null;
                    }
                }
                else
                {
                    IsMessageDeleted = false;
                    obj.IsSelected = false;
                    _CurrentMessageItemMouseDown = null;
                }
            }

            _JustExpanded = false;
        }

        #endregion

    }
}
