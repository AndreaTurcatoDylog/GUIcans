using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace Core
{
    /// <summary>
    /// This User Control rappresents the a container of object.
    /// The list of object is diveded in several page. 
    /// When a page is choosen the list is shown
    /// </summary>
    public partial class NavigationPager : DisposableUserControl
    {
        #region Fields

        /// <summary>
        /// Describes wheter the pagination button is created or not
        /// </summary>
        private bool _ButtonsCreated;

        /// <summary>
        /// The store of original list of items
        /// </summary>
        private IEnumerable _OriginalInnerList;

        /// <summary>
        /// The number of pages. It is calculated by NumberOfItems\NumberOfItemsInPage
        /// </summary>
        private int _NumberOfPages;

        /// <summary>
        /// The total number of items in list
        /// </summary>
        private int _NumberOfItems;

        #endregion

        #region Properties

        /// <summary>
        /// Get\Set the visual component with a list that will be paged
        /// </summary>
        public UIElement InnerItemControl
        {
            get { return (UIElement)GetValue(InnerItemControlProperty); }
            set { SetValue(InnerItemControlProperty, value); }
        }

        [Bindable(true)]
        /// <summary>
        /// Get\Set the associated GroupName. The pages are a set of RadioButton of the same GroupName
        /// </summary>
        public string PageGroupName
        {
            get
            {
                return (string)GetValue(PageGroupNameProperty);
            }
            set
            {
                SetValue(PageGroupNameProperty, value);
            }
        }

        [Bindable(true)]
        /// <summary>
        /// Get\Set the number of items in a single page
        /// </summary>
        public int NumberOfItemsInPage
        {
            get
            {
                return (int)GetValue(NumberOfItemsInPageProperty);
            }
            set
            {
                SetValue(NumberOfItemsInPageProperty, value);
            }
        }

        //[Bindable(true)]
        /// <summary>
        /// Get\Set The source of items. This source will be internally stored and it will be binded to 
        /// the InnerItemControl component
        /// </summary>
        public IEnumerable ItemsSource
        {
            get
            {
                return (IEnumerable)GetValue(ItemsSourceProperty);
            }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        /// <summary>
        /// Get\Set the list of pages. 
        /// </summary>
        public ObservableCollection<IPaginationButton> Pages
        {
            get
            {
                return (ObservableCollection<IPaginationButton>)GetValue(PagesProperty);
            }
            set
            {
                SetValue(PagesProperty, value);
            }
        }

        /// <summary>
        /// Get\Set the Current opened page
        /// </summary>
        public int CurrentPage
        {
            get
            {
                return (int)GetValue(CurrentPageProperty);
            }
            set
            {
                SetValue(CurrentPageProperty, value);
            }
        }

        /// <summary>
        /// Get\Set the SelectedItem
        /// </summary>
        public object SelectedItem
        {
            get
            {
                return (object)GetValue(SelectedItemProperty);
            }
            set
            {
                SetValue(SelectedItemProperty, value);
            }
        }

        /// <summary>
        /// Get\Set the OnlyOnePageVisible.
        /// In case if only one pagination button exists, this property define whether 
        /// this single page must be visible or not
        /// </summary>
        public bool OnlyOnePageVisible
        {
            get
            {
                return (bool)GetValue(OnlyOnePageVisibleProperty);
            }
            set
            {
                SetValue(OnlyOnePageVisibleProperty, value);
            }
        }

        #endregion

        #region Dependency Properties

        public static DependencyProperty InnerItemControlProperty =
         DependencyProperty.Register("InnerItemControl", typeof(UIElement), typeof(NavigationPager));

        public static DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(NavigationPager), new PropertyMetadata(OnItemSourceChangedCallBack));

        private static readonly DependencyProperty PagesProperty =
          DependencyProperty.Register("Pages", typeof(ObservableCollection<IPaginationButton>), typeof(NavigationPager));

        private static readonly DependencyProperty PageGroupNameProperty =
          DependencyProperty.Register("PageGroupName", typeof(string), typeof(NavigationPager));

        private static readonly DependencyProperty NumberOfItemsInPageProperty =
         DependencyProperty.Register("NumberOfItemsInPage", typeof(int), typeof(NavigationPager), new PropertyMetadata(1));

        private static readonly DependencyProperty CurrentPageProperty =
         DependencyProperty.Register("CurrentPage", typeof(int), typeof(NavigationPager), new PropertyMetadata(OnCurrentPageChangedCallBack));

        private static readonly DependencyProperty SelectedItemProperty =
        DependencyProperty.Register("SelectedItem", typeof(object), typeof(NavigationPager), new PropertyMetadata(OnSelectedItemChangedCallBack));

        private static readonly DependencyProperty OnlyOnePageVisibleProperty =
        DependencyProperty.Register("OnlyOnePageVisible", typeof(bool), typeof(NavigationPager), new PropertyMetadata(true));

        #endregion

        #region Constructor

        /// <summary>
        /// The constructor
        /// </summary>
        public NavigationPager()
        {
            InitializeComponent();

            // Create the list of pages
            Pages = new ObservableCollection<IPaginationButton>();

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
            try
            {
                if (register)
                {
                    (InnerItemControl as Selector).SelectionChanged += OnSelectionChanged;
                }
                else
                {
                    (InnerItemControl as Selector).SelectionChanged -= OnSelectionChanged;

                    DisposePageList();
                }
            }
            catch(Exception ex)
            {
                CoreLog.Instance.Append($"[{ex.Source}] - {ex.Message} - {ex.StackTrace}", CoreLogType.Error);
            }
        }

        /// <summary>
        /// Create the pages.
        /// Calculate the number of pages and render the items into the first page
        /// Go always to the first page to render the Height of the list to the right value 
        /// (see the OnGridLayoutUpdated and OnCurrentPageChangedCallBack methods)
        /// </summary>
        public void LoadList()
        {
            // Dispose all the pages becouse new ones will be created
            DisposePageList();

            SelectedItem = null;

            if (ItemsSource != null)
            {
                // Store the original list
                _OriginalInnerList = ItemsSource.Cast<object>().ToList();

                // Get the number of items
                _NumberOfItems = ItemsSource.Cast<object>().ToList().Count();

                // Calculate the number of pages
                _NumberOfPages = _NumberOfItems / NumberOfItemsInPage;
                if (_NumberOfItems % NumberOfItemsInPage > 0)
                {
                    _NumberOfPages++;
                }

                if (_NumberOfPages == 0)
                {
                    _NumberOfPages = 1;
                }

                // Go always to the first page 
                GoToPage(0);
            }
        }

        /// <summary>
        /// Returns the range of the specificated page
        /// </summary>
        /// <param name="pageIndex"> The index of the page </param>
        /// <returns>  Returns a tuple that rappresent a range: (Item1 = Firt, Item2 = Last) </returns>
        private Tuple<int, int> GetRange(int pageIndex)
        {
            var first = (pageIndex * NumberOfItemsInPage);
            var last = (first + NumberOfItemsInPage) - 1;
            if (last > _NumberOfItems - 1)
            {
                last = _NumberOfItems - 1;
            }

            return Tuple.Create(first, last);
        }

        /// <summary>
        /// Create a new PaginationButton that rappresent a page
        /// </summary>
        private IPaginationButton CreatePage(int index, double height)
        {
            // Calculate the margin
            Thickness margin = (index == _NumberOfPages - 1) ? margin = new Thickness(30, 3, 3, 6) : margin = new Thickness(30, 3, 3, 0);

            // Create the page
            PaginationButton page = PaginationButtonFactory.Get(PaginationButtonType.Parameters);
            page.Width = 80;
            page.Height = height;
            page.Margin = margin;
            page.GroupName = PageGroupName;
            page.Tag = index;

            // Add mouse down event to new page
            page.PreviewMouseDown += OnPagePreviewMouseDown;

            return page;
        }

        /// <summary>
        /// Render the pagination buttons (the clickable button that show a specificated number of items).
        /// The Height dimension of each button depends from the Height of the list hosted in the ContentControl so
        /// this buttons must be created after the ContentControl is rendered.
        /// For this scenario this method is called in OnGridLayoutUpdated event fired when the layout of the grid that
        /// host the ContentControl is rendered.
        /// </summary>
        private void CreatePaginationButtons()
        {
            try
            {
                if (Pages.Count == 0)
                {
                    // Get the placemente and misure of the visual list
                    var placement = (InnerContentControl as FrameworkElement).GetAbsolutePlacement();
                    if (placement != null && placement.Height > 0)
                    {
                        var marginHeight = (_NumberOfPages - 1) * 3;
                        var height = (placement.Height - marginHeight) / _NumberOfPages;

                        for (var index = 0; index < _NumberOfPages; index++)
                        {
                            var page = CreatePage(index, height);
                            Pages.Add(page);
                        }

                        if (Pages.Any())
                        {
                            (Pages.ElementAt(CurrentPage) as PaginationButton).IsChecked = true;

                            if (_NumberOfPages == 1)
                            {
                                (Pages.ElementAt(0) as PaginationButton).IsChecked = true;

                                if (OnlyOnePageVisible)
                                {
                                    (Pages.ElementAt(0) as PaginationButton).Visibility = Visibility.Visible;
                                }
                                else
                                {
                                    (Pages.ElementAt(0) as PaginationButton).Visibility = Visibility.Hidden;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CoreLog.Instance.Append($"[{ex.Source}] - {ex.Message} - {ex.StackTrace}", CoreLogType.Error);
            }
        }

        /// <summary>
        /// Go to the specificated page and visualize the filter list
        /// </summary>
        private void GoToPage(int pageIndex)
        {
            try
            {
                // Get the range
                var range = GetRange(pageIndex);

                if (_OriginalInnerList != null)
                {
                    var numerOfItems = (range.Item2 - range.Item1) + 1;

                    var innerList = _OriginalInnerList.Cast<object>().ToList();

                    IList items = null;
                    if (range.Item1 + numerOfItems <= innerList.Count())
                    {
                        // Filter the items and bind a new list
                        items = innerList.GetRange(range.Item1, numerOfItems);
                    }
                    else
                    {
                        items = innerList.GetRange(range.Item1, innerList.Count());
                    }

                    if (items != null)
                    {
                        // Set the ItemsSource of the nested list control
                        (InnerItemControl as ItemsControl).ItemsSource = items;

                        // Set the selected item
                        var innerItemControl = (InnerItemControl as Selector);
                        if (SelectedItem != null)
                        {
                            innerItemControl.SelectedItem = SelectedItem;
                        }
                    }

                    CurrentPage = pageIndex;
                    if (Pages.Any())
                    {
                        (Pages.ElementAt(CurrentPage) as PaginationButton).IsChecked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                CoreLog.Instance.Append($"[{ex.Source}] - {ex.Message} - {ex.StackTrace}", CoreLogType.Error);
            }
        }

        /// <summary>
        /// Return the Page object by specificated IBaseItem
        /// </summary>
        public IPaginationButton GetPage(object item)
        {
            if (_OriginalInnerList != null && Pages != null)
            {
                var innerList = _OriginalInnerList.Cast<object>().ToList();
                if (innerList != null && innerList.Count > 0)
                {
                    var index = innerList.IndexOf(item);
                    var pageIndex = index / NumberOfItemsInPage;

                    return Pages[pageIndex];
                }
            }

            return null;
        }
        /// <summary>
        /// Return the Page index object by specificated IBaseItem
        /// </summary>
        public int? GetPageIndex(object item)
        {
            if (_OriginalInnerList != null && Pages != null)
            {
                var innerList = _OriginalInnerList.Cast<object>().ToList();
                if (innerList != null && innerList.Count > 0)
                {
                    var index = innerList.IndexOf(item);
                    var pageIndex = index / NumberOfItemsInPage;

                    return pageIndex;
                }
            }

            return null;
        }


        /// <summary>
        /// Dispose the event associated to pages
        /// </summary>
        private void DisposePageList()
        {
            if (Pages != null)
            {
                foreach (var page in Pages)
                {
                    (page as PaginationButton).PreviewMouseDown -= OnPagePreviewMouseDown;
                }
            }

            Pages.Clear();
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the Item Source changes
        /// </summary>
        private static void OnItemSourceChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var c = sender as NavigationPager;
            if (c != null && c.ItemsSource!=null)
            {
                c.LoadList();
            }
        }

        /// <summary>
        /// Occurs when the Item Source changes
        /// If the CurrentPage is not equals to the first page (index = 0) and if the CurrentPage
        /// has less items of the first page the Height of the list is more less then right value and
        /// The pagination buttons will be smaller than it should be.
        /// To prevent this scenario the list is rendered for the first page
        /// and after the buttons are rendered then it could be possible to go to the specificated page
        /// </summary>
        private static void OnCurrentPageChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                var c = sender as NavigationPager;
                if (c != null)
                {
                    var pageIndex = Convert.ToInt32(e.NewValue);

                    // if the pageIndex is not the first page index go to the specificated page only
                    // if buttons are already created
                    var stop = (pageIndex > 0 && !c._ButtonsCreated);

                    if (!stop)
                        c.GoToPage(pageIndex);
                }
            }
            catch(Exception ex)
            {
                CoreLog.Instance.Append($"[{ex.Source}] - {ex.Message} - {ex.StackTrace}", CoreLogType.Error);
            }
        }

        /// <summary>
        /// Occurs when the SelectedItem is changed
        /// </summary>
        private static void OnSelectedItemChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                var c = sender as NavigationPager;
                if (c != null)
                {
                    // Set the ItemsSource of the nested list control
                    var innerItemControl = (c.InnerItemControl as Selector);
                    var selectedItem = c.SelectedItem;
                    if (selectedItem != null)
                    {
                        innerItemControl.SelectedItem = selectedItem;
                    }
                    else
                    {
                        if (c._OriginalInnerList != null)
                        {
                            var innerList = c._OriginalInnerList.Cast<object>().ToList();
                            if (innerList.Count > 0)
                            {
                                var obj = innerList[0];
                                innerItemControl.SelectedItem = obj;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CoreLog.Instance.Append($"[{ex.Source}] - {ex.Message} - {ex.StackTrace}", CoreLogType.Error);
            }
        }

        /// <summary>
        /// Occurs when the Page is clicked with mouse
        /// </summary>
        private void OnPagePreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                var page = (sender as PaginationButton);

                // Go to the specificated list
                CurrentPage = Convert.ToInt32(page.Tag);
            }
            catch(Exception ex)
            {
                CoreLog.Instance.Append($"[{ex.Source}] - {ex.Message} - {ex.StackTrace}", CoreLogType.Error);
            }
        }

        /// <summary>
        /// Occurs when the layout of the grid that host the ContentControl is rendered.
        /// Call the method that render the pagination buttons (the clickable button that show a specificated number of items).
        /// The Height dimension of each button depends from the Height of the list hosted in the ContentControl so
        /// this buttons must be created after the ContentControl is rendered.
        /// </summary>
        private void OnGridLayoutUpdated(object sender, EventArgs e)
        {
            if (Pages != null && Pages.Count == 0)
            {
                // Create the pagination buttons
                CreatePaginationButtons();

                // The pagination is created only if Pages list have any items
                _ButtonsCreated = (Pages != null && Pages.Count> 0);
                if (_ButtonsCreated && CurrentPage > 0)
                {
                    // If the CurrentPage is not equals to the first page (index = 0) and if the CurrentPage
                    // has less items of the first page the Height of the list is more less then right value and
                    // The pagination buttons will be smaller than it should be.
                    // To prevent this scenario in OnCurrentPageChangedCallBack the list is rendered always by the first page
                    // in the LoadList method and after the buttons are rendered it could be possible 
                    // to go to the specificated page
                    GoToPage(CurrentPage);
                }
            }
        }

        /// <summary>
        /// Occurs when the selected item changes
        /// </summary>
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var innerItemControl = (InnerItemControl as Selector);
                if (innerItemControl.SelectedItem != null)
                {
                    SelectedItem = innerItemControl.SelectedItem;
                }
            }
            catch (Exception ex)
            {
                CoreLog.Instance.Append($"[{ex.Source}] - {ex.Message} - {ex.StackTrace}", CoreLogType.Error);
            }
        }

        #endregion
    }
}

