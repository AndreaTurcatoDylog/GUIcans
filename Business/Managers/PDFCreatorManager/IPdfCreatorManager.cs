using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public interface IPdfCreatorManager
    {
        event EventHandler<string> ReportCreationStarted;
        event EventHandler<string> ReportCreated;
        event EventHandler<int> MaxProgressValueChanged;
        void CreateReport(string pathFileName, string logoPath, ExtendedBackgroundWorker backgroundWorker = null);
    }
}
