using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class WritingModeEventArgs: EventArgs
    {
        public bool IsWritingMode { get; private set; }

        public WritingModeEventArgs(bool isWritingMode)
        {
            IsWritingMode = isWritingMode;
        }
    }
}
