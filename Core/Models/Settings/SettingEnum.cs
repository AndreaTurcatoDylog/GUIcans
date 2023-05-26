using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public enum Setting
    {
        EMPTY = -1,
        TEXT = 0,
        BOOLEAN = 1,
        NUMERIC = 2,
        CHECKED = 3,
        BIT = 4,
        LINK = 5,
        BIT_NUMBER = 6,
        LINK_WITH_VALUE = 7,
        BIT_CONTAINER = 8,
        CHECKED_CONTAINER = 9,
        COMMON_CHECKED_CONTAINER = 10,
        LINK_UP = 11,
        LINK_WITH_BIT_RESULT_VALUE = 12,
        LINK_WITH_SINGLE_RESULT_VALUE = 13,
        LINK_WITH_COMMON_SINGLE_RESULT_VALUE = 14,
    }
}
