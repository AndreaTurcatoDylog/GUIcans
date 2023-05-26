using Common;
using Core;
using Core.ResourceManager.Cultures;
using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Business
{
    /// <summary>
    /// This class manages the Creation of the Report.
    /// It is a DialogViemodel that means it is an external dialog with its own lif circle
    /// </summary>

    public class CreateReportViewModel : ViewModelBase, IDialogViewModel, IDisposable
    {             
        #region Members

        private ExtendedBackgroundWorker _BackgroundWorker;

        private int _MaxProgressValue;
        private int _ExportProgressValue;

        private string _ProgressMessage;

        // Specify whether a converting operation is running or not
        private bool _IsConverting;

        // Specify whether the window is enabled.
        // It is not enabled during converting operation
        private bool _IsEnabled;

        // Specify whether the preview must be shown after the report is creted or not
        private bool _ShowPreview;

        // It is the name of the file of created report
        private string _CreatedReportFileName;

        // The additional string message to the Message status
        private string _AdditionalStatusMessage;

        // Specify the percentage message [0%..100%] shown in converting operation
        private string _ConvertingPercentageMessage;

        // The path where the report will be saved
        private string _ReportFileNamePath;

        // The status of the Window
        private CreateReportStatusEnum _CreateReportStatus;

        // The log
        private CommonLog _Log;

        // The PDF creator manager
        private IPdfCreatorManager _PdfCreatorManager;

        // The Logo path
        private string _LogoPath;

        // Contains the path of all the Created Reports
        private IList<string> _CreatedReportPaths;

        #endregion

        #region Properties

        /// <summary>
        /// Get\Set the Window Code
        /// </summary>
        public int DialogServiceKey { get; private set; }

        /// <summary>
        /// Get\Set the status of the Window
        /// </summary>
        public CreateReportStatusEnum CreateReportStatus
        {
            get { return _CreateReportStatus; }
            set
            {
                _CreateReportStatus = value;
                OnPropertyChanged("CreateReportStatus");
            }
        }

        /// <summary>
        /// Get\Set the ProgressMessage. 
        /// It is the Text in the Progress Bar when converting operation is running
        /// </summary>
        public string ProgressMessage
        {
            get { return _ProgressMessage; }
            set
            {
                _ProgressMessage = value;
                OnPropertyChanged("ProgressMessage");
            }
        }

        /// <summary>
        /// Get\Set the IsConverting. 
        /// Specify whether a converting operation is running or not
        /// </summary>
        public bool IsConverting
        {
            get { return _IsConverting; }
            set
            {
                _IsConverting = value;
                IsEnabled = !value;
                OnPropertyChanged("IsConverting");
            }
        }

        /// <summary>
        /// Get\Set the IsConverting. 
        // Specify whether the window is enabled.It is not enabled during converting operation
        /// </summary>
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set
            {
                _IsEnabled = value;
                OnPropertyChanged("IsEnabled");
            }
        }

        /// <summary>
        /// Get\Set the ExportProgressValue. 
        /// Used to update the progress bar value
        /// </summary>
        public int ExportProgressValue
        {
            get
            {
                return _ExportProgressValue;
            }
            set
            {
                _ExportProgressValue = value;
                OnPropertyChanged("ExportProgressValue");
            }
        }

        /// <summary>
        /// Get\Set the MaxProgressValue. 
        /// Used to set the max value of progress bar 
        /// </summary>
        public int MaxProgressValue
        {
            get
            {
                return _MaxProgressValue;
            }
            set
            {
                _MaxProgressValue = value;
                OnPropertyChanged("MaxProgressValue");
            }
        }

        /// <summary>
        /// Get\Set the Converting Percentage Message
        /// Specify the percentage message [0%..100%] shown in converting operation
        /// </summary>
        public string ConvertingPercentageMessage
        {
            get { return _ConvertingPercentageMessage; }
            set
            {
                _ConvertingPercentageMessage = value;
                OnPropertyChanged("ConvertingPercentageMessage");
            }
        }

        /// <summary>
        /// Get\Set the Report file name path
        /// The path where the report will be saved
        /// </summary>
        public string ReportFileNamePath
        {
            get
            {
                return _ReportFileNamePath;
            }
            set
            {
                _ReportFileNamePath = value;
                OnPropertyChanged("ReportFileNamePath");
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// The Abort Command
        /// </summary>
        private ICommand _AbortCommand;
        public ICommand AbortCommand
        {
            get { return _AbortCommand; }
            set { _AbortCommand = value; }
        }

        /// <summary>
        /// The Exit Command
        /// </summary>
        private ICommand _ExitCommand;
        public ICommand ExitCommand
        {
            get { return _ExitCommand; }
            set { _ExitCommand = value; }
        }

        #endregion

        #region Event Handler

        public event EventHandler Closed;        

        #endregion

        #region Actions

        public Action CloseAction { get; set; }

        #endregion

        #region Constructor

        public CreateReportViewModel()
        {
            DialogServiceKey = (int)Common.DialogServiceKey.CreateReport;

            // Create background worker
            _BackgroundWorker = new ExtendedBackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            // Create the Commands
            AbortCommand = new RelayCommand(AbortExecute);
            ExitCommand = new RelayCommand(ExitExecute);

            // Set properties
            IsConverting = false;
            MaxProgressValue = 100;

            // Create the Collections
            _CreatedReportPaths = new List<string>();

            // Register events
            RegisterEvents(true);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Register\Unregister events
        /// </summary>
        private void RegisterEvents(bool register)
        {
            if (register)
            {                
            }
            else
            {             
            }           
        }

        /// <summary>
        /// Register\Unregister the Backgorund worker events
        /// </summary>
        private void RegisterCreationReportEvents(bool register)
        {
            if (register)
            {
                if (_PdfCreatorManager != null)
                {
                    _PdfCreatorManager.ReportCreationStarted += OnReportCreationStarted;
                    _PdfCreatorManager.ReportCreated += OnReportCreated;
                    _PdfCreatorManager.MaxProgressValueChanged += OnMaxProgressValueChanged;
                }

                if (_BackgroundWorker != null)
                {
                    _BackgroundWorker.DoWork += OnBackGroundWorkerDoWork;
                    _BackgroundWorker.ProgressChanged += OnBackGroundWorkerProgressChanged;
                    _BackgroundWorker.RunWorkerCompleted += OnBackgroundWorkerCompleted;
                    _PdfCreatorManager.MaxProgressValueChanged += OnMaxProgressValueChanged;
                }
            }
            else
            {
                if (_PdfCreatorManager != null)
                {
                    _PdfCreatorManager.ReportCreationStarted -= OnReportCreationStarted;
                    _PdfCreatorManager.ReportCreated -= OnReportCreated;
                }

                if (_BackgroundWorker != null)
                {
                    _BackgroundWorker.DoWork -= OnBackGroundWorkerDoWork;
                    _BackgroundWorker.ProgressChanged -= OnBackGroundWorkerProgressChanged;
                    _BackgroundWorker.RunWorkerCompleted -= OnBackgroundWorkerCompleted;
                }
            }
        }       

        /// <summary>
        /// Create a new Report
        /// </summary>
        public void CreateReport(CreateReportStatusEnum createReportStatus, string additionalStatusMessage, string reportFileNamePath, bool showPreview, string logoPath, CommonLog log, IPdfCreatorManager pdfCreatorManager)
        {
            IsConverting = true;

            // Set Properties
            ExportProgressValue = 0;
            ConvertingPercentageMessage = "0";

            // Cleare the Collection
            _CreatedReportPaths?.Clear();

            // Create background worker
            _BackgroundWorker = new ExtendedBackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
           
            _LogoPath = logoPath;
            _ShowPreview = showPreview;
            _Log = log;
            _PdfCreatorManager = pdfCreatorManager;
            _CreateReportStatus = createReportStatus;
            _AdditionalStatusMessage = additionalStatusMessage;

            // Create the Visible path
            if (createReportStatus != CreateReportStatusEnum.Error)
            {
                _CreatedReportFileName = GetSavePathFileNameReport(reportFileNamePath);
                ReportFileNamePath = CreateTheFormattedPath(_CreatedReportFileName);
            }
            else
            {
                ProgressMessage = GetStatusMessage(createReportStatus, additionalStatusMessage);
            }

            // Register the BackGroundWorker events
            RegisterCreationReportEvents(true);


            // Set properties
            IsConverting = true;

            // Create the Report
            _BackgroundWorker?.RunWorkerAsync();
        }

        /// <summary>
        /// In the Path the "//" with ">"
        /// </summary>
        private string CreateTheFormattedPath(string orignalPath)
        {
            var newString = orignalPath.Replace("\\", "//");
           return newString.Replace("//", ">");
        }

        /// <summary>
        /// Returns the path + file name of the saved report
        /// </summary>
        private string GetSavePathFileNameReport(string originalPath)
        {
            string fileName = string.Empty;

            try
            {
                var path = System.IO.Path.GetDirectoryName(originalPath);
                var fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(originalPath);

                int index = 0;
                fileName = originalPath;
                while (File.Exists(fileName))
                {
                    index = index + 1;
                    fileName = $@"{path}\{fileNameWithoutExtension}_{index}.pdf";
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }

            return fileName;
        }

        /// <summary>
        /// Returns a message associated to the Status of creating operation
        /// </summary>
        private string GetStatusMessage(CreateReportStatusEnum createReportStatus, string additionalMessage = null)
        {
            var message = string.Empty;
            switch (createReportStatus)
            {
                case CreateReportStatusEnum.Creating: message = CultureResources.GetString("Message_Creating_Report"); break;
                case CreateReportStatusEnum.Success: message = CultureResources.GetString("Message_Report_Created_Successfully"); break;
                case CreateReportStatusEnum.Warning: message = CultureResources.GetString("Message_Warning"); break;
                case CreateReportStatusEnum.Error: message = CultureResources.GetString("Message_Creating_Report_Error"); break;
            }

            if (!string.IsNullOrEmpty(additionalMessage))
            {
                message = string.Format(message, additionalMessage);
            }

            return message;
        }

        /// <summary>
        /// Trigger the closed event
        /// </summary>
        private void ClosedEventTriggered(object sender, EventArgs eventArgs)
        {
            Closed?.Invoke(sender, eventArgs);
        }

        /// <summary>
        /// Exit 
        /// 1. Send message to the Window
        /// 2. Unregister the Backgroundworker events
        /// 3. Trigger the ClosedEvent 
        /// </summary>
        private void Exit()
        {
            // Close Window
            CloseAction();

            // Unregister the BackGroundWorker events
            RegisterCreationReportEvents(false);

            // Trigger the event
            ClosedEventTriggered(this, EventArgs.Empty);
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs to Create the Report
        /// </summary>
        public void CreateReportExecute(object param)
        {
            _BackgroundWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Occurs when the back ground worker is completed
        /// </summary>
        private void OnBackgroundWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Change the Progress Message
            ProgressMessage = GetStatusMessage(CreateReportStatus, _AdditionalStatusMessage);
            IsConverting = false;
        }

        /// <summary>
        /// Occurs when the BackGround worker starts
        /// </summary>
        private void OnBackGroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            var mustExport = true;

            while (mustExport)
            {
                var backgroundWorker = (sender as ExtendedBackgroundWorker);

                try
                {
                    IsConverting = true;                                       

                    // Change the Progress Message
                    ProgressMessage = GetStatusMessage(CreateReportStatus, _AdditionalStatusMessage);

                    // Create the report
                    _PdfCreatorManager.CreateReport(_CreatedReportFileName, _LogoPath, backgroundWorker);

                    // Change the staus
                    CreateReportStatus = CreateReportStatusEnum.Success;
                }
                catch (DirectoryNotFoundException ex)
                {
                    _Log.Debug(ex.Message);

                    // Change the staus
                    CreateReportStatus = CreateReportStatusEnum.Error;
                    _AdditionalStatusMessage = Constants.DirectoryNotFoundError;
                }
                catch (Exception ex)
                {
                    _Log.Debug(ex.Message);

                    // Change the staus
                    CreateReportStatus = CreateReportStatusEnum.Error;
                    _AdditionalStatusMessage = Constants.UnknownError;

                    if (!(ex is IOException))
                    {
                        mustExport = true;
                        break;
                    }
                }
                finally
                {
                    MaxProgressValue = 100;
                    backgroundWorker.ReportProgressWithExeption(100);
                }

                mustExport = false;
            }
        }

        /// <summary>
        /// Occurs when the Report Creation is just started
        /// </summary>
        private void OnReportCreationStarted(object sender, string e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                ExportProgressValue = 0;
                ConvertingPercentageMessage = "0";
                ReportFileNamePath = CreateTheFormattedPath(e);
            }));
        }

        /// <summary>
        /// Occurs when a Report is created
        /// </summary>
        private void OnReportCreated(object sender, string e)
        {
            if (!string.IsNullOrEmpty(e))
            {
                _CreatedReportPaths.Add(e);
            }
        }

        /// <summary>
        /// Occurs when the Max value of progress bar is changed
        /// </summary>
        private void OnMaxProgressValueChanged(object sender, int e)
        {
            MaxProgressValue = e;
        }

        /// <summary>
        /// Occurs when the BackGround worker change the progress
        /// </summary>
        private void OnBackGroundWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (MaxProgressValue > 0)
                {
                    ExportProgressValue = (int)(((decimal)e.ProgressPercentage / (decimal)MaxProgressValue) * 100);

                    var actualValue = (int)(((decimal)ExportProgressValue / (decimal)MaxProgressValue)*100);
                    ConvertingPercentageMessage = (actualValue).ToString();
                }
            }));
        }        

        /// <summary>
        /// Abort the Creation of the Report
        /// </summary>
        public void AbortExecute(object param)
        {
            _BackgroundWorker.CancelAsync();

            try
            {
                // Delete the Created Reports
                foreach (var createdReportPath in _CreatedReportPaths)
                {
                    if (!string.IsNullOrEmpty(createdReportPath) && File.Exists(createdReportPath))
                    {
                        File.Delete(createdReportPath);
                    }
                }

                _CreatedReportPaths.Clear();

                // Exit
                // 1. Send message to the Window
                // 2. Unregister the Backgroundworker events
                // 3. Trigger the ClosedEvent 
                Exit();
            }
            catch(Exception ex)
            {
                _Log.Debug(ex.Message);
            }
        }

        /// <summary>
        /// Close the Window
        /// </summary>
        public void ExitExecute(object param)
        {
            if (!string.IsNullOrEmpty(_CreatedReportFileName) && _ShowPreview
                && _CreateReportStatus == CreateReportStatusEnum.Success)
            {
                // Delete the Created Reports
                foreach (var createdReportPath in _CreatedReportPaths)
                {
                    if (!string.IsNullOrEmpty(createdReportPath) && File.Exists(createdReportPath))
                    {
                        Process.Start(createdReportPath);
                    }
                }

                _CreatedReportPaths.Clear();
                //if (File.Exists(_CreatedReportFileName))
                //{
                //    Process.Start(_CreatedReportFileName);
                //}
            }

            // Exit
            // 1. Send message to the Window
            // 2. Unregister the Backgroundworker events
            // 3. Trigger the ClosedEvent 
            Exit();
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Dispose the object
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);

            // Unregister events
            RegisterEvents(false);

            _BackgroundWorker.Dispose();
        }

        #endregion
    }
}
