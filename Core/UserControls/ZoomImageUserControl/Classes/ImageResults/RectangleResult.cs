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
    public class RectangleResult: IImageResult
    {
        public int X1 { get; set; }
        public int Y1 { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        #region Methods

        /// <summary>
        /// Draw the result
        /// </summary>
        public  void Draw(Canvas canvas)
        {
            // Create the rectangle
            var rectangle = new Rectangle();
            rectangle.Width = Width;
            rectangle.Height = Height;

            rectangle.StrokeThickness = 1;
            rectangle.Stroke = Brushes.Red;

            canvas.Children.Add(rectangle);
            Canvas.SetLeft(rectangle, X1);
            Canvas.SetTop(rectangle, Y1);
        }

        #endregion
    }
}
