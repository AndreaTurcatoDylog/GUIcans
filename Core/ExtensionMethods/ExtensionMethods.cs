using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Size = System.Windows.Size;

namespace Core
{
    public static class ExtensionMethods
    {
        #region Extension String

        public static int CountStringOccurrences(this string text, string pattern)
        {
            // Loop through all instances of the string 'text'.
            int count = 0;
            int i = 0;
            while ((i = text.IndexOf(pattern, i)) != -1)
            {
                i += pattern.Length;
                count++;
            }
            return count;
        }

        /// <summary>
        /// Replace the character at index with new character
        /// </summary>
        public static string ReplaceAtIndex(this string word, int index, char newCharacter)
        {
            char[] letters = word.ToCharArray();
            letters[index] = newCharacter;
            return string.Join("", letters);
        }

        #endregion

        #region Extension DateTime

        /// <summary>
        /// Change the Time of a DateTime
        /// </summary>
        /// <example>
        /// DateTime dateTime = DateTime.Now;
        /// dateTime = dateTime.ChangeTime(10,10,10,0);
        /// </example>
        public static DateTime ChangeTime(this DateTime dateTime, int hours, int minutes, int seconds, int milliseconds)
        {
            return new DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                hours,
                minutes,
                seconds,
                milliseconds,
                dateTime.Kind);
        }

        #endregion

        #region Extension DirectoryInfo

        public static IEnumerable<FileInfo> GetFilesByExtensions(this DirectoryInfo dirInfo, params string[] extensions)
        {
            var allowedExtensions = new HashSet<string>(extensions, StringComparer.OrdinalIgnoreCase);

            return dirInfo.EnumerateFiles()
                          .Where(f => allowedExtensions.Contains(f.Extension));
        }

        #endregion

        #region Extension Bitmap 

        /// <summary>
        /// Convert the Bitmap in a BitmapImage
        /// </summary>
        public static BitmapImage ToBitmapImage(this Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }

