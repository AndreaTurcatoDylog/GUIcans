﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Core
{
    /// <summary>
    /// Interaction logic for SpinningProgressBar.xaml
    /// The spinner start the animation when its visibility changes
    /// </summary>
    public partial class SpinningProgressBar : DisposableUserControlBase
    {
        #region Members

        private readonly DispatcherTimer _AnimationTimer;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the minimum.
        /// </summary>
        /// <value>The minimum.</value>
        public int Minimum
        {
            get { return (int)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        /// <summary>
        /// Gets or sets the maximum.
        /// </summary>
        /// <value>The maximum.</value>
        public int Maximum
        {
            get { return (int)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Get\Set the Text shown on the right of the spinning 
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Get\Set the Spinner Color
        /// </summary>
        public Brush SpinnerColor
        {
            get { return (Brush)GetValue(SpinnerColorProperty); }
            set { SetValue(SpinnerColorProperty, value); }
        }

        /// <summary>
        /// Get\Set the Text Color
        /// </summary>
        public Brush TextColor
        {
            get { return (Brush)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        /// <summary>
        /// Get\Set the Milliseconds of the animation of the Spinning
        /// </summary>
        public int Milliseconds
        {
            get { return (int)GetValue(MillisecondsProperty); }
            set { SetValue(MillisecondsProperty, value); }
        }

        /// <summary>
        /// Get\Set the Dimension of the Spinninn
        /// </summary>
        public double Dimension
        {
            get { return (double)GetValue(DimensionProperty); }
            set { SetValue(DimensionProperty, value); }
        }

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty MinimumProperty =
           DependencyProperty.Register("Minimum", typeof(int), typeof(SpinningProgressBar), new UIPropertyMetadata(1));

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(int), typeof(SpinningProgressBar), new UIPropertyMetadata(1));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int), typeof(SpinningProgressBar), new UIPropertyMetadata(100));

        public static readonly DependencyProperty TextProperty =
           DependencyProperty.Register("Text", typeof(string), typeof(SpinningProgressBar), new UIPropertyMetadata(string.Empty));

        public static readonly DependencyProperty SpinnerColorProperty =
           DependencyProperty.Register("SpinnerColor", typeof(Brush), typeof(SpinningProgressBar), new UIPropertyMetadata(Brushes.Red));

        public static readonly DependencyProperty TextColorProperty =
           DependencyProperty.Register("TextColor", typeof(Brush), typeof(SpinningProgressBar), new UIPropertyMetadata(Brushes.Black));

        public static readonly DependencyProperty MillisecondsProperty =
          DependencyProperty.Register("Milliseconds", typeof(int), typeof(SpinningProgressBar), new UIPropertyMetadata(300));

        public static readonly DependencyProperty DimensionProperty =
          DependencyProperty.Register("Dimension", typeof(double), typeof(SpinningProgressBar), new UIPropertyMetadata(40.0));

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CircularProgressBar"/> class.
        /// </summary>
        public SpinningProgressBar()
        {
            InitializeComponent();

            LayoutRoot.DataContext = this;

            _AnimationTimer = new DispatcherTimer(DispatcherPriority.ContextIdle, Dispatcher)
            {
                Interval = new TimeSpan(0, 0, 0, 0, Milliseconds)
            };

            Start();
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
                _AnimationTimer.Tick += OnAnimationTick;
                IsVisibleChanged += OnVisibleChanged;
            }
            else
            {
                _AnimationTimer.Tick -= OnAnimationTick;
                IsVisibleChanged -= OnVisibleChanged;
            }
        }

        /// <summary>
        /// Sets the position.
        /// </summary>
        /// <param name="ellipse">The ellipse.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="posOffSet">The pos off set.</param>
        /// <param name="step">The step to change.</param>
        private static void SetPosition(DependencyObject ellipse, double offset, double posOffSet, double step)
        {
            ellipse.SetValue(Canvas.LeftProperty, 50 + (Math.Sin(offset + (posOffSet * step)) * 50));
            ellipse.SetValue(Canvas.TopProperty, 50 + (Math.Cos(offset + (posOffSet * step)) * 50));
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        private void Start()
        {
            _AnimationTimer.Start();
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        private void Stop()
        {
            _AnimationTimer.Stop();
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles the animation tick.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnAnimationTick(object sender, EventArgs e)
        {
            _spinnerRotate.Angle = (_spinnerRotate.Angle + 36) % 360;
        }

        /// <summary>
        /// Handles the loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnCanvasLoaded(object sender, RoutedEventArgs e)
        {
            const double offset = Math.PI;
            const double step = Math.PI * 2 / 10.0;

            SetPosition(_circle0, offset, 0.0, step);
            SetPosition(_circle1, offset, 1.0, step);
            SetPosition(_circle2, offset, 2.0, step);
            SetPosition(_circle3, offset, 3.0, step);
            SetPosition(_circle4, offset, 4.0, step);
            SetPosition(_circle5, offset, 5.0, step);
            SetPosition(_circle6, offset, 6.0, step);
            SetPosition(_circle7, offset, 7.0, step);
            SetPosition(_circle8, offset, 8.0, step);
        }

        /// <summary>
        /// Handles the unloaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnCanvasUnloaded(object sender, RoutedEventArgs e)
        {
            Stop();
        }

        /// <summary>
        /// Handles the visible changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var isVisible = (bool)e.NewValue;

            if (isVisible)
            {
                Start();
            }
            else
            {
                Stop();
            }
        }

        #endregion
    }
}
