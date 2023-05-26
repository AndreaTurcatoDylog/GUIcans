using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    // The ImageMouseDownWithoutTransformation is fired if a click on image occurs 
    // whithout transformation (Translate or Zoom). The MouseX and MouseY coordinates on Image are
    // returned in parameters
    public class ZoomImagePositionValuesEventArgs: EventArgs
    {
        public double PositionX { get; private set; }
        public double PositionY { get; private set; }

        public ZoomImagePositionValuesEventArgs(double positionX, double positiony)
        {
            PositionX = positionX;
            PositionY = positiony;
        }
    }
}
