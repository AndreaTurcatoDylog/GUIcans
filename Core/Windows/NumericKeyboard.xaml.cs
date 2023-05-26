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

namespace Core
{
    /// <summary>
    /// Interaction logic for Numeric.xaml
    /// </summary>
    public partial class NumericKeyboard : KeyboardBase
    {
        #region Fields

        private static NumericKeyboard _Instance;

        private Dictionary<string, string> _NumericCharacters;

        #endregion

        #region Properties

        /// <summary>
        /// Singleton
        /// </summary>
        public static NumericKeyboard Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new NumericKeyboard();
                }

                return _Instance;
            }
        }

        #endregion

        #region Dependency Properties
        #endregion

        #region Constructor

        public NumericKeyboard()
        {
            InitializeComponent();

            // Create the dictionary
            _NumericCharacters = new Dictionary<string, string>();

            _Instance = this;

            // Set the data context
            LayoutRoot.DataContext = this;
        }

        public NumericKeyboard(KeyboardLayout layout)
            : base(layout)
        {
            InitializeComponent();

            _Instance = this;

            // Set the data context
            LayoutRoot.DataContext = this;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load the layout
        /// </summary>
        public override bool LoadLayout(KeyboardLayout keyboardLayout)
        {
            if (keyboardLayout != null)
            {
                _NumericCharacters = keyboardLayout.NumericCharacters;
                CurrentDictionary = _NumericCharacters;

                return true;
            }

            return false;
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the Window is loaded.
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            CurrentDictionary = _NumericCharacters;
        }

        /// <summary>
        /// Occurs when a normal button is clicked
        /// </summary>
        private void OnNormalButtonTouchDown(object sender, TouchEventArgs e)
        {
            var content = (sender as Button).Content?.ToString();
            OnKeypressed(new KeyboardButtonEventArgs(KeyboardButtonType.Normal, content));

            e.Handled = true;
        }

        /// <summary>
        /// Occurs when a normal button is clicked
        /// </summary>
        private void OnNormalButtonClick(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice != null)
            {
                e.Handled = true;
            }
            else
            {
                var content = (sender as Button).Content?.ToString();
                OnKeypressed(new KeyboardButtonEventArgs(KeyboardButtonType.Normal, content));
                e.Handled = true;
            }
        }

        /// <summary>
        /// The enter button close the Keybord
        /// </summary>
        private void OnEnterTouchDown(object sender, TouchEventArgs e)
        {
            OnKeypressed(new KeyboardButtonEventArgs(KeyboardButtonType.Enter));
            AsyncHide();

            e.Handled = true;
        }

        /// <summary>
        /// The enter button close the Keybord
        /// </summary>
        private void OnEnterButtonClick(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice != null)
            {
                e.Handled = true;
            }
            else
            {
                if (sender is Button button)
                {
                    // Fire the Preview Mouse down event
                    MouseButtonEventArgs arg = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left);
                    arg.RoutedEvent = Button.PreviewMouseUpEvent;
                    // _CurrentPressedButton.RaiseEvent(arg);

                    button.RaiseEvent(arg);
                    OnKeypressed(new KeyboardButtonEventArgs(KeyboardButtonType.Enter));
                    AsyncHide();

                    // e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Disable the propagate of the muouse up event
        /// </summary>
        private void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {

            e.Handled = true;
        }

        /// <summary>
        /// Occurs when the backspace button is clicked
        /// </summary>
        private void OnBackspaceButtonTouchDown(object sender, TouchEventArgs e)
        {
            OnKeypressed(new KeyboardButtonEventArgs(KeyboardButtonType.Backspace));
            e.Handled = true;
        }

        /// <summary>
        /// Occurs when the backspace button is clicked
        /// </summary>
        private void OnBackspaceButtonClick(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice != null)
            {
                e.Handled = true;
            }
            else
            {
                OnKeypressed(new KeyboardButtonEventArgs(KeyboardButtonType.Backspace));
                e.Handled = true;
            }
        }

        /// <summary>
        /// Occurs when the decimal separator button is clicked
        /// </summary>
        private void OnDecimalSeparatorButtonTouchDown(object sender, TouchEventArgs e)
        {
            var content = (sender as Button).Content?.ToString();
            OnKeypressed(new KeyboardButtonEventArgs(KeyboardButtonType.DecimalSeparator, content));
            e.Handled = true;
        }

        /// <summary>
        /// Occurs when the decimal separator button is clicked
        /// </summary>
        private void OnDecimalSeparatorButtonClick(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice != null)
            {
                e.Handled = true;
            }
            else
            {
                var content = (sender as Button).Content?.ToString();
                OnKeypressed(new KeyboardButtonEventArgs(KeyboardButtonType.DecimalSeparator, content));
                e.Handled = true;
            }
        }

        /// <summary>
        /// Occurs when the exit button is clicked
        /// </summary>
        private void OnExitTouchDown(object sender, TouchEventArgs e)
        {
            OnKeypressed(new KeyboardButtonEventArgs(KeyboardButtonType.Exit));
            AsyncHide();
            e.Handled = true;
        }

        /// <summary>
        /// Occurs when the exit button is clicked
        /// </summary>
        private void OnExitButtonClick(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice != null)
            {
                e.Handled = true;
            }
            else
            {
                OnKeypressed(new KeyboardButtonEventArgs(KeyboardButtonType.Exit));
                AsyncHide();
                e.Handled = true;
            }
        }



        #endregion

        private void KeyboardBase_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
    }
}
