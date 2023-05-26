using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Status : ISettings
    {
        #region Properties

        #region PDF Report

        [JsonProperty("PDFSavePath")]
        public string PDFSavePath { get; set; }

        #endregion

        #region Others

        [JsonProperty("StartSoftwarePath")]
        public string StartSoftwarePath { get; set; }

        [JsonProperty("Domain")]
        public string Domain { get; set; }

        #endregion

        #region Filters

        [JsonProperty("FromDate")]
        public string FromDate { get; set; }

        [JsonProperty("ToDate")]
        public string ToDate { get; set; }

        [JsonProperty("Filter")]
        public string Filter { get; set; }

        #endregion

        #region Others

        /// <summary>
        /// Specify whether the Remote Report is a Child.
        /// If true the Exit button and the Window Main Bar (with exit, minimaze and maximaze buttons) are not visible
        /// </summary>
        [JsonProperty("ISChildApplication")]
        public bool ISChildApplication { get; set; }

        #endregion

        #endregion
    }
}
