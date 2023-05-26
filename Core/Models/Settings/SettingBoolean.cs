using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{

    public class SettingBoolean : SettingWithValue<bool>
    {
        #region Constructor

        public SettingBoolean(string label, bool value)
            : base((int)Setting.BOOLEAN, label, value)
        {
            Value = value;
            _OriginalValue = value;
        }

        #endregion

        #region Methods

        public override object GetValue()
        {
            return Value ? "1" : "0";
        }

        /// <summary>
        /// The to string method
        /// </summary>
        public override string ToString()
        {
            return Value ? "1" : "0";
        }

        #endregion
    }
}
