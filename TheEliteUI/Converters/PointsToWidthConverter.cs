using System;
using System.Globalization;
using System.Windows.Data;
using TheEliteUI.Model;

namespace TheEliteUI.Converters
{
    public class PointsToWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 150 + ((System.Convert.ToInt32(value) * 300) / (double)Ranking.MaxPoints);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
