using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace Core
{
    public class ToggleButtonActive : ToggleButton, IDisposable, IActivate
    {
        #region Events

        public event EventHandler Deactivated;

        #endregion

        #region Constructor

        public ToggleButtonActive()
            : base()
        {
            Loaded += OnLoaded;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Register\Unregister all events
        /// </summary>
        protected virtual void RegisterEvents(bool register)
        {
            if (register)
            {
                LostFocus += OnLostFocus;
            }
            else
            {

                Loaded -= OnLoaded;
                LostFocus -= OnLostFocus;
            }
        }

        /// <summary>
        /// Active the component
        /// </summary>
        public void Activate()
        {
        }

        /// <summary>
        /// Deactive component
        /// </summary>
        public void Deactivate()
        {
            Deactivated?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Events

        /// <summary>
        /// The loaded event
        /// </summary>
        private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            RegisterEvents(true);
        }

        /// <summary>
        /// Occurs when the control lost focus
        /// </summary>
        private void OnLostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            Deactivate();
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Dispose the object
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            RegisterEvents(false);
        }

        #endregion
    }
}
