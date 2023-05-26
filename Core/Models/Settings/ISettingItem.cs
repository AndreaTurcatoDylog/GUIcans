using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public interface ISettingItemOption
    {}

    public interface ISettingItem: IDisposable
    {
        #region Properties

        int Type { get; set; }

        string Label { get; set; }

        bool IsUpdated { get; set; }

        bool IsUpdateChanged { get; }

        ISettingItemOption Options { get; }

        #endregion

        #region Events Handlers

        event EventHandler ValueChanged;

        #endregion

        #region Methods

        bool IsSettingItemUpdated();

        void OnIsValueChanged(object sender, EventArgs eventArgs);

        #endregion
    }
}
