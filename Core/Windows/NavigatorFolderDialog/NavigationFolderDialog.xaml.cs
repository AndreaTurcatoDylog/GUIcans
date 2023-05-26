using Core.ResourceManager.Cultures;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Core
{
    /// <summary>
    /// Interaction logic for NavigationFolderDialog.xaml
    /// </summary>
    public partial class NavigationFolderDialog : Window
    {
        #region Members

        private NavigationFolderDialogViewModel _NavigationFolderDialogViewModel;

        #endregion

        #region Properties

        /// <summary>
        /// Get\Set the IsFavoriteDirty. 
        /// Specify whether the Favorite Folders collection is changed
        /// </summary>
        public bool IsFavoriteDirty { get; private set; }

        /// <summary>
        /// Get\Set the List of favorite folders
        /// </summary>
        public IList<FavoriteFolder> FavoriteFolders { get; private set; }

        /// <summary>
        /// Get\Set the Selected Folder
        /// </summary>
        public string SelectedFolderPath { get; private set; }

        /// <summary>
        /// Get\Set the Result of window
        /// </summary>
        public System.Windows.Forms.DialogResult Result { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public NavigationFolderDialog(string currentPath, string title, string domain, IList<FavoriteFolder> favoriteFolders)
        {
            InitializeComponent();

            Loaded += OnWindowLoaded;

            _NavigationFolderDialogViewModel = new NavigationFolderDialogViewModel(currentPath, title, domain, favoriteFolders);

            // Create the actions
            _NavigationFolderDialogViewModel.CloseAction = new Action(() => Close());
            _NavigationFolderDialogViewModel.ScrollToRigthEndAction = new Action(() => PathScroolViewer.ScrollToRightEnd());
             _NavigationFolderDialogViewModel.ScrollToHorizontalForwardAction = new Action(() => PathScroolViewer.ScrollToHorizontalOffset(PathScroolViewer.HorizontalOffset + 1));
            _NavigationFolderDialogViewModel.ScrollToHorizontalBackwardAction = new Action(() => PathScroolViewer.ScrollToHorizontalOffset(PathScroolViewer.HorizontalOffset - 1));

            // Set the Data Context
            DataContext = _NavigationFolderDialogViewModel;
        }

        /// <summary>
        /// Constructor 
        /// </summary>
        public NavigationFolderDialog(string currentPath, string domain, IList<FavoriteFolder> favoriteFolders)
            : this(currentPath, string.Empty, domain, favoriteFolders)
        { }

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

                FoldersListView.SelectionChanged += (s, ev) => _NavigationFolderDialogViewModel.SelectFolderCommand.Execute(s);
                PathScroolViewer.PreviewMouseDown += (s, ev) => _NavigationFolderDialogViewModel.NavigatorMouseDownCommand.Execute(ev);
                PathScroolViewer.ScrollChanged += (s, ev) => _NavigationFolderDialogViewModel.NavigatorScroolChangedCommand.Execute(ev);
                NavigatorItemsControl.PreviewMouseMove += (s, v) => _NavigationFolderDialogViewModel.NavigatorMouseMoveCommand.Execute(s);
            }
            else
            {
                Loaded -= OnWindowLoaded;
                Closing -= OnWindowClosing;

                FoldersListView.SelectionChanged -= (s, ev) => _NavigationFolderDialogViewModel.SelectFolderCommand.Execute(ev);
                PathScroolViewer.PreviewMouseDown -= (s, ev) => _NavigationFolderDialogViewModel.NavigatorMouseDownCommand.Execute(ev);
                PathScroolViewer.ScrollChanged -= (s, ev) => _NavigationFolderDialogViewModel.NavigatorScroolChangedCommand.Execute(ev);
                NavigatorItemsControl.PreviewMouseMove -= (s, ev) => _NavigationFolderDialogViewModel.NavigatorMouseMoveCommand.Execute(s);
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
            // Set the results
            IsFavoriteDirty = _NavigationFolderDialogViewModel.IsFavoriteDirty;
            FavoriteFolders = _NavigationFolderDialogViewModel.FavoriteFolders;
            SelectedFolderPath = _NavigationFolderDialogViewModel.SelectedFolderPath;
            Result = _NavigationFolderDialogViewModel.Result;

            // Unregister the events
            RegisterEvents(false);

            // Dispose the View Model
            _NavigationFolderDialogViewModel.Dispose();
        }

        #endregion
    }
}

