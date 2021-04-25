using System;
using System.Globalization;
using System.Windows.Data;

namespace TheEliteUI.Converters
{
    public class StageToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return UiExtensions.CreateImgSource($"Stages/{(int)value}.jpg");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
