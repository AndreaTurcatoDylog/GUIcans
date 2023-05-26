using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Core
{
    /// <summary>
    /// This custom control create a TabItem hosted in a TabControl with an X button
    /// to close the TabItem
    /// </summary>
    public class TabItemClosable : TabItem
    {

        #region Properties

        /// <summary>
        /// Get\Set the Can Close Button.
        /// Specify whehter the tab item can be closed
        /// </summary>
        public bool CanClose
        {
            get { return (bool)this.GetValue(CanCloseProperty); }
            set
            {
                this.SetValue(CanCloseProperty, value);
            }
        }

        #endregion

        #region Dependency Properties

        private static readonly DependencyProperty CanCloseProperty =
          DependencyProperty.Register("CanClose", typeof(bool), typeof(TabItemClosable));

        #endregion

        #region Commands

        /// <summary>
        /// This Command close the TabItem
        /// </summary>
        private ICommand _CloseTabItemCommand;
        public ICommand CloseTabItemCommand
        {
            get { return _CloseTabItemCommand; }
            set { _CloseTabItemCommand = value; }
        }

        #endregion

        #region Routed Event Handler

        /// <summary>
        /// Expose and raise the OnClosedEvent event
        /// </summary>
        public event RoutedEventHandler OnClosed
        {
            add { AddHandler(OnClosedEvent, value); }
            remove { RemoveHandler(OnClosedEvent, value); }
        }

        #endregion

        #region Routed Events

        public static readonly RoutedEvent OnClosedEvent =
                        EventManager.RegisterRoutedEvent("OnClosed", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(TabItemClosable));

        #endregion

        #region Constructor

        public TabItemClosable()
            : base()
        {
            // Create the Commands
            CloseTabItemCommand = new RelayCommand(OnClosedExecute);
        }


        #endregion

        #region Methods

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the TabItem is closed
        /// </summary>
        private void OnClosedExecute(object param)
        {
            RaiseEvent(new RoutedEventArgs(OnClosedEvent));
        }

        #endregion
    }
}
