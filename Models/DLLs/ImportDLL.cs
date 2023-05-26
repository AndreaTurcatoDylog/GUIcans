using System;
using System.Text;
using System.Runtime.InteropServices;

namespace Models
{
    internal class ImportDLL
    {
        #region DoppiaDLL

        [DllImport("doppiadll.dll")]
        public static extern Int32 LoadTiffar([MarshalAs(UnmanagedType.BStr)]String filename, int ptrbuf, int nRow, int nCol, int dataByteDepth, int ptr2Info);

        [DllImport("doppiadll.dll")]
        public static extern void drawRectONRGBA(int bptr, int stride, int nrow, ref Structs.tRect rect, double[] matrix);

        [DllImport("doppiadll.dll")]
        public static extern void cleanBUFFER(int bptr, int stride, int nrow);

        [DllImport("doppiadll.dll")]
        public static extern long BA_PointerAct(AGENT_ACTS action, int dst, int src, int nrow, int ncol, int databyte);

        #endregion
    }
}