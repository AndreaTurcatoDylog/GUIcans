using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    /// <summary>
    /// This class is a model for the Favorite Folders
    /// </summary>
    public class FavoriteFolders
    {
        #region Properties

        [JsonProperty("Favorites")]
        public List<SharedFolder> Favorites { get; set; }

        #endregion

        #region Constructor

        public FavoriteFolders()
        {
            Favorites = new List<SharedFolder>();
        }

        #endregion
    }
}
