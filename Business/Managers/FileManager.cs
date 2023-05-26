using Common;
using Core;
using Core.ResourceManager.Cultures;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    /// <summary>
    /// This class manages:
    /// 1) The Read of a specificated .TXT File
    /// 2) The Read\Save of a specificated ImageFileInformation (.JSON)
    /// </summary>
    public class FileManager
    {
        #region Members

        private string _PathApplication;
        private ICommonLog _Log;

        #endregion

        #region Constructor

        public FileManager(ICommonLog log)
        {
            _PathApplication = $@"{Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName)}";
            _Log = log;            
        }

        #endregion

        #region Image File Information

        /// <summary>
        /// Load the informations from file and return object with that information
        /// </summary>
        public ImageData LoadImageInformationsFile(string pathImageFolder, string fileName)
        {
            ImageData imageData = null;
            string outputPathWithFileName = string.Empty;

            try
            {
                var jsonFileName = Path.GetFileNameWithoutExtension(fileName) + ".json";
                outputPathWithFileName = Path.Combine(pathImageFolder, Constants.InformationFolderName, jsonFileName);

                if (File.Exists(outputPathWithFileName))
                {
                    // Read file into a string and deserialize JSON to a type
                    var settings = new JsonSerializerSettings();
                    settings.MissingMemberHandling = MissingMemberHandling.Error;

                    return FileJsonManager<ImageData>.LoadFromFile(outputPathWithFileName, Encoding.UTF7);
                }
            }
            catch (Exception ex)
            {
                _Log.Error(ex.Message);

                // Delete the corrupted file
                imageData = null;
                if (File.Exists(outputPathWithFileName))
                {
                    File.Delete(outputPathWithFileName);
                }

                // Show a message box
                var message = CultureResources.GetString("Message_File_Corrupted");
                message = string.Format(message, fileName + ".json");
                var messageBox = new CustomMessageBox(MessageBoxType.Error, message, MessageBoxButtonsType.OK);
                messageBox.ShowMessageBox();
            }

            return imageData;
        }

        /// <summary>
        /// Create the Json File of Selected Image
        /// </summary>
        public OperationResult SaveImageInformationsFile(string pathImageFolder, ImageData imageData)
        {
            try
            {
                var docPath = Path.Combine(pathImageFolder, "DOC");
                if (!Directory.Exists(docPath))
                {
                    Directory.CreateDirectory(Path.Combine(pathImageFolder, "DOC"));
                }

                var fileName = Path.GetFileNameWithoutExtension(imageData.FileName) + ".json";
                var fullPath = Path.Combine(docPath, fileName);

                FileJsonManager<ImageData>.SaveFile(fullPath, imageData);

                return new OperationResult(true, string.Empty);
            }
            catch (Exception ex)
            {
                _Log.Debug(ex.Message);
                return new OperationResult(false, ex.Message);
            }
        }

        #endregion

        #region TXT Files

        /// <summary>
        /// Read the specificated Txt file if exists.
        /// Save the content in a array of string where each element is a line of file
        /// </summary>
        public string[] ReadTxtFile(string path)
        {            
            if (File.Exists(path))
            {
                try
                {
                    // Open the file to read from.
                    var readText = File.ReadAllLines(path);
                    return readText;
                }
                catch(Exception ex)
                {
                    _Log.Debug(ex.Message);
                }
            }

             return new string[0];          
        }

        #endregion

        #region Json Files

        /// <summary>
        /// Load the Favorite Folders
        /// </summary>
        public FavoriteFolders LoadFavoriteFolders()
        {
            try
            {
                var path = Path.Combine(_PathApplication, Constants.FavoriteFoldesFileName);
                if (File.Exists(path))
                {
                    return FileJsonManager<FavoriteFolders>.LoadFromFile(path, Encoding.UTF8);
                }                
            }
            catch(Exception ex)
            {
                _Log.Debug(ex.Message);
                return new FavoriteFolders();
            }

            return new FavoriteFolders();
        }

        /// <summary>
        /// Save the Favorite Folders
        /// </summary>
        public void SaveFavoriteFolders(FavoriteFolders favoriteFolders)
        {
            try
            {
                var path = Path.Combine(_PathApplication, Constants.FavoriteFoldesFileName);

                // Save Setting file
                FileJsonManager<FavoriteFolders>.SaveFile(path, favoriteFolders);
            }
            catch (Exception ex)
            {
                _Log.Debug(ex.Message);
            }
        }

        #endregion
    }
}
