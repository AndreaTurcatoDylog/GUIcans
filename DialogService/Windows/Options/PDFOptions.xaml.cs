using Business;
using Common;
using Core;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace DialogService
{
    /// <summary>
    /// Interaction logic for ImageOptions.xaml
    /// </summary>
    public partial class PDFOptions : DisposableUserControl
    {
        #region Properties

        /// <summary>
        /// Get\Set theOptions
        /// </summary>
        public Options Options
        {
            get { return (Options)GetValue(OptionsProperty); }
            set { SetValue(OptionsProperty, value); }
        }

        /// <summary>
        /// Get\Set the Logo Image
        /// </summary>
        public BitmapImage LogoImage
        {
            get { return (BitmapImage)GetValue(LogoImageProperty); }
            set { SetValue(LogoImageProperty, value); }
        }

        #endregion

        #region Dependency Properties

        private static readonly DependencyProperty OptionsProperty =
        DependencyProperty.Register("Options", typeof(Options), typeof(PDFOptions),
            new PropertyMetadata());

        private static readonly DependencyProperty LogoImageProperty =
          DependencyProperty.Register("LogoImage", typeof(BitmapImage), typeof(PDFOptions), 
              new PropertyMetadata());        

        #endregion

        #region Constructor

        public PDFOptions()
        {
            InitializeComponent();

            // Set the DataContext
            LayoutRoot.DataContext = this;
        }

        #endregion

        #region Events        

        /// <summary>
        /// OCcurs when the logo must be choosen
        /// </summary>
        private void OnChooseLogoImage(object sender, RoutedEventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "all (*.*)|*.*|bmp (*.bmp)|*.bmp|jpg (*.jpg)|*.jpg|tif (*.tif)|*.tif|png (*.png)|*.png";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    var logoFilePath = openFileDialog.FileName;
                    if (logoFilePath != null)
                    {                        
                        Options.LogoImage = Helper.HelperBitmapImage.CreateFromFile(logoFilePath);
                    }
                }
            }
        }

        #endregion
    }
}
