using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public abstract class SettingItemBase : ModelBase, ISettingItem
    {
        #region Fields

        private int _Type;
        private string _Label;
        private bool _IsUpdated;

        protected object _OriginalValue;

        #endregion

        #region Properties

        /// <summary>
        /// Get\Set the type of the item
        /// </summary>
        public int Type
        {
            get { return _Type; }
            set
            {
                _Type = value;
                OnPropertyChanged("Type");
            }
        }

        /// <summary>
        /// Get\Set the label of the item
        /// </summary>
        public string Label
        {
            get { return _Label; }
            set
            {
                _Label = value;
                OnPropertyChanged("Label");
            }
        }

        /// <summary>
        /// Get\Set the IsUpdated information. It is set when the value changes.
        /// This property can be enabled\disabled by 'EnableUpdateInformation property'
        /// </summary>
        public bool IsUpdated
        {
            get { return _IsUpdated; }
            set
            {
                _IsUpdated = value;
                OnPropertyChanged("IsUpdated");
            }
        }

        /// <summary>
        /// Get\Set the options
        /// </summary>
        public ISettingItemOption Options { get; private set; }

        /// <summary>
        /// Specify if the update is changed
        /// </summary>
        public bool IsUpdateChanged { get; protected set; }

        #endregion

        #region Event Handlers

        public event EventHandler ValueChanged;
        public event EventHandler ValueUpdated;

        #endregion

        #region Constructor

        public SettingItemBase(){}

        public SettingItemBase(int type, string label, ISettingItemOption options)
        {
            Type = type;
            Label = label;
            IsUpdated = false;
            Options = options;

            IsUpdateChanged = false;
        }

        public SettingItemBase(int type,  string label)
            :this(type, label, null)
        {}

        #endregion

        #region Methods

        public virtual bool IsSettingItemUpdated()
        {
            return IsUpdated;
        }

        /// <summary>
        /// Fire the value changed event
        /// </summary>
        public void OnIsValueChanged(object sender, EventArgs eventArgs)
        {
            ValueChanged?.Invoke(this, eventArgs);
        }

        /// <summary>
        /// Fire the value updated event
        /// </summary>
        public void OnIsValueUpdated(object sender, EventArgs eventArgs)
        {
            ValueUpdated?.Invoke(this, eventArgs);
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Dispose the object
        /// </summary>
        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);
            DisposeObjects();
        }

        protected virtual void DisposeObjects() { }

        #endregion
    }
}
