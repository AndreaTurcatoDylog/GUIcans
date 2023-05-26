using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// This class store the ZoomImageValues:
    /// ZoomX, ZoomY, TranslateX, TranslateY.
    /// </summary>
    public class ZoomImageControlValues
    {
        public double ZoomX { get; private set; }
        public double ZoomY { get; private set; }
        public double TranslatedOffesetX { get; private set; }
        public double TranslatedOffesetY { get; private set; }

        public ZoomImageControlValues(double zoomX, double zoomY, double translatedOffesetX, double translatedOffesetY)
        {
            ZoomX = zoomX;
            ZoomY = zoomY;
            TranslatedOffesetX = translatedOffesetX;
            TranslatedOffesetY = translatedOffesetY;
        }
    }
}
