using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Core
{
    /// <summary>
    /// Interaction logic for CustomMessageBox.xaml
    /// </summary>
    public partial class FolderCredentials : Window
    {
        #region Members

        private FolderCredentialsViewModel _FolderCredentialsViewModel;

        #endregion

        #region Properties

        /// <summary>
        /// The result of operation
        /// </summary>
        public System.Windows.Forms.DialogResult Result { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        public FolderCredentials(FolderCredentialsViewModel folderCredentialsViewModel)
        {
            InitializeComponent();

            _FolderCredentialsViewModel = folderCredentialsViewModel;

            // Create the actions
            _FolderCredentialsViewModel.CloseAction = new Action(() => Close());

            // Set the Data Context
            DataContext = folderCredentialsViewModel;
        }       

        #endregion

        #region Methods

        /// <summary>
        /// Register\Unregister events
        /// </summary>
        private void RegisterEvents(bool register)
        {
            if (register)
            {
                Closing += OnWindowClosing;
            }
            else
            {
                Loaded -= OnWindowLoaded;
                Closing -= OnWindowClosing;
            }
        }

        /// <summary>
        /// Show the message box in blocked or no blocked style
        /// </summary>
        public void ShowWindow(bool isBlockedWindow = true)
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

            Result = _FolderCredentialsViewModel.Result.WindowResult;
        }       

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the custom message box is loaded
        /// </summary>
        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            RegisterEvents(true);
        }

        /// <summary>
        /// Occurs when the custom message box is closing
        /// </summary>
        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            RegisterEvents(false);
        }

        #endregion
    }
}
