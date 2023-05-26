using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    /// <summary>
    /// This class stores the init values of Software
    /// </summary>
    public class Commands : SettingsBase
    {
        #region Properties

        #region Filters

        [JsonProperty("FromDate")]
        public string FromDate { get; set; }

        [JsonProperty("ToDate")]
        public string ToDate { get; set; }

        [JsonProperty("Filter")]
        public string Filter { get; set; }


        #endregion

        #endregion
    }
}
