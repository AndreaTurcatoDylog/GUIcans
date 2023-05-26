using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public abstract class SettingLinkBase : SettingItemBase
    {
        #region Fields

        protected List<ISettingItem> _Next;

        #endregion

        #region Properties

        public List<ISettingItem> Next
        {
            get { return _Next; }
            set
            {
                _Next = value;
                OnPropertyChanged("Next");
            }
        }

        #endregion

        #region Constructor

        public SettingLinkBase(Setting settingType, string label)
            : this(settingType, label, null)
        { }

        public SettingLinkBase(Setting settingType, string label, List<ISettingItem> next)
            : base((int)settingType, label)
        {
            Next = next;
            IsUpdated = false;
        }

        #endregion

        #region Virtual Methods

        public override bool IsSettingItemUpdated()
        {
            var index = 0;
            var isUpdated = false;

            while (!isUpdated && index <= Next?.Count - 1)
            {
                var item = Next[index];
                if (item != null)
                {
                    isUpdated = item.IsSettingItemUpdated();
                }

                index++;
            }

            IsUpdated = isUpdated;

            return isUpdated;
        }

        #endregion
    }
}
