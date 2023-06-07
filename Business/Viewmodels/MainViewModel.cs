//#define SHOW_ADDITIONAL

using Business;
using Common;
using Core;
using Core.ResourceManager.Cultures;
using Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Business
{
    public class MainViewModel : ViewModelBase
    {
        #region Members

        // The list of Images Data
        private ObservableCollection<ImageData> _ImagesData;

        // The list of fullpath and name of the images files 
        // found in the selected folder
        private IEnumerable<string> _FileNames;

        // It is the selected product in the list
        private ImageData _SelectedImageData;

        private string _PathImagesFolder;
        private int _NumberOfImagesInPage;

        // The Managers
        private ImageManager _ImageManager;
        private FileManager _FileManager;
        private ImageFileManager _ImageFileManager;
        private LogoManager _LogoManager;

        private CreateReportViewModel _CreateReportViewModel;
        private OptionsViewModel _OptionsViewModel;

        private CommonLog _Log;

        // The filters
        private string _FileNameTextFilter;
        private DateTime? _FromDateFilter;
        private DateTime? _ToDateFilter;

        // The total number of images
        private int _NumberOfImages;

        // It is the current active page
        private int _PageIndex;

        // Specify the index of choosen image
        private int _ImageIndex;

        // Specify the number of pages
        private int _NumberOfPages;

        // Specify whether an error is occured.
        // 1. A folder in Settings does not exists
        // 2. A folder exists but not images are found
        // 3. The Init file is corrupted
        // 4. The Settings file is corrupted
        private bool _IsErrorOccured;

        // If an error occurs a message is set
        private string _ErrorMessage;

        // Specify whether is possible to move to the next Image or not
        private bool _CanMoveNext;

        // Specify whether is possible to move to the previus Image or not
        private bool _CanMoveBack;

        // The additional string message to the Message status
        private string _AdditionalStatusMessage;

        // The status of the Window
        private CreateReportStatusEnum _CreateReportStatus;

        // Specify that the loading image operation is running
        private bool _IsLoadingImages;

        // Specify whether the move back of Pages is performed
        private bool _IsPageMovedBack;

        // Specify the applied image formats
        private string _ImageFormatsFilter;

        // The Current Cause and Action
        private string _CurrentCause;
        private string _CurrentAction;

        // The additiona and comments content
        private string _AdditionalContent;
        private string _GlobalCommentContent;
        private bool _AdditionalExist;

        private Credentials _LastCredentials;
        private FavoriteFolders _FavoriteFolders;

        private Dictionary<ReportAddedContentType, string> _ReportAddedContents;

        private Options _Options;
        private Filters _Filters;

        private IList<Point> _DatasetCoordinates;
        private WriteableBitmap _ImageNotesLayer;

        #endregion

        #region Properties

        /// <summary>
        /// Get the Title of the Window + Version
        /// </summary>
        public string Title
        {
            get
            {
                return $"Dylog Remote Report - {System.Reflection.Assembly.GetEntryAssembly().GetName().Version}";
            }
        }

        /// <summary>
        /// Get\Set the number of images in one page of the list
        /// </summary>
        public int NumberOfImagesInPage
        {
            get { return _NumberOfImagesInPage; }
            set
            {
                _NumberOfImagesInPage = value;
                OnPropertyChanged("NumberOfImagesInPage");
            }
        }

        /// <summary>
        /// Returns the number of images in folder
        /// </summary>
        public int NumberOfImages
        {
            get { return _NumberOfImages; }
            set
            {
                _NumberOfImages = value;
                OnPropertyChanged("NumberOfImages");
            }
        }

        /// <summary>
        /// Get the Index of choosen Page.
        /// The change in the PageIndex triggers the loading of images
        /// </summary>
        public int PageIndex
        {
            get
            {
                return _PageIndex;
            }
            set
            {
                _PageIndex = value;
                OnPropertyChanged("PageIndex");

                // Trigger the event
                TriggerPageChangedEvent(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Get\Set the Image index
        /// </summary>
        public int ImageIndex
        {
            get
            {
                return _ImageIndex;
            }
            set
            {
                _ImageIndex = value;
                OnPropertyChanged("ImageIndex");
            }
        }

        /// <summary>
        /// Get\Set the list of ImageData
        /// </summary>
        public ObservableCollection<ImageData> ImagesData
        {
            get { return _ImagesData; }
            set
            {
                _ImagesData = value;
                OnPropertyChanged("ImagesData");
            }
        }

        /// <summary>
        /// Get\Set the selected ImageData in the list
        /// </summary>
        public ImageData SelectedImageData
        {
            get { return _SelectedImageData; }
            set
            {
                // Restore the IsSelected value of previus item
                if (_SelectedImageData != null)
                {
                    _SelectedImageData.IsSelected = false;
                }

                if (value != null)
                {
                    // Update the CurrentCause and the CurrentAction
                    CurrentCause = value.Cause;
                    CurrentAction = value.Action;

                    // Update new item
                    _SelectedImageData = value;
                    _SelectedImageData.IsSelected = true;

                    OnPropertyChanged("SelectedImageData");

                    // Set the enabled of Forward and Next 
                    if (ImagesData != null)
                    {
                        var selectedIndex = ImagesData.IndexOf(SelectedImageData);
                        if (PageIndex > 0)
                        {
                            ImageIndex = (selectedIndex + 1) + (PageIndex * NumberOfImagesInPage);
                        }
                        else
                        {
                            ImageIndex = (selectedIndex + 1);
                        }

                        CanMoveBack = ImageIndex > 1;
                        CanMoveNext = ImageIndex < NumberOfImages;

                        //additional bitmap layer to draw OVER the image
                        ImageNotesLayer = new WriteableBitmap(_SelectedImageData.WriteableBitmapImage.PixelWidth, _SelectedImageData.WriteableBitmapImage.PixelHeight, _SelectedImageData.WriteableBitmapImage.DpiX, _SelectedImageData.WriteableBitmapImage.DpiY, PixelFormats.Bgra32, null);
                        
                        //setting the image pixels as transparent
                        byte[] pixels = new byte[ImageNotesLayer.PixelHeight * ImageNotesLayer.PixelWidth * ImageNotesLayer.Format.BitsPerPixel / 8];
                        for (int i = 0; i < pixels.Length; i++) pixels[i] = 0;
                        ImageNotesLayer.WritePixels(
                            new Int32Rect(0, 0, ImageNotesLayer.PixelWidth, ImageNotesLayer.PixelHeight),
                            pixels,
                            ImageNotesLayer.PixelWidth * ImageNotesLayer.
                             Format.BitsPerPixel / 8, 0);

                    }
                }
            }
        }

        /// <summary>
        /// Get\Set the Path with images inside.
        /// </summary>
        public string PathImagesFolder
        {
            get { return _PathImagesFolder; }
            set
            {
                //Save the Content before to change the value property
                SaveGlobalCommentFile(_PathImagesFolder);

                // Change the Property Value
                _PathImagesFolder = value;
                OnPropertyChanged("PathImagesFolder");

                // Invoke the event
                PathImageFolderChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Get\Set the File Name filter.
        /// Used to filter File Name 
        /// </summary>
        public string FileNameTextFilter
        {
            get { return _FileNameTextFilter; }
            set
            {
                _FileNameTextFilter = value;
                OnPropertyChanged("FileNameTextFilter");
            }
        }

        /// <summary>
        /// Get\Set the FromDateFilter Date Filter.
        /// </summary>
        public DateTime? FromDateFilter
        {
            get { return _FromDateFilter; }
            set
            {
                _FromDateFilter = value;
                OnPropertyChanged("FromDateFilter");
            }
        }

        /// <summary>
        /// Get\Set the ToDateFilter.
        /// </summary>
        public DateTime? ToDateFilter
        {
            get { return _ToDateFilter; }
            set
            {
                _ToDateFilter = value;
                OnPropertyChanged("ToDateFilter");
            }
        }

        /// <summary>
        /// Get\Set the Is Error Occured
        /// </summary>
        public bool IsErrorOccured
        {
            get { return _IsErrorOccured; }
            set
            {
                _IsErrorOccured = value;
                OnPropertyChanged("IsErrorOccured");

                if (value)
                {
                    IsLoadingImages = false;
                }
            }
        }

        /// <summary>
        /// Get\Set the Is Error Message
        /// </summary>
        public string ErrorMessage
        {
            get { return _ErrorMessage; }
            set
            {
                _ErrorMessage = value;
                IsErrorOccured = (!string.IsNullOrEmpty(value));

                OnPropertyChanged("ErrorMessage");
            }
        }

        /// <summary>
        /// Get\Set the Is Touch Application
        /// </summary>
        public bool IsTouchApplication
        {
            get
            {
                return LocalSettings.Instance.IsTouchApplication;
            }
        }

        /// <summary>
        /// Get\Set the Is Loading Images
        /// </summary>
        public bool IsLoadingImages
        {
            get { return _IsLoadingImages; }
            set
            {
                _IsLoadingImages = value;
                OnPropertyChanged("IsLoadingImages");

                if (value)
                {
                    IsErrorOccured = false;
                }
            }
        }

        /// <summary>
        /// Get\Set the information that specify whether is possible to move to the previus Image or not
        /// </summary>
        public bool CanMoveBack
        {
            get { return _CanMoveBack; }
            set
            {
                _CanMoveBack = value;
                OnPropertyChanged("CanMoveBack");
            }
        }

        /// <summary>
        /// Get\Set the information that specify whether is possible to move to the next Image or not
        /// </summary>
        public bool CanMoveNext
        {
            get { return _CanMoveNext; }
            set
            {
                _CanMoveNext = value;
                OnPropertyChanged("CanMoveNext");
            }
        }

        /// <summary>
        /// Get\Set the IsChildApplication.
        /// If true the Exit button and the Window Main Bar (with exit, minimaze and maximaze buttons) are not visible
        /// </summary>
        public bool IsChildApplication
        {
            get { return LocalSettings.Instance.ISChildApplication; }
        }

        /// <summary>
        /// Get\Set the Current Cause
        /// </summary>
        public string CurrentCause
        {
            get { return _CurrentCause; }
            set
            {
                _CurrentCause = value;
                OnPropertyChanged("CurrentCause");
            }
        }

        /// <summary>
        /// Get\Set the Current Action
        /// </summary>
        public string CurrentAction
        {
            get { return _CurrentAction; }
            set
            {
                _CurrentAction = value;
                OnPropertyChanged("CurrentAction");
            }
        }

        /// <summary>
        /// Get\Set the Additional Content.
        /// It is the content of the Additional.txt file
        /// </summary>
        public string AdditionalContent
        {
            get { return _AdditionalContent; }
            set
            {
                _AdditionalContent = value;
                OnPropertyChanged("AdditionalContent");
            }
        }

        /// <summary>
        /// Get\Set the Additional Content.
        /// It is the content of the Additional.txt file
        /// </summary>
        public string GlobalCommentContent
        {
            get { return _GlobalCommentContent; }
            set
            {
                _GlobalCommentContent = value;
                OnPropertyChanged("GlobalCommentContent");
            }
        }

        /// <summary>
        /// Get\Set the Additional Exist
        /// Specify whether the Additional content exists
        /// </summary>
        public bool AdditionalExist
        {
            get { return _AdditionalExist; }
            set
            {
                _AdditionalExist = value;
                OnPropertyChanged("AdditionalExist");
            }
        }

        /// <summary>
        /// Get\Set the Filters
        /// </summary>
        public Filters Filters
        {
            get { return _Filters; }
            set
            {
                _Filters = value;
                OnPropertyChanged("Filters");
            }
        }

        /// <summary>
        /// Specify whether the Additional Text must be shown or not.
        /// The Additional Text is a readonly Text. Its content is loaded from a file
        /// stored in the Image's folder and it is written by external application.
        /// This Additional information is used for customization and it shown
        /// based on the DEFINE "SHOW_ADDITIONAL".
        /// </summary>
        public bool ShowAdditional { get; private set; }

        /// <summary>
        /// transparent bitmap level above the bitmap image to draw points and fits
        /// </summary>
        public WriteableBitmap ImageNotesLayer
        {
            get { return _ImageNotesLayer; }
            set
            {
                _ImageNotesLayer = value;
                OnPropertyChanged("ImageNotesLayer");
            }
        }

        #endregion

        #region Event Handler

        private event EventHandler PageChanged;
        private event EventHandler PathImageFolderChanged;
        public event EventHandler CloseApplicationRequested;

        #endregion

        #region Commands

        /// <summary>
        /// Get\Set the MinimazeCommand
        /// </summary>
        public ICommand MinimazeCommand { get; private set; }

        /// <summary>
        /// Get\Set the MassimazeCommand
        /// </summary>
        public ICommand MassimazeCommand { get; private set; }

        /// <summary>
        /// Get\Set the PreviusCommand
        /// </summary>
        public ICommand PreviusCommand { get; private set; }

        /// <summary>
        /// Get\Set the NextCommand
        /// </summary>
        public ICommand NextCommand { get; private set; }

        /// <summary>
        /// Get\Set the SetImageInformationCommand
        /// </summary>
        public ICommand SetImageInformationCommand { get; private set; }

        /// <summary>
        /// Get\Set the SetActionInformationCommand
        /// </summary>
        public ICommand SetActionInformationCommand { get; private set; }

        /// <summary>
        /// Get\Set the CreateReportCommand
        /// </summary>
        public ICommand CreateReportCommand { get; private set; }

        /// <summary>
        /// Get\Set the OptionsCommand
        /// </summary>
        public ICommand OptionsCommand { get; private set; }

        /// <summary>
        /// Get\Set the OpenFiltersComman
        /// </summary>
        public ICommand OpenFiltersCommand { get; private set; }

        /// <summary>
        /// Get\Set the FilterChangedCommand
        /// </summary>
        public ICommand FilterChangedCommand { get; private set; }

        /// <summary>
        /// Get\Set the ClearTempFiltersCommand
        /// </summary>
        public ICommand ClearFiltersCommand { get; private set; }

        /// <summary>
        /// Get\Set the FolderDialogCommand
        /// </summary>
        public ICommand FolderDialogCommand { get; private set; }

        /// <summary>
        /// Get\Set the LoadCommand
        /// </summary>
        public ICommand LoadCommand { get; private set; }

        /// <summary>
        /// Get\Set the AbortLoadingCommand
        /// </summary>
        public ICommand AbortLoadingCommand { get; private set; }

        /// <summary>
        /// Get\Set the ChangePageCommand
        /// </summary>
        public ICommand ChangePageCommand { get; private set; }

        /// <summary>
        /// Get\Set the GetCoordinatesCommand
        /// </summary>
        public ICommand GetCoordinatesCommand { get; private set; }

        /// <summary>
        /// Get\Set the SetCoordinatesCommand
        /// </summary>
        public ICommand SetCoordinatesCommand { get; private set; }

        /// <summary>
        /// clear the coordinates taken
        /// </summary>
        public ICommand ClearCoordinatesCommand { get; private set; }

        /// <summary>
        /// Get\Set the ExitCommand
        /// </summary>
        public ICommand ExitCommand { get; private set; }

        #endregion

        #region Constructor

        public MainViewModel(BitmapImage resourceLogoBitmapImage)
        {
            _Log = new CommonLog();

            Filters = new Filters();

            // Create the Viewmodels
            _CreateReportViewModel = new CreateReportViewModel();

            // Create Managers
            _ImageManager = new ImageManager(_Log);
            _FileManager = new FileManager(_Log);
            _ImageFileManager = new ImageFileManager(_Log);
            _LogoManager = new LogoManager(resourceLogoBitmapImage, _Log);

            // Create the log for the Core 
            CoreLog.Instance.SetFunction(CoreLogType.Error, _Log.Debug);
            CoreLog.Instance.SetFunction(CoreLogType.Warning, _Log.Debug);

            // Create the Images Data Collection
            ImagesData = new ObservableCollection<ImageData>();

            // Create the Report Added Contents
            _ReportAddedContents = new Dictionary<ReportAddedContentType, string>();
            _ReportAddedContents.Add(ReportAddedContentType.Batch, string.Empty);
            _ReportAddedContents.Add(ReportAddedContentType.Additional, string.Empty);
            _ReportAddedContents.Add(ReportAddedContentType.Comments, string.Empty);

            // Create the DataSetCoordinates Collection
            _DatasetCoordinates = new List<Point>();

            // Initialize
            Initialize();

            // Create the Commands
            MinimazeCommand = new RelayCommand(MinimazeCommandExecute);
            MassimazeCommand = new RelayCommand(MassimazeCommandExecute);
            NextCommand = new RelayCommand(NextImageExecute);
            PreviusCommand = new RelayCommand(PreviusImageExtecute);
            SetImageInformationCommand = new RelayCommand(SetInformationExecute);
            CreateReportCommand = new RelayCommand(CreateReportExecute);
            OptionsCommand = new RelayCommand(OptionsExtecute);
            FilterChangedCommand = new RelayCommand(FilterChangedExecute);
            ClearFiltersCommand = new RelayCommand(ClearFiltersExecute);
            OpenFiltersCommand = new RelayCommand(OpenFiltersExecute);
            ExitCommand = new RelayCommand(ExitExecute);
            FolderDialogCommand = new RelayCommand(FolderDialogExecute);
            LoadCommand = new RelayCommand(LoadExecute);
            AbortLoadingCommand = new RelayCommand(AbortLoadingExecute);
            ChangePageCommand = new RelayCommand<ChangePageEventArgs>(ChangePageExecute);
            GetCoordinatesCommand = new RelayCommand<PointEventArgs>(GetCoordinatesExecute);            
            ClearCoordinatesCommand = new RelayCommand<EventArgs>(ClearCoordinatesExecute);            
        }

        #endregion

        #region Methods

        /// <summary>
        /// Register\Unregister the Events
        /// </summary>
        private void RegisterEvents(bool register)
        {
            if (register)
            {
                PageChanged += OnPageChanged;
                PathImageFolderChanged += OnPathImageFolderChanged;
                _CreateReportViewModel.Closed += OnCreateReportViewModelClosed;

            }
            else
            {
                PageChanged -= OnPageChanged;
                PathImageFolderChanged -= OnPathImageFolderChanged;
                _CreateReportViewModel.Closed -= OnCreateReportViewModelClosed;
            }
        }

        /// <summary>
        /// 1) Set the Properties
        /// 2) Set the number of images in one page
        /// 3) Set the language
        /// 4) Create the Last Credentials
        /// 5) Create the Formats Filter
        /// 6) Set the filters
        /// 7) Create the Logo
        /// 8) Register the events
        /// 9) Set the Software Type
        /// 10) Get the Favorite Folders from file
        /// </summary>
        private void Initialize()
        {
            try
            {
#if SHOW_ADDITIONAL
                ShowAdditional = true;
#else
                ShowAdditional = false;
#endif

                // Set Properties                      
                CanMoveNext = true;
                CanMoveBack = true;
                ErrorMessage = string.Empty;

                // Set the number of images in one page
                NumberOfImagesInPage = LocalSettings.Instance.NumberOfImagesInOnePage;

                // Change the language
                var cultureInfo = new CultureInfo(LocalSettings.Instance.Language);
                cultureInfo.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
                CultureResources.ChangeCulture(cultureInfo);
                Thread.CurrentThread.CurrentCulture = cultureInfo;

                // Create the Last Credentials              
                _LastCredentials = new Credentials(LocalSettings.Instance.LastCredentials);

                // Create the Formats Filter
                CreateTheImageFormatsFilter();

                // Set the filters
                SetInitialFilters();

                // Create the Default logo if not exists
                _LogoManager.CreateLogoFolderAndDefaultLogo();

                // Register the events
                RegisterEvents(true);

                // Set the Software Type
                SetTheSoftwareType();

                // Get the Favorite Folders from file
                _FavoriteFolders = _FileManager?.LoadFavoriteFolders();
            }
            catch (Exception ex)
            {
                _Log.Debug(ex.Message);
            }
        }

        /// <summary>
        /// Create the Image formats filter
        /// </summary>
        private void CreateTheImageFormatsFilter()
        {
            _ImageFormatsFilter = string.Empty;

            // Create the Image formats filter
            if (LocalSettings.Instance?.ImageFormats != null)
            {
                foreach (var format in LocalSettings.Instance?.ImageFormats)
                {
                    if (string.IsNullOrEmpty(_ImageFormatsFilter))
                    {
                        _ImageFormatsFilter = $"^{format}$";
                    }
                    else
                    {
                        _ImageFormatsFilter = $"{_ImageFormatsFilter}|^{format}$";
                    }
                }
            }
        }

        /// <summary>
        /// Set TheSoftware Type (Lite or Normal)
        /// </summary>
        private void SetTheSoftwareType()
        {
            if ((VersionTypeEnum)LocalSettings.Instance.VersionType == VersionTypeEnum.LiteVersion)
            {
                if (_CreateReportStatus != CreateReportStatusEnum.Error)
                {
                    CreateReportExecute(null);
                }
                else
                {
                    var isBlockedWindow = (VersionTypeEnum)LocalSettings.Instance.VersionType == VersionTypeEnum.LiteVersion;
                    var messageBox = new CustomMessageBox(MessageBoxType.Error, _AdditionalStatusMessage, MessageBoxButtonsType.OK);
                    messageBox.ShowMessageBox(isBlockedWindow);

                    // Exit from application
                    ExitExecute(null);
                }
            }
        }

        /// <summary>
        /// Set the initial filters 
        /// </summary>
        private void SetInitialFilters()
        {
            try
            {
                // Set the filters
                FileNameTextFilter = LocalSettings.Instance.Filter;

                if (!string.IsNullOrEmpty(LocalSettings.Instance.FromDate))
                {
                    var fromDate = DateTime.ParseExact(LocalSettings.Instance.FromDate, "yyyy-MM-dd", null);
                    //FromDateFilter = fromDate.ToString("yyyy-MM-dd");
                    FromDateFilter = fromDate;

                    if (!string.IsNullOrEmpty(LocalSettings.Instance.ToDate))
                    {
                        var toDate = DateTime.ParseExact(LocalSettings.Instance.ToDate, "yyyy-MM-dd", null);
                        //ToDateFilter = toDate.ToString("yyyy-MM-dd");
                        ToDateFilter = toDate;
                    }
                }
            }
            catch (Exception ex)
            {
                FileNameTextFilter = string.Empty;
                FromDateFilter = null;
                ToDateFilter = null;

                // Show Error message
                var isBlockedWindow = (VersionTypeEnum)LocalSettings.Instance.VersionType == VersionTypeEnum.LiteVersion;
                var messageBox = new CustomMessageBox(MessageBoxType.Error, $"Error parsing filters: {ex.Message} - Filters not applied", MessageBoxButtonsType.OK);
                messageBox.ShowMessageBox(isBlockedWindow);
            }
        }

        /// <summary>
        /// Load The additional file
        /// </summary>
        private void LoadAdditionalFile(string pathImage)
        {
            AdditionalContent = string.Empty;
            AdditionalExist = false;

            if (ShowAdditional)
            {
                // Get the Additional.txt file content
                var pathAdditionalFile = Path.Combine(pathImage, Constants.AdditionalFileName);
                if (File.Exists(pathAdditionalFile))
                {
                    var additionalFileContent = _FileManager.ReadTxtFile(pathAdditionalFile);
                    AdditionalContent = string.Join(Environment.NewLine, additionalFileContent);
                    AdditionalExist = true;
                }
            }
        }

        /// <summary>
        /// Load The Global Comments file
        /// </summary>
        private void LoadGlobalCommentslFile(string pathImage)
        {
            GlobalCommentContent = string.Empty;

            try
            {
                // Get the Additional.txt file content
                var pathFile = Path.Combine(pathImage, Constants.GlobalCommentFileName);
                if (File.Exists(pathFile))
                {
                    var globalCommentContent = _FileManager.ReadTxtFile(pathFile);
                    GlobalCommentContent = string.Join(Environment.NewLine, globalCommentContent);
                }
            }
            catch (Exception ex)
            {
                _Log.Debug(ex.Message);
            }
        }        

        /// <summary>
        /// Get the Filtered List
        /// </summary>
        private IEnumerable<string> GetFilteredList()
        {
            // Execute the filter action
            IEnumerable<string> filteredList = new List<string>(_FileNames);

            if (_FileNames != null)
            {
                if (string.IsNullOrEmpty(Filters.FileName) && Filters.FromDate == null && Filters.ToDate == null)
                {
                    return _FileNames;
                }
                else
                {
                    // Filter by FileName if needed
                    if (!string.IsNullOrEmpty(Filters.FileName))
                    {
                        filteredList = filteredList.Where(f => Path.GetFileName(f).Contains(Filters.FileName));
                    }

                    // Filter by FromDate and ToDate if needed
                    if (Filters.FromDate != null && (Filters.ToDate != null))
                    {
                        filteredList = filteredList.Where(f => new FileInfo(f).CreationTime.Date >= Filters.FromDate &&
                                                                new FileInfo(f).CreationTime.Date <= Filters.ToDate);
                    }
                    else
                    {
                        // Filter by FromDate if needed
                        if (Filters.FromDate != null)
                        {
                            filteredList = filteredList.Where(f => new FileInfo(f).CreationTime.Date >= Filters.FromDate);
                        }

                        // Filter by ToDate if needed
                        if (Filters.ToDate != null)
                        {
                            filteredList = filteredList.Where(f => new FileInfo(f).CreationTime.Date <= Filters.ToDate);
                        }
                    }
                }
            }

            return filteredList;
        }

        /// <summary>
        /// Returns:
        /// 1. The number of pages
        /// 2. The index of the first image in the page
        /// 3. The number of element in the Range
        /// </summary>
        private (int numberOfPages, int firstIndex, int numberOfElementInRange) GetPageParameters(int numberOfImages, int numberOfImagesInPage)
        {
            // Calculate the Number of Pages
            var numberOfPages = numberOfImages / numberOfImagesInPage;
            if (numberOfImages % numberOfImagesInPage > 0)
            {
                numberOfPages = numberOfPages + 1;
            }

            // Calculate the Range                   
            var firstIndex = (PageIndex * numberOfImagesInPage);
            if (firstIndex < 0)
            {
                firstIndex = 0;
            }

            // Get the number of element in the range
            var numberOfElementInRange = (firstIndex + numberOfImagesInPage <= numberOfImages) ?
                                        numberOfImagesInPage :
                                        numberOfImages - firstIndex;

            return (numberOfPages, firstIndex, numberOfElementInRange);
        }

        /// <summary>
        /// Create the image data from loaded path image
        /// </summary>
        private List<ImageData> CreateTheImageDataCollection(string targetDirectory)
        {
            var imageDataCollection = new List<ImageData>();

            try
            {
                // Get the Filtered list
                var filteredList = GetFilteredList()?.ToList();
                if (filteredList != null && filteredList.Any())
                {
                    NumberOfImages = filteredList.Count();
                    IsErrorOccured = false;

                    // GEt:
                    /// 1. The number of pages
                    /// 2. The index of the first image in the page
                    /// 3. The number of element in the Range
                    var result = GetPageParameters(NumberOfImages, NumberOfImagesInPage);
                    _NumberOfPages = result.numberOfPages;

                    // Get the elements in range
                    filteredList = filteredList.GetRange(result.firstIndex, result.numberOfElementInRange);

                    // Create the Image Data of elements
                    foreach (var imagePath in filteredList)
                    {
                        if (!string.IsNullOrEmpty(imagePath))
                        {
                            var imageData = CreateImageData(imagePath);

                            // Add the created Image Data to the collection
                            if (imageData != null)
                            {
                                imageDataCollection.Add(imageData);
                            }
                        }
                    }
                }
                else
                {
                    NumberOfImages = 0;
                    if (SelectedImageData != null)
                    {
                        SelectedImageData.WriteableBitmapImage = null;
                    }
                    IsErrorOccured = true;
                    ImageIndex = 0;
                    CanMoveBack = false;
                    CanMoveNext = false;
                    ErrorMessage = CultureResources.GetString("Message_Images_Not_Found");
                }
            }
            catch (Exception ex)
            {
                _Log.Debug(ex.Message);
                throw;
            }

            return imageDataCollection;
        }

        /// <summary>
        /// Create an Image Data from Path
        /// </summary>
        private ImageData CreateImageData(string imagePath)
        {
            var fileExt = Path.GetExtension(Path.GetFileName(imagePath));
            if (IsImageFilterMatched(imagePath, _ImageFormatsFilter))
            {
                // Create the writeable bitmap
                WriteableBitmap writeableBitmap = null;
                if (fileExt.ToLower() == ".16" || fileExt.ToLower() == ".tif")
                {
                    writeableBitmap = _ImageManager.GetWriteableBitmap(imagePath);
                    writeableBitmap = _ImageManager.Rotate(writeableBitmap, LocalSettings.Instance.RotationAngle);
                }
                else
                {
                    var bitmapImage = _ImageManager.Rotate(imagePath, LocalSettings.Instance.RotationAngle);
                    writeableBitmap = new WriteableBitmap(bitmapImage);
                }

                // Create the Cretion Date
                var creationDateTime = File.GetCreationTime(imagePath);

                // Load the Action and Cause informations
                string cause = string.Empty;
                string action = string.Empty;

                var imageData = _FileManager.LoadImageInformationsFile(PathImagesFolder, Path.GetFileNameWithoutExtension(imagePath));
                if (imageData != null)
                {
                    cause = imageData.Cause;
                    action = imageData.Action;
                }

                //Freeze the writeable Bitmap
                writeableBitmap.Freeze();

                

                // Add Image Data to the collection
                return new ImageData(writeableBitmap, LocalSettings.Instance.RotationAngle, imagePath, creationDateTime, cause, action);
            }

            return null;
        }

        /// <summary>
        /// Specify whether the specificated image match the filter.
        /// The normal extension are ".tif", ".bmp", ".jpg", ".png", ".16"
        /// The extended extension is "_16.tif" (Constants.Image16BitFile)
        /// </summary>
        private bool IsImageFilterMatched(string imagePath, string filter)
        {
            if (imagePath.ToLower().EndsWith(Constants.Image16BitFile))
            {
                return _ImageFormatsFilter.Contains(".16");
            }

            var fileExt = Path.GetExtension(Path.GetFileName(imagePath));
            return (Regex.IsMatch(fileExt, _ImageFormatsFilter, RegexOptions.IgnoreCase));
        }

        /// <summary>
        /// This method create the file name in according to Lot name and the Date filters.
        /// Save the Report in specificated Directory or in the Default ReportsFolderName directory
        /// </summary>
        /// <returns></returns>
        private string CreateReportPathFileName(string lotName)
        {
            var pathFileName = string.Empty;

            // Choose the Directory where save the Report
            var reportPath = string.Empty;

            if ((VersionTypeEnum)LocalSettings.Instance.VersionType == VersionTypeEnum.LiteVersion)
            {
                if (Directory.Exists(LocalSettings.Instance.PDFSavePath))
                {
                    reportPath = LocalSettings.Instance.PDFSavePath;
                }
            }
            else
            {
                var selectedPath = string.Empty;
                if (!string.IsNullOrEmpty(LocalSettings.Instance?.PDFSavePath) &&
                    Directory.Exists(LocalSettings.Instance?.PDFSavePath))
                {
                    selectedPath = LocalSettings.Instance.PDFSavePath;
                }
                else
                {
                    selectedPath = PathImagesFolder;
                }

                // Choose the folder to save the Report
                var favorites = CreateFavoriteFoldersFromSettings();

                var title = CultureResources.GetString("Label_Select_Report_Destination");

                var folderDialog = new NavigationFolderDialog(selectedPath, title, _LastCredentials.Domain, favorites);
                folderDialog.ShowWindow(true);

                // Update the favorite folder list if necessary
                if (folderDialog.IsFavoriteDirty)
                {
                    _FavoriteFolders.Favorites = CreateSharedFolderFromFavoriteFolders(folderDialog.FavoriteFolders).ToList();
                }

                if (folderDialog.Result == DialogResult.OK)
                {
                    reportPath = folderDialog.SelectedFolderPath;
                }
                else
                {
                    return string.Empty;
                }
            }

            // Create the file name of the Report
            var fileName = string.Empty;

            if (FromDateFilter != null)
            {
                var from = FromDateFilter.Value.ToString("yyyy-MM-dd");
                if (ToDateFilter != null)
                {
                    var to = ToDateFilter.Value.ToString("yyyy-MM-dd");
                    fileName = $"{lotName}_FROM_{from.Replace(@"/", "_")}_TO_{to.Replace(@"/", "_")}.pdf";
                }
                else
                {
                    fileName = $"{lotName}_{from.Replace(@"/", "_")}.pdf";
                }
            }
            else
            {
                fileName = $"{lotName}.pdf";
            }

            // Create the full path
            if (Directory.Exists(reportPath))
            {
                pathFileName = Path.Combine(reportPath, fileName);

                // Change status
                _CreateReportStatus = CreateReportStatusEnum.Creating;
                _AdditionalStatusMessage = string.Empty;
            }
            else
            {
                pathFileName = Path.Combine(Constants.ReportsFolderName, fileName);

                // Change status
                _CreateReportStatus = CreateReportStatusEnum.Warning;
                _AdditionalStatusMessage = CultureResources.GetString("Message_Warning_Path_Not_Exists");
            }

            return pathFileName;
        }

        /// <summary>
        /// Create a list of favorite folders from settings
        /// </summary>
        /// <returns></returns>
        private IList<FavoriteFolder> CreateFavoriteFoldersFromSettings()
        {
            var result = new List<FavoriteFolder>();

            if (_FavoriteFolders?.Favorites != null)
            {
                foreach (var sharedFolder in _FavoriteFolders?.Favorites)
                {
                    if (sharedFolder != null)
                    {
                        var favoriteFolder = new FavoriteFolder(sharedFolder.FriendlyName, sharedFolder.Folder, sharedFolder.Domain, sharedFolder.Username, sharedFolder.Password);
                        result.Add(favoriteFolder);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Create a list of shared folders in Settings from Favorite folders
        /// </summary>
        /// <returns></returns>
        private IList<SharedFolder> CreateSharedFolderFromFavoriteFolders(IList<FavoriteFolder> favoriteFolders)
        {
            var result = new List<SharedFolder>();

            if (favoriteFolders != null)
            {
                foreach (var favoriteFolder in favoriteFolders)
                {
                    if (favoriteFolder != null)
                    {
                        var sharedFolder = new SharedFolder(favoriteFolder.FriendlyName, favoriteFolder.Folder, favoriteFolder.Domain, favoriteFolder.Username, favoriteFolder.Password);
                        result.Add(sharedFolder);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Save the GlobalComment.txt file
        /// </summary>
        private async void SaveGlobalCommentFile(string commentPath)
        {
            try
            {
                if (!string.IsNullOrEmpty(commentPath) && !IsErrorOccured)
                {
                    await Task.Run(() =>
                    {
                        var path = Path.Combine(commentPath, Constants.GlobalCommentFileName);
                        File.WriteAllText(path, GlobalCommentContent);
                    });
                }
            }
            catch (Exception ex)
            {
                _Log.Debug(ex.Message);
            }
        }

        /// <summary>
        /// Get the article name from files.
        /// The last part of string must match the pattern 'YYYYMMDD-HHmmSSXXX':
        /// YYYY: Year (ex. 2020)
        /// MM: Month (ex. 01)
        /// DD: Day (ex. 01)
        /// HH: Hour (ex. 04)
        /// mm: Minutes (ex. 07)
        /// SS: seconds (ex. 00)
        /// XXX: progressive number from 000 to 999
        /// </summary>
        private string GetArticleNameFromFiles(IEnumerable<string> paths)
        {
            var articleName = string.Empty;
            var numberOfFilesMatched = 0;

            if (paths != null && paths.Any())
            {
                foreach (var path in paths)
                {
                    if (!string.IsNullOrEmpty(path))
                    {
                        var fileName = Path.GetFileNameWithoutExtension(paths.ElementAt(0));
                        var splittedString = fileName.Split('_');
                        if (splittedString.Count() > 0)
                        {
                            // Check whether the file name match the Pattern
                            var pattern = splittedString.Last();
                            var patternIsMatched = false;
                            if (pattern.Count() == 18)
                            {
                                var splittedPattern = pattern.Split('-');
                                if (splittedPattern != null && splittedPattern.Count() == 2)
                                {
                                    // Check whehter the first part of pattern match 'YYYYMMDD'
                                    var firstPathIsMatched = false;
                                    var firstPart = splittedPattern.ElementAt(0);
                                    if (firstPart.Count() == 8)
                                    {
                                        var isYearConverted = int.TryParse(firstPart.Substring(0, 4), out int year);
                                        var isMonthConverted = int.TryParse(firstPart.Substring(4, 2), out int month);
                                        var isDayConverted = int.TryParse(firstPart.Substring(6, 2), out int day);

                                        firstPathIsMatched = isYearConverted && isMonthConverted && isDayConverted &&
                                                             (month >= 1 && month <= 12) && (day >= 1 && day <= 31);

                                        var secondPartIsMatched = false;
                                        if (firstPathIsMatched)
                                        {
                                            var secondPart = splittedPattern.ElementAt(1);
                                            if (secondPart.Count() == 9)
                                            {
                                                var isHourConverted = int.TryParse(secondPart.Substring(0, 2), out int hour);
                                                var isMinutesConverted = int.TryParse(secondPart.Substring(2, 2), out int minutes);
                                                var isSecondsConverted = int.TryParse(secondPart.Substring(4, 2), out int seconds);
                                                var isProgressiveConverted = int.TryParse(secondPart.Substring(6, 3), out int progressive);

                                                secondPartIsMatched = isHourConverted && isMinutesConverted && isSecondsConverted &&
                                                                     isProgressiveConverted && (progressive >= 0 && progressive <= 999);
                                            }
                                        }

                                        patternIsMatched = firstPathIsMatched && secondPartIsMatched;
                                    }
                                }
                            }

                            if (patternIsMatched)
                            {

                                // Check the View and image type
                                var count = splittedString.Count();
                                var imageType = splittedString.ElementAt(count - 2).ToUpper();
                                var view = splittedString.ElementAt(count - 3).ToUpper();

                                // Get the article name
                                patternIsMatched = (view == "L" || view == "R") &&
                                    (imageType == "B" || imageType == "G" || imageType == "W" || imageType == "E");

                                if (patternIsMatched)
                                {
                                    // The article name is calculate only one time
                                    if (string.IsNullOrEmpty(articleName))
                                    {
                                        articleName = splittedString[0];
                                        for (var index = 1; index <= count - 4; index++)
                                        {
                                            var text = splittedString[index];
                                            articleName = $"{articleName}_{text}";
                                        }
                                    }

                                    // Increments the number of files matched
                                    numberOfFilesMatched++;
                                }
                            }
                        }
                    }
                }
            }

            var limit = paths?.Count() / 2;
            if (numberOfFilesMatched < limit)
            {
                articleName = "------------";
            }

            return articleName;
        }

        /// <summary>
        /// Save the settings 
        /// </summary>
        private void SaveSettings()
        {
            try
            {
                LocalSettings.Instance.LastCredentials = _LastCredentials;
                LocalSettings.Instance.StartFolderPath = PathImagesFolder;
                LocalSettings.Instance.Filter = FileNameTextFilter;
                LocalSettings.Instance.FromDate = FromDateFilter?.ToString("yyyy-MM-dd");
                LocalSettings.Instance.ToDate = ToDateFilter?.ToString("yyyy-MM-dd");

                LocalSettings.Instance.Save();
            }
            catch (Exception ex)
            {
                _Log.Debug(ex.Message);
            }
        }

        /// <summary>
        /// Clear all the Information
        /// </summary>
        private void ClearInformation()
        {
            ImagesData = new ObservableCollection<ImageData>();
            if (SelectedImageData != null)
            {
                SelectedImageData.WriteableBitmapImage = null;
                SelectedImageData = null;
            }

            IsErrorOccured = false;
            ErrorMessage = string.Empty;
        }

        /// <summary>
        /// Create the Images List
        /// </summary>
        private async Task LoadTheImagesList()
        {
            try
            {
                // Create the ImagesData from loaded files path
                var imagesData = new List<ImageData>();

                await Task.Run(() =>
                {
                    IsLoadingImages = true;
                    imagesData = CreateTheImageDataCollection(PathImagesFolder);

                    IsLoadingImages = false;
                });

                // Fills the collection of ImageData
                ImagesData = new ObservableCollection<ImageData>(imagesData);

                if (ImagesData.Any())
                {
                    IsErrorOccured = false;
                    SelectedImageData = _IsPageMovedBack ? SelectedImageData = ImagesData?.Last() : ImagesData?.First();
                }

                _IsPageMovedBack = false;
            }
            catch (Exception ex)
            {
                _Log.Debug(ex.Message);
            }
        }

        /// <summary>
        /// Set the Error 
        /// </summary>
        private void SetError(string errorMessage)
        {
            ImageIndex = 0;

            if (SelectedImageData != null)
            {
                SelectedImageData.WriteableBitmapImage = null;
            }

            CanMoveBack = false;
            CanMoveNext = false;
            IsErrorOccured = true;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Change the the Last Credentials with a favorite information with the specificated Path
        /// </summary>
        private void UpdateTheLastCredentials(string path)
        {
            // Change the Settings
            var favoriteFolder = _FavoriteFolders.Favorites?.FirstOrDefault(f => f.Folder == path);
            if (favoriteFolder != null)
            {
                // Update the Last Credentials
                _LastCredentials.Set(favoriteFolder.Domain, favoriteFolder.Username, favoriteFolder.Password);
            }
        }

        /// <summary>
        /// Trigger the Page Changed Event
        /// </summary>
        private void TriggerPageChangedEvent(object sender, EventArgs eventArgs)
        {
            PageChanged?.Invoke(sender, eventArgs);
        }

        /// <summary>
        /// Invoke the Close Application requested Event
        /// </summary>
        private void CloseApplicationRequestedInvoke(object sender, EventArgs eventArgs)
        {
            CloseApplicationRequested?.Invoke(sender, eventArgs);
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the Page is changed
        /// </summary>        
        private async void OnPageChanged(object sender, EventArgs e)
        {
            // Load the Images
            if (!string.IsNullOrEmpty(PathImagesFolder) && !IsLoadingImages)
            {
                await LoadTheImagesList();
            }
        }

        /// <summary>
        /// Occurs when the Path Image Folder is changed
        /// </summary>
        private async void OnPathImageFolderChanged(object sender, EventArgs e)
        {
            var errorMessage = string.Empty;

            if (!string.IsNullOrEmpty(PathImagesFolder))
            {
                // Set the Parameters  
                IsLoadingImages = true;
                NumberOfImages = 0;

                // Change the Last Credentials
                UpdateTheLastCredentials(PathImagesFolder);

                // Try to get the image file names
                var filesResult = await _ImageFileManager.GetFileNamesFromPath(PathImagesFolder, _ImageFormatsFilter, _LastCredentials, _FavoriteFolders?.Favorites);

                // Set the list of file names
                _FileNames = filesResult.FileNames;

                if (filesResult.IsSuccess)
                {
                    // Load the Images List
                    await LoadTheImagesList();

                    // Get the Additional and GlobalComments file
                    LoadAdditionalFile(PathImagesFolder);
                    LoadGlobalCommentslFile(PathImagesFolder);
                }
                else
                {
                    SetError(filesResult.Message);
                }
            }
            else
            {
                SetError(CultureResources.GetString("Message_Empty_Path"));
            }

            IsLoadingImages = false;
        }

        /// <summary>
        /// Occurs when the user wants to Minimaze the window
        /// </summary>
        private void MinimazeCommandExecute(object param)
        {
            if (param is Window window)
            {
                window.WindowState = System.Windows.WindowState.Minimized;
            }
        }

        /// <summary>
        /// Occurs when the user wants to Massimaze the window
        /// </summary>
        private void MassimazeCommandExecute(object param)
        {
            if (param is Window window)
            {
                if (window.WindowState == WindowState.Normal)
                {
                    window.WindowState = System.Windows.WindowState.Maximized;
                }
                else
                {
                    window.WindowState = System.Windows.WindowState.Normal;
                }
            }
        }

        /// <summary>
        /// Occurs when the Next Image must be shown
        /// </summary>
        private void NextImageExecute(object param)
        {
            // Get the index of the selected image data
            var selectedIndex = ImagesData.IndexOf(SelectedImageData);
            selectedIndex = selectedIndex + 1;

            if (selectedIndex < ImagesData.Count())
            {
                SelectedImageData = ImagesData.ElementAt(selectedIndex);
            }
            else
            {
                // if is not the last page, move forward to next page
                if (PageIndex + 1 < _NumberOfPages)
                {
                    PageIndex = PageIndex + 1; // The change in the PageIndex triggers the loading of images
                }
            }
        }

        /// <summary>
        /// Occurs when the Previus Image must be shown
        /// </summary>
        private void PreviusImageExtecute(object param)
        {
            if (ImagesData != null && ImagesData.Count() > 0)
            {
                var selectedIndex = ImagesData.IndexOf(SelectedImageData);
                selectedIndex = selectedIndex - 1;

                if (selectedIndex >= 0)
                {
                    SelectedImageData = ImagesData.ElementAt(selectedIndex);
                }
                else
                {
                    // if is not the first page, move back to previus page
                    if (PageIndex - 1 >= 0)
                    {
                        _IsPageMovedBack = true;
                        PageIndex = PageIndex - 1; // The change in the PageIndex triggers the loading of images
                    }
                }
            }
        }

        /// <summary>
        /// Occurs when the Textbox  or TextBoxKeyboard of Cause or Action information lost focus 
        /// Exists two different methods for the touch and normal input
        /// </summary>
        private void SetInformationExecute(object param)
        {
            var backupCurrentCase = SelectedImageData.Cause;
            var backupCurrentAction = SelectedImageData.Action;

            try
            {
                if (SelectedImageData != null && (SelectedImageData.Cause != CurrentCause || SelectedImageData.Action != CurrentAction))
                {
                    SelectedImageData.Cause = CurrentCause;
                    SelectedImageData.Action = CurrentAction;

                    // Save the information on file
                    _FileManager.SaveImageInformationsFile(PathImagesFolder, SelectedImageData);
                }
            }
            catch (Exception ex)
            {
                var messageBox = new CustomMessageBox(MessageBoxType.Error, ex.Message, MessageBoxButtonsType.OK);
                messageBox.ShowMessageBox();

                // Restore the last information 
                SelectedImageData.Cause = backupCurrentCase;
                SelectedImageData.Action = backupCurrentAction;
                CurrentCause = backupCurrentCase;
                SelectedImageData.Action = backupCurrentAction;
            }
        }

        /// <summary>
        /// Open the Options Dialog
        /// </summary>
        private void OptionsExtecute(object param)
        {
            _OptionsViewModel = new OptionsViewModel(_Log, LocalSettings.Instance);
            RegisterExternalDialogEvents(true);

            // Open the External Dialog
            ExternalDialogManager.Instance.OpenDialog(_OptionsViewModel);
        }

        /// <summary>
        /// Occurs when the Options are saved
        /// </summary>
        private void OnOptionsViewModelSaved(object sender, Options options)
        {
            _Options = options;
            if (options != null)
            {
                if (options.NeedReloaded)
                {
                    // Change the Number of Images in a single Page
                    NumberOfImagesInPage = options.Settings.NumberOfImagesInOnePage;

                    // Create the Formats Filter
                    CreateTheImageFormatsFilter();

                    OnPathImageFolderChanged(this, EventArgs.Empty);

                    RegisterExternalDialogEvents(false);
                }
            }
        }

        /// <summary>
        /// Occurs when The OptionsViewModel is closed       
        /// </summary>
        private void OnOptionsViewModelClosed(object sender, EventArgs e)
        {
            RegisterExternalDialogEvents(false);
        }

        /// <summary>
        /// Register\Unregister the External Dialog Events
        /// </summary>
        private void RegisterExternalDialogEvents(bool register)
        {
            if (register)
            {
                _OptionsViewModel.Saved += OnOptionsViewModelSaved;
                _OptionsViewModel.Closed += OnOptionsViewModelClosed;
            }
            else
            {
                _OptionsViewModel.Saved -= OnOptionsViewModelSaved;
                _OptionsViewModel.Closed -= OnOptionsViewModelClosed;
            }
        }

        /// <summary>
        /// Create the PDF Report
        /// </summary>
        private void CreateReportExecute(object param)
        {
            try
            {
                IPdfCreatorManager pdfCreatorManager = null;

                var logoPath = _LogoManager.GetLogoPath();
                var reportpathFileName = string.Empty;
                List<string> filteredList = null;

                if (!string.IsNullOrEmpty(PathImagesFolder))
                {
                    // string articleName = string.Empty;

                    // Get the Lot and Article names from the Path
                    var lotName = new DirectoryInfo(PathImagesFolder).Name;
                    //var parent = new DirectoryInfo(PathImagesFolder).Parent;
                    //if (parent != null)
                    //{
                    //    articleName = new DirectoryInfo(PathImagesFolder).Parent.Name;
                    //}

                    // Get the path file name
                    reportpathFileName = CreateReportPathFileName(lotName);

                    // Create the Report
                    if (!string.IsNullOrEmpty(reportpathFileName))
                    {
                        // Read the Batch file
                        var pathApplication = $@"{Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)}";
                        var bacthFilePath = Path.Combine(pathApplication, Constants.BatchFileName);
                        var batchFileContent = _FileManager.ReadTxtFile(bacthFilePath);

                        // Get the filtered list
                        filteredList = GetFilteredList()?.ToList();

                        // Get the name of article from file name                   
                        var articleName = GetArticleNameFromFiles(filteredList);

                        _ReportAddedContents[ReportAddedContentType.Batch] = string.Join(string.Empty, batchFileContent);
                        _ReportAddedContents[ReportAddedContentType.Additional] = AdditionalContent;
                        _ReportAddedContents[ReportAddedContentType.Comments] = string.Join(string.Empty, GlobalCommentContent);

                        var reportType = (ReportTypeEnum)LocalSettings.Instance.ReportType;
                        pdfCreatorManager = PdfCreatorFactory.Get(reportType, _Log, batchFileContent, _ReportAddedContents, filteredList,
                                                                      lotName, articleName, LocalSettings.Instance);
                    }
                }
                else if ((VersionTypeEnum)LocalSettings.Instance.VersionType == VersionTypeEnum.LiteVersion)
                {
                    _CreateReportStatus = CreateReportStatusEnum.Error;
                    _AdditionalStatusMessage = CultureResources.GetString("Message_Images_Not_Found");
                }

                var reportMustBeCreated = true;
                if (!string.IsNullOrEmpty(reportpathFileName))
                {
                    if (filteredList != null && filteredList.Count() > 1000)
                    {
                        var numberOfReports = filteredList.Count / 1000;
                        if (filteredList.Count % 1000 > 0)
                        {
                            numberOfReports++;
                        }

                        var message = string.Format(CultureResources.GetString("Message_Reports_Partioned"), numberOfReports);
                        var messageBox = new CustomMessageBox(MessageBoxType.Confirm, message, MessageBoxButtonsType.OK_Cancel);
                        messageBox.ShowDialog();

                        reportMustBeCreated = messageBox.Result == Core.MessageBoxResult.Yes || messageBox.Result == Core.MessageBoxResult.OK;
                    }

                    if (reportMustBeCreated)
                    {
                        var showPreview = LocalSettings.Instance.OpenReportAfterCreated;

                        _CreateReportViewModel.CreateReport(_CreateReportStatus, _AdditionalStatusMessage, reportpathFileName, showPreview,
                            logoPath, _Log, pdfCreatorManager);

                        // Open the External Dialog
                        ExternalDialogManager.Instance.OpenDialog(_CreateReportViewModel);
                    }
                }
            }
            catch (Exception ex)
            {
                var messageBox = new CustomMessageBox(MessageBoxType.Error, ex.Message, MessageBoxButtonsType.OK);
                messageBox.ShowDialog();

                _Log.Debug(ex.Message);
            }
        }

        /// <summary>
        /// Occurs when the report is created and the Dialog closed
        /// </summary>
        private void OnCreateReportViewModelClosed(object sender, EventArgs e)
        {
            if ((VersionTypeEnum)LocalSettings.Instance.VersionType == VersionTypeEnum.LiteVersion)
            {
                ExitExecute(null);
            }
        }

        /// <summary>
        /// Occurs when the Filters must be opened\closed
        /// </summary>
        private void OpenFiltersExecute(object param)
        {
            if (Filters != null)
            {
                Filters.AreFiltersOpened = !Filters.AreFiltersOpened;
            }
        }

        /// <summary>
        /// Accours when the filters changes
        /// </summary>
        private void FilterChangedExecute(object param)
        {          
            // Update the Filters
            if (Filters != null &&  !string.IsNullOrEmpty(PathImagesFolder) && !IsLoadingImages)
            {
                Filters.UpdateFilters();

                // The change in the PageIndex triggers the loading of images
                PageIndex = 0;
            }
        }

        /// <summary>
        /// Occurs when the filters are Cleared
        /// </summary>
        private void ClearFiltersExecute(object param)
        {
            // Clear the Filters
            if (Filters != null && !string.IsNullOrEmpty(PathImagesFolder) && !IsLoadingImages)
            {
                Filters.TableFiltersReset();

                // The change in the PageIndex triggers the loading of images
                PageIndex = 0;
            }
        }

        /// <summary>
        /// The Exit Command Execute 
        /// </summary>
        private void ExitExecute(object param)
        {
            RegisterEvents(false);

            // Disconnect the Connected Net Folder
            ConnectionNetFolderWrapper.Instance.ConnectToSharedFolder.Disconnect();

            // Save Files
            SaveGlobalCommentFile(PathImagesFolder);
            SaveSettings();

            // Save the Favorite Folders
            _FileManager?.SaveFavoriteFolders(_FavoriteFolders);

            // Dispose all the objects
            _CreateReportViewModel?.Dispose();

            // Invoke the event
            CloseApplicationRequestedInvoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// The Load Command Execute 
        /// </summary>
        private void LoadExecute(object param)
        {
            PathImagesFolder = LocalSettings.Instance.StartFolderPath;
        }

        /// <summary>
        /// The Abort Loading Command Execute 
        /// </summary>
        private void AbortLoadingExecute(object param)
        {
            IsErrorOccured = true;            
            ErrorMessage = CultureResources.GetString("Loading images aborted by user");
        }

        /// <summary>
        /// Occurs to change the Page
        /// </summary>
        private void ChangePageExecute(object param)
        {
            if (param is ChangePageEventArgs changePageEventArgs)
            {
                var pageIndex = changePageEventArgs.CurrentPage - 1;
                if (pageIndex >= 0 && pageIndex < _NumberOfPages)
                {
                    PageIndex = pageIndex;
                }                
            }
        }

        /// <summary>
        /// Occurs to Get the Coordinates
        /// </summary>
        private void GetCoordinatesExecute(object param)
        {
            if (param is PointEventArgs coordinates)
            {
                _DatasetCoordinates.Add(coordinates.Coordinates);

                // drawing a cross over the dataset point
                for (int i = 0; i < _DatasetCoordinates.Count; i++)
                {
                    try
                    {
                        var width = 7;
                        var height = 7;
                        var X_coord = (int)_DatasetCoordinates[i].X - width / 2;
                        var Y_coord = (int)_DatasetCoordinates[i].Y - height / 2;

                        var rect = new Int32Rect(X_coord, Y_coord, width, height);

                        var stride = (width * ImageNotesLayer.Format.BitsPerPixel + 7) / 8;
                        byte[] colorData = new byte[stride * height];

                        for (int j = 0; j < width; j ++)
                        {
                            for (int k = 0; k < height; k++)
                            {
                                if ( j == k  || j == height - k - 1) // cross pattern
                                {
                                    colorData[(height * k + j) * ImageNotesLayer.Format.BitsPerPixel / 8 + 0 ] = 0; // B blue
                                    colorData[(height * k + j) * ImageNotesLayer.Format.BitsPerPixel / 8 + 1 ] = 0; // G green
                                    colorData[(height * k + j) * ImageNotesLayer.Format.BitsPerPixel / 8 + 2 ] = 255; // R red
                                    colorData[(height * k + j) * ImageNotesLayer.Format.BitsPerPixel / 8 + 3 ] = 255; // A alpha
                                }
                            }
                        }
                        ImageNotesLayer.WritePixels(rect, colorData, stride, 0);
                    }
                    catch
                    {
                        //this handles the case where I try to create a point on the edge of the image
                    }
                }
            }
        }
        /// <summary>
        /// Open the Folder Dialog to choose a folder
        /// </summary>
        private void FolderDialogExecute(object param)
        {
            try
            {
                // Choose  the folder 
                var title = CultureResources.GetString("Caption_Select_The_Images_Folder");

                var favorites = CreateFavoriteFoldersFromSettings();

                var folderDialog = new NavigationFolderDialog(string.Empty, title, _LastCredentials.Domain, favorites);
                folderDialog.ShowWindow(true);

                // Update the favorite folder list if necessary
                if (folderDialog.IsFavoriteDirty)
                {
                    _FavoriteFolders.Favorites = CreateSharedFolderFromFavoriteFolders(folderDialog.FavoriteFolders).ToList();
                }

                if (folderDialog.Result == DialogResult.OK)
                {
                    // Set tha path images folder
                    PathImagesFolder = folderDialog.SelectedFolderPath;

                    // Clear the latest information
                    ClearInformation();

                    // The change in the PageIndex triggers the loading of images                 
                    PageIndex = 0;
                }
            }
            catch(Exception ex)
            {
                _Log.Debug(ex.Message);
            }
        }

        private void ClearCoordinatesExecute(object param)
        {
            _DatasetCoordinates.Clear();

            //clear the bitmap
            Int32Rect rect = new Int32Rect(0, 0, ImageNotesLayer.PixelWidth, ImageNotesLayer.PixelHeight);
            int bytesPerPixel = ImageNotesLayer.Format.BitsPerPixel / 8;
            byte[] empty = new byte[rect.Width * rect.Height * bytesPerPixel]; // cache this one
            int emptyStride = rect.Width * bytesPerPixel;
            ImageNotesLayer.WritePixels(rect, empty, emptyStride, 0);
        }

        #endregion
    }
}
