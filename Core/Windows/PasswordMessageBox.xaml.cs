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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Core
{
    /// <summary>
    /// Interaction logic for PasswordMessageBox.xaml
    /// </summary>
    public partial class PasswordMessageBox : AdjustableWindow
    {
        #region Constants

        private readonly string AsteriskIconName = "DT_ASTERISK_002";
        private readonly string AsteriskColor = "GradiantSteelBlue";

        #endregion

        #region Fields

        /// <summary>
        /// The keyboard
        /// </summary>
        private StandardKeyboard _StandardKeyboard;

        /// <summary>
        /// The password edited
        /// </summary>
        private string _Password;

        #endregion

        #region Properties

        /// <summary>
        /// The list of images. When the in password fild a character is added\delete a new image is added\delete
        /// </summary>
        public ObservableCollection<Path> Asterisks { get; set; }

        #endregion

        #region Constructor

        public PasswordMessageBox()
        {
            InitializeComponent();

            _Password = string.Empty;
            Asterisks = new ObservableCollection<Path>();

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
            {
                UserIdTextBox.KeyboardClosed += OnUserIdTextBoxKeyboardClosed;
                UserIdTextBox.KeyboardExit += OnUserIdTextBoxKeyboardExit;
                UserIdTextBox.ComponentIsLoaded += OnUserIdTextBoxComponentIsLoaded;
            }
            else
            {
                UserIdTextBox.KeyboardClosed -= OnUserIdTextBoxKeyboardClosed;
                UserIdTextBox.KeyboardExit -= OnUserIdTextBoxKeyboardExit;
                UserIdTextBox.ComponentIsLoaded -= OnUserIdTextBoxComponentIsLoaded;
            }
        }

        /// <summary>
        /// Create a new Asterisk
        /// </summary>
        private Path CreateNewAsterik()
        {
            var asterisk = new Path()
            {
                Stroke = Brushes.Black,
                Fill = (Brush)Application.Current.FindResource(AsteriskColor),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                RenderTransformOrigin = new Point(0.5, 0.5),
                Stretch = Stretch.Fill,
                Data = (Geometry)Application.Current.FindResource(AsteriskIconName),
                StrokeThickness = 0,
                Width = 25,
                Height = 25,
                Margin = new Thickness(5, 0, 0, 0)
            };

            return asterisk;
        }

        /// <summary>
        /// Set the background of Asteriks border.
        /// Depends whether a password is editing or not
        /// </summary>
        private void SetPasswordBorder(bool isActive)
        {
            if (isActive)
            {
                PasswordBorder.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xFE, 0xC0, 0x00));
                PasswordBorder.Effect = new DropShadowEffect
                {
                    Direction = -45,
                    BlurRadius = 4,
                    ShadowDepth = 4,
                    Opacity = 0.5
                };
            }
            else
            {
                PasswordBorder.Background = Brushes.White;
                PasswordBorder.Effect = null;
            }
        }

        private void ShowKeyaboard()
        {
            SetPasswordBorder(true);

            // Listen to key pressed
            _StandardKeyboard.KeyPressed += OnKeyboardKeyPressed;

            //Open the keyboard
            _StandardKeyboard.ShowDialog();

            // Stop to listen to keypressed
            _StandardKeyboard.KeyPressed -= OnKeyboardKeyPressed;

            SetPasswordBorder(false);
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
            _StandardKeyboard = StandardKeyboard.Instance;

            //SetFocus(UserIdTextBox);
        //    UserIdTextBox.OpenKeyBoard();
        }

        /// <summary>
        /// Occurs when the user id textbox is fully loaded
        /// </summary>
        private void OnUserIdTextBoxComponentIsLoaded(object sender, EventArgs e)
        {
            SetFocus(UserIdTextBox);
            UserIdTextBox.Activate();
        }

        /// <summary>
        /// Set the focus of specificated component
        /// </summary>
        private void SetFocus(IInputElement inputElement)
        {
            inputElement.Focus();
            FocusManager.SetFocusedElement(this, inputElement);
        }

        /// <summary>
        /// Occurs when the keyboard exits  (it is closed without save value)
        /// </summary>
        private void OnUserIdTextBoxKeyboardExit(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// When the keyboard on user id is closed with save the focus is moved on the password field
        /// </summary>
        private void OnUserIdTextBoxKeyboardClosed(object sender, EventArgs e)
        {
            SetFocus(PasswordBorder);
            ShowKeyaboard();
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

                    // Update Password
                    _Password = $"{_Password}{args.Content}";

                    // Add new Asterisk
                    var asterisk = CreateNewAsterik();
                    Asterisks.Add(asterisk);
                    break;

                case KeyboardButtonType.Backspace:

                    // Update Password
                    if (_Password.Length > 0)
                    {
                        _Password = _Password.Remove(_Password.Length - 1, 1);

                        // Delete last Asterisk
                        RemoveLastAsterisk();
                    }

                    break;

                case KeyboardButtonType.Enter:

                    // Exit from password window
                    Close();
                    break;

                case KeyboardButtonType.Exit:

                    // Exit from password window
                    _Password = string.Empty;
                    Close();

                    break;
            }
        }

        #endregion
    }
}
