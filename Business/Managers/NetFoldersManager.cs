using Common;
using Core;
using Core.ResourceManager.Cultures;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Business
{    
    /// <summary>
    /// Manages the Net Folders
    /// </summary>
    public class NetFoldersManager
    {
        #region Members
        
        private ICommonLog _Log;

        #endregion

        #region Constructor

        public NetFoldersManager(ICommonLog log)
        {            
            _Log = log;
        }

        #endregion

        #region Methods

        /// <summary>
        ///  Check whether the specificated path is a Net address
        /// </summary>
        public bool IsNetPath(string path)
        {
            // Check whether it is a net address
            return path.ElementAt(0) == '\\';           
        }

        public async Task<ConnectionFolderResult> ConnectToNetFolder(string pathImage, Credentials credentials, IList<SharedFolder> favoriteFolders = null)
        {                      
            var isConnectionEstablished = false;
            var errorMessage = string.Empty;

            await Task.Run(() =>
            {               
                try
                {                   
                    if (!string.IsNullOrEmpty(pathImage) && credentials!=null)
                    {
                        //var connectToSharedFolder = new ConnectToSharedFolder();
                        var netUser = credentials.UserName;
                        var netPassword = credentials.Password;                        
                        var netDomain = credentials.Domain;

                        var code = ConnectionNetFolderWrapper.Instance.ConnectToSharedFolder.Connect(pathImage, netDomain, netUser, netPassword);
                        if (code == 0)
                        {
                            isConnectionEstablished = true;
                        }
                        else
                        {
                            var message = ConnectionNetFolderWrapper.Instance.ConnectToSharedFolder.GetErrorMessage(code);
                            errorMessage = $"{code}: {CultureResources.GetString(message)}";
                        }

                        if (isConnectionEstablished)
                        {
                            var netDirectoryExists = NetDirectoryExists(pathImage, favoriteFolders);
                            if (netDirectoryExists != DirectoryExistsResult.Exists)                            
                            {
                                errorMessage = string.Format(CultureResources.GetString("MESSAGE_ERROR_PATH_NOT_FOUND"), pathImage);
                            }
                        }
                        
                    }
                }
                catch (Exception ex)
                {
                    _Log.Error(ex.Message);
                }
            });

            return new ConnectionFolderResult(isConnectionEstablished, errorMessage);
        }

        /// <summary>
        /// Returns if the net Path exists or not
        /// </summary>
        public DirectoryExistsResult NetDirectoryExists(string path, IList<SharedFolder> favoriteFolders = null)
        {
            DirectoryInfo root = null;
            bool error = false;
            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    var splittedString = path.Split('\\');
                    splittedString = splittedString.Where(s => !string.IsNullOrEmpty(s)).ToArray();

                    if (splittedString.Count() > 0)
                    {
                        // Try to make a Ping to the server
                        var serverName = splittedString.First();
                        serverName = serverName.Trim('\\');
                        serverName = $@"{serverName}";

                        Ping ping = new Ping();
                        PingReply pingReply = ping.Send(serverName);
                        if (pingReply.Status == IPStatus.Success)
                        {
                            if (splittedString.Count() > 1)
                            {
                                // Try to get directory to find if directory exists
                                root = new DirectoryInfo(path);
                                root.GetDirectories();
                            }

                            return DirectoryExistsResult.Exists;
                        }
                        else
                        {
                            error = true;
                        }
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    error = true;
                    _Log.Error(ex.Message);
                }
                catch (Exception ex)
                {
                    if (ex is IOException || ex is PingException)
                    {
                        error = true;
                    }

                    _Log.Error(ex.Message);
                }

                if (error)
                {
                    return ManageDirectoryAccessError(root, path, favoriteFolders);
                }
            }

            return DirectoryExistsResult.NotExists;
        }

        /// <summary>
        /// Manage the error when tryng to access to Dicretory
        /// </summary>
        private DirectoryExistsResult ManageDirectoryAccessError(DirectoryInfo root, string path, IList<SharedFolder> favoriteFolders = null)
        {
            if (root != null)
            {
                // Search the credentials from Favorite
                var favoriteFolder = favoriteFolders?.FirstOrDefault(p => p.Folder == path);
                if (favoriteFolder != null)
                {
                    var secureString = AESCryptography.DecryptString(favoriteFolder.Password);

                    // Create the credentials
                    var credentials = new NetworkCredential();
                    credentials.Domain = favoriteFolder.Domain;
                    credentials.UserName = favoriteFolder.Username;
                    credentials.Password = new NetworkCredential("", secureString).Password;

                    // Try to connectwith the credentials
                    var remotePath = root.FullName;
                    var connectToSharedFolder = new ConnectToSharedFolder(remotePath, credentials);
                    var connectionResult = connectToSharedFolder.Connect();

                    if (connectionResult == 0)
                    {
                        return DirectoryExistsResult.Exists;
                    }
                }
            }

            return DirectoryExistsResult.IOError;
        }

        #endregion
    }
}
