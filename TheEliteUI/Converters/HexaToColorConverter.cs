using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TheEliteUI.Converters
{
    public class HexaToColorConverter : IValueConverter
    {
        private static BrushConverter _brushConverter = new BrushConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return _brushConverter.ConvertFrom($"#{value}");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
