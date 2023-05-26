using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for CalendarKeyboard.xaml
    /// </summary>
    public partial class CalendarKeyboard : KeyboardBase
    {
        #region Fields

        private static CalendarKeyboard _Instance;

        #endregion

        #region Properties
        /// <summary>
        /// Singleton
        /// </summary>
        public static CalendarKeyboard Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new CalendarKeyboard();
                }

                return _Instance;
            }
        }

        #endregion

        #region Events

        public event EventHandler OnDateChanged;
        public event EventHandler OnDateResetted;

        #endregion

        #region Constructor

        public CalendarKeyboard()
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
            return true;
        }

        #endregion

        #region Events

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
        private  void OnEnterButtonClick(object sender, MouseButtonEventArgs e)
        {
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
        /// Occurs when the exit button is clicked
        /// </summary>
        private void OnBackSpaceTouchDown(object sender, TouchEventArgs e)
        {
            OnDateResetted?.Invoke(this, e);
            e.Handled = false;
        }

        /// <summary>
        /// Occurs when the exit button is clicked
        /// </summary>
        private void OnBackSpaceButtonClick(object sender, MouseButtonEventArgs e)
        {
            OnDateResetted?.Invoke(this, e);
            e.Handled = false;
        }

        /// <summary>
        /// Occurs when the Date is changed
        /// </summary>
        private void SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                OnDateChanged?.Invoke(this, e);
                e.Handled = true;
            }
            catch(Exception)
            {

            }
        }

        /// <summary>
        /// Used to fixes the focus issue (double click in Keyboard button after a date is choosen).
        /// Released the mouse focus when the clicked UIElement was a calendar day
        /// </summary>
        private void OnCalendarGotMouseCapture(object sender, MouseEventArgs e)
        {
            try
            {

                UIElement originalElement = e.OriginalSource as UIElement;
                if (originalElement is CalendarDayButton || originalElement is CalendarItem)
                {
                    originalElement.ReleaseMouseCapture();
                }
            }
            catch(Exception)
            {

            }
        }

        /// <summary>
        /// Used to fixes the focus issue (double touch in Keyboard button after a date is choosen).
        /// Released the touch focus when the clicked UIElement was a calendar day
        /// </summary>
        private void OnCalendarGotTouchCapture(object sender, TouchEventArgs e)
        {
            try
            {
                UIElement originalElement = e.OriginalSource as UIElement;
                if (originalElement is CalendarDayButton || originalElement is CalendarItem)
                {
                    originalElement.ReleaseAllTouchCaptures();
                }
            }
            catch(Exception)
            {

            }
        }

        #endregion
    }
}
