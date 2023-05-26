using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Settings
    {
        #region Fields

        private static Settings _Instance;
        private CultureInfo _CultureInfo;

        #endregion

        #region Properties

        public static Settings Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new Settings();
                }
                return _Instance;
            }
        }

        public CultureInfo CultureInfo
        {
            set
            {
                _CultureInfo = value;
                CultureInfoChanged?.Invoke(this, new CultureInfoEventArgs(value));
            }
            get
            {
                return _CultureInfo;
            }
        }

        #endregion

        #region Event Handler

        public event EventHandler CultureInfoChanged;

        #endregion

        #region Constructor

        private Settings() { }

        #endregion  
    }
}
