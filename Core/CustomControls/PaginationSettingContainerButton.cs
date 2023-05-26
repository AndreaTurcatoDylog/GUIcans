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
    public class PaginationSettingContainerButton : RadioButton
    {
        #region Properties

        /// <summary>
        /// Get\Set the Title text
        /// Rappresent the Title visible on the screen inside the button
        /// </summary>
        [Bindable(true)]
        public string TitleText
        {
            get
            {
                return (string)GetValue(TitleTextProperty);
            }
            set
            {
                SetValue(TitleTextProperty, value);
            }
        }

        /// <summary>
        /// Get\Set the reject found
        /// True: if almost one element in the associated list has found a reject item
        /// </summary>
        [Bindable(true)]
        public bool IsRejectFound
        {
            get
            {
                return (bool)GetValue(IsRejectFoundProperty);
            }
            set
            {
                SetValue(IsRejectFoundProperty, value);
            }
        }

        /// <summary>
        /// Get\Set the is element modified.
        /// True: if almost one element in the associated list is modified
        /// </summary>
        [Bindable(true)]
        public bool IsElementModified
        {
            get
            {
                return (bool)GetValue(IsElementModifiedProperty);
            }
            set
            {
                SetValue(IsElementModifiedProperty, value);
            }
        }

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty TitleTextProperty =
        DependencyProperty.RegisterAttached("TitleText", typeof(string), typeof(PaginationSettingContainerButton),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        public static readonly DependencyProperty IsRejectFoundProperty =
          DependencyProperty.RegisterAttached("IsRejectFound", typeof(bool), typeof(PaginationSettingContainerButton),
              new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        public static readonly DependencyProperty IsElementModifiedProperty =
            DependencyProperty.RegisterAttached("IsElementModified", typeof(bool), typeof(PaginationSettingContainerButton),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        #endregion

        #region Constructor

        static PaginationSettingContainerButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PaginationSettingContainerButton), new FrameworkPropertyMetadata(typeof(PaginationSettingContainerButton)));
        }

        public PaginationSettingContainerButton()
        {
            //Loaded += OnLoaded;
        }

        #endregion

        #region Methods

        public void SetChecked(bool isChecked)
        {
            IsChecked = isChecked;
        }

        #endregion
    }
}
