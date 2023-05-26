using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public static class PaginationButtonFactory
    {
        public static PaginationButton Get(PaginationButtonType paginationButtonType)
        {
            switch (paginationButtonType)
            {
                case PaginationButtonType.Standard:
                    return new PaginationButton();
                case PaginationButtonType.Parameters:
                    return new PaginationSettingButton();
            }

            return null;
        }
    }
}
