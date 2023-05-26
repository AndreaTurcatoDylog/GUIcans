using System;

namespace Core
{
    public class SettingUpdatedEventArgs: EventArgs
    {
        public ISettingItem SettingItem { get; private set; }

        public SettingUpdatedEventArgs(ISettingItem settingItem)
        {
            SettingItem = settingItem;
        }
    }
}
