using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Core
{
    /// <summary>
    /// This component rappresent the button that paginate the components
    /// </summary>
    public class PaginationButton : RadioButton, IPaginationButton
    {
        #region Properties

        /// <summary>
        /// Get\Set With of the component
        /// </summary>
        [Bindable(true)]
        public double PaginationWidth
        {
            get
            {
                return (double)GetValue(PaginationWithProperty);
            }
            set
            {
                SetValue(PaginationWithProperty, value);
            }
        }

        /// <summary>
        /// Get\Set Height of the component
        /// </summary>
        [Bindable(true)]
        public double PaginationHeight
        {
            get
            {
                return (double)GetValue(PaginationHeightProperty);
            }
            set
            {
                SetValue(PaginationHeightProperty, value);
            }
        }

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty PaginationWithProperty =
            DependencyProperty.RegisterAttached("PaginationWith", typeof(double), typeof(PaginationButton),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        public static readonly DependencyProperty PaginationHeightProperty =
           DependencyProperty.RegisterAttached("PaginationHeight", typeof(double), typeof(PaginationButton),
               new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        #endregion

        #region Constructor

        static PaginationButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PaginationButton), new FrameworkPropertyMetadata(typeof(PaginationButton)));
        }

        public PaginationButton()
        { }

        #endregion
    }
}
