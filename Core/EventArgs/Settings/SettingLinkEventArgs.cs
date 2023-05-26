using System;
using System.Collections.Generic;

namespace Core
{
    public class SettingLinkEventArgs: EventArgs
    {
        public string Key { get; private set; }
        public List<ISettingItem> SettingItems { get; private set; }

        public ISettingItem SettingItemSender { get; private set; }

        public SettingLinkEventArgs(string key, List<ISettingItem> settingItems, ISettingItem settingItemSender)
        {
            Key = key;
            SettingItems = settingItems;
            SettingItemSender = settingItemSender;
        }
    }
}
