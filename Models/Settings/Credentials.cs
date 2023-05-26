using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Credentials
    {
        #region Properties

        /// <summary>
        /// Get\Set the Domain
        /// </summary>
        [JsonProperty("Domain")]
        [JsonConverter(typeof(EncryptingJsonConverter), "")]
        public string Domain { get; set; }

        /// <summary>
        /// Get\Set the UserName
        /// </summary>
        [JsonProperty("UserName")]
        [JsonConverter(typeof(EncryptingJsonConverter), "")]
        public string UserName { get; set; }

        /// <summary>
        /// Get\Set the Password
        /// </summary>
        [JsonProperty("Password")]
        [JsonConverter(typeof(EncryptingJsonConverter), "")]
        public string Password { get; set; }

        #endregion

        #region Constructor
        public Credentials()
        {           
        }

        public Credentials(Credentials credentials)
        {
            if (credentials!=null)
            {
                Set(credentials.Domain, credentials.UserName, credentials.Password);
            }
        }

        public Credentials(string domain, string userName, string password)
        {
            Set(domain, userName, password);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set the velues
        /// </summary>
        public void Set(string domain, string userName, string password)
        {
            Domain = domain;
            UserName = userName;
            Password = password;
        }

        /// <summary>
        /// Copy the values
        /// </summary>
        public void Copy(Credentials credentials)
        {
            if (credentials != null)
            {
                Domain = credentials.Domain;
                UserName = credentials.UserName;
                Password = credentials.Password;
            }
        }

        #endregion
    }
}
