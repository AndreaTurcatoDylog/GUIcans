using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// The DialogViewmodel is an external dialog with its own life circle.
    /// To use a DialogViewmodel:
    /// 1) the viewmodel must implements the IDialogViewModel Interface
    /// 2) create an int DialogServiceCode, distinct one for each dialog
    /// 3) create a new XAML Window (XAMLWindow) and derive it from DialogBaseWindow    
    /// 4) Create a DialogServiceFactory and derive it from DialogServiceBase.
    ///    Implements the abstract method ChooseDialog like example:
    ///     ex:
    ///     public void CreateDialog(IDialogViewModel dialogViewModel, bool blocked = true)
    ///     {
    ///        if (dialogViewModel != null)
    ///        {
    ///             switch (dialogViewModel.DialogServiceCode) // see point 2)
    ///             {
    ///                case 0:
    ///                    return  new XAMLWindow(dialogViewModel); // see point 3)
    ///             }
    ///        }
    ///     }   
    /// </summary>
    public interface IDialogViewModel
    {
        int DialogServiceKey { get; }

        Action CloseAction { get; set; }
    }
}