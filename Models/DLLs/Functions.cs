using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Models
{
    public static class Functions
    {
        public static int CreateBufferFromTifImage(string imagePath, int pointerBuffer, int numberOfRow, int numberOfColumn, int dataByteDepth, int pointer2Info)
        {
            return ImportDLL.LoadTiffar(imagePath, pointerBuffer, numberOfRow, numberOfColumn, dataByteDepth, pointer2Info);
        }

        public static long BA_PointerAct(AGENT_ACTS action, int dst, int src, int numberOfRow, int numberOfColumn, int databyte)
        {
            return ImportDLL.BA_PointerAct(action, dst, src, numberOfRow, numberOfColumn, databyte);
        }
    }
}
