using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Core
{
    public class CircleResult : IResult
    {

        public int X1 { get; set; }
        public int Y1 { get; set; }
        public int X2 { get; set; }
        public int Y2 { get; set; }

        #region Methods

        /// <summary>
        /// Draw the result
        /// </summary>
        public void Draw(Canvas canvas)
        {
            // Create the rectangle
            var ellipse = new Ellipse();
            ellipse.Width = X2;
            ellipse.Height = Y2;
            ellipse.StrokeThickness = 1;
            ellipse.Stroke = Brushes.Blue;

            canvas.Children.Add(ellipse);
            Canvas.SetLeft(ellipse, X1);
            Canvas.SetTop(ellipse, Y1);
        }

        #endregion
    }
}
