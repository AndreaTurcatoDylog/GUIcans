using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Threading;

namespace Core
{

    public enum SliderButton
    {
        None = -1,
        Previus = 0,
        Next = 1,
    }

    /// <summary>
    /// Interaction logic for PaginationUserControl.xaml
    /// </summary>
    public partial class PaginationUserControl : DisposableUserControl
    {
        #region Members

        private SliderButton _CurrentSliderButton;
        private DispatcherTimer _SliderButtonsDispatcherTimer;

        #endregion

        #region Properties       

        /// <summary>
        /// The collection of pagination buttons
        /// </summary>
        public ObservableCollection<PaginationButton> PaginationButtons { get; set; }

        /// <summary>
        /// Get\Set the PageNumber.
        /// The PageNumber is Value+1 becouse the PageNumber is in [1.. TotalPages]
        /// </summary>
        public int PageNumber
        {
            get { return (int)GetValue(PageNumberProperty); }
            private set { SetValue(PageNumberProperty, value); }
        }

        /// <summary>
        /// Get\Set the TotalPages.
        /// The TotalPages number of total pages (PaginationButtons.Count)
        /// </summary>
        public int TotalPages
        {
            get { return (int)GetValue(TotalPagesProperty); }
            private set { SetValue(TotalPagesProperty, value); }
        }

        /// <summary>
        /// Get\Set the IsSliderVisible
        /// </summary>
        public bool IsSliderVisible
        {
            get { return (bool)GetValue(IsSliderVisibleProperty); }
            private set { SetValue(IsSliderVisibleProperty, value); }
        }

        /// <summary>
        /// Get\Set the SliderHeight
        /// </summary>
        public double SliderHeight
        {
            get { return (double)GetValue(SliderHeightProperty); }
            private set { SetValue(SliderHeightProperty, value); }
        }

        /// <summary>
        /// Get\Set the SliderWidth
        /// </summary>
        public double SliderWidth
        {
            get { return (double)GetValue(SliderWidthProperty); }
            private set { SetValue(SliderWidthProperty, value); }
        }

        /// <summary>
        /// Get\Set the Value. 
        /// When a Pagination Button is clicked a Value is returned
        /// </summary>
        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }        

        /// <summary>
        /// Get\Set the Number of Pagination Button
        /// </summary>
        public int NumberOfPage
        {
            get { return (int)GetValue(NumberOfPageProperty); }
            set { SetValue(NumberOfPageProperty, value); }
        }

        /// <summary>
        /// Get\Set the With of a single Pagination Button
        /// </summary>
        public double PaginationButtonWidth
        {
            get { return (double)GetValue(PaginationButtonWidthProperty); }
            set { SetValue(PaginationButtonWidthProperty, value); }
        }


        /// <summary>
        /// Get\Set the Height of a single Pagination Button
        /// </summary>
        public double PaginationButtonHeight
        {
            get { return (double)GetValue(PaginationButtonHeightProperty); }
            private set { SetValue(PaginationButtonHeightProperty, value); }
        }

        /// <summary>
        /// Get\Set the Group Name. 
        /// A group of Pagination Button must belong to the same group
        /// </summary>
        public string GroupName
        {
            get { return (string)GetValue(GroupNameProperty); }
            set { SetValue(GroupNameProperty, value); }
        }

        /// <summary>
        /// Get\Set the Number Of Elements
        /// Rappresent the total amount of elements
        /// </summary>
        public int NumberOfElements
        {
            get { return (int)GetValue(NumberOfElementsProperty); }
            set { SetValue(NumberOfElementsProperty, value); }
        }

        /// <summary>
        /// Get\Set the Number Of Elements in a single page
        /// </summary>
        public int NumberOfElementInPage
        {
            get { return (int)GetValue(NumberOfElementInPageProperty); }
            set { SetValue(NumberOfElementInPageProperty, value); }
        }
       
        #endregion

        #region Dependency Properties
      
        private static readonly DependencyProperty ValueProperty =
          DependencyProperty.Register("Value", typeof(int), typeof(PaginationUserControl), new PropertyMetadata(ValueChangedCallBack));

        private static readonly DependencyProperty TotalPagesProperty =
         DependencyProperty.Register("TotalPages", typeof(int), typeof(PaginationUserControl), new PropertyMetadata(0));

        private static readonly DependencyProperty PageNumberProperty =
         DependencyProperty.Register("PageNumber", typeof(int), typeof(PaginationUserControl), new PropertyMetadata(0));

        private static readonly DependencyProperty IsSliderVisibleProperty =
         DependencyProperty.Register("IsSliderVisible", typeof(bool), typeof(PaginationUserControl), new PropertyMetadata(false));

