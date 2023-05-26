using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class SettingChecked : SettingItemBase, ISettingWithValue
    {
        #region Fields

        private object _Value;

        private bool _Checked;

        private bool _Enabled;

        #endregion

        #region Properties

        public object Value
        {
            get { return _Value; }
            set
            {
                _Value = value;
                OnPropertyChanged("Value");
            }
        }

        public bool Checked
        {
            get { return _Checked; }
            set
            {
                var changed = Checked != value;

                _Checked = value;
                OnPropertyChanged("Checked");
               
                if (changed)
                {
                    // Fire the  checked changed event
                    CheckedChanged?.Invoke(this, EventArgs.Empty);
                }

                // Set update information 
                var originalValue = Convert.ToBoolean(_OriginalValue);
                if (_OriginalValue != null)
                {
                    var temp = IsUpdated;
                    IsUpdated = (value != originalValue);
                    IsUpdateChanged = value != IsUpdated;
                }
            }
        }

        public bool Enabled
        {
            get { return _Enabled; }
            set
            {
                _Enabled = value;
                OnPropertyChanged("Enabled");
            }
        }

        #endregion

        #region EventHandler

        public event EventHandler CheckedChanged;

        #endregion

        #region Constructor

        public SettingChecked()
        {
            Type = (int)Setting.CHECKED;
        }

        public SettingChecked(string label, object value)
            : base((int)Setting.CHECKED, label)
        {
            Value = value;
            Checked = false;

            IsUpdated = false;
            Enabled = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the original value (usually called after a save operation)
        /// </summary>
        public void UpdateOriginalValue()
        {
            _OriginalValue = Checked;

            IsUpdated = false;
            IsUpdateChanged = false;
        }

        /// <summary>
        /// Remove the changes in the object restoring the original value
        /// </summary>
        public void RestoreOriginalValue()
        {
            if (_OriginalValue != null)
            {
                Checked = Convert.ToBoolean(_OriginalValue);
            }
            else
            {
                Checked = false;
                _OriginalValue = false;
            }

            IsUpdated = false;
            IsUpdateChanged = false;
        }

        /// <summary>
        /// Returns the value
        /// </summary>
        public object GetValue()
        {
            return Value;
        }

        public void SetOriginalValue(bool originalValue)
        {
            _OriginalValue = originalValue;
        }

        #endregion
    }
}
