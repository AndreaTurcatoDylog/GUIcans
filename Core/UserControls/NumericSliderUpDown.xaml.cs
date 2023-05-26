using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for NumericSliderUpDown.xaml
    /// </summary>
    public partial class NumericSliderUpDown : DisposableUserControlBase, IActivate, IHasPopUp
    {
        #region Fields

        /// <summary>
        /// The parent adjustable window
        /// </summary>
        private IAdjustableFrameworkElement _ParentWindow;

        private bool _IsKeyboardOpened;

        #endregion

        #region Properties

        /// <summary>
        /// Specify whether the slider is visible or not
        /// </summary>
        public bool IsSliderVisible
        {
            get { return (bool)this.GetValue(IsSliderVisibleProperty); }
            set
            {
                this.SetValue(IsSliderVisibleProperty, value);
            }
        }

        /// <summary>
        /// The writing mode specify if the component is going to write 
        /// </summary>
        public bool IsWritingMode
        {
            get { return (bool)this.GetValue(IsWritingProperty); }
            private set
            {
                this.SetValue(IsWritingProperty, value);
            }
        }

        /// <summary>
        /// The writing mode of numeric TextBox
        /// </summary>
        public bool IsNumericTextBoxWritingMode
        {
            get { return (bool)this.GetValue(IsNumericTextBoxWritingModeProperty); }
            private set
            {
                this.SetValue(IsNumericTextBoxWritingModeProperty, value);
            }
        }

        /// <summary>
        /// Get\Set Numeric Value
        /// </summary>
        public double NumericValue
        {
            get { return (double)GetValue(NumericValueProperty); }
            set
            {
                SetValue(NumericValueProperty, value);
            }
        }

        /// <summary>
        /// Get\Set Min Value
        /// </summary>
        public double MinValue
        {
            get { return (double)GetValue(MinValueProperty); }
            set
            {
                SetValue(MinValueProperty, value);
                NumericTextBox.MinValue = value;
            }
        }

        /// <summary>
        /// Get\Set Max Value
        /// </summary>
        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set
            {
                SetValue(MaxValueProperty, value);
                NumericTextBox.MaxValue = value;
            }
        }

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

        /// <summary>
        /// Get\Set Step Value
        /// </summary>
        public double StepValue
        {
            get { return (double)GetValue(StepValueProperty); }
            set
            {
                SetValue(StepValueProperty, value);
            }
        }

        /// <summary>
        /// Controls if user control and all its child do not have focus
        /// </summary>
        public event RoutedEventHandler UserControlLostFocus
        {
            add { AddHandler(UserControlLostFocusEvent, value); }
            remove { RemoveHandler(UserControlLostFocusEvent, value); }
        }

        #endregion

        #region Dependency Properties

        private static void OnSliderVisibleChangedCallBack(
        DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var c = sender as NumericSliderUpDown;
            if (c != null)
            {
                c.SetGrid((bool)e.NewValue);
            }
        }

        public static readonly RoutedEvent UserControlLostFocusEvent =
            EventManager.RegisterRoutedEvent("UserControlLostFocus",
                                         RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NumericSliderUpDown));

        private static readonly DependencyProperty IsSliderVisibleProperty =
         DependencyProperty.Register("IsSliderVisible", typeof(bool), typeof(NumericSliderUpDown), new PropertyMetadata(OnSliderVisibleChangedCallBack));

        private static readonly DependencyProperty IsWritingProperty =
         DependencyProperty.Register("IsWritingMode", typeof(bool), typeof(NumericSliderUpDown));

        private static readonly DependencyProperty IsNumericTextBoxWritingModeProperty =
      DependencyProperty.Register("IsNumericTextBoxWritingMode", typeof(bool), typeof(NumericSliderUpDown));

        private static readonly DependencyProperty NumericValueProperty =
          DependencyProperty.Register("NumericValue", typeof(double), typeof(NumericSliderUpDown));

        private static readonly DependencyProperty MinValueProperty =
          DependencyProperty.Register("MinValue", typeof(double), typeof(NumericSliderUpDown), new PropertyMetadata(0.0));

        private static readonly DependencyProperty MaxValueProperty =
          DependencyProperty.Register("MaxValue", typeof(double), typeof(NumericSliderUpDown), new PropertyMetadata(9999999.0));

        private static readonly DependencyProperty DecimalNumbersProperty =
          DependencyProperty.Register("DecimalNumbers", typeof(int), typeof(NumericSliderUpDown), new PropertyMetadata(2));

        private static readonly DependencyProperty StepValueProperty =
         DependencyProperty.Register("StepValue", typeof(double), typeof(NumericSliderUpDown), new PropertyMetadata(1.0));

        #endregion

        #region Event Handlers

        public event EventHandler Deactivated;

        #endregion

        #region Constructor

        public NumericSliderUpDown()
        {
            InitializeComponent();

            LayoutRoot.DataContext = this;
            Loaded += OnLoaded;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Register\Unregister events
        /// </summary>
        protected override void RegisterEvents(bool register)
        {
            if (register)
            {
                GotFocus += OnGotFocus;
                IsKeyboardFocusWithinChanged += OnIsKeyboardFocusWithinChanged;
                UserControlLostFocus += OnUserControlLostFocus;
                UpSliderButton.Click += OnUpButtonClick;
                UpButton.Click += OnUpButtonClick;
                DownButton.Click += OnDownButtonClick;
                NumericTextBox.PreviewTextInput += OnNumericTextBoxPreviewTextInput;
                NumericTextBox.KeyboardKeyPressed += OnKeyboardKeyPressed;
                NumericTextBox.KeyboardClosed += OnKeyboardClosed;
                NumericTextBox.KeyboardExit += OnKeyboardClosed;
                NumericSlider.ValueChanged += OnSliderValueChanged;
                NumericSlider.TouchDown += OnSliderTouchDown;
                NumericTextBox.PreviewMouseDown += OnNumericTextBoxPreviewMouseDown;
                NumericTextBox.TextChanged += OnNumericTextBoxTextChanged;
            }
            else
            {
                Loaded -= OnLoaded;
                IsKeyboardFocusWithinChanged -= OnIsKeyboardFocusWithinChanged;
                UserControlLostFocus -= OnUserControlLostFocus;
                UpSliderButton.Click -= OnUpButtonClick;
                UpButton.Click -= OnUpButtonClick;
                DownButton.Click -= OnDownButtonClick;
                NumericTextBox.PreviewTextInput -= OnNumericTextBoxPreviewTextInput;
                NumericTextBox.KeyboardKeyPressed -= OnKeyboardKeyPressed;
                NumericTextBox.KeyboardClosed -= OnKeyboardClosed;
                NumericTextBox.KeyboardExit -= OnKeyboardClosed;
                NumericSlider.ValueChanged -= OnSliderValueChanged;
                NumericSlider.TouchDown -= OnSliderTouchDown;
                NumericTextBox.PreviewMouseDown -= OnNumericTextBoxPreviewMouseDown;
                NumericTextBox.TextChanged += OnNumericTextBoxTextChanged;
            }
        }

        /// <summary>
        /// Update the slider in according to new value in textbox
        /// </summary>
        private void UpdateSlider()
        {
            try
            {
                if (!string.IsNullOrEmpty(NumericTextBox.Text))
                {
                    var newValue = double.Parse(NumericTextBox.Text, CultureInfo.InvariantCulture);
                    if (newValue >= MinValue && newValue <= MaxValue)
                    {
                        NumericSlider.Value = newValue;
                    }
                }
                else
                {
                    NumericSlider.Value = -1;
                }
            }
            catch (Exception ex)
            {
                CoreLog.Instance.Append($"[{ex.Source}] - {ex.Message} - {ex.StackTrace}", CoreLogType.Error);
            }
        }

        /// <summary>
        /// Activate the component
        /// </summary>
        public void Activate()
        {
            IsWritingMode = true;
            IsNumericTextBoxWritingMode = true;
            NumericTextBox.Focus();
        }

        /// <summary>
        /// Deactivate the component
        /// </summary>
        public void Deactivate()
        {
            IsWritingMode = false;
            IsNumericTextBoxWritingMode = false;

            Deactivated?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Set the grid in according to the visibility of slider
        /// </summary>
        private void SetGrid(bool isSliderVisible)
        {
            if (isSliderVisible)
            {
                SliderGridColumn.Width = new GridLength(1, GridUnitType.Star);
                NumeriTextBoxGridColumn.Width = new GridLength(80, GridUnitType.Pixel);
            }
            else
            {
                SliderGridColumn.Width = new GridLength(0, GridUnitType.Pixel);
                NumeriTextBoxGridColumn.Width = new GridLength(1, GridUnitType.Star);
            }
        }

        /// <summary>
        /// Reload the pop up
        /// </summary>
        public void ReloadPopUp()
        {
            RangePopUp.IsOpen = false;
            RangePopUp.IsOpen = true;
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the user control is loaded
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Get the parent window
            _ParentWindow = (IAdjustableFrameworkElement)this.GetParentAdjustableFrameworkElement();

            // Set grid
            SetGrid(IsSliderVisible);

            // Set numeric value
            NumericTextBox.SetNumericValue();
            UpdateSlider();
        }

        /// <summary>
        /// Occurs when the component get focus
        /// </summary>
        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            Activate();
        }

        /// <summary>
        /// Occurs when the the up button is clicked.
        /// Controls that new value is in range and set the focus to numeric text box
        /// </summary>
        private void OnUpButtonClick(object sender, RoutedEventArgs e)
        {
            var newValue = Math.Round(NumericValue + StepValue, DecimalNumbers);
            NumericValue = newValue;
            if (newValue <= MaxValue)
            {
                NumericValue = Math.Round(newValue, DecimalNumbers);
            }
            else
            {
                NumericValue = MaxValue;
            }
        }

        /// <summary>
        /// Occurs when the the down button is clicked.
        /// Controls that new value is in range and set the focus to numeric text box
        /// </summary>
        private void OnDownButtonClick(object sender, RoutedEventArgs e)
        {
            var newValue = Math.Round(NumericValue - StepValue, DecimalNumbers);
            NumericValue = newValue;
            if (newValue >= MinValue)
            {
                NumericValue = Math.Round(newValue, DecimalNumbers);
            }
            else
            {
                NumericValue = MinValue;
            }
        }

        /// <summary>
        /// Occurs when the text in numeric text box is edited. 
        /// This event prevents the not digit characters 
        /// </summary>
        private void OnNumericTextBoxPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;

            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        /// <summary>
        /// When the numeric text box is choosen:
        /// 1. keyboard is shown
        /// 2. The inner property IsWritingMode of the NumericTextbox will be changed
        /// </summary>
        private void OnNumericTextBoxPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _IsKeyboardOpened = true;
        }

        /// <summary>
        /// Set the text of the text box in according to kayboard's button pressed
        /// </summary>
        private void OnKeyboardKeyPressed(object sender, EventArgs e)
        {
            var args = (KeyboardButtonEventArgs)e;
            switch (args.KeyboardButtonType)
            {
                case KeyboardButtonType.Enter:
                case KeyboardButtonType.Exit:
                    _IsKeyboardOpened = false;
                    break;
            }
        }

        /// <summary>
        /// Occurs when the numeric keyboard is closed.
        /// When the Keyboard is open\closed the the inner property IsWritingMode 
        /// of the NumericTextbox will be changed. In this scenario the component change it again becouse of
        /// is the IsWriting mode of whole component that manage the behaviur (changed of style)
        /// </summary>
        private void OnKeyboardClosed(object sender, EventArgs e)
        {
            // Prvent the change in inner component (NumericTextBox)
            IsNumericTextBoxWritingMode = true;
        }

        /// <summary>
        /// Occurs when the slider is touched. It will works with touch only with handled = true
        /// </summary>
        private void OnSliderTouchDown(object sender, TouchEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// Occurs when the slider changes its value.
        /// It updates the Numeric Value
        /// </summary>
        private void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (NumericSlider.Value >= MinValue && NumericSlider.Value <= MaxValue)
            {
                NumericValue = Math.Round(NumericSlider.Value, DecimalNumbers);
            }
        }

        /// <summary>
        /// Only if the keyboard is not opened the pop up is closed.
        /// It is necessary to avoid the flickering pop up caused by IsWriting mode changes in  the process
        /// </summary>

        private void OnUserControlLostFocus(object sender, RoutedEventArgs e)
        {
            if (!_IsKeyboardOpened)
            {
                Deactivate();
            }
        }
    
        /// <summary>
        /// Occurs when the focus changes. 
        /// Check whether the user control lost focus and no childs have it.
        /// If user control lost focus an event is raised to close the pop up.
        /// </summary>
        private void OnIsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.OldValue == true && (bool)e.NewValue == false)
            {
                RaiseEvent(new RoutedEventArgs(NumericSliderUpDown.UserControlLostFocusEvent, this));
            }
        }

        /// <summary>
        /// Occurs when the Numeric value in Text box changes
        /// </summary>
        private void OnNumericTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateSlider();
        }

        #endregion
    }
}
