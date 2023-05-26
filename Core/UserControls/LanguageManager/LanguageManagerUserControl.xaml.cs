using Core.ResourceManager.Cultures;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Core
{
    /// <summary>
    /// Interaction logic for LanguageManagerUserControl.xaml
    /// </summary>
    public partial class LanguageManagerUserControl : UserControl
    {
        #region Properties

        /// <summary>
        /// It is the ItemSource associated to the ListView.
        /// Contains all theflag images and the culture name.
        /// </summary>
        public ObservableCollection<LanguageItem> ItemsSource
        {
            get { return (ObservableCollection<LanguageItem>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// It is the Selected Item
        /// </summary>
        public LanguageItem SelectedItem
        {
            get { return (LanguageItem)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// It is the Choosen Language Code
        /// </summary>
        public string ChoosenLanguageCode
        {
            get { return (string)GetValue(ChoosenLanguageCodeProperty); }
            set { SetValue(ChoosenLanguageCodeProperty, value); }
        }

        /// <summary>
        /// It is the Choosen Language  ID
        /// </summary>
        public LanguageID ChoosenLanguageID
        {
            get { return (LanguageID)GetValue(ChoosenLanguageIDProperty); }
            set { SetValue(ChoosenLanguageIDProperty, value); }
        }

        #endregion

        #region Dependency Properties

        private static readonly DependencyProperty ItemsSourceProperty =
          DependencyProperty.Register("ItemsSource", typeof(ObservableCollection<LanguageItem>), typeof(LanguageManagerUserControl), new PropertyMetadata());

        private static readonly DependencyProperty SelectedItemProperty =
         DependencyProperty.Register("SelectedItem", typeof(LanguageItem), 
             typeof(LanguageManagerUserControl), new PropertyMetadata(OnSelectedItemChangedCallBack));

        private static readonly DependencyProperty ChoosenLanguageCodeProperty =
         DependencyProperty.Register("ChoosenLanguageCode", typeof(string), typeof(LanguageManagerUserControl), new PropertyMetadata());

        private static readonly DependencyProperty ChoosenLanguageIDProperty =
         DependencyProperty.Register("ChoosenLanguageID", typeof(LanguageID), typeof(LanguageManagerUserControl), new PropertyMetadata());

        #endregion

        #region Constructor

        public LanguageManagerUserControl()
        {
            InitializeComponent();

            // Set the DataContext
            LayoutRoot.DataContext = this;

            ItemsSource = new ObservableCollection<LanguageItem>()
            {
                new LanguageItem(LanguageID.English, "Label_English", "eng" , Properties.Resources.English.ToBitmapImage()),
                new LanguageItem(LanguageID.USA, "Label_USA", "eng" , Properties.Resources.USA.ToBitmapImage()),
                new LanguageItem(LanguageID.Italy, "Label_Italy", "it", Properties.Resources.Italy.ToBitmapImage()),
                new LanguageItem(LanguageID.French, "Label_France", "fr", Properties.Resources.France.ToBitmapImage()),
            };
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set the Language by the specificated code
        /// the code is: "it", "eng" etc...
        /// </summary>
        private void SetLanguage(string languageCode, string languageName)
        {
            var previusLanguage = ItemsSource.FirstOrDefault(l => l.IsApplyed);
            var language = ItemsSource.FirstOrDefault(l => l.CultureCode?.ToLower() == languageCode?.ToLower()
                                                        && l.CultureName?.ToLower() == languageName?.ToLower());
            if (previusLanguage != null && language != null && !language.IsApplyed)
            {
                previusLanguage.IsApplyed = false;
                language.IsApplyed = true;

                // Update the ChoosenLanguageCode
                ChoosenLanguageCode = language.CultureCode;
                ChoosenLanguageID = language.LanguageID;

                // Update the Language
                CultureResources.ChangeCulture(new CultureInfo(ChoosenLanguageCode));

                // Change the Language Item name
                foreach (var item in ItemsSource)
                {
                    item.ChangeLanguage();
                }
            }
        }        

        #endregion

        #region Events       

        /// <summary>
        /// The SelectedItem Changed callback. Used to set the Language
        /// </summary>
        private static void OnSelectedItemChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is LanguageManagerUserControl s)
            {
                s.SetLanguage(s?.SelectedItem?.CultureCode, s?.SelectedItem?.CultureName);
            }
        }           

        /// <summary>
        /// Occurs when the User Control is loaded
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var isLanguageSetted = false;

            // Set the active item (current language)
            if (ChoosenLanguageCode != null)
            {
                var choosenLanguage = LanguageToLanguageID.Convert(ChoosenLanguageCode);
                var language = ItemsSource.FirstOrDefault(l => l.LanguageID == choosenLanguage);
                if (language != null)
                {
                    language.IsApplyed = true;
                    isLanguageSetted = true;
                }
            }

            if (!isLanguageSetted)
            {
                ItemsSource[0].IsApplyed = true;
            }
        }

        #endregion        
    }
}
