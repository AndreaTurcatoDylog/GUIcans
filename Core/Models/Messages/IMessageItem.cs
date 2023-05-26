using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public interface IMessageItem
    {
        string Title { get;}

        string Message { get; }

        bool IsSelected { get; set; }

        GenericMessageType GenericMessageType { get; }
    }
}
