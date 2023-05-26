using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public abstract class SettingWithValue<T> : SettingItemBase, ISettingWithValue
    {
        #region Members

        protected T _Value;

        #endregion

        #region Properties

        public T Value
        {
            get { return _Value; }
            set
            {
                if (!EqualityComparer<T>.Default.Equals(_Value, value))
                {
                    _Value = value;
                    OnPropertyChanged("Value");

                    // Fire the value changed events
                    OnIsValueChanged(this, new SettingValueEventArgs(value));

                    if (_OriginalValue != null)
                    {
                        var temp = IsUpdated;
                        IsUpdated = !EqualityComparer<T>.Default.Equals(value, (T)_OriginalValue);
                        IsUpdateChanged = IsUpdated != temp;
                    }
                }
            }
        }

        public static byte[] SettingInformation = new byte[] { 232, 155, 125, 60, 248, 174, 168, 121, 224, 145, 217, 195, 10 };

        #endregion

        #region Constructor

        public SettingWithValue(int type, string label, T value)
            : base(type, label)
        {
            Value = value;
            _OriginalValue = value;

            IsUpdated = false;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the value
        /// </summary>

        public virtual object GetValue()
        {
            return Value;
        }

        /// <summary>
        /// Updates the original value (usually called after a save operation)
        /// </summary>
        public virtual void UpdateOriginalValue()
        {
            IsUpdated = false;
            IsUpdateChanged = false;

            _OriginalValue = Value;
            Value = (T)_OriginalValue;
        }

        /// <summary>
        /// Remove the changes in the object restoring the original value
        /// </summary>
        public void RestoreOriginalValue()
        {
            IsUpdated = false;
            IsUpdateChanged = false;

            Value = (T)_OriginalValue;
        }

        #endregion
    }
}
