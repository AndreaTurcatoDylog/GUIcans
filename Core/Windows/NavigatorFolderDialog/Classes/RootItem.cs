using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Core
{
    public class RootItem
    {
        public int ID { get; private set; }

        public DrawingImage Image { get; private set; }
        public string Caption { get; private set; }

        public RootItem(int id, DrawingImage image, string caption)
        {
            ID = id;
            Image = image;
            Caption = caption;
        }
    }

}
