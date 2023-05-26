using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Core
{
    /// <summary>
    /// Interaction logic for CustomMessageBox.xaml
    /// </summary>
    public partial class CustomMessageBox : Window
    {
        #region Properties

        public MessageBoxResult Result { get; private set; }

        /// <summary>
        /// Get\Set the RectangleTitleColorFill
        /// </summary>
        public Brush RectangleTitleColorFill
        {
            get { return (Brush)GetValue(RectangleTitleColorFillProperty); }
            set { SetValue(RectangleTitleColorFillProperty, value); }
        }

        /// <summary>
        /// Get\Set the RectangleBodyColor
        /// </summary>
        public Brush RectangleBodyColor
        {
            get { return (Brush)GetValue(RectangleBodyColorProperty); }
            set { SetValue(RectangleBodyColorProperty, value); }
        }

        /// <summary>
        /// Get\Set the CaptionMessage
        /// </summary>
        public string CaptionMessage
        {
            get { return (string)GetValue(CaptionMessageProperty); }
            set { SetValue(CaptionMessageProperty, value); }
        }

        /// <summary>
        /// Get\Set the BodyMessage
        /// </summary>
        public string BodyMessage
        {
            get { return (string)GetValue(BodyMessageProperty); }
            set { SetValue(BodyMessageProperty, value); }
        }

        /// <summary>
        /// Get\Set the List of buttons
        /// </summary>
        public ObservableCollection<Button> Buttons
        {
            get { return (ObservableCollection<Button>)GetValue(ButtonsProperty); }
            set { SetValue(ButtonsProperty, value); }
        }

        #endregion

        #region Dependency Properties

        private static readonly DependencyProperty RectangleTitleColorFillProperty =
         DependencyProperty.Register("RectangleTitleColorFill", typeof(Brush), typeof(CustomMessageBox));

        private static readonly DependencyProperty RectangleBodyColorProperty =
         DependencyProperty.Register("RectangleBodyColor", typeof(Brush), typeof(CustomMessageBox));

        private static readonly DependencyProperty CaptionMessageProperty =
          DependencyProperty.Register("CaptionMessage", typeof(string), typeof(CustomMessageBox));

        private static readonly DependencyProperty BodyMessageProperty =
         DependencyProperty.Register("BodyMessage", typeof(string), typeof(CustomMessageBox));

        private static readonly DependencyProperty ButtonsProperty =
         DependencyProperty.Register("Buttons", typeof(ObservableCollection<Button>), typeof(CustomMessageBox));

        #endregion

        #region Constructor

        /// <summary>
        /// Create a new custom message box
        /// </summary>
        /// <param name="caption">The caption of window </param>
        /// <param name="message"> The body message of message box </param>
        /// <param name="color"> The color of message box </param>
        /// <param name="buttons"> The list of buttons in the message box </param>
        public CustomMessageBox(string caption, string message, Color color, ObservableCollection<Button> buttons, Window owner = null)
        {
            InitializeComponent();            
            Topmost = true;

            // Set the Owner
            Owner = (owner == null)? Application.Current.MainWindow
                                   : Owner = owner;            

            //Setup properties
            Setup(caption, message, buttons);
            SetupColor(color);

            // Set loaded event
            Loaded += OnCustomMessageBoxLoaded;

            // Set datacontex
            LayoutRoot.DataContext = this;
        }

        /// <summary>
        /// Create a new custom message box
        /// </summary>
        /// <param name="caption"> The caption of window </param>
        /// <param name="message"> The body message of message box </param>
        /// <param name="color"> The color of message box </param>
        /// <param name="messageBoxButtonsType"> Exists several standard button types. Every type has standard buttons </param>
        public CustomMessageBox(string caption, string message, Color color, MessageBoxButtonsType messageBoxButtonsType, Window owner = null)
            : this(caption, message, color, MessageBoxButtonsFactory.Get(messageBoxButtonsType), owner)
        { }


        /// <summary>
        /// Create a new custom message box
        /// </summary>
        /// <param name="caption">The caption of window </param>
        /// <param name="message"> The body message of message box </param>
        /// <param name="styleName"> The style of message box </param>
        /// <param name="buttons"> The list of buttons in the message box </param>
        public CustomMessageBox(string caption, string message, string styleName, ObservableCollection<Button> buttons, Window owner = null)
        {
            InitializeComponent();

            // Set the Owner
            Owner = (owner == null) ? Application.Current.MainWindow
                                   : Owner = owner;

            Setup(caption, message, buttons);
            SetupStyle(styleName);

            // Set loaded event
            Loaded += OnCustomMessageBoxLoaded;

            // Set datacontex
            LayoutRoot.DataContext = this;
        }

        /// <summary>
        /// Create a new custom message box
        /// </summary>
        /// <param name="caption">The caption of window </param>
        /// <param name="message"> The body message of message box </param>
        /// <param name="color"> The color of message box </param>
        /// <param name="messageBoxButtonsType"> Exists several standard button types. Every type has standard buttons </param>
        public CustomMessageBox(string caption, string message, string styleName, MessageBoxButtonsType messageBoxButtonsType, Window owner = null)
            : this(caption, message, styleName, MessageBoxButtonsFactory.Get(messageBoxButtonsType), owner)
        {}


        /// <summary>
        ///  Create a new custom message box
        /// </summary>
        /// <param name="standardTypes"> Exists several standard type. Every type set the style of message box </param>
        /// <param name="message"> The body message </param>
        /// <param name="buttons"> the list of buttons </param>
        public CustomMessageBox(MessageBoxType standardTypes, string message, ObservableCollection<Button> buttons, Window owner = null)
        {
            InitializeComponent();

            // Set the Owner
            Owner = (owner == null) ? Application.Current.MainWindow
                                   : Owner = owner;

            // Setup(message, buttons);
            BodyMessage = message;
            Buttons = buttons;
            Style = MessageBoxTypeFactory.Get(standardTypes);

            // Set loaded event
            Loaded += OnCustomMessageBoxLoaded;

            // Set datacontex
            LayoutRoot.DataContext = this;
        }

        /// <summary>
        ///  Create a new custom message box
        /// </summary>
        /// <param name="standardTypes"> Exists several standard type. Every type set the style of message box </param>
        /// <param name="message"> The body message </param>
        /// <param name="messageBoxButtonsType"> Exists several standard button types. Every type has standard buttons </param>
        public CustomMessageBox(MessageBoxType standardTypes, string message, MessageBoxButtonsType messageBoxButtonsType, Window owner = null)
            : this(standardTypes, message, MessageBoxButtonsFactory.Get(messageBoxButtonsType), owner)
        {}

        #endregion

        #region Methods

        /// <summary>
        /// Register\Unregister events
        /// </summary>
        private void RegisterEvents(bool register)
        {
            if (register)
            {
                Closing += OnCustomMessageBoxClosing;
            }
            else
            {
                Loaded -= OnCustomMessageBoxLoaded;
                Closing -= OnCustomMessageBoxClosing;
            }

            RegisterButtonEvent(register);
        }

        /// <summary>
        /// Register\Unregister the event associated to buttons
        /// </summary>
        private void RegisterButtonEvent(bool register)
        {
            if (Buttons != null)
            {
                foreach (var button in Buttons)
                {
                    if (register)
                    {
                        button.Click += new RoutedEventHandler(OnButtonClick);
                    }
                    else
                    {
                        button.Click -= new RoutedEventHandler(OnButtonClick);
                    }
                }
            }
        }

        /// <summary>
        /// Set the values in construcotr of some parameters
        /// </summary>
        /// <param name="caption"> The caption of message box </param>
        /// <param name="message"> The body message of message box </param>
        /// <param name="buttons"> The list of buttons of message box </param>
        private void Setup(string caption, string message, ObservableCollection<Button> buttons)
        {
            CaptionMessage = caption;
            BodyMessage = message;
            Buttons = buttons;
        }

        /// <summary>
        /// Set the values in construcotr of some parameters
        /// </summary>
        /// <param name="message"> The body message of message box </param>
        /// <param name="buttons"> The list of buttons of message box </param>
        private void Setup(string message, ObservableCollection<Button> buttons)
        {
            Setup(string.Empty, message, buttons);
        }

        /// <summary>
        /// Set the color of message Box
        /// </summary>
        private void SetupColor(Color color)
        {
            var solidColorBrush = new SolidColorBrush(color);
            RectangleTitleColorFill = solidColorBrush;
            RectangleBodyColor = solidColorBrush;
        }

        /// <summary>
        /// Set the style of message Box
        /// </summary>
        private void SetupStyle(string styleName)
        {
            var style = Application.Current.FindResource(styleName) as Style;
            if (style != null)
            {
                this.Style = style;
            }
        }

        /// <summary>
        /// Show the message box in blocked or no blocked style
        /// </summary>
        public void ShowMessageBox(bool isBlockedWindow = true)
        {
            if (isBlockedWindow)
            {
                var blockedTrasparentWindow = new BlockedTrasparentWindow(this);
                blockedTrasparentWindow.ShowDialog();
            }
            else
            {
                ShowDialog();
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when a  button is pressed. Close the custom message box and set its Result
        /// </summary>
        protected void OnButtonClick(object sender, EventArgs e)
        {
            // Set the message box result
            var buttonResult = (sender as ImageButton).ButtonResult;
            switch (buttonResult)
            {
                case ButtonResult.None: Result = MessageBoxResult.None; break;
                case ButtonResult.Yes: Result = MessageBoxResult.Yes; break;
                case ButtonResult.No: Result = MessageBoxResult.No; break;
                case ButtonResult.Ok: Result = MessageBoxResult.OK; break;
                case ButtonResult.Cancel: Result = MessageBoxResult.Cancel; break;
                case ButtonResult.Back: Result = MessageBoxResult.Back; break;
            }

            // Close the message box
            Close();
        }

        /// <summary>
        /// Occurs when the custom message box is loaded
        /// </summary>
        private void OnCustomMessageBoxLoaded(object sender, RoutedEventArgs e)
        {
            RegisterEvents(true);
        }

        /// <summary>
        /// Occurs when the custom message box is closing
        /// </summary>
        private void OnCustomMessageBoxClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            RegisterEvents(false);
        }

        #endregion
    }
}
