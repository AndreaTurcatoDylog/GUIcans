using System;
using System.Text;
using System.Runtime.InteropServices;

namespace Models
{
    internal class Structs
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct tRect
        {
            int cMIN;
            int cMAX;
            int rMIN;
            int rMAX;
            int colore;

            public tRect(int rm, int rM, int cm, int cM, int colr) { cMIN = cm; cMAX = cM; rMIN = rm; rMAX = rM; colore = colr; }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct XYPoint
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct iRect
    {
        [FieldOffset(0)]
        public int minCOl;
        [FieldOffset(4)]
        public int maxCOL;
        [FieldOffset(8)]
        public int right;
        [FieldOffset(12)]
        public int bottom;
    }
}
