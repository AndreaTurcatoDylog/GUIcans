using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Core
{
    public class SettingLinkSingleChoiseContainer : SettingLinkWithValueBase
    {
       #region Constructor

        public SettingLinkSingleChoiseContainer(string label)
            : this(label, null, null, true)
        { }

        public SettingLinkSingleChoiseContainer(string label, List<ISettingItem> next, object value, bool resultVisible)
            : base(Setting.LINK_WITH_SINGLE_RESULT_VALUE,label, next, value, resultVisible)
        {}

        #endregion

        #region Methods

        /// <summary>
        ///  Set the children values (ISettingItems in Next List)
        /// </summary>
        protected override void SetChildrenValues()
        {
            // Get the numeric value
            var value = Convert.ToInt32(_OriginalValue);

            foreach (var item in Next)
            {
                if (item is SettingChecked)
                {
                    var settingItem = (SettingChecked)item;
                    var settingItemValue = Convert.ToInt32(settingItem.Value);

                    var isChecked= (value == settingItemValue);

                    settingItem.Checked = isChecked;
                    settingItem.IsUpdated = false;
                    if (isChecked)
                    {
                        settingItem.SetOriginalValue(isChecked);

                        // Set the label value of link
                        if (_ResultVisible)
                        {
                            Value = settingItem.Label;
                        }
                        else
                        {
                            Value = null;
                        }

                        // Fire the value changed events
                        OnIsValueChanged(this, new SettingValueEventArgs(settingItem.Label));
                    }
                }
            }
        }

        // Update the value in according to logic through parent and ISettingItems in Next List.
        // Every element in Next is a single choice so the value of parent must be choosen item
        protected override void ValueChangedAction(object sender)
        {
            var label = string.Empty;

            var settingItem = (sender as SettingChecked);
            if (settingItem.Checked)
            {
                // Get the label of choosen item
                label = Next.Where(p => p is SettingChecked)
                            .SingleOrDefault(s => (s as SettingChecked).Value == settingItem.Value)?.Label;

                // Fire the value changed events
                OnIsValueChanged(this, new SettingValueEventArgs(label));

                // Set the value if it is visible
                Value = (_ResultVisible) ? label : null;
            }
        }

        /// <summary>
        /// Update the original value
        /// </summary>
        public override void UpdateOriginalValue()
        {
            base.UpdateOriginalValue();

            var checkedSettingItem = Next.Where(p => p is SettingChecked)
                                         .SingleOrDefault(s => (s as SettingChecked).Checked);

            _OriginalValue = (checkedSettingItem as SettingChecked).Value;
        }

        /// <summary>
        /// Get the value
        /// </summary>
        public override object GetValue()
        {
            var settingItem = Next.Where(p => p!=null && p is SettingChecked)
                            .SingleOrDefault(s => (s as SettingChecked).Checked);

            if (settingItem != null)
            {
                return (settingItem as SettingChecked).Value;
            }

            return null;
        }

        #endregion
    }
}
