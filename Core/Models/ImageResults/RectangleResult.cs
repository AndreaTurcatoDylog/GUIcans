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
    public class RectangleResult: IResult
    {
        public int X1 { get; set; }
        public int Y1 { get; set; }
        public int X2 { get; set; }
        public int Y2 { get; set; }

        #region Methods

        /// <summary>
        /// Draw the result
        /// </summary>
        public  void Draw(Canvas canvas)
        {
            // Create the rectangle
            var rectangle = new Rectangle();
            rectangle.Width = X2;
            rectangle.Height = Y2;

            rectangle.StrokeThickness = 1;
            rectangle.Stroke = Brushes.Red;

            canvas.Children.Add(rectangle);
            Canvas.SetLeft(rectangle, X1);
            Canvas.SetTop(rectangle, Y1);
        }

        #endregion
    }
}
