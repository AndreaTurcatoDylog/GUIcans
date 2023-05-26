using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Business
{
    /// <summary>
    /// The Splash Screen View Model
    /// </summary>
    public class SplashScreenViewModel: ViewModelBase
    {
        #region Members

        private BitmapImage _SplashImage;

        #endregion

        #region Properties

        /// <summary>
        /// Get the Title of the Window + Version
        /// </summary>
        public string TitleVersion
        {
            get
            {
                return $"Remote Report - rel. {System.Reflection.Assembly.GetEntryAssembly().GetName().Version}";
            }
        }

        /// <summary>
        /// Get the Image of the page
        /// </summary>
        public BitmapImage SplashImage
        {
            get
            {
                return _SplashImage;
            }
            set
            {
                _SplashImage = value;
                OnPropertyChanged("SplashImage");
            }
        }

        #endregion

        #region Constructor

        public SplashScreenViewModel(BitmapImage bitmapImage)
        {
            SplashImage = bitmapImage;
        }

        #endregion
    }
}
