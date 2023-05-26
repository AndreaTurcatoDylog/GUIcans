using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class KeyboardButtonEventArgs: EventArgs
    {
        public KeyboardButtonType KeyboardButtonType { get; private set; }

        public string Content { get; private set; }

        public KeyboardButtonEventArgs(KeyboardButtonType keyboardButtonType, string content)
        {
            KeyboardButtonType = keyboardButtonType;
            Content = content;
        }

        public KeyboardButtonEventArgs(KeyboardButtonType keyboardButtonType)
            :this(keyboardButtonType, string.Empty)
        {
            KeyboardButtonType = keyboardButtonType;
        }
    }
}
