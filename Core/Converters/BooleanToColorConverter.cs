using System.Windows.Media;

namespace Core
{
    public sealed class BooleanToColorConverter : BooleanConverter<Brush>
    {
        public BooleanToColorConverter() :
            base(new SolidColorBrush(Colors.Transparent), new SolidColorBrush(Colors.Transparent))
        { }
    }
}
