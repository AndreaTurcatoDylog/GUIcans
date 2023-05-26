using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class SettingBit : SettingWithValue<bool>
    {       
        #region Constructor

        public SettingBit(string label, bool value)
            : base((int)Setting.BIT, label, value)
        {
            Value = value;

            _OriginalValue = value;
        }

        #endregion

        #region Method

        /// <summary>
        /// Set the original value
        /// </summary>
        public void SetOriginalValue(bool value)
        {
            _OriginalValue = value;
        }

        public override object GetValue()
        {
            return Value ? "1" : "0";
        }

        #endregion
    }
}
