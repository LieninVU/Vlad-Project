using System;
using System.Globalization;
using System.Windows.Data;

namespace ForVlad.Converters
{
    public class DaysRemainingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "∞";

            if (value is DateTime dateTime)
                return FormatRemaining(dateTime);

            var nullableDate = value as DateTime?;
            if (nullableDate.HasValue)
                return FormatRemaining(nullableDate.Value);

            return "—";
        }

        private static string FormatRemaining(DateTime endDate)
        {
            var days = (int)Math.Ceiling((endDate.Date - DateTime.Now.Date).TotalDays);
            if (days < 0)
                return "просрочен";
            if (days == 0)
                return "сегодня";
            return $"{days} дн.";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
