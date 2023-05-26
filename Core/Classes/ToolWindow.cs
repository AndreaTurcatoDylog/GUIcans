using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using static Core.ScreenManager;

namespace Core
{
    /// <summary>
    /// This is a class with the title bar and the three button for close, minimaze and maximaze the window.
    /// When a Window is a ToolWindow us the code below:
    /// <Border BorderBrush="Silver" BorderThickness="1">
    ///    <DockPanel x:Name="RootWindow" >
    ///        <ContentPresenter DockPanel.Dock="Top" Style= "{StaticResource ToolWindowTemplate}" Content= "{Binding}" />

    ///        //Main Grid
    ///        < Grid Name= "MainGrid" >
    ///            < Put here the component of the View>
    ///        </Grid>
    //</DockPanel>
    //</Border>
    /// </summary>
    public class ToolWindow : AdjustableWindow
    {
        #region Members

        private ScreenManager _ScreenManager;

        #endregion

        #region Commands

        /// <summary>
        /// The Mimimaze Window Command
        /// </summary>
        private ICommand _MinimazeCommand;
        public ICommand MinimazeCommand
        {
            get { return _MinimazeCommand; }
            set { _MinimazeCommand = value; }
        }

        /// <summary>
        /// The Massiimaze Window Command
        /// </summary>
        private ICommand _MassimazeCommand;
        public ICommand MassimazeCommand
        {
            get { return _MassimazeCommand; }
            set { _MassimazeCommand = value; }
        }

        /// <summary>
        /// The Exit command
        /// </summary>
        private ICommand _ExitCommand;
        public ICommand ExitCommand
        {
            get { return _ExitCommand; }
            set { _ExitCommand = value; }
        }

        #endregion

        #region Constructor

        public ToolWindow()
            : base("LayoutRoot")
        {
            // Create the Screen Manager
            _ScreenManager = new ScreenManager();

            // Set the Max Height
            MaxHeight = GetMaxHeight();

            // Create the Commands
            MinimazeCommand = new RelayCommand(MinimazeCommandExtecute);
            MassimazeCommand = new RelayCommand(MassimazeCommandExtecute);
            ExitCommand = new RelayCommand(ExitExecute);
        }

        #endregion       

        #region Events

        /// <summary>
        /// Occurs when the user wants to Minimaze the window
        /// </summary>
        private void MinimazeCommandExtecute(object param)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Occurs when the user wants to Massimaze the window
        /// </summary>
        private void MassimazeCommandExtecute(object param)
        {
            // Set the Max Height
            MaxHeight = GetMaxHeight();

            // Set the Window State
            WindowState = (WindowState == WindowState.Normal) ? WindowState.Maximized
                                                              : WindowState.Normal;
        }

        /// <summary>
        /// Returns the Max Height
        /// </summary>
        /// <returns></returns>
        private double GetMaxHeight()
        {
            var monitorInfo = _ScreenManager.GetMonitorSize();
            if (monitorInfo != null && monitorInfo.WorkArea != null)
            {
                //return Math.Abs(monitorInfo.WorkArea.Bottom - monitorInfo.WorkArea.Top) + SystemParameters.WindowCaptionHeight - 13;
                return Math.Abs(monitorInfo.WorkArea.Bottom - monitorInfo.WorkArea.Top) + SystemParameters.WindowCaptionHeight - 8;
            }

            return 0;
        }

        /// <summary>
        /// Occurs when left button of the mouse is down the Title Bar
        /// </summary>
        protected void OnTitleBarMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //DragMove();
        }

        /// <summary>
        /// The Exit Command Execute 
        /// </summary>
        public void ExitExecute(object param)
        {
            Close();
        }

        #endregion
    }
}
