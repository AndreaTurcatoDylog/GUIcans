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
    public abstract class TextBoxKeyboardBase : TextBox, IDisposable, IActivate
    {
        #region Members

        protected IAdjustableFrameworkElement _AdjustableContainer;

        /// <summary>
        /// A copy of the original text it is stored so if the exit keyboard key
        /// is pressed the Text will be restored to original
        /// </summary>
        protected string _OriginalText;

        /// <summary>
        /// Specify if the keyboard is closed without saving (exit mode).
        /// TRUE: The keyboard is closed without saving (exit mode).
        /// FALSE: The keyboard is closed saving the value (closed mode).
        /// </summary>
        protected bool _IsKeyboardExit;

        /// <summary>
        /// Specify the keyboard associated with textbox
        /// </summary>
        protected KeyboardBase _Keyboard;

        /// <summary>
        /// Specify whether the position of Keyboard is fixed 
        /// (ex. Standard Keyboard is always in the center of Window) or it is float (ex. Numeric Keyboard is always near the TextBox)
        /// </summary>
        protected bool _IsPositionFixed;

        #endregion

        #region Events

        public event EventHandler KeyboardKeyPressed;
        public event EventHandler KeyboardOpened;
        public event EventHandler KeyboardClosed;
        public event EventHandler KeyboardExit;
        public event EventHandler WritingModeChanged;
        public event EventHandler ComponentIsLoaded;
        public event EventHandler Deactivated;
        public event EventHandler TemporaryValueChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Specify the type of keyboard
        /// </summary>
        public KeyboardLayoutType KeyboardType
        {
            get { return (KeyboardLayoutType)this.GetValue(KeyboardTypeProperty); }
            set
            {
                this.SetValue(KeyboardTypeProperty, value);
            }
        }

        /// <summary>
        /// Specify if the Keyboard must be disabled
        /// </summary>
        public bool DisableKeyboard
        {
            get { return (bool)this.GetValue(DisableKeyboardProperty); }
            set
            {
                this.SetValue(DisableKeyboardProperty, value);
            }
        }

        /// <summary>
        /// Specify whether the textbox is in blocked windows or not
        /// TRUE: when the kyboard appers the textbox is under a blocked opaque window
        /// </summary>
        public bool IsBlockedWindow
        {
            get { return (bool)this.GetValue(IsBlockedWindowProperty); }
            set
            {
                this.SetValue(IsBlockedWindowProperty, value);
            }
        }

        /// <summary>
        /// The writing mode specify if TextBoxKeyboard has keyboard associated
        /// </summary>
        public bool IsWritingMode
        {
            get { return (bool)this.GetValue(IsWritingProperty); }
            set
            {
                this.SetValue(IsWritingProperty, value);
                WritingModeChanged?.Invoke(this, new WritingModeEventArgs(value));
            }
        }

        /// <summary>
        /// The offset X for keyboard position. It is necessary in some situation where must place the 
        /// Kyboard more left or right by calculate position
        /// </summary>
        public double OffsetX
        {
            get { return (double)this.GetValue(OffsetXProperty); }
            set { this.SetValue(OffsetXProperty, value); }
        }

        /// <summary>
        /// The offset Y for keyboard position. It is necessary in some situation where must place the 
        /// Kyboard more Up or Down by calculate position
        /// </summary>
        public double OffsetY
        {
            get { return (double)this.GetValue(OffsetYProperty); }
            set { this.SetValue(OffsetYProperty, value); }
        }

        /// <summary>
        /// Specify whether the Text is equals to the original or not (Dirty if it is modified)
        /// </summary>
        public bool IsDirty
        {
            get { return _OriginalText != Text; }
        }

        #endregion

        #region Dependency Properties

        private static readonly DependencyProperty KeyboardTypeProperty =
          DependencyProperty.Register("KeyboardType", typeof(KeyboardLayoutType), typeof(TextBoxKeyboardBase));

        private static readonly DependencyProperty DisableKeyboardProperty =
          DependencyProperty.Register("DisableKeyboard", typeof(bool), typeof(TextBoxKeyboardBase), new PropertyMetadata(false));

        private static readonly DependencyProperty IsWritingProperty =
          DependencyProperty.Register("IsWritingMode", typeof(bool), typeof(TextBoxKeyboardBase), new PropertyMetadata(false));

        private static readonly DependencyProperty IsBlockedWindowProperty =
         DependencyProperty.Register("IsBlockedWindow", typeof(bool), typeof(TextBoxKeyboardBase), new PropertyMetadata(true));

        private static readonly DependencyProperty OffsetXProperty =
          DependencyProperty.Register("OffsetX", typeof(double), typeof(TextBoxKeyboardBase));

        private static readonly DependencyProperty OffsetYProperty =
         DependencyProperty.Register("OffsetY", typeof(double), typeof(TextBoxKeyboardBase));

        #endregion

        #region Constructor

        public TextBoxKeyboardBase()
        {
            Loaded += OnLoaded;
            _IsPositionFixed = true;
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Calculate the new position of Keyboard in screen if needed
        /// </summary>
        /// <returns> Return new Keyboard position (if it is changed) </returns>
        protected virtual Rect CalculateKeyboardPositions(Rect textBoxPosition)
        {
            return new Rect(_Keyboard.Left, _Keyboard.Top, _Keyboard.Width, _Keyboard.Height);
        }

        /// <summary>
        /// Move the parent windows if it is needed.
        /// </summary>
        /// <returns>
        ///  Returns the new text box position (ic could be changed after the parent window is moved)
        /// </returns>
        protected virtual Rect MoveParentWindow(Rect textBoxPosition)
        {
            return textBoxPosition;
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
                PreviewMouseDown += OnPreviewMouseDown;
            }
            else
            {
                Loaded -= OnLoaded;
                PreviewMouseDown -= OnPreviewMouseDown;
            }
        }

        /// <summary>
        /// Force the stop of editing in text box.
        /// Permites to save or undo the value in text
        /// </summary>
        public void CloseKeyboard(bool saveValue)
        {
            try
            {
                _IsKeyboardExit = !saveValue;

                // Deactivate the component
                Deactivate();

                // Close the Keyboard
                _Keyboard.Hide();

                // Fire the keyboard close\exit event
                OnKeyboardClosed(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                CoreLog.Instance.Append($"[{ex.Source}] - {ex.Message} - {ex.StackTrace}", CoreLogType.Error);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// The load event register all events
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _Keyboard = KeyboardFactory.Get(KeyboardType);
            _AdjustableContainer = (IAdjustableFrameworkElement)this.GetParentAdjustableFrameworkElement();

            IsBlockedWindow = !DisableKeyboard;
            IsReadOnly = !DisableKeyboard;

            RegisterEvents(true);

            // Fire event loaded finished
            ComponentIsLoaded?.Invoke(sender, e);
        }

        /// <summary>
        /// Set the text of the text box in according to keyboard's button pressed.
        /// The TemporaryValuechangedEvent specify if user pressed a key and the temporary value is changed.
        /// Reminds that the real value is changed when the Keyboard is closed.
        /// It could be useful to know the value when the user press the key of Keyboard whitout exit, so an event is fired.
        /// </summary>
        protected virtual void OnKeyboardKeyPressed(object sender, EventArgs e)
        {
            if (IsWritingMode)
            {
                var args = (KeyboardButtonEventArgs)e;
                KeyboardKeyPressed?.Invoke(this, e);

                switch (args.KeyboardButtonType)
                {
                    case KeyboardButtonType.Normal:
                    case KeyboardButtonType.DecimalSeparator:
                        Text = $"{Text}{args.Content}";
                        break;
                    case KeyboardButtonType.Backspace:
                        if (Text.Length > 0)
                        {
                            Text = Text.Remove(Text.Length - 1, 1);
                        }
                        break;
                    case KeyboardButtonType.Enter:
                        _IsKeyboardExit = false;
                        break;
                    case KeyboardButtonType.Exit:
                        Text = _OriginalText;
                        _IsKeyboardExit = true;
                        break;
                }

                TemporaryValueChanged?.Invoke(this, new TextBoxKeyboardKeyPressedEventArgs(Text));
            }
        }

        /// <summary>
        /// On mouse click in text box the keyboard will appears
        /// </summary>
        private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!DisableKeyboard)
            {
                var calledAfterTouchDown = e.CalledAfterTouchDown();
                if (!calledAfterTouchDown)
                {
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        Activate();
                    }
                }

                e.Handled = true;
            }
        }

        /// <summary>
        /// Active the component
        /// </summary>
        public void Activate()
        {
            // Raise the Keyboard opened event
            OnKeyboardOpened(this, EventArgs.Empty);

            try
            {
                // Store the Original text
                _OriginalText = Text;

                // Set the focus
                this.Focus();

                // Set the writing mode and register keypressed event
                IsWritingMode = true;

                // Get the position of the TextBoxKeyboard
                var textBoxPosition = this.GetAbsolutePlacement(true);

                // Register event to listen to keyboard keypressed 
                _Keyboard.KeyPressed += OnKeyboardKeyPressed;

                // Get the Keyboard position
                Rect newKeyboardPosition;

                if ((!_IsPositionFixed) || (double.IsNaN(_Keyboard.Left) || double.IsNaN(_Keyboard.Top) ||
                    double.IsNaN(_Keyboard.Width) || double.IsNaN(_Keyboard.Height)))
                {
                    _Keyboard.Show();

                    newKeyboardPosition = CalculateKeyboardPositions(textBoxPosition);
                    _Keyboard.Top = newKeyboardPosition.Top;
                    _Keyboard.Left = newKeyboardPosition.Left;

                    _Keyboard.Hide();
                }

                // Move the parent windows if needed
                textBoxPosition = MoveParentWindow(textBoxPosition);

                // Show keyboard
                if (IsBlockedWindow)
                {
                    // _Keyboard.Hide();
                    var blockedTrasparentWindow = new BlockedKeyboardWindow(_Keyboard, textBoxPosition);
                    blockedTrasparentWindow.Open();
                    // blockedTrasparentWindow.ShowDialog();
                }
                else
                {
                    _Keyboard.Hide();
                    _Keyboard.ShowDialog();
                }

                // Stop the writing mode and unregister keypressed event
                //IsWritingMode = false;
                Deactivate();

                // Unregister event to listen to keyboard keypressed 
                _Keyboard.KeyPressed -= OnKeyboardKeyPressed;

                // Restore position of parent window (if changed)
                _AdjustableContainer?.RestorePosition();

                // Fire the keyboard close\exit event
                OnKeyboardClosed(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                CoreLog.Instance.Append($"[{ex.Source}] - {ex.Message} - {ex.StackTrace}", CoreLogType.Error);
            }
        }

        /// <summary>
        /// Deactive component
        /// </summary>
        public void Deactivate()
        {
            IsWritingMode = false;
            Deactivated?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Fire the keyboard Opened event.
        /// </summary>
        protected void OnKeyboardOpened(object sender, EventArgs eventArgs)
        {
            // Fire Keyboard closed event
            KeyboardOpened?.Invoke(this, eventArgs);
        }


        /// <summary>
        /// Fire the keyboard closed or keyboard exit. Depends if the value is saved or not
        /// </summary>
        protected void OnKeyboardClosed(object sender, EventArgs eventArgs)
        {
            if (_IsKeyboardExit)
            {
                // Fire Keyboard exit event
                KeyboardExit?.Invoke(this, eventArgs);
            }
            else
            {
                // Fire Keyboard closed event
                KeyboardClosed?.Invoke(this, eventArgs);
            }
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
