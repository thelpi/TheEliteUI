using System;
using System.Globalization;
using System.Windows.Data;

namespace TheEliteUI.Converters
{
    public class TimeToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int time = System.Convert.ToInt32(value);
            var minutes = time / 60;
            var secondes = time % 60;
            return $"{minutes.ToString().PadLeft(2, '0')}:{secondes.ToString().PadLeft(2, '0')}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
