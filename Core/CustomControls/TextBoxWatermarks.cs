using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Core
{
    /// <summary>
    /// This component rappresent a TextBox with a Watermaks.
    /// The Watermark is visible when Text is null or empty
    /// </summary>
    public class TextBoxWatermarks : TextBox
    {
        #region Properties

        /// <summary>
        /// Get\Set the WaterMarkText
        /// </summary>
        public string WaterMarkText
        {
            get { return (string)this.GetValue(WaterMarkTextProperty); }
            set
            {
                this.SetValue(WaterMarkTextProperty, value);
            }
        }

        /// <summary>
        /// Get\Set the EnterCommand
        /// </summary>
        public ICommand EnterCommand
        {
            get { return (ICommand)this.GetValue(EnterCommandProperty); }
            set
            {
                this.SetValue(EnterCommandProperty, value);
            }
        }

        #endregion

        #region Dependency Properties

        private static readonly DependencyProperty WaterMarkTextProperty =
          DependencyProperty.Register("WaterMarkText", typeof(string), typeof(TextBoxWatermarks));

        private static readonly DependencyProperty EnterCommandProperty =
          DependencyProperty.Register("EnterCommand", typeof(ICommand), typeof(TextBoxWatermarks));

        #endregion
    }
}
