using System;
using System.Collections.Generic;
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
    /// Interaction logic for IpAddressInput.xaml
    /// </summary>
    public partial class IpAddressInput : DisposableUserControlBase
    {
        #region Fields

        /// <summary>
        /// A list of all Ip TextBoxes in the user control
        /// </summary>
        private IList<NumericTextBoxKeyboard> _IpTextBoxes;

        /// <summary>
        /// The index of the Text box in the iswriting mode.
        /// Used to shift from Text box to another when keyboard decimal separator is pressed
        /// </summary>
        private byte _CurrentTextBoxIndex;

        /// <summary>
        /// Specify if the keyboard is not closes becouse the focus moved to another ip textbox
        /// </summary>
        private bool _FocuseMoved;

        #endregion

        #region Constructor

        public IpAddressInput()
        {
            InitializeComponent();

            _IpTextBoxes = new List<NumericTextBoxKeyboard>()
            {
                FirstValueNumericTextBox,
                SecondValueNumericTextBox,
                ThirdValueNumericTextBox,
                FourthValueNumericTextBox
            };

            _CurrentTextBoxIndex = 0;

            LayoutRoot.DataContext = this;
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
                //Loaded += IpAddressInput_Loaded;
            }
            else
            {
                //Loaded -= IpAddressInput_Loaded;
            }

            RegisterTextBoxesEvents(register);
        }

        private void IpAddressInput_Loaded(object sender, RoutedEventArgs e)
        {
            // Set the offsets
            var firstValueNumericTextBoxAbsolutePlacement = FirstValueNumericTextBox.GetAbsolutePlacement(true);
            var offeset = firstValueNumericTextBoxAbsolutePlacement.Width + 16;

            FirstValueNumericTextBox.OffsetX = (offeset)*3;
            SecondValueNumericTextBox.OffsetX = (offeset) * 2;
            ThirdValueNumericTextBox.OffsetX = (offeset) * 1;
        }

        /// <summary>
        /// Register events for all iptextboxes in user control
        /// </summary>
        private void RegisterTextBoxesEvents(bool register)
        {
            foreach(var ipTextBox in _IpTextBoxes)
            {
                if (register)
                {
                    ipTextBox.KeyboardKeyPressed += OnKeyboardKeyPressed;
                    ipTextBox.KeyboardClosed += OnKeyboardClosed;
                    ipTextBox.PreviewMouseDown += OnIpTextBoxPreviewMouseDown;
                }
                else
                {
                    ipTextBox.KeyboardKeyPressed -= OnKeyboardKeyPressed;
                    ipTextBox.KeyboardClosed -= OnKeyboardClosed;
                    ipTextBox.PreviewMouseDown -= OnIpTextBoxPreviewMouseDown;
                }
            }
        }

        private void OnIpTextBoxPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var textBox = (sender as NumericTextBoxKeyboard);
            _CurrentTextBoxIndex = (byte) _IpTextBoxes.IndexOf(textBox);
        }

        #endregion

        #region Events

        /// <summary>
        /// When the keyboard is closed by clicking decimal separator the focus move on the next ip textbox
        /// </summary>
        private void OnKeyboardClosed(object sender, EventArgs e)
        {
            if (_FocuseMoved && _CurrentTextBoxIndex < 3)
            {
                var ipTextBox = _IpTextBoxes[_CurrentTextBoxIndex];
                ipTextBox.AdjustNumericValue();

                _CurrentTextBoxIndex++;

                ipTextBox = _IpTextBoxes[_CurrentTextBoxIndex];
                ipTextBox.Activate();
            }

            _FocuseMoved = false;
        }

        /// <summary>
        /// Set the text of the text box in according to kayboard's button pressed
        /// </summary>
        private void OnKeyboardKeyPressed(object sender, EventArgs e)
        {
            var args = (KeyboardButtonEventArgs)e;
            var textBox = (sender as NumericTextBoxKeyboard);
            switch (args.KeyboardButtonType)
            {
                case KeyboardButtonType.DecimalSeparator:
                    _FocuseMoved = true;
                    var ipTextBox = _IpTextBoxes[_CurrentTextBoxIndex];
                    ipTextBox.CloseKeyboard(true);
                    break;
            }
        }

        #endregion
    }
}
