using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public enum AGENT_ACTS
    {
        /// <summary>
        /// Copy
        /// </summary>
        AGENT_ROT_000CW = 0,           

        /// <summary>
        /// Copy and inverts columns
        /// </summary>
        AGENT_ROT_000CW_M = 1,

        /// <summary>
        /// Rotate 90° clockwise
        /// </summary>
        AGENT_ROT_090CW = 2,

        /// <summary>
        /// Rotate 90° clockwise and inverts rows
        /// </summary>
        AGENT_ROT_090CW_M = 3,

        /// <summary>
        /// Rotate 180° clockwise
        /// </summary>
        AGENT_ROT_180CW = 4,

        /// <summary>
        /// Rotate 180° clockwise and inverts columns
        /// </summary>
        AGENT_ROT_180CW_M = 5,

        /// <summary>
        /// Rotate 270° clockwise
        /// </summary>
        AGENT_ROT_270CW = 6,

        /// <summary>
        /// Rotate 270° clockwise and inverts rows
        /// </summary>
        AGENT_ROT_270CW_M = 7          
    }

}
