using Common;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public static class PdfCreatorFactory
    {
        public static IPdfCreatorManager Get(ReportTypeEnum reportTypeEnum, ICommonLog log, string[] batchFileContent, Dictionary<ReportAddedContentType, string> reportAddedContents, IList<string> imagesPath, string lot, string article, LocalSettings localSettings)
        {
            switch(reportTypeEnum)
            {
                case ReportTypeEnum.RerportWithBatch: return new PdfCreatorWithBatchContentManager(log, batchFileContent, imagesPath, localSettings.Title, 
                                                                     localSettings.MainSubtitle, localSettings.ImageSubtitle, lot, article,
                                                                     localSettings.NumberOfImageInOnePageLayout, localSettings.RotationAngle);

                case ReportTypeEnum.ReportWithActionAndCause: return new PdgCreatorWithCauseAndActionManager(log, reportAddedContents, imagesPath, localSettings.Title, 
                                                                       localSettings.MainSubtitle, lot, article,
                                                                       localSettings.NumberOfImageInOnePageLayout, localSettings.RotationAngle);
            }

            return null;
        }
    }
}
