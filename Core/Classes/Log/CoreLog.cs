using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// This class is a Singleton and it is used to write a Log.
    /// The project Core is indipendent from Log used by the Main Project becouse this class store
    /// a delegate. In Main Project the SetFunction must me called to set a method of specificated type
    /// </summary>
    public class CoreLog
    {
        #region Delegate

        public delegate void WriteFunction(string message);

        #endregion

        #region Members

        private static CoreLog _Instance;

        private static WriteFunction _WriteErrorFunc;
        private static WriteFunction _WriteWarningFunc;

        #endregion

        #region Properties

        public static CoreLog Instance
        {
            get
            {
                if (_Instance==null)
                {
                    _Instance = new CoreLog();
                }

                return _Instance;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Call this method to write a Message
        /// </summary>
        public void Append(string message, CoreLogType coreLogType)
        {
            WriteFunction writeFunction = null;

            switch (coreLogType)
            {
                case CoreLogType.Error: writeFunction = _WriteErrorFunc; break;
                case CoreLogType.Warning: writeFunction = _WriteWarningFunc; break;
            }

            if (writeFunction!=null)
            {
                writeFunction.Invoke(message);
            }
        }

        /// <summary>
        /// Set the function of specificated type
        /// </summary>
        public void SetFunction(CoreLogType coreLogType, WriteFunction writeFunction = null)
        {
            switch (coreLogType)
            {
                case CoreLogType.Error: _WriteErrorFunc = writeFunction; break;
                case CoreLogType.Warning: _WriteWarningFunc = writeFunction; break;
            }
        }

        #endregion
    }
}
