using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class ChangePageEventArgs : EventArgs
    {
        public int CurrentPage { get; private set; }

        public ChangePageEventArgs(int currentPage)
        {
            CurrentPage = currentPage;
        }
    }
}
