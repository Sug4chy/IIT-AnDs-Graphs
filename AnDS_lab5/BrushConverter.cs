using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace AnDS_lab5;

public class BrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value as SolidColorBrush ?? throw new InvalidCastException();

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value as SolidColorBrush ?? throw new InvalidCastException();
}