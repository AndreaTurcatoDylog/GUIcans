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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SpeedDetailsImageVisualizer
{
    /// <summary>
    /// Interaction logic for FiltersUserControl.xaml
    /// </summary>
    public partial class ThumbnailsUserControl : UserControl
    {
        #region Properties

        /// <summary>
        /// Get\Set the list of ImageData
        /// </summary>
        public ObservableCollection<ImageData> ImagesData
        {
            get { return (ObservableCollection<ImageData>)GetValue(ImagesDataProperty); }
            set { SetValue(ImagesDataProperty, value); }
        }

        /// <summary>
        /// Get\Set the selected ImageData in the list
        /// </summary>
        public ImageData SelectedImageData
        {
            get { return (ImageData)GetValue(SelectedImageDataProperty); }
            set { SetValue(SelectedImageDataProperty, value); }
        }

        /// <summary>
        /// Get\Set the MaxNumberOfImages
        /// It is the the max number of images shown in the UserControl
        /// </summary>
        public int MaxNumberOfImages
        {
            get { return (int)GetValue(MaxNumberOfImagesProperty); }
            set { SetValue(MaxNumberOfImagesProperty, value); }
        }

        /// <summary>
        /// Get\Set the ThumbnailHeight
        /// </summary>
        public double ThumbnailHeight
        {
            get { return (double)GetValue(ThumbnailHeightProperty); }
            private set { SetValue(ThumbnailHeightProperty, value); }
        }

        #endregion

        #region Dependency Properties

        private static readonly DependencyProperty ImagesDataProperty =
            DependencyProperty.Register("ImagesData", typeof(ObservableCollection<ImageData>), typeof(ThumbnailsUserControl), new PropertyMetadata(null));

        private static readonly DependencyProperty SelectedImageDataProperty =
            DependencyProperty.Register("SelectedImageData", typeof(ImageData), typeof(ThumbnailsUserControl), new PropertyMetadata(null));

        private static readonly DependencyProperty MaxNumberOfImagesProperty =
           DependencyProperty.Register("MaxNumberOfImages", typeof(int), typeof(ThumbnailsUserControl), new PropertyMetadata(1));

        private static readonly DependencyProperty ThumbnailHeightProperty =
           DependencyProperty.Register("ThumbnailHeight", typeof(double), typeof(ThumbnailsUserControl), new PropertyMetadata(0.0));

        #endregion

        #region Constructor

        public ThumbnailsUserControl()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this;
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the size of control changes
        /// </summary>
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                if (ActualHeight > 0)
                {
                    var count = ImagesData?.Count();
                    if (count > 0 && MaxNumberOfImages > 0)
                    {
                        var realHeight = ActualHeight - (11 * (MaxNumberOfImages));
                        ThumbnailHeight = realHeight / MaxNumberOfImages;

                        if (ThumbnailHeight < 1)
                        {
                            ThumbnailHeight = 1;
                        }
                    }
                }
            }
            catch (Exception)
            {
                ThumbnailHeight = 50;
            }
        }

        #endregion
    }
}
