using Common;
using Core;
using Core.ResourceManager.Cultures;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Business
{
    /// <summary>
    /// Manage the creation of PDF report
    /// </summary>
    public class PdfCreatorWithBatchContentManager: PdfManagerCreatorBase
    {
        #region Constants

        private readonly int MaxBachLinesInMainPage = 56;
        private readonly int MaxBachLinesInOnePage = 62;

        #endregion

        #region Members

        private double _ImageWidth;
        private double _ImageHeight;

        private IList<string> _ImagesPath;
        private int _TotalPages;
        private int _NumberOfImagesInOnePage;

        private string _Title;
        private string _Subtitle;
        private string _Subtitle_2;
        public string _Lot;
        public string _Article;

        private double _AngleRotatation;
        private string[] _BatchFileContent;

        #endregion

        #region Constructor

        public PdfCreatorWithBatchContentManager(ICommonLog log, string[] batchFileContent, IList<string> imagesPath, string title, string subtitle, 
                                 string subtitle_2, string lot, string article,  int numberOfImagesInOnePage, double angleRotation = 0)
             : base(log)
        {
            _Log = log;
            _BatchFileContent = batchFileContent;
            _Lot = lot;
            _Article = article;
            _Title = title;
            _Subtitle = subtitle;
            _Subtitle_2 = subtitle_2;
            _ImagesPath = imagesPath;
            _NumberOfImagesInOnePage = numberOfImagesInOnePage;
            _AngleRotatation = angleRotation;

            Initialize(_NumberOfImagesInOnePage);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initiatialize properties
        /// </summary>
        private void Initialize(int numberOfImageInPage)
        {
            switch (numberOfImageInPage)
            {
                case 2:
                    _ImageWidth = 220;
                    _ImageHeight = 300;
                    break;
                case 4:
                    _ImageWidth = 213;
                    _ImageHeight = 253;
                    break;
                case 6:
                    _ImageWidth = 140;
                    _ImageHeight = 180;
                    break;
                default:
                    _NumberOfImagesInOnePage = 6;
                    _ImageWidth = 140;
                    _ImageHeight = 180;
                    break;
            }
        }

        /// <summary>
        /// Create the report
        /// </summary>
        public override void CreateReport(string pathFileName, string logoPath, ExtendedBackgroundWorker backgroundWorker = null)
        {
            PdfDocument pdfDocument = null;
            IEnumerable<IEnumerable<string>> pdfFiles = null;
            var destinationPath = pathFileName;

            // Calculate the number of PDF files
            if (_ImagesPath != null && _ImagesPath.Any())
            {
                pdfFiles = _ImagesPath.Split(1000);
            }

            if (_NumberOfImagesInOnePage > 0)
            {
                int currentPage = 1;
                _TotalPages = 0;

                // Calculate the number of pages
                if (_ImagesPath != null && _ImagesPath.Any())
                {
                    _TotalPages = _ImagesPath.Count / _NumberOfImagesInOnePage;
                    if (_ImagesPath.Count % _NumberOfImagesInOnePage > 0)
                    {
                        _TotalPages = _TotalPages + 1;
                    }
                }
                else
                {
                    _TotalPages = 1;
                }

                try
                {
                    // Calculate the number of batch pages
                    var numberOfContentPage = 0;
                    if (_BatchFileContent != null && _BatchFileContent.Length > 0)
                    {
                        numberOfContentPage = _BatchFileContent.Length / MaxBachLinesInOnePage;
                        if (_BatchFileContent.Length % MaxBachLinesInOnePage > 0)
                        {
                            numberOfContentPage++;
                        }

                        _TotalPages = _TotalPages + numberOfContentPage;
                    }

                    if (pdfFiles != null && pdfFiles.Any())
                    {
                        var indexPDFPart = 1;

                        foreach (var pdfFile in pdfFiles)
                        {
                            var incrementPageValue = 0;
                            if (indexPDFPart == 1)
                            {
                                incrementPageValue = 2;
                            }

                            // Set the Background worker and get the max and the increment values
                            var values = SetBackGroundWorker(pdfFile, _NumberOfImagesInOnePage, incrementPageValue);
                            var backGroundWorkerIncrement = values.Item1;
                            var backGroundWorkerMaxValue = values.Item2;

                            // Reset the BackgroundWorker
                            backgroundWorker?.Reset();

                            // Create the destination PartPDFPath
                            var destinationPartPDFPath = destinationPath;
                            if (pdfFiles.Count() > 1)
                            {
                                var tempFolderPath = Path.GetDirectoryName(destinationPath);
                                var tempFileName = Path.GetFileNameWithoutExtension(destinationPath) + $"_PART_{ indexPDFPart}.pdf";
                                destinationPartPDFPath = Path.Combine(tempFolderPath, tempFileName);
                            }

                            /// if file with that name already exists 
                            /// the File Name will be changed (a number index will be added to the end of name) 
                            destinationPartPDFPath = GetValidFileNamePath(destinationPartPDFPath);

                            // Trigger the Creation Report Started Event
                            TriggerReportCreationStarted(this, destinationPartPDFPath);

                            // Create the document
                            pdfDocument = new PdfDocument(_Title, string.Empty, AuthorName, AuthorName);

                            PdfPage page = null;
                            XGraphics gfx = null;
                            var bottom = 20;

                            // Create and Draw the Main Page
                            if (indexPDFPart == 1 && numberOfContentPage > 0)
                            {
                                page = pdfDocument.AddPage(PageSize.A4, PageOrientation.Portrait);
                                gfx = XGraphics.FromPdfPage(page);

                                // Draw the Main Page
                                DrawMainPage(page, gfx, bottom, logoPath);

                                if (numberOfContentPage > 0)
                                {
                                    bottom = bottom + 57;
                                    DrawBatchPage(page, currentPage, gfx, bottom);
                                }

                                // Update the background worker
                                backgroundWorker?.ReportIncrementProgressWithExeption(backGroundWorkerIncrement);

                                // Drawn the Batch pages                          
                                for (var contentPageIndex = 2; contentPageIndex <= numberOfContentPage; contentPageIndex++)
                                {
                                    var skip = contentPageIndex * MaxBachLinesInOnePage;
                                    var newArray = _BatchFileContent.Skip(skip).Take(MaxBachLinesInOnePage - 1).ToArray();
                                    var text = String.Join(Environment.NewLine, newArray);

                                    // Dispose the previuse GFX
                                    gfx?.Dispose();

                                    page = pdfDocument.AddPage();
                                    gfx = XGraphics.FromPdfPage(page);
                                    DrawBatchPage(page, contentPageIndex, gfx, 20);
                                }

                                currentPage = currentPage + numberOfContentPage;

                                // Update the background worker
                                backgroundWorker?.ReportIncrementProgressWithExeption(backGroundWorkerIncrement);
                            }

                            if (pdfFile != null && pdfFile.Count() > 0)
                            {
                                int index = 0;
                                while (index < pdfFile.Count())
                                {
                                    double imagePageBottom = 0;
                                    if (numberOfContentPage > 0 || index > 0)
                                    {
                                        page = pdfDocument.AddPage();
                                        imagePageBottom = 20;
                                    }
                                    else
                                    {
                                        imagePageBottom = 80;
                                    }

                                    // Draw Image Page
                                    DrawImagePage(page, _AngleRotatation, index, currentPage, imagePageBottom, pdfFile.ToList());

                                    index = index + _NumberOfImagesInOnePage;
                                    currentPage = currentPage + 1;

                                    // Update the background worker
                                    backgroundWorker?.ReportIncrementProgressWithExeption(backGroundWorkerIncrement);
                                }
                            }                            

                            // Save the Report                       
                            pdfDocument.Save(destinationPartPDFPath);

                            // Trigger the Report Created Event
                            TriggerReportCreated(this, destinationPartPDFPath);

                            indexPDFPart = indexPDFPart + 1;

                            // Update the background worker
                            if (backgroundWorker != null && !backgroundWorker.CancellationPending)
                            {
                                backgroundWorker?.ReportProgressWithExeption(backGroundWorkerMaxValue);
                            }
                            else
                            {
                                backgroundWorker?.ReportIncrementProgressWithExeption(backGroundWorkerIncrement);
                            }

                            // Dispose the document and GFX
                            pdfDocument?.Dispose();
                            gfx?.Dispose();
                        }
                    }
                    else
                    {
                        DrawEmptyReport(destinationPath, logoPath, numberOfContentPage, backgroundWorker);
                    }
                }
                catch (Exception ex)
                {
                    _Log.Error($"{ex.Message}  - {ex.StackTrace} - {ex.Source}");
                    throw ex;
                }
                finally
                {
                    pdfDocument?.Close();
                    pdfDocument?.Dispose();
                }
            }
        }

        /// <summary>
        /// Draw an Empty Report
        /// </summary>
        private void DrawEmptyReport(string destinationPath,  string logoPath, int numberOfContentPage, ExtendedBackgroundWorker backgroundWorker = null)
        {
            PdfDocument pdfDocument = null;
            XGraphics gfx = null;

            try
            {
                // Create the document
                pdfDocument = new PdfDocument(_Title, string.Empty, AuthorName, AuthorName);

                var titleHeight = 30;
                var font = new XFont("SansSerifFLF", 10, XFontStyle.Regular);
                var message = CultureResources.GetString("Message_Images_Not_Found");

                var page = pdfDocument.AddPage();
                gfx = XGraphics.FromPdfPage(page);

                // Reset the BackgroundWorker
                backgroundWorker?.Reset();

                // Trigger the Set of Max value of Progress Event
                TriggerMaxProgressValueChanged(this, 100);

                /// if file with that name already exists 
                /// the File Name will be changed (a number index will be added to the end of name) 
                destinationPath = GetValidFileNamePath(destinationPath);               

                // Trigger the Creation Report Started Event
                TriggerReportCreationStarted(this, destinationPath);

                double bottom = 20;
                // Draw the main page
                DrawMainPage(page, gfx, bottom, logoPath);

                if (numberOfContentPage > 0)
                {
                    bottom = bottom + 57;
                    DrawBatchPage(page, 1, gfx, bottom);
                }

                // Drawn the Batch pages                          
                for (var contentPageIndex = 2; contentPageIndex <= numberOfContentPage; contentPageIndex++)
                {
                    var skip = contentPageIndex * MaxBachLinesInOnePage;
                    var newArray = _BatchFileContent.Skip(skip).Take(MaxBachLinesInOnePage - 1).ToArray();
                    var text = String.Join(Environment.NewLine, newArray);

                    // Dispose the previuse GFX
                    gfx?.Dispose();

                    page = pdfDocument.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    DrawBatchPage(page, contentPageIndex, gfx, 20);                  
                }

                var currentPage = 1 + numberOfContentPage;

                gfx?.Dispose();

                // Add a new page
                page = pdfDocument.AddPage();
                gfx = XGraphics.FromPdfPage(page);

                bottom = 20;

                // Draw the Report Informations
                double marginLeft = 45;
                double marginRight = page.Width - marginLeft;
                DrawReportInformations(_Subtitle, marginLeft, marginRight, titleHeight, ref bottom, currentPage, page, gfx);

                // Draw the Message
                bottom = bottom + 30;
                var x = (page.Width / 2) - (message.Count());
                DrawText(message, x, bottom, page.Width, titleHeight, font, gfx);

                // Save the Report                       
                pdfDocument.Save(destinationPath);

                // Trigger the Report Created Event
                TriggerReportCreated(this, destinationPath);

                // Update the background worker
                if (backgroundWorker != null && !backgroundWorker.CancellationPending)
                {
                    backgroundWorker?.ReportProgressWithExeption(100);
                }                
            }
            catch (Exception ex)
            {
                _Log.Error($"{ex.Message}  - {ex.StackTrace} - {ex.Source}");
                throw ex;
            }
            finally
            {
                gfx?.Dispose();

                pdfDocument?.Close();
                pdfDocument?.Dispose();
            }
        }

        /// <summary>
        /// Draw the Main Header
        /// </summary>
        private void DrawMainHeader(double marginLeft, double marginRight, ref double bottom, string logoPath, PdfPage page, XGraphics gfx)
        {
            // Draw Title
            var titleHeight = 37;
            var font = new XFont("Verdana", 15, XFontStyle.Bold);
            DrawText(_Title, marginLeft, bottom, page.Width / 2, titleHeight, font, gfx);

            // Draw Logo
            if (File.Exists(logoPath))
            {
                XImage image = XImage.FromFile(logoPath);

                var logoHeight = 40;
                var logoWidth = (image.Size.Width / image.Size.Height) * logoHeight;

                var imageX = marginRight - logoWidth;
                gfx.DrawImage(image, imageX, bottom - 5, logoWidth, logoHeight);
            }

            // Separator
            bottom = bottom + titleHeight + 5;
            gfx.DrawLine(XPens.DarkGreen, marginLeft, bottom, marginRight + 30, bottom);
        }

        /// <summary>
        /// Draw the Report Informations
        /// </summary>
        private void DrawReportInformations(string text, double marginLeft, double marginRight, double titleHeight, ref double bottom, int currentPageIndex, PdfPage page, XGraphics gfx)
        {            
            // Draw Subtitle
            var rect = new XRect(marginLeft, bottom - 8, page.Width / 3, titleHeight);
            gfx.DrawRectangle(XPens.Black, XBrushes.SugarPaper, rect);

            DrawText(text, marginLeft + 2, bottom - 8, page.Width / 3, titleHeight, _FontBold, gfx, XBrushes.White);

            // Draw Informations
            var informationX = marginRight - 280;

            // Current page and Number of page
            text = $"{currentPageIndex} / {_TotalPages}";
            DrawText(text, informationX + 240, bottom -10, page.Width / 3, titleHeight, _FontBold, gfx);

            // Lot
            text = CultureResources.GetString("Label_Number_Lot");
            DrawText(text, informationX, bottom - 10, page.Width / 3, titleHeight, _FontBold, gfx);

            text = _Lot;
            DrawText(text, informationX + 75, bottom - 10, page.Width / 3, titleHeight, _FontRegular, gfx);

            // Article
            text = CultureResources.GetString("Label_Number_Article");
            DrawText(text, informationX, bottom + 3, page.Width / 2, titleHeight, _FontBold, gfx);

            text = _Article;
            DrawText(text, informationX + 75, bottom + 3, page.Width / 2, titleHeight, _FontRegular, gfx);

            // Number of images
            text = CultureResources.GetString("Label_Number_Of_Images");
            DrawText(text, informationX, bottom + 16, page.Width / 3, titleHeight, _FontBold, gfx);

            text = _ImagesPath.Count().ToString();
            DrawText(text, informationX + 75, bottom + 16, page.Width / 3, titleHeight, _FontRegular, gfx);

            // Separator
            bottom = bottom + 37;
            gfx.DrawLine(XPens.DarkGreen, marginLeft, bottom, marginRight + 30, bottom);
        }

        /// <summary>
        /// Draw the Content of the BatchFile
        /// </summary>
        private void DrawBatchFileContent(string text, double marginLeft, double marginRight, double contentHeight, ref double bottom, PdfPage page, XGraphics gfx)
        {
            bottom = bottom + 15;
           
            // Draw Text
            DrawText(text, marginLeft + 2, bottom - 8, page.Width - 10, contentHeight, _FontRegular, gfx);

            // Separator
            gfx.DrawLine(XPens.DarkGreen, marginLeft, contentHeight - 10, marginRight + 30, contentHeight - 10);
        }

        /// <summary>
        /// Draw the Page
        /// </summary>
        private void DrawBatchPage(PdfPage page, int currentPage, XGraphics gfx, double bottom)
        {
            // Set properties
            double marginLeft = 45;
            double marginRight = page.Width - marginLeft;
            double titleHeight = 37;

            // Draw the Report Informations
            DrawReportInformations(_Subtitle, marginLeft, marginRight, titleHeight, ref bottom, currentPage, page, gfx);

            if (_BatchFileContent != null && _BatchFileContent.Length > 0)
            {
                // Draw the Batch content
                var skip = 0;
                var take = 0;
                if (currentPage == 1)
                {
                    take = MaxBachLinesInMainPage;
                }
                else
                {
                    skip = ((currentPage - 1) * MaxBachLinesInOnePage) - (MaxBachLinesInOnePage - MaxBachLinesInMainPage);
                    take = MaxBachLinesInOnePage;
                }

                var newArray = _BatchFileContent.Skip(skip).Take(take).ToArray();

                var text = String.Join(Environment.NewLine, newArray);
                DrawBatchFileContent(text, marginLeft, marginRight, page.Height, ref bottom, page, gfx);
            }
        }

        /// <summary>
        /// Draw the Main Page
        /// </summary>
        private void DrawMainPage(PdfPage page, XGraphics gfx, double bottom, string logoPath)
        {
            // Set properties
            double marginLeft = 45;
            double marginRight = page.Width - marginLeft;
            //double titleHeight = 37;

            // Draw the Header
            DrawMainHeader(marginLeft, marginRight, ref bottom, logoPath, page, gfx);
        }

        /// <summary>
        /// Draw the Page
        /// </summary>
        private void DrawImagePage(PdfPage page, double rotate, int firstImageIndex, int currentPage,  double bottom, IList<string> imagesPath)
        {
            var gfx = XGraphics.FromPdfPage(page);

            // Set properties
            double marginLeft = 45;
            double marginRight = page.Width - marginLeft;
           // double bottom = 20;
            double titleHeight = 37;

            // Draw the Report Informations
            DrawReportInformations(_Subtitle, marginLeft, marginRight, titleHeight, ref bottom, currentPage, page, gfx);

            #region Draw Images

            // Set font and number of images
            var fontBold = new XFont("Verdana", 8, XFontStyle.Bold);
            var font = new XFont("Verdana", 8, XFontStyle.Regular);

            int max = _NumberOfImagesInOnePage;
            if (imagesPath.Count < firstImageIndex + max)
            {
                max = imagesPath.Count - firstImageIndex;
            }

            // Get the sublist from original list.
            // Get a number of elements equals to the number of image that must be shown in page
            var sublist = imagesPath.ToList().GetRange(firstImageIndex, max);
            var index = 0;
            while (index < max)
            {
                var currentFilePath = sublist.ElementAt(index);

                bottom = bottom + 8;
                var rightImageBottom = bottom;

                #region Image in the Left side

                var textMargin = marginLeft + 3;

                // Draw image file Name
                var fileName = Path.GetFileName(currentFilePath);
                DrawText(fileName, textMargin, bottom, page.Width / 2, 30, fontBold, gfx);

                bottom = bottom + 10;

                // Draw image Reject date time
                var creationDateTime = File.GetCreationTime(currentFilePath);
                DrawText(creationDateTime.ToString(), textMargin, bottom, page.Width / 2, 30, fontBold, gfx);

                bottom = bottom + 25;

                // Draw the image
                DrawProductImage(currentFilePath, marginLeft, bottom, _ImageWidth, _ImageHeight, rotate, gfx);

                #endregion

                #region Image in the Rigth side

                index = index + 1;
                if (index < max)
                {
                    currentFilePath = sublist.ElementAt(index);

                    textMargin = marginLeft + (page.Width / 2);

                    // Draw image file Name
                    fileName = Path.GetFileName(currentFilePath);
                    DrawText(fileName, textMargin + 2, rightImageBottom, page.Width / 2, 30, fontBold, gfx);

                    rightImageBottom = rightImageBottom + 10;

                    // Draw image Reject date time
                    creationDateTime = File.GetCreationTime(currentFilePath);
                    DrawText(creationDateTime.ToString(), textMargin + 2, rightImageBottom, page.Width / 2, 30, fontBold, gfx);

                    rightImageBottom = rightImageBottom + 25;

                    // Draw the image
                    DrawProductImage(currentFilePath, textMargin, rightImageBottom, _ImageWidth, _ImageHeight, rotate, gfx);
                }

                #endregion

                // Horizontal Separator
                bottom = bottom + _ImageHeight + 20;
                gfx.DrawLine(XPens.DarkGreen, marginLeft, bottom, marginRight + 30, bottom);

                index++;
            }

            ClearMemory();

            #endregion
        }

        #endregion
    }
}

