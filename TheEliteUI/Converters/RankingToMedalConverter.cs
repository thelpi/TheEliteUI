using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TheEliteUI.Converters
{
    public class RankingToMedalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (System.Convert.ToInt32(value))
            {
                case 1:
                    return Colors.Gold;
                case 2:
                    return Colors.Silver;
                case 3:
                    return Colors.Peru;
                default:
                    return Colors.Lavender;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
