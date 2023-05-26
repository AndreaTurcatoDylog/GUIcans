using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Common
{
    public interface ICommonLog
    {
        // ILog Append { get; }
        void Debug(string message);
        void Error(string message);
    }

    /// <summary>
    /// This class is a wrapper for Log4Net
    /// </summary>
    public class CommonLog : ICommonLog
    {
        #region Fields

        private readonly ILog _Log;

        #endregion

        #region Properties

        public ILog Append
        {
            get { return _Log; }
        }

        #endregion

        #region Constructor

        public CommonLog()
        {
            // The App Config must be set as Resource in its Properties->Build Action
            Uri uri = new Uri("pack://application:,,,/App.config");
            var streamResourceInfo = Application.GetResourceStream(uri);

            _Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log4net.Config.XmlConfigurator.Configure(streamResourceInfo.Stream);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Append a new string to the Debug Log
        /// </summary>
        public void Debug(string message)
        {
            var line = "------------------------------------------------------------------------";
            var logMessage = $"{Environment.NewLine}{line}{Environment.NewLine}{DateTime.UtcNow}" +
                             $"{Environment.NewLine}{message}";

            _Log.Debug(logMessage);
        }

        /// <summary>
        /// Append a new string to the Error Log
        /// </summary>
        public void Error(string message)
        {
            var line = "------------------------------------------------------------------------";
            var logMessage = $"{Environment.NewLine}{message}{Environment.NewLine}{line}{Environment.NewLine}";

            _Log.Debug(logMessage);
        }

        #endregion
    }
}
