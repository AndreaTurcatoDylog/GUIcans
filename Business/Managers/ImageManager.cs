using Common;
using Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Business
{
    public class ImageManager
    {
        #region Constants

        private readonly double DefaultDpiXValue = 96d;
        private readonly double DefaultDpiYValue = 96d;

        #endregion

        #region Members

        // unsafe int* px2;

        private ICommonLog _Log;

        #endregion

        #region Constructor

        public ImageManager(ICommonLog log)
        {
            _Log = log;
        }

        #endregion

        #region Methods

        /// <summary>
        ///  Create a writeable bitmap from an image loaded by path with dpiX and dpiY equals to default value
        /// </summary>
        /// <param name="pathImage">The path of image</param>
        /// <param name="pixelFormat"></param>
        /// <returns> The writeable bitmap created from the loaded TIFF image</returns>
        public WriteableBitmap CreateBufferFromTiffImage(string pathImage, PixelFormat pixelFormat, int bitsPerPixel)
        {
            return CreateBufferFromTiffImage(pathImage, DefaultDpiXValue, DefaultDpiYValue, pixelFormat, bitsPerPixel);
        }

        /// <summary>
        /// Create a writeable bitmap from a tiff image loaded by path
        /// </summary>
        /// <param name="pathImage">The path of image</param>
        /// <param name="dpiX">The horizontal dot per inch (dpi)</param>
        /// <param name="dpiY">The vertical dot per inch (dpi)</param>
        /// <param name="pixelFormat">The pixel format</param>
        /// <returns> The writeable bitmap created from the loaded TIFF image</returns>
        public WriteableBitmap CreateBufferFromTiffImage(string pathImage, double dpiX, double dpiY, PixelFormat pixelFormat, int bitsPerPixel)
        {
            WriteableBitmap writeableBitmap;
            int height;
            int width;

            try
            {
                // Doesn't read whole file, just metadata.
                using (var imageStream = File.OpenRead(pathImage))
                {
                    var decoder = BitmapDecoder.Create(imageStream, BitmapCreateOptions.IgnoreColorProfile, BitmapCacheOption.Default);
                    height = decoder.Frames[0].PixelHeight;
                    width = decoder.Frames[0].PixelWidth;
                }

                // Create the writeable bitmap
                writeableBitmap = new WriteableBitmap(width, height, dpiX, dpiY, pixelFormat, null);
                int pBuffer = (int)writeableBitmap.BackBuffer;

                writeableBitmap.Lock();

                // Get the byte per pixel
                var bytePerPixel = (bitsPerPixel % 8 == 0) ? bitsPerPixel / 8 : 1 + bitsPerPixel / 8;

                // Create buffer from tiff image
                Functions.CreateBufferFromTifImage(pathImage, pBuffer, height, width, bytePerPixel, 0);

                // Calculate the buffer size by width, height and bits per pixel
                //var stride = (width * pixelFormat.BitsPerPixel) / 8;
                var stride = (width * bitsPerPixel) / 8;
                var bufferSize = height * stride;

                // Create the result buffer
                //  byte[] byteArray = new byte[bufferSize];

                //string fileName = @"C:\Simone\byteIniziale.txt";
                //if (File.Exists(fileName))
                //{
                //    File.Delete(fileName);
                //}

                unsafe
                {
                    // Create example

                    var exampleImage = new byte[256, 256];
                    for (byte i = 0; i < 255; i++)
                    {
                        for (byte j = 0; j < 255; j++)
                        {
                            exampleImage[i, j] = j;
                        }
                    }

                    IntPtr mypointer;
                    fixed (byte* firstresult = &exampleImage[0, 0])
                    {
                        mypointer = (IntPtr)firstresult;
                    }

                    var self = (int*)pBuffer;

                    //using (StreamWriter sw = File.CreateText(fileName))
                    //{

                    //    for (int i = 0; i < bufferSize; i++)
                    //        sw.WriteLine($"element[{i}] = {pippo[i]};");
                    //}


                    var top = (IntPtr)pBuffer;

                    var simone = (IntPtr)pBuffer;

                    var myself = (IntPtr)pBuffer;

                }

                writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
                writeableBitmap.Unlock();
            }
            catch (Exception ex)
            {
                _Log.Error(ex.Message);
                writeableBitmap = null;
            }

            return writeableBitmap;
        }

        /// <summary>
        /// Return the Writeable image from a Path of image
        /// </summary>
        public WriteableBitmap GetWriteableBitmap(string pathImage)
        {
            WriteableBitmap writeableBitmap = null;

            var pixelFormats = PixelFormats.Default;
            int bitPerPixel = 0;

            try
            {
                var extension = Path.GetExtension(pathImage).ToLower();
                if (extension.Equals(".16") || extension.Equals(".tif"))
                {
                    if (extension.Equals(".16"))
                    {
                        pixelFormats = PixelFormats.Gray16;
                        bitPerPixel = 12;
                    }
                    else
                    {
                        var source = new BitmapImage(new Uri(pathImage));
                        var bitsPerPixelInf = source.Format.BitsPerPixel;

                        switch (bitsPerPixelInf)
                        {
                            case 8: pixelFormats = PixelFormats.Gray8; break;
                            case 16: pixelFormats = PixelFormats.Gray16; break;
                            case 32: pixelFormats = PixelFormats.Gray32Float; break;
                        }

                        bitPerPixel = pixelFormats.BitsPerPixel;
                    }

                    // Get the Writable bitmap
                    writeableBitmap = CreateBufferFromTiffImage(pathImage, pixelFormats, bitPerPixel);
                }
                else
                {
                    var bitmap = new BitmapImage(new Uri(pathImage, UriKind.Relative));
                    writeableBitmap = new WriteableBitmap(bitmap);
                }
            }
            catch (Exception ex)
            {
                _Log.Error(ex.Message);
                throw;
            }

            return writeableBitmap;
        }

        /// <summary>
        /// Rotate the Writable bitmap
        /// </summary>
        public WriteableBitmap Rotate(WriteableBitmap source, double angle)
        {
            WriteableBitmap writeableBitmap = source;

            unsafe
            {
                if (angle == 90)
                {
                    var width = source.PixelWidth;
                    var height = source.PixelHeight;

                    writeableBitmap = new WriteableBitmap(height, width, source.DpiX, source.DpiY, source.Format, null);

                    var sourceBuffer = source.BackBuffer;
                    var destinationBuffer = writeableBitmap.BackBuffer;

                    writeableBitmap.Lock();

                    Functions.BA_PointerAct(AGENT_ACTS.AGENT_ROT_090CW, (int)destinationBuffer, (int)sourceBuffer, height, width, 1);

                    writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, height, width));
                    writeableBitmap.Unlock();
                }
            }

            return writeableBitmap;
        }

        /// <summary>
        /// Returns a bitmap of rotate image loaded from file
        /// </summary>
        public BitmapImage Rotate(string imagePath, double angle)
        {
            try
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.UriSource = new Uri(imagePath);

                switch (angle)
                {
                    case 90: bitmapImage.Rotation = Rotation.Rotate90; break;
                    case 180: bitmapImage.Rotation = Rotation.Rotate180; break;
                    case 270: bitmapImage.Rotation = Rotation.Rotate270; break;
                }

                bitmapImage.EndInit();

                return bitmapImage;
            }
            catch(Exception ex)
            {
                _Log.Error(ex.Message);
                return null;
            }
        }

        #endregion
    }
}
