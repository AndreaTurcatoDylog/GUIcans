using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Runtime.InteropServices;

namespace Core
{
    /// <summary>
    /// Interaction logic for StandardKeyboard.xaml.
    /// The Keyboard works with the Phisical Keyboard also
    /// </summary>
    public partial class StandardKeyboard : KeyboardBase
    {
        #region DLL Imports

        //[DllImport("user32.dll")]
        //static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        #endregion

        #region Members

        private static StandardKeyboard _Instance;

        private Dictionary<string, string> _LowerCaseLetter;
        private Dictionary<string, string> _UpperCaseLetter;
        private Dictionary<string, string> _SpecialCharacters;

        /// <summary>
        /// Specify the current pressed button. Used when a Key from phisical Keyboard is pressed.
        /// The Pressed Button is stored becouse its Tag property is set to "1" and in Button Style a Trigger 
        /// for the pressed appearence is fired. When the Button is realeas the Tag property is set to "null"
        /// </summary>
        private Button _CurrentPressedButton;

        /// <summary>
        /// It is a collection of Normal Buttons
        /// </summary>
        private IEnumerable<Button> _NormalButtons;

        private Button _BackspaceButton;

        private bool _IsShiftPressed;

        #endregion

        #region Properties

        /// <summary>
        /// Singleton
        /// </summary>
        public static StandardKeyboard Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new StandardKeyboard();
                }

