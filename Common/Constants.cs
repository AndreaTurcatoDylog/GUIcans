using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Constants
    {       
        #region Folders & Files

        public static readonly string InformationFolderName = "DOC";
        public static readonly string LogoFolderName = "Logo";
        public static readonly string ReportsFolderName = "report";
        public static readonly string LogoFileName = "Logo.bmp";
        public static readonly string LogoName = "Logo";

        public static readonly string BatchFileName = "Batch.txt";
        public static readonly string AdditionalFileName = "Additional.txt";
        public static readonly string GlobalCommentFileName = "GlobalComment.txt";

        #endregion

        #region Images

        public static readonly string Image16BitFile = @"_16.tif";
        public static readonly string LogoImageFileFilter = @".jpg|.png|.tif|.gif$|.bmp";

        #endregion

        #region Settings

        public static readonly string SettingsFile = "Settings.json";
        public static readonly string CommandsFile = "Commands.json";
        public static readonly string StatusFile = "Status.json";

        #endregion

        #region Favorite Folders

        public static readonly string FavoriteFoldesFileName = "FavoriteFolders.json";

        #endregion

        #region Errors

        public static readonly string UnknownError = "ERROR 0001";
        public static readonly string DirectoryNotFoundError = "ERROR 0002";

        #endregion     
    }
}
