using System.Windows;

namespace Core
{
    /// <summary>
    /// Interaction logic for DynamicBitNumberSettings.xaml
    /// </summary>
    public partial class DynamicBitNumberSettings : DisposableUserControlBase
    {
        #region Properties

        /// <summary>
        /// Get\Set the Setting Bit number
        /// </summary>
        public SettingBitNumber SettingBitNumber
        {
            get { return (SettingBitNumber)GetValue(SettingBitNumberProperty); }
            set { SetValue(SettingBitNumberProperty, value); }
        }

        /// <summary>
        /// Get\Set the Integer Number
        /// </summary>
        public int IntegerNumber
        {
            get { return (int)GetValue(IntegerNumberProperty); }
            set { SetValue(IntegerNumberProperty, value); }
        }

        #endregion

        #region Dependency Properties

        private static readonly DependencyProperty SettingBitNumberProperty =
          DependencyProperty.Register("SettingBitNumber", typeof(SettingBitNumber), typeof(DynamicBitNumberSettings));

        private static readonly DependencyProperty IntegerNumberProperty =
          DependencyProperty.Register("IntegerNumber", typeof(int), typeof(DynamicBitNumberSettings));

        #endregion

        #region Constructor

        public DynamicBitNumberSettings()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this;
        }

        #endregion

        #region Methods

        #endregion
    }
}
