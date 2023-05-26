using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Core
{
    public class LanguageItem : ModelBase
    {
        #region Members

        private string _CultureName;
        private string _CultureCode;
        private bool _IsApplyed;

        #endregion

        #region Properties

        /// <summary>
        /// It is the ID of the Item
        /// </summary>
        public LanguageID LanguageID { get; private set; }

        /// <summary>
        /// Get\Set the Image
        /// </summary>
        public BitmapImage Image { get; set; }

        /// <summary>
        /// Get\Set the Label
        /// </summary>
        public string CultureName
        {
            get { return _CultureName; }
            set
            {
                _CultureName = value;
                OnPropertyChanged("CultureName");
            }
        }

        /// <summary>
        /// Get\Set the Culture Code
        /// ex. "it", "eng" etc...
        /// </summary>
        public string CultureCode
        {
            get { return _CultureCode; }
            set
            {
                _CultureCode = value;
                OnPropertyChanged("CultureCode");
            }
        }

        /// <summary>
        /// Get\Set the Is IsApplyed 
        /// </summary>
        public bool IsApplyed
        {
            get { return _IsApplyed; }
            set
            {
                _IsApplyed = value;
                OnPropertyChanged("IsApplyed");
            }
        }

        #endregion

        #region Constructor

        public LanguageItem(LanguageID languageID, string cultureName, string cultureCode, BitmapImage image, bool isApplyed = false)
        {
            CultureName = cultureName;
            CultureCode = cultureCode;
            Image = image;
            LanguageID = languageID;
            IsApplyed = isApplyed;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Change the language
        /// </summary>
        public void ChangeLanguage()
        {
            OnPropertyChanged("CultureName");
        }

        #endregion
    }
}
