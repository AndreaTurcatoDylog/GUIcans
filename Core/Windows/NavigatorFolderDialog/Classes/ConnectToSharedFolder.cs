using Core.ResourceManager.Cultures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// This class permits to Connect to a Shared Folder
    /// </summary>
    public class ConnectToSharedFolder : IDisposable
    {
        #region DLL Import

        [DllImport("mpr.dll")]
        private static extern int WNetAddConnection2(NetResource netResource,
            string password, string username, int flags);

        [DllImport("mpr.dll")]
        private static extern int WNetCancelConnection2(string name, int flags,
            bool force);

        [StructLayout(LayoutKind.Sequential)]
        public class NetResource
        {
            public ResourceScope Scope;
            public ResourceType ResourceType;
            public ResourceDisplaytype DisplayType;
            public int Usage;
            public string LocalName;
            public string RemoteName;
            public string Comment;
            public string Provider;
        }

        #endregion

        #region Members

        private string _NetworkName;
        private NetworkCredential _Credentials;

        #endregion

        #region Constructor

        public ConnectToSharedFolder()
        {
            _Credentials = new NetworkCredential();
        }

        public ConnectToSharedFolder(string networkName, NetworkCredential credentials, bool connect = false)
        {
            _NetworkName = networkName;
            _Credentials = credentials;

            if (connect)
            {
                Connect();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Connect to the Shared Folder
        /// </summary>
        public int Connect()
        {
             Disconnect();
            int result = -1;

            try
            {
                if (!string.IsNullOrEmpty(_NetworkName))
                {
                    var length = _NetworkName.Length;
                    if (_NetworkName.ElementAt(length - 1) == '\\')
                    {
                        _NetworkName = _NetworkName.Substring(0, length - 1);
                    }
                }

                var netResource = new NetResource
                {
                    Scope = ResourceScope.GlobalNetwork,
                    ResourceType = ResourceType.Disk,
                    DisplayType = ResourceDisplaytype.Share,
                    RemoteName = _NetworkName
                };

                var userName = string.IsNullOrEmpty(_Credentials.Domain)
                   ? _Credentials.UserName
                   : string.Format(@"{0}\{1}", _Credentials.Domain, _Credentials.UserName);


                result = WNetAddConnection2(
                      netResource,
                      _Credentials.Password,
                      userName,
                      0);
            }
            //catch (UnauthorizedAccessException ex)
            //{
               
            //}
            //catch (IOException ex)
            //{

            //}
            //catch(PingException ex)
            //{

            //}
            catch (Exception ex)
            {
                CoreLog.Instance.Append($"[{ex.Source}] - {ex.Message} - {ex.StackTrace}", CoreLogType.Error);
            }

            return result;

            //if (result != 0)
            //{
            //    return ConnectedToSharedFolderResult.Fails;
            //    //throw new Win32Exception(result, "Error connecting to remote share");
            //}

            //return ConnectedToSharedFolderResult.Connected;
        }

        /// <summary>
        /// Connect to the Shared Folder
        /// </summary>
        public int Connect(string networkName, string domain, string username, string password)
        {
            _NetworkName = networkName;
            _Credentials.Domain = domain;
            _Credentials.UserName = username;
            _Credentials.Password = password;

            return Connect();
        }

        /// <summary>
        /// Return the error message for connection failure
        /// </summary>
        public string GetErrorMessage(int errorCode)
        {
            var result = "Message_Unknown_Error";
            switch (errorCode)
            {
                case 3: result = "MESSAGE_ERROR_PATH_NOT_FOUND"; break;
                case 5: result = "MESSAGE_ERROR_ACCESS_DENIED"; break;
                case 53: result = "Messager_Error_Bad_Netpath"; break;
                case 67: result = "MESSAGE_ERROR_BAD_NET_NAME"; break;
                case 86: result = "Message_Error_Invalid_Password"; break;
                case 1219: result = "Message_Multiple_Connection"; break;
                case 1326: result = "Message_Error_Logon"; break;
                case 1908: result = "MESSAGE_ERROR_DOMAIN_CONTROLLER_NOT_FOUND"; break;
                case 1909: result = "MESSAGE_ERROR_ACCOUNT_LOCKED_OUT"; break;
                case 2202: result = "Message_Error_Bad_Username"; break;
            }

            return result;
            //return $"{"Label_Error"}: {result}";
        }

        /// <summary>
        /// Disconnect from the Shared Folder
        /// </summary>
        public void Disconnect()
        {
            WNetCancelConnection2(_NetworkName, 0, true);
        }

        #endregion

        #region IDisposable Implementation

        public void Dispose()
        {
            Disconnect();
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
