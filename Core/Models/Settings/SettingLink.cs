using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class SettingLink : SettingLinkBase
    {
        #region Constructor

        public SettingLink(string label)
            : this(label, null)
        { }

        public SettingLink(string label, List<ISettingItem> next)
            : base(Setting.LINK, label, next)
        {
            IsUpdated = false;
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Dispose the object
        /// </summary>
        protected override void DisposeObjects()
        {

            foreach (var settingItem in Next)
            {
                settingItem?.Dispose();
            }

            base.DisposeObjects();
        }

        #endregion
    }
}
