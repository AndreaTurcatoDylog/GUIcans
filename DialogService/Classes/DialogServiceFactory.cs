using Business;
using Common;
using Core;
using Models;
using System;
using System.Windows;

namespace DialogService
{           
    /// <summary>
    /// This factory creates the specificated Window
    /// </summary>
    public class DialogServiceFactory : DialogServiceBase
    {
        #region Methods

        /// <summary>
        /// Create a new Window by the specificated Window
        /// </summary>
        public override Window ChooseDialog(IDialogViewModel dialogViewModel)
        {            
            if (dialogViewModel != null)
            {
                switch ((DialogServiceKey)dialogViewModel.DialogServiceKey)
                {
                    case DialogServiceKey.CreateReport:
                        return  new CreateReportWindow(dialogViewModel);
                    case DialogServiceKey.Options:
                        return new OptionsWindow(dialogViewModel);
                }
            }

            return null;
        }

        #endregion
    }
}