        /// <summary>
        /// Save the BitmapImage into the specificated path
        /// </summary>
        public static void Save(this BitmapImage image, string filePath)
        {
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));

            using (var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
            {
                encoder.Save(fileStream);
            }
        }

        public static System.Drawing.Bitmap BitmapFromSource(this BitmapSource bitmapsource)
        {
            //convert image format
            var src = new System.Windows.Media.Imaging.FormatConvertedBitmap();
            src.BeginInit();
            src.Source = bitmapsource;
            src.DestinationFormat = System.Windows.Media.PixelFormats.Bgra32;
            src.EndInit();

            //copy to bitmap
            Bitmap bitmap = new Bitmap(src.PixelWidth, src.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var data = bitmap.LockBits(new Rectangle(System.Drawing.Point.Empty, bitmap.Size), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            src.CopyPixels(System.Windows.Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
            bitmap.UnlockBits(data);

            return bitmap;
        }

        #endregion

        #region  Extension MouseButtonEventArgs

        /// <summary>
        /// Prevent to call the button click after a touch down.
        /// Used in a mouse click\down event
        /// </summary>
        public static bool CalledAfterTouchDown(this MouseButtonEventArgs mouseButtonEventArgs)
        {
            var stylus = mouseButtonEventArgs.StylusDevice?.StylusButtons;
            if (stylus != null && stylus.Count() > 0)
            {
                var buttonState = stylus[0].StylusButtonState;
                return buttonState == StylusButtonState.Down;
            }

            return false;
        }

        #endregion

        #region Extension XAML 

        /// <summary>
        /// Returns the size of Text in XAML depending on font, size and stretch and style
        /// </summary>
        public static Size MeasureTextSize(this string text, System.Windows.Media.FontFamily fontFamily, System.Windows.FontStyle fontStyle, FontWeight fontWeight, FontStretch fontStretch, double fontSize)
        {
            FormattedText ft = new FormattedText(text,
                                                 CultureInfo.CurrentCulture,
                                                 FlowDirection.LeftToRight,
                                                 new Typeface(fontFamily, fontStyle, fontWeight, fontStretch),
                                                 fontSize,
                                                 System.Windows.Media.Brushes.Black);

            return new System.Windows.Size(ft.Width, ft.Height);
        }

        /// <summary>
        /// Return the Placement of object in the screen
        /// </summary>
        public static Rect GetAbsolutePlacement(this FrameworkElement element, bool relativeToScreen = false)
        {
            try
            {
                var absolutePos = element.PointToScreen(new System.Windows.Point(0, 0));
                if (relativeToScreen)
                {
                    return new Rect(absolutePos.X, absolutePos.Y, element.ActualWidth, element.ActualHeight);
                }

                var posMW = Application.Current.MainWindow.PointToScreen(new System.Windows.Point(0, 0));
                absolutePos = new System.Windows.Point(absolutePos.X - posMW.X, absolutePos.Y - posMW.Y);
                return new Rect(absolutePos.X, absolutePos.Y, element.ActualWidth, element.ActualHeight);
            }
            catch (Exception ex)
            {
                CoreLog.Instance.Append($"[{ex.Source}] - {ex.Message} - {ex.StackTrace}", CoreLogType.Error);
            }

            return new Rect(0, 0, element.ActualWidth, element.ActualHeight);
        }

        /// <summary>
        /// Get the parent Adjustable Framework Element of specificated obbject
        /// </summary>
        public static IAdjustableFrameworkElement GetParentAdjustableFrameworkElement(this DependencyObject child)
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null)
            {
                return null;
            }

            var parent = parentObject as IAdjustableFrameworkElement;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                return GetParentAdjustableFrameworkElement(parentObject);
            }
        }

        /// <summary>
        /// Find the components of specificated type
        /// </summary>
        /// <example>
        /// FindVisualChildren<TextBlock>(window))
        /// </example>
        public static IEnumerable<T> FindVisualChildren<T>(this FrameworkElement frameworkElement, DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(frameworkElement, child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        /// <summary>
        /// Returns a list of all IDisposable elements inside the window
        /// </summary>
        public static IEnumerable<IDisposable> FindIDisposableVisualChildren(this FrameworkElement frameworkElement, DependencyObject depObj)
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is IDisposable)
                    {
                        yield return (IDisposable)child;
                    }

                    foreach (var childOfChild in FindIDisposableVisualChildren(frameworkElement, child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        /// <summary>
        /// Find all  components in a Framework element like window or user control
        /// </summary>
        /// <example>
        /// FindVisualChildren(window))
        /// </example>
        public static IEnumerable<FrameworkElement> FindVisualChildren(this FrameworkElement frameworkElement, DependencyObject depObj)
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is FrameworkElement)
                    {
                        yield return (FrameworkElement)child;
                    }

                    foreach (var childOfChild in FindVisualChildren(frameworkElement, child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        #endregion

        #region Extension IntPtr

        /// <summary>
        /// Returns the pointer of an object
        /// </summary>
        public static IntPtr ToIntPtr(this object target)
        {
            return GCHandle.Alloc(target).ToIntPtr();
        }

        /// <summary>
        /// Returns the GCHandle of an object
        /// </summary>
        public static GCHandle ToGcHandle(this object target)
        {
            return GCHandle.Alloc(target);
        }

        /// <summary>
        /// Returns the IntPtr of a GCHandle
        /// </summary>
        public static IntPtr ToIntPtr(this GCHandle target)
        {
            return GCHandle.ToIntPtr(target);
        }

        #endregion

        #region Extensions Linq

        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> source, int length)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException("length");

            var section = new List<T>(length);

            foreach (var item in source)
            {
                section.Add(item);

                if (section.Count == length)
                {
                    yield return section.AsReadOnly();
                    section = new List<T>(length);
                }
            }

            if (section.Count > 0)
                yield return section.AsReadOnly();
        }

        #endregion
    }
}
