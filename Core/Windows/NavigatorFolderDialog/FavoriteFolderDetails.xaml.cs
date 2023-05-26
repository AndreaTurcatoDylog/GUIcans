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
    public partial class FavoriteFolderDetails : Window
    {
        #region Members

        FavoriteFolderDetailsViewModel _FavoriteFolderDetailsViewModel;

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
        public FavoriteFolderDetails(FavoriteFolderDetailsViewModel favoriteFolderDetailsViewModel)
        {
            InitializeComponent();

            _FavoriteFolderDetailsViewModel = favoriteFolderDetailsViewModel;

            // Create the action
            _FavoriteFolderDetailsViewModel.CloseAction = new Action(() => Close());

            // Set the Data Context
            DataContext = favoriteFolderDetailsViewModel;
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

            Result = _FavoriteFolderDetailsViewModel.Result;
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
