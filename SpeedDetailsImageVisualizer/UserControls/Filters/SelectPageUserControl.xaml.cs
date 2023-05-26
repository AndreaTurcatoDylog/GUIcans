using Business;
using Core;
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

namespace SpeedDetailsImageVisualizer
{
    /// <summary>
    /// Interaction logic for SelectPageUserControl.xaml
    /// </summary>
    public partial class SelectPageUserControl : DisposableUserControl
    {
        #region Properties

        /// <summary>
        /// Get\Set the CurrentPage. The number of the selected page
        /// </summary>
        public int CurrentPage
        {
            get { return (int)GetValue(CurrentPageProperty); }
            set { SetValue(CurrentPageProperty, value); }
        }

        /// <summary>
        /// Get the number of the Total Page.
        /// </summary>
        public int TotalPage
        {
            get { return (int)GetValue(TotalPageProperty); }
            set { SetValue(TotalPageProperty, value); }
        }

        /// <summary>
        /// GetSet the PreviusCommand
        /// </summary>
        public ICommand PreviusCommand
        {
            get { return (ICommand)GetValue(PreviusCommandProperty); }
            set { SetValue(PreviusCommandProperty, value); }
        }

        /// <summary>
        /// GetSet the NextCommand
        /// </summary>
        public ICommand NextCommand
        {
            get { return (ICommand)GetValue(NextCommandProperty); }
            set { SetValue(NextCommandProperty, value); }
        }

        #endregion

        #region Dependency Properties

        private static readonly DependencyProperty CurrentPageProperty =
         DependencyProperty.Register("CurrentPage", typeof(int), typeof(SelectPageUserControl), new PropertyMetadata(0));

        private static readonly DependencyProperty TotalPageProperty =
            DependencyProperty.Register("TotalPage", typeof(int), typeof(SelectPageUserControl), new PropertyMetadata(0));

        private static readonly DependencyProperty PreviusCommandProperty =
            DependencyProperty.Register("PreviusCommand", typeof(ICommand), typeof(SelectPageUserControl), new PropertyMetadata(null));

        private static readonly DependencyProperty NextCommandProperty =
            DependencyProperty.Register("NextCommand", typeof(ICommand), typeof(SelectPageUserControl), new PropertyMetadata(null));

        #endregion

        #region Events Handlers
       
        public event EventHandler ChangePageLostFocus;
        public event EventHandler<ChangePageEventArgs> CurrenPageChanged;

        #endregion  

        #region Constructor

        public SelectPageUserControl()
        {
            InitializeComponent();

            // Set the DataContext
            LayoutRoot.DataContext = this;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Register\Unregister the Events
        /// </summary>
        protected override void RegisterEvents(bool register)
        {
            if (register)
            {
                ChangePageLostFocus += OnChangePageLostFocus;
            }
            else
            {
                ChangePageLostFocus -= OnChangePageLostFocus;
            }
        }

        /// <summary>
        /// Trigger the ChangePageLostFocus Event
        /// </summary>
        protected void TriggerChangePageLostFocusEvent(object sender, EventArgs eventArgs)
        {
            ChangePageLostFocus?.Invoke(sender, eventArgs);
        }

        /// <summary>
        /// Rais the CurrentPage Event
        /// </summary>
        protected void TriggerCurrenPageEvent(object sender, ChangePageEventArgs changePageEventArgs)
        {
            CurrenPageChanged?.Invoke(sender, changePageEventArgs);
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when a key is UP on the ChangePageTextBox 
        /// </summary>
        private void OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (sender is TextBox changePageTextBox)
            {
                var text = changePageTextBox.Text;
                if (!string.IsNullOrEmpty(text))
                {
                    var isConverted = int.TryParse(changePageTextBox.Text, out int previusPage);
                    if (isConverted)
                    {
                        if (previusPage <= 0 || previusPage > TotalPage)
                        {
                            changePageTextBox.Text = CurrentPage.ToString();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Occurs when the ChangePageTextBox Lost the Focus
        /// </summary>
        private void OnChangePageLostFocus(object sender, EventArgs e)
        {
            if (sender is TextBox changePageTextBox)
            {
                changePageTextBox.Text = CurrentPage.ToString();
            }
        }

        /// <summary>
        /// Occurs when the Change Page TextBox lost the focus
        /// </summary>
        protected void OnChangePageTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            TriggerChangePageLostFocusEvent(sender, e);
        }

        /// <summary>
        /// Occurs when a key is pressed on the ChangePageTextBox 
        /// </summary>
        protected void OnChangePageKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox changePageTextBox && e.Key == Key.Enter)
            {
                var converted = int.TryParse(changePageTextBox.Text, out int currentPage);
                if (converted)
                {
                    // Trigger the CurrentPage Event
                    TriggerCurrenPageEvent(sender, new ChangePageEventArgs(currentPage));
                }
            }
        }

        #endregion
    }
}
