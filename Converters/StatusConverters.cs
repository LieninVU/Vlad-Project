using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using ForVlad.Models;
using ForVlad.Services;

namespace ForVlad.Converters
{
    public class EnumToRussianConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return EnumLocalization.ToRussian(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Конвертер типа договора в цвет
    public class ContractTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ContractType)
            {
                var type = (ContractType)value;
                switch (type)
                {
                    case ContractType.Rental:
                        return new SolidColorBrush(Color.FromRgb(52, 152, 219));    // Синий
                    case ContractType.Leasing:
                        return new SolidColorBrush(Color.FromRgb(155, 89, 182));   // Фиолетовый
                    default:
                        return new SolidColorBrush(Color.FromRgb(52, 152, 219));
                }
            }
            return new SolidColorBrush(Color.FromRgb(52, 152, 219));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Конвертер доступности в цвет
    public class BoolToAvailabilityColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                bool isAvailable = (bool)value;
                return isAvailable 
                    ? new SolidColorBrush(Color.FromRgb(46, 204, 113))   // Зеленый
                    : new SolidColorBrush(Color.FromRgb(231, 76, 60));   // Красный
            }
            return new SolidColorBrush(Color.FromRgb(149, 165, 166));    // Серый
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Конвертер bool в текст доступности
    public class BoolToAvailableTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                bool isAvailable = (bool)value;
                return isAvailable ? "Доступно" : "Занято";
            }
            return "Неизвестно";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
