using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Core
{
    /// <summary>
    /// Created and extended Background worker.
    /// Added:
    /// 1) An increment Progress (a reset method usually must be called)
    /// 2) A Report with exception. If the Background worker is aborted an Exception will be thrown.
    ///    It can be used to terminate an aborted operation.
    /// </summary>
    public class ExtendedBackgroundWorker : BackgroundWorker
    {
        #region Fields

        private int _Step;

        #endregion

        #region Methods

        /// <summary>
        /// Reset the value of Background worker
        /// </summary>
        public void Reset()
        {
            try
            {
                _Step = 0;
                ReportProgress(_Step);
            }
            catch (Exception)
            {
                throw new ExtendedBackgroundWorkerAbortedException();
            }
        }

        /// <summary>
        /// Set the progress. If the worker is aborted an Exception is thrown
        /// </summary>
        public void ReportProgressWithExeption(int percentage)
        {
            if (!CancellationPending)
            {
                _Step = percentage;
                ReportProgress(percentage);
            }
            else
            {
                throw new ExtendedBackgroundWorkerAbortedException();
            }
        }

        /// <summary>
        /// Increment the progress. If the worker is aborted an Exception is thrown
        /// </summary>
        public void ReportIncrementProgressWithExeption(int incrementStep)
        {
            if (!CancellationPending)
            {
                _Step = _Step + incrementStep;
                ReportProgress(_Step);
            }
            else
            {
                throw new ExtendedBackgroundWorkerAbortedException();
            }
        }

        #endregion
    }
}
