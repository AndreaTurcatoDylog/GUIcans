using Common;
using Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Business
{
    /// <summary>
    /// This class manages the Logo
    /// </summary>
    public class LogoManager
    {
        #region Members

        private BitmapImage _ResourceLogoBitmapImage;
        private ICommonLog _Log;

        #endregion

        #region Constructor

        public LogoManager(BitmapImage resourceLogoBitmapImage, ICommonLog log)
        {
            _ResourceLogoBitmapImage = resourceLogoBitmapImage;
            _Log = log;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create:
        /// 1) The Logo Folder if not exists
        /// 2) The DefaultLogo if not exists
        /// </summary>
        public void CreateLogoFolderAndDefaultLogo()
        {
            // Create the Logo Folder if not exists
            Directory.CreateDirectory(Constants.LogoFolderName);

            // Get the files in the Directory depending by filters
            var logo = Directory.EnumerateFiles(Constants.LogoFolderName, "*.*")
                                .OrderBy(l => Path.GetFileName(l))
                                .FirstOrDefault(f => Path.GetFileNameWithoutExtension(f) == Constants.LogoFileName &&
                                                Regex.IsMatch(Path.GetFileName(f), Constants.LogoImageFileFilter));
            if (logo == null)
            {
                CreateDefaultLogo();
            }
        }

        /// <summary>
        /// Returns the Logo Path. 
        /// If Logo not exists the default Logo path is returned
        /// </summary>
        public string GetLogoPath()
        {
            var result = string.Empty;

            // Get the file called "LogoName" in the Directory depending by filters
            var logoFilePath = Directory.EnumerateFiles(Constants.LogoFolderName, "*.*")
                                    .OrderBy(l => Path.GetFileName(l))
                                    .FirstOrDefault(f => Path.GetFileNameWithoutExtension(f) == Constants.LogoFileName &&
                                                Regex.IsMatch(Path.GetFileName(f), Constants.LogoImageFileFilter));

            if(File.Exists(logoFilePath))
            {
                return logoFilePath;
            }

            // Create the Default Logo and returns its file path
            CreateDefaultLogo();
            return Path.Combine(Constants.LogoFolderName, Constants.LogoFileName); 
        }


        /// <summary>
        /// Create a Default Logo from Resources if not exists.
        /// </summary>
        private void CreateDefaultLogo()
        {
            var defaultLogoFilePath = Path.Combine(Constants.LogoFolderName, Constants.LogoFileName);
            if (!File.Exists(defaultLogoFilePath))
            {
                _ResourceLogoBitmapImage?.Save(defaultLogoFilePath);
            }
        }

        /// <summary>
        /// This method create a new Logo Image from the specificated BitmapImage in the Logo Folder.
        /// If a file named "Logo.*" exists in the Folder it is renamed as "Logo_Date_Time.*".
        /// With the above operation always one and only one file named "Logo.*" will exists in the Logo Folder
        /// </summary>
        public void CreateNewLogo(BitmapImage bitmapImage)
        {
            try
            {
                if (bitmapImage != null)
                {
                   
                    // Get the file called "LogoName" in the Directory 
                    var logoFilePath = Directory.EnumerateFiles(Constants.LogoFolderName, "*.*")
                                            .OrderBy(l => Path.GetFileName(l))
                                            .FirstOrDefault(f => Path.GetFileNameWithoutExtension(f).ToLower() == Constants.LogoName.ToLower());

                    if (logoFilePath!=null && File.Exists(logoFilePath))
                    {
                        // Rename the file named "Logo.* in the Logo Folder
                        var extension = Path.GetExtension(logoFilePath);
                        var backFileName = $"{Constants.LogoName}_{DateTime.UtcNow.ToString("yyyy_MM_dd_HHmmss")}{extension}";
                        var pathBackFileName = Path.Combine(Constants.LogoFolderName, backFileName);
                        File.Move(logoFilePath, pathBackFileName);

                        // Create the new Log in the Folder Path                        
                        bitmapImage.Save(Path.Combine(Constants.LogoFolderName, Constants.LogoFileName));
                    }
                }
            }
            catch(Exception ex)
            {
                var customMessageBox = new CustomMessageBox(MessageBoxType.Error, ex.Message, MessageBoxButtonsType.OK);
                customMessageBox.ShowDialog();

                _Log.Debug(ex.Message);
            }
        }

        #endregion
    }
}
