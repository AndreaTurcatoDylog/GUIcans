using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Core
{
    public class ImagePaginationButton: PaginationButton
    {
        #region Properties

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

        #endregion

        #region Dependendcy Properties

        /// <summary>
        /// Defines the image of button
        /// </summary>
        public static readonly DependencyProperty ImageButtonPathProperty =
            DependencyProperty.RegisterAttached("ImageButtonPath", typeof(Geometry), typeof(ImagePaginationButton),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        /// <summary>
        /// Defines the margin of text (content) of button
        /// </summary>
        public static readonly DependencyProperty ImageButtonContentMarginProperty = DependencyProperty.RegisterAttached(
            "ImageButtonContentMargin", typeof(string), typeof(ImagePaginationButton),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        #endregion
    }
}
