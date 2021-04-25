using System;
using System.Windows.Media.Imaging;

namespace TheEliteUI
{
    internal static class UiExtensions
    {
        internal static BitmapFrame CreateImgSource(string resourceName)
        {
            var imgUri = new Uri(
                $"pack://application:,,,/TheEliteUI;component/Resources/{resourceName}",
                UriKind.RelativeOrAbsolute);

            return BitmapFrame.Create(imgUri);
        }
    }
}
