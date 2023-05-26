using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Core
{
    /// <summary>
    /// This abstract class can be used when need to create an user control and make it disposable.
    /// Garbage collector does not manage the unsubscription of events so it is necessary to make it manually to avoid memory leak.
    /// 
    /// This class can be used for all user controls.
    /// 
    /// 1. Derive from UserControl class
    /// The class derive from UserControl to inherit all its behaviour
    /// 
    /// 2. RegisterEvents(bool register) abstract method.
    /// This method must be implemented from children classes. Every class could have subscription of events.
    /// Write code in child class to subscription\unsubscription its events in according to value of register parameter
    /// ex. if (register)
    /// {
    ///      event1 += OnEvent1;
    ///      event2 += OnEvent2;
    ///      ....
    ///      eventN += OnEventN
    /// }
    /// else
    /// {
    ///      event1 -= OnEvent1;
    ///      event2 -= OnEvent2;
    ///      ....
    ///      eventN -= OnEventN 
    /// }
    /// 
    /// 3. RegisterBaseEvents(bool register) Method 
    /// Paramenters 
    /// bool register: true subscription of events - false unsubscription of events
    /// 
    /// Description
    /// This method subscription\unsubscription the base events. The useful base event is the closing of parent window.
    /// 
    /// Closing parent window:
    /// In all user control there is the need to free resources when it is closed. 
    /// To do that the method get the parent window (object that hosts the user control) and write a custom behaviour when it will be closed.
    /// 
    /// The Method calls the RegisterEvents(bool register) method so all the events of child class will be subscripted or unsubscripted.
    /// 
    /// In this scenario when the child class is loaded all its own events (and events of base class) will be subscripted. 
    /// When the parent window wiil be closed all its own events (and events of base class) be unsubscripted. 
    /// </summary>
    public abstract  class DisposableUserControlBase: UserControl
    {
        #region Field

        Window _ParentWindow;

        #endregion

        #region Event Handler

        public event EventHandler LoadedEventHandler;
        public event EventHandler ClosedEventHandler;

        #endregion

        #region Virtual

        protected virtual void RegisterEvents(bool register) { }

        // Executed code after component is Loaded
        protected virtual void Load() { }

        #endregion

        #region Constructor

        public DisposableUserControlBase()
            :base()
        {
            Loaded += DisposableControlLoaded;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Register\Unregister all events
        /// </summary>
        private void RegisterBaseEvents(bool register)
        {
            // var parentWindow = Window.GetWindow(this);
            //var parentWindow = GetParentWindow(this);

            if (_ParentWindow != null)
            {
                if (register)
                {
                    _ParentWindow.Closing += ParentWindowClosing;
                }
                else
                {
                    Loaded -= DisposableControlLoaded;
                    _ParentWindow.Closing -= ParentWindowClosing;
                }
            }
            else
            {

            }

            RegisterEvents(register);
        }

        private  Window GetParentWindow(DependencyObject child)
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null)
            {
                return null;
            }

            var parent = parentObject as Window;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                return GetParentWindow(parentObject);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the control is loaded
        /// </summary>
        private void DisposableControlLoaded(object sender, RoutedEventArgs e)
        {
            if (this.Template != null)
            {
                _ParentWindow = GetParentWindow(this);
            }

            RegisterBaseEvents(true);

            Load();

            LoadedEventHandler?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// The on apply template can be override for:
        /// 1. builds the remainder of an element visual tree.
        /// 2. running code that relies on the visual tree from templates having been applied, 
        /// such as obtaining references to named elements that came from a template.
        /// 3. introducing services that only make sense to exist after the visual tree from templates is complete.
        /// 4. setting states and properties of elements within the template that are dependent on other factors.
        /// For instance, property values might only be discoverable by knowing the parent element, or when a specific derived class uses a common template.
        /// </summary>
        //public override void OnApplyTemplate()
        //{
        //    base.OnApplyTemplate();
        //    if (this.Template != null)
        //    {
        //        _ParentWindow = GetParentWindow(this);
        //    }
        //}
        
        /// <summary>
        /// Occurs when the parent windows is closing
        /// </summary>
        private void ParentWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            RegisterBaseEvents(false);
            ClosedEventHandler?.Invoke(this, new EventArgs());
        }

        #endregion
    }
}
