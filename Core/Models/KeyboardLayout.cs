using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class KeyboardLayout
    {
        #region 

        private readonly char SplitCharacter = ',';

        #endregion

        #region Properties

        public Dictionary<string, string> LowerCaseLetter { get; private set; }
        public Dictionary<string, string> UpperCaseLetter { get; private set; }
        public Dictionary<string, string> SpecialCharacters { get; private set; }
        public Dictionary<string, string> NumericCharacters { get; private set; }

        #endregion

        #region Constructors

        public KeyboardLayout()
        {
            LowerCaseLetter = new Dictionary<string, string>();
            UpperCaseLetter = new Dictionary<string, string>();
            SpecialCharacters = new Dictionary<string, string>();
            NumericCharacters = new Dictionary<string, string>();
        }

        #endregion

        #region Methods

        public void CreateLayout(string resources)
        {
            LowerCaseLetter.Clear();
            UpperCaseLetter.Clear();
            SpecialCharacters.Clear();
            NumericCharacters.Clear();

            // The list of dictionaries to fiil
            var dictionaries = new List<Dictionary<string, string>>()
            {
               LowerCaseLetter,
               UpperCaseLetter,
               SpecialCharacters,
               NumericCharacters
            };


            var characters = resources.Replace('\n'.ToString(), string.Empty).Split('\r');
            try
            {
                string line;
                var index = 0;
                var charIndex = 0;
                while (charIndex < characters.Length)
                {
                    line = characters[charIndex];

                    var splittedString = line.Split(SplitCharacter);
                    var count = splittedString.Count();

                    if (count >= 1 && count <= 3)
                    {
                        var key = splittedString[0];
                        var value = string.Empty;

                        if (count == 2)
                        {
                            value = splittedString[count - 1];
                        }
                        else if (count == 3 && line.Last() == SplitCharacter)
                        {
                            value = new string(SplitCharacter, 1);
                        }

                        dictionaries[index].Add(key, value);

                        if (key == "030")
                        {
                            index++;
                        }
                    }

                    charIndex++;
                }
            }
            catch (Exception ex)
            {
                CoreLog.Instance.Append($"[{ex.Source}] - {ex.Message} - {ex.StackTrace}", CoreLogType.Error);
            }

            //// Read from file
            //using (var reader = new StreamReader(path))
            //{
            //    try
            //    {
            //        string line;
            //        var index = 0;
            //        while ((line = reader.ReadLine()) != null)
            //        {
            //            var splittedString = line.Split(SplitCharacter);
            //            var count = splittedString.Count();

            //            if (count >= 1 && count <= 3)
            //            {
            //                var key = splittedString[0];
            //                var value = string.Empty;

            //                if (count == 2)
            //                {
            //                    value = splittedString[count - 1];
            //                }
            //                else if (count == 3 && line.Last() == SplitCharacter)
            //                {
            //                    value = new string(SplitCharacter, 1);
            //                }

            //                dictionaries[index].Add(key, value);

            //                if (key == "030")
            //                {
            //                    index++;
            //                }
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {

            //    }
            //    finally
            //    {
            //        reader.Close();
            //    }
            //}
        }

        #endregion
    }
}
