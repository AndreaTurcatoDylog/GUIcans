using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class FilesResult
    {
        // The list of fullpath and name of the images files 
        // found in the selected folder
        public IEnumerable<string> FileNames { get; private set; }

        public string Message { get; private set; }

        public bool IsSuccess { get; private set; }

        #region Constructor

        public FilesResult(bool isSuccess, IEnumerable<string> fileNames, string message)
        {
            IsSuccess = isSuccess;
            FileNames = fileNames;
            Message = message;
        }

        #endregion
    }
}
