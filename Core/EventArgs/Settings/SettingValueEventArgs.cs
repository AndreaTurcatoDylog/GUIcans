using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class SettingValueEventArgs : EventArgs
    {
        public object Value { get; private set; }

        public SettingValueEventArgs(object value)
        {
            Value = value;
        }
    }
}
