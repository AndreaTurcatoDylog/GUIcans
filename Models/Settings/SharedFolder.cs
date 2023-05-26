using Core;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class SharedFolder : ModelBase
    {
        #region Members

        private string _FriendlyName;
        private string _Folder;
        private string _Domain;
        private string _Username;
        private string _Password;

        #endregion

        #region Properties

        [JsonProperty("FriendlyName")]
        public string FriendlyName
        {
            get { return _FriendlyName; }
            set
            {
                _FriendlyName = value;
                OnPropertyChanged("FriendlyName");
            }
        }

        [JsonProperty("Folder")]
        public string Folder
        {
            get { return _Folder; }
            set
            {
                _Folder = value;
                OnPropertyChanged("Folder");
            }
        }

        [JsonProperty("Domain")]
        [JsonConverter(typeof(EncryptingJsonConverter), "")]
        public string Domain
        {
            get { return _Domain; }
            set
            {
                _Domain = value;
                OnPropertyChanged("Domain");
            }
        }

        [JsonProperty("Username")]
        [JsonConverter(typeof(EncryptingJsonConverter), "")]
        public string Username
        {
            get { return _Username; }
            set
            {
                _Username = value;
                OnPropertyChanged("Username");
            }
        }

        [JsonProperty("Password")]
        [JsonConverter(typeof(EncryptingJsonConverter), "")]
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

        #region Constructor

        public SharedFolder(string friendlyName, string folder, string domain, string username, string password)
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
        public SharedFolder Clone()
        {
            return new SharedFolder(FriendlyName, Folder, Domain, Username, Password);
        }

        #endregion        
    }
}
