using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// Contains the information of an Area: Top, Left, Width, Height
    /// </summary>
    public class AreaDimension
    {
        #region Properties

        public int Left { get; private set; }
        public int Top { get; private set; }
        public int Bottom { get; private set; }
        public int Right { get; private set; }

        public System.Drawing.Size Size => new System.Drawing.Size(Right - Left, Bottom - Top);

        #endregion

        #region Constructor

        public AreaDimension(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        #endregion
    }
}
