using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// A base class for an observable model
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        #region Implementation of INotifyPropertyChanged

        /// <summary>
        /// Raises the the property changed event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the Notify Property changed event for the named property
        /// </summary>
        /// <param name="propertyName"></param>
        public void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                //Raise the on property changed
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Raises the notify property changed event for the given property
        /// </summary>
        /// <param name="property"></param>
        public void OnPropertyChanged(Expression<Func<object>> property)
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                var propertyName = ViewModelBase.GetPropertyName(property);

                //Raise the on property changed
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Uses reflection to get the name of the property
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string GetPropertyName(Expression<Func<object>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("Invalid Property");
            }

            PropertyInfo property;

            var memberExpression = expression.Body as MemberExpression;

            if (memberExpression == null)
            {
                //check for unary expression
                var unaryExpress = expression.Body as UnaryExpression;

                if (unaryExpress == null)
                {
                    throw new ArgumentException("The expression is not a member access expression");
                }
                else
                {
                    var unaryMember = unaryExpress.Operand as MemberExpression;

                    if (unaryMember == null)
                    {
                        throw new ArgumentException("The unary expression is not a member access expression");
                    }

                    //get the property info
                    property = unaryMember.Member as PropertyInfo;
                }
            }
            else
            {
                //get the property info
                property = memberExpression.Member as PropertyInfo;
            }


            if (property == null)
            {
                throw new ArgumentException("The member expression does not have access to the property");
            }

            var getMethod = property.GetGetMethod(true);
            if (getMethod.IsStatic)
            {
                throw new ArgumentException("The referenced property is a static");
            }

            return property.Name;
        }

        /// <summary>
        /// Set the property with the specified value. If the value is not equal with the field then the field is
        /// set, a PropertyChanged event is raised and it returns true.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="field">Reference to the backing field of the property.</param>
        /// <param name="value">The new value for the property.</param>
        /// <param name="propertyName">The property name. This optional parameter can be skipped
        /// because the compiler is able to create it automatically.</param>
        /// <returns>True if the value has changed, false if the old and new value were equal.</returns>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(field, value)) { return false; }

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion
    }
}
