using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class SettingRadioList
    {
        #region Fields
        #endregion

        #region Properties

        public ObservableCollection<ISettingItem> Radios { get; set; }
        public ISettingItem Checked { get; set; }

        #endregion

        #region Constructor

        public SettingRadioList()
        {

        }

        public SettingRadioList(IEnumerable<ISettingItem> radios)
        {
            Radios = new ObservableCollection<ISettingItem>(radios);
            Checked = null;
        }

        #endregion

        #region Methods

        
        }

        #endregion
    }

