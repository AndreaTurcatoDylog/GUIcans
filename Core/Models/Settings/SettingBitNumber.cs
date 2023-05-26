using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class SettingBitNumber
    {
        #region Fields
        #endregion

        #region Properties

        public ObservableCollection<ISettingItem> Bits { get; set; }
        public int IntegerNumber { get; set; }

        #endregion

        #region Constructor

        public SettingBitNumber()
        {

        }

        public SettingBitNumber(IEnumerable<string> labels, int number)
        {
            CreateBitNumber(labels, number);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create the labels and the bit information from integer number.
        /// A single bit inforamtion is made up by a label and bool value (0 = false, 1 = true)
        /// </summary>
        private void CreateBitNumber(IEnumerable<string> labels, int number)
        {
            IntegerNumber = number;

            if (labels != null)
            {
                // Calculate the binary digit
                string binary = Convert.ToString(number, 2);

                // Calculate the max number that can be rappresented with a specificated number of bit
                var exp = labels.Count();
                var maxNumber = Math.Pow(2, exp)-1;

                if (labels.Count() >= binary.Count() && number <= maxNumber)
                {
                    // Fill the most significant digit to 0
                    binary = binary.PadLeft(labels.Count(), '0');

                    Bits = new ObservableCollection<ISettingItem>();

                    int index = 0;
                    foreach (var key in labels)
                    {
                        bool binaryValue;
                        if (index <= binary.Count() - 1)
                        {
                            binaryValue = (binary[index] == '1') ? true : false;
                        }
                        else
                        {
                            binaryValue = false;
                        }

                        // Create new bit setting object
                        var settingBit = new SettingBit(key, binaryValue);

                        // Add boolean setting to list
                        Bits.Insert(0, settingBit);

                        index++;
                    }
                }
            }
        }

        #endregion
    }
}
