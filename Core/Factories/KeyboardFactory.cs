using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public static class KeyboardFactory
    {

        public static KeyboardBase Get(KeyboardLayoutType keyboardType)
        {
            switch (keyboardType)
            {
                case KeyboardLayoutType.Standard:
                    return StandardKeyboard.Instance;
                case KeyboardLayoutType.Numeric:
                    return NumericKeyboard.Instance;
                case KeyboardLayoutType.Calendar:
                    return new CalendarKeyboard();
            }

            return null;
        }
    }
}
