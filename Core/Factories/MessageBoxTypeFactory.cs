using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Core
{
    public static class MessageBoxTypeFactory
    {

        public static Style Get(MessageBoxType messageBoxType)
        {
            switch (messageBoxType)
            {
                case MessageBoxType.Confirm:
                    return Application.Current.TryFindResource("ConfirmMessageBoxStyle") as Style;
                case MessageBoxType.Error:
                    return Application.Current.TryFindResource("ErrorMessageBoxStyle") as Style;
                case MessageBoxType.Warning:
                    return Application.Current.TryFindResource("WarningMessageBoxStyle") as Style; 
            }

            return null;
        }
    }
}
