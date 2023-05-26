using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Core
{
    /// <summary>
    /// Interaction logic for PinMessageBox.xaml
    /// </summary>
    public partial class PinMessageBox : AdjustableWindow
    {
        #region Constants

        private readonly byte NumberOfAsterisks = 4;
        private readonly string AsteriskIconName = "DT_ASTERISK_002";
        private readonly string AsteriskActiveColor = "GradiantRed";

        #endregion

        #region Fields

        /// <summary>
        /// The keyboard
        /// </summary>
        private NumericKeyboard _NumericKeyboard;

        /// <summary>
        /// The pin edited
        /// </summary>
        private string _Pin;

        /// <summary>
        /// Specify the current asterisk that will become active or inactive
        /// </summary>
        private int _CurrentAsteriskIndex;

        #endregion

        #region Properties

        /// <summary>
        /// The list of images. When the in password fild a character is added\delete a new image is added\delete
        /// </summary>
        public ObservableCollection<Path> Asterisks { get; set; }

        #endregion

        #region Constructor

        public PinMessageBox()
        {
            InitializeComponent();

            _Pin = string.Empty;
            Asterisks = new ObservableCollection<Path>();

            _CurrentAsteriskIndex = -1;

            // The loaded event
            Loaded += OnLoaded;

            // Set datacontex
            LayoutRoot.DataContext = this;
        }

        #endregion
        #region Methods

        /// <summary>
        /// Register\Unregister events
        /// </summary>
        protected override void RegisterEvents(bool register)
        {
            base.RegisterEvents(register);

            if (register)
            {}
            else
            {}
        }

        /// <summary>
        /// Create a new Asterisk
        /// </summary>
        private void CreateNewAsteriks(int numberOfAsterisks, bool isActive)
        {
            Brush backGround = isActive ? backGround = (Brush)Application.Current.FindResource(AsteriskActiveColor)
               : backGround = Brushes.Gray;

            for (var index = 0; index < numberOfAsterisks; index++) {
                var asterisk = new Path()
                {
                    Stroke = Brushes.Black,
                    Fill = backGround,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    RenderTransformOrigin = new Point(0.5, 0.5),
                    Stretch = Stretch.Fill,
                    Data = (Geometry)Application.Current.FindResource(AsteriskIconName),
                    StrokeThickness = 0,
                    Width = 45,
                    Height = 45,        
                    Margin = new Thickness(5, 0, 0, 0)
                };

                // Add item to collection
                Asterisks.Add(asterisk);
            }
        }

        /// <summary>
        /// Change the state of the asterisk specificated by index position in Asterisks collection
        /// </summary>
        /// <param name="isActive"> True: Asterisk is active - False: Asterisk is inactive </param>
        /// <param name="index">The position in Asterisks collection </param>
        private void ChangeAsteriskState(bool isActive, int index)
        {
            Brush backGround = isActive ? backGround = (Brush)Application.Current.FindResource(AsteriskActiveColor)
              : backGround = Brushes.Gray;

            var asterisk = Asterisks.ElementAt(index);

            asterisk.Fill = backGround;
        }

        private void ShowKeyaboard()
        {
           // var borderPlacement = PasswordBorder.GetAbsolutePlacement(true);
            _NumericKeyboard.Show();
           _NumericKeyboard.SetPositionOnBottom();

            // Listen to key pressed
            _NumericKeyboard.KeyPressed += OnKeyboardKeyPressed;

            //Open the keyboard
            _NumericKeyboard.Hide();
            _NumericKeyboard.ShowDialog();

            // Stop to listen to keypressed
            _NumericKeyboard.KeyPressed -= OnKeyboardKeyPressed;
        }

        /// <summary>
        /// Remove the last Asterisk
        /// </summary>
        private void RemoveLastAsterisk()
        {
            Asterisks.Remove(Asterisks.Last());
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the window loaded
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Get Keyboard
            _NumericKeyboard = NumericKeyboard.Instance;

            // Create Asterisks
            CreateNewAsteriks(NumberOfAsterisks, false);

            // Show keybaord
            ShowKeyaboard();
        }

        /// <summary>
        /// Occurs when the keyboard exits  (it is closed without save value)
        /// </summary>
        private void OnUserIdTextBoxKeyboardExit(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Draw an Astrisk if password is editing
        /// </summary>
        protected virtual void OnKeyboardKeyPressed(object sender, EventArgs e)
        {
            var args = (KeyboardButtonEventArgs)e;

            switch (args.KeyboardButtonType)
            {
                case KeyboardButtonType.Normal:
                case KeyboardButtonType.DecimalSeparator:

                    if (_CurrentAsteriskIndex < NumberOfAsterisks-1)
                    {
                        // Update Password
                        _Pin = $"{_Pin}{args.Content}";

                        // Change asterisk state
                        _CurrentAsteriskIndex++;
                        ChangeAsteriskState(true, _CurrentAsteriskIndex);
                    }
                    break;

                case KeyboardButtonType.Backspace:

                    // Update Password
                    if (_Pin.Length > 0)
                    {
                        _Pin = _Pin.Remove(_Pin.Length - 1, 1);

                        // Change asterisk state
                        ChangeAsteriskState(false, _CurrentAsteriskIndex);
                        _CurrentAsteriskIndex--;
                    }

                    break;

                case KeyboardButtonType.Enter:

                    // Exit from password window
                    Close();
                    break;

                case KeyboardButtonType.Exit:

                    // Exit from password window
                    _Pin = string.Empty;
                    Close();

                    break;
            }
        }

        #endregion
    }
}
