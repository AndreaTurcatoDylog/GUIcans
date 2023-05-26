using Common;
using Core;
using Core.ResourceManager.Cultures;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Business
{    
    public class ImageFileManager
    {
        #region Members

        private NetFoldersManager _NetFoldersManager;
        private ICommonLog _Log;

        #endregion

        #region Constructor

        public ImageFileManager(ICommonLog log)
        {
            _NetFoldersManager = new NetFoldersManager(log);
            _Log = log;
        }

        #endregion

        #region Methods               

        /// <summary>
        /// Get the path of files in the selected folder 
        /// </summary>
        public async Task<FilesResult> GetFileNamesFromPath(string path, string filters, Credentials credentials, IList<SharedFolder> favoriteFolders = null)
        {                      
            var isErrorFound = false;
            var fileNames = new List<string>();
            var message = string.Empty;

            try
            {
                // If it is a NET Folder try to connect 
                if (_NetFoldersManager.IsNetPath(path))
                {
                    var netResult = await _NetFoldersManager.ConnectToNetFolder(path, credentials, favoriteFolders);

                    isErrorFound = !netResult.IsConnected;
                    message = netResult.Message;
                }

                if (!isErrorFound)
                {
                    await Task.Run(() =>
                    {
                        if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
                        {
                        // Get the files in the Directory depending by filters
                        if (!string.IsNullOrEmpty(filters))
                            {
                                fileNames = Directory.EnumerateFiles(path, "*.*")
                                                    .Where(f => Regex.IsMatch(Path.GetExtension(Path.GetFileName(f)),
                                                                    filters, RegexOptions.IgnoreCase) ||
                                                                (f.ToLower().EndsWith(Constants.Image16BitFile) && filters.Contains(".16"))
                                                            )
                                                    .ToList();
                            }

                            isErrorFound = fileNames?.Count() == 0;
                            if (isErrorFound)
                            {
                                message = string.Format(CultureResources.GetString("Message_Images_Not_Found"), path);
                            }
                        }
                        else
                        {
                            message = string.Format(CultureResources.GetString("MESSAGE_ERROR_PATH_NOT_FOUND"), path);
                        }
                    });
                }

                var fileRetrivedSuccessfully = !isErrorFound;
                return new FilesResult(fileRetrivedSuccessfully, fileNames, message);
            }
            catch(Exception ex)
            {
                _Log.Debug(ex.Message);
                return new FilesResult(false, fileNames, ex.Message);                
            }
        }       

        #endregion

    }
}
