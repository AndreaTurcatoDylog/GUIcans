using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Core
{
    /// <summary>
    /// This class rappresent a Disposable User Control 
    /// It is used in Navigation Mechanism where a single User control is loaded in a Main Window at one time; when navigates in another page
    /// so another User Control is loaded, but the previus one is not disposed becouse the Main Window is closed only when 
    /// the Application is shouted down. In this bad scenario  only the current User Control is disposed.
    /// This class is created to use an User Control in Navigation Mechanism and it will disposed when it is unloaded.
    /// It will dispose all its internal disposable components.
    /// </summary>
    public class NavigationDisposableUserControl : UserControl, IDisposable
    {
        #region Constructor

        public NavigationDisposableUserControl()
            : base()
        {
            RegisterEvents(true);
        }

        #endregion

        #region Methods

        protected virtual void Load() { }

        protected virtual void Closing() { }

        /// <summary>
        /// Register\Unregister events
        /// </summary>
        private void RegisterEvents(bool register)
        {
            if (register)
            {
                Loaded += OnLoaded;
                Unloaded += OnUnloaded;
            }
            else
            {
                Unloaded -= OnUnloaded;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs on load
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Load();
        }

        /// <summary>
        /// Occurs when the User control is unloaded
        /// </summary>
        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Dispose();
        }

        #endregion

        #region IDisposableImplementation

        /// <summary>
        /// Dispose all the objects
        /// </summary>
        public void Dispose()
        {
            RegisterEvents(false);

            Closing();

            // Dispose all objects
            var disposableElements = this.FindIDisposableVisualChildren(this);
            foreach (var element in disposableElements)
            {
                element.Dispose();
            }
        }

        #endregion
    }
}
