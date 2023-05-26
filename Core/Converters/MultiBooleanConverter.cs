using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Core
{
    //    public class MultiBooleanConverter<T> : IMultiValueConverter
    //    {
    //        public T True { get; set; }
    //        public T False { get; set; }

    //        public MultiBooleanConverter(T trueValue, T falseValue)
    //        {
    //            True = trueValue;
    //            False = falseValue;
    //        }

    //        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    //        {
    //            var confirmed = false;

    //            if (values != null)
    //            {
    //                var index = 0;
    //                var falseValueFound = false;
    //                while (index < values.Count() && !falseValueFound)
    //                {
    //                    falseValueFound = !(values[index] is bool value) || !value;
    //                    index++;
    //                }

    //                confirmed = !falseValueFound;
    //            }

    //            return confirmed ? True : False;
    //        }

    //        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    //        {
    //            return null;
    //        }
    //    }
    //}

    public enum BooleanOperatorType
    {
        AND = 0,
        OR = 1
    }

    /// <summary>
    /// It is a converter for the multiple conditions.
    /// The Result will be given by the logical AND\OR operator (the MultiBooleanOperatorType property)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MultiBooleanConverter<T> : IMultiValueConverter
    {
        public T True { get; set; }
        public T False { get; set; }

        public BooleanOperatorType BooleanOperatorType { get; set; }

        public MultiBooleanConverter(T trueValue, T falseValue)
        {
            True = trueValue;
            False = falseValue;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            switch (BooleanOperatorType)
            {
                case BooleanOperatorType.AND: return AndOperatorConverter(values);
                case BooleanOperatorType.OR: return OrOperatorConverter(values);
            }

            return False;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }

        /// <summary>
        /// Use the AND Converter.
        /// Return TRUE if ALL THE VALUES ARE TRUE: FALSE OTHERWISE
        /// </summary>
        private T AndOperatorConverter(object[] values)
        {
            var result = false;

            if (values != null)
            {
                var index = 0;
                var falseValueFound = false;
                while (index < values.Count() && !falseValueFound)
                {
                    falseValueFound = !(values[index] is bool value) || !value;
                    index++;
                }

                result = !falseValueFound;
            }

            return result ? True : False;
        }

        /// <summary>
        /// Use the OR Converter
        /// Return TRUE if ALMOST ON VALUE IS TRUE: FALSE OTHERWISE
        /// </summary>
        private T OrOperatorConverter(object[] values)
        {
            var result = false;

            if (values != null)
            {
                var index = 0;
                var trueValueFound = false;
                while (index < values.Count() && !trueValueFound)
                {
                    trueValueFound = (values[index] is bool value) && value;
                    index++;
                }

                result = trueValueFound;
            }

            return result ? True : False;
        }

    }
}