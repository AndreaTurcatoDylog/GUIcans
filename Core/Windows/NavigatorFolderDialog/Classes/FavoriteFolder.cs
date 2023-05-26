using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class FavoriteFolder : INotifyPropertyChanged
    {
        #region Members

        private string _FriendlyName;
        private string _Folder;
        private string _Domain;
        private string _Username;
        private string _Password;

        #endregion

        #region Properties

        /// <summary>
        /// Get\Set the Friendly Name
        /// </summary>
        public string FriendlyName
        {
            get { return _FriendlyName; }
            set
            {
                _FriendlyName = value;
                NotifyPropertyChanged("FriendlyName");

                // Fire the Friendly Name Changed
                OnFriendlyNameChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Get\Set the Folder
        /// </summary>
        public string Folder
        {
            get { return _Folder; }
            set
            {
                _Folder = value;
                NotifyPropertyChanged("Folder");
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
                NotifyPropertyChanged("Domain");
            }
        }

        /// <summary>
        /// Get\Set the Username
        /// </summary>
        public string Username
        {
            get { return _Username; }
            set
            {
                _Username = value;
                NotifyPropertyChanged("Username");
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
                NotifyPropertyChanged("Password");
            }
        }

        #endregion

        #region Event Handlers

        public event EventHandler FriendlyNameChanged;

        #endregion

        #region Constructor

        public FavoriteFolder(string friendlyName, string folder, string domain, string username, string password)
        {
            FriendlyName = friendlyName;
            Folder = folder;
            Domain = domain;
            Username = username;
            Password = password;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Perform a deep Clone
        /// </summary>
        public FavoriteFolder Clone()
        {
            return new FavoriteFolder(FriendlyName, Folder, Domain, Username, Password);
        }

        #endregion

        #region Events

        /// <summary>
        /// Fire the Event whent the Friendly Name changed
        /// </summary>
        protected void OnFriendlyNameChanged(EventArgs e)
        {
            FriendlyNameChanged?.Invoke(this, e);
        }

        #endregion

        #region INotifyPropertyChanged Impelementations

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion
    }
}
