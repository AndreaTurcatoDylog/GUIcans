using Core.ResourceManager.Cultures;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Core
{
    public static class MessageBoxButtonsFactory
    {
        public static ObservableCollection<Button> Get(MessageBoxButtonsType customMessageBoxButtonsTypes)
        {
            var Buttons = new ObservableCollection<Button>();

            switch (customMessageBoxButtonsTypes)
            {
                case MessageBoxButtonsType.OK_Cancel:
                    Buttons.Add(CreateButton("CustomLittleButtonOKStyle"));
                    Buttons.Add(CreateButton("CustomLittleButtonNoStyle"));
                    break;
                case MessageBoxButtonsType.Yes_No:
                    Buttons.Add(CreateButton("CustomLittleButtonYesStyle"));
                    Buttons.Add(CreateButton("CustomLittleButtonNoStyle"));
                    break;
                case MessageBoxButtonsType.Yes_No_Retry:
                    Buttons.Add(CreateButton("CustomLittleButtonYesStyle"));
                    Buttons.Add(CreateButton("CustomLittleButtonNoStyle"));
                    Buttons.Add(CreateButton("CustomLittleButtonBackStyle"));
                    break;
                case MessageBoxButtonsType.OK:
                    Buttons.Add(CreateButton("CustomLittleButtonOKStyle"));
                    break;
            }

            return Buttons;
        }

        /// <summary>
        /// This will be deleted
        /// </summary>
        /// <param name="content"></param>
        /// <param name="backGroundColor"></param>
        /// <param name="buttonImage"></param>
        /// <param name="buttonContentMargin"></param>
        /// <param name="buttonResult"></param>
        /// <returns></returns>
        private static ImageButton CreateButton(string content, string backGroundColor, string buttonImage, string buttonContentMargin, ButtonResult buttonResult)
        {
            return new ImageButton()
            {
                Height = 70,
                Width = 70,
                FontSize = 11,
                Background = Application.Current.Resources[backGroundColor] as System.Windows.Media.Brush,
                Foreground = System.Windows.Media.Brushes.Black,
                ImageButtonPath = Application.Current.Resources[buttonImage] as Geometry,
                ImageButtonContentMargin = buttonContentMargin,
                Content = content,
                Margin = new Thickness(15, 0, 0, 0),
                ButtonResult = buttonResult
            };
        }

        /// <summary>
        /// Create the button in according to the specificated style
        /// </summary>
        private static ImageButton CreateButton(string style)
        {
            return new ImageButton()
            {
                Style = Application.Current.FindResource(style) as Style,
            };
        }
    }
}
