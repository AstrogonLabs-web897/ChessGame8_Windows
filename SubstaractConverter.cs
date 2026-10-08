using System;
using System.Globalization;
using System.Windows.Data;

namespace ChessGame8
{
    [ValueConversion(typeof(double), typeof(double))]
    public class SubstaractConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) 
        {
            if ( value is double width && parameter is double SubstractAmount) { return width - SubstractAmount; }
            return Binding.DoNothing;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) 
        {
            throw new NotImplementedException();
        }
    }
}
