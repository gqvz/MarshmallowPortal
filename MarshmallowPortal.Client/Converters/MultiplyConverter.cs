using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace MarshmallowPortal.Client.Converters;

public class MultiplyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (int)System.Convert.ToSingle(value) * System.Convert.ToSingle(parameter);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}