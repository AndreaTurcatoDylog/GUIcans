using Core.ResourceManager.Cultures;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Core
{
    /// <summary>
    /// Interaction logic for SettingNavigator.xaml
    /// </summary>
    public partial class SettingNavigator : DisposableUserControl
    {
        #region Constants

        public readonly string NavigatorHomeButtonStyle = "NavigatorHomeButtonStyle";
        public readonly string NavigatorButtonStyle =  "NavigatorButtonStyle";
        public readonly string LabelHome = "Label_Home";

        #endregion

        #region Members

        private List<List<ISettingItem>> _Navigator;
        private List<string> _NavigatorButtonLabels;

        private List<ISettingItem> UpdatedSettingItems;

        // A list of the nodes (single node = ISettingItem) in a path 
        // (path= 'Home' -> Node_1 -> Node_2 -> ... -> Node_N)
        private List<ISettingItem> _SettingItemsNodes;
        private int _Tag;

        // A list of nodes where to check the update information
        private List<ISettingItem>  _SettingItemsNodesToUpdate;

       // private int _NumberOfUpdatedElements;

        #endregion

        #region Properties

        public ObservableCollection<ImageButton> NavigatorButtons { get; set; }

        /// <summary>
        /// Get\Set the Item Source
        /// </summary>
        public ObservableCollection<ISettingItem> ItemSource
        {
            get { return (ObservableCollection<ISettingItem>)GetValue(ItemSourceProperty); }
            set {SetValue(ItemSourceProperty, value); }
        }

        /// <summary>
        /// Get\Set the information whether exists almost one updated ISettingItem
        /// </summary>
        public bool ExistUpdateElements
        {
            get { return (bool)GetValue(ExistUpdateElementsProperty); }
            set {
                SetValue(ExistUpdateElementsProperty, value);
            }
        }

        [Bindable(true)]
        /// <summary>
        /// Get\Set the information whether exists almost one updated ISettingItem
        /// </summary>
        public bool Restore
        {
            get { return (bool)GetValue(RestoreProperty);}
            set{ SetValue(RestoreProperty, value);}
        }

        /// <summary>
        /// Get\Set the number of updated elements
        /// </summary>
        public int NumberOfUpdatedElements
        {
            get { return (int)GetValue(NumberOfUpdatedElementsProperty); }
            set
            {
                SetValue(NumberOfUpdatedElementsProperty, value);
                ExistUpdateElements = value > 0;
            }
        }

        #endregion

        #region Dependency Properties

        private static readonly DependencyProperty ItemSourceProperty =
          DependencyProperty.Register("ItemSource", typeof(ObservableCollection<ISettingItem>), typeof(SettingNavigator));

        private static readonly DependencyProperty ExistUpdateElementsProperty =
         DependencyProperty.Register("ExistUpdateElements", typeof(bool), typeof(SettingNavigator));

        private static readonly DependencyProperty NumberOfUpdatedElementsProperty =
         DependencyProperty.Register("NumberOfUpdatedElements", typeof(int), typeof(SettingNavigator));

        private static readonly DependencyProperty RestoreProperty =
        DependencyProperty.Register("Restore", typeof(bool), typeof(SettingNavigator));

        #endregion

        #region Constructor

        public SettingNavigator()
        {
            InitializeComponent();

            NumberOfUpdatedElements = 0;
            UpdatedSettingItems = new List<ISettingItem>();

            NavigatorButtons = new ObservableCollection<ImageButton>();
            _NavigatorButtonLabels = new List<string>();

            _SettingItemsNodes = new List<ISettingItem>();
            _Navigator = new List<List<ISettingItem>>();

            _Tag = 0;

            // Set the data context
            LayoutRoot.DataContext = this;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Register\Unregister events
        /// </summary>
        protected override void RegisterEvents(bool register)
        {
            if (register)
            {
                DynamicSettigs.LinkClicked += OnDynamicSettigsLinkClicked;
                DynamicSettigs.ValueUpdated += OnDynamicSettigsValueUpdated;
                Settings.Instance.CultureInfoChanged += OnCultureInfoChanged;
            }
            else
            {
                DynamicSettigs.LinkClicked -= OnDynamicSettigsLinkClicked;
                DynamicSettigs.ValueUpdated -= OnDynamicSettigsValueUpdated;
                Settings.Instance.CultureInfoChanged -= OnCultureInfoChanged;
                DisposeButtonList();
            }
        }

        /// <summary>
        /// Occues in onLoaded event
        /// </summary>
        protected override void Load()
        {
            var content = CultureResources.GetString(LabelHome);
            NavigatorButtons.Add(CreateNavigatorButton(content, 0, NavigatorHomeButtonStyle));
            _NavigatorButtonLabels.Add(LabelHome);

            _SettingItemsNodes.Add(new SettingLink(LabelHome));
        }

        /// <summary>
        /// Delete all navigation buttons in list from choosen position to end
        /// </summary>
        private void DeleteAllButtonsStartingByPosition(int position)
        {
            for (int index = NavigatorButtons.Count() - 1; index >= position; index--) {

                // Delete navigation button and dispose the click event
                var navigatorButton = NavigatorButtons[index];
                navigatorButton.Click -= new RoutedEventHandler(OnNavigationButtonClick);
                NavigatorButtons.Remove(navigatorButton);

                // Delete node in path
                _SettingItemsNodes.RemoveAt(index);

                _Navigator.RemoveAt(index);
                _NavigatorButtonLabels.RemoveAt(index);

                _Tag--;
            }
        }

        /// <summary>
        /// Create a new navigator button
        /// </summary>
        private ImageButton CreateNavigatorButton(string content, int nodeIndex, string styleName)
        {
            var style = styleName;

            var length = content?.Length;
            if (length > 16 && length<=22)
            {
                style = $"{styleName}_002";
            }
            else if (length > 22 && length<=28)
            {
                style = $"{styleName}_003";
            }
            else if (length > 28 && length<32)
            {
                style = $"{styleName}_004";
            }
            else if (length >= 32)
            {
                style = $"{styleName}_004";
                content = $"{content.Substring(0, 32)}...";
            }

            var navigatorButton = new ImageButton()
            {
                Style = Application.Current.FindResource(style) as Style,
                Content = content?.ToUpper(),
                Tag = nodeIndex
            };

            // Add click event to new navigator button
            navigatorButton.Click += new RoutedEventHandler(OnNavigationButtonClick);

            return navigatorButton;
        }

        /// <summary>
        /// Change the content of all navigator buttons in according to culture info
        /// </summary>
        private void ChangeNavigatorButtonsLanguage()
        {
            var index = 0;
            foreach (ImageButton navigatorButton in NavigatorButtons)
            {
                var key = _NavigatorButtonLabels[index];
                var content = CultureResources.GetString(key);
                navigatorButton.Content = content?.ToUpper();

                index++;
            }
        }

        /// <summary>
        /// Dispose the event associated to navigator buttons
        /// </summary>
        private void DisposeButtonList()
        {
            if (NavigatorButtons != null)
            {
                foreach (var navigatorButton in NavigatorButtons)
                {
                    navigatorButton.Click -= new RoutedEventHandler(OnNavigationButtonClick);
                }
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when a navigator button is pressed.
        /// It deletes all the navigation buttons forward
        /// </summary>
        protected void OnNavigationButtonClick(object sender, EventArgs e)
        {
            var elements = ItemSource;

            if (!_Navigator.Any())
            {
                _Navigator.Add(new List<ISettingItem>(ItemSource));
            }

            Button button = sender as Button;
            var position = (int)button.Tag;


            ItemSource = new ObservableCollection<ISettingItem>(_Navigator.ElementAt(position));

            // Set the update information for all link
            SetLinkUpdateInformation();

            // Delate all unused buttons 
            DeleteAllButtonsStartingByPosition(position + 1);
        }

        /// <summary>
        /// Set the update information for all links
        /// </summary>
        private void SetLinkUpdateInformation()
        {
            var list = _SettingItemsNodesToUpdate;
            if (list != null && list.Any())
            {
                foreach (var settingItem in list)
                {
                    settingItem?.IsSettingItemUpdated();
                }

                _SettingItemsNodesToUpdate.Clear();
                _SettingItemsNodesToUpdate = null;
            }
        }

        private void AddToUpdatedItemList(ISettingItem settingItem)
        {
            if (!UpdatedSettingItems.Contains(settingItem))
            {
                UpdatedSettingItems.Add(settingItem);
            }
        }

        private void AddToUpdatedItemList(IEnumerable<ISettingItem> settingItems)
        {
            foreach (var settingItem in settingItems)
            {
                if (!UpdatedSettingItems.Contains(settingItem))
                {
                    UpdatedSettingItems.Add(settingItem);
                }
            }
        }

        /// <summary>
        /// After a save operation all modified ISettingItems must update their original value
        /// </summary>
        public void UpdatedOriginalValues()
        {
            NumberOfUpdatedElements = 0;
            if (UpdatedSettingItems != null && UpdatedSettingItems.Count() > 0)
            {
                foreach (var settingItem in UpdatedSettingItems)
                {
                    if (settingItem is ISettingWithValue)
                    {
                        (settingItem as ISettingWithValue).UpdateOriginalValue();
                    }
                    else
                    {
                        settingItem.IsUpdated = false;
                    }
                }
            }
        }

        /// <summary>
        /// Remove the changes in the object restoring the original value
        /// </summary>
        public void RestoreOriginalValues()
        {
            NumberOfUpdatedElements = 0;
            if (UpdatedSettingItems != null && UpdatedSettingItems.Count() > 0)
            {
                foreach (var settingItem in UpdatedSettingItems)
                {
                    if (settingItem is ISettingWithValue)
                    {
                        (settingItem as ISettingWithValue).RestoreOriginalValue();
                    }
                    else
                    {
                        settingItem.IsUpdated = false;
                    }
                }
            }
        }

        /// <summary>
        /// Occurs when a setting link is clicked. This event perform the navigation beetween pages
        /// </summary>
        private void OnDynamicSettigsLinkClicked(object sender, EventArgs e)
        {
            // Get the event args
            var args = (SettingLinkEventArgs)e;
            if (args.SettingItemSender is SettingLinkUp)
            {
                var position = (int)NavigatorButtons[NavigatorButtons.Count - 2].Tag;
                ItemSource = new ObservableCollection<ISettingItem>(_Navigator.ElementAt(position));

                // Set the update information for all link
                SetLinkUpdateInformation();

                DeleteAllButtonsStartingByPosition(position + 1);
            }
            else 
            {
                if(!_Navigator.Any())
                {
                    _Navigator.Add(new List<ISettingItem>(ItemSource));
                }

                // Set the update information for all link
                SetLinkUpdateInformation();

                // Get the event args
                ItemSource = new ObservableCollection<ISettingItem>(args.SettingItems);
                _Tag++;

                // Add new node to path
                _SettingItemsNodes.Add(args.SettingItemSender);

                // Create new navigator button
                var content = CultureResources.GetString(args.Key);

                var buttonIndex = _SettingItemsNodes.Count() - 1;
                var navigatorButton = CreateNavigatorButton(content, buttonIndex, NavigatorButtonStyle);

                // Add items to collections
                NavigatorButtons.Add(navigatorButton);
                _Navigator.Add(new List<ISettingItem>(ItemSource));
                _NavigatorButtonLabels.Add(args.Key);
            }
        }

        /// <summary>
        /// Occurs when a ISettingItem changes its value
        /// </summary>
        private void OnDynamicSettigsValueUpdated(object sender, EventArgs e)
        {
            var args = (e as SettingUpdatedEventArgs);
            var isUpdated = args.SettingItem.IsUpdated;

            // Update the number of updated elements 
            NumberOfUpdatedElements = isUpdated ? NumberOfUpdatedElements + 1
                                                : NumberOfUpdatedElements - 1;

            // Update the update information of links 
            var list = _SettingItemsNodes.Where(i => i is SettingLinkBase).ToList();
            if (list != null && list.Any())
            {
                _SettingItemsNodesToUpdate = list;
            }

            // Add element to list
            AddToUpdatedItemList(args.SettingItem);
            AddToUpdatedItemList(_SettingItemsNodesToUpdate);
        }

        /// <summary>
        /// Occurs when the culture info is changed. 
        /// The new culture info is got from eventArgs of type CultureInfoEventArgs.
        /// When the event occurs all the displayed setting items and navigation button change their content
        /// in according to new culture info
        /// </summary>
        private void OnCultureInfoChanged(object sender, EventArgs e)
        {
            foreach (var settingItem in ItemSource)
            {
                if (e != null && settingItem?.Label != null)
                {
                    // Get the new culture info
                    var cultureInfo = (e as CultureInfoEventArgs).CultureInfo;

                    // This is a workaround. To fire the "propertyChanged" event in SettingItem Object the Label property
                    // must be changed, so the event is triggered and the converter"LabelToLanguageTextConverter"
                    // is fired. So the contents with new language will be set.
                    var temp = settingItem.Label;
                    settingItem.Label = temp;
                }
            }

            ChangeNavigatorButtonsLanguage();
        }

        #endregion
    }
}
