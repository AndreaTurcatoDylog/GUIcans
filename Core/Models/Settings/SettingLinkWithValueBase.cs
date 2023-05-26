using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public abstract class SettingLinkWithValueBase: SettingLinkBase, ISettingWithValue
    {
        #region Fields

        protected object _Value;
        protected bool _ResultVisible;

        #endregion

        #region Properties

        public object Value
        {
            get { return _Value; }
            set
            {
                _Value = value;
                OnPropertyChanged("Value");
            }
        }

        #endregion

        #region Constructor

        public SettingLinkWithValueBase(Setting settingType, string label)
            : this(settingType,label, null, null, true)
        { }

        public SettingLinkWithValueBase(Setting settingType,string label, List<ISettingItem> next, object value, bool resultVisible)
            : base(settingType,label, next)
        {
            Value = value;
            _OriginalValue = value;

            Next = next;
            IsUpdated = false;

            _ResultVisible = resultVisible;

            // Set the values of children and set events
            SetChildrenValues();
            RegisterEvents(true);
        }

        #endregion

        #region Virtual Methods

        /// <summary>
        ///  Set the children values (ISettingItems in Next List)
        /// </summary>
        protected virtual void SetChildrenValues() { }

        // Update the value in according to logic through parent and ISettingItems in Next List
        protected virtual void ValueChangedAction(object sender)
        {}

        #endregion

        #region Methods

        /// <summary>
        /// Register\unregister events. Begin\end to listen to all ISettigItems in Next list.
        /// Every change in ISettingItems in Next list make a change to this object becouse it is the parent of group
        /// (generally changes its value connected with its 'childs')
        /// </summary>
        private void RegisterEvents(bool register)
        {
            foreach (var settingItem in Next)
            {
                if (settingItem != null)
                {
                    if (register)
                    {
                        settingItem.ValueChanged += OnSettingItemValueChanged;
                    }
                    else
                    {
                        settingItem.ValueChanged -= OnSettingItemValueChanged;
                    }
                }
            }
        }

        /// <summary>
        /// Updates the original value (usually called after a save operation)
        /// </summary>
        public virtual void UpdateOriginalValue()
        {
            IsUpdated = false;
            foreach (var setting in Next)
            {
                if (setting is ISettingWithValue)
                {
                    (setting as ISettingWithValue).UpdateOriginalValue();
                }
            }
        }

        /// <summary>
        /// Remove the changes in the object restoring the original value
        /// </summary>
        public virtual void RestoreOriginalValue()
        {
            IsUpdated = false;
            IsUpdateChanged = false;
            foreach (var setting in Next)
            {
                if (setting is ISettingWithValue)
                {
                    (setting as ISettingWithValue).RestoreOriginalValue();
                }
            }

            SetChildrenValues();
        }

        /// <summary>
        /// Returns information whether the object has value or not
        /// </summary>
        public bool HasValue()
        {
            return Value != null;
        }

        public abstract object GetValue();
       

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the Value of SettingItem in Next list changes
        /// </summary>
        private void OnSettingItemValueChanged(object sender, EventArgs e)
        {
            ValueChangedAction(sender);
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Dispose the object
        /// </summary>
        protected override void DisposeObjects()
        {
            base.DisposeObjects();
            RegisterEvents(false);

            foreach (var settingItem in Next)
            {
                settingItem?.Dispose();
            }
        }

        #endregion
    }
}
