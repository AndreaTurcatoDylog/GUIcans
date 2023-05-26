using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// Rappresent the a Framework element enable to work with Keyboard.
    /// This element can move up its position to make space to keyboard and restore its position when keyboard is closed.
    /// The Width and Height properties are used in Numeric Keyboard
    /// The MoveUp and RestorePosition methods are used in Text Keyboard
    /// </summary>
    public interface IAdjustableFrameworkElement
    {
        double ActualWidth  { get;  }

        double ActualHeight  { get;  }

        double Width { get; set; }

        double Height { get; set; }

        void MoveUp(double newTopValue);

        void RestorePosition();
    }
}
