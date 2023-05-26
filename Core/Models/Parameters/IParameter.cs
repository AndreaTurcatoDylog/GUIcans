using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public interface IParameter : ISettingItem
    {
        #region Properties

        /// <summary>
        /// It is the subtitle
        /// </summary>
        string Subtitle { get; set; }

        /// <summary>
        /// Spcify whether the Parameter has a reject
        /// </summary>
        bool IsRejectFound { get; set; }

        /// <summary>
        /// Specify whether the parameter can be edited or not
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Specify whether the inserted value is used (ex. calculation) or not
        /// </summary>
        bool IsUsed { get; set; }

        int Category { get; set; }

        int View { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the reject information changes
        /// </summary>
        event EventHandler OnIsRejectFoundChanged;

        #endregion
    }
}
