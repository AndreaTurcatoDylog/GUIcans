using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DialogService
{
    /// <summary>
    /// The base class for the Dialog Window
    /// </summary>
    //public abstract class DialogBaseWindow : Window
    //{
    //    #region Members

    //    protected IDialogViewModel _ViewModel;

    //    #endregion

    //    #region Constructor

    //    public DialogBaseWindow(IDialogViewModel windowViewModel)
    //    {
    //        _ViewModel = windowViewModel;
    //        DataContext = _ViewModel;

    //        _ViewModel.CloseAction = new Action(() => Close());
    //    }

    //    #endregion

    //    #region Methods

    //    /// <summary>
    //    /// Show the message box in blocked or no blocked style
    //    /// </summary>
    //    public void ShowMessageBox(bool isBlockedWindow = true)
    //    {
    //        if (isBlockedWindow)
    //        {
    //            var blockedTrasparentWindow = new BlockedTrasparentWindow(this);
    //            blockedTrasparentWindow.ShowDialog();
    //        }
    //        else
    //        {
    //            ShowDialog();
    //        }
    //    }

    //    #endregion
    //}
}
