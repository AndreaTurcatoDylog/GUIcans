using Business;
using Core;
using Models;
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

namespace SpeedDetailsImageVisualizer
{
    /// <summary>
    /// Interaction logic for RemoteReportFiltersUserControl.xaml
    /// </summary>
    public partial class RemoteReportFiltersUserControl : FiltersBaseUserControl
    {       
        #region Properties        

        /// <summary>
        /// Get\Set the Filters
        /// </summary>
        public Filters Filters
        {
            get { return (Filters)GetValue(FiltersProperty); }
            set { SetValue(FiltersProperty, value); }
        }        

        #endregion

        #region Commands        

        /// <summary>
        /// Get\Set the OpenFiltersCommand
        /// </summary>
        public ICommand OpenFiltersCommand { get; private set; }       

        /// <summary>
        /// GetSet the ClearFiltersCommand
        /// </summary>
        public ICommand ClearFiltersCommand
        {
            get { return (ICommand)GetValue(ClearFiltersProperty); }
            set { SetValue(ClearFiltersProperty, value); }
        }

        #endregion        

        #region Dependency Properties

        private static readonly DependencyProperty FiltersProperty =
            DependencyProperty.Register("Filters", typeof(Filters), typeof(RemoteReportFiltersUserControl), new PropertyMetadata(null));

       
        
        private static readonly DependencyProperty ClearFiltersProperty =
            DependencyProperty.Register("ClearFiltersCommand", typeof(ICommand), typeof(RemoteReportFiltersUserControl), new PropertyMetadata(null));

        #endregion       

        #region Constructor

        public RemoteReportFiltersUserControl()
        {
            InitializeComponent();

            // Set the Datacontext
            LayoutRoot.DataContext = this;

            // Create the Commands
            OpenFiltersCommand = new RelayCommand(OpenFiltersExecute);
        }

        #endregion

        #region Methods

        ///// <summary>
        ///// Register\Unregister the Events
        ///// </summary>
        //protected override void RegisterEvents(bool register)
        //{
        //    if (register)
        //    {
        //        ChangePageLostFocus += OnChangePageLostFocus;
        //    }
        //    else
        //    {
        //        ChangePageLostFocus -= OnChangePageLostFocus;
        //    }
        //}

        #endregion

        #region Events        

        

        /// <summary>
        /// Occurs when the Filters must be opened\closed
        /// </summary>
        private void OpenFiltersExecute(object param)
        {
            if (Filters != null)
            {
                Filters.AreFiltersOpened = !Filters.AreFiltersOpened;
            }
        }

        #endregion      
    }
}
