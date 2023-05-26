using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public abstract class SettingNumericItemBase : SettingItemBase, ISettingWithValue
    {
        #region Fields

        private double _Value;

        #endregion

        #region Properties

        public double Value
        {
            get { return _Value; }
            set
            {
                _Value = value;
                OnPropertyChanged("Value");

                if (_OriginalValue != null)
                {
                    var temp = IsUpdated;
                    IsUpdated = Convert.ToDouble(_OriginalValue) != Convert.ToDouble(value);
                    IsUpdateChanged = IsUpdated != temp;
                }
            }
        }

        public ISettingItemOption SettingNumericOption { get; private set; }

        #endregion

        #region Constructor

        public SettingNumericItemBase(string label, double value, ISettingItemOption settingItemOption)
            : base((int)Setting.NUMERIC, label, settingItemOption)
        {
            Value = value;
            _OriginalValue = value;

            SettingNumericOption = (ISettingNumericOption)settingItemOption;

            IsUpdated = false;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the original value (usually called after a save operation)
        /// </summary>
        public void UpdateOriginalValue()
        {
            IsUpdated = false;
            IsUpdateChanged = false;

            _OriginalValue = Value;
            Value = Convert.ToDouble(_OriginalValue);
        }

        /// <summary>
        /// Remove the changes in the object restoring the original value
        /// </summary>
        public void RestoreOriginalValue()
        {
            IsUpdated = false;
            IsUpdateChanged = false;

            Value = Convert.ToDouble(_OriginalValue);
        }

        public object GetValue()
        {
            return Value;
        }

        #endregion
    }
}
