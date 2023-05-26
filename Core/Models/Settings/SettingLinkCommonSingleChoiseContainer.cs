using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class SettingLinkCommonSingleChoiseContainer : SettingLinkBase, ISettingWithValue, IDisposable
    {
        #region Fields

        private List<List<ISettingItem>> _SettingCheckedItems;
        private List<int> _NoCommonValues;

        #endregion

        #region Constructor

        public SettingLinkCommonSingleChoiseContainer(string label, List<ISettingItem> next, params int[] noCommonValues)
            : base(Setting.LINK_WITH_COMMON_SINGLE_RESULT_VALUE, label, next)
        {
            // Set the no common values
            _NoCommonValues = new List<int>(noCommonValues);

            // Set the common setting items
            SetCommonSingleChoiseSettings();

            // Register events
            RegisterEvents(true);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Register\Unregister the event for eache SettingChecked
        /// </summary>
        private void RegisterEvents(bool register)
        {
            if (register)
            {
                // Insert the subcription events
            }
            else
            {
                // Insert the unsubscription events
            }

            RegisterSettingCheckedItemsEvents(register);
        }

        /// <summary>
        /// Register\Unregister the events on all Setting Checked items
        /// </summary>
        private void RegisterSettingCheckedItemsEvents(bool register)
        {
            foreach (var checkedItemList in _SettingCheckedItems)
            {
                foreach (var checkeItem in checkedItemList)
                {
                    if (checkeItem != null && checkeItem is SettingChecked)
                    {
                        if (register)
                        {
                            (checkeItem as SettingChecked).CheckedChanged += CommonSingleChoiseCheckedChanged;
                        }
                        else
                        {
                            (checkeItem as SettingChecked).CheckedChanged -= CommonSingleChoiseCheckedChanged;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Set the common single changes.
        /// Disable\Enable the setting checked in according to the choose of every element.
        /// </summary>
        /// <param name="createList"> specify wheter the value must be save in _SettingCheckedItems list of not</param>

        private void SetCommonSingleChoiseSettings()
        {
            _SettingCheckedItems = new List<List<ISettingItem>>();

            // The list of setting that are checked
            var settingCheckedList = new List<ISettingItem>();

            // The list of settings that have common setting checked
            var containers = Next.Where(s => s is SettingLinkSingleChoiseContainer).ToList();
            foreach (var container in containers)
            {
                // Create the list without excluded elements
                var checkedItems = (container as SettingLinkSingleChoiseContainer).Next;
                checkedItems = checkedItems.Where(s => (s != null) && (s is SettingChecked)).ToList();

                if (checkedItems.Any())
                {
                    _SettingCheckedItems.Add(checkedItems);

                    // Get the setting that is checked in the list
                    var settingChecked = checkedItems.SingleOrDefault(s => (s is SettingChecked) && (s as SettingChecked).Checked);
                    if (settingChecked != null)
                    {
                        settingCheckedList.Add(settingChecked);
                    }
                }
            }

            // For each setting that is checked it is necessary to change the enabled information of its siblings
            foreach (var settingChecked in settingCheckedList)
            {
                ChangeEnableOfSibling((settingChecked as SettingChecked));
            }
        }

        /// <summary>
        /// Change the enable of the sibling of the setting checked that changed its value
        /// </summary>
        private void ChangeEnableOfSibling(SettingChecked settingChecked)
        {
            var isExludedValue = _NoCommonValues.Contains(Convert.ToInt32(settingChecked.Value));
            if (!isExludedValue)
            {
                // Get the position of setting checked that chamged is Checked
                var list = _SettingCheckedItems.First();
                var el = list.SingleOrDefault(s => (s is SettingChecked) && (Convert.ToInt32((s as SettingChecked).Value) == (Convert.ToInt32(settingChecked.Value))));
                var pos = list.IndexOf(el);

                // Enable\Disable all setting checked of all siblings
                foreach (var element in _SettingCheckedItems)
                {
                    var settingElement = (element[pos] as SettingChecked);
                    if (settingElement != settingChecked)
                    {
                        settingElement.Enabled = !settingChecked.Checked;
                    }
                }
            }
        }

        /// <summary>
        /// Restore the original value
        /// </summary>
        public void RestoreOriginalValue()
        {
            // Stop to listen to setting checked items event
            RegisterSettingCheckedItemsEvents(false);

            // Call the restore method of all nested objects
            IsUpdated = false;
            IsUpdateChanged = false;
            foreach (var setting in Next)
            {
                if (setting is ISettingWithValue)
                {
                    (setting as ISettingWithValue).RestoreOriginalValue();
                }
            }

            // Reset the enable information of all settingChecked items
            foreach (var container in _SettingCheckedItems)
            {
                foreach (var settingChecked in container)
                {
                    (settingChecked as SettingChecked).Enabled = true;
                }
            }

            // Set the common setting items
            SetCommonSingleChoiseSettings();

            // Resume the listening setting checked items event
            RegisterSettingCheckedItemsEvents(true);
        }

        /// <summary>
        /// Update the original value
        /// </summary>
        public void UpdateOriginalValue()
        {
            IsUpdated = false;
            foreach (var setting in Next)
            {
                if (setting is ISettingWithValue)
                {
                    (setting as ISettingWithValue).UpdateOriginalValue();
                }
            }
        }

        /// <summary>
        /// Get the value tha is a combination of all the choosen common values
        /// </summary>
        public object GetValue()
        {
            var result = string.Empty;
            var containers = Next.Where(s => s is SettingLinkSingleChoiseContainer).ToList();
            foreach (var container in containers)
            {
                if (container is ISettingWithValue)
                {
                    var value = (container as ISettingWithValue).GetValue();
                    result = $"{result}{value}";
                }
            }

            return result;
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the checked of common radio changes
        /// </summary>
        private void CommonSingleChoiseCheckedChanged(object sender, EventArgs e)
        {
            if (_SettingCheckedItems != null && _SettingCheckedItems.Any())
            {
                // For the setting that is checked it is necessary to change the enabled information of its siblings
                var settingChecked = (sender as SettingChecked);
                ChangeEnableOfSibling((settingChecked as SettingChecked));
            }
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Dispose the object
        /// </summary>
        public override void Dispose()
        {
            GC.SuppressFinalize(this);
            RegisterEvents(false);

            base.Dispose();
        }

        #endregion
    }
}
