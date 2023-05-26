using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class GenericMessage: IMessageItem
    {
        #region Properties

        public string Title { get;  private set; }

        public string Message { get;  private set; }

        public bool IsSelected { get; set; }

        public GenericMessageType GenericMessageType { get; private set; }

        #endregion

        #region Constructor

        public GenericMessage(string title, string message, GenericMessageType messageType)
        {
            Title = title;
            Message = message;
            GenericMessageType = messageType;
        }

        #endregion
    }
}
