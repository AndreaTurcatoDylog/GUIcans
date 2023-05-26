using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Core
{
    public class TextBoxKeyboard : TextBoxKeyboardBase
    {
        #region Constructor

        static TextBoxKeyboard()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBoxKeyboard), new FrameworkPropertyMetadata(typeof(TextBoxKeyboard)));
        }

        public TextBoxKeyboard()
            :base()
        {
            OffsetY = 20;
            KeyboardType = KeyboardLayoutType.Standard;
        }

        #endregion

        #region Methods

        protected override Rect MoveParentWindow(Rect elementPosition)
        {
            var position = elementPosition;

            // Get the keyboard placement
            var keyboardposition = _Keyboard.GetAbsolutePlacement(true);

            // Calculate the difference between keyboard top position and element top position
            var difference = ((keyboardposition.Top - elementPosition.Top) - elementPosition.Height);
            if (difference <= 0)
            {
                _AdjustableContainer?.MoveUp(difference - OffsetY);

                var newTop = position.Top + (difference - OffsetY);
                position = new Rect(position.Left, newTop, position.Width, position.Height);
            }

            return position;
        }

        #endregion
    }
}
