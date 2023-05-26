using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    /// <summary>
    /// This class rappresents the Connection Net Folder Wrapper
    /// It is a Singleton becouse the connection must be persistent and must be disconnected when
    /// the folder changes
    /// </summary>
    public class ConnectionNetFolderWrapper
    {
        #region Members

        private static ConnectionNetFolderWrapper _Instance;      

        #endregion

        #region Properties

        public static ConnectionNetFolderWrapper Instance
        {
            get
            {
                if (_Instance == null)
                {
                    return _Instance = new ConnectionNetFolderWrapper();
                }

                return _Instance;
            }
        }

        /// <summary>
        /// Get\Set the Connect To Shared Folder
        /// </summary>
        public ConnectToSharedFolder ConnectToSharedFolder { get; private set; }

        #endregion

        #region Constructor

        public ConnectionNetFolderWrapper()
        {
            ConnectToSharedFolder = new ConnectToSharedFolder();
        }

        #endregion       
    }
}
