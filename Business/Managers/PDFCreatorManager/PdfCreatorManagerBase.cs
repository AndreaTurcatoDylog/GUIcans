using Common;
using Core;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public abstract class PdfManagerCreatorBase : IPdfCreatorManager
    {
        #region Constants

        protected readonly string AuthorName = "Dylog";

        #endregion

        #region Members

        protected ICommonLog _Log;

        protected XFont _FontRegular;
        protected XFont _FontBold;

        protected XFont _LittleFontRegular;
        protected XFont _LittleFontBold;

        protected XFont _BigFontBold;

        protected IList<XImage> _XImages;
        protected IList<MemoryStream> _MemoryStreams;

        protected int _TotalReportNumberOfPage;

        #endregion

        #region Event Handlers   

        public event EventHandler<string> ReportCreationStarted;
        public event EventHandler<string> ReportCreated;
        public event EventHandler<int> MaxProgressValueChanged;

        #endregion

        #region Constructor

        public PdfManagerCreatorBase(ICommonLog log)
        {
            _Log = log;

            _FontRegular = new XFont("Verdana", 10, XFontStyle.Regular);
            _FontBold = new XFont("Verdana", 10, XFontStyle.Bold);

            _LittleFontBold = new XFont("Verdana", 8, XFontStyle.Bold);
            _LittleFontRegular = new XFont("Verdana", 8, XFontStyle.Regular);

            _BigFontBold = new XFont("Verdana", 15, XFontStyle.Bold);

            _XImages = new List<XImage>();
            _MemoryStreams = new List<MemoryStream>();
        }

        #endregion

        #region Abstract Methods

        abstract public void CreateReport(string pathFileName, string logoPath, ExtendedBackgroundWorker backgroundWorker = null);

        #endregion

        #region Methods

        /// <summary>
        /// Draw the text in page.
        /// The rectangle is created with X, Y, Width and Height dimensions that specify the max dimesion of the text area.
        /// </summary>
        protected int DrawText(string text, double x, double y, double width, double height, XFont font, XGraphics gfx, XBrush brush = null)
        {
            var maxWidth = width - 5;

            if (text == null)
            {
                text = string.Empty;
                return -1;
            }

            // Set the Default brush
            if (brush == null)
            {
                brush = XBrushes.Black;
            }

            XTextFormatter tf = new XTextFormatter(gfx);
            XRect rect = new XRect(x, y, width, height);
           
            //var textWidth = gfx.MeasureString(text + '\u2026', font).Width;
            //if (textWidth> maxWidth)
            //{
            //    // Truncate the string
            //    int stringlength = text.Length;
            //    var index = 1;
            //    var pippo = gfx.MeasureString(text.Substring(0, stringlength) + '\u2026', font).Width;
            //    while (gfx.MeasureString(text.Substring(0, stringlength) + '\u2026', font).Width > maxWidth)
            //    {
            //        pippo = gfx.MeasureString(text.Substring(0, stringlength) + '\u2026', font).Width;

            //        stringlength -= (int)Math.Ceiling(text.Length * Math.Pow(0.5f, index));
            //       // stringlength -= (int)Math.Ceiling(text.Length * Math.Pow(0.5f, index));
            //        index++;
            //    }

               

            //    if (stringlength - 5 >=0)
            //    {
            //        stringlength = stringlength - 5;
            //    }
            //    text = text.Substring(0, stringlength) + "...";

            //}

            gfx.DrawRectangle(XBrushes.Transparent, rect);

            return tf.DrawString(text, font, brush, rect, XStringFormats.TopLeft);
        }

        /// <summary>
        /// Draw a text with a rectangle around
        /// </summary>
        protected int DrawBorderText(string text, double x, double y, double width, double height, XFont font, XGraphics gfx)
        {
            // Calculate the Height of the Text with the specificated WIDTH and FONT                 
            XTextFormatter tf = new XTextFormatter(gfx);          
            var rect_1 = new XRect(x + 10, y, width - 60, height);
            var absoluteTextHeight = tf.GetDrawStringHeight(text, font, rect_1);

            // Draw Rectangle    
            var rect = new XRect(x, y - 4, width, absoluteTextHeight + 20);                       
            gfx.DrawRectangle(XPens.Black, XBrushes.Transparent, rect);

            // Draw Text
            int residualidx = -3;
            residualidx = DrawText(text, x + 10, y, width - 10, height, font, gfx);

            return residualidx;
        }
       
        /// <summary>
        /// Clear the Memory for one single Page
        /// </summary>
        public void ClearMemory()
        {
            foreach (var xImage in _XImages)
            {
                xImage?.Dispose();
            }

            foreach (var memoryStream in _MemoryStreams)
            {
                memoryStream?.Close();
                memoryStream?.Dispose();
            }

            _XImages?.Clear();
            _MemoryStreams?.Clear();
        }

        /// <summary>
        /// Draw the single image
        /// </summary>
        protected void DrawProductImage(string pathImage, double x, double y, double width, double height, double rotate, XGraphics gfx)
        {
            try
            {
                // Draw the Border for the image. This space is the max space for the image
                var pen = new XPen(XColors.Black, 1);

                var rect = new XRect(x, y - 7, width + 30, height + 14);
                gfx.DrawRectangle(XBrushes.Transparent, rect);
                gfx.DrawRectangle(pen, rect);

                // Load the image from file
                var img = Image.FromFile(pathImage);

                // Rotate the image if needed
                if (rotate > 0)
                {
                    switch (rotate)
                    {
                        case 90: img.RotateFlip(RotateFlipType.Rotate90FlipNone); break;
                    }
                }

                // Save the image in a stream
                var stream = new System.IO.MemoryStream();               

                img.Save(stream, ImageFormat.Tiff);
                img.Dispose();
                stream.Position = 0;

                // Load the image in pdf image
                var pdfImage = XImage.FromStream(stream);
                _XImages.Add(pdfImage);
                _MemoryStreams.Add(stream);

                // Resize the image in proportinal way
                double widthImage = 0;
                double heightImage = 0;

                if (pdfImage.Size.Height > pdfImage.Size.Width)
                {
                    heightImage = rect.Height - 10;
                    widthImage = (pdfImage.Size.Width / pdfImage.Size.Height) * heightImage;
                    if (widthImage > rect.Width)
                    {
                        widthImage = rect.Width - 10;
                        heightImage = (pdfImage.Size.Height / pdfImage.Size.Width) * rect.Width;
                    }
                }
                else if (pdfImage.Size.Width == pdfImage.Size.Height)
                {
                    widthImage = rect.Width - 10;
                    heightImage = widthImage;
                }
                else
                {
                    widthImage = rect.Width - 10;
                    heightImage = (pdfImage.Size.Height / pdfImage.Size.Width) * rect.Width;
                    if (heightImage > rect.Height)
                    {
                        heightImage = rect.Height - 10;
                        widthImage = (pdfImage.Size.Width / pdfImage.Size.Height) * heightImage;
                    }
                }

                // Center the image in current Border
                // Calculate the image Left
                var offsetBorderX = (width + 14) / 2;
                var offestImageX = widthImage / 2;

                var offesetX = offsetBorderX - offestImageX;
                var imageLeft = x + 3 + offesetX;

                // Calculate the image Top
                var offsetBorderY = (height + 14) / 2;
                var offestImageY = heightImage / 2;

                var offesetY = offsetBorderY - offestImageY;
                var imageTop = y - 7 + offesetY;

                // Draw the image
                gfx.DrawImage(pdfImage, imageLeft, imageTop, widthImage, heightImage);
            }
            catch (Exception ex)
            {
                _Log.Error($"{ex.Message}  - {ex.StackTrace} - {ex.Source}");
            }
        }

        /// <summary>
        ///  Set the BackGroundWorker.
        ///  Set the Max (Trigger event)  and increment value.         
        /// </summary>
        /// <returns> Value 1: Increment value
        ///           Value 2: MaxValue
        /// </returns>
        protected (int, int) SetBackGroundWorker(IEnumerable<string> images, int numberOfImagesInOnePage, int incrementPage)
        {
            // Calculate the Background worker increment
            var numberOfImages = images.Count();
            var existImages = images.Count() > 0;

            int backGroundWorkerIncrement = 0;
            var numberOfPages = (numberOfImages / numberOfImagesInOnePage);

            // Increment the page of the specificated value
            numberOfPages = numberOfPages + incrementPage;

            if (numberOfPages < 100)
            {
                numberOfPages = numberOfPages * 100;
            }

            backGroundWorkerIncrement = (numberOfPages / 100);

            // Trigger the max changed event
            TriggerMaxProgressValueChanged(this, numberOfPages);

            return (backGroundWorkerIncrement, numberOfImages);
        }

        /// Return a valid File Name Path .
        /// if file with that name already exists the File Name will be changed (a number index will be added to the end of name) 
        protected string GetValidFileNamePath(string fileNamePath)
        {
            var result = fileNamePath;

            var path = Path.GetDirectoryName(fileNamePath);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileNamePath);

            int index = 0;
            while (File.Exists(result))
            {
                index = index + 1;
                result = $@"{path}\{fileNameWithoutExtension}_(COPY {index}).pdf";
            }

            return result;
        }

        /// <summary>
        /// Trigger the Report Creation Started Event
        /// </summary>
        protected void TriggerReportCreationStarted(object sender, string reportPath)
        {
            ReportCreationStarted?.Invoke(this, reportPath);
        }

        /// <summary>
        /// Trigger the Report Created Event
        /// </summary>
        protected void TriggerReportCreated(object sender, string reportPath)
        {
            ReportCreated?.Invoke(this, reportPath);
        }

        /// <summary>
        /// Trigger the Max Progress Value Changed Event
        /// </summary>
        protected void TriggerMaxProgressValueChanged(object sender, int maxValue)
        {
            MaxProgressValueChanged?.Invoke(this, maxValue);
        }

        #endregion

    }
}
