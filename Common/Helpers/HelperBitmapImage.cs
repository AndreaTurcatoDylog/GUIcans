using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Common
{
    public class HelperBitmapImage
    {
        /// <summary>
        /// Create a Bitmap image from file
        /// </summary>
        public BitmapImage CreateFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    bitmapImage.UriSource = new Uri(filePath, UriKind.RelativeOrAbsolute);
                    bitmapImage.EndInit();

                    return bitmapImage;
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return null;
        }
    }
}
