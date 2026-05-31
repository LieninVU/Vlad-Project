using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using ForVlad.Models;

namespace ForVlad.Converters
{
    // Конвертер статуса договора в цвет
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ContractStatus)
            {
                var status = (ContractStatus)value;
                switch (status)
                {
                    case ContractStatus.Draft:
                        return new SolidColorBrush(Color.FromRgb(149, 165, 166)); // Серый
                    case ContractStatus.Signed:
                        return new SolidColorBrush(Color.FromRgb(41, 128, 185));   // Синий
                    case ContractStatus.Active:
                        return new SolidColorBrush(Color.FromRgb(46, 204, 113));   // Зеленый
                    case ContractStatus.Suspended:
                        return new SolidColorBrush(Color.FromRgb(243, 156, 18)); // Оранжевый
                    case ContractStatus.Completed:
                        return new SolidColorBrush(Color.FromRgb(52, 152, 219)); // Синий
                    case ContractStatus.Terminated:
                        return new SolidColorBrush(Color.FromRgb(231, 76, 60)); // Красный
                    default:
                        return new SolidColorBrush(Color.FromRgb(149, 165, 166));
                }
            }
            return new SolidColorBrush(Color.FromRgb(149, 165, 166));
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