                return _Instance;
            }
        }

        public Dictionary<string, string> NumericCharacters { get; private set; }

        public bool IsCapsOn
        {
            get { return (bool)GetValue(IsCapsOnProperty); }
            private set { SetValue(IsCapsOnProperty, value); }
        }

        public bool IsSpecialOn
        {
            get { return (bool)GetValue(IsSpecialOnProperty); }
            private set { SetValue(IsSpecialOnProperty, value); }
        }

        #endregion

        #region Dependency Properties

        private static readonly DependencyProperty IsCapsOnProperty =
         DependencyProperty.Register("IsCapsOn", typeof(bool), typeof(StandardKeyboard));

        private static readonly DependencyProperty IsSpecialOnProperty =
        DependencyProperty.Register("IsSpecialOn", typeof(bool), typeof(StandardKeyboard));

        #endregion

        #region Constructor

        public StandardKeyboard()
            : base()
        {
            InitializeComponent();

            // Create the dictionaries
            _LowerCaseLetter = new Dictionary<string, string>();
            _UpperCaseLetter = new Dictionary<string, string>();
            _SpecialCharacters = new Dictionary<string, string>();
            NumericCharacters = new Dictionary<string, string>();

            _Instance = this;

            // Get all the Buttons
            _NormalButtons = this.FindVisualChildren<Button>(this);
            _BackspaceButton = BackspaceKeybordButton;

            // Set the data contex
            LayoutRoot.DataContext = this;
        }

        public StandardKeyboard(KeyboardLayout layout)
            : base(layout)
        {
            InitializeComponent();

            _Instance = this;

            // Get all the Normal Buttons
            _NormalButtons = this.FindVisualChildren<Button>(this);

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
                _LowerCaseLetter = new Dictionary<string, string>(keyboardLayout.LowerCaseLetter);
                _UpperCaseLetter = keyboardLayout.UpperCaseLetter;
                _SpecialCharacters = keyboardLayout.SpecialCharacters;
                NumericCharacters = keyboardLayout.NumericCharacters;

                CurrentDictionary = _LowerCaseLetter;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Set the kayboard position
        /// </summary>
        protected override void SetKeyboardPosition()
        {
            SetPositionOnBottom();
            //// Get center of desktop working area
            //var desktopWorkingArea = SystemParameters.WorkArea;
            //var desktopWorkingAreaCenter = desktopWorkingArea.Width / 2;

            //// Get center of keyboard
            //var keyboardCenter = this.Width / 2;

            //// Place Keyboard on the center and bottom
            //Left = (desktopWorkingAreaCenter - keyboardCenter);
            //Top = desktopWorkingArea.Bottom - this.Height - 5;
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the Window is loaded.
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            CurrentDictionary = _LowerCaseLetter;
            IsCapsOn = false;
            IsSpecialOn = false;
        }

        /// <summary>
        /// Changes the caps nn value and the layout for uppecase\lowercase letters
        /// </summary>
        private void OnCpasButtonTouchDown(object sender, TouchEventArgs e)
        {
            IsCapsOn = !IsCapsOn;
            if (!IsSpecialOn)
            {
                CurrentDictionary = IsCapsOn ? _UpperCaseLetter : _LowerCaseLetter;
            }

            e.Handled = true;
        }

        /// <summary>
        /// Changes the caps nn value and the layout for uppecase\lowercase letters
        /// </summary>
        private void OnCpasButtonClick(object sender, MouseButtonEventArgs e)
        {
            // Prevent to call the button click after a touch down
            if (e.StylusDevice != null)
            {
                e.Handled = true;
            }
            else
            {
                IsCapsOn = !IsCapsOn;

                if (System.Windows.Forms.Control.IsKeyLocked(System.Windows.Forms.Keys.CapsLock))
                {
                    IsCapsOn = true;
                }

                if (!IsSpecialOn)
                {
                    CurrentDictionary = IsCapsOn ? _UpperCaseLetter : _LowerCaseLetter;
                }
            }
        }

        /// <summary>
        /// Occurs when the special key must be shown
        /// </summary>
        private void OnSpecialButtonTouchDown(object sender, TouchEventArgs e)
        {
            IsSpecialOn = !IsSpecialOn;
            if (IsSpecialOn)
            {
                CurrentDictionary = _SpecialCharacters;
            }
            else
            {
                CurrentDictionary = IsCapsOn ? _UpperCaseLetter : _LowerCaseLetter;
            }

            e.Handled = true;
        }

        /// <summary>
        /// Occurs when the special key must be shown
        /// </summary>
        private void OnSpecialButtonClick(object sender, MouseButtonEventArgs e)
        {
            // Prevent to call the button click after a touch down
            if (e.StylusDevice != null)
            {
                e.Handled = true;
            }
            else
            {
                IsSpecialOn = !IsSpecialOn;
                if (IsSpecialOn)
                {
                    CurrentDictionary = _SpecialCharacters;
                }
                else
                {
                    CurrentDictionary = IsCapsOn ? _UpperCaseLetter : _LowerCaseLetter;
                }
            }
        }

        /// <summary>
        /// The enter button close the Keybord
        /// </summary>
        private void OnEnterButtonTouchDown(object sender, TouchEventArgs e)
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
            // Prevent to call the button click after a touch down
            if (e.StylusDevice != null)
            {
                e.Handled = true;
            }
            else
            {
                OnKeypressed(new KeyboardButtonEventArgs(KeyboardButtonType.Enter));
                AsyncHide();
            }
        }

        /// <summary>
        /// Occurs when a normal button is touched
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
            // Prevent to call the button click after a touch down
            if (e.StylusDevice != null)
            {
                e.Handled = true;
            }
            else
            {
                var content = (sender as Button).Content?.ToString();
                OnKeypressed(new KeyboardButtonEventArgs(KeyboardButtonType.Normal, content));
            }
        }

        /// <summary>
        /// Occurs when the backspace button is touched
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
            // Prevent to call the button click after a touch down
            if (e.StylusDevice != null)
            {
                e.Handled = true;
            }
            else
            {
                OnKeypressed(new KeyboardButtonEventArgs(KeyboardButtonType.Backspace));
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
            // Prevent to call the button click after a touch down
            if (e.StylusDevice != null)
            {
                e.Handled = true;
            }
            else
            {
                var content = (sender as Button).Content?.ToString();
                OnKeypressed(new KeyboardButtonEventArgs(KeyboardButtonType.DecimalSeparator, content));
            }
        }

        /// <summary>
        /// Occurs when the exit button is clicked
        /// </summary>

        private void OnExitButtonTouchDown(object sender, TouchEventArgs e)
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
            // Prevent to call the button click after a touch down
            if (e.StylusDevice != null)
            {
                e.Handled = true;
            }
            else
            {
                OnKeypressed(new KeyboardButtonEventArgs(KeyboardButtonType.Exit));
                AsyncHide();
            }
        }

        /// <summary>
        /// Use this event to Get a Special Buttons (a button with not a Text content) from Phisical Keyboard.
        /// After his event the OnTextInput is fired.
        /// Change the Tag to "1" value to fires the Trigger in Button Style to change the appareance in Pressed Button
        /// </summary>
        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (_CurrentPressedButton != null)
            {
                _CurrentPressedButton.Tag = null;
                _CurrentPressedButton = null;
            }

            switch (e.Key)
            {
                case Key.Return: _CurrentPressedButton = EnterKeybordButton; break;
                case Key.Back: _CurrentPressedButton = BackspaceKeybordButton; _CurrentPressedButton.Tag = "1"; break;
                case Key.Escape: _CurrentPressedButton = EscapeKeyboardButton; break;
                case Key.CapsLock: _CurrentPressedButton = CapsKeybordButton; break;
                case Key.LeftShift:
                case Key.RightShift:
                    if (!IsSpecialOn && !IsCapsOn && (!System.Windows.Forms.Control.IsKeyLocked(System.Windows.Forms.Keys.CapsLock)))
                    {
                        _IsShiftPressed = true;
                        IsCapsOn = true;
                        CurrentDictionary = _UpperCaseLetter;
                    }
                    break;
                case Key.Space: _CurrentPressedButton = SpaceKeyboardButton; _CurrentPressedButton.Tag = "1"; break;
                case Key.LeftCtrl:
                case Key.RightCtrl: _CurrentPressedButton = SpecialKeyboardButton; break;
            }

            if (_CurrentPressedButton != null)
            {
                // Fire the Preview Mouse down event
                MouseButtonEventArgs arg = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left);
                arg.RoutedEvent = Button.PreviewMouseDownEvent;
                _CurrentPressedButton.RaiseEvent(arg);

                e.Handled = true;
            }
            else
            {
                //var content = KeyInterop.VirtualKeyFromKey(e.Key);
                //var inputChar = (char)content;
                //OnKeypressed(new KeyboardButtonEventArgs(KeyboardButtonType.Normal, inputChar.ToString()));
            }
        }

        /// <summary>
        /// Occurs when the Key is down from Phisical Keyboard.
        /// Use this event to Get a Normal Buttons (a button with a Text content) from Phisical Keyboard.
        /// This event is fired after the OnPreviewKeyDown event only if a Special Buttons is not pressed.
        /// </summary>
        private void OnTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Text))
            {
                //Get the input key
                var keyChar = (Char)System.Text.Encoding.ASCII.GetBytes(e.Text)[0];

                // Get all Buttons of Keyboard
                if (_NormalButtons != null)
                {
                    var currentChar = keyChar.ToString();

                    // Get the Button with the Content with the KeyChar from Normal Buttons
                    _CurrentPressedButton = _NormalButtons.FirstOrDefault(b => b != null && b.Content != null && b?.Content?.ToString() == currentChar);
                }

                if (_CurrentPressedButton != null)
                {
                    // Change the Tag to fires the Trigger in Button Style to change the appareance to Pressed Button
                    _CurrentPressedButton.Tag = "1";

                    // Fire the Preview Mouse down event
                    MouseButtonEventArgs arg = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left);
                    arg.RoutedEvent = Button.PreviewMouseDownEvent;
                    _CurrentPressedButton.RaiseEvent(arg);
                }
            }
        }

        /// <summary>
        /// Occurs when the Key is Up from Phisical Keyboard
        /// </summary>
        private void OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (_CurrentPressedButton != null)
            {
                // Change the Tag to fires the Trigger in Button Style to change the appareance in Realesed Button
                _CurrentPressedButton.Tag = null;
            }

            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                if (!IsSpecialOn && !System.Windows.Forms.Control.IsKeyLocked(System.Windows.Forms.Keys.CapsLock) && _IsShiftPressed)
                {
                    IsCapsOn = false;
                    _IsShiftPressed = false;
                    CurrentDictionary = _LowerCaseLetter;
                }
            }
        }

        /// <summary>
        /// When the Keyboard become visible\invisible check for the Caps of the Phisical keyboard
        /// to updates its value
        /// </summary>
        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsSpecialOn)
            {
                MouseButtonEventArgs arg = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left);
                arg.RoutedEvent = Button.PreviewMouseDownEvent;
                SpecialKeyboardButton.RaiseEvent(arg);
            }

            // Checks Capslock is on\off
            IsCapsOn = (System.Windows.Forms.Control.IsKeyLocked(System.Windows.Forms.Keys.CapsLock));
            CurrentDictionary = IsCapsOn ? _UpperCaseLetter : _LowerCaseLetter;
        }

        #endregion
    }
}
