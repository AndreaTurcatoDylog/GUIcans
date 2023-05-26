using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedDetailsImageVisualizer
{
    public static class Constants
    {
        public static readonly string InformationFolderName = "DOC";
        public static readonly string ImageFileFilter = @".jpg|.png|.16|.tif|.gif$";
        public static readonly string LogoImageFileFilter = @".jpg|.png|.tif|.gif$|.bmp";

        public static readonly string InitFileName = "Init.json";
        public static readonly string SettingFileName = "Settings.json";
        public static readonly string LogoFolderName = "Logo";
        public static readonly string ReportsFolderName = "report";
        public static readonly string DefaultLogoName = "DefaultLogo.bmp";
        public static readonly string BatchFileName = "Batch.txt";
        public static readonly string AdditionalFileName = "Additional.txt";

        public static readonly bool CallCreateSettingsSoftwareDefaultValue = true;
        public static readonly string PathImageDefaultValue = string.Empty;
        public static readonly string FromDateDefaultValue = string.Empty;
        public static readonly string ToDateDefaultValue = string.Empty;
        public static readonly string FilterDefaultValue = string.Empty;
        public static readonly double RotateAngleDefaultValue = 90.0;
        public static readonly int NumberOfImageInOnePageLayoutDefaultValue = 4;
        public static readonly bool CanSaveCredentials = true;

        public static readonly bool IsTouchApplication = false;

        public static readonly string UnknownError = "ERROR 0001";
        public static readonly string DirectoryNotFoundError = "ERROR 0002";
    }
}
