using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace InstaDownloader.Converters
{
    public class BoolToVisibilityConverter: IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isVisible;
            if (bool.TryParse(value.ToString(), out isVisible) && isVisible)
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}