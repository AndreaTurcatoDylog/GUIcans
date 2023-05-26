#define DEFAULT_IS_TOUCH_APPLICATION

using Core;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public interface ISettings { }

    public abstract class SettingsBase : ModelBase, ISettings
    {
        #region Members

        private List<string> _ImageFormats;

        #endregion

        #region Properties       

        /// <summary>
        /// Get\Set the StartFolderPath.
        /// </summary>
        [JsonProperty("StartFolderPath")]
        public string StartFolderPath { get; set; }

        [JsonProperty("ISChildApplication")]
        public bool ISChildApplication { get; set; }

        [JsonProperty("IsTouchApplication")]
        public bool IsTouchApplication { get; set; }

        #region Image Format Filter

        [JsonProperty("ImageFormats")]
        public List<string> ImageFormats
        {
            get
            {
                if (_ImageFormats == null)
                {
                    _ImageFormats = new List<string>();
                }

                return _ImageFormats;
            }
            set
            {
                _ImageFormats = value;
            }
        }

        #endregion

        #region PDF Report

        [JsonProperty("PDFSavePath")]
        public string PDFSavePath { get; set; }

        [JsonProperty("RotationAngle")]
        public double RotationAngle { get; set; }

        [JsonProperty("NumberOfImageInOnePageLayout")]
        public int NumberOfImageInOnePageLayout { get; set; }

        #endregion

        #endregion

        #region Constructor

        public SettingsBase() { }

        public SettingsBase(SettingsBase settingBase)
        {
            Copy(settingBase);
        }

        #endregion

        #region Methods      

        /// <summary>
        /// Copy the values of the specificated settings
        /// </summary>
        public void Copy(SettingsBase settings)
        {
            if (settings != null)
            {
                // Custom Settings
                StartFolderPath = settings.StartFolderPath;
                ISChildApplication = settings.ISChildApplication;
                IsTouchApplication = settings.IsTouchApplication;
            }
        }

        /// <summary>
        /// Set the default values
        /// </summary>
        public void SetDefaultValue()
        {
            StartFolderPath = string.Empty;
            ISChildApplication = false;

#if  (DEFAULT_IS_TOUCH_APPLICATION)            
        IsTouchApplication = true;
#else  
        IsTouchApplication = false;
#endif
        }

        #endregion
    }
}
