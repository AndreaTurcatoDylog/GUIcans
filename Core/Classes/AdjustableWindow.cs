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
    /// This window manage the Keyboard
    /// </summary>
    public class AdjustableWindow : DisposableBaseWindow, IAdjustableFrameworkElement
    {
        #region Fields

        /// <summary>
        /// The root element name of the window.
        /// The element will be moved up if it is necessary when the keyboard will be shown
        /// </summary>
        private string _RootElementName;

        #endregion

        #region Properties

        /// <summary>
        /// The root element of the windows
        /// </summary>
        public FrameworkElement RootElement { get; protected set; }

        #endregion

        #region Constructors

        public AdjustableWindow() :
          this(string.Empty)
        { }

        public AdjustableWindow(string rootElementName)
            : base()
        {
            _RootElementName = rootElementName;
        }

        #endregion

        #region Methods

        /// <summary>
        /// OCcurs in OnLoaded event
        /// </summary>
        protected override void Load()
        {
            RootElement = this.FindName(_RootElementName) as FrameworkElement;
        }

        /// <summary>
        /// Move the Root element up in new position
        /// </summary>
        public void MoveUp(double newTopValue)
        {
            if (RootElement != null)
            {
                TranslateTransform translateTransform = new TranslateTransform(0, newTopValue);
                RootElement.RenderTransform = new TranslateTransform(0, newTopValue);
            }
        }

        /// <summary>
        /// Restire the Root element in the original position
        /// </summary>
        public void RestorePosition()
        {
            if (RootElement != null)
            {
                RootElement.RenderTransform = null;
            }
        }

        #endregion

        #region Events

        #endregion
    }
}
