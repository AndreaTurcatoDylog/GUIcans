using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public static class FileJsonManager<T> where T : class
    {
        #region Methods

        /// <summary>
        /// Load the Machines from file
        /// </summary>
        public static T LoadFromFile(string pathFile, Encoding encoding, JsonSerializerSettings jsonSerializerSettings = null)
        {
            try
            {
                if (File.Exists(pathFile))
                {
                    //var text = File.ReadAllText(pathFile, Encoding.UTF7);
                    var text = File.ReadAllText(pathFile, encoding);

                    return (jsonSerializerSettings == null) ? JsonConvert.DeserializeObject<T>(text)
                                                         : JsonConvert.DeserializeObject<T>(text, jsonSerializerSettings);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return null;
        }

        /// <summary>
        /// Save the specificated file
        /// </summary>
        public static void SaveFile(string fullPath, object objectToSave)
        {
            if (objectToSave is T)
            {
                try
                {
                    // Serialize JSON directly to a file
                    using (StreamWriter file = new StreamWriter(fullPath, false, Encoding.UTF8))
                    {
                        var serializer = new JsonSerializer
                        {
                            Formatting = Formatting.Indented
                        };
                        serializer.Serialize(file, objectToSave);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        #endregion
    }
}
