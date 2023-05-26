using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// This class manage the Monitor information
    /// </summary>
    public class MonitorInfo
    {
        #region Properties

        public AreaDimension Screen { get; private set; }
        public AreaDimension WorkArea { get; private set; }

        #endregion

        #region Constructor

        public MonitorInfo(AreaDimension screen, AreaDimension workArea)
        {
            Screen = screen;
            WorkArea = workArea;
        }

        public MonitorInfo(int screenLeft, int screenTop, int screenRight, int screenBottom,
            int workAreaLeft, int workAreaTop, int workAreaRight, int workAreaBottom)
        {
            Screen = new AreaDimension(screenLeft, screenTop, screenRight, screenBottom);
            WorkArea = new AreaDimension(screenLeft, workAreaTop, workAreaRight, workAreaBottom);
        }

        #endregion
    }
}
