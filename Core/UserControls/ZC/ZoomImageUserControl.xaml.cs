using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
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

        // Specify if the Values are setted and are not changed by tranformations
        // The values are: ZoomX, ZoomY, TranslateX, TranslateY.
        bool _AreValuesSetted;

        // 1) Specify if a transformaation (Translate or Zoom) occurs
        // 2) Specify if the mouse is button down
        // This two members works together to fire the ImageMouseDownWithoutTransformation event.
        // The ImageMouseDownWithoutTransformation is fired if a click on image occurs 
        // whithout transformation (Translate or Zoom). The MouseX and MouseY coordinates on Image are
        // returned in parameters
        private bool _TransformationOccurs;
        private bool _IsMouseDown;

        #endregion

        #region Event Handler

        public event EventHandler ImageZoomed;
        public event EventHandler ImageScrolled;
        public event EventHandler DatasetCoordinatesGot;

        // The ImageMouseDownWithoutTransformation is fired if a click on image occurs 
        // whithout transformation (Translate or Zoom). The MouseX and MouseY coordinates on Image are
        // returned in parameters
        public event EventHandler ImageMouseDownWithoutTransformation;

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

        /// <summary>
        /// It is the ScroolViewer
        /// </summary>
        public ScrollViewer ScrollViewer
        {
            get { return (ScrollViewer)GetValue(ScrollViewerProperty); }
            set { SetValue(ScrollViewerProperty, value); }
        }

        /// <summary>
        /// Get\Set the Width of Viewport
        /// </summary>
        public double ViewportWidth
        {
            get { return (double)GetValue(ViewportWidthProperty); }
            set { SetValue(ViewportWidthProperty, value); }
        }

        /// <summary>
        /// Get\Set the Width of Scroolable area
        /// </summary>
        public double ScrollableWidth
        {
            get { return (double)GetValue(ScrollableWidthProperty); }
            set { SetValue(ScrollableWidthProperty, value); }
        }

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
        /// Get\Set the list of results image drawn in canvas
        /// </summary>
        public ObservableCollection<IImageResult> ResultsImage
        {
            get { return (ObservableCollection<IImageResult>)GetValue(ResultsImageProperty); }
            set { SetValue(ResultsImageProperty, value); }
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
        /// Get\Set the Is Zoom Enabled.
        /// The Zoom is enabled if the ImageSource is not null (an image is loaded)
        /// </summary>
        public bool IsZoomEnabled
        {
            get { return (bool)GetValue(IsZoomEnabledProperty); }
            set { SetValue(IsZoomEnabledProperty, value); }
        }

        /// <summary>
        /// Get\Set the Is BorderEffect Visible.
        /// Specif whether the Border effect is visible or not
        /// </summary>
        public bool IsBorderEffectVisible
        {
            get { return (bool)GetValue(IsBorderEffectVisibleProperty); }
            set { SetValue(IsBorderEffectVisibleProperty, value); }
        }

        // It is the value of pixel X-Coordinate when mouse is down
        public double MouseOnImageCoordinateX { get; private set; }

        // It is the value of pixel Y-Coordinate when mouse is down
        public double MouseOnImageCoordinateY { get; private set; }

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
                new PropertyMetadata());

        private static readonly DependencyProperty ScalarZoomXProperty =
            DependencyProperty.Register("ScalarZoomX", typeof(double), typeof(ZoomImageUserControl), new PropertyMetadata(1.0));

        private static readonly DependencyProperty ScalarZoomYProperty =
           DependencyProperty.Register("ScalarZoomY", typeof(double), typeof(ZoomImageUserControl), new PropertyMetadata(1.0));

        private static readonly DependencyProperty HorizontalRulersVisibilityProperty =
            DependencyProperty.Register("HorizontalRulersVisibility", typeof(Visibility), typeof(ZoomImageUserControl), new PropertyMetadata(Visibility.Visible));

        private static readonly DependencyProperty VerticalRulersVisibilityProperty =
            DependencyProperty.Register("VerticalRulersVisibility", typeof(Visibility), typeof(ZoomImageUserControl), new PropertyMetadata(Visibility.Visible));

        private static readonly DependencyProperty ResultsImageProperty =
           DependencyProperty.Register("ResultsImage", typeof(ObservableCollection<IImageResult>), typeof(ZoomImageUserControl));

        private static readonly DependencyProperty HorizontalRuleMarginProperty =
           DependencyProperty.Register("HorizontalRuleMargin", typeof(string), typeof(ZoomImageUserControl));

        private static readonly DependencyProperty VerticalRuleMarginProperty =
           DependencyProperty.Register("VerticalRuleMargin", typeof(string), typeof(ZoomImageUserControl));

        private static readonly DependencyProperty ResultImagesMarginProperty =
          DependencyProperty.Register("ResultImagesMargin", typeof(string), typeof(ZoomImageUserControl));

        private static readonly DependencyProperty RefreshProperty =
          DependencyProperty.Register("Refresh", typeof(bool), typeof(ZoomImageUserControl),
        new PropertyMetadata(OnRefreshRequestCallBack));

        private static readonly DependencyProperty IsZoomEnabledProperty =
         DependencyProperty.Register("IsZoomEnabled", typeof(bool), typeof(ZoomImageUserControl));

        private static readonly DependencyProperty IsBorderEffectVisibleProperty =
          DependencyProperty.Register("IsBorderEffectVisible", typeof(bool), typeof(ZoomImageUserControl), new PropertyMetadata(true));

        private static readonly DependencyProperty ScrollViewerProperty =
         DependencyProperty.Register("ScrollViewer", typeof(ScrollViewer), typeof(ZoomImageUserControl));

        private static readonly DependencyProperty ViewportWidthProperty =
            DependencyProperty.Register("ViewportWidth", typeof(double), typeof(ZoomImageUserControl),
                new UIPropertyMetadata((d, e) =>
                {
                    var self = (ZoomImageUserControl)d;
                    self.ViewportWidth = self.ImageScrollViewer.ViewportWidth;
                }));

        private static readonly DependencyProperty ScrollableWidthProperty =
            DependencyProperty.Register("ScrollableWidth", typeof(double), typeof(ZoomImageUserControl),
                new UIPropertyMetadata((d, e) =>
                {
                    var self = (ZoomImageUserControl)d;
                    self.ScrollableWidth = self.ImageScrollViewer.ScrollableWidth;
                }));

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

            ScrollViewer = ImageScrollViewer;
            RenderOptions.SetBitmapScalingMode(ImageResult, BitmapScalingMode.HighQuality);

            // Set the DataContext
            LayoutRoot.DataContext = this;
        }

        #endregion

        #region Methods       

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

                MainImage.ManipulationStarting += OnMainImageManipulationStarting;
                MainImage.ManipulationDelta += OnMainImageManipulationDelta;
                MainImage.PreviewTouchDown += OnMainImagePreviewTouchDown;
                MainImage.PreviewTouchUp += OnMainImagePreviewTouchUp;
                MainImage.MouseMove += OnMainImageMouseMove;

                ZoomOutButton.Click += OnZoomOutButtonClick;
                ZoomInButton.Click += OnZoomInButtonClick;

                //ZoomOutButton.GotMouseCapture += ButtonGotMouseCapture;               
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
                ImageScrollViewer.ManipulationDelta -= OnMainImageManipulationDelta;

                MainImage.ManipulationStarting -= OnMainImageManipulationStarting;
                MainImage.ManipulationDelta -= OnMainImageManipulationDelta;
                MainImage.PreviewTouchDown -= OnMainImagePreviewTouchDown;
                MainImage.PreviewTouchUp -= OnMainImagePreviewTouchUp;
                MainImage.MouseMove -= OnMainImageMouseMove;

                ZoomOutButton.Click -= OnZoomOutButtonClick;
                ZoomInButton.Click -= OnZoomInButtonClick;

                RefreshButton.PreviewMouseDown -= OnRefreshMouseDown;
                _ZoomBackgroundWorker.DoWork -= OnZoomBackgroundWorkerDoWork;
            }
        }

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
            double offsetX = 0;
            double offsetY = 0;

            if (MainImage != null && ImageScrollViewer != null)
            {
                try
                {
                    // Get the offset
                    var imageWidth = MainImage.ActualWidth * ScalarZoomX;
                    var imageHeight = MainImage.ActualHeight * ScalarZoomY;

                    var viewPortWidth = ImageScrollViewer.ViewportWidth;
                    var viewPortHeight = ImageScrollViewer.ViewportHeight;

                    offsetX = (viewPortWidth / 2) - (imageWidth / 2);
                    offsetY = (viewPortHeight / 2) - (imageHeight / 2);
                }
                catch (Exception ex)
                {
                    offsetX = 0;
                    offsetY = 0;

                    CoreLog.Instance.Append(ex.Message, CoreLogType.Error);
                }
            }

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
        }

        /// <summary>
        /// Translate the result images canvas and the vertical and horizontal rules in according to main image position
        /// </summary>
        private void TranslateComponents()
        {
            // Get the offset
            var offset = GetOffset();

            if (offset != null)
            {
                // Translate the horizontal rule
                var marginX = (MainImage?.DesiredSize.Width < ImageScrollViewer?.ViewportWidth) ? Convert.ToInt32(offset.Item1) : 0;
                HorizontalRuleMargin = $"{marginX},0,0,0";

                // Translate the vertical rule
                var marginY = (MainImage.DesiredSize.Height < ImageScrollViewer.ViewportHeight) ? Convert.ToInt32(offset.Item2) : 0;
                VerticalRuleMargin = $"0,{marginY},0,0";

                // Translate the result images canvas
                // ResultImagesMargin = $"{marginX},{marginY},0,0";
            }
        }

        /// <summary>
        /// Refresh the component
        /// </summary>
        private void RefreshUserControl()
        {
            if (MainImage != null && ImageScrollViewer != null)
            {
                ScalarZoomX = ScalarZoomY = 1;
                RenderOptions.SetBitmapScalingMode(ImageResult, BitmapScalingMode.HighQuality);

                ImageScrollViewer.ScrollToTop();
                ImageScrollViewer.ScrollToLeftEnd();
                TranslateComponents();
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
        /// Get the values:
        /// ZoomX, ZoomY, TranlsateX, TranslateY
        /// </summary>
        /// <returns></returns>
        public ZoomImageControlValues GetZoomValues()
        {
            if (ImageScrollViewer != null)
            {
                return new ZoomImageControlValues(ScalarZoomX, ScalarZoomY, ImageScrollViewer.HorizontalOffset, ImageScrollViewer.VerticalOffset);
            }

            return null;
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
                    //_MinWidthValue = MainImage.DesiredSize.Width;
                    //_MinHeightValue = MainImage.DesiredSize.Height;
                    _MinWidthValue = MainImage.ActualWidth;
                    _MinHeightValue = MainImage.ActualHeight;
                }

                if (zoomMode == ZoomMode.ZoomOut)
                {
                    ScalarZoomX = ScalarZoomX * ZoomValue;
                    ScalarZoomY = ScalarZoomY * ZoomValue;

                    _TransformationOccurs = true;
                }
                else if (zoomMode == ZoomMode.ZoomIn)
                {
                    if (ZoomValue > 0)
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
                    else
                    {
                        RefreshUserControl();
                    }

                    _TransformationOccurs = true;
                }

                // Change the BitmapScalingMode in according to zoom value
                if (ScalarZoomX <= 1.2 || ScalarZoomY <= 1.2)
                {
                    RenderOptions.SetBitmapScalingMode(ImageResult, BitmapScalingMode.HighQuality);
                }
                else
                {
                    RenderOptions.SetBitmapScalingMode(ImageResult, BitmapScalingMode.NearestNeighbor);
                }
            }
        }

        /// <summary>
        /// Set the values:
        /// ZoomX, ZoomY, TranslateX, TranslateY.
        /// </summary>
        public void SetValues(double zoomX, double zoomY, double translatedOffesetX, double translatedOffesetY)
        {
            _AreValuesSetted = true;

            ScalarZoomX = zoomX;
            ScalarZoomY = zoomY;

            ImageScrollViewer.ScrollToBottom();
            ImageScrollViewer.ScrollToLeftEnd();

            var x = translatedOffesetX;
            ImageScrollViewer.ScrollToHorizontalOffset(translatedOffesetX);
            ImageScrollViewer.ScrollToVerticalOffset(translatedOffesetY);

            // Change the BitmapScalingMode in according to zoom value
            if (ScalarZoomX <= 1.2 || ScalarZoomY <= 1.2)
            {
                RenderOptions.SetBitmapScalingMode(ImageResult, BitmapScalingMode.HighQuality);
            }
            else
            {
                RenderOptions.SetBitmapScalingMode(ImageResult, BitmapScalingMode.NearestNeighbor);
            }
        }

        /// <summary>
        /// Set the values:
        /// 1) ZoomX, ZoomY.
        /// 2) TranslateX, TranslateY.
        /// </summary>
        public void SetValues(ZoomImageControlValues zoomImageControlValues)
        {
            if (zoomImageControlValues != null)
            {
                SetValues(zoomImageControlValues.ZoomX, zoomImageControlValues.ZoomY, zoomImageControlValues.TranslatedOffesetX, zoomImageControlValues.TranslatedOffesetY);
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
                // await Task.Delay(TimeSpan.FromSeconds(1.0));
            }
        }

        /// <summary>
        /// Occurs when left button mouse is up on the scrool viewer with image inside
        /// </summary>
        private void OnImageScrollViewerMouseUp(object sender, MouseButtonEventArgs e)
        {
            _LastDragPoint = null;
            //ImageScrollViewer.Cursor = Cursors.Arrow;
            ImageScrollViewer.Cursor = Cursors.Hand;
            ImageScrollViewer.ReleaseMouseCapture();

            if (_IsImageTranslated)
            {
                // Restore position after the image is translated Up when the user wanted 
                // to see details covered by Buttons
                MainImage.RenderTransform = null;
                ImageResult.RenderTransform = null;
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

            if (!_TransformationOccurs && _IsMouseDown)
            {
                // Fire the ImageMouseDownWithoutTransformation event
                ImageMouseDownWithoutTransformation?.Invoke(this, new ZoomImagePositionValuesEventArgs(MouseOnImageCoordinateX, MouseOnImageCoordinateY));
            }

            _TransformationOccurs = false;
            _IsMouseDown = false;
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
                        ImageResult.RenderTransform = translateTransform;
                    }

                    ImageScrollViewer.ScrollToHorizontalOffset(ImageScrollViewer.HorizontalOffset - dX);
                    ImageScrollViewer.ScrollToVerticalOffset(ImageScrollViewer.VerticalOffset - dY);

                    _TransformationOccurs = true;
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
            if (ImageSource != null && MainImage.ActualWidth > 0 && MainImage.ActualHeight > 0)
            {
                var mousePosition = e.GetPosition((IInputElement)sender);
                if (mousePosition.X <= ImageScrollViewer.ViewportWidth && mousePosition.Y < ImageScrollViewer.ViewportHeight)
                {
                    _LastDragPoint = mousePosition;
                    Mouse.Capture(ImageScrollViewer);

                    // Store the pixel coordinate by mouse down on Main Image
                    var proportionalX = ImageSource.Width / MainImage.ActualWidth;
                    var proportionalY = ImageSource.Height / MainImage.ActualHeight;

                    var imageMousePosition = e.GetPosition(MainImage);
                    MouseOnImageCoordinateY = imageMousePosition.X * proportionalX;
                    MouseOnImageCoordinateY = imageMousePosition.Y * proportionalY;

                    // Adjust the Coordinates if are oute of Image dimensione range
                    if (MouseOnImageCoordinateX < 0)
                    {
                        MouseOnImageCoordinateX = 0;
                    }
                    else if (MouseOnImageCoordinateX > ImageSource.Width)
                    {
                        MouseOnImageCoordinateX = ImageSource.Width;
                    }

                    if (MouseOnImageCoordinateY < 0)
                    {
                        MouseOnImageCoordinateY = 0;
                    }
                    else if (MouseOnImageCoordinateY > ImageSource.Height)
                    {
                        MouseOnImageCoordinateY = ImageSource.Height;
                    }

                    //Console.WriteLine($"--------------------------------------------------------------------------");
                    //Console.WriteLine($"imageMousePosition.Y = {imageMousePosition.Y}- MouseOnImageCoordinateY = {MouseOnImageCoordinateY}");
                    //Console.WriteLine($"MouseOnImageCoordinateY {MouseOnImageCoordinateY} - ZoomValue: {ZoomValue}");
                    //Console.WriteLine($"--------------------------------------------------------------------------");

                    _TransformationOccurs = false;
                    _IsMouseDown = true;
                }
            }
        }

        /// <summary>
        /// Occurs when the scrool bar of scrool viewer changes its value
        /// </summary>
        private void OnImageScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            double XMousePositionOnScrollViewer = 0;
            double YMousePositionOnScrollViewer = 0;

            if (sender is ScrollViewer scrollViewer && !_AreValuesSetted)
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

                if (oldExtentWidth <= 0)
                {
                    oldExtentWidth = 1;
                }

                if (oldExtentHeight <= 0)
                {
                    oldExtentHeight = 1;
                }

                var relx = offsetX / oldExtentWidth;
                var rely = offsetY / oldExtentHeight;

                offsetX = Math.Max(relx * e.ExtentWidth - XMousePositionOnScrollViewer, 0);
                offsetY = Math.Max(rely * e.ExtentHeight - YMousePositionOnScrollViewer, 0);

                scrollViewer.ScrollToHorizontalOffset(offsetX);
                scrollViewer.ScrollToVerticalOffset(offsetY);

                // Raise events
                ImageScrolled?.Invoke(this, EventArgs.Empty);
            }

            _AreValuesSetted = false;
        }

        /// <summary>
        /// Occurs when the Main Image is touch up
        /// </summary>
        private void OnMainImagePreviewTouchUp(object sender, TouchEventArgs e)
        {
            lock (_Mutex)
            {
                _NumberOfTouchPoints--;
            }

            if (_IsImageTranslated)
            {
                MainImage.RenderTransform = null;
                _IsImageTranslated = false;
                _TranslatedOffesetY = 0;
                ImageScrollViewer.ScrollToBottom();
            }
        }

        /// <summary>
        /// Occurs when the Main Image is touch down
        /// </summary>
        private void OnMainImagePreviewTouchDown(object sender, TouchEventArgs e)
        {
            lock (_Mutex)
            {
                _NumberOfTouchPoints++;
            }
        }

        /// <summary>
        /// Occurs when the Manipulation in Main Image starting
        /// </summary>
        private void OnMainImageManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            e.ManipulationContainer = ImageScrollViewer;
            e.Handled = true;
        }

        /// <summary>
        /// Occurs when the user pinch the canvas with fingers. It is produce zoom in and zoom out
        /// </summary>
        private void OnMainImageManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            int numberOfPoints = 0;

            lock (_Mutex)
            {
                numberOfPoints = _NumberOfTouchPoints;
            }

            if (numberOfPoints >= 2)
            {
                // Perform the Zoom
                var zoomMode = (e?.DeltaManipulation?.Scale.X < 1 || e?.DeltaManipulation?.Scale.Y < 1) ? ZoomMode.ZoomIn : ZoomMode.ZoomOut;
                PerformZoom(zoomMode);

                e.Handled = true;
            }
            else
            {
                // Perform Zoom if needed
                if (e?.DeltaManipulation?.Scale.X != 1 || e?.DeltaManipulation?.Scale.Y != 1)
                {
                    var zoomMode = (e.DeltaManipulation.Scale.X < 1 || e.DeltaManipulation.Scale.Y < 1) ? ZoomMode.ZoomIn : ZoomMode.ZoomOut;
                    PerformZoom(zoomMode);
                }

                // Perform extra Translation in Y axis if needed
                var dY = e.DeltaManipulation.Translation.Y;
                if (ImageScrollViewer.VerticalOffset == ImageScrollViewer.ScrollableHeight && dY < 0)
                {
                    _TranslatedOffesetY = _TranslatedOffesetY - dY;
                    TranslateTransform translateTransform = new TranslateTransform(0, -_TranslatedOffesetY);
                    MainImage.RenderTransform = translateTransform;
                    //ImageResultCanvas.RenderTransform = translateTransform;

                    _IsImageTranslated = true;
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
        /// Occurs when the user make a double click.
        /// It delete the zoom in\out and the translate
        /// </summary>
        private void OnImageScrollViewerPreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //RefreshUserControl();
        }

        /// <summary>
        /// Occurs when the user clicks the button to the zoom Out
        /// </summary>       
        private void OnZoomOutButtonClick(object sender, RoutedEventArgs e)
        {
            PerformZoom(ZoomMode.ZoomOut);
        }

        /// <summary>
        /// Occurs when the user clicks the button to the zoom Out
        /// </summary>       
        private void OnZoomInButtonClick(object sender, RoutedEventArgs e)
        {
            PerformZoom(ZoomMode.ZoomIn);
        }

        /// <summary>
        /// Ocurs when a button got the Mouse Capture
        /// </summary>
        private void ButtonGotMouseCapture(object sender, MouseEventArgs e)
        {
            if (sender is Button button)
            {
                button.ReleaseMouseCapture();
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
