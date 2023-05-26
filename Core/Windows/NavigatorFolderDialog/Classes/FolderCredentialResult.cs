using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class FolderCredentialResult
    {
        #region Properties

        /// <summary>
        /// Get\Set the Domain
        /// </summary>
        public string Domain { get; private set; }

        /// <summary>
        /// Get\Set the Username
        /// </summary>
        public string Username { get; private set; }

        /// <summary>
        /// Get\Set the Password
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        /// Get\Set the ConnectionResult
        /// The result of connetction operation
        /// </summary>
        public int ConnectionResult { get; private set; }

        /// <summary>
        /// Get\Set the Result
        /// The result of operation
        /// </summary>
        public System.Windows.Forms.DialogResult WindowResult { get; private set; }

        #endregion

        #region Constructor

        public FolderCredentialResult()
            : this(string.Empty, string.Empty, string.Empty, -1, System.Windows.Forms.DialogResult.Cancel)
        {}

        public FolderCredentialResult(string domain, string username, string password, int connectionResult, System.Windows.Forms.DialogResult result)
        {
            Domain = domain;
            Username = username;
            Password = password;
            ConnectionResult = connectionResult;
            WindowResult = result;
        }

        #endregion
    }
}
