using System.Windows;

namespace Core
{

    /// <summary>
    /// Interaction logic for DynamicRadioListSettings.xaml
    /// </summary>
    public partial class DynamicRadioListSettings : DisposableUserControlBase
    {

        #region Properties

        /// <summary>
        /// Get\Set the Setting Radio List
        /// </summary>
        public SettingRadioList SettingRadioList
        {
            get { return (SettingRadioList)GetValue(SettingRadioListProperty); }
            set { SetValue(SettingRadioListProperty, value); }
        }

        #endregion

        #region Dependency Properties

        private static readonly DependencyProperty SettingRadioListProperty =
          DependencyProperty.Register("SettingRadioList", typeof(SettingRadioList), typeof(DynamicRadioListSettings));

        #endregion

        #region Constructor

        public DynamicRadioListSettings()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this;
        }

        #endregion

        #region Methods

        private void ClearRadioList()
        {
            foreach(var radio in SettingRadioList.Radios)
            {
                (radio as SettingChecked).Checked = false;
            }
        }

        #endregion

        #region Events

        #endregion
    }
}
