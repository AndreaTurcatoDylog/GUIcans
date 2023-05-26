using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Core
{
    /// <summary>
    /// Ths Point EventArgs
    /// </summary>
    public class PointEventArgs : EventArgs
    {
        public Point Coordinates { get; private set; }

        public PointEventArgs(Point coordinates)
        {
            Coordinates = coordinates;
        }
    }

    /// <summary>
    /// Interaction logic for ZoomImageUserControl.xaml
    /// </summary>
    public partial class ZoomImageUserControl : DisposableUserControlBase
    {
        #region Constants

        private readonly double ZoomValue = 1.1;

        #endregion

        #region Members

        /// <summary>
        /// Store the number of touch point used in manipulation in Touch screen
        /// </summary>
        private volatile int _NumberOfTouchPoints;

        /// <summary>
        /// A Mutex used in manipulation in Touch screen
        /// </summary>
        private object _Mutex = new object();


        /// <summary>
        /// It is set when the user click the left button of mouse or tip with finger on main image
        /// </summary>
        private Point? _LastDragPoint;

        /// <summary>
        /// The Min width of the image. It is useful to prevent an extreme zoom in
        /// </summary>
        private double _MinWidthValue;

        /// <summary>
        /// The Min height of the image. It is useful to prevent an extreme zoom in
        /// </summary>
        private double _MinHeightValue;

        /// <summary>
        /// Used to zoom the image when the button keep pressed
        /// </summary>
        private BackgroundWorker _ZoomBackgroundWorker;

        /// <summary>
        /// Specify whether zoom is performed by buttons (not mouse\pinch)
        /// </summary>
        private bool _IsZoomFromButton;

        /// <summary>
        /// Specify whether the zoom is ZoomOut or ZoomIn. 
        /// Used in Background worker that starts when a '+' or '-' button is pressed.
        /// </summary>
        private ZoomMode _ZoomMode;

        // Specify if the Image is translated up (over the Scroolviever)
        private bool _IsImageTranslated;

        // Specify offset Y when the Image is translated up (over the Scroolviever)
        private double _TranslatedOffesetY;

        // Specify offset Y when the Image is translated right (over the Scroolviever)
        private double _TranslatedOffesetX;

        #endregion

        #region Event Handler

        public event EventHandler ImageZoomed;
        public event EventHandler ImageScrolled;
        public event EventHandler DatasetCoordinatesGot;

        #endregion

        #region Properties

        /// <summary>
        /// Get\Set the CanChooseCoordinates
        /// </summary>
        public bool CanChooseCoordinates
        {
            get { return (bool)GetValue(CanChooseCoordinatesProperty); }
            set { SetValue(CanChooseCoordinatesProperty, value); }
        }

        /// <summary>
        /// Get\Set the CoordinateMessage
        /// </summary>
        public string CoordinateMessage
        {
            get { return (string)GetValue(CoordinateMessageProperty); }
            set { SetValue(CoordinateMessageProperty, value); }
        }

        public ScrollViewer ScrollViewer
        {
            get { return (ScrollViewer)GetValue(ScrollViewerProperty); }
            set { SetValue(ScrollViewerProperty, value); }
        }

        private static readonly DependencyProperty ScrollViewerProperty =
          DependencyProperty.Register("ScrollViewer", typeof(ScrollViewer), typeof(ZoomImageUserControl));


        /// <summary>
        /// Get\Set The image source
        /// </summary>
        public WriteableBitmap ImageSource
        {
            get { return (WriteableBitmap)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        /// <summary>
        /// Get\Set The Results source
        /// </summary>
        public WriteableBitmap ResultsSource
        {
            get { return (WriteableBitmap)GetValue(ResultsSourceProperty); }
            set { SetValue(ResultsSourceProperty, value); }
        }

        /// <summary>
        /// Get\Set the X coordinate that occurs when mouse moves on image in string format
        /// </summary>
        public string HorizontalCoordinateInImage
        {
            get { return (string)GetValue(HorizontalCoordinateInImageProperty); }
            set { SetValue(HorizontalCoordinateInImageProperty, value); }
        }

        /// <summary>
        /// Get\Set the Y coordinate that occurs when mouse moves on image in string format
        /// </summary>
        public string VerticalCoordinateInImage
        {
            get { return (string)GetValue(VerticalCoordinateInImageProperty); }
            set { SetValue(VerticalCoordinateInImageProperty, value); }
        }

        /// <summary>
        /// Get\Set scalar value zoom for x-axis
        /// </summary>
        public double ScalarZoomX
        {
            get { return (double)GetValue(ScalarZoomXProperty); }
            set { SetValue(ScalarZoomXProperty, value); }
        }

        /// <summary>
        /// Get\Set scalar value zoom for y-axis
        /// </summary>
        public double ScalarZoomY
        {
            get { return (double)GetValue(ScalarZoomYProperty); }
            set { SetValue(ScalarZoomYProperty, value); }
        }

        /// <summary>
        /// Get\Set visibility of horizontal rule
        /// </summary>
        public Visibility HorizontalRulersVisibility
        {
            get { return (Visibility)GetValue(HorizontalRulersVisibilityProperty); }
            set { SetValue(HorizontalRulersVisibilityProperty, value); }
        }

        /// <summary>
        /// The scalar value zoom for y-axis
        /// </summary>
        public Visibility VerticalRulersVisibility
        {
            get { return (Visibility)GetValue(VerticalRulersVisibilityProperty); }
            set { SetValue(VerticalRulersVisibilityProperty, value); }
        }

        /// <summary>
        /// Get\Set the horizontal rule margin
        /// </summary>
        public string HorizontalRuleMargin
        {
            get { return (string)GetValue(HorizontalRuleMarginProperty); }
            private set { SetValue(HorizontalRuleMarginProperty, value); }
        }

        /// <summary>
        /// Get\Set the vertical rule margin
        /// </summary>
        public string VerticalRuleMargin
        {
            get { return (string)GetValue(VerticalRuleMarginProperty); }
            private set { SetValue(VerticalRuleMarginProperty, value); }
        }

        /// <summary>
        /// Get\Set the result images margin
        /// </summary>
        public string ResultImagesMargin
        {
            get { return (string)GetValue(ResultImagesMarginProperty); }
            private set { SetValue(ResultImagesMarginProperty, value); }
        }

        /// <summary>
        /// Get\Set the refresh information
        /// </summary>
        public bool Refresh
        {
            get { return (bool)GetValue(RefreshProperty); }
            set { SetValue(RefreshProperty, value); }
        }

        /// <summary>
        /// Get\Set the Mouse X
        /// </summary>
        public double MouseX
        {
            get { return (double)GetValue(MouseXProperty); }
            set { SetValue(MouseXProperty, value); }
        }

        /// <summary>
        /// Get\Set the Mouse Y
        /// </summary>
        public double MouseY
        {
            get { return (double)GetValue(MouseYProperty); }
            set { SetValue(MouseYProperty, value); }
        }

        /// <summary>
        /// Get\Set the Is Zoom Enabled.
        /// The Zoom is enabled if the ImageSource is not null (an image is loaded)
        /// </summary>
        public bool IsZoomEnabled
        {
            get { return (bool)GetValue(IsZoomEnabledProperty); }
            set { SetValue(IsZoomEnabledProperty, value); }
        }

        #endregion

        #region Dependency Properties

        private static readonly DependencyProperty CanChooseCoordinatesProperty =
          DependencyProperty.Register("CanChooseCoordinates", typeof(bool), typeof(ZoomImageUserControl), new PropertyMetadata(false));

        private static readonly DependencyProperty CoordinateMessageProperty =
          DependencyProperty.Register("CoordinateMessage", typeof(string), typeof(ZoomImageUserControl), new PropertyMetadata(string.Empty));

        private static readonly DependencyProperty HorizontalCoordinateInImageProperty =
          DependencyProperty.Register("HorizontalCoordinateInImage", typeof(string), typeof(ZoomImageUserControl), new PropertyMetadata(string.Empty));

        private static readonly DependencyProperty VerticalCoordinateInImageProperty =
          DependencyProperty.Register("VerticalCoordinateInImage", typeof(string), typeof(ZoomImageUserControl), new PropertyMetadata(string.Empty));

        private static readonly DependencyProperty ImageSourceProperty =
          DependencyProperty.Register("ImageSource", typeof(WriteableBitmap), typeof(ZoomImageUserControl),
                new PropertyMetadata(OnImageSourceChangedCallBack));

        private static readonly DependencyProperty ResultsSourceProperty =
          DependencyProperty.Register("ResultsSource", typeof(WriteableBitmap), typeof(ZoomImageUserControl),
                new PropertyMetadata(OnResultSourceChangedCallBack));

        private static readonly DependencyProperty ScalarZoomXProperty =
            DependencyProperty.Register("ScalarZoomX", typeof(double), typeof(ZoomImageUserControl), new PropertyMetadata(1.0));

        private static readonly DependencyProperty ScalarZoomYProperty =
           DependencyProperty.Register("ScalarZoomY", typeof(double), typeof(ZoomImageUserControl), new PropertyMetadata(1.0));

        private static readonly DependencyProperty HorizontalRulersVisibilityProperty =
            DependencyProperty.Register("HorizontalRulersVisibility", typeof(Visibility), typeof(ZoomImageUserControl), new PropertyMetadata(Visibility.Visible));

        private static readonly DependencyProperty VerticalRulersVisibilityProperty =
            DependencyProperty.Register("VerticalRulersVisibility", typeof(Visibility), typeof(ZoomImageUserControl), new PropertyMetadata(Visibility.Visible));

        private static readonly DependencyProperty HorizontalRuleMarginProperty =
           DependencyProperty.Register("HorizontalRuleMargin", typeof(string), typeof(ZoomImageUserControl));

        private static readonly DependencyProperty VerticalRuleMarginProperty =
           DependencyProperty.Register("VerticalRuleMargin", typeof(string), typeof(ZoomImageUserControl));

        private static readonly DependencyProperty ResultImagesMarginProperty =
          DependencyProperty.Register("ResultImagesMargin", typeof(string), typeof(ZoomImageUserControl));

        private static readonly DependencyProperty RefreshProperty =
          DependencyProperty.Register("Refresh", typeof(bool), typeof(ZoomImageUserControl),
        new PropertyMetadata(OnRefreshRequestCallBack));

        private static readonly DependencyProperty MouseXProperty =
          DependencyProperty.Register("MouseX", typeof(double), typeof(ZoomImageUserControl));

        private static readonly DependencyProperty MouseYProperty =
          DependencyProperty.Register("MouseY", typeof(double), typeof(ZoomImageUserControl));

        private static readonly DependencyProperty IsZoomEnabledProperty =
         DependencyProperty.Register("IsZoomEnabled", typeof(bool), typeof(ZoomImageUserControl));

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ZoomImageUserControl()
        {
            InitializeComponent();

            // TODO:
            // This line of code avoid Exception in Output console.
            // Must test if all works fine yet
            ResultImagesMargin = "0,0,0,0";

            CoordinateMessage = $"X = - Y = ";
            CanChooseCoordinates = false;

            _ZoomBackgroundWorker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true
            };

            LayoutRoot.DataContext = this;

            ScrollViewer = ImageScrollViewer;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Occurs when the component is loaded
        /// </summary>
        protected override void Load()
        {

            ScrollViewer = ImageScrollViewer;

            // Get the dimensions of the Border Image. Used to set the dimensions of Image
            //var widthBorderImage = MainImageBorder.ActualWidth;
            //var heightBorderImage = MainImageBorder.ActualHeight;

            //Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
            //{
            //    HorizontalCoordinateInImage = $"(X: 000)";
            //    VerticalCoordinateInImage = $"(Y: 000)";

            //    //ImageResult.Source = DrawRubbish();
            //    //ResultsSource = new WriteableBitmap((int)widthBorderImage, (int)heightBorderImage, 96d, 96d, PixelFormats.Bgra32, null);
            //    //for (int x = 0; x < 300; x++)
            //    //{
            //    //    DrawPixel(100, x);
            //    //}

            //    //if (ResultsImage != null)
            //    //{
            //    //    _DrawImageResult = new DrawImageResult(ImageResultCanvas, new List<IImageResult>(ResultsImage));
            //    //}
            //    //else
            //    //{
            //    //    _DrawImageResult = new DrawImageResult(ImageResultCanvas, new List<IImageResult>());
            //    //}
            //}));

            //Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ContextIdle, new Action(() =>
            //{

            //}));
        }

        /// <summary>
        /// Register\Unregister events
        /// </summary>
        protected override void RegisterEvents(bool register)
        {
            if (register)
            {
                ImageScrollViewer.PreviewMouseWheel += OnImageScrollViewerPreviewMouseWheel;
                ImageScrollViewer.ScrollChanged += OnImageScrollViewerScrollChanged;
                ImageScrollViewer.MouseMove += OnImageScrollViewerMouseMove;
                ImageScrollViewer.MouseUp += OnImageScrollViewerMouseUp;
                ImageScrollViewer.PreviewMouseLeftButtonDown += OnImageScrollViewerPreviewMouseLeftButtonDown;
                ImageScrollViewer.PreviewMouseDoubleClick += OnImageScrollViewerPreviewMouseDoubleClick;

                MainImage.ManipulationStarting += MainImageManipulationStarting;
                MainImage.ManipulationDelta += MainImageManipulationDelta;
                MainImage.PreviewTouchDown += MainImagePreviewTouchDown;
                MainImage.PreviewTouchUp += MainImagePreviewTouchUp;
                MainImage.MouseMove += OnMainImageMouseMove;

                ZoomOutButton.PreviewMouseDown += OnZoomOutMouseDown;
                ZoomOutButton.PreviewMouseUp += OnZoomButtonPreviewMouseUp;
                ZoomOutButton.PreviewTouchDown += OnZoomOutTouchDown;
                ZoomOutButton.PreviewTouchUp += OnZoomTouchUp;

                ZoomInButton.PreviewMouseDown += OnZoomInMouseDown;
                ZoomInButton.PreviewMouseUp += OnZoomButtonPreviewMouseUp;
                ZoomInButton.PreviewTouchDown += OnZoomInPreviewTouchDown;
                ZoomInButton.PreviewTouchUp += OnZoomTouchUp;

                RefreshButton.PreviewMouseDown += OnRefreshMouseDown;

                _ZoomBackgroundWorker.DoWork += OnZoomBackgroundWorkerDoWork;
            }
            else
            {
                ImageScrollViewer.PreviewMouseWheel -= OnImageScrollViewerPreviewMouseWheel;
                ImageScrollViewer.ScrollChanged -= OnImageScrollViewerScrollChanged;
                ImageScrollViewer.MouseMove -= OnImageScrollViewerMouseMove;
                ImageScrollViewer.MouseUp -= OnImageScrollViewerMouseUp;
                ImageScrollViewer.PreviewMouseLeftButtonDown -= OnImageScrollViewerPreviewMouseLeftButtonDown;
                ImageScrollViewer.PreviewMouseDoubleClick -= OnImageScrollViewerPreviewMouseDoubleClick;
                ImageScrollViewer.ManipulationDelta -= MainImageManipulationDelta;

                MainImage.ManipulationStarting -= MainImageManipulationStarting;
                MainImage.ManipulationDelta -= MainImageManipulationDelta;
                MainImage.PreviewTouchDown -= MainImagePreviewTouchDown;
                MainImage.PreviewTouchUp -= MainImagePreviewTouchUp;
                MainImage.MouseMove -= OnMainImageMouseMove;

                ZoomOutButton.PreviewMouseDown -= OnZoomOutMouseDown;
                ZoomOutButton.PreviewMouseUp -= OnZoomButtonPreviewMouseUp;
                ZoomOutButton.PreviewTouchDown -= OnZoomOutTouchDown;
                ZoomOutButton.PreviewTouchUp -= OnZoomTouchUp;

                ZoomInButton.PreviewMouseDown -= OnZoomInMouseDown;
                ZoomInButton.PreviewMouseUp -= OnZoomButtonPreviewMouseUp;
                ZoomInButton.PreviewTouchDown -= OnZoomInPreviewTouchDown;
                ZoomInButton.PreviewTouchUp -= OnZoomTouchUp;

                RefreshButton.PreviewMouseDown -= OnRefreshMouseDown;

                _ZoomBackgroundWorker.DoWork -= OnZoomBackgroundWorkerDoWork;
            }
        }

        /// <summary>
        /// Perform the Zoom
        /// </summary>
        private void PerformZoom(ZoomMode zoomMode)
        {
            if (ImageSource != null)
            {
                if (_MinHeightValue == 0 || _MinWidthValue == 0)
                {
                    _MinWidthValue = MainImage.DesiredSize.Width;
                    _MinHeightValue = MainImage.DesiredSize.Height;
                }

                if (zoomMode == ZoomMode.ZoomOut)
                {
                    ScalarZoomX = ScalarZoomX * ZoomValue;
                    ScalarZoomY = ScalarZoomY * ZoomValue;
                }
                else if (zoomMode == ZoomMode.ZoomIn)
                {
                    double valueX = ScalarZoomX / ZoomValue;
                    double valueY = ScalarZoomY / ZoomValue;
                    if (MainImage.DesiredSize.Width > _MinWidthValue &&
                        MainImage.DesiredSize.Height > _MinHeightValue)
                    {
                        ScalarZoomX = valueX;
                        ScalarZoomY = valueY;
                    }
                    else
                    {
                        RefreshUserControl();
                    }
                }
            }
        }

        ///// <summary>
        ///// Get the current Mouse's Coordinates
        ///// </summary>
        //public Point GetImageCoordsAt(object sender, MouseEventArgs e)
        //{
        //    //if (sender != null && sender is Image imageControl && imageControl.IsMouseOver)
        //    if (sender != null)
        //    {
        //        if (sender is Image imageControl)
        //        {

        //            var controlSpacePosition = e.GetPosition(imageControl);
        //            //var imageControl = this.Child as Image;              
        //            if (imageControl != null && imageControl.Source != null)
        //            {
        //                // Convert from control space to image space
        //                var x = Math.Floor(controlSpacePosition.X * imageControl.Source.Width / imageControl.ActualWidth);
        //                var y = Math.Floor(controlSpacePosition.Y * imageControl.Source.Height / imageControl.ActualHeight);

        //                return new Point(x, y);
        //            }
        //        }
        //    }
        //    return new Point(-1, -1);
        //}


        /// <summary>
        /// Get the current Mouse's Coordinates
        /// </summary>
        public Point GetImageCoordsAt(Image image, MouseEventArgs e)
        {
            var controlSpacePosition = e.GetPosition(image);

            if (image != null && image.Source != null)
            {
                // Convert from control space to image space
                var x = Math.Floor(controlSpacePosition.X * image.Source.Width / image.ActualWidth);
                var y = Math.Floor(controlSpacePosition.Y * image.Source.Height / image.ActualHeight);

                return new Point(x, y);
            }

            return new Point(-1, -1);
        }       

        /// <summary>
        /// Returns the offsetX and offsetY. Used to translate the rules dependendig by main image position
        /// </summary>
        /// <returns> Item1: offsetX. Item2: offsetY </returns>
        private Tuple<double, double> GetOffset()
        {
            // Get the offset
            var imageWidth = MainImage.ActualWidth * ScalarZoomX;
            var imageHeight = MainImage.ActualHeight * ScalarZoomY;

            var viewPortWidth = ImageScrollViewer.ViewportWidth;
            var viewPortHeight = ImageScrollViewer.ViewportHeight;

            var offsetX = (viewPortWidth / 2) - (imageWidth / 2);
            var offsetY = (viewPortHeight / 2) - (imageHeight / 2);

            return Tuple.Create(offsetX, offsetY);
        }

        /// <summary>
        /// Set rules value lines. The rules value lines are the line showing value on the rules
        /// </summary>
        private void SetRulesValueLines(double positionX, double positionY)
        {
            // Translate the horizontal and vertical rules in according of main image position
            TranslateComponents();

            // Set  ValueX and ValueY
            var valueX = positionX;
            var valueY = positionY;

            HorizontalCoordinateInImage = $"(X: {valueX.ToString("000")})";
            VerticalCoordinateInImage = $"(Y: {valueY.ToString("000")})";

            MouseX = positionX;
            MouseY = positionY;
        }

        /// <summary>
        /// Translate the result images canvas and the vertical and horizontal rules in according to main image position
        /// </summary>
        private void TranslateComponents()
        {
            // Get the offset
            var offset = GetOffset();

            // Translate the horizontal rule
            var marginX = (MainImage.DesiredSize.Width < ImageScrollViewer.ViewportWidth) ? Convert.ToInt32(offset.Item1) : 0;
            HorizontalRuleMargin = $"{marginX},0,0,0";

            // Translate the vertical rule
            var marginY = (MainImage.DesiredSize.Height < ImageScrollViewer.ViewportHeight) ? Convert.ToInt32(offset.Item2) : 0;
            VerticalRuleMargin = $"0,{marginY},0,0";

            // Translate the result images canvas
            // ResultImagesMargin = $"{marginX},{marginY},0,0";
        }

        /// <summary>
        /// Refresh the component
        /// </summary>
        private void RefreshUserControl()
        {
            if (MainImage != null && ImageScrollViewer != null)
            {
                ScalarZoomX = ScalarZoomY = 1;
                ImageScrollViewer.ScrollToTop();
                ImageScrollViewer.ScrollToLeftEnd();
                TranslateComponents();
            }
        }

        /// <summary>
        /// Performs the Zoom in\out in the background worker
        /// </summary>
        private void OnZoomBackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            _IsZoomFromButton = true;

            while (!e.Cancel)
            {
                Dispatcher.BeginInvoke(new ThreadStart(delegate
                {
                    PerformZoom(_ZoomMode);
                }));


                if (_ZoomBackgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                }

                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// Start the BackgroundWorker to perform the Zoom In\Out
        /// </summary>
        private void StartBackGroundWorker(ZoomMode zoomMode)
        {
            _ZoomMode = zoomMode;

            if (_ZoomBackgroundWorker.IsBusy)
            {
                _ZoomBackgroundWorker.CancelAsync();
            }
            else
            {
                _ZoomBackgroundWorker.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Trigger the Dataset Coordinates Got
        /// </summary>
        private void TriggerDatasetCoordinatesGot(object sender, EventArgs pointEventArgs)
        {
            DatasetCoordinatesGot?.Invoke(sender, pointEventArgs);
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when left button mouse is up on the scrool viewer with image inside
        /// </summary>
        private void OnImageScrollViewerMouseUp(object sender, MouseButtonEventArgs e)
        {
            _LastDragPoint = null;
            ImageScrollViewer.Cursor = Cursors.Arrow;
            ImageScrollViewer.ReleaseMouseCapture();

            if (_IsImageTranslated)
            {
                // Restore position after the image is translated Up when the user wanted 
                // to see details covered by Buttons
                MainImage.RenderTransform = null;
                _IsImageTranslated = false;

                if (_TranslatedOffesetY > 0)
                {
                    ImageScrollViewer.ScrollToBottom();
                }
                if (_TranslatedOffesetX > 0)
                {
                    ImageScrollViewer.ScrollToLeftEnd();
                }

                _TranslatedOffesetY = 0;
                _TranslatedOffesetX = 0;
            }
        }

        /// <summary>
        /// Occurs when mouse is moving on the scrool viewer with image inside
        /// </summary>
        private void OnImageScrollViewerMouseMove(object sender, MouseEventArgs e)
        {
            if (!CanChooseCoordinates)
            {
                if (_LastDragPoint.HasValue && e.LeftButton == MouseButtonState.Pressed)
                {
                    Point position = e.GetPosition(ImageScrollViewer);

                    double dX = position.X - _LastDragPoint.Value.X;
                    double dY = position.Y - _LastDragPoint.Value.Y;

                    _LastDragPoint = position;

                    if (ImageScrollViewer.VerticalOffset == ImageScrollViewer.ScrollableHeight && dY < 0)
                    {
                        // Translate the image Up when the user want to see details covered by Buttons
                        _TranslatedOffesetY = _TranslatedOffesetY - dY;
                        _IsImageTranslated = true;
                    }

                    if (ImageScrollViewer.HorizontalOffset == 0 && dX > 0)
                    {
                        // Translate the image Right when the user want to see details covered by Buttons
                        _TranslatedOffesetX = _TranslatedOffesetX + dX;
                        _IsImageTranslated = true;
                    }

                    if (_IsImageTranslated)
                    {
                        TranslateTransform translateTransform = new TranslateTransform(_TranslatedOffesetX, -_TranslatedOffesetY);
                        MainImage.RenderTransform = translateTransform;
                    }

                    ImageScrollViewer.ScrollToHorizontalOffset(ImageScrollViewer.HorizontalOffset - dX);
                    ImageScrollViewer.ScrollToVerticalOffset(ImageScrollViewer.VerticalOffset - dY);

                }
            }
        }

        /// <summary>
        /// Occurs when the mouse on image is moving
        /// </summary>
        private void OnMainImageMouseMove(object sender, MouseEventArgs e)
        {
            // Get  mouse position
            var mousePosition = e.GetPosition((IInputElement)sender);
            SetRulesValueLines(mousePosition.X, mousePosition.Y);
        }

        /// <summary>
        /// Occurs when the left mouse button on scrool viewer with main image inside is pressed
        /// </summary>
        private void OnImageScrollViewerPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mousePosition = e.GetPosition((IInputElement)sender);
            if (mousePosition.X <= ImageScrollViewer.ViewportWidth && mousePosition.Y < ImageScrollViewer.ViewportHeight)
            {
                _LastDragPoint = mousePosition;
                ImageScrollViewer.Cursor = Cursors.SizeAll;
                Mouse.Capture(ImageScrollViewer);
            }
        }

        /// <summary>
        /// Occurs when the scrool bar of scrool viewer changes its value
        /// </summary>
        private void OnImageScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            double XMousePositionOnScrollViewer = 0;
            double YMousePositionOnScrollViewer = 0;

            if (sender is ScrollViewer scrollViewer)
            {
                if ((Math.Abs(e.ExtentHeightChange) < 0) && (Math.Abs(e.ExtentWidthChange) < 0)) return;

                if (_IsZoomFromButton)
                {
                    XMousePositionOnScrollViewer = MainImage.Width / 2;
                    YMousePositionOnScrollViewer = MainImage.Height / 2;
                }
                else
                {
                    XMousePositionOnScrollViewer = Mouse.GetPosition(scrollViewer).X;
                    YMousePositionOnScrollViewer = Mouse.GetPosition(scrollViewer).Y;
                }

                var offsetX = e.HorizontalOffset + XMousePositionOnScrollViewer;
                var offsetY = e.VerticalOffset + YMousePositionOnScrollViewer;

                var oldExtentWidth = e.ExtentWidth - e.ExtentWidthChange;
                var oldExtentHeight = e.ExtentHeight - e.ExtentHeightChange;

                var relx = offsetX / oldExtentWidth;
                var rely = offsetY / oldExtentHeight;

                offsetX = Math.Max(relx * e.ExtentWidth - XMousePositionOnScrollViewer, 0);
                offsetY = Math.Max(rely * e.ExtentHeight - YMousePositionOnScrollViewer, 0);

                scrollViewer.ScrollToHorizontalOffset(offsetX);
                scrollViewer.ScrollToVerticalOffset(offsetY);

                // Raise events
                ImageScrolled?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Occurs when the Main Image is touch up
        /// </summary>
        private void MainImagePreviewTouchUp(object sender, TouchEventArgs e)
        {
            lock (_Mutex)
            {
                _NumberOfTouchPoints--;
            }

            if (_IsImageTranslated)
            {
                MainImage.RenderTransform = null;
                _IsImageTranslated = false;

                if (_TranslatedOffesetY > 0)
                {
                    ImageScrollViewer.ScrollToBottom();
                }
                if (_TranslatedOffesetX > 0)
                {
                    ImageScrollViewer.ScrollToLeftEnd();
                }

                _TranslatedOffesetY = 0;
                _TranslatedOffesetX = 0;
            }
        }

        /// <summary>
        /// Occurs when the Main Image is touch down
        /// </summary>
        private void MainImagePreviewTouchDown(object sender, TouchEventArgs e)
        {
            lock (_Mutex)
            {
                _NumberOfTouchPoints++;
            }
        }

        /// <summary>
        /// Occurs when the Manipulation in Main Image starting
        /// </summary>
        private void MainImageManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            e.ManipulationContainer = ImageScrollViewer;
            e.Handled = true;
        }

        /// <summary>
        /// Occurs when the user pinch the canvas with fingers. It is produce zoom in and zoom out
        /// </summary>
        private void MainImageManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            int numberOfPoints = 0;

            lock (_Mutex)
            {
                numberOfPoints = _NumberOfTouchPoints;
            }

            if (numberOfPoints >= 2)
            {
                // Perform the Zoom
                var zoomMode = (e.DeltaManipulation.Scale.X < 1 || e.DeltaManipulation.Scale.Y < 1) ? ZoomMode.ZoomIn : ZoomMode.ZoomOut;
                PerformZoom(zoomMode);

                e.Handled = true;
            }
            else
            {
                // Perform Zoom if needed
                if (e.DeltaManipulation.Scale.X != 1 || e.DeltaManipulation.Scale.Y != 1)
                {
                    var zoomMode = (e.DeltaManipulation.Scale.X < 1 || e.DeltaManipulation.Scale.Y < 1) ? ZoomMode.ZoomIn : ZoomMode.ZoomOut;
                    PerformZoom(zoomMode);
                }

                //// Perform extra Translation in Y axis if needed
                //var dY = e.DeltaManipulation.Translation.Y;
                //if (ImageScrollViewer.VerticalOffset == ImageScrollViewer.ScrollableHeight && dY < 0)
                //{
                //    _TranslatedOffesetY = _TranslatedOffesetY - dY;
                //    TranslateTransform translateTransform = new TranslateTransform(0, -_TranslatedOffesetY);
                //    MainImage.RenderTransform = translateTransform;

                //    _IsImageTranslated = true;
                //}

                var dY = e.DeltaManipulation.Translation.Y;
                var dX = e.DeltaManipulation.Translation.X;
                if (ImageScrollViewer.VerticalOffset == ImageScrollViewer.ScrollableHeight && dY < 0)
                {
                    // Translate the image Up when the user want to see details covered by Buttons
                    _TranslatedOffesetY = _TranslatedOffesetY - dY;
                    _IsImageTranslated = true;
                }
                else
                {
                    dY = 0;
                }

                if (ImageScrollViewer.HorizontalOffset == 0 && dX > 0)
                {
                    // Translate the image Right when the user want to see details covered by Buttons
                    _TranslatedOffesetX = _TranslatedOffesetX + dX;
                    _IsImageTranslated = true;
                }
                else
                {
                    dX = 0;
                }

                if (_IsImageTranslated)
                {
                    TranslateTransform translateTransform = new TranslateTransform(_TranslatedOffesetX, -_TranslatedOffesetY);
                    MainImage.RenderTransform = translateTransform;
                }

                // Translate
                ImageScrollViewer.ScrollToHorizontalOffset(ImageScrollViewer.HorizontalOffset - e.DeltaManipulation.Translation.X);
                ImageScrollViewer.ScrollToVerticalOffset(ImageScrollViewer.VerticalOffset - e.DeltaManipulation.Translation.Y);
            }
        }        

        /// <summary>
        /// Occurs when the mouse wheel is rotated and produce zoom in/out
        /// </summary>
        private void OnImageScrollViewerPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            _IsZoomFromButton = false;

            // Perform the Zoom
            var zoomMode = e.Delta > 0 ? ZoomMode.ZoomOut : ZoomMode.ZoomIn;
            PerformZoom(zoomMode);

            // Get the mouse position
            var mousePosition = e.GetPosition((IInputElement)sender);
            SetRulesValueLines(mousePosition.X, mousePosition.Y);

            // Raise events
            ImageZoomed?.Invoke(this, new EventArgs());

            e.Handled = true;
        }

        /// <summary>
        /// The refresh callback. Used to recognize changes in Refresh property
        /// </summary>
        private static void OnRefreshRequestCallBack(
        DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is ZoomImageUserControl c)
            {
                if (c.Refresh)
                {
                    c.RefreshUserControl();
                }
            }
        }

        /// <summary>
        /// The refresh callback. Used to recognize changes in Image Source property
        /// </summary>
        private static void OnImageSourceChangedCallBack(
        DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is ZoomImageUserControl c)
            {
                if (c.ImageSource != null)
                {
                    c.IsZoomEnabled = true;

                    // Set initial value
                    c._MinWidthValue = 0;
                    c._MinHeightValue = 0;

                    // Refresh
                    c.RefreshUserControl();
                }
                else
                {
                    c.IsZoomEnabled = false;
                }
            }
        }

        /// <summary>
        /// The refresh callback. Used to recognize changes in Image Source property
        /// </summary>
        private static void OnResultSourceChangedCallBack(
        DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is ZoomImageUserControl c)
            {
                //ResultsSource = new WriteableBitmap((int)widthBorderImage, (int)heightBorderImage, 96d, 96d, PixelFormats.Bgra32, null);
                //c.ResultsSource.SetValue(c.ResultsSource.PixelWidth, (int)c.widthBorderImage);
                //for (int x = 0; x < 300; x++)
                //{
                //    DrawPixel(100, x);
                //}
            }
        }

        /// <summary>
        /// Occurs when the user make a double click.
        /// It delete the zoom in\out and the translate
        /// </summary>
        private void OnImageScrollViewerPreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            RefreshUserControl();
        }

        /// <summary>
        /// Occurs when the user clicks the button to perform the zoom in
        /// </summary>
        private void OnZoomInMouseDown(object sender, MouseButtonEventArgs e)
        {
            StartBackGroundWorker(ZoomMode.ZoomIn);

            // e.Handled = true;
        }

        /// <summary>
        /// Occurs when the user touch the button to perform the zoom in
        /// </summary>
        private void OnZoomInPreviewTouchDown(object sender, TouchEventArgs e)
        {
            StartBackGroundWorker(ZoomMode.ZoomIn);

            e.Handled = true;
        }

        /// <summary>
        /// Occurs when the user clicks the button to the zoom Out
        /// </summary>
        private void OnZoomOutMouseDown(object sender, MouseButtonEventArgs e)
        {
            StartBackGroundWorker(ZoomMode.ZoomOut);

            // e.Handled = true;
        }

        /// <summary>
        /// Occurs when the user touch the button to perform the zoom out
        /// </summary>
        private void OnZoomOutTouchDown(object sender, TouchEventArgs e)
        {
            StartBackGroundWorker(ZoomMode.ZoomOut);

            e.Handled = true;
        }

        /// <summary>
        /// When the '+' or '-' button is released the Background worker is stopped
        /// </summary>
        private void OnZoomButtonPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_ZoomBackgroundWorker.IsBusy)
            {
                _ZoomBackgroundWorker.CancelAsync();
            }
        }

        /// <summary>
        /// When the '+' or '-' button is released the Background worker is stopped
        /// </summary>
        private void OnZoomTouchUp(object sender, TouchEventArgs e)
        {
            if (_ZoomBackgroundWorker.IsBusy)
            {
                _ZoomBackgroundWorker.CancelAsync();
            }
        }

        /// <summary>
        /// Occurs to delete the zoom in\out and the translate
        /// </summary>
        private void OnRefreshMouseDown(object sender, MouseButtonEventArgs e)
        {
            RefreshUserControl();
        }

        /// <summary>
        /// Occurs when the mouse is moving on the image
        /// </summary>
        private void OnImagePreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (sender is Image image)
            {
                if (image.IsMouseOver)
                {
                    var position = GetImageCoordsAt(image, e);
                    CoordinateMessage = $"X = {position.X} - Y = {position.Y}";
                }
            }
        }

        /// <summary>
        /// Occurs when the mouse is left clicked on the Image
        /// </summary>
        private void OnImagePreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CanChooseCoordinates)
            {
                if (sender is Image image)
                {
                    var position = GetImageCoordsAt(image, e);
                    TriggerDatasetCoordinatesGot(sender, new PointEventArgs(position));
                }
            }
        }

        #endregion
    }
}
