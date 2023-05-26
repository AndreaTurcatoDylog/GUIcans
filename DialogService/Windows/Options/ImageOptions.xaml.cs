using Business;
using Core;
using Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DialogService
{
    /// <summary>
    /// Interaction logic for ImageOptions.xaml
    /// </summary>
    public partial class ImageOptions : DisposableUserControl
    {
        #region Members

        private IList<string> _Formats;
        private bool _IsInitializing;

        #endregion

        #region Properties

        /// <summary>
        /// Get\Set The Perform Rotation. Specify whether the perform the rotation or not
        /// </summary>
        public bool PerformRotation
        {
            get { return (bool)GetValue(PerformRotationProperty); }
            set { SetValue(PerformRotationProperty, value); }
        }       

        /// <summary>
        /// Get\Set theOptions
        /// </summary>
        public Options Options
        {
            get { return (Options)GetValue(OptionsProperty); }
            set { SetValue(OptionsProperty, value); }
        }

        /// <summary>
        /// Get\Set the Rotation Values
        /// </summary>
        public ObservableCollection<bool> RotationValues 
        {
            get { return (ObservableCollection<bool>)GetValue(RotationValuesProperty); }
            set { SetValue(RotationValuesProperty, value); }
        }

        /// <summary>
        /// Get\Set the Image Filters
        /// </summary>
        public ObservableCollection<bool> CheckedImageList
        {
            get { return (ObservableCollection<bool>)GetValue(CheckedImageListProperty); }
            set { SetValue(CheckedImageListProperty, value); }
        }

        #endregion

        #region Dependency Properties

        private static readonly DependencyProperty PerformRotationProperty =
          DependencyProperty.Register("PerformRotation", typeof(bool), typeof(ImageOptions), new PropertyMetadata());

        private static readonly DependencyProperty OptionsProperty =
            DependencyProperty.Register("Options", typeof(Options), typeof(ImageOptions), new PropertyMetadata());

        private static readonly DependencyProperty CheckedImageListProperty =
            DependencyProperty.Register("CheckedImageList", typeof(ObservableCollection<bool>), typeof(ImageOptions), new PropertyMetadata());

        private static readonly DependencyProperty RotationValuesProperty =
            DependencyProperty.Register("RotationValues", typeof(ObservableCollection<bool>), typeof(ImageOptions), new PropertyMetadata());

        #endregion

        #region Commands

        /// <summary>
        /// The Changed Filter  Command
        /// </summary>
        private ICommand _ChangeFilterCommand;
        public ICommand ChangeFilterCommand
        {
            get { return _ChangeFilterCommand; }
            set { _ChangeFilterCommand = value; }
        }

        /// <summary>
        /// The Selected Rotation Command
        /// </summary>
        private ICommand _SelectedRotationCommand;
        public ICommand SelectedRotationCommand
        {
            get { return _SelectedRotationCommand; }
            set { _SelectedRotationCommand = value; }
        }

        #endregion

        #region Constructor

        public ImageOptions()
        {
            InitializeComponent();

            // Create the Imge fileFormats
            _Formats = new List<string> { ".jpg" ,".tif", ".bmp", ".png", ".16" };

            // Create the Checked Image List
            CheckedImageList = new ObservableCollection<bool>()
            {
                false, false, false, false, false
            };

            // Create the Rotation values
            RotationValues = new ObservableCollection<bool>()
            {
                false, false
            };
            
            // Set the DataContext
            LayoutRoot.DataContext = this;            
        }

        #endregion

        #region Methods

        /// <summary>
        /// The Load code of the User Control
        /// </summary>
        protected override void Load()
        {
            Initialize();
        }

        /// <summary>
        /// Initialize the Settings
        /// </summary>
        private void Initialize()
        {
            _IsInitializing = true;

            if (Options != null && _Formats != null)
            {
                // Set the Image Filters                              
                for (var index = 0; index < 5; index++)
                {
                    var isEnabled = Options.Settings.ImageFormats.Contains(_Formats[index]);
                    CheckedImageList[index] = isEnabled;
                }

                // Set the Rotation Values 
                PerformRotation = (Options.Settings.RotationAngle == 90.0);               
            }

            _IsInitializing = false;
        }

        #endregion

        #region Events       

        /// <summary>
        /// Occurs when an Image Filter is Checked
        /// </summary>
        private void OnImageFilterChecked(object sender, RoutedEventArgs e)
        {
            if (!_IsInitializing)
            {
                if (sender is ToggleButton toggleButton && toggleButton.Tag is string extension)
                {
                    if (!string.IsNullOrEmpty(extension) && !Options.Settings.ImageFormats.Contains(extension))
                    {
                        Options.Settings.ImageFormats.Add(extension);
                    }
                }
            }
        }

        /// <summary>
        /// Occurs when an Image Filter is Unchecked
        /// </summary>
        private void OnImageFilterUnchecked(object sender, RoutedEventArgs e)
        {
            if (!_IsInitializing)
            {
                if (sender is ToggleButton toggleButton && toggleButton.Tag is string extension)
                {
                    var isChecked = toggleButton.IsChecked.Value;
                    if (!string.IsNullOrEmpty(extension) && Options.Settings.ImageFormats.Contains(extension))
                    {
                        Options.Settings.ImageFormats.Remove(extension);
                    }
                }
            }
        }

        /// <summary>
        /// Occurs when the rotation is cChecked
        /// </summary>
        private void OnRotationChecked(object sender, RoutedEventArgs e)
        {
            if (!_IsInitializing)
            {
                try
                {
                    if (sender is ToggleButton radioButton)
                    {                        
                        Options.Settings.RotationAngle = 90.0;
                    }
                }
                catch(Exception)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Occurs when the rotation is Unchecked
        /// </summary>
        private void OnRotationUnchecked(object sender, RoutedEventArgs e)
        {
            if (!_IsInitializing)
            {
                try
                {
                    if (sender is ToggleButton radioButton)
                    {
                        Options.Settings.RotationAngle = 0.0;
                    }
                }
                catch (Exception)
                {
                    return;
                }
            }
        }

        #endregion        
    }
}
