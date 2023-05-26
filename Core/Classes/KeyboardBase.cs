using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Core
{
    public abstract class KeyboardBase: DisposableBaseWindow
    {
        #region Event Handler

        public event EventHandler KeyPressed;

        #endregion

        #region Properties

        public Dictionary<string, string> CurrentDictionary
        {
            get { return (Dictionary<string, string>)GetValue(CurrentDictionaryProperty); }
            protected set { SetValue(CurrentDictionaryProperty, value); }
        }

        #endregion

        #region Dependency Properties

        private static readonly DependencyProperty CurrentDictionaryProperty =
         DependencyProperty.Register("CurrentDictionary", typeof(Dictionary<string, string>), typeof(StandardKeyboard));

        #endregion

        #region Constructor

        public KeyboardBase()
        {
            Loaded += OnLoaded;
        }

        public KeyboardBase(KeyboardLayout layout)
            :this()
        {
            LoadLayout(layout);
        }

        #endregion

        #region Abstract Methods

        public abstract bool LoadLayout(KeyboardLayout keyboardLayout);

        #endregion

        #region Method

        /// <summary>
        /// Register\Unregister events
        /// </summary>
        protected override void RegisterEvents(bool register) {
            if (!register)
            {
                Loaded -= OnLoaded;
            }
        }

        /// <summary>
        /// Raise the on key pressed event
        /// </summary>
        protected void OnKeypressed(EventArgs e)
        {
            KeyPressed?.Invoke(this, e);
        }

        /// <summary>
        /// Set the kayboard position
        /// </summary>
        protected virtual void SetKeyboardPosition()
        {}

        /// <summary>
        /// Hide the keyboard wating 100 ms
        /// </summary>
        protected void AsyncHide()
        {
            SetKeyboardPosition();
            this.Hide();
        }

        /// <summary>
        /// Close the Keyboard
        /// </summary>
        public void CloseKeyboard()
        {
            AsyncHide();
        }

        /// <summary>
        /// Set position of Keyboard on the screen
        /// </summary>
        public void SetPosition(int left, int top)
        {
            Left = left;
            Top = top;
        }

        public void SetPositionOnBottom()
        {
            // Get center of desktop working area
            var desktopWorkingArea = SystemParameters.WorkArea;
            var desktopWorkingAreaCenter = desktopWorkingArea.Width / 2;

            // Get center of keyboard
            var keyboardCenter = this.Width / 2;

            // Place Keyboard on the center and bottom
            Left = (desktopWorkingAreaCenter - keyboardCenter);
            Top = desktopWorkingArea.Bottom - this.Height - 5;
        }

        #endregion

        #region Events

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            SetKeyboardPosition();
        }

        #endregion
    }
}
