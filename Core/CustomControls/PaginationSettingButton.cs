using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Core
{
    public class PaginationSettingButton : PaginationButton
    {
        #region Fields

        /// <summary>
        /// Rappresents the number of updated elements in the page
        /// </summary>
        public int NumberOfUpdatedElements;

        /// <summary>
        /// Rappresents the number of rejects in the page
        /// </summary>
        //private int _NumberOfRejects;

        #endregion

        #region Properties

        /// <summary>
        /// Get\Set IsElementModified. Specify the information almost one element in page is modified
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

        /// <summary>
        /// Get\Set IsRejectFound. Specify the information almost one element in page has a recjet
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

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty IsElementModifiedProperty =
            DependencyProperty.RegisterAttached("IsElementModified", typeof(bool), typeof(PaginationSettingButton),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        public static readonly DependencyProperty IsRejectFoundProperty =
           DependencyProperty.RegisterAttached("IsRejectFound", typeof(bool), typeof(PaginationSettingButton),
               new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        #endregion

        #region Constructor

        static PaginationSettingButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PaginationSettingButton), new FrameworkPropertyMetadata(typeof(PaginationSettingButton)));
        }

        public PaginationSettingButton()
        {
            NumberOfUpdatedElements = 0;
            //_NumberOfRejects = 0;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set the update information of the page
        /// </summary>
        //public void SetUpdateInformation(bool isUpdated)
        //{
        //    SetUpdateInformation(_NumberOfUpdatedElements, isUpdated);
        //}

        /// <summary>
        /// Set the update information of the page.
        /// This method set the numner of updated elements also
        /// </summary>
        public void SetUpdateInformation(bool isUpdated)
        {
            // Update the number of updated elements 
            NumberOfUpdatedElements = isUpdated ? NumberOfUpdatedElements + 1
                                                 : NumberOfUpdatedElements - 1;

            if (NumberOfUpdatedElements < 0)
            {
                NumberOfUpdatedElements = 0;
            }

            // Update the element modified property
            IsElementModified = NumberOfUpdatedElements > 0;
        }

        ///// <summary>
        ///// Set the reject found information of the page
        ///// </summary>
        //public void SetRejectFoundInformation(bool isRejectFound)
        //{
        //    // Update the element modified property
        //    IsRejectFound = isRejectFound;
        //}

        #endregion
    }
}
