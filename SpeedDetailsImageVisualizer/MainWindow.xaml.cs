using Business;
using Common;
using Core;
using Core.ResourceManager.Cultures;
using DialogService;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SpeedDetailsImageVisualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ToolWindow
    {
        #region Members

        private int _NumberOfResized;
        private MainViewModel _MainViewModel;

        #endregion        

        #region Constructor

        public MainWindow()            
        {
            InitializeComponent();

            _NumberOfResized = 0;

            // Create  keyboard Layout
            var keyboarLayout = new KeyboardLayout();
            keyboarLayout.CreateLayout(Properties.Resources.KeyboardEnglishLayout);

            // Create standard and numeric keyboard
            var standardKeyboard = new StandardKeyboard(keyboarLayout);
            var numericKeyboard = new NumericKeyboard(keyboarLayout);

            // Create the view model
            var logoBitmapImage = Properties.Resources.Logo.ToBitmapImage();
            _MainViewModel = new MainViewModel(logoBitmapImage);

            // Register the External Dialogs
            RegisterExternalDialog(true);

            // Set the Data Context
            DataContext = _MainViewModel;
        }

        #endregion

        #region Methods       

        /// <summary>
        /// Register\Unregister the events
        /// </summary>
        protected override void RegisterEvents(bool register)
        {
            if (register)
            {
               // MouseLeftButtonDown += delegate { DragMove(); };
                Closed += (s, e) => Application.Current.Shutdown();               

                if (_MainViewModel != null)
                {
                    _MainViewModel.CloseApplicationRequested += (s, e) => Close();
                }
            }
            else
            {
                Closed -= (s, e) => Application.Current.Shutdown();

                if (_MainViewModel != null)
                {
                    _MainViewModel.CloseApplicationRequested -= (s, e) => Close();
                }
            }
        }

        // Subscribe\Unsubscrive the External Dialog
        private void RegisterExternalDialog(bool register)
        {
            foreach (var externalDialogKey in (DialogServiceKey[])Enum.GetValues(typeof(DialogServiceKey)))
            {
                if (externalDialogKey != DialogServiceKey.None)
                {
                    if (register)
                    {
                        ExternalDialogManager.Instance.Subscribe((int)externalDialogKey, OnOpenExternalDialog);
                    }
                    else
                    {
                        ExternalDialogManager.Instance.Unsubscribe((int)externalDialogKey, OnOpenExternalDialog);
                    }
                }
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the Main Window chenges its size.
        /// If it is a ChildApplication the SW is: 
        /// 1) minimize
        /// 2) resized
        /// 3)maximed
        /// In case of ChildApplication only two resized are allowed
        /// </summary>
        private void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _NumberOfResized++;           

            if (_MainViewModel != null && _MainViewModel.IsChildApplication)
            {
                if (_NumberOfResized > 1)
                {
                    ResizeMode = ResizeMode.NoResize;                    
                }
            }

            if (this.WindowState == WindowState.Maximized)
            {
                this.BorderThickness = new Thickness(8);               
            }
            else
            {
                this.BorderThickness = new Thickness(0);
            }
        }

        /// <summary>
        /// Occurs when the Visibility of the Title Bar changes
        /// </summary>
        private void TitleBarIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool isVisible)
            {
                this.WindowState = isVisible ? WindowState.Maximized : WindowState.Minimized;
            }
        }

        /// <summary>
        /// Occurs when the MainViewow is rendered
        /// </summary>
        private void OnWindowContentRendered(object sender, EventArgs e)
        {
            _MainViewModel.LoadCommand.Execute(null);
        }

        /// <summary>
        /// Open the specificated Dialog
        /// </summary>
        private void OnOpenExternalDialog(object args)
        {
            if (args is IDialogViewModel viewModel)
            {
                var dialogServiceFactory = new DialogServiceFactory();
                dialogServiceFactory.CreateDialog(viewModel);
            }
        }

        /// <summary>
        /// Occurs when the LeftButton is down.
        /// If it is a ChildApplication the Dragmove is disabled
        /// </summary>
        private void OnWindowMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_MainViewModel != null && !_MainViewModel.IsChildApplication)
            {
                DragMove();
            }
        }

        /// <summary>
        /// Occurs when the window is closed
        /// </summary>
        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {          
            // Unregister the External Dialog
            RegisterExternalDialog(false);           
        }

        #endregion        
    }
}
