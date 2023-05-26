using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Core
{
    public abstract class DisposableUserControl : UserControl, IDisposable
    {
        #region Fields

        protected Window _ParentWindow;

        #endregion

        #region Constructor

        public DisposableUserControl()
            : base()
        {
            Loaded += OnLoaded;    
        }

        #endregion

        #region Virtual

        protected virtual void RegisterEvents(bool register) { }

        protected virtual void Load() { }

        #endregion

        #region Methods

        /// <summary>
        /// Register\Unregister all events
        /// </summary>
        private void RegisterBaseEvents(bool register)
        {
            if (register)
            {
                if (_ParentWindow != null)
                {
                    _ParentWindow.Closing += ParentWindowClosing;
                }
            }
            else
            {
                Loaded -= OnLoaded;

                if (_ParentWindow != null)
                {
                    _ParentWindow.Closing -= ParentWindowClosing;
                }
            }

            RegisterEvents(register);
        }

        private void ParentWindowClosing(object sender, CancelEventArgs e)
        {}

        #endregion

        #region Events

        /// <summary>
        /// The load event register all events
        /// </summary>
        protected void OnLoaded(object sender, RoutedEventArgs e)
        {
            _ParentWindow = Window.GetWindow(this);

            RegisterBaseEvents(true);

            // Execute some actions after the use control is loaded
            Load();
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Dispose the object
        /// </summary>
        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);

            RegisterBaseEvents(false);
            DisposeObjects();
        }

        /// <summary>
        /// Dispose the objects in user control
        /// </summary>
        protected virtual void DisposeObjects() { }

        #endregion
    }
}
