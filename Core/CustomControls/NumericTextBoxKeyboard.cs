using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Core
{
    public class NumericTextBoxKeyboard : TextBoxKeyboardBase
    {
        #region Envet Handler

        public event EventHandler OutOfRangeReached;
        public event EventHandler NumericValueChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Get\Set Decimal Numbers.
        /// Specify the number of decimal number after decimal separator
        /// </summary>
        public int DecimalNumbers
        {
            get { return (int)GetValue(DecimalNumbersProperty); }
            set
            {
                SetValue(DecimalNumbersProperty, value);
            }
        }

        // <summary>
        /// Get\Set OutOfRange. 
        /// True: value not in range [MinValue, MaxValue]
        /// </summary>
        public bool IsOutOfRange
        {
            get { return (bool)GetValue(IsOutOfRangeProperty); }
            set
            {
                SetValue(IsOutOfRangeProperty, value);
            }
        }

        // <summary>
        /// Get\Set the Min Value 
        /// </summary>
        [Bindable(true)]
        public double MinValue
        {
            get { return (double)GetValue(MinValueProperty); }
            set
            {
                SetValue(MinValueProperty, value);
            }
        }

        // <summary>
        /// Get\Set the Max Value 
        /// </summary>
        [Bindable(true)]
        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set
            {
                SetValue(MaxValueProperty, value);
            }
        }

        #endregion

        #region Dependency Properties

        private static readonly DependencyProperty IsOutOfRangeProperty =
         DependencyProperty.Register("IsOutOfRange", typeof(bool), typeof(NumericTextBoxKeyboard));

        private static readonly DependencyProperty MinValueProperty =
        DependencyProperty.Register("MinValue", typeof(double), typeof(NumericTextBoxKeyboard),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        private static readonly DependencyProperty MaxValueProperty =
        DependencyProperty.Register("MaxValue", typeof(double), typeof(NumericTextBoxKeyboard),
            new FrameworkPropertyMetadata(1000.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        private static readonly DependencyProperty DecimalNumbersProperty =
         DependencyProperty.Register("DecimalNumbers", typeof(int), typeof(NumericTextBoxKeyboard), new PropertyMetadata(2));

        #endregion

        #region Constructor

        static NumericTextBoxKeyboard()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericTextBoxKeyboard), new FrameworkPropertyMetadata(typeof(NumericTextBoxKeyboard)));
        }

        public NumericTextBoxKeyboard()
            : base()
        {
            KeyboardType = KeyboardLayoutType.Numeric;
            _IsPositionFixed = false;

            // Register loaded event
            Loaded += OnLoaded;
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
                KeyboardClosed += OnNumericKeyboardClosed;
                TextChanged += OnTextChanged;
            }
            else
            {
                Loaded -= OnLoaded;
                KeyboardClosed -= OnNumericKeyboardClosed;
                TextChanged -= OnTextChanged;
            }

            base.RegisterEvents(register);
        }

        /// <summary>
        /// Controls whether the number is out of range [MinValue, MaxValue] or not
        /// </summary>
        protected bool IsNumberOutOfRange(string text)
        {
            if (MinValue != double.NaN || MaxValue != double.NaN)
            {
                try
                {
                    if (!string.IsNullOrEmpty(Text))
                    {
                        var number = double.Parse(Text, CultureInfo.InvariantCulture);
                        return IsNumberOutOfRange(number);
                    }
                    else
                    {
                        return true;
                    }
                }
                catch(Exception ex)
                {
                    CoreLog.Instance.Append($"[{ex.Source}] - {ex.Message} - {ex.StackTrace}", CoreLogType.Error);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Controls whether the number is out of range [MinValue, MaxValue] or not
        /// </summary>
        protected bool IsNumberOutOfRange(double number)
        {
            if (MinValue != double.NaN || MaxValue != double.NaN)
            {
                return (number > MaxValue || number < MinValue);
            }

            return false;
        }

        /// <summary>
        /// Show the keyboard
        /// </summary>
        protected override Rect CalculateKeyboardPositions(Rect elementPosition)
        {
            const int spaceX = 8;

            var left = _Keyboard.Left;
            var top = _Keyboard.Top;

            try { 
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
            catch(Exception ex)
            {
                CoreLog.Instance.Append($"[{ex.Source}] - {ex.Message} - {ex.StackTrace}", CoreLogType.Error);
            }

            // if something goes wrong so return positionon top left of the screen
            return new Rect(0, 0, _Keyboard.Width, _Keyboard.Height);
        }

        /// <summary>
        /// Set the numeric value in according to the range ([MinValue, MaxValue])
        /// </summary>
        public void SetNumericValue()
        {
            try
            {
                if (!string.IsNullOrEmpty(Text))
                {
                    double newValue = double.Parse(Text, CultureInfo.InvariantCulture);

                    newValue = Math.Round(newValue, DecimalNumbers);
                    //Text = newValue.ToString(ResourceManager.Cultures.Resources.Culture);
                    Text = newValue.ToString(CultureInfo.InvariantCulture);
                    IsOutOfRange = false;
                }
                else
                {
                    IsOutOfRange = true;
                }
            }
            catch (Exception ex)
            {
                CoreLog.Instance.Append($"[{ex.Source}] - {ex.Message} - {ex.StackTrace}", CoreLogType.Error);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// The loaded event
        /// </summary>
        /// <param name="e"></param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            SetNumericValue();
        }

        /// <summary>
        /// Controls if the format is correct
        /// </summary>
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            double newValue;
            var parsed = double.TryParse(Text, System.Globalization.NumberStyles.Any, ResourceManager.Cultures.Resources.Culture, out newValue);
            if (!parsed && !string.IsNullOrEmpty(Text))
            {
                //var decimalSeparator = ResourcesManager.Cultures.Resources.Culture.NumberFormat.NumberDecimalSeparator;
                Text = Text.Remove(Text.Length - 1, 1);
            }
            else
            {
                NumericValueChanged?.Invoke(sender, e);
            }
        }

        /// <summary>
        /// Set the text of the text box in according to kayboard's button pressed
        /// </summary>
        protected override void OnKeyboardKeyPressed(object sender, EventArgs e)
        {
            // Call the keypressed base
            base.OnKeyboardKeyPressed(sender, e);

            var args = (KeyboardButtonEventArgs)e;
            switch (args.KeyboardButtonType)
            {
                case KeyboardButtonType.Normal:
                case KeyboardButtonType.DecimalSeparator:
                case KeyboardButtonType.Backspace:
                    IsOutOfRange = IsNumberOutOfRange(Text);
                    break;

                case KeyboardButtonType.Enter:
                    SetNumericValue();
                    break;

                case KeyboardButtonType.Exit:
                    IsOutOfRange = false;
                    break;           
            }
        }

        /// <summary>
        /// After closed the keybaord the numeric value is adjusted and checks if there are errors
        /// </summary>
        public void AdjustNumericValue()
        {
            if (MinValue != double.NaN || MaxValue != double.NaN)
            {
                var error = false;
                var parsed = double.TryParse(Text, System.Globalization.NumberStyles.Any, ResourceManager.Cultures.Resources.Culture, out double number);
                if (parsed)
                {
                    if (number < MinValue)
                    {
                        error = true;
                        Text = MinValue.ToString();
                    }
                    else if (number > MaxValue)
                    {
                        error = true;
                        Text = MaxValue.ToString();
                    }

                    if (Text[Text.Length - 1] == '.' || Text[Text.Length - 1] == ',')
                    {
                        Text = Text.Remove(Text.Length - 1, 1);
                    }

                    if (error)
                    {
                        OutOfRangeReached?.Invoke(this, EventArgs.Empty);
                    }
                }
                else
                {
                    Text = MinValue.ToString();
                }
            }

            IsOutOfRange = false;
        }

        /// <summary>
        /// When the kyaboard is closed the number is adjusted if is out of range , in this case the numeric value is reset.
        /// The OutOfRangeReached event is fired
        /// </summary>
        private void OnNumericKeyboardClosed(object sender, EventArgs e)
        {
            AdjustNumericValue();
        }

        #endregion
    }
}
