using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Core
{
    public interface IDialogService
    {
        void CreateDialog(IDialogViewModel windowViewModel, bool blocked = true);
    }

    /// <summary>
    /// The base class for the dialog service
    /// </summary>
    public abstract class DialogServiceBase : IDialogService
    {
        #region Methods

        /// <summary>
        /// Create a new Window by the specificated Window
        /// </summary>
        public void CreateDialog(IDialogViewModel dialogViewModel, bool blocked = true)
        {
            Window window = null;

            if (dialogViewModel != null)
            {
                window = ChooseDialog(dialogViewModel);
            }

            if (window != null)
            {
                if (blocked)
                {
                    var blockedTrasparentWindow = new BlockedTrasparentWindow(window);
                    blockedTrasparentWindow.ShowDialog();
                }
                else
                {
                    window.ShowDialog();
                }
            }
        }

        /// <summary>
        /// Create the Window from specificated ViewModel
        /// </summary>
        public abstract Window ChooseDialog(IDialogViewModel dialogViewModel);

        #endregion
    }
}
