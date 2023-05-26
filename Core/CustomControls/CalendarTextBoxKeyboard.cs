using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Core
{
    public class CalendarTextBoxKeyboard : TextBoxKeyboardBase
    {
        #region Envet Handler

        // public event EventHandler NumericValueChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Get\Set DateTime.
        /// </summary>
        public DateTime? DateTimeValue
        {
            get { return (DateTime?)GetValue(DateTimeValueProperty); }
            set{ SetValue(DateTimeValueProperty, value); }
        }

        #endregion

        #region Dependency Properties

        private static readonly DependencyProperty DateTimeValueProperty =
        DependencyProperty.Register("DateTimeValue", typeof(DateTime?), typeof(CalendarTextBoxKeyboard), new PropertyMetadata(OnDateTimeValueChangedCallBack));

        #endregion

        #region Constructor

        static CalendarTextBoxKeyboard()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarTextBoxKeyboard), new FrameworkPropertyMetadata(typeof(CalendarTextBoxKeyboard)));
        }

        public CalendarTextBoxKeyboard()
            : base()
        {
            KeyboardType = KeyboardLayoutType.Calendar;

            // Register loaded event
           // Loaded += OnLoaded;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Register\Unregister all events
        /// </summary>
        protected override void RegisterEvents(bool register)
        {
            if (register)
            {
                (_Keyboard as CalendarKeyboard).OnDateChanged += OnFilterDateChanged;
                (_Keyboard as CalendarKeyboard).OnDateResetted += OnFilterDateResetted;
            }
            else
            {
                (_Keyboard as CalendarKeyboard).OnDateChanged -= OnFilterDateChanged;
                (_Keyboard as CalendarKeyboard).OnDateResetted -= OnFilterDateResetted;
            }

            base.RegisterEvents(register);
        }

        /// <summary>
        /// Show the keyboard
        /// </summary>
        protected override Rect CalculateKeyboardPositions(Rect elementPosition)
        {
            const int spaceX = 8;

            var left = _Keyboard.Left;
            var top = _Keyboard.Top;

            try
            {
                // Get the position on the main windows of the Adjustable parent container
                var adjustableElementPosition = ((FrameworkElement)_AdjustableContainer).GetAbsolutePlacement(true);

                // Calculate the Actual Height of the Adjustable parent container
                var adjustableElementPositionHeight = adjustableElementPosition.Top + adjustableElementPosition.Height;

                // Get the difference between the actual height of the Adjustable parent and the acutal height of text box
                var difference = (double)(adjustableElementPositionHeight - (elementPosition.Top + elementPosition.Height));

                // Calculate the difference between text box top position and parent adjustable container top position
                var topDifference = (double)(adjustableElementPositionHeight - (_Keyboard.Height + elementPosition.Top));
                if (topDifference <= 0)
                {
                    top = elementPosition.Top + elementPosition.Height - _Keyboard.Height + Math.Abs(difference);
                }
                else
                {
                    top = elementPosition.Top;
                }

                // Calculate the difference between text box right position and parent window right position
                var rightDifference = (double)(_AdjustableContainer?.ActualWidth - (_Keyboard.Width - elementPosition.Left));
                if (rightDifference <= 0)
                {
                    left = elementPosition.Left - (OffsetX + _Keyboard.Width + spaceX);
                }
                else
                {
                    left = elementPosition.Left + elementPosition.Width + OffsetX + spaceX;
                }

                return new Rect(left, top, _Keyboard.Width, _Keyboard.Height);
            }
            catch (Exception ex)
            {
                CoreLog.Instance.Append($"[{ex.Source}] - {ex.Message} - {ex.StackTrace}", CoreLogType.Error);
            }

            // if something goes wrong so return positionon top left of the screen
            return new Rect(0, 0, _Keyboard.Width, _Keyboard.Height);
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the DateTimeValue changes
        /// </summary>
        private static void OnDateTimeValueChangedCallBack(
        DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var c = sender as CalendarTextBoxKeyboard;
            if (c != null && c.DateTimeValue == null)
            {
                c.Text = string.Empty;
            }
        }

        /// <summary>
        /// Occurs when the Date is changed
        /// </summary>
        private void OnFilterDateChanged(object sender, EventArgs e)
        {
            try
            {
                var source = (e as SelectionChangedEventArgs)?.Source;

                if (source != null)
                {
                    var selectedDate = (source as Calendar).SelectedDate.Value;
                    Text = selectedDate.ToString("yyyy-MM-dd");

                    // Set the DateTime Value
                    DateTimeValue = selectedDate;
                }
            }
            catch(Exception ex)
            {
                CoreLog.Instance.Append($"[{ex.Source}] - {ex.Message} - {ex.StackTrace}", CoreLogType.Error);
            }
        }

        /// <summary>
        /// Occurs when the filter date has no value
        /// </summary>
        private void OnFilterDateResetted(object sender, EventArgs e)
        {
            Text = string.Empty;
            DateTimeValue = null;
        }        

        #endregion
    }
}

