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
using System.Windows.Shapes;

namespace Core
{
    /// <summary>
    /// Interaction logic for BlockedKeyboardWindow.xaml
    /// </summary>
    public partial class BlockedKeyboardWindow : Window
    {
        #region Fields

        /// <summary>
        /// The content of the blocked trasparent window
        /// </summary>
        Window _Content;

        #endregion

        #region Constructor

        public BlockedKeyboardWindow(Window content, Rect elementPosition)
        {
            InitializeComponent();

            _Content = content;
            selectRect.Rect = elementPosition;

        }

        #endregion

        #region Methods

        /// <summary>
        /// Register all events
        /// </summary>
        private void RegisterEvents(bool register)
        {
            if (register) { }
            else
            { }

            RegisterContentEvents(register);
        }

        /// <summary>
        /// Register the events of content if it only exists
        /// </summary>
        private void RegisterContentEvents(bool register)
        {
            if (_Content != null)
            {
                if (register)
                {
                    _Content.Closed += OnContentClosed;
                    _Content.IsVisibleChanged += OnContentIsVisibleChanged;
                }
                else
                {
                    _Content.Closed -= OnContentClosed;
                    _Content.IsVisibleChanged -= OnContentIsVisibleChanged;
                }
            }
        }

        /// <summary>
        /// Show the Window and the Keyboard
        /// </summary>
        public void Open()
        {
            try
            {
                if (_Content != null)
                {
                    if (_Content.IsVisible)
                    {
                        _Content.Hide();
                    }

                    Show();
                    _Content.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                CoreLog.Instance.Append($"[{ex.Source}] - {ex.Message} - {ex.StackTrace}", CoreLogType.Error);

                Close();
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// The onload event
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            RegisterEvents(true);

            //try
            //{
            //    if (_Content != null)
            //    {
            //        _Content.ShowDialog();
            //    }
            //}
            //catch(Exception ex)
            //{
            //    Close();
            //}
        }

        /// <summary>
        /// When the content windows is closed the parent must be closed also
        /// </summary>
        private void OnContentClosed(object sender, EventArgs e)
        {
            Close();
        }

        private void OnContentIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_Content.Visibility != Visibility.Visible)
            {
                Close();
            }
        }


        /// <summary>
        /// The on closing event
        /// </summary>
        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            RegisterEvents(false);
        }

        #endregion
    }
}
