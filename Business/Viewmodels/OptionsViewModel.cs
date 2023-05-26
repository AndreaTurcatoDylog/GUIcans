using Common;
using Core;
using Core.ResourceManager.Cultures;
using Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Business
{
    public class Options : ModelBase
    {
        #region Members

        private Models.Settings _Settings;
        private string _Language;
        private int _NumberOfImagesInPage;
        private BitmapImage _LogoImage;

        #endregion

        #region Properties

        /// <summary>
        /// Get\Set the LogoImage
        /// </summary>
        public BitmapImage LogoImage
        {
            get { return _LogoImage; }
            set
            {
                _LogoImage = value;
                OnPropertyChanged("LogoImage");
            }
        }

        /// <summary>
        /// Get\Set the Language
        /// </summary>
        public string Language
        {
            get { return _Language; }
            set
            {
                _Language = value;
                OnPropertyChanged("Language");
            }
        }

        /// <summary>
        /// Get\Set the Language
        /// </summary>
        public Models.Settings Settings
        {
            get { return _Settings; }
            set
            {
                _Settings = value;
                OnPropertyChanged("Settings");
            }
        }

        /// <summary>
        /// Get\Set the Number of Images in one Page
        /// </summary>
        public int NumberOfImagesInOnePage
        {
            get { return _NumberOfImagesInPage; }
            set
            {
                _NumberOfImagesInPage = value;
                OnPropertyChanged("NumberOfImagesInPage");
            }
        }

        /// <summary>
        /// Get\Set the Nedd Reloaded
        /// </summary>
        public bool NeedReloaded { get; set; }

        #endregion
    }

    /// <summary>
    /// This class manages the Options
    /// </summary>
    public class OptionsViewModel : ViewModelBase, IDialogViewModel
    {
        #region Members

        private Models.Settings _OriginalSettings;
        private Models.Settings _Settings;
        private Options _Options;

        private LogoManager _LogoManager;

        private ICommonLog _Log;

        #endregion

        #region Properties

        /// <summary>
        /// Get\Set the Options
        /// </summary>
        public Options Options
        {
            get { return _Options; }
            set
            {
                _Options = value;
                OnPropertyChanged("Options");
            }
        }

        /// <summary>
        /// Get\Set the Window Code
        /// </summary>
        public int DialogServiceKey { get; private set; }

        /// <summary>
        /// Get\Set the NeedReload.
        /// Specify whehter the changes in the option causes a reload
        /// </summary>
        public bool NeedReload { get; private set; }

        #endregion

        #region Commands

        /// <summary>
        /// The Save Command
        /// </summary>
        private ICommand _SaveCommand;
        public ICommand SaveCommand
        {
            get { return _SaveCommand; }
            set { _SaveCommand = value; }
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

        #region Event Handler

        public event EventHandler<Options> Saved;
        public event EventHandler Closed;

        #endregion

        #region Actions

        public Action CloseAction { get; set; }

        #endregion

        #region Constructor

        public OptionsViewModel(ICommonLog log, Models.Settings settings)
        {
            DialogServiceKey = (int)Common.DialogServiceKey.Options;
            _Log = log;

            _LogoManager = new LogoManager(null, _Log);

            _Settings = settings;
            _OriginalSettings = new Models.Settings(settings);

            Options = new Options();
            Options.Settings = settings;
            //Options.Language = Options.Settings.Language;
            //Options.NumberOfImagesInPage = Options.Settings.NumberOfImagesInOnPage;

            Initialize(settings);

            // Create the Commands
            SaveCommand = new RelayCommand(SaveExecute);
            ExitCommand = new RelayCommand(ExitExecute);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize the Options
        /// </summary>
        private void Initialize(Models.Settings settings)
        {
            if (_Settings != null)
            {
                Options.Language = settings.Language;
                Options.NumberOfImagesInOnePage = settings.NumberOfImagesInOnePage;
            }

            // Set the logo
            Options.LogoImage = Helper.HelperBitmapImage.CreateFromFile(Path.Combine(Constants.LogoFolderName, Constants.LogoFileName));
        }

        /// <summary>
        /// Specify whehter exists almost an important change which cause a reload
        /// </summary>
        /// <returns></returns>
        private bool ExistImportantChanges()
        {
            var areFormatsEquals = _OriginalSettings.ImageFormats.All(Options.Settings.ImageFormats.Contains)
                && _OriginalSettings.ImageFormats.Count == Options.Settings.ImageFormats.Count;

            return _OriginalSettings.RotationAngle != Options.Settings.RotationAngle || 
                _OriginalSettings.NumberOfImagesInOnePage!= Options.Settings.NumberOfImagesInOnePage ||
                !areFormatsEquals;
        }

        /// <summary>
        /// Trigger the Saved Event
        /// </summary>
        private void TriggerSavedEvent(object sender, Options eventArgs)
        {
            Saved?.Invoke(sender, eventArgs);
        }

        /// <summary>
        /// Trigger the Closed Event
        /// </summary>
        private void TriggerClosedEvent(object sender, EventArgs eventArgs)
        {
            Closed?.Invoke(sender, eventArgs);
        }

        #endregion

        #region Events

        /// <summary>
        /// Save the Settings
        /// </summary>
        public void SaveExecute(object param)
        {
            _Settings.Language = Options.Language;
            Options.NeedReloaded = ExistImportantChanges();

            _LogoManager.CreateNewLogo(Options.LogoImage);

            // Close Window
            CloseAction();

            // Trigger the Saved Event
            TriggerSavedEvent(this, Options);
        }

        /// <summary>
        /// Close the Window
        /// </summary>
        public void ExitExecute(object param)
        {
            // Restore the Settings
            _Settings.Copy(_OriginalSettings);
            CultureResources.ChangeCulture(new CultureInfo(_Settings.Language));

            // Close Window
            CloseAction();

            // Trigger the Closed Event
            TriggerClosedEvent(this, EventArgs.Empty);
        }

        #endregion
    }
}
