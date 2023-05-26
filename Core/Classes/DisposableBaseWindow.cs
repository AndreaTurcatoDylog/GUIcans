using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Core
{
    /// <summary>
    /// This class dispose all the IDisposable elements inside the window
    /// </summary>
    public abstract class  DisposableBaseWindow: Window, IDisposable
    {
        #region Properties
        #endregion

        #region Constructor

        public DisposableBaseWindow()
        {
            Loaded += OnLoaded;
        }

        #endregion

        #region Virtual

        protected virtual void Load() { }

        protected virtual void BeforeClose() { }

        protected virtual void RegisterEvents(bool register) { }

        #endregion

        #region Methods

        /// <summary>
        /// Register\Unregister all events
        /// </summary>
        private void RegisterBaseEvents(bool register)
        {
            if (register)
            {
                Closing += OnClosing;
            }
            else
            {
                Loaded -= OnLoaded;
                Closing -= OnClosing;
            }

            RegisterEvents(register);
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the window is loaded
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            RegisterBaseEvents(true);

            Load();
        }

        /// <summary>
        /// Occurs when the window is closing
        /// </summary>
        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            BeforeClose();

            Dispose();
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Dispose the object
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);

            // Unregister all events
            RegisterBaseEvents(false);

            // Dispose all the IDisposable element inside the window
            var disposableElements = this.FindIDisposableVisualChildren(this);
            foreach (var element in disposableElements)
            {
                element.Dispose();
            }
        }

        #endregion
    }
}
