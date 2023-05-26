using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    /// <summary>
    /// The Filters
    /// </summary>
    public class Filters : FiltersBase
    {
        #region Members

        private string _FileName;
        private string _TempFileName;

        #endregion

        #region Properties

        /// <summary>
        /// Get\Set the FileName Filter
        /// </summary>
        public string FileName
        {
            get { return _FileName; }
            set
            {
                _FileName = value;
                OnPropertyChanged("FileName");
            }
        }

        /// <summary>
        /// Get\Set the Temp FileName Filter.
        /// It is the temp value not used for filter
        /// </summary>
        public string TempFileName
        {
            get { return _TempFileName; }
            set
            {
                _TempFileName = value;
                OnPropertyChanged("TempFileName");
            }
        }

        #endregion

        #region Constructor

        public Filters() : base() { }

        public Filters(int take, int skip)
            : base(take, skip)
        { }

        #endregion

        #region Methods

        /// <summary>
        /// Returns a collection rappresentation with one filter in each position
        /// </summary>
        public override IEnumerable<string> GetFiltersList()
        {
            var result = new List<string>();

            // Add the base filters
            result.AddRange(base.GetFiltersList());

            // Add the filters
            result.Add(FileName);

            return result;
        }

        /// <summary>
        /// Update the filters with the current temporary values
        /// </summary>
        public override void UpdateFilters()
        {
            base.UpdateFilters();
            FileName = TempFileName;

            CheckFilters();

            IsFiltered = IsTableFiltered();
        }

        /// <summary>
        /// Set the Temp filters with the current  values
        /// </summary>
        public override void SetTempFilters()
        {
            if (AreFiltersOpened)
            {
                TempFileName = FileName;
                base.SetTempFilters();
            }
        }

        /// <summary>
        /// Returns the is filtered information.
        /// The Table is filtered if almost one filter is setted
        /// </summary>
        /// <returns></returns>
        public override bool IsTableFiltered()
        {
            return FromDate.HasValue || ToDate.HasValue
                        || !string.IsNullOrEmpty(FileName);
        }

        /// <summary>
        /// Check the filters and show the Icon
        /// 1. All filters are resetted => no icon is shown
        /// 2. One or more filters changes but Enter is not pressed => The Exclamation Point is shown (filters not applied)
        /// 3. One or more filters changes and Enter is pressed => The Check is shown (filters applied)
        /// </summary>
        protected override void CheckFilters()
        {
            if (string.IsNullOrEmpty(FileName) &&
               string.IsNullOrEmpty(TempFileName) && !FromDate.HasValue && !TempFromDate.HasValue
               && !ToDate.HasValue && !TempToDate.HasValue)
            {
                FiltersExists = false;
            }
            else
            {
                FiltersExists = true;
            }

            // Check the FileName
            var isFileNameEquals = (FileName == _TempFileName);

            // Check the From Date
            var isFromDateEquals = false;
            if (FromDate.HasValue && TempFromDate.HasValue)
            {
                isFromDateEquals = FromDate.Value == TempFromDate.Value;
            }
            else
            {
                isFromDateEquals = !FromDate.HasValue && !TempFromDate.HasValue;
            }

            // Check the To Date
            var isToDateEquals = false;
            if (ToDate.HasValue && TempToDate.HasValue)
            {
                isToDateEquals = ToDate.Value == TempToDate.Value;
            }
            else
            {
                isToDateEquals = !ToDate.HasValue && !TempToDate.HasValue;
            }

            // Set the IsTableFiltered
            IsFiltered = isFileNameEquals && isFromDateEquals && isToDateEquals;
        }

        /// <summary>
        /// Returns TRUE whether the Filters are equals
        /// </summary>
        public override bool FiltersEquals(FiltersBase filters)
        {
            if (filters != null)
            {
                var Filters = filters as Filters;
                var isBaseEquals = base.FiltersEquals(Filters);
                return isBaseEquals && Filters.FileName == FileName;
            }

            return false;
        }

        /// <summary>
        /// Reset the Filters
        /// </summary>
        public override void TableFiltersReset()
        {
            // Reset the filters
            base.TableFiltersReset();
            TempFileName = string.Empty;

            FiltersExists = false;

            // Update the filters
            UpdateFilters();
        }

        /// <summary>
        /// Clear the Temp Filters
        /// </summary>
        public override void ClearTempFilters()
        {
            // Clear the temp filters
            base.ClearTempFilters();
            TempFileName = string.Empty;
        }

        /// <summary>
        /// Returns a clone of the filters
        /// </summary>
        public override FiltersBase Clone()
        {
            return new Filters()
            {
                FromDate = this.FromDate,
                ToDate = this.ToDate,
                FileName = this.FileName,
                Take = this.Take,
                Skip = this.Skip,
            };
        }

        #endregion
    }
}
