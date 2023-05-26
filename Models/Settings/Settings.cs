using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Settings : SettingsBase
    {
        #region Members

        private string _Title;
        private string _MainSubtitle;
        private string _ImageSubtitle;
        private bool _OpenReportAfterCreated;

        #endregion

        #region Properties

        #region Titles

        [JsonProperty("Title")]
        public string Title
        {
            get { return _Title; }
            set
            {
                _Title = value;
                OnPropertyChanged("Title");
            }
        }

        [JsonProperty("MainSubtitle")]
        public string MainSubtitle
        {
            get { return _MainSubtitle; }
            set
            {
                _MainSubtitle = value;
                OnPropertyChanged("MainSubtitle");
            }
        }

        [JsonProperty("ImageSubtitle")]
        public string ImageSubtitle
        {
            get { return _ImageSubtitle; }
            set
            {
                _ImageSubtitle = value;
                OnPropertyChanged("ImageSubtitle");
            }
        }

        #endregion

        #region Version

        /// <summary>
        /// The type of the report.
        /// 0: ReportWithActionAndCause
        /// 1: RerportWithBatch
        /// </summary>
        [JsonProperty("ReportType")]
        public int ReportType { get; set; }

        /// <summary>
        /// The version of the SW.
        /// 0: FullVersion
        /// 1: LiteVersion
        /// </summary>
        [JsonProperty("VersionType")]
        public int VersionType { get; set; }

        #endregion

        #region PDF Report

        [JsonProperty("OpenReportAfterCreated")]
        public bool OpenReportAfterCreated
        {
            get { return _OpenReportAfterCreated; }
            set
            {
                _OpenReportAfterCreated = value;
                OnPropertyChanged("OpenReportAfterCreated");
            }
        }

        #endregion

        #region Filters

        [JsonProperty("Filter")]
        public string Filter { get; set; }


        [JsonProperty("FromDate")]
        public string FromDate { get; set; }

        [JsonProperty("ToDate")]
        public string ToDate { get; set; }

        #endregion

        #region Others

        [JsonProperty("Language")]
        public string Language { get; set; }

        [JsonProperty("NumberOfImagesInOnePage")]
        public int NumberOfImagesInOnePage { get; set; }

        [JsonProperty("SaveStatusOnExit")]
        public bool SaveStatusOnExit { get; set; }

        #endregion        

        #region Last Credentials

        [JsonProperty("LastCredentials")]
        public Credentials LastCredentials { get; set; }

        #endregion

        #endregion

        #region Constructor

        public Settings()
        {}

        public Settings(Settings setting)
            :base(setting)
        {
            Copy(setting);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Copy the values of the specificated settings
        /// </summary>
        public void Copy(Settings settings)
        {
            if (settings != null)
            {                
                base.Copy(settings);

                // Settings
                Title = settings.Title;
                MainSubtitle = settings.MainSubtitle;
                ImageSubtitle = settings.ImageSubtitle;                            
                ReportType = settings.ReportType;
                VersionType = settings.VersionType;
                OpenReportAfterCreated = settings.OpenReportAfterCreated;
                Filter = settings.Filter;
                FromDate = settings.FromDate;
                ToDate = settings.ToDate;
                Language = settings.Language;

                if (settings.NumberOfImagesInOnePage <= 0)
                {
                    settings.NumberOfImagesInOnePage = 7;                   
                }

                NumberOfImagesInOnePage = settings.NumberOfImagesInOnePage;

                SaveStatusOnExit = settings.SaveStatusOnExit;               

                PDFSavePath = settings.PDFSavePath;
                RotationAngle = settings.RotationAngle;
                NumberOfImageInOnePageLayout = settings.NumberOfImageInOnePageLayout;

                // Last Credentials
                LastCredentials = new Credentials(settings.LastCredentials);             

                // Set the Image Formats
                ImageFormats = new List<string>();
                if (settings.ImageFormats!=null)
                {
                    foreach (var imageFormat in settings.ImageFormats)
                    {
                        ImageFormats.Add(imageFormat);
                    }
                }
            }
        }

        /// <summary>
        /// Set the default values
        /// </summary>
        public new void SetDefaultValue()
        {
            base.SetDefaultValue();

            Title = string.Empty;
            MainSubtitle = string.Empty;
            ImageSubtitle = string.Empty;
            ReportType = 0;
            VersionType = 0;
            OpenReportAfterCreated = false;
            Filter = string.Empty;
            FromDate = string.Empty;
            ToDate = string.Empty;
            Language = "en";
            NumberOfImagesInOnePage = 7;
            SaveStatusOnExit = false;

            PDFSavePath = string.Empty;
            RotationAngle = 0;
            NumberOfImageInOnePageLayout = 2;   

            ImageFormats = new List<string>();
            LastCredentials = new Credentials();
        }

        #endregion
    }
}