        private static readonly DependencyProperty NumberOfPageProperty =
         DependencyProperty.Register("NumberOfPage", typeof(int), typeof(PaginationUserControl));

        private static readonly DependencyProperty NumberOfElementInPageProperty =
         DependencyProperty.Register("NumberOfElementInPage", typeof(int), typeof(PaginationUserControl));

        private static readonly DependencyProperty GroupNameProperty =
         DependencyProperty.Register("GroupName", typeof(string), typeof(PaginationUserControl));

        private static readonly DependencyProperty PaginationButtonWidthProperty =
         DependencyProperty.Register("PaginationButtonWidth", typeof(double), typeof(PaginationUserControl));

        private static readonly DependencyProperty PaginationButtonHeightProperty =
         DependencyProperty.Register("PaginationButtonHeight", typeof(double), typeof(PaginationUserControl));

        private static readonly DependencyProperty SliderHeightProperty =
         DependencyProperty.Register("SliderHeight", typeof(double), typeof(PaginationUserControl), new PropertyMetadata(0.0));

        private static readonly DependencyProperty SliderWidthProperty =
           DependencyProperty.Register("SliderWidth", typeof(double), typeof(PaginationUserControl), new PropertyMetadata(0.0));

        private static readonly DependencyProperty NumberOfElementsProperty =
         DependencyProperty.Register("NumberOfElements", typeof(int), typeof(PaginationUserControl), new PropertyMetadata(NumberOfElementsChangedCallBack));
       
        #endregion

        #region Constructor

        public PaginationUserControl()
        {
            try
            {
                InitializeComponent();

                // Create the Dispatcher Time
                _SliderButtonsDispatcherTimer = new DispatcherTimer();
                _SliderButtonsDispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 200);

                PaginationButtons = new ObservableCollection<PaginationButton>();
                Value = -1;
                TotalPages = 0;

                // Set the data context
                LayoutRoot.DataContext = this;
            }
            catch(Exception ex)
            {
                CoreLog.Instance.Append($"[{ex.Source}] - {ex.Message} - {ex.StackTrace}", CoreLogType.Error);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Register\Unregister the events
        /// </summary>
        protected override void RegisterEvents(bool register)
        {
            if (register)
            {
                _SliderButtonsDispatcherTimer.Tick += OnSliderButtonsDispatcherTimerTick;
            }
            else
            {
                _SliderButtonsDispatcherTimer.Tick -= OnSliderButtonsDispatcherTimerTick;
                DisposePaginationButtons();
            }
        }        

        /// <summary>
        /// Create the pages
        /// </summary>
        private void CreatePages()
        {
            try
            {
                // Delete the previus buttons
                DisposePaginationButtons();
                PaginationButtons.Clear();

                for (var i = 0; i < NumberOfPage; i++)
                {
                    // Create the pagination button
                    var paginationButton = new PaginationButton();
                    paginationButton.Width = PaginationButtonWidth;
                    paginationButton.Height = PaginationButtonHeight;
                    paginationButton.GroupName = GroupName;
                    paginationButton.Content = i;

                    // Slider
                    SliderWidth = (PaginationButtonWidth - 10) > 0 ? PaginationButtonWidth - 10 : 10;

                    if (i < NumberOfPage - 1)
                    {
                        paginationButton.Margin = new Thickness(0, 0, 0, 5);
                    }

                    paginationButton.Click += OnPaginationButtonClick;

                    // Add pagination to collection
                    PaginationButtons.Add(paginationButton);
                }

                // Set the TotalPages
                TotalPages = PaginationButtons.Count();

                //Change The ActivePage
                ChangeActivePage(Value);

                if (PaginationButtons.Count > 0)
                {                                       
                    //Change The ActivePage
                    //ChangeActivePage(Value);

                    // Change the SliderMaxValue
                    Slider.Maximum = PaginationButtons.Count;
                }
                else
                {
                    Slider.Maximum = 0;
                }
            }
            catch(Exception ex)
            {
                CoreLog.Instance.Append($"[{ex.Source}] - {ex.Message} - {ex.StackTrace}", CoreLogType.Error);
            }
        }

        /// <summary>
        /// Calculate the new Dimensions:
        /// 1) Pagination Button Height
        /// 2) Slider Height
        /// </summary>
        private void CalculateNewDimensions()
        {
            try
            {
                if (ActualHeight > 0)
                {
                    if (NumberOfElementInPage > 0)
                    {
                        NumberOfPage = NumberOfElements / NumberOfElementInPage;
                        if (NumberOfElements % NumberOfElementInPage > 0)
                        {
                            NumberOfPage = NumberOfPage + 1;
                        }

                        var realHeight = ActualHeight - (5 * (NumberOfPage - 1));
                        PaginationButtonHeight = realHeight / NumberOfPage;

                        if (PaginationButtonHeight < 70)
                        {
                            PaginationButtonHeight = 70;
                        }

                        // Set the Slider Height
                        SliderHeight = PaginationButtonHeight;

                        // Set the IsSliderVisible
                        var result = (SliderHeight * NumberOfPage) + (5 * (NumberOfPage - 1));
                        IsSliderVisible = (SliderHeight * NumberOfPage) + (5 * (NumberOfPage - 1)) > ActualHeight;
                    }
                }
            }
            catch (Exception ex)
            {
                CoreLog.Instance.Append($"[{ex.Source}] - {ex.Message} - {ex.StackTrace}", CoreLogType.Error);
            }
        }

        /// <summary>
        /// Create and renders the pagination buttons
        /// </summary>
        private void RenderButtons()
        {
            CalculateNewDimensions();
            CreatePages();
        }

        /// <summary>
        /// Unregister the events associates with the Pagination Buttons
        /// </summary>
        public void DisposePaginationButtons()
        {
            foreach (var paginationButton in PaginationButtons)
            {
                paginationButton.Click -= OnPaginationButtonClick;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the Item Source changes
        /// </summary>
        private static void NumberOfElementsChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var c = sender as PaginationUserControl;
            if (c != null)
            {
                c.RenderButtons();
            }
        }        

        /// <summary>
        /// Occurs when the Active page index changes
        /// </summary>
        private static void ValueChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var c = sender as PaginationUserControl;
            if (c != null)
            {
                var value = Convert.ToInt32(e.NewValue);
                c.ChangeActivePage(value);
            }
        }        

        /// <summary>
        /// Occurs when a pagination button is clicked.
        /// Set the property Valueof component with the Content of the clicked Pagionation Button
        /// </summary>
        private void OnPaginationButtonClick(object sender, RoutedEventArgs e)
        {
            var paginationButton = (sender as PaginationButton);
            if (paginationButton != null)
            {
                Value = Convert.ToInt32(paginationButton?.Content);
            }
        }

        /// <summary>
        /// Make checked the PageButton with index equals to CurrentActivePageIndex
        /// </summary>              
        private void ChangeActivePage(int value)
        {
            if (PaginationButtons != null && PaginationButtons.Any() && value>-1 && value < PaginationButtons.Count())
            {
                var paginationButton = PaginationButtons.ElementAt(value);
                if (paginationButton.IsChecked.HasValue && !paginationButton.IsChecked.Value)
                {
                    paginationButton.IsChecked = true;
                }

                // Change the PageNumber
                PageNumber = Value + 1;
            }
        }

        /// <summary>
        /// Occurs when the size of control changes
        /// </summary>
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            CalculateNewDimensions();
            CreatePages();            
        }        

