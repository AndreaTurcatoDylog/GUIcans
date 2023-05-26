using Core.ResourceManager.Cultures;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Core
{
    public class NavigationFolderDialogViewModel: ViewModelBase, IDisposable
    {
        #region Constants

        public readonly int NumberOfItemInPage = 16;
        public readonly string NavigatorHomeButtonStyle = "NavigatorHomeButtonStyle";
        public readonly string NavigatorButtonStyle = "NavigatorButtonStyle";
        public readonly string LabelHome = "PC";

        #endregion

        #region Members

        private string _Title;
        private string _FullPathOfSelectedItem;
        private RootItem _SelectedRoot;
        private ObservableCollection<ImageButton> _NavigatorButtons;
        private List<RootItem> _RootItems;
        private bool _ExistFolders;
        private string _CurrentPath;
        private ObservableCollection<NavigationDialogItem> _Items;
        private int _PageIndex;
        private int _NumberOfElements;
        private bool _IsTryConnecting;
        private string _Domain;
        private string _GoToPath;
        private bool _ScroolChanged;
        private Point _OldMousePosition;
        private List<string> _NavigatorButtonLabels;
        private INavigatorFolder _NavigatorFolder;
        private NavigationDialogItem _SelectedFolder;
        private CancellationTokenSource _CancellationTokenSource;

        // Specify whether the set of parameters get from constructor
        private bool _IsFavoriteLoad;

        // Prevent the multiple call of the methods to load folders
        private bool _IsJustLoaded;

        #endregion

        #region Event Handler

        public event EventHandler RootChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Get\Set the Result of window
        /// </summary>
        public System.Windows.Forms.DialogResult Result { get; private set; }

        /// <summary>
        /// Get\Set the selected folder
        /// </summary>
        public string SelectedFolder { get; private set; }

        /// <summary>
        /// Get\Set the SelectedPath
        /// </summary>
        public string SelectedFolderPath { get; private set; }

        /// <summary>
        /// Get\Set the List of favorite folders
        /// </summary>
        public IList<FavoriteFolder> FavoriteFolders { get; private set; }

        /// <summary>
        /// Get\Set the IsFavoriteDirty. 
        /// Specify whether the Favorite Folders collection is changed
        /// </summary>
        public bool IsFavoriteDirty { get; private set; }

        /// <summary>
        /// Get\Set the Title
        /// </summary>
        public string Title
        {
            get { return _Title; }
            set
            {
                _Title = value;
                OnPropertyChanged("Title");
            }
        }

        /// <summary>
        /// Get\Set the FullPathOfSelectedItem
        /// </summary>
        public string FullPathOfSelectedItem
        {
            get { return _FullPathOfSelectedItem; }
            set
            {
                _FullPathOfSelectedItem = value;
                OnPropertyChanged("FullPathOfSelectedItem");
            }
        }

        /// <summary>
        /// Get\Set the Selected Root Item
        /// </summary>
        public RootItem SelectedRoot
        {
            get { return _SelectedRoot; }
            set
            {
                _SelectedRoot = value;
                OnPropertyChanged("SelectedRoot");

                // Fire the event
                RootChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Get\Set the List of Roots (ThisPc, Net, Favorite...)
        /// </summary>
        public List<RootItem> RootItems
        {
            get { return _RootItems; }
            set
            {
                _RootItems = value;
                OnPropertyChanged("RootItems");
            }
        }
        
        /// <summary>
        /// Get\Set the Collection of Navigator buttons
        /// </summary>
        public ObservableCollection<ImageButton> NavigatorButtons
        {
            get { return _NavigatorButtons; }
            set
            {
                _NavigatorButtons = value;
                OnPropertyChanged("NavigatorButtons");
            }
        }

        /// <summary>
        /// Get\Set the Exists Folders. 
        /// Specify whether exists sub folders in the current path
        /// </summary>
        public bool ExistFolders
        {
            get { return _ExistFolders; }
            set
            {
                _ExistFolders = value;
                OnPropertyChanged("ExistFolders");
            }
        }

        /// <summary>
        /// Get\Set the Current Path
        /// </summary>
        public string CurrentPath
        {
            get { return _CurrentPath; }
            set
            {
                _CurrentPath = value;
                OnPropertyChanged("CurrentPath");
            }
        }

        /// <summary>
        /// Get\Set the Items. There are the Folders, PC ecc shown in the list
        /// </summary>
        public ObservableCollection<NavigationDialogItem> Items
        {
            get { return _Items; }
            set
            {
                _Items = value;
                OnPropertyChanged("Items");
            }
        }


        /// <summary>
        /// Get the Index of choosen Page.
        /// The change in the PageIndex triggers the loading of images
        /// </summary>
        public int PageIndex
        {
            get { return _PageIndex; }
            set
            {
                _PageIndex = value;
                OnPropertyChanged("PageIndex");

                PageIndexChangedAsync();
            }
        }

        /// <summary>
        /// Get\Set the Number of elements. It is the total number of the elments (not filtered)
        /// Used in PaginationUserControl
        /// </summary>
        public int NumberOfElements
        {
            get { return _NumberOfElements; }
            set
            {
                _NumberOfElements = value;
                OnPropertyChanged("NumberOfElements");
            }
        }

        /// <summary>
        /// Get\Set the is try connecting.
        /// Specify if it is trying to connect to some path.
        /// </summary>
        public bool IsTryConnecting
        {
            get { return _IsTryConnecting; }
            set
            {
                _IsTryConnecting = value;
                OnPropertyChanged("IsTryConnecting");
            }
        }

        /// <summary>
        /// Get\Set the domain
        /// </summary>
        public string Domain
        {
            get { return _Domain; }
            set
            {
                _Domain = value;
                OnPropertyChanged("Domain");
            }
        }

        /// <summary>
        /// Get\Set the Go To Path
        /// </summary>
        public string GoToPath
        {
            get { return _GoToPath; }
            set
            {
                _GoToPath = value;
                OnPropertyChanged("GoToPath");
            }
        }

        #endregion

        #region Actions

        public Action ScrollToRigthEndAction;

        public Action ScrollToHorizontalForwardAction;

        public Action ScrollToHorizontalBackwardAction;

        public Action CloseAction { get; set; }

        #endregion

        #region Commands

        /// <summary>
        /// The Folder double clicked Command
        /// </summary>
        private ICommand _FolderMouseDoubleClickCommand;
        public ICommand FolderMouseDoubleClickCommand
        {
            get { return _FolderMouseDoubleClickCommand; }
            set { _FolderMouseDoubleClickCommand = value; }
        }

        /// <summary>
        /// The Up Command
        /// </summary>
        private ICommand _UpCommand;
        public ICommand UpCommand
        {
            get { return _UpCommand; }
            set { _UpCommand = value; }
        }

        /// <summary>
        /// The Select Command
        /// </summary>
        private ICommand _SelectCommand;
        public ICommand SelectCommand
        {
            get { return _SelectCommand; }
            set { _SelectCommand = value; }
        }

        /// <summary>
        /// The Add to favorite Command
        /// </summary>
        private ICommand _AddToFavoriteCommand;
        public ICommand AddToFavoriteCommand
        {
            get { return _AddToFavoriteCommand; }
            set { _AddToFavoriteCommand = value; }
        }

        /// <summary>
        /// The Delete to favorite Command
        /// </summary>
        private ICommand _DeleteFavoriteCommand;
        public ICommand DeleteFavoriteCommand
        {
            get { return _DeleteFavoriteCommand; }
            set { _DeleteFavoriteCommand = value; }
        }

        /// <summary>
        /// The Modify to favorite Command
        /// </summary>
        private ICommand _ModifyFavoriteCommand;
        public ICommand ModifyFavoriteCommand
        {
            get { return _ModifyFavoriteCommand; }
            set { _ModifyFavoriteCommand = value; }
        }

        /// <summary>
        /// The Select Folder Command
        /// </summary>
        private ICommand _SelectFolderCommand;
        public ICommand SelectFolderCommand
        {
            get { return _SelectFolderCommand; }
            set { _SelectFolderCommand = value; }
        }

        /// <summary>
        /// The Go TO Command
        /// </summary>
        private ICommand _GoToCommand;
        public ICommand GoToCommand
        {
            get { return _GoToCommand; }
            set { _GoToCommand = value; }
        }

        /// <summary>
        /// The Navigator Mouse Down Command
        /// </summary>
        private ICommand _NavigatorMouseDownCommand;
        public ICommand NavigatorMouseDownCommand
        {
            get { return _NavigatorMouseDownCommand; }
            set { _NavigatorMouseDownCommand = value; }
        }

        /// <summary>
        /// The Navigator scroll changed Command
        /// </summary>
        private ICommand _NavigatorScroolChangedCommand;
        public ICommand NavigatorScroolChangedCommand
        {
            get { return _NavigatorScroolChangedCommand; }
            set { _NavigatorScroolChangedCommand = value; }
        }

        /// <summary>
        /// The mouse move on Navigator Command
        /// </summary>
        private ICommand _NavigatorMouseMoveCommand;
        public ICommand NavigatorMouseMoveCommand
        {
            get { return _NavigatorMouseMoveCommand; }
            set { _NavigatorMouseMoveCommand = value; }
        }

        /// <summary>
        /// The Exit Command
        /// </summary>
        private ICommand _ExitCommand;
        public ICommand ExitCommand
        {
            get { return _ExitCommand; }
            set { _ExitCommand = value; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public NavigationFolderDialogViewModel(string currentPath, string title, string domain, IList<FavoriteFolder> favoriteFolders)
        {
            // Register events
            RegisterEvents(true);

            // Initialize components
            Initiliaze(currentPath, title, domain, favoriteFolders);
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
                RootChanged += OnRootChanged;
            }
            else
            {
                RootChanged -= OnRootChanged;
            }
        }

        /// <summary>
        /// Initialize the component
        /// </summary>
        private async void Initiliaze(string currentPath, string title, string domain, IList<FavoriteFolder> favoriteFolders)
        {
            _IsFavoriteLoad = true;

            // Set the Title
            Title = title;

            // Set the Domain
            Domain = domain;

            // Set the is Favorite dirty
            IsFavoriteDirty = false;

            // Set the FavoriteFolders
            if (favoriteFolders != null)
            {
                FavoriteFolders = favoriteFolders;
            }
            else
            {
                FavoriteFolders = new List<FavoriteFolder>();
            }

            // Get Drawing image
            var pcImage = Application.Current.TryFindResource("PC") as DrawingImage;
            var netImage = Application.Current.TryFindResource("NETWORK") as DrawingImage;
            var favoritetImage = Application.Current.TryFindResource("STAR") as DrawingImage;
            var goToImage = Application.Current.TryFindResource("PEN") as DrawingImage;

            RootItems = new List<RootItem>
            {
                new RootItem((int)NavigatorFolderType.ThisPc, pcImage, "PC"),
                new RootItem((int)NavigatorFolderType.Net, netImage, "NET"),
                new RootItem((int)NavigatorFolderType.Favorite, favoritetImage, "FAVORITES"),
                new RootItem((int)NavigatorFolderType.GoTo, goToImage, "GO TO")
            };

            var goToFavorite = (FavoriteFolders != null && favoriteFolders.Any());
            if (goToFavorite)
            {
                SelectedRoot = RootItems.ElementAt((int)NavigatorFolderType.Favorite);
            }
            else
            {
                // Set the Current Path
                CurrentPath = currentPath;

                var navigatorType = NavigatorFolderType.None;
                if (!string.IsNullOrEmpty(currentPath))
                {
                    navigatorType = NavigatorFolderFactory.GetNavigatorTypeFromPath(currentPath);
                }

                if (navigatorType != NavigatorFolderType.None)
                {
                    SelectedRoot = RootItems.ElementAt((int)navigatorType);
                }
                else
                {
                    SelectedRoot = RootItems.ElementAt((int)NavigatorFolderType.ThisPc);
                }
            }

            // Create the Commands
            FolderMouseDoubleClickCommand = new RelayCommand(OnFolderMouseDoubleClickCommandExecute);
            UpCommand = new RelayCommand(OnUpCommandExecute);
            SelectCommand = new RelayCommand(OnSelectCommandExecute);
            AddToFavoriteCommand = new RelayCommand(OnAddToFavoriteCommandExecute);
            DeleteFavoriteCommand = new RelayCommand(OnDeleteCommandExecute);
            ModifyFavoriteCommand = new RelayCommand(OnModifyCommandExecute);
            SelectFolderCommand = new RelayCommand(OnSelectFolderCommandExecute);
            GoToCommand = new RelayCommand(OnGoToCommandExecute);
            NavigatorMouseDownCommand = new RelayCommand(OnNavigatorMouseDownExecute);
            NavigatorScroolChangedCommand = new RelayCommand(OnNavigatorScroolChangedCommandExecute);
            NavigatorMouseMoveCommand = new RelayCommand(OnNavigatorMouseMoveCommandExecute);
            ExitCommand = new RelayCommand(OnExitCommandExecute);

            // Get the Folders from path
            _CancellationTokenSource = new CancellationTokenSource();
            var error = await GetFilteredFoldersAsync(currentPath, 0, (NavigatorFolderType)SelectedRoot.ID, _CancellationTokenSource.Token);

            if (error)
            {
                CurrentPath = string.Empty;
                await GetFilteredFoldersAsync(CurrentPath, 0, (NavigatorFolderType)SelectedRoot.ID, _CancellationTokenSource.Token);
            }

            // Create the Navigator buttons
            NavigatorButtons = new ObservableCollection<ImageButton>();
            _NavigatorButtonLabels = new List<string>();
            if ((NavigatorFolderType)SelectedRoot.ID != NavigatorFolderType.Favorite 
                && !string.IsNullOrEmpty(CurrentPath))
            {
                CreateNavigatorButtonsFromPath(CurrentPath);
            }
            else
            {
                CreateNavigatorButtonsFromPath(string.Empty);
            }

            _IsFavoriteLoad = false;

            _IsJustLoaded = false;
        }

        /// <summary>
        /// Create all the navigator buttons from specificate path
        /// </summary>
        private void CreateNavigatorButtonsFromPath(string path)
        {
            if (NavigatorButtons != null)
            {
                NavigatorButtons.Clear();
                _NavigatorButtonLabels.Clear();

                var splittedString = path.Split('\\');
                splittedString = splittedString.Where(s => !string.IsNullOrEmpty(s)).ToArray();

                var index = 0;
                NavigatorButtons.Add(CreateNavigatorButton(_NavigatorFolder.RootName, index, NavigatorHomeButtonStyle));

                if (splittedString.Count() > 0)
                {
                    index++;
                    foreach (var currentString in splittedString)
                    {
                        NavigatorButtons.Add(CreateNavigatorButton(currentString, index, NavigatorButtonStyle));
                        index++;
                    }
                }
            }
        }

        /// <summary>
        /// Create a new navigator button
        /// </summary>
        private ImageButton CreateNavigatorButton(string content, int nodeIndex, string styleName)
        {
            var style = styleName;

            var length = content?.Length;
            if (length > 16 && length <= 22)
            {
                style = $"{styleName}_002";
            }
            else if (length > 22 && length <= 28)
            {
                style = $"{styleName}_003";
            }
            else if (length > 28 && length < 32)
            {
                style = $"{styleName}_004";
            }
            else if (length >= 32)
            {
                style = $"{styleName}_004";
                content = $"{content.Substring(0, 32)}...";
            }

            var navigatorButton = new ImageButton()
            {
                Style = Application.Current.FindResource(style) as Style,
                Content = content?.ToUpper(),
                Tag = nodeIndex
            };

            // Add click event to new navigator button
            //navigatorButton.Click += new RoutedEventHandler(OnNavigationButtonClick);
            navigatorButton.PreviewMouseUp += new MouseButtonEventHandler(OnNavigationButtonClick);

            return navigatorButton;
        }

        /// <summary>
        /// Check whether the path exists or not.
        /// It is an async operation becouse of the Net operation that could be very slow.
        /// </summary>
        private async Task<DirectoryExistsResult> IsPathExists(string path, CancellationToken token)
        {
            var directoryExistsResult = DirectoryExistsResult.NotExists;

            token.ThrowIfCancellationRequested();

            await Task.Run(() =>
            {
                try
                {
                    token.ThrowIfCancellationRequested();

                    // Check whether the specificated path exists or not
                    directoryExistsResult = _NavigatorFolder.DirectoryExists(path, token);

                    token.ThrowIfCancellationRequested();
                }
                catch (Exception ex)
                {
                    CoreLog.Instance.Append($"[{ex.Source}] - {ex.Message} - {ex.StackTrace}", CoreLogType.Error);
                }
            });

            return directoryExistsResult;
        }

        /// <summary>
        /// Check whether the path exists and only in that case get the Folders
        /// </summary>
        private async Task<bool> GetFolderIfDirectoryExistsAsync(string path, int pageIndex, bool error, CancellationToken token)
        {
            IList<NavigationDialogItem> list = null;

            token.ThrowIfCancellationRequested();

            await Task.Run(() =>
            {
                list = _NavigatorFolder?.GetFolders(path, token, ref error);

                token.ThrowIfCancellationRequested();
            });

            token.ThrowIfCancellationRequested();

            if (list != null && _NavigatorFolder != null && !error)
            {
                token.ThrowIfCancellationRequested();

                // Create the list of Folders
                Items = new ObservableCollection<NavigationDialogItem>(
                    list?.Skip(pageIndex * NumberOfItemInPage)?.Take(NumberOfItemInPage)?.ToList());

                // Set the Number of Elements and Exist Folders properties
                NumberOfElements = list.Count();
                ExistFolders = Items?.Count() > 0;

                // Set the current path and page index
                CurrentPath = path;

                // Scrool the Navigator buttons list to the end
                if (NavigatorButtons != null)
                {
                    ScrollToRigthEndAction();
                }

                token.ThrowIfCancellationRequested();
            }
            else
            {
                //ExistFolders = false;
            }

            return error;
        }

        /// <summary>
        /// Delete all navigation buttons in list from choosen position to end
        /// </summary>
        private void DeleteAllButtonsStartingByPosition(int startPosition)
        {
            if (NavigatorButtons != null && startPosition <= NavigatorButtons.Count() - 1)
            {
                for (int index = NavigatorButtons.Count() - 1; index >= startPosition; index--)
                {
                    // Delete navigation button and dispose the click event
                    var navigatorButton = NavigatorButtons[index];
                    //navigatorButton.Click -= new RoutedEventHandler(OnNavigationButtonClick);
                    navigatorButton.PreviewMouseUp -= new MouseButtonEventHandler(OnNavigationButtonClick);
                    NavigatorButtons.Remove(navigatorButton);
                }
            }
        }

        /// <summary>
        /// Get the selected folder and exit from Navigator
        /// </summary>
        private void SelectFolder(ListView listFolders)
        {

            if (listFolders?.SelectedItems.Count > 0)
            {
                SelectedFolder = (listFolders?.SelectedItems[0] as NavigationDialogItem).Name;
                SelectedFolderPath = (listFolders?.SelectedItems[0] as NavigationDialogItem).FolderFullPath;

                Result = System.Windows.Forms.DialogResult.OK;
            }
            else if (!string.IsNullOrEmpty(CurrentPath))
            {
                SelectedFolder = new DirectoryInfo(CurrentPath).Name;
                SelectedFolderPath = CurrentPath;

                Result = System.Windows.Forms.DialogResult.OK;
            }

            if (Result != System.Windows.Forms.DialogResult.None)
            {
                CloseWindow();
            }
        }

        // Free memory and close the Window
        private void CloseWindow()
        {
            if (NavigatorButtons != null)
            {
                // Free memory of Navigator buttons
                foreach (var navigatorButton in NavigatorButtons)
                {

                    navigatorButton.PreviewMouseUp -= new MouseButtonEventHandler(OnNavigationButtonClick);
                }
            }

            // Close the Window
            CloseAction();
        }

        #endregion

        #region Events

        /// <summary>
        /// Get the filtered folders in async mode
        /// </summary>
        private async Task<bool> GetFilteredFoldersAsync(string path, int pageIndex, NavigatorFolderType rootItemType, CancellationToken token)
        {
           // _IsJustLoaded = true;

            bool error = false;

            NavigatorFolderType pathFolderType;
            DirectoryExistsResult directoryExists = DirectoryExistsResult.NotExists;

            IsTryConnecting = true;

            if (!string.IsNullOrEmpty(path))
            {
                if (rootItemType == NavigatorFolderType.Favorite || rootItemType == NavigatorFolderType.GoTo)
                {
                    pathFolderType = NavigatorFolderFactory.GetNavigatorTypeFromPath(path);
                }
                else
                {
                    pathFolderType = rootItemType;
                }

                token.ThrowIfCancellationRequested();

                directoryExists = await IsPathExists(path, token);

                token.ThrowIfCancellationRequested();

                // Check whether the specificated path exists or not
                if (directoryExists == DirectoryExistsResult.Exists)
                {
                    token.ThrowIfCancellationRequested();

                    error = await GetFolderIfDirectoryExistsAsync(path, pageIndex, error, token);

                    token.ThrowIfCancellationRequested();
                }
                else if (directoryExists == DirectoryExistsResult.IOError &&
                    pathFolderType == NavigatorFolderType.Net)
                {
                    token.ThrowIfCancellationRequested();

                    IsTryConnecting = false;

                    // Create the model and view models              
                    var domain = string.Empty;
                    var userName = string.Empty;
                    var password = string.Empty;

                    FavoriteFolder favoriteFolder = null;
                    if (FavoriteFolders != null)
                    {
                        favoriteFolder = FavoriteFolders.FirstOrDefault(f => f.Folder == path);
                    }
                    if (favoriteFolder!=null)
                    {
                        domain = favoriteFolder.Domain;
                        userName = favoriteFolder.Username;
                        password = favoriteFolder.Password; ;
                    }

                    var folderCredentialsViewModel = new FolderCredentialsViewModel(path, domain, userName, password);

                    // Show the Credential Window
                    var folderCredentialsWindow = new FolderCredentials(folderCredentialsViewModel);

                    token.ThrowIfCancellationRequested();

                    folderCredentialsWindow.ShowWindow(true);
                    if (folderCredentialsViewModel.Result.WindowResult == System.Windows.Forms.DialogResult.OK
                        && folderCredentialsViewModel.Result.ConnectionResult == 0)
                    {
                        if (favoriteFolder != null)
                        {
                            IsFavoriteDirty = true;
                            favoriteFolder.Username = folderCredentialsViewModel.Result.Username;
                            favoriteFolder.Domain = folderCredentialsViewModel.Result.Domain;
                            favoriteFolder.Password = folderCredentialsViewModel.Result.Password;
                        }

                        token.ThrowIfCancellationRequested();

                        error = await GetFolderIfDirectoryExistsAsync(path, pageIndex, error, token);

                        token.ThrowIfCancellationRequested();
                    }
                    else
                    {
                        error = true;
                    }
                }
                else
                {
                    error = true;
                }
            }
            else
            {
                token.ThrowIfCancellationRequested();

                await GetFolderIfDirectoryExistsAsync(path, pageIndex, error, token);

                token.ThrowIfCancellationRequested();
            }

            IsTryConnecting = false;

            if (error)
            {
                if ((NavigatorFolderType)SelectedRoot.ID == NavigatorFolderType.GoTo)
                {
                    ExistFolders = false;
                }
                else
                {
                    //ExistFolders = true;
                    ExistFolders = Items?.Count()>0;
                }

                var message = string.Format(CultureResources.GetString("Message_Can_Not_Open_Folder"), path);
                var messageBox = new CustomMessageBox(MessageBoxType.Error, message, MessageBoxButtonsType.OK);
                messageBox.ShowMessageBox();
            }

            //_IsJustLoaded = false;

            return error;
        }

        /// <summary>
        /// Occurs when the Page Index changes its value
        /// </summary>
        private async void PageIndexChangedAsync()
        {
            try
            {
                if (!_IsJustLoaded && PageIndex > -1)
                {
                    //Get the Folders from path
                    _CancellationTokenSource = new CancellationTokenSource();
                    await GetFilteredFoldersAsync(CurrentPath, PageIndex, (NavigatorFolderType)SelectedRoot.ID, _CancellationTokenSource.Token);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine("[OnItemSourceChangedCallBack] - " + ex.Message);
            }
            finally
            {
                _CancellationTokenSource = null;
            }
        }

        /// <summary>
        /// Occurs when the Selected Root changes its value
        /// </summary>
        private async Task SelectedRootChangedAsync()
        {
            try
            {
                if (SelectedRoot != null)
                {
                    // Set the navigator folder
                    _NavigatorFolder = NavigatorFolderFactory.Get((NavigatorFolderType)SelectedRoot.ID, Domain, FavoriteFolders);

                    if (!_IsFavoriteLoad)
                    {
                        ExistFolders = false;

                        IsTryConnecting = true;

                        PageIndex = -1;

                        // Delete the Navigator buttons
                        DeleteAllButtonsStartingByPosition(0);

                        // Get the Folders from path
                        CurrentPath = string.Empty;

                        // Get the filtered folders
                        _CancellationTokenSource = new CancellationTokenSource();
                        await GetFilteredFoldersAsync(CurrentPath, PageIndex, (NavigatorFolderType)SelectedRoot.ID, _CancellationTokenSource.Token);

                        // Set the Root Caption
                        CreateNavigatorButtonsFromPath(string.Empty);

                        // Reset the FullPathOfSelectedItem
                        FullPathOfSelectedItem = string.Empty;

                        IsTryConnecting = false;
                    }
                }
                else
                {
                    ExistFolders = false;
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine("[OnSelectedRootChangedCallBack] - " + ex.Message);
            }

            finally
            {
                _CancellationTokenSource = null;
            }
        }

        /// <summary>
        /// Occurs when a navigator button is pressed.
        /// It deletes all the navigation buttons forward.
        /// </summary>
        protected async void OnNavigationButtonClick(object sender, EventArgs e)
        {
            if (!_ScroolChanged)
            {
                try
                {
                    ExistFolders = false;

                    var button = sender as Button;
                    var position = (int)button.Tag;

                    // Delate all unused buttons 
                    DeleteAllButtonsStartingByPosition(position + 1);

                    CurrentPath = _NavigatorFolder.CreateSubPath(CurrentPath, position);

                    // Get the Folders
                    //GetFilteredFolders(CurrentPath, PageIndex, (NavigatorFolderType)SelectedRoot.ID);
                    _CancellationTokenSource = new CancellationTokenSource();
                    await GetFilteredFoldersAsync(CurrentPath, PageIndex, (NavigatorFolderType)SelectedRoot.ID, _CancellationTokenSource.Token);

                    // ExistFolders = true;
                    ExistFolders = Items?.Count() > 0;
                }
                catch (OperationCanceledException)
                {
                }
                finally
                {
                    _CancellationTokenSource = null;

                    // Go to the first page
                    PageIndex = 0;
                }
            }
            else
            {
                _ScroolChanged = false;
            }
        }

        /// <summary>
        /// Occurs when the Add to favorite is executed
        /// </summary>
        private void OnAddToFavoriteCommandExecute(object param)
        {
            if (param is ListView foldersList)
            {
                bool error = false;

                // Get the selected folder
                if (foldersList?.SelectedItems.Count > 0)
                {
                    SelectedFolder = (foldersList?.SelectedItems[0] as NavigationDialogItem).Name;
                    SelectedFolderPath = (foldersList?.SelectedItems[0] as NavigationDialogItem).FolderFullPath;
                }
                else
                {
                    if (!string.IsNullOrEmpty(CurrentPath))
                    {
                        SelectedFolder = new DirectoryInfo(CurrentPath).Name;
                        SelectedFolderPath = CurrentPath;
                    }
                    else
                    {
                        error = true;
                    }
                }

                if (!error)
                {
                    // Create the model and view models
                    var favoriteFolder = new FavoriteFolder(string.Empty, SelectedFolderPath, string.Empty, string.Empty, string.Empty);
                    var favoriteFolderDetailsViewModel = new FavoriteFolderDetailsViewModel(favoriteFolder);

                    // Show the window
                    var favoriteFolderDetailsWindow = new FavoriteFolderDetails(favoriteFolderDetailsViewModel);
                    favoriteFolderDetailsWindow.ShowWindow(true);
                    if (favoriteFolderDetailsWindow.Result == System.Windows.Forms.DialogResult.OK)
                    {
                        if (favoriteFolderDetailsViewModel.FavoriteFolder != null)
                        {
                            FavoriteFolders.Add(favoriteFolderDetailsViewModel.FavoriteFolder);
                            IsFavoriteDirty = true;
                        }
                    }

                    // Dispose the view model
                    favoriteFolderDetailsViewModel.Dispose();
                }
            }
        }

        /// <summary>
        /// Occurs when the Select is executed
        /// </summary>
        private void OnSelectCommandExecute(object param)
        {
            if (param is ListView listFolders)
            {               
                SelectFolder(listFolders);
            }
        }

        /// <summary>
        /// Occurs when the user performs a double click to a folder.
        /// The new path is created.
        /// </summary>
        private async void OnFolderMouseDoubleClickCommandExecute(object param)
        {
            if (param is StackPanel sender)
            {
                //  IsTryConnecting = true;

                // Get the Folders
                var currentPath = ((NavigationDialogItem)sender.DataContext).FolderFullPath;
                var rootType = (NavigatorFolderType)SelectedRoot.ID;

                switch (rootType)
                {
                    case NavigatorFolderType.ThisPc:
                    case NavigatorFolderType.Net:
                        try
                        {
                            // Save the last value of Current path
                            var tempCurrentPath = CurrentPath;

                            // Set the new current path
                            CurrentPath = currentPath;

                            // Get the folders
                            PageIndex = -1;
                            _IsJustLoaded = true;

                            _CancellationTokenSource = new CancellationTokenSource();
                            var error = await GetFilteredFoldersAsync(currentPath, 0, (NavigatorFolderType)SelectedRoot.ID, _CancellationTokenSource.Token);

                            PageIndex = 0;
                            _IsJustLoaded = false;

                            if (!error)
                            {
                                // Create the new Navigator button
                                var index = Convert.ToInt32(NavigatorButtons?.Last()?.Tag);
                                if (index < 0)
                                {
                                    index = 0;
                                }

                                index++;
                                var lastFolderName = new DirectoryInfo(currentPath).Name;

                                var style = index == 0 ? NavigatorHomeButtonStyle : NavigatorButtonStyle;
                                var navigatorButton = CreateNavigatorButton(lastFolderName, index, style);
                                NavigatorButtons.Add(navigatorButton);

                                // Scrool the Navigator buttons list to the end
                                ScrollToRigthEndAction();
                            }
                            else
                            {
                                // Restore the Current Path if an error occurs
                                CurrentPath = tempCurrentPath;
                            }
                        }
                        catch (OperationCanceledException)
                        {
                        }
                        finally
                        {
                            _CancellationTokenSource = null;
                        }

                        break;

                    case NavigatorFolderType.Favorite:
                        await GoToFavoritePath(_SelectedFolder);
                        break;
                }

                // IsTryConnecting = false;
            }
        }

        /// <summary>
        /// OCcurs when the "UP" command is executed
        /// </summary>
        private async void OnUpCommandExecute(object param)
        {
            var currentPath = CurrentPath;
            try
            {
                if (!string.IsNullOrEmpty(currentPath))
                {
                    // Delete the last navigator button if it is not the only one
                    var count = NavigatorButtons.Count();
                    if (count > 1)
                    {
                        DeleteAllButtonsStartingByPosition(count - 1);
                    }

                    // Set the current path. Controls whether the path is a Root (C:\\, D:\\ ecc..)
                    var isRoot = NavigatorButtons.Count() == 1;
                    if (isRoot)
                    {
                        currentPath = string.Empty;
                    }
                    else
                    {
                        // Go to up in Tree 
                        currentPath = _NavigatorFolder.GoUpInTreeFolder(currentPath);
                    }

                    // Get the SubFolders
                    _CancellationTokenSource = new CancellationTokenSource();
                    await GetFilteredFoldersAsync(currentPath, 0, (NavigatorFolderType)SelectedRoot.ID, _CancellationTokenSource.Token);

                    // Scrool The navigator buttons list to the end
                    ScrollToRigthEndAction();
                }
                else
                {
                    CurrentPath = string.Empty;
                    PageIndex = 0;
                }
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                _CancellationTokenSource = null;
            }

        }

        /// <summary>
        /// Move the component to the Selected Favorite Path
        /// </summary>
        private async Task GoToFavoritePath(NavigationDialogItem selectedFolder)
        {
            if (selectedFolder != null)
            {
                try
                {
                    IsTryConnecting = true;

                    // Get the favorite path
                    var currentPath = selectedFolder.FolderFullPath;

                    var navigatorType = NavigatorFolderFactory.GetNavigatorTypeFromPath(currentPath);
                    _NavigatorFolder = NavigatorFolderFactory.Get(navigatorType, Domain, FavoriteFolders);

                    //_NavigatorFolder = NavigatorFolderFactory.GetByPath(currentPath, FavoriteFolders);
                    _CancellationTokenSource = new CancellationTokenSource();
                    var error = await GetFilteredFoldersAsync(currentPath, 0, (NavigatorFolderType)SelectedRoot.ID, _CancellationTokenSource.Token);
                    if (!error)
                    {
                        _IsFavoriteLoad = true;

                        if (navigatorType != NavigatorFolderType.None)
                        {
                            // Move to the Root by type
                            SelectedRoot = RootItems.ElementAt((int)navigatorType);

                            // Delete all Navigator buttons
                            DeleteAllButtonsStartingByPosition(0);

                            // Create the new navigator buttons
                            CreateNavigatorButtonsFromPath(currentPath);

                            // Scrool the Navigator buttons list to the end
                            ScrollToRigthEndAction();
                        }
                    }
                    else
                    {
                        _NavigatorFolder = NavigatorFolderFactory.Get(NavigatorFolderType.Favorite, Domain, FavoriteFolders);
                    }

                    _IsFavoriteLoad = false;
                    IsTryConnecting = false;
                }
                catch (OperationCanceledException)
                {
                    // IsTryConnecting = false;
                }
                finally
                {
                    _CancellationTokenSource = null;
                }
            }
        }

        /// <summary>
        /// Occurs when the Delete Favorite is executed
        /// </summary>
        private void OnDeleteCommandExecute(object param)
        {
            if (param is ListView foldersList)
            {
                try
                {
                    if (foldersList?.SelectedItems.Count > 0)
                    {
                        // Get the element
                        var navigationDialogItem = (foldersList.SelectedItems[0] as NavigationDialogItem);

                        // Get the index of element
                        var index = foldersList.Items.IndexOf(navigationDialogItem);

                        // Remove element from lists by index
                        FavoriteFolders.RemoveAt(index);
                        Items.RemoveAt(index);

                        ExistFolders = FavoriteFolders?.Count > 0;

                        // Update the IsFavoriteDirty
                        IsFavoriteDirty = true;
                    }
                }
                catch (Exception ex)
                {
                    CoreLog.Instance.Append($"[{ex.Source}] - {ex.Message} - {ex.StackTrace}", CoreLogType.Error);
                }
            }
        }

        /// <summary>
        /// Cancel the Async operation
        /// </summary>
        private void CancelRunningAsyncOperation()
        {
            if (_CancellationTokenSource != null)
            {
                _CancellationTokenSource.Cancel();
            }

            _CancellationTokenSource = null;
        }

        /// <summary>
        /// Occurs when the Modify Favorite is executed
        /// </summary>
        private void OnModifyCommandExecute(object param)
        {
            if (param is ListView foldersList)
            {
                try
                {
                    IsTryConnecting = false;

                    CancelRunningAsyncOperation();

                    if (foldersList?.SelectedItems.Count > 0)
                    {
                        // Get the element
                        var navigationDialogItem = (foldersList.SelectedItems[0] as NavigationDialogItem);

                        if (navigationDialogItem != null)
                        {
                            // Get the index of element
                            var index = foldersList.Items.IndexOf(navigationDialogItem);

                            // Get the element from lists by index
                            FavoriteFolder favoriteFolder = null;
                            if (index < FavoriteFolders.Count())
                            {
                                favoriteFolder = FavoriteFolders.ElementAt(index);
                            }

                            if (favoriteFolder != null)
                            {
                                // Create the model and view models
                                var favoriteFolderDetailsViewModel = new FavoriteFolderDetailsViewModel(favoriteFolder, true);

                                // Show the window
                                var favoriteFolderDetailsWindow = new FavoriteFolderDetails(favoriteFolderDetailsViewModel);
                                favoriteFolderDetailsWindow.ShowWindow(true);
                                if (favoriteFolderDetailsWindow.Result == System.Windows.Forms.DialogResult.OK)
                                {
                                    if (favoriteFolderDetailsViewModel.FavoriteFolder != null)
                                    {
                                        // Update the navigationDialogItem
                                        navigationDialogItem.Name = favoriteFolderDetailsViewModel.FavoriteFolder.FriendlyName;
                                        navigationDialogItem.FolderFullPath = favoriteFolderDetailsViewModel.FavoriteFolder.Folder;

                                        // Update the IsFavoriteDirty
                                        IsFavoriteDirty = true;
                                    }
                                }

                                // Dispose the view model
                                favoriteFolderDetailsViewModel.Dispose();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CoreLog.Instance.Append($"[{ex.Source}] - {ex.Message} - {ex.StackTrace}", CoreLogType.Error);
                }
            }
        }

        /// <summary>
        /// Occurs when the selection in folder list changes
        /// </summary>
        private void OnSelectFolderCommandExecute(object param)
        {
            if (param is ListView listFolders)
            {
                if (listFolders?.SelectedItems.Count > 0)
                {
                    _SelectedFolder = listFolders?.SelectedItems[0] as NavigationDialogItem;
                    if (_SelectedFolder != null)
                    {
                        FullPathOfSelectedItem = _SelectedFolder.FolderFullPath;
                    }
                }
            }
        }

        /// <summary>
        /// Occurs when the mouse is pressed on Navigator ScroolViever.
        /// Set the ScroolChanged to false
        /// </summary>
        private void OnNavigatorMouseDownExecute(object param)
        {
            _ScroolChanged = false;
        }

        /// <summary>
        /// Occurs when the Navigator ScroolViever change its scroll
        /// </summary>
        private void OnNavigatorScroolChangedCommandExecute(object param)
        {
            _ScroolChanged = true;
        }

        /// <summary>
        /// Occurs when the mouse is moving on Navigator
        /// </summary>
        private void OnNavigatorMouseMoveCommandExecute(object param)
        {
            //private void OnPreviewMouseMove(object sender, MouseEventArgs e)
            //{
                Point newMousePosition = Mouse.GetPosition((ItemsControl)param);

                if (Mouse.LeftButton == MouseButtonState.Pressed)
                {
                if (newMousePosition.X < _OldMousePosition.X)
                {
                    //PathScroolViewer.ScrollToHorizontalOffset(PathScroolViewer.HorizontalOffset + 1);
                    ScrollToHorizontalForwardAction();
                }

                if (newMousePosition.X > _OldMousePosition.X)
                {
                    //PathScroolViewer.ScrollToHorizontalOffset(PathScroolViewer.HorizontalOffset - 1);
                    ScrollToHorizontalBackwardAction();
                }
            }
                else
                {
                    _OldMousePosition = newMousePosition;
                }
            //}

        }

        /// <summary>
        /// Occurs when the Domain is changed
        /// </summary>
        private async void DomainPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                _IsJustLoaded = true;

                _NavigatorFolder = NavigatorFolderFactory.Get(NavigatorFolderType.Net, Domain, FavoriteFolders);

                PageIndex = -1;

                // Delete the Navigator buttons
                DeleteAllButtonsStartingByPosition(0);

                // Get the Folders from path
                CurrentPath = string.Empty;

                // Get the filtered folders
                _CancellationTokenSource = new CancellationTokenSource();
                await GetFilteredFoldersAsync(CurrentPath, PageIndex, NavigatorFolderType.Net, _CancellationTokenSource.Token);
                // PageIndex = 0;

                //GetFilteredFolders(CurrentPath, PageIndex, (NavigatorFolderType)SelectedRoot.ID);

                // Set the Root Caption
                CreateNavigatorButtonsFromPath(string.Empty);

                // Reset the FullPathOfSelectedItem
                FullPathOfSelectedItem = string.Empty;

                //await GetFilteredFoldersAsync(string.Empty, 0, NavigatorFolderType.Net);
                await SelectedRootChangedAsync();

                PageIndex = 0;

                _IsJustLoaded = false;
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                _CancellationTokenSource = null;
            }

        }

        /// <summary>
        /// Occurs when the GOTO command is executed
        /// </summary>
        private async void OnGoToCommandExecute(object param)
        {
            if (param is string text && !string.IsNullOrEmpty(text))
            {
                await GotoPathAsync(GoToPath);
            }
        }

        /// <summary>
        /// Go to the specificated path
        /// </summary>
        private async Task GotoPathAsync(string currentPath)
        {
            try
            {
                IsTryConnecting = true;

                // Get the favorite path
                //var currentPath = GoToPath;

                var navigatorType = NavigatorFolderFactory.GetNavigatorTypeFromPath(currentPath);
                _NavigatorFolder = NavigatorFolderFactory.Get(navigatorType, Domain, FavoriteFolders);

                _CancellationTokenSource = new CancellationTokenSource();
                var error = await GetFilteredFoldersAsync(currentPath, 0, (NavigatorFolderType)SelectedRoot.ID, _CancellationTokenSource.Token);
                if (!error)
                {
                    _IsFavoriteLoad = true;

                    if (navigatorType != NavigatorFolderType.None)
                    {
                        // Move to the Root by type
                        SelectedRoot = RootItems.ElementAt((int)navigatorType);

                        // Delete all Navigator buttons
                        DeleteAllButtonsStartingByPosition(0);

                        // Create the new navigator buttons
                        CreateNavigatorButtonsFromPath(currentPath);

                        // Scrool the Navigator buttons list to the end
                        ScrollToRigthEndAction();

                        ExistFolders = Items.Count() > 0;
                    }
                }
                else
                {
                    _NavigatorFolder = NavigatorFolderFactory.Get(NavigatorFolderType.Favorite, Domain, FavoriteFolders);
                }

                _IsFavoriteLoad = false;
                IsTryConnecting = false;
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                _CancellationTokenSource = null;
            }
        }

        /// <summary>
        /// Occurs when on the Goto the Enter key is pressed
        /// </summary>
        private async void GoToKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                await GotoPathAsync(GoToPath);
            }
        }

        /// <summary>
        /// Occurs when the Root changes
        /// </summary>
        private async void OnRootChanged(object sender, EventArgs e)
        {
            await SelectedRootChangedAsync();
        }

        /// <summary>
        /// Occours when the Exit button is touched
        /// </summary>
        private void OnExitTouchDown(object sender, TouchEventArgs e)
        {
            Result = System.Windows.Forms.DialogResult.Cancel;

            CloseWindow();
            e.Handled = true;
        }

        /// <summary>
        /// Occours when the Exit command is executed
        /// </summary>
        private void OnExitCommandExecute(object param)
        {
            if (_CancellationTokenSource != null)
            {
                _CancellationTokenSource.Cancel();
                _CancellationTokenSource = null;
            }

            //if (e.StylusDevice != null)
            //{
            //    e.Handled = true;
            //}
            //else
            //{
            // Set the Results

            Result = System.Windows.Forms.DialogResult.Cancel;
            SelectedFolderPath = string.Empty;

            // Close the window
            CloseWindow();
            // }
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Dispose the objects
        /// </summary>
        public void Dispose()
        {
            RegisterEvents(false);
        }

        #endregion
    }
}
