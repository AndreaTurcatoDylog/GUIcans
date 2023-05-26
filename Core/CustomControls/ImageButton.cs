using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Core
{
    public class ImageButton : Button
    {
        #region Field

        Window _ParentWindow;

        #endregion

        #region Properties

        /// <summary>
        /// Get\Set Is Touched
        /// </summary>
        [Bindable(true)]
        public bool IsTouched
        {
            get
            {
                return (bool)GetValue(IsTouchedProperty);
            }
            set
            {
                SetValue(IsTouchedProperty, value);
            }
        }

        /// <summary>
        /// Get\Set ButtonPath
        /// </summary>
        [Bindable(true)]
        public Geometry ImageButtonPath
        {
            get
            {
                return (Geometry)GetValue(ImageButtonPathProperty);
            }
            set
            {
                SetValue(ImageButtonPathProperty, value);
            }
        }

        /// <summary>
        /// Get\Set the Image button content margin
        /// </summary>
        [Bindable(true)]
        public string ImageButtonContentMargin
        {
            get { return (string)GetValue(ImageButtonContentMarginProperty); }
            set { SetValue(ImageButtonContentMarginProperty, value); }
        }

        /// <summary>
        /// Get\Set the button result
        /// </summary>
        [Bindable(true)] 
        public ButtonResult ButtonResult
        {
            get { return (ButtonResult)GetValue(ButtonResultProperty); }
            set { SetValue(ButtonResultProperty, value); }
        }

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Defines the image of button
        /// </summary>
        public static readonly DependencyProperty IsTouchedProperty =
            DependencyProperty.RegisterAttached("IsTouched", typeof(bool), typeof(ImageButton),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        /// <summary>
        /// Defines the image of button
        /// </summary>
        public static readonly DependencyProperty ImageButtonPathProperty =
            DependencyProperty.RegisterAttached("ImageButtonPath", typeof(Geometry), typeof(ImageButton),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        /// <summary>
        /// Defines the margin of text (content) of button
        /// </summary>
        public static readonly DependencyProperty ImageButtonContentMarginProperty = DependencyProperty.RegisterAttached(
            "ImageButtonContentMargin", typeof(string), typeof(ImageButton),
                new FrameworkPropertyMetadata("0,0,0,0", FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        /// <summary>
        /// Defines result of button (default none)
        /// </summary>
        public static readonly DependencyProperty ButtonResultProperty = DependencyProperty.RegisterAttached(
            "ButtonResult", typeof(ButtonResult), typeof(ImageButton),
                new FrameworkPropertyMetadata(ButtonResult.None, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        #endregion

        #region Constructor

        static ImageButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageButton), new FrameworkPropertyMetadata(typeof(ImageButton)));
        }

        public ImageButton()
        {
            Loaded += OnLoaded;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the parent window
        /// </summary>
        private Window GetParentWindow(DependencyObject child)
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null)
            {
                return null;
            }

            var parent = parentObject as Window;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                return GetParentWindow(parentObject);
            }
        }

        /// <summary>
        /// Register\Unregister the events
        /// </summary>
        private void RegisterEvents(bool register)
        {
            if (_ParentWindow != null)
            {
                if (register)
                {
                    _ParentWindow.Closing += ParentWindowClosing;
                    TouchDown += ImageButton_TouchDown;
                    TouchUp += OnImageButtonTouchUp;
                }
                else
                {
                    Loaded -= OnLoaded;
                    _ParentWindow.Closing -= ParentWindowClosing;
                    TouchDown += ImageButton_TouchDown;
                    TouchUp += OnImageButtonTouchUp;
                }
            }
            else
            {

            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the Image Button is loaded
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (this.Template != null)
            {
                _ParentWindow = GetParentWindow(this);
            }

            RegisterEvents(true);
        }

        /// <summary>
        /// Occurs when the button is touched down
        /// </summary>
        private void ImageButton_TouchDown(object sender, TouchEventArgs e)
        {
            IsTouched = true;
        }

        /// <summary>
        /// Occurs when the button is touched up
        /// </summary>
        private void OnImageButtonTouchUp(object sender, TouchEventArgs e)
        {
            IsTouched = false;
        }


        /// <summary>
        /// Occurs when the parent windows ia closing
        /// </summary>
        private void ParentWindowClosing(object sender, CancelEventArgs e)
        {
            RegisterEvents(false);
        }

        #endregion
    }
}