        /// <summary>
        /// Occurs when the Mouse is Up in the Slider
        /// </summary>
        private void OnSliderPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            // The SliderValue is the PageNumber which is equals to Value + 1
            Value = Convert.ToInt32(Slider.Value) - 1;
            ChangeActivePage(Value);
        }

        /// <summary>
        /// Occurs when the Previus button of the Slider is pressed
        /// </summary>
        private void OnPreviusMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (PageNumber - 1 > 0)
            {
                PageNumber = PageNumber - 1;
            }

            _CurrentSliderButton = SliderButton.Previus;
            _SliderButtonsDispatcherTimer.Start();
        }

        /// <summary>
        /// Occurs when the Next button of the Slider is pressed
        /// </summary>
        private void OnNextMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (PageNumber + 1 <= PaginationButtons.Count())
            {
                PageNumber = PageNumber + 1;
            }

            _CurrentSliderButton = SliderButton.Next;
            _SliderButtonsDispatcherTimer.Start();           
        }

        /// <summary>
        /// Occurs when the Dispatcher reaches the interval
        /// </summary>       
        private void OnSliderButtonsDispatcherTimerTick(object sender, EventArgs e)
        {
            switch(_CurrentSliderButton)
            {
                case SliderButton.Previus:
                    if (PageNumber - 1 > 0)
                    {
                        PageNumber = PageNumber - 1;
                    }
                    break;
                case SliderButton.Next:
                    if (PageNumber + 1 <= PaginationButtons.Count())
                    {
                        PageNumber = PageNumber + 1;
                    }
                    break;
            }
        }

        /// <summary>
        /// Occurs when the Mouse is Up in the Slider's button (previus or next)
        /// </summary>
        private void OnSliderButtonMouseUp(object sender, MouseButtonEventArgs e)
        {
            _SliderButtonsDispatcherTimer.Stop();
            _CurrentSliderButton = SliderButton.None;

            // Update the value 
            Value = PageNumber - 1;
            ChangeActivePage(Value);
        }

        #endregion        
    }
}
