using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Core
{
    /// <summary>
    /// Interaction logic for DelayButtonUserControl.xaml
    /// </summary>
    public partial class DelayButtonUserControl : DisposableUserControlBase
    {
        #region Constants

        private readonly string PopUpLoadingMessage = "Hold button";
        private readonly string PopUpTargetReachedMessage = "Release button";

        #endregion

        #region Fields

        /// <summary>
        /// The background worker used for the progress bar
        /// </summary>
        private BackgroundWorker _BackgroundWorker;

        /// <summary>
        /// The button image for the component inserted outside the usercontrol
        /// </summary>
        /// <example>
        /// <common:DelayButtonUserControl>
        //    <common:DelayButtonUserControl.InnerButton>
        //        <common:ImageButton>
        //       </common:ImageButton>
        //    </common:DelayButtonUserControl.InnerButton>
        //  </common:DelayButtonUserControl>
        /// </example>
        private ImageButton _ButtonImage;

        /// <summary>
        /// Used to calculate the step
        /// </summary>
        private double _Max;

        /// <summary>
        /// Used to calculate the step
        /// </summary>
        private int _WaitingTime;

        private Brush[] _Colors;

        #endregion

        #region Event Handler

        public event EventHandler TargetReached;

        #endregion

        #region Properties

        public UIElement InnerButton
        {
            get { return (UIElement)GetValue(InnerButtonProperty); }
            set { SetValue(InnerButtonProperty, value); }
        }

        /// <summary>
        /// The target milliseconds.
        /// This property set the last value of progress bar.
        /// Press button until TargetMilliseconds value to reach the target.
        /// </summary>
        public int TargetMilliseconds
        {
            get { return (int)GetValue(TargetMillisecondsProperty); }
            set { SetValue(TargetMillisecondsProperty, value); }
        }

        /// <summary>
        /// The message of popup
        /// </summary>
        public string PopUpMessage
        {
            get { return (string)GetValue(PopUpMessageProperty); }
            private set { SetValue(PopUpMessageProperty, value); }
        }

        /// <summary>
        /// The color of progress bar. It can changes during the elapsed time
        /// </summary>
        public Brush ProgressBarBrush
        {
            get { return (Brush)GetValue(ProgressBarBrushProperty); }
            private set { SetValue(ProgressBarBrushProperty, value); }
        }

        #endregion

        #region Dependency Properties

        public static DependencyProperty InnerButtonProperty =
            DependencyProperty.Register("InnerButton", typeof(UIElement), typeof(DelayButtonUserControl), new PropertyMetadata(null));

        public static readonly DependencyProperty TargetMillisecondsProperty = DependencyProperty.RegisterAttached(
            "TargetMilliseconds", typeof(int), typeof(DelayButtonUserControl));

        private static readonly DependencyProperty PopUpMessageProperty =
         DependencyProperty.Register("PopUpMessage", typeof(string), typeof(DelayButtonUserControl));


        private static readonly DependencyProperty ProgressBarBrushProperty =
         DependencyProperty.Register("ProgressBarBrush", typeof(Brush), typeof(DelayButtonUserControl));

        #endregion

        #region Constructor

        public DelayButtonUserControl()
        {
            InitializeComponent();

            // Create the background worker
            _BackgroundWorker = new BackgroundWorker();
            _BackgroundWorker.WorkerReportsProgress = true;
            _BackgroundWorker.WorkerSupportsCancellation = true;

            // Register loaded event
            Loaded += OnLoaded;

            // Set the data contex
            LayoutRoot.DataContext = this;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Register\unregister events
        /// </summary>
        protected override void RegisterEvents(bool register)
        {
            if (register)
            {
                _BackgroundWorker.DoWork += OnBackgroundWorkerDoWork;
                _BackgroundWorker.ProgressChanged += OnBackgroundWorkerProgressChanged;

                if (InnerButton != null)
                {
                    _ButtonImage = (ImageButton)InnerButton;
                    _ButtonImage.PreviewMouseLeftButtonDown += OnButtonMouseLeftButtonDown;
                    _ButtonImage.TouchDown += OnButtonImageTouchDown;
                    _ButtonImage.PreviewMouseLeftButtonUp += OnButtonMouseLeftButtonUp;
                    _ButtonImage.TouchUp += OnButtonImageTouchUp;
                    _ButtonImage.PreviewTouchDown += OnButtonImageTouchDown;
                    _ButtonImage.PreviewTouchUp += OnButtonImageTouchUp;
                }
            }
            else
            {
                _BackgroundWorker.DoWork -= OnBackgroundWorkerDoWork;
                _BackgroundWorker.ProgressChanged -= OnBackgroundWorkerProgressChanged;

                if (_ButtonImage != null)
                {
                    _ButtonImage.MouseLeftButtonDown -= OnButtonMouseLeftButtonDown;
                    _ButtonImage.TouchDown -= OnButtonImageTouchDown;
                    _ButtonImage.PreviewTouchDown -= OnButtonImageTouchDown;
                    _ButtonImage.MouseLeftButtonUp -= OnButtonMouseLeftButtonUp;
                    _ButtonImage.TouchUp += OnButtonImageTouchUp;
                    _ButtonImage.PreviewTouchUp -= OnButtonImageTouchUp;
                }
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// The loaded event
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_ButtonImage != null)
            {
                _ButtonImage.IsManipulationEnabled = true;

                DelayProgressBar.Width = _ButtonImage.ActualWidth + 24;
                DelayProgressBar.Height = _ButtonImage.ActualHeight + 24;
            }

            // Laod the gradiant colors
            _Colors = new Brush[3]
            {
                Application.Current.TryFindResource("GradiantDarkRed") as Brush,
                Application.Current.TryFindResource("GradiantOrange") as Brush,
                Application.Current.TryFindResource("GradiantGreen") as Brush,
            };

            ProgressBarBrush = _Colors[0];
        }

        /// <summary>
        ///  The job of back ground worker
        /// </summary>
        private void OnBackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            Stopwatch sw = new Stopwatch();

            sw.Start();
            for (int i = 1; i <= _Max; i++)
            {
                if (_BackgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                (sender as BackgroundWorker).ReportProgress(i);

                Thread.Sleep(_WaitingTime);
            }

            // Debug time elapsed
            sw.Stop();
            var elapsed = sw.Elapsed;
        }

        /// <summary>
        /// Occurs when progress changes
        /// </summary>
        private void OnBackgroundWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            DelayProgressBar.Value = e.ProgressPercentage;

            if (e.ProgressPercentage == DelayProgressBar.Maximum)
            {
                PopUpMessage = $"{PopUpTargetReachedMessage}";
                ProgressBarBrush = _Colors[2];
            }
            else
            {
                PopUpMessage = $"{PopUpLoadingMessage} {e.ProgressPercentage}%";
            }

            // Set the progress bar color
            if (DelayProgressBar.Value == (DelayProgressBar.Maximum / 2) )
            {
                ProgressBarBrush = _Colors[1];
            }
        }

        /// <summary>
        /// Occurs when the button is touched
        /// </summary>
        private void OnButtonImageTouchDown(object sender, TouchEventArgs e)
        {
           // _ButtonImage.IsTouched = true;

            // Progress bar color
            ProgressBarBrush = _Colors[0];

            // Calculate step
            _Max = DelayProgressBar.Maximum;
            _WaitingTime = Convert.ToInt32(TargetMilliseconds / _Max);
            var step = _Max / _WaitingTime;

            // Run Background worker
            if (!_BackgroundWorker.IsBusy)
            {
                DelayProgressBar.Value = 0;
                DelayProgressBar.Visibility = Visibility.Visible;

                // PopUP
                DelayPopUp.IsOpen = true;

                // Run background worker
                _BackgroundWorker.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Occurs when the mouse left button is down on the button
        /// </summary>
        private void OnButtonMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Progress bar color
            ProgressBarBrush = _Colors[0];

            // Calculate step
            _Max = DelayProgressBar.Maximum;
            _WaitingTime = Convert.ToInt32(TargetMilliseconds / _Max);
            var step = _Max / _WaitingTime;

            // Run Background worker
            if (!_BackgroundWorker.IsBusy)
            {
                DelayProgressBar.Value = 0;
                DelayProgressBar.Visibility = Visibility.Visible;

                // PopUP
                DelayPopUp.IsOpen = true;

                // Run background worker
                _BackgroundWorker.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Occurs when the touche in button is release
        /// </summary>
        private void OnButtonImageTouchUp(object sender, TouchEventArgs e)
        {
           // _ButtonImage.IsTouched = false;

            DelayProgressBar.Visibility = Visibility.Hidden;
            DelayPopUp.IsOpen = false;

            _BackgroundWorker.CancelAsync();

            if (DelayProgressBar.Value == DelayProgressBar.Maximum)
            {
                // Raise events
                TargetReached?.Invoke(this, new EventArgs());
            }
        }

        /// <summary>
        /// Occurs when the mouse left button is up on the button
        /// </summary>
        private void OnButtonMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DelayProgressBar.Visibility = Visibility.Hidden;
            DelayPopUp.IsOpen = false;

            _BackgroundWorker.CancelAsync();

            if (DelayProgressBar.Value == DelayProgressBar.Maximum)
            {
                // Raise events
                TargetReached?.Invoke(this, new EventArgs());
            }
        }

        #endregion
    }
}
