using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class BooleanToEnabledConverter : BooleanConverter<bool>
    {
        public BooleanToEnabledConverter() :
            base(true, false)
        { }
    }
}
