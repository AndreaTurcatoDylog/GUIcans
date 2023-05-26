using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// This is a model for the External Dialog Message. 
    /// It is used for sending a message in the Messager structure
    /// </summary>
    public class ExternalDialogMessage
    {
        public IDialogViewModel ViewModel { get; set; }
    }
}
