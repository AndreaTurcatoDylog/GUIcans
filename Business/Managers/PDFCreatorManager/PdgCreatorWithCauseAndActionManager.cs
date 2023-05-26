using Common;
using Core;
using Core.ResourceManager.Cultures;
using Models;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace Business
{
    public class PdgCreatorWithCauseAndActionManager : PdfManagerCreatorBase
    {
        #region Members

        private double _ImageWidth;
        private double _ImageHeight;

        private IList<string> _ImagesPath;
        private int _NumberOfImagesInOnePage;

        private string _Title;
        private string _Subtitle;
        public string _Lot;
        public string _Article;

        private double _AngleRotatation;

        private Dictionary<ReportAddedContentType, string> _ReportAddedContents;

        private string _AdditionalContent;
        private string _CommentsContent;

        private FileManager _FileManager;

        #endregion

        #region Constructor

        public PdgCreatorWithCauseAndActionManager(ICommonLog log, Dictionary<ReportAddedContentType, string> reportAddedContents, IList<string> imagesPath, string title, string subtitle,
                                 string lot, string article, int numberOfImagesInOnePage, double angleRotation = 0)
            : base(log)
        {
            _FileManager = new FileManager(log);

            _Log = log;
            _ReportAddedContents = reportAddedContents;
            _AdditionalContent = reportAddedContents[ReportAddedContentType.Additional];
            _CommentsContent = reportAddedContents[ReportAddedContentType.Comments];

            _Lot = lot;
            _Article = article;
            _Title = title;
            _Subtitle = subtitle;
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
                    _ImageWidth = 200;
                    _ImageHeight = 280;
                    break;
                case 3:
                    _ImageWidth = 133;
                    _ImageHeight = 173;
                    break;
                case 4:
                    _ImageWidth = 100;
                    _ImageHeight = 140;
                    break;
                default:
                    _NumberOfImagesInOnePage = 4;
                    _ImageWidth = 100;
                    _ImageHeight = 140;
                    break;
            }
        }

        /// <summary>
        /// Draw the Extra Page
        /// </summary>
        private int DrawSingleExtraPage(string text, PdfPage page, int entraPageNumber, int documentPageNumber, XGraphics gfx)
        {
            // Set properties
            double marginLeft = 45;
            double marginRight = page.Width - marginLeft;
            double bottom = 20;
            double titleHeight = 37;

            if (text != null && text.Length > 0)
            {
                var currentPage = documentPageNumber + entraPageNumber;

                // Draw the Report Informations
                DrawReportInformations(marginLeft, marginRight, titleHeight, ref bottom, currentPage, page, gfx);

                bottom = bottom + 35;

                // Draw the Extra Text           
                var lastDrawnIndex = DrawBorderText(text, marginLeft, bottom + 4, marginRight - 48, page.Height - bottom - 50, _FontRegular, gfx);

                return lastDrawnIndex;
            }

            return 0;
        }

        /// <summary>
        /// Returns the number of the extra pages
        /// </summary>
        private int GetNumberOfExtraPage(string text, PdfPage page)
        {
            var numberOfExtraPage = 0;

            try
            {
                if (!string.IsNullOrEmpty(text))
                {
                    var gfx = XGraphics.FromPdfPage(page);

                    // Set properties
                    double marginLeft = 45;
                    double marginRight = page.Width - marginLeft;
                    var additionalPageHeight = page.Height;

                    // Get the XTextFormatter and the Rectangle
                    XTextFormatter tf = new XTextFormatter(gfx);
                    var rect = new XRect(marginLeft, 0, marginRight - 60, additionalPageHeight);

                    // Calculate the Absoluter Text Height
                    var absoluteTextHeight = tf.GetDrawStringHeight(text, _FontRegular, rect);

                    // Calculate the number of extra page
                    numberOfExtraPage = Convert.ToInt32(absoluteTextHeight / additionalPageHeight);
                    var mod = (absoluteTextHeight % additionalPageHeight);
                    if (absoluteTextHeight % additionalPageHeight > 0)
                    {
                        numberOfExtraPage++;
                    }

                    gfx.Dispose();
                }
            }
            catch (Exception ex)
            {
                // Show a message box                
                var messageBox = new CustomMessageBox(MessageBoxType.Error, ex.Message, MessageBoxButtonsType.OK);
                messageBox.ShowMessageBox();
            }

            return numberOfExtraPage;
        }

        /// <summary>
        /// Draw the Extrapages.
        /// Each pages is created by the characters of the specificated text
        /// </summary>
        private void DrawExtraPages(int totalExtraPages, string text, int currentDocumentPage, PdfDocument document, int backGroundWorkerIncrement, ExtendedBackgroundWorker backgroundWorker = null)
        {
            if (!string.IsNullOrEmpty(text) && totalExtraPages > 0)
            {
                for (var extraPageNumber = 0; extraPageNumber < totalExtraPages; extraPageNumber++)
                {
                    var page = document.AddPage();
                    var gfx = XGraphics.FromPdfPage(page);

                    var lastTextIndex = DrawSingleExtraPage(text, page, extraPageNumber, currentDocumentPage, gfx);
                    if (lastTextIndex > -1)
                    {
                        text = text.Remove(0, lastTextIndex - 1);
                    }
                }

                // Update the background worker
                backgroundWorker?.ReportIncrementProgressWithExeption(backGroundWorkerIncrement);
            }
        }

        /// <summary>
        /// Create the report
        /// </summary>
        public override void CreateReport(string pathFileName, string logoPath, ExtendedBackgroundWorker backgroundWorker = null)
        {
            PdfDocument pdfDocument;
            IEnumerable<IEnumerable<string>> pdfFiles = null;
            var destinationPath = pathFileName;

            int backGroundWorkerIncrement = 0;

            if (_NumberOfImagesInOnePage > 0)
            {
                int currentReportPage = 1;
                _TotalReportNumberOfPage = 0;

                // Calculate the number of PDF files
                if (_ImagesPath != null && _ImagesPath.Any())
                {
                    pdfFiles = _ImagesPath.Split(1000);
                }

                // The number of Additional pages and Comments pages             
                var numberOfAdditionalPages = 0;
                var numberOfCommentsPages = 0;

                // Calculate the partial number of pages (without extra pages)
                var pagesCount = CalculateTheNumberOfPagesWithoutExtraPages(pdfFiles);
                _TotalReportNumberOfPage = _TotalReportNumberOfPage + pagesCount;

                if (pdfFiles != null && pdfFiles.Any())
                {
                    var indexPDFPart = 1;
                    pdfDocument = null;

                    try
                    {                        
                        foreach (var pdfFile in pdfFiles)
                        {
                            var existImages = pdfFile.Count() > 0;                            

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

                            // Create and Draw the Main Page
                            var page = pdfDocument.AddPage(PageSize.A4, PageOrientation.Portrait);

                            // Calculate the total extra pages and the number of Report pages
                            var incrementPageValue = 0;
                            if (indexPDFPart == 1)
                            {
                                numberOfAdditionalPages = GetNumberOfExtraPage(_AdditionalContent, page);
                                numberOfCommentsPages = GetNumberOfExtraPage(_CommentsContent, page);

                                incrementPageValue = numberOfAdditionalPages + numberOfCommentsPages;

                                _TotalReportNumberOfPage = _TotalReportNumberOfPage + incrementPageValue;
                            }

                            // Set the Background worker and get the max and the increment values
                            
                            var values = SetBackGroundWorker(pdfFile, _NumberOfImagesInOnePage, incrementPageValue);
                            backGroundWorkerIncrement = values.Item1;
                            var backGroundWorkerMaxValue = values.Item2;

                            // Draw the first Report page
                            DrawPage(page, _AngleRotatation, true, 0, currentReportPage, logoPath, pdfFile.ToList());

                            // Update the background worker                    
                            backgroundWorker?.ReportIncrementProgressWithExeption(backGroundWorkerIncrement);

                            if (pdfFile != null && existImages)
                            {
                                int index = _NumberOfImagesInOnePage;
                                currentReportPage = currentReportPage + 1;

                                while (index < backGroundWorkerMaxValue)
                                {
                                    page = pdfDocument.AddPage();
                                    DrawPage(page, _AngleRotatation, false, index, currentReportPage, logoPath, pdfFile.ToList());

                                    index = index + _NumberOfImagesInOnePage;
                                    currentReportPage = currentReportPage + 1;

                                    // Update the background worker                            
                                    backgroundWorker?.ReportIncrementProgressWithExeption(backGroundWorkerIncrement);
                                }

                                var max = pdfFiles.Count();
                                if (indexPDFPart == pdfFiles.Count())
                                {
                                    // Drawn the Additional Pages and the Comments Pages                                
                                    DrawExtraPages(numberOfAdditionalPages, _AdditionalContent, currentReportPage, pdfDocument, backGroundWorkerIncrement, backgroundWorker);
                                    currentReportPage = currentReportPage + numberOfAdditionalPages;

                                    DrawExtraPages(numberOfCommentsPages, _CommentsContent, currentReportPage, pdfDocument, backGroundWorkerIncrement, backgroundWorker);
                                    currentReportPage = currentReportPage + numberOfCommentsPages;
                                }
                            }
                            else
                            {
                                var titleHeight = 30;
                                var message = CultureResources.GetString("Message_Images_Not_Found");

                                var gfx = XGraphics.FromPdfPage(page);
                                var x = (page.Width / 2) - (message.Count());
                                DrawText(message, x, 150, page.Width, titleHeight, _FontRegular, gfx);
                            }

                            // Save the Report                       
                            pdfDocument.Save(destinationPartPDFPath);

                            // Trigger the Report Created Event
                            TriggerReportCreated(this, destinationPartPDFPath);

                            indexPDFPart = indexPDFPart + 1;

                            pdfDocument.Dispose();

                            // Update the background worker
                            if (backgroundWorker != null && !backgroundWorker.CancellationPending)
                            {
                                backgroundWorker?.ReportProgressWithExeption(backGroundWorkerMaxValue);
                            }

                        } // end foreach
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

                        indexPDFPart = indexPDFPart + 1;
                    }
                }
                else // Report With no Image
                {
                    DrawEmptyReport(pathFileName, logoPath, backGroundWorkerIncrement, backgroundWorker);
                }
            }
        }        

        /// <summary>
        /// Draw an empty report with only Header and no image 
        /// </summary>
        private void DrawEmptyReport(string pathFileName, string logoPath, int backGroundWorkerIncrement, ExtendedBackgroundWorker backgroundWorker = null)
        {
            PdfDocument pdfDocument = null;
            XGraphics gfx = null;
            var destinationPath = pathFileName;                  
            var numberOfAdditionalPages = 0;
            var numberOfCommentsPages = 0;

            if (_NumberOfImagesInOnePage > 0)
            {
                int currentReportPage = 1;
                _TotalReportNumberOfPage = 0;

                try
                {
                    backgroundWorker?.Reset();

                    // Trigger the Set of Max value of Progress Event
                    TriggerMaxProgressValueChanged(this, 100);

                    /// if file with that name already exists 
                    /// the File Name will be changed (a number index will be added to the end of name) 
                    destinationPath = GetValidFileNamePath(destinationPath);

                    // Trigger the Creation Report Started Event
                    TriggerReportCreationStarted(this, destinationPath);

                    // Set the Total Report Page
                    _TotalReportNumberOfPage = 1;                   

                    // Create the document                        
                    pdfDocument = new PdfDocument(_Title, string.Empty, AuthorName, AuthorName);

                    // Create and Draw the Main Page
                    var page = pdfDocument.AddPage(PageSize.A4, PageOrientation.Portrait);

                    // Draw the first Report page
                    DrawPage(page, _AngleRotatation, true, 0, currentReportPage, logoPath, null);

                    // Update the background worker                    
                    backgroundWorker?.ReportProgressWithExeption(1);

                    // Draw the Magges Image Not Found
                    var titleHeight = 30;
                    var message = CultureResources.GetString("Message_Images_Not_Found");

                    gfx = XGraphics.FromPdfPage(page);
                    var x = (page.Width / 2) - (message.Count());
                    DrawText(message, x, 150, page.Width, titleHeight, _FontRegular, gfx);

                    // Drawn the Additional Pages and the Comments Pages                                
                    DrawExtraPages(numberOfAdditionalPages, _AdditionalContent, currentReportPage, pdfDocument, backGroundWorkerIncrement, backgroundWorker);
                    currentReportPage = currentReportPage + numberOfAdditionalPages;

                    DrawExtraPages(numberOfCommentsPages, _CommentsContent, currentReportPage, pdfDocument, backGroundWorkerIncrement, backgroundWorker);
                    currentReportPage = currentReportPage + numberOfCommentsPages;

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
        }

        /// <summary>
        /// Returns the number of pages without extra page
        /// </summary>
        private int CalculateTheNumberOfPagesWithoutExtraPages(IEnumerable<IEnumerable<string>> pdfFiles)
        {
            var result = 0;

            if (pdfFiles != null && pdfFiles.Any())
            {
                var count = 0;
                foreach (var pdfFile in pdfFiles)
                {
                    // Calculate the number of pages
                    if (pdfFile != null && pdfFile.Any())
                    {
                        count = pdfFile.Count() / _NumberOfImagesInOnePage;
                        if (pdfFile.Count() % _NumberOfImagesInOnePage > 0)
                        {
                            count = count + 1;
                        }
                    }

                    result = result + count;
                }
            }

            return result;
        }

        /// <summary>
        /// Draw the Main Header
        /// </summary>
        private void DrawMainHeader(double marginLeft, double marginRight, ref double bottom, string logoPath, PdfPage page, XGraphics gfx)
        {
            // Draw Title
            var titleHeight = 37;
            var right = marginRight - 48;

            var rect = new XRect(marginLeft, bottom - 8, right, titleHeight);
            gfx.DrawRectangle(XPens.Black, XBrushes.SugarPaper, rect);

            DrawText(_Title, marginLeft + 5, bottom, page.Width / 2, titleHeight, _BigFontBold, gfx, XBrushes.White);

            // Draw Logo
            if (File.Exists(logoPath))
            {
                XImage image = XImage.FromFile(logoPath);

                var logoHeight = titleHeight - 5;
                var logoWidth = (image.Size.Width / image.Size.Height) * logoHeight;

                double diff = (titleHeight - logoHeight) / 2;
                double y = diff + bottom - 8;

                //var imageX = marginRight - logoWidth  - 1;
                var imageX = marginRight - (2 * diff) - logoWidth - 3;
                gfx.DrawImage(image, imageX, y, logoWidth, logoHeight);

                image.Dispose();
            }

            // Separator
            bottom = bottom + titleHeight + 5;
        }

        /// <summary>
        /// Draw the Report Informations
        /// </summary>
        private void DrawReportInformations(double marginLeft, double marginRight, double titleHeight, ref double bottom, int currentPageIndex, PdfPage page, XGraphics gfx)
        {
            // Draw Subtitle and the Draw the Subtitle Rectangle
            var text = _Subtitle;

            var rect = new XRect(marginLeft, bottom - 8, 230, titleHeight);
            gfx.DrawRectangle(XPens.Black, XBrushes.SugarPaper, rect);

            DrawText(text, marginLeft + 5, bottom - 8, page.Width / 3, titleHeight, _FontRegular, gfx, XBrushes.White);

            // Draw Informations
            rect = new XRect(marginLeft + 230, bottom - 8, marginRight - 278, titleHeight);
            gfx.DrawRectangle(XPens.Black, XBrushes.Transparent, rect);

            var titleInformationX = marginRight - 270;
            var informationX = titleInformationX + 55;

            // Current Page \ Number of Pages
            var currentPageFont = new XFont("Verdana", 8, XFontStyle.Bold);
            text = $"{currentPageIndex}/{_TotalReportNumberOfPage}";
            var textWidth = gfx.MeasureString(text, currentPageFont).Width;
            DrawText(text, marginRight - textWidth - 10, bottom + 15, page.Width / 3, titleHeight, currentPageFont, gfx);

            // Lot
            text = "N° lot:";
            DrawText(text, titleInformationX, bottom - 5, page.Width / 3, titleHeight, _LittleFontBold, gfx);

            text = _Lot;
            DrawText(text, informationX, bottom - 5, page.Width / 3, titleHeight, _LittleFontRegular, gfx);

            // Article
            text = "N° article:";
            DrawText(text, titleInformationX, bottom + 6, page.Width / 2, titleHeight, _LittleFontBold, gfx);

            text = _Article;
            DrawText(text, informationX, bottom + 6, page.Width / 2, titleHeight, _LittleFontRegular, gfx);

            // Number of images
            text = "N° images:";
            DrawText(text, titleInformationX, bottom + 17, page.Width / 3, titleHeight, _LittleFontBold, gfx);

            text = _ImagesPath.Count().ToString();
            DrawText(text, informationX, bottom + 17, page.Width / 3, titleHeight, _LittleFontRegular, gfx);
        }

        /// <summary>
        /// Draw the Page
        /// </summary>
        private void DrawPage(PdfPage page, double rotate, bool isHeaderVisible, int firstImageIndex, int currentPage, string logoPath, IList<string> imagesPath)
        {
            var gfx = XGraphics.FromPdfPage(page);

            // Set properties
            double marginLeft = 45;
            // double marginRight = page.Width - marginLeft;
            double marginRight = page.Width - marginLeft;
            double bottom = 20;
            double titleHeight = 37;
            string text = string.Empty;

            // Draw the Header
            if (isHeaderVisible)
            {
                DrawMainHeader(marginLeft, marginRight, ref bottom, logoPath, page, gfx);
            }

            XPen rectanglePen = new XPen(XColors.Black, 1);

            // Draw the Report Informations
            DrawReportInformations(marginLeft, marginRight, titleHeight, ref bottom, currentPage, page, gfx);

            bottom = bottom + 40;

            if (_ImagesPath != null && _ImagesPath.Count() > 0)
            {
                // Draw the Images               
                bottom = bottom + 1;
                int max = _NumberOfImagesInOnePage;
                if (imagesPath.Count < firstImageIndex + max)
                {
                    max = imagesPath.Count - firstImageIndex;
                }

                // Get the sublist from original list.
                // Get a number of elements equals to the number of image that must be shown in page
                var sublist = imagesPath.ToList().GetRange(firstImageIndex, max);
                foreach (var currentFilePath in sublist)
                {
                    var textMargin = marginLeft + _ImageWidth + 30;

                    var midX = ((_ImageWidth + 30) - marginLeft) / 2;
                    var midY = ((_ImageHeight + 14) - (-7)) / 2;
                    XPoint objXpoint = new XPoint(midX, midY);

                    // Draw the image
                    DrawProductImage(currentFilePath, marginLeft, bottom, _ImageWidth, _ImageHeight, rotate, gfx);

                    // Draw Rectangle for File Name and Creation Date
                    var fileNameRectWidth = marginRight - textMargin - 3;
                    var fileNameRect = new XRect(textMargin, bottom - 7, fileNameRectWidth, 40);
                    gfx.DrawRectangle(rectanglePen, fileNameRect);

                    // Draw image file Name
                    var fileName = Path.GetFileName(currentFilePath);
                    if (fileName.Length > 46)
                    {
                        fileName = fileName.Substring(0, 46) + "...";
                    }

                    DrawText("File Name:", textMargin + 5, bottom - 3, page.Width / 2, 30, _LittleFontBold, gfx);
                    DrawText(fileName, textMargin + 55, bottom - 3, page.Width / 2, 30, _LittleFontRegular, gfx);

                    // Draw image creation date time
                    var creationDateTime = File.GetCreationTime(currentFilePath);
                    DrawText("Creation:", textMargin + 5, bottom + 7, page.Width / 2, 30, _LittleFontBold, gfx);
                    DrawText(creationDateTime.ToString(), textMargin + 55, bottom + 7, page.Width / 2, 30, _LittleFontRegular, gfx);

                    // Draw Rectangle for Cause and Action information 
                    var rectWidth = marginRight - textMargin - 3;
                    var recHeight = _ImageHeight - 23;
                    var rect = new XRect(textMargin, bottom + 30, rectWidth, recHeight);
                    gfx.DrawRectangle(rectanglePen, rect);

                    // Draw rectangle and Text of Cause (Title and Text)
                    double textHeight = (recHeight / 2) - 20;

                    rect = new XRect(textMargin, bottom + 30, rectWidth, 15);
                    gfx.DrawRectangle(XPens.Black, XBrushes.SugarPaper, rect);

                    ImageData imageData = null;
                    if (_FileManager != null)
                    {
                        imageData = _FileManager.LoadImageInformationsFile(Path.GetDirectoryName(currentFilePath), Path.GetFileNameWithoutExtension(currentFilePath));
                    }

                    var cause = imageData != null ? imageData.Cause : string.Empty;
                    DrawText("Cause:", textMargin + 5, bottom + 33, 50, 10, _LittleFontRegular, gfx, XBrushes.White);
                    DrawText(cause, textMargin + 5, bottom + 47, rectWidth - 10, textHeight, _LittleFontRegular, gfx);

                    // Draw rectangle and Text of Action (Title and Text)
                    rect = new XRect(textMargin, bottom + 50 + textHeight, rectWidth, 15);
                    gfx.DrawRectangle(XPens.Black, XBrushes.SugarPaper, rect);

                    var action = imageData != null ? imageData.Action : string.Empty;
                    DrawText("Action:", textMargin + 5, bottom + 53 + textHeight, 50, 10, _LittleFontRegular, gfx, XBrushes.White);
                    DrawText(action, textMargin + 5, bottom + 51 + textHeight + 16, rectWidth - 10, textHeight, _LittleFontRegular, gfx);

                    // Update the bottom
                    bottom = bottom + _ImageHeight + 19;
                }
            }

            ClearMemory();
            gfx.Dispose();
        }

        #endregion
    }
}
