using Core.ResourceManager.Cultures;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core
{
    public class NetNavigatorFolder : NavigatorFolderBase
    {
        #region Members

        private IList<NavigationDialogItem> _NetworkComputers;
        private ConnectToSharedFolder _ConnectToSharedFolder;
        //private static NetNavigatorFolder _Instance;
        private string _Domain;

        #endregion

        #region Properties

        ///// <summary>
        ///// Singleton
        ///// </summary>
        //public static NetNavigatorFolder Instance
        //{
        //    get
        //    {
        //        if (_Instance == null)
        //        {
        //            _Instance = new NetNavigatorFolder();
        //        }

        //        return _Instance;
        //    }
        //}

        /// <summary>
        /// Get\Set the root name
        /// </summary>
        public override string RootName
        {
            get { return "NET"; }
        }

        /// <summary>
        /// Get\Set the Favorite folders
        /// </summary>
        public IList<FavoriteFolder> FavoriteFolders { get; private set; }

        #endregion

        #region Constructor

        public NetNavigatorFolder(string domain, IList<FavoriteFolder> favoriteFolders)
            : base()
        {
            _Domain = domain;

            // Create the credentials and shared folder class
            //var credentials = new NetworkCredential();
            _ConnectToSharedFolder = new ConnectToSharedFolder();

            _NetworkComputers = new List<NavigationDialogItem>();

            FavoriteFolders = favoriteFolders;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the IP Address from Machine name
        /// </summary>
        private static string GetIPAddressFromMachineName(string machinename)
        {
            string ipAddress = string.Empty;
            try
            {
                IPAddress ip = Dns.GetHostEntry(machinename).AddressList.Where(o => o.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).First();
                ipAddress = ip.ToString();
            }
            catch (Exception ex)
            {
                CoreLog.Instance.Append($"[{ex.Source}] - {ex.Message} - {ex.StackTrace}", CoreLogType.Error);
            }
            return ipAddress;
        }


        /// <summary>
        /// Returns the information about the path exists or not
        /// </summary>
        public override DirectoryExistsResult DirectoryExists(string path, CancellationToken cancellationToken)
        {
            DirectoryInfo root = null;
            bool error = false;

            cancellationToken.ThrowIfCancellationRequested();

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

                        cancellationToken.ThrowIfCancellationRequested();

                        Ping ping = new Ping();
                        PingReply pingReply = ping.Send(serverName);
                        if (pingReply.Status == IPStatus.Success)
                        {
                            if (splittedString.Count() > 1)
                            {

                                cancellationToken.ThrowIfCancellationRequested();

                                // Try to get directory to find if directory exists
                                root = new DirectoryInfo(path);
                                root.GetDirectories();

                                cancellationToken.ThrowIfCancellationRequested();
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
                    CoreLog.Instance.Append($"[{ex.Source}] - {ex.Message} - {ex.StackTrace}", CoreLogType.Error);
                }
                catch (Exception ex)
                {
                    if (ex is IOException || ex is PingException)
                    {
                        error = true;
                    }

                    CoreLog.Instance.Append($"[{ex.Source}] - {ex.Message} - {ex.StackTrace}", CoreLogType.Error);
                }

                if (error)
                {
                    return ManageDirectoryAccessError(root, path, cancellationToken);
                }

            }

            return DirectoryExistsResult.NotExists;
        }

        /// <summary>
        /// Manage the error when tryng to access to Dicretory
        /// </summary>
        /// <returns></returns>
        private DirectoryExistsResult ManageDirectoryAccessError(DirectoryInfo root, string path, CancellationToken cancellationToken)
        {
            if (root != null)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Search the credentials from Favorite
                var favoriteFolder = FavoriteFolders.FirstOrDefault(p => p.Folder == path);
                if (favoriteFolder != null)
                {
                   // var secureString = AESCryptography.DecryptString(favoriteFolder.Password);

                    // Create the credentials
                    var credentials = new NetworkCredential();
                    credentials.Domain = favoriteFolder.Domain;
                    credentials.UserName = favoriteFolder.Username;
                    credentials.Password = new NetworkCredential("", favoriteFolder.Password).Password;
                    //credentials.Password = new NetworkCredential("", secureString).Password;

                    // Try to connectwith the credentials
                    var remotePath = root.FullName;
                    var connectToSharedFolder = new ConnectToSharedFolder(remotePath, credentials);
                    var connectionResult = connectToSharedFolder.Connect();

                    if (connectionResult == 0)
                    {
                        return DirectoryExistsResult.Exists;
                    }
                    else
                    {
                        return DirectoryExistsResult.IOError;
                    }
                }
                else
                {
                    return DirectoryExistsResult.IOError;
                }
            }
            else
            {
                return DirectoryExistsResult.IOError;
            }
        }

        /// <summary>
        /// Returns the Root in path
        /// </summary>
        public override string GetRoot(string currentPath)
        {
            if (string.IsNullOrEmpty(currentPath))
            {
                // Split the path
                var splittedString = currentPath.Split('\\');
                splittedString = splittedString.Where(s => !string.IsNullOrEmpty(s)).ToArray();

                if (splittedString.Count() > 0)
                {
                    return splittedString[0];
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Check whether the specificated path is an IP Address or not.
        /// if it is a machine name so it is turned in an IP address
        /// </summary>
        private void ValidateAndGetIpAddress(ref string path)
        {
            var isIPaddress = IPAddress.TryParse(path, out IPAddress address);
            if (!isIPaddress)
            {
                path = GetIPAddressFromMachineName(path);
            }

           // return path;
        }

        /// <summary>
        /// Returns all the Sub Folders
        /// </summary>
        public override IList<NavigationDialogItem> GetFolders(string path, CancellationToken token, ref bool error)
        {
            IList<NavigationDialogItem> list = null;

            token.ThrowIfCancellationRequested();

            // Determines whether the path is ThisPc or not
            var isRootNet = string.IsNullOrEmpty(path);
            if (isRootNet)
            {
                if (_NetworkComputers != null && _NetworkComputers.Count() == 0)
                {
                    token.ThrowIfCancellationRequested();

                    list = GetPaginatedNetworkComputers(out error);
                    _NetworkComputers = list;

                    token.ThrowIfCancellationRequested();
                }
                else
                {
                    token.ThrowIfCancellationRequested();

                    list = _NetworkComputers;

                    token.ThrowIfCancellationRequested();
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(path))
                {
                    token.ThrowIfCancellationRequested();

                    var splittedString = path.Split('\\');
                    splittedString = splittedString.Where(s => !string.IsNullOrEmpty(s)).ToArray();

                    // Make the Server name in right format and 
                    // turned in an IP Address
                    if (splittedString.Count() == 1)
                    {
                        path = path.Trim('\\');
                        ValidateAndGetIpAddress(ref path);
                        path = $@"\\{path}";
                    }

                    // Get the Items
                    if (splittedString.Count() > 1)
                    {
                        token.ThrowIfCancellationRequested();

                        var rootDirectory = new DirectoryInfo(path);
                        list = GetPaginatedSubFolders(rootDirectory, 0, out error);

                        token.ThrowIfCancellationRequested();
                    }
                    else
                    {
                        token.ThrowIfCancellationRequested();

                        list = GetPaginatedServerSubFolders(path, token, out error);

                        token.ThrowIfCancellationRequested();
                    }
                }
                else
                {
                    error = true;
                }
            }

            if (!error && list != null)
            {
                return list;
            }

            return null;
        }

        /// <summary>
        /// Return the error message for connection failure
        /// </summary>
        private string GetErrorMessage(int errorCode)
        {
            var result = CultureResources.GetString("Message_Unknown_Error");
            switch (errorCode)
            {
                case 3: result = CultureResources.GetString("MESSAGE_ERROR_PATH_NOT_FOUND"); break;
                case 5: result = CultureResources.GetString("MESSAGE_ERROR_ACCESS_DENIED"); break;
                case 53: result = CultureResources.GetString("MESSAGE_ERROR_ACCESS_DENIED"); break;
                case 67: result = CultureResources.GetString("MESSAGE_ERROR_BAD_NET_NAME"); break;
                case 86: result = CultureResources.GetString("Message_Error_Invalid_Password"); break;
                case 1326: result = CultureResources.GetString("Message_Error_Logon"); break;
                case 1908: result = CultureResources.GetString("MESSAGE_ERROR_DOMAIN_CONTROLLER_NOT_FOUND"); break;
                case 1909: result = CultureResources.GetString("MESSAGE_ERROR_ACCOUNT_LOCKED_OUT"); break;
                case 2202: result = CultureResources.GetString("Message_Error_Bad_Username"); break;
            }

            return $"{CultureResources.GetString("Label_Error")}: {result}";
        }

        /// <summary>
        /// Returns a paginated list of Sub folders of Server by server name
        /// </summary>
        private IList<NavigationDialogItem> GetPaginatedServerSubFolders(string serverName, CancellationToken token, out bool error)
        {
            var result = new List<NavigationDialogItem>();

            token.ThrowIfCancellationRequested();

            error = false;

            try
            {
                // Get the local shares 
                var shi = ShareCollection.LocalShares;
                if (shi != null)
                {
                    string server = serverName;

                    // Enumerate shares on a remote computer
                    if (server != null && server.Trim().Length > 0)
                    {
                        token.ThrowIfCancellationRequested();

                        shi = ShareCollection.GetShares(server);

                        token.ThrowIfCancellationRequested();

                        if (shi != null)
                        {
                            foreach (Share si in shi)
                            {
                                // Get only Disk
                                if (si.ShareType == ShareType.Disk)
                                {                                    
                                   // var path = si.Root.FullName;
                                    var path = ValidatePath(si.Root.FullName);
                                    result.Add(new NavigationDialogItem(_FolderBitmapImage, si.NetName, path));
                                }
                            }
                        }
                    }
                }
            }           
            catch (Exception ex)
            {
                error = true;
                CoreLog.Instance.Append($"[{ex.Source}] - {ex.Message} - {ex.StackTrace}", CoreLogType.Error);
            }

            return result;
        }

        /// <summary>
        /// Check if the Net Path is valid.
        /// Sometimes in the flow some "\" characters are added to the start of the string.
        /// This method check whether the path is valid or not. It is not valid if to the start
        /// of the string there are more then 2 "\" character. The method fix the Path.
        /// </summary>
        private string ValidatePath(string originalPath)
        {
            var result = originalPath;
            if (originalPath.StartsWith(@"\"))
            {
                var index = 1;                
                while(result.ElementAt(index) == '\\')
                {
                    index++;
                }

                var count = result.Length - index;
                result = result.Substring(index, count );
            }

            return $@"\\{result}";
        }

        /// <summary>
        /// Returns a paginated list of Sub folders
        /// </summary>
        private IList<NavigationDialogItem> GetPaginatedSubFolders(DirectoryInfo root, int tryingCount, out bool error)
        {
            var result = new List<NavigationDialogItem>();
            error = false;

            try
            {
                // DELETE
               // throw new IOException();
                // END DELETE

                if (root != null)
                {
                    DirectoryInfo[] subDirs = null;

                    // Find all the subdirectories under this directory.
                    subDirs = root.GetDirectories();
                    foreach (var dirInfo in subDirs)
                    {
                        // Create the item
                        result.Add(new NavigationDialogItem(_FolderBitmapImage, dirInfo.Name, dirInfo.FullName));
                    }
                }

                return result;
            }
            //catch(UnauthorizedAccessException)
            //{

            //}
            catch (IOException ex)
            {
                if (tryingCount < 1)
                {
                    // Search the credentials from Favorite
                    var favoriteFolder = FavoriteFolders.FirstOrDefault(p => p.Folder == root.FullName);
                    if (favoriteFolder != null)
                    {
                        var secureString = AESCryptography.DecryptString(favoriteFolder.Password);

                        var credentials = new NetworkCredential();
                        credentials.Domain = favoriteFolder.Domain;
                        credentials.UserName = favoriteFolder.Username;
                        credentials.Password = new NetworkCredential("", secureString).Password;

                        var remotePath = root.FullName;
                        var connectToSharedFolder = new ConnectToSharedFolder(remotePath, credentials);
                        var connectionResult = connectToSharedFolder.Connect();

                        if (connectionResult == 0)
                        {
                            tryingCount = tryingCount + 1;
                            return GetPaginatedSubFolders(root, tryingCount, out error);
                        }
                        else
                        {
                            tryingCount = tryingCount + 1;

                            // Create the model and view models
                            var folderCredentialsViewModel = new FolderCredentialsViewModel(root.FullName, credentials.Domain, credentials.UserName, credentials.Password);

                            // Show the Credential Window
                            var folderCredentialsWindow = new FolderCredentials(folderCredentialsViewModel);
                            folderCredentialsWindow.ShowWindow(true);
                            if (folderCredentialsWindow.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                return GetPaginatedSubFolders(root, tryingCount, out error);
                            }
                            else
                            {
                                error = true;
                            }
                        }
                    }
                    else
                    {
                        // Create the model and view models
                        var folderCredentialsViewModel = new FolderCredentialsViewModel(root.FullName, string.Empty, string.Empty, string.Empty);

                        // Show the Credential Window
                        var folderCredentialsWindow = new FolderCredentials(folderCredentialsViewModel);
                        folderCredentialsWindow.ShowWindow(true);
                        if (folderCredentialsWindow.Result == System.Windows.Forms.DialogResult.OK)
                        {
                            return GetPaginatedSubFolders(root, tryingCount, out error);
                        }
                    }

                    CoreLog.Instance.Append($"[{ex.Source}] - {ex.Message} - {ex.StackTrace}", CoreLogType.Error);
                }
                else
                {
                    error = true;
                }
            }
            catch (Exception ex)
            {
                error = true;
                CoreLog.Instance.Append($"[{ex.Source}] - {ex.Message} - {ex.StackTrace}", CoreLogType.Error);
            }

            return result;
        }

        /// <summary>
        /// Returns a paginated list of Drives
        /// </summary>
        private IList<NavigationDialogItem> GetPaginatedNetworkComputers(out bool error)
        {
            var result = new List<NavigationDialogItem>();
            error = false;
            var domain = string.Empty;

            try
            {
                domain = (string.IsNullOrEmpty(_Domain))? domain = Environment.UserDomainName.ToString() 
                                                        : domain = _Domain;

                if (!string.IsNullOrEmpty(domain))
                {
                    
                    string directoryEntry = $"WinNT://{domain}";
                    
                    using (DirectoryEntry workgroup = new DirectoryEntry(directoryEntry))
                    {
                        foreach (DirectoryEntry child in workgroup.Children)
                        {
                            result.Add(new NavigationDialogItem(_FolderBitmapImage, child.Name, child.Name));
                        }
                    }
                }

                return result.OrderBy(p => p.Name).ToList();
            }
            catch (Exception ex)
            {
                error = true;
                CoreLog.Instance.Append($"[{ex.Source}] - {ex.Message} - {ex.StackTrace}", CoreLogType.Error);
            }

            return result;
        }

        /// <summary>
        /// Create a sub Path from 0 to limit.
        /// "0": position of first part of path
        /// "limit": position of the last part of sub path 
        /// </summary>
        public override string CreateSubPath(string currentPath, int limit)
        {
            var subPath = string.Empty;

            // Update the CurrentPath
            if (!string.IsNullOrEmpty(currentPath))
            {
                // Split the path
                var splittedString = currentPath.Split('\\');
                splittedString = splittedString.Where(s => !string.IsNullOrEmpty(s)).ToArray();

                if (splittedString.Count() > 0)
                {
                    splittedString[0] = $@"\\{splittedString[0]}\";
                    for (var index = 0; index < limit; index++)
                    {
                        subPath = Path.Combine(subPath, splittedString[index]);
                    }
                }
            }

            return subPath;
        }

        /// <summary>
        /// Returns the Path going Up in the Tree 
        /// </summary>
        public override string GoUpInTreeFolder(string currentPath)
        {
            var result = string.Empty;

            // Update the CurrentPath
            if (!string.IsNullOrEmpty(currentPath))
            {
                // Split the path
                var splittedString = currentPath.Split('\\');
                splittedString = splittedString.Where(s => !string.IsNullOrEmpty(s)).ToArray();

                if (splittedString.Count() > 0)
                {
                    splittedString[0] = $@"\\{splittedString[0]}\";
                    for (var index = 0; index < splittedString.Count()-1; index++)
                    {
                        result = Path.Combine(result, splittedString[index]);
                    }
                }
            }

            return result;
        }

        #endregion
    }
}
