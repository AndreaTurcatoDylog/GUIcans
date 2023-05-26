using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class SettingText : SettingWithValue<string>
    {
        #region Constructor

        public SettingText(string label, string value)
            : base((int)Setting.TEXT, label, value)
        {}

        #endregion

        #region Method

        public override object GetValue()
        {
            return $"\"{Value}\"";
        }

        #endregion
    }
}
