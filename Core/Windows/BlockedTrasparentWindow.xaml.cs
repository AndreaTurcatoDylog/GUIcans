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
    /// Interaction logic for BlockedTrasparentWindow.xaml
    /// </summary>
    public partial class BlockedTrasparentWindow : Window
    {
        #region Fields

        /// <summary>
        /// The content of the blocked trasparent window
        /// </summary>
        Window _Content;

        #endregion

        #region Constructors

        public BlockedTrasparentWindow(Window content)
        {
            InitializeComponent();

            _Content = content;
        }

        public BlockedTrasparentWindow()
            :this(null)
        {}

        #endregion

        #region Methods

        /// <summary>
        /// Register all events
        /// </summary>
        private void RegisterEvents(bool register)
        {
            if (register){}
            else
            {}

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
                }
                else
                {
                    _Content.Closed -= OnContentClosed;
                }
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

            try
            {
                if (_Content != null)
                {
                    _Content.ShowDialog();
                }
            }
            catch(Exception ex)
            {
                CoreLog.Instance.Append($"[{ex.Source}] - {ex.Message} - {ex.StackTrace}", CoreLogType.Error);

                Close();
            }

        }

        /// <summary>
        /// When the content windows is closed the parent must be closed also
        /// </summary>
        private void OnContentClosed(object sender, EventArgs e)
        {
            Close();
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
