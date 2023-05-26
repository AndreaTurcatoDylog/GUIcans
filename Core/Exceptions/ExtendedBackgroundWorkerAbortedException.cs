using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// Used when the Background worker is aborted.
    /// Capture this exception to kwow when a Background worker is aborted.
    /// The Background worker is an extended object created becouse the common BackGround worker has no
    /// mechanism to kwnow if it is aborted
    /// </summary>
    public class ExtendedBackgroundWorkerAbortedException : Exception
    {
        public ExtendedBackgroundWorkerAbortedException()
        { }

        public ExtendedBackgroundWorkerAbortedException(string message)
            : base(message)
        { }
    }
}
