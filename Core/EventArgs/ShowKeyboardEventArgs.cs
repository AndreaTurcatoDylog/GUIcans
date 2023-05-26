using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Core
{
    public class ShowKeyboardEventArgs: EventArgs
    {
        public Rect ElementPosition { get; private set; }

        public ShowKeyboardEventArgs(Rect elementPosition)
        {
            ElementPosition = elementPosition;
        }
    }
}
