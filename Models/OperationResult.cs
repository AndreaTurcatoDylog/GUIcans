using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{   
    public class OperationResult
    {       
        public string Message { get; private set; }

        public bool IsSuccess{ get; private set; }

        #region Constructor

        public OperationResult(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        #endregion
    }
}
