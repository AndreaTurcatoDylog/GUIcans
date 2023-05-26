using Core.ResourceManager.Cultures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Core
{
    public class FolderCredentialsViewModel : ViewModelBase
    {
        #region Members

        private string _SharedFolder;
        private string _Domain;
        private string _Username;
        private string _Password;
        private string _TitleMessage;
        private bool _IsTryConnecting;
        private bool _IsConnectionFailure;
        private bool _IsPasswordClear;
        private string _ErrorMessage;

        #endregion

        #region Properties

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
        /// Get\Set the SharedFolder
        /// </summary>
        public string SharedFolder
        {
            get { return _SharedFolder; }
            set
            {
                _SharedFolder = value;
                OnPropertyChanged("SharedFolder");
            }
        }

        /// <summary>
        /// Get\Set the Domain
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
        /// Get\Set the User Name
        /// </summary>
        public string Username
        {
            get { return _Username; }
            set
            {
                _Username = value;
                OnPropertyChanged("Username");
            }
        }

        /// <summary>
        /// Get\Set the Password
        /// </summary>
        public string Password
        {
            get { return _Password; }
            set
            {
                _Password = value;
                OnPropertyChanged("Password");
            }
        }

        /// <summary>
        /// Get\Set the is try connection
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
        /// Get\Set the is try connection
        /// </summary>
        public bool IsConnectionFailure
        {
            get { return _IsConnectionFailure; }
            set
            {
                _IsConnectionFailure = value;
                OnPropertyChanged("IsConnectionFailure");
            }
        }

        /// <summary>
        /// Get\Set the ErrorMessage
        /// </summary>
        public string ErrorMessage
        {
            get { return _ErrorMessage; }
            set
            {
                _ErrorMessage = value;
                OnPropertyChanged("ErrorMessage");
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
        /// Get\Set the Result
        /// The result of operation
        /// </summary>
        public FolderCredentialResult Result { get; private set; }

        #endregion

        #region Commands

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

        public FolderCredentialsViewModel()
            : this(string.Empty, string.Empty, string.Empty, string.Empty)
        {
        }

        public FolderCredentialsViewModel(string sharedFolder, string domain, string username, string password)
        {
            // Set the properties
            TitleMessage = CultureResources.GetString("Label_Insert_Credentials");

            SharedFolder = sharedFolder;
            Domain = domain;
            Username = username;
            Password = password;

            // Create the Commands
            OKCommand = new RelayCommand(OkExecute);
            ExitCommand = new RelayCommand(ExitExecute);
        }

        #endregion

        #region Events

        /// <summary>
        /// Close the Window with Ok result
        /// </summary>
        public async void OkExecute(object param)
        {
            ErrorMessage = string.Empty;
            IsTryConnecting = true;

            var passwordBox = param as PasswordBox;

            TitleMessage = CultureResources.GetString("Label_Connecting");

            IsConnectionFailure = false;

            var credentials = new NetworkCredential();
            credentials.Domain = Domain;
            credentials.UserName = Username;
            credentials.Password = new NetworkCredential("", passwordBox.Password).Password;

            var remotePath = SharedFolder;
            var connectToSharedFolder = new ConnectToSharedFolder(remotePath, credentials);

            var connectionResult = -1;
            await Task.Run(() =>
            {
                connectionResult = connectToSharedFolder.Connect();
            });

            if (connectionResult == 0)
            {
                // var encryptedBytes = AESCryptography.EncryptStringToBytes_Aes(credentials.Password, AESCryptography.Information, AESCryptography.IV);

                //// EncryptedPassword = Encoding.ASCII.GetString(encryptedBytes);
                // var encryptedPassword = EncryptedPassword = Convert.ToBase64String(encryptedBytes, 0, encryptedBytes.Length, Base64FormattingOptions.None);
                // Result = new FolderCredentialResult(credentials.Domain, credentials.UserName, encryptedPassword, connectionResult, System.Windows.Forms.DialogResult.OK);


                Result = new FolderCredentialResult(credentials.Domain, credentials.UserName, credentials.Password, connectionResult, System.Windows.Forms.DialogResult.OK);
                CloseAction();
            }
            else
            {
                Result = new FolderCredentialResult(string.Empty, string.Empty, string.Empty, connectionResult, System.Windows.Forms.DialogResult.OK);

                TitleMessage = CultureResources.GetString("Label_Error");

                // Set the Error Message
                var errorMessage = connectToSharedFolder.GetErrorMessage(connectionResult);
                ErrorMessage = CultureResources.GetString(errorMessage);               

                IsConnectionFailure = true;
                IsTryConnecting = false;
            }
        }


        /// <summary>
        /// Close the Window with Cancel result
        /// </summary>
        public void ExitExecute(object param)
        {
            Result = new FolderCredentialResult(string.Empty, string.Empty, string.Empty, -1, System.Windows.Forms.DialogResult.Cancel);
            CloseAction();
        }

        #endregion
    }
}

