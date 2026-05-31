using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ForVlad.Converters
{
    public class IntToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                int count = (int)value;
                return count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            if (value is bool)
            {
                bool val = (bool)value;
                return val ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IntToVisibilityInverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                int count = (int)value;
                return count > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            if (value is bool)
            {
                bool val = (bool)value;
                return val ? Visibility.Collapsed : Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}