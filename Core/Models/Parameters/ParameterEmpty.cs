using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// Rappresent an empty parameter
    /// </summary>
    public class ParameterEmpty: SettingItemBase, IParameter
    {
        #region Fields

        private string _Subtitle;
        private bool _IsRejectFound;
        private bool _IsUsed;
        private bool _IsEnabled;
        private int _Category;
        private int _View;

        #endregion


        #region Properties

        /// <summary>
        /// Get\Set the Subtitle
        /// </summary>
        public string Subtitle
        {
            get { return _Subtitle; }
            set
            {
                _Subtitle = value;
                OnPropertyChanged("Subtitle");
            }
        }

        /// <summary>
        /// Get\Set the is Reject found.
        /// It is the information that specify whether the parameter has almost one reject or no
        /// </summary>
        public bool IsRejectFound
        {
            get { return _IsRejectFound; }
            set
            {
                _IsRejectFound = value;

                OnIsRejectFoundChanged?.Invoke(this, EventArgs.Empty);
                OnPropertyChanged("IsRejectFound");
            }
        }

        /// <summary>
        /// Get\Set the is Reject found.
        /// Specify whether the inserted value is used (ex. calculation) or not
        /// </summary>
        public bool IsUsed
        {
            get { return _IsUsed; }
            set
            {
                _IsUsed = value;
                OnPropertyChanged("IsUsed");
            }
        }

        /// <summary>
        /// Get\Set the isEnabled information.
        /// Specify whether the parameter can be edited or not
        /// </summary>
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set
            {
                _IsEnabled = value;
                OnPropertyChanged("IsEnabled");
            }
        }

        /// <summary>
        /// Get\Set 
        /// 
        /// </summary>
        public int View
        {
            get { return _View; }
            set
            {
                _View = value;
                OnPropertyChanged("View");
            }
        }

        /// <summary>
        /// Get\Set 
        /// 
        /// </summary>
        public int Category
        {
            get { return _Category; }
            set
            {
                _Category = value;
                OnPropertyChanged("Category");
            }
        }

        #endregion

        #region Event Handlers

        public event EventHandler OnIsRejectFoundChanged;

        #endregion


        public ParameterEmpty(int view, int category)
            : base((int)Setting.EMPTY, string.Empty)
        {
            View = view;
            Category = category;
        }
    }
}
