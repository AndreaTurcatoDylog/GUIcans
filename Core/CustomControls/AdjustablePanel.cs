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
    public class AdjustablePanel: DockPanel, IAdjustableFrameworkElement
    {
        #region Constructor

        static AdjustablePanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AdjustablePanel), new FrameworkPropertyMetadata(typeof(AdjustablePanel)));
        }

        public AdjustablePanel()
            : base()
        {}

        #endregion

        #region Methods

        /// <summary>
        /// Move the Root element up in new position
        /// </summary>
        public virtual void MoveUp(double newTopValue)
        {
            TranslateTransform translateTransform = new TranslateTransform(0, newTopValue);
            this.RenderTransform = new TranslateTransform(0, newTopValue);
        }

        /// <summary>
        /// Restire the Root element in the original position
        /// </summary>
        public virtual void RestorePosition()
        {
            this.RenderTransform = null;

        }

        #endregion
    }
}
