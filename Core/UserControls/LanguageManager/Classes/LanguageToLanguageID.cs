using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// Convert the Language string code ('eng', 'it', ecc) to the Language ID
    /// </summary>
    internal static class LanguageToLanguageID
    {
        public static LanguageID Convert(string languageCode)
        {
            switch (languageCode)
            {
                case "en": return LanguageID.English;
                case "it": return LanguageID.Italy;
                case "fr": return LanguageID.French;
                default: return LanguageID.None;
            }           
        }
    }
}
