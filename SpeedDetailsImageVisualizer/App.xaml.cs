using Business;
using Common;
using Core;
using Core.ResourceManager.Cultures;
using DialogService;
using Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SpeedDetailsImageVisualizer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Events

        /// <summary>
        /// Occurs whent the application Starts 
        /// </summary>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);           

            // Initialize the Culture
            CultureResources.Initialize();

            var _PathApplication = $@"{Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName)}";

            ScreenManager.Initialize();

            // Create and load the local settings
            LocalSettings.Instance.Load(_PathApplication);

            var useSplashScreen = false;
           
            if (useSplashScreen)
            {
                SplashScreenStartUp();
            }
            else
            {
                NormalStartUp();
            }
        }

        /// <summary>
        /// Use the Splash Screen on Start Up
        /// </summary>
        private void SplashScreenStartUp()
        {
            // Get image from resoources
            var splashImage = SpeedDetailsImageVisualizer.Properties.Resources.DylogSplashReportTool.ToBitmapImage();
            var splashScreenViewModel = new SplashScreenViewModel(splashImage);

            var splashScreen = new SplashScreenWindow();
            splashScreen.DataContext = splashScreenViewModel;
            this.MainWindow = splashScreen;
            splashScreen.Show();

            //in order to ensure the UI stays responsive, we need to
            //do the work on a different thread
            Task.Factory.StartNew(() =>
            {
                //simulate some work being done
                System.Threading.Thread.Sleep(3000);

                //since we're not on the UI thread
                //once we're done we need to use the Dispatcher
                //to create and show the main window
                this.Dispatcher.Invoke(() =>
                {
                    //initialize the main window, set it as the application main window
                    //and close the splash screen                    
                    MainWindow = new MainWindow();

                    // Close the splash screen
                    splashScreen.Close();

                    // Show the main window
                    MainWindow.Show();
                });
            });
        }

        /// <summary>
        /// Use the Normal Start Up
        /// </summary>
        private void NormalStartUp()
        {
            //in order to ensure the UI stays responsive, we need to
            //do the work on a different thread
            Task.Factory.StartNew(() =>
            {               
                //since we're not on the UI thread
                //once we're done we need to use the Dispatcher
                //to create and show the main window
                this.Dispatcher.Invoke(() =>
                {
                    //initialize the main window, set it as the application main window
                    //and close the splash screen                    
                    MainWindow = new MainWindow();
                    MainWindow.Show();
                });
            });
        }

        /// <summary>
        /// Occurs when the Application is closed
        /// </summary>
        private void OnApplicationExit(object sender, ExitEventArgs e)
        {
           
        }

        #endregion
    }
}
