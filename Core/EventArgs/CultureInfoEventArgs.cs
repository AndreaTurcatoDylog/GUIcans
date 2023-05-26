using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class CultureInfoEventArgs : EventArgs
    {
        #region Properties

        public CultureInfo CultureInfo { get; private set; }

        #endregion

        #region Constructor

        public CultureInfoEventArgs(CultureInfo cultureInfo)
        {
            CultureInfo = cultureInfo;
        }

        #endregion
    }
}
