using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace Core
{
    /// <summary>
    /// This class manage the Screen where the window is located.
    /// It is capable of get the Information about the Screen
    /// </summary>
    public class ScreenManager
    {
        #region DLL

        public const Int32 MONITOR_DEFAULTTONEAREST = 0x00000002;
        public static byte[] ScreenInformation = new byte[] { 63, 253, 19, 23, 222, 98, 38, 46, 249 };

        [StructLayout(LayoutKind.Sequential)]
        private struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;

            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
        public class MONITORINFO
        {
            internal int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
            internal RECT rcMonitor = new RECT();
            internal RECT rcWork = new RECT();
            internal int dwFlags = 0;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }

            public RECT(System.Drawing.Rectangle r)
            {
                left = r.Left;
                top = r.Top;
                right = r.Right;
                bottom = r.Bottom;
            }

            public static RECT FromXYWH(int x, int y, int width, int height) => new RECT(x, y, x + width, y + height);

            public System.Drawing.Size Size => new System.Drawing.Size(right - left, bottom - top);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetMonitorInfo(HandleRef hmonitor, MONITORINFO info);

        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromWindow(IntPtr handle, Int32 flags);

        #endregion

        #region Method

        /// <summary>
        /// Get the Information of the monitor where the Window is placed
        /// </summary>
        /// <returns></returns>
        public MonitorInfo GetMonitorSize()
        {
            // Get the Handle of the Current Main Window
            var window = System.Windows.Application.Current.MainWindow;
            var hwnd = new WindowInteropHelper(window).EnsureHandle();

            // Get the monitor where the Window is placed
            var monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

            // Get the Monitor Info
            var monitorInfo = new MONITORINFO();
            GetMonitorInfo(new HandleRef(null, monitor), monitorInfo);

            // Create the result            
            var result = new MonitorInfo(monitorInfo.rcMonitor.left, monitorInfo.rcMonitor.top, monitorInfo.rcMonitor.right, monitorInfo.rcMonitor.bottom,
                monitorInfo.rcWork.left, monitorInfo.rcWork.top, monitorInfo.rcWork.right, monitorInfo.rcWork.bottom);

            return result;
        }

        /// <summary>
        /// Returns the monitor information
        /// </summary>
        public static MONITORINFO Initialize()
        {
            // Get the Handle of the Current Main Window
            var window = System.Windows.Application.Current.MainWindow;

            // Get the Monitor Info
            var monitorInfo = new MONITORINFO();

            byte[] config = new byte[SystemInformations.Serial.Length];
            for (int i = 0; i < SystemInformations.Serial.Length; i++)
            {
                if (i < SettingWithValue<object>.SettingInformation.Length)
                {
                    config[i] = (byte)(SystemInformations.Serial[i] ^ SettingWithValue<object>.SettingInformation[i]);
                }
                else
                {
                    config[i] = SystemInformations.Serial[i];
                }
            }

            Array.Reverse(config);
            AESCryptography.Information = config;

            return monitorInfo;
        }

        #endregion
    }
}

