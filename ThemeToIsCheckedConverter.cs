using System;
using System.Globalization;
using System.Windows.Data;
namespace ChessGame8
{
    public class ThemeToIsCheckedConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) 
        {
            // Проверяем, что value и parameter не null
            if (value == null || parameter == null)
            {
                throw new ArgumentNullException("Параметры value и parameter не могут быть null");

            }
                
            else
            {
                // Значение темы (string)
                //string theme = value as string;
                // Параметр (ожидаемая тема)
                //string expectedTheme = parameter as string;
                // Дополнительная проверка на null
                if (value is not string theme || parameter is not string expectedTheme)
                    throw new ArgumentNullException("Значения theme и expectedTheme не могут быть null");
                else
                {
                    // Если тема совпадает с ожидаемой, возвращаем true
                    return theme == expectedTheme;

                }
            }
        }
        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Проверяем, что value и parameter не null
            if (value == null || parameter == null)
                throw new ArgumentNullException("Параметры value и parameter не могут быть null");
            else
            {  // Если IsChecked истинно, возвращаем ожидаемую тему
            bool isChecked = (bool)value;
               // Дополнительная проверка на null
                if (parameter is not string expectedTheme)
                    throw new ArgumentNullException("Значение expectedTheme не может быть null");
                return isChecked ? expectedTheme : null;

            }
          
        }

    }
}
