using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class ParameterContainerUpdatedEventArgs: EventArgs
    {
        public bool IsUpdated { get; private set; }

        public object Tag { get; private set; }

        public ParameterContainerUpdatedEventArgs(bool isUpdated, object tag = null)
        {
            IsUpdated = isUpdated;
            Tag = tag;
        }
    }
}
