using System;
using Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    /// <summary>
    /// The Base class of the Filters
    /// </summary>
    public abstract class FiltersBase : ModelBase
    {
        #region Members

        private DateTime? _FromDate;
        private DateTime? _ToDate;
        private DateTime? _TempFromDate;
        private DateTime? _TempToDate;
        private int _Take;
        private int _Skip;
        private bool _FiltersExists;
        private bool _IsFiltered;
        private bool _AreFiltersOpened;

        #endregion

        #region Properties

        /// <summary>
        /// Get\Set the From Date Filter
        /// </summary>
        public DateTime? FromDate
        {
            get { return _FromDate; }
            set
            {
                _FromDate = value;
                OnPropertyChanged("FromDate");
            }
        }

        /// <summary>
        /// Get\Set the To Date Filter
        /// </summary>
        public DateTime? ToDate
        {
            get { return _ToDate; }
            set
            {
                _ToDate = value;
                OnPropertyChanged("ToDate");
            }
        }

        /// <summary>
        /// Get\Set the Temp FromDate.
        /// It is the temp value not used for filter
        /// </summary>
        public DateTime? TempFromDate
        {
            get { return _TempFromDate; }
            set
            {
                _TempFromDate = value;
                OnPropertyChanged("TempFromDate");
            }
        }

        /// <summary>
        /// Get\Set the Temp ToDate.
        /// It is the temp value not used for filter
        /// </summary>
        public DateTime? TempToDate
        {
            get { return _TempToDate; }
            set
            {
                _TempToDate = value;
                OnPropertyChanged("TempToDate");
            }
        }

        /// <summary>
        /// Get\Set the Take
        /// </summary>
        public int Take
        {
            get { return _Take; }
            set
            {
                _Take = value;
                OnPropertyChanged("Take");
            }
        }

        /// <summary>
        /// Get\Set the Skip
        /// </summary>
        public int Skip
        {
            get { return _Skip; }
            set
            {
                _Skip = value;
                OnPropertyChanged("Skip");
            }
        }

        /// <summary>
        /// Returns the From Date Ticks
        /// </summary>
        public long FromDateTicks
        {
            get
            {
                if (FromDate.HasValue)
                {
                    return FromDate.Value.Ticks;
                }

                return -1;
            }
        }

        /// <summary>
        /// Returns the To Date Ticks
        /// </summary>
        public long ToDateTicks
        {
            get
            {
                if (ToDate.HasValue)
                {
                    return ToDate.Value.Ticks;
                }

                return -1;
            }
        }

        /// <summary>
        /// Returns the Is Filtered information
        /// </summary>
        public bool IsFiltered
        {
            get { return _IsFiltered; }
            set
            {
                _IsFiltered = value;
                OnPropertyChanged("IsFiltered");
            }
        }

        /// <summary>
        /// Get\Set the  Filters Exists
        /// Specify whether Filters exists or not
        /// </summary>        
        public bool FiltersExists
        {
            get { return _FiltersExists; }
            set
            {
                _FiltersExists = value;
                OnPropertyChanged("FiltersExists");
            }
        }

        /// <summary>
        /// Get\Set the AreFilterOpened.
        /// Specify whehter the filters are opened for the settings or not
        /// </summary>
        public bool AreFiltersOpened
        {
            get { return _AreFiltersOpened; }
            set
            {
                _AreFiltersOpened = value;
                OnPropertyChanged("AreFiltersOpened");
            }
        }

        #endregion

        #region Constructor

        public FiltersBase() { }

        public FiltersBase(int take, int skip)
        {
            Take = take;
            Skip = skip;
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Returns a clone of the filters
        /// </summary>
        public abstract FiltersBase Clone();

        #endregion

        #region Methods

        /// <summary>
        /// Returns the table filtered information
        /// </summary>
        public virtual bool IsTableFiltered()
        {
            return false;
        }

        /// <summary>
        /// Reset the Table Filter
        /// </summary>
        public virtual void TableFiltersReset()
        {
            ClearTempFilters();
        }

        /// <summary>
        /// Clear the Temp filters
        /// </summary>
        public virtual void ClearTempFilters()
        {
            TempFromDate = null;
            TempToDate = null;
        }

        /// <summary>
        /// Update the filters with the current temporary values
        /// </summary>
        public virtual void UpdateFilters()
        {
            FromDate = TempFromDate;
            ToDate = TempToDate;
        }

        /// <summary>
        /// Set the Temp filters with the current  values
        /// </summary>
        public virtual void SetTempFilters()
        {
            TempFromDate = FromDate;
            TempToDate = ToDate;
        }

        /// <summary>
        /// Check the filters and show the Icon
        /// 1. All filters are resetted => no icon is shown
        /// 2. One or more filters changes but Enter is not pressed => The Exclamation Point is shown (filters not applied)
        /// 3. One or more filters changes and Enter is pressed => The Check is shown (filters applied)
        /// </summary>
        protected virtual void CheckFilters()
        { }

        /// <summary>
        /// Returns a collection rappresentation with a filter in each element
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<string> GetFiltersList()
        {
            var result = new List<string>();

            result.Add(FromDate.ToString());
            result.Add(ToDate.ToString());

            return result;
        }

        /// <summary>
        /// Returns TRUE whehter the Filters are equals
        /// </summary>
        public virtual bool FiltersEquals(FiltersBase filters)
        {
            if (filters != null)
            {
                return filters?.FromDate == FromDate && filters.ToDate == ToDate;
            }

            return false;
        }

        /// <summary>
        /// Reset the Filters
        /// </summary>
        public void ResetFilters()
        {
            FromDate = null;
            ToDate = null;

            TableFiltersReset();
        }

        #endregion
    }
}
