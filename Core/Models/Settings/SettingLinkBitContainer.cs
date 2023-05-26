using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class SettingLinkBitContainer : SettingLinkWithValueBase
    {
        #region Fields

        private int _NumericValue;

        #endregion

        #region Constructor

        public SettingLinkBitContainer(string label)
            : this(label, null, null, true)
        { }

        public SettingLinkBitContainer(string label, List<ISettingItem> next, object value, bool resultVisible)
            : base(Setting.LINK_WITH_BIT_RESULT_VALUE,label, next, value, resultVisible)
        {}

        #endregion

        #region Methods

        /// <summary>
        ///  Set the children values (ISettingItems in Next List)
        /// </summary>
        protected override void SetChildrenValues()
        {
            int result = 0;
            int ValueToReach = Convert.ToInt32(Value);

            for (var index = Next.Count() - 1; index > 0; index--)
            {
                var settingItem = Next[index];
                var exp = index - 1;
                var tempResult = result + Convert.ToInt32(Math.Pow(2, exp)); ;

                if (tempResult <= ValueToReach)
                {
                    if (settingItem != null && settingItem is SettingBit)
                    {
                        var settingBit = (settingItem as SettingBit);
                        settingBit.Value = true;
                        settingBit.SetOriginalValue(true);
                    }

                    result = tempResult;
                }

                if (settingItem != null && settingItem is SettingBit)
                {
                    settingItem.IsUpdated = false;
                }
            }

            // Update the value with integer conversion
            _NumericValue = result;

            if (_ResultVisible)
            {
                Value = _NumericValue;
            }
            else
            {
                Value = null;
            }
        }

        // Update the value in according to logic through parent and ISettingItems in Next List.
        // Every element in Next is a bit so the value of parent must be the integer result conversion 
        protected override void ValueChangedAction(object sender)
        {
            int result = 0;
            var settingItems = Next.Where(i => i is SettingBit).ToList();

            for (var index = settingItems.Count() - 1; index >= 0; index--)
            {
                var settingItem = settingItems[index] as SettingBit;
                if (settingItem.Value)
                {
                    var exp = index;
                    result += Convert.ToInt32(Math.Pow(2, exp));
                }
            }

            // Update the value with integer conversion
            _NumericValue = result;

            if (_ResultVisible)
            {
                Value = _NumericValue;
            }
            else
            {
                Value = null;
            }
        }

        /// <summary>
        /// Get the value
        /// </summary>
        public override object GetValue()
        {
            return _NumericValue;
        }

            #endregion
        }
}
