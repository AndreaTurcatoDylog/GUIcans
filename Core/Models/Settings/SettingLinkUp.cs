using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class SettingLinkUp: SettingLinkBase
    {
        #region Constructor

        public SettingLinkUp(string label)
            : this(label, null)
        { }

        public SettingLinkUp(string label, List<ISettingItem> next)
            : base(Setting.LINK_UP, label, next)
        {
            IsUpdated = false;
        }

        #endregion

        #region Methods

        public override bool IsSettingItemUpdated()
        {
            return false;
        }

        #endregion
        }
}
