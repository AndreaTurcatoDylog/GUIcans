using Core.ResourceManager.Cultures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Core
{
    public class FavoriteFolderDetailsViewModel : ViewModelBase, IDisposable
    {
        #region Members

        private FavoriteFolder _FavoriteFolder;
        private bool _CanCreateShortcut;
        private bool _IsModifyWindow;
        private bool _IsPasswordClear;
        private string _Password;

        // It is the Title message
        private string _TitleMessage;

        #endregion

        #region Properties

        /// <summary>
        /// Get\Set the Favorite Folder
        /// </summary>
        public FavoriteFolder FavoriteFolder
        {
            get { return _FavoriteFolder; }
            set
            {
                _FavoriteFolder = value;
                OnPropertyChanged("FavoriteFolder");
            }
        }

        /// <summary>
        /// Get\Set the message for connection
        /// </summary>
        public string TitleMessage
        {
            get { return _TitleMessage; }
            set
            {
                _TitleMessage = value;
                OnPropertyChanged("TitleMessage");
            }
        }

        /// <summary>
        /// Get\Set the Result
        /// The result of operation
        /// </summary>
        public System.Windows.Forms.DialogResult Result { get; private set; }

        /// <summary>
        /// Get\Set the CanCreateShortcut
        /// Specify whether is possible to create the shortCut
        /// </summary>
        public bool CanCreateShortcut
        {
            get { return _CanCreateShortcut; }
            set
            {
                _CanCreateShortcut = value;
                OnPropertyChanged("CanCreateShortcut");
            }
        }

        /// <summary>
        /// Get\Set the Is Password Clear
        /// </summary>
        public bool IsPasswordClear
        {
            get { return _IsPasswordClear; }
            set
            {
                _IsPasswordClear = value;
                OnPropertyChanged("IsPasswordClear");
            }
        }

        /// <summary>
        /// Get\Set the IsModifyWindow
        /// Specify whether it is a modify window or not
        /// </summary>
        public bool IsModifyWindow
        {
            get { return _IsModifyWindow; }
            set
            {
                _IsModifyWindow = value;
                OnPropertyChanged("IsModifyWindow");
            }
        }

        /// <summary>
        /// Get\Set the Password.
        /// Used to insert a new password and to show 
        /// some number of "*" when must display something.
        /// N.B. For security a password must never be in variabile or
        /// binding property. 
        /// In added Password operation The property is written but is
        /// overriden as soon as possible with random characters and when
        /// the password must be shown the "*" charaters is shown.
        /// The clear password is shown only when it is added the first time.
        /// </summary>
        /// <see cref="https://stackoverflow.com/questions/1483892/how-to-bind-to-a-passwordbox-in-mvvm?page=1&tab=votes#tab-top"/>
        public string Password
        {
            get { return _Password; }
            set
            {
                _Password = value;
                OnPropertyChanged("Password");
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// The Show Password Command
        /// </summary>
        private ICommand _ShowPasswordCommand;
        public ICommand ShowPasswordCommand
        {
            get { return _ShowPasswordCommand; }
            set { _ShowPasswordCommand = value; }
        }

        /// <summary>
        /// The Ok Command
        /// </summary>
        private ICommand _OKCommand;
        public ICommand OKCommand
        {
            get { return _OKCommand; }
            set { _OKCommand = value; }
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

        #region Actions

        public Action CloseAction { get; set; }

        #endregion

        #region Constructor

        public FavoriteFolderDetailsViewModel(FavoriteFolder favoriteFolder, bool isModifyWindow = false)
        {
            // Set the Favorite folder
            FavoriteFolder = favoriteFolder;

            // Set the IsModifyWindow
            IsModifyWindow = isModifyWindow;

            // Set the CanCreateShortcut
            CanCreateShortcut = !string.IsNullOrEmpty(favoriteFolder?.FriendlyName);

            // Set the title message
            TitleMessage = CultureResources.GetString("Label_Create_Shortcut");

            // Create the Commands
            OKCommand = new RelayCommand(OkExecute);
            ExitCommand = new RelayCommand(ExitExecute);

            // Register the events
            RegisterEvents(true);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Register\Unregister the events
        /// </summary>
        private void RegisterEvents(bool register)
        {
            if (register)
            {
                FavoriteFolder.FriendlyNameChanged += OnFriendlyNameChanged;
            }
            else
            {
                FavoriteFolder.FriendlyNameChanged -= OnFriendlyNameChanged;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Close the Window
        /// </summary>
        public void OkExecute(object param)
        {
            if (param is PasswordBox passwordBox)
            {                
                FavoriteFolder.Password = passwordBox.Password;             
            }

            Result = System.Windows.Forms.DialogResult.OK;            
            CloseAction();
        }

        /// <summary>
        /// Occurs when the Friendly name is changed
        /// </summary>
        private void OnFriendlyNameChanged(object sender, EventArgs e)
        {
            CanCreateShortcut = !string.IsNullOrEmpty(FavoriteFolder?.FriendlyName);
        }        

        //private void ChangeThePasswordWithRandomCharacter(ref string password)
        //{
        //    var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        //    var random = new Random();
        //    for (int i = 0; i < password.Length; i++)
        //    {
        //        var character = chars[random.Next(chars.Length)];
        //        password.ReplaceAtIndex(i, character);
        //        //unsafe
        //        //{
        //            //password[i] = chars[random.Next(chars.Length)];
        //        //}
        //    }
        //}

        /// <summary>
        /// Close the Window
        /// </summary>
        public void ExitExecute(object param)
        {
            Result = System.Windows.Forms.DialogResult.Cancel;
            CloseAction();
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
