using Business;
using Common;
using Core;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace SpeedDetailsImageVisualizer
{
    /// <summary>
    /// Interaction logic for CreateReportWindow.xaml
    /// </summary>
    public partial class CreateReportWindow : System.Windows.Window
    {       
        #region Constructor

        public CreateReportWindow(CreateReportStatusEnum createReportStatus, string additionalStatusMessage, string reportFileNamePath, string logoPath, bool showPreview, VersionTypeEnum versionType, CommonLog log, IPdfCreatorManager pdfCreatorManager)
        {
            InitializeComponent();

            Style style = null;
            switch(versionType)
            {
                case VersionTypeEnum.FullVersion:
                    style = Application.Current.FindResource("CustomLittleButtonBackStyle") as Style;
                    break;
                case VersionTypeEnum.LiteVersion:
                    style = Application.Current.FindResource("CustomLittleButtonOKStyle") as Style;
                    break;
            }

            if (style != null)
            {
                ExitButton.Style = style;
            }           
        }

        #endregion        
    }
}
