using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Core
{
    public class PathTrimTextBlock : TextBlock, IDisposable
    {
        #region Members

        #endregion

        #region Properties

        /// <summary>
        /// Get\Set Text of the TextBlock
        /// </summary>
        public string SourceText
        {
            get { return (string)GetValue(SourceTextProperty); }
            set { SetValue(SourceTextProperty, value); }
        }

        /// <summary>
        /// Get\Set Char seprator
        /// It is the character used to sepate the folder in Path
        /// </summary>
        public char CharacterSeparator
        {
            get { return (char)GetValue(CharacterSeparatorProperty); }
            set { SetValue(CharacterSeparatorProperty, value); }
        }

        /// <summary>
        /// Get\Set Offset
        /// </summary>
        public double Offset
        {
            get { return (double)GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }
        #endregion

        #region Dependency Properties

        public static DependencyProperty SourceTextProperty =
         DependencyProperty.Register("SourceText", typeof(string), typeof(PathTrimTextBlock),
                new PropertyMetadata(OnSourceTextChangedCallBack));

        public static DependencyProperty CharacterSeparatorProperty =
         DependencyProperty.Register("CharacterSeparator", typeof(char), typeof(PathTrimTextBlock));

        public static DependencyProperty OffsetProperty =
         DependencyProperty.Register("Offset", typeof(double), typeof(PathTrimTextBlock));

        #endregion

        #region Constructor

        public PathTrimTextBlock()
        {
            //Loaded += OnLoaded;
            RegisterEvents(true);
        }

        /// <summary>
        /// Occurs when the component is loaded
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
           // RegisterEvents(true);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Register\Unregister all events
        /// </summary>
        protected void RegisterEvents(bool register)
        {
            if (register)
            {
                SizeChanged += OnSizeChanged;
            }
            else
            {
              //  Loaded -= OnLoaded;

            }
        }

        ///// <summary>
        ///// Trim the Path if needed
        ///// </summary>
        //private void TrimPath()
        //{
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(SourceText))
        //        {
        //            // Get the Size of the string in the TextBlock
        //            var stringSize = SourceText.MeasureTextSize(FontFamily, FontStyle, FontWeight, FontStretch, FontSize);

        //            // Check if the stringSize is too long for the Border and in case reduce it
        //            if (stringSize != null && stringSize.Width + Offset > ActualWidth && ActualWidth > 0)
        //            {
        //                var newString = String.Copy(SourceText);
        //                newString = newString.Replace(">>", string.Empty);

        //                // Split the original string
        //                var splittedString = newString.Split(CharacterSeparator);

        //                splittedString = splittedString.Where(s => !string.IsNullOrEmpty(s)).ToArray();

        //                if (splittedString.Count() > 0)
        //                {
        //                    // Replace the beginning of the string
        //                    newString = newString.Replace(splittedString[0], "..");
        //                    newString = newString.Replace(">>", string.Empty);

        //                    // Get the new size
        //                    stringSize = newString.MeasureTextSize(FontFamily, FontStyle, FontWeight, FontStretch, FontSize);

        //                    bool isTooLong = stringSize != null && stringSize.Width + Offset > ActualWidth;
        //                    var index = 1;
        //                    while (isTooLong && index < splittedString.Count() - 1)
        //                    {
        //                        newString = newString.Replace(splittedString[index], string.Empty);
        //                        newString = newString.Replace(">>", $@"{CharacterSeparator}");

        //                        index = index + 1;

        //                        stringSize = newString.MeasureTextSize(FontFamily, FontStyle, FontWeight, FontStretch, FontSize);

        //                        isTooLong = stringSize != null && stringSize.Width + Offset > ActualWidth;
        //                    }

        //                    int count = newString.Count(f => f == CharacterSeparator);
        //                    if (count == 1)
        //                    {
        //                        newString = ".." + CharacterSeparator + splittedString.Last();
        //                    }

        //                    // Control whether the result string is in range of size
        //                    stringSize = newString.MeasureTextSize(FontFamily, FontStyle, FontWeight, FontStretch, FontSize);

        //                    isTooLong = stringSize != null && stringSize.Width + Offset > ActualWidth;
        //                    if (isTooLong && newString.Count() > 0 && newString.Count() >= 3) 
        //                    {
        //                        var intialString = newString.Substring(0, 3) + "..";
        //                        var numberOfCharacterToDelete = 1;
        //                        var tempString = newString;
        //                        while (isTooLong)
        //                        {
        //                            tempString = intialString + newString.Substring(4 + numberOfCharacterToDelete, newString.Length - (4 + numberOfCharacterToDelete));
        //                            stringSize = tempString.MeasureTextSize(FontFamily, FontStyle, FontWeight, FontStretch, FontSize);

        //                            isTooLong = stringSize != null && stringSize.Width + Offset > ActualWidth;

        //                            numberOfCharacterToDelete = numberOfCharacterToDelete + 1;
        //                        }

        //                        Text = tempString;
        //                    }
        //                    else
        //                    {
        //                        Text = newString;
        //                    }
        //                }
        //                else
        //                {
        //                    Text = SourceText;
        //                }
        //            }
        //            else
        //            {
        //                Text = SourceText;
        //            }
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        Console.WriteLine("[TrimPath] - " + ex.Message);
        //    }
        //}

        /// <summary>
        /// Trim the Path if needed
        /// </summary>
        private void TrimPath()
        {
            try
            {
                if (!string.IsNullOrEmpty(SourceText))
                {
                    // Used when the string path is trimmed. In this case in the string some
                    // double characted separator occurs and must be replaced with only one Character separator
                    var doubleCharacterSeparator = $"{CharacterSeparator}{CharacterSeparator}";

                    // Get the Size of the string in the TextBlock
                    var stringSize = SourceText.MeasureTextSize(FontFamily, FontStyle, FontWeight, FontStretch, FontSize);

                    // Check if the stringSize is too long for the Border and in case reduce it
                    if (stringSize != null && stringSize.Width + Offset > ActualWidth && ActualWidth > 0)
                    {
                        var newString = String.Copy(SourceText);
                        newString = newString.Replace(doubleCharacterSeparator, string.Empty);

                        // Split the original string
                        var splittedString = newString.Split(CharacterSeparator);

                        splittedString = splittedString.Where(s => !string.IsNullOrEmpty(s)).ToArray();

                        if (splittedString.Count() > 0)
                        {
                            // Replace the beginning of the string
                            newString = newString.Replace(splittedString[0], "..");
                            newString = newString.Replace(doubleCharacterSeparator, string.Empty);

                            // Get the new size
                            stringSize = newString.MeasureTextSize(FontFamily, FontStyle, FontWeight, FontStretch, FontSize);

                            bool isTooLong = stringSize != null && stringSize.Width + Offset > ActualWidth;
                            var index = 1;
                            while (isTooLong && index < splittedString.Count() - 1)
                            {
                                // Replace only 1 occurence of the splittedString[index]
                                var regex = new Regex(Regex.Escape(splittedString[index]));
                                newString = regex.Replace(newString, string.Empty, 1);
                               
                                newString = newString.Replace(doubleCharacterSeparator, $@"{CharacterSeparator}");

                                index = index + 1;

                                stringSize = newString.MeasureTextSize(FontFamily, FontStyle, FontWeight, FontStretch, FontSize);

                                isTooLong = stringSize != null && stringSize.Width + Offset > ActualWidth;
                            }

                            int count = newString.Count(f => f == CharacterSeparator);
                            if (count == 1)
                            {
                                newString = ".." + CharacterSeparator + splittedString.Last();
                            }

                            // Control whether the result string is in range of size
                            stringSize = newString.MeasureTextSize(FontFamily, FontStyle, FontWeight, FontStretch, FontSize);

                            isTooLong = stringSize != null && stringSize.Width + Offset > ActualWidth;
                            if (isTooLong && newString.Count() > 0 && newString.Count() >= 3)
                            {
                                var intialString = newString.Substring(0, 3) + "..";
                                var numberOfCharacterToDelete = 1;
                                var tempString = newString;
                                while (isTooLong)
                                {
                                    tempString = intialString + newString.Substring(4 + numberOfCharacterToDelete, newString.Length - (4 + numberOfCharacterToDelete));
                                    stringSize = tempString.MeasureTextSize(FontFamily, FontStyle, FontWeight, FontStretch, FontSize);

                                    isTooLong = stringSize != null && stringSize.Width + Offset > ActualWidth;

                                    numberOfCharacterToDelete = numberOfCharacterToDelete + 1;
                                }

                                Text = tempString;
                            }
                            else
                            {
                                Text = newString;
                            }
                        }
                        else
                        {
                            Text = SourceText;
                        }
                    }
                    else
                    {
                        Text = SourceText;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[TrimPath] - " + ex.Message);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the Original Text in TextBlock changes
        /// </summary>
        private static void OnSourceTextChangedCallBack(
        DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var c = sender as PathTrimTextBlock;
            if (c != null)
            {
                if (c.SourceText != null)
                {
                    c.TrimPath();
                }
            }
        }

        /// <summary>
        /// Occurs when the size changes.
        /// In this event the Text of the child will be Trim if needed
        /// </summary>
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is PathTrimTextBlock textBlock)
            {
               TrimPath();
            }
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Dispose the object
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            RegisterEvents(false);
        }

        #endregion
    }
}
