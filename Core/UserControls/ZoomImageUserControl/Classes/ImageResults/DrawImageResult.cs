using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Core
{
    public class DrawImageResult
    {
        #region Members

        private Canvas _Canvas;

        #endregion

        #region Properties

        public List<IImageResult> _Results { get; private set; }

        #endregion

        #region Constructor

        public DrawImageResult(Canvas canvas, List<IImageResult> results)
        {
            _Results = results;
            _Canvas = canvas;
        }

        public DrawImageResult(Canvas canvas): this(canvas, new List<IImageResult>())
        {
            
        }

        ////public DrawImageResult(Expression<Func<IResult, bool> filter)
        //public DrawImageResult(Func<IResult, bool> filter)
        //{
        //    _Results = _Results.Where(filter).ToList();
        //}

        #endregion

        #region Methods

        /// <summary>
        /// Draw all results 
        /// </summary>
        public void Draw()
        {
            if (_Results != null)
            {
                foreach (var result in _Results)
                {
                    result.Draw(_Canvas);
                }
            }
        }

        /// <summary>
        /// Add a result
        /// </summary>
        public void AddResult(IImageResult result) {
            _Results.Add(result);
        }

        /// <summary>
        /// Add a list of results
        /// </summary>
        public void AddResults(IEnumerable<IImageResult> results)
        {
            _Results.AddRange(results);
        }

        #endregion
    }
}
