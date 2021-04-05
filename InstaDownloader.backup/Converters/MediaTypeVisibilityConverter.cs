using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using InstaDownloader.Utils;

namespace InstaDownloader.Converters
{
    public class MediaTypeVisibilityConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var mediaType = (MediaType) value;
            var mediaTypeParam = (MediaType) parameter;
            return mediaType == mediaTypeParam
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}