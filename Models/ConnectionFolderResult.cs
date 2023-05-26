using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    /// <summary>
    /// The class Result for a connection to a Net Folder
    /// </summary>
    public class ConnectionFolderResult
    {
        #region Properties

        public bool IsConnected { get; private set; }

        public string Message { get; private set; }


        #endregion

        #region Constructor

        public ConnectionFolderResult(bool isConnected, string message)
        {
            IsConnected = isConnected;
            Message = message;
        }

        #endregion
    }
}
