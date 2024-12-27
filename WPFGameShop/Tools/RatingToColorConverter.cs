using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Media;

using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using System.Windows.Data;

namespace WPFGameShop.Tools
{
    public class RatingToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int rating)
            {
                double normalizedValue = (double)rating / 5;

                byte red = (byte)(255*(1-normalizedValue));
                byte green = (byte)(255*(1 * normalizedValue));
                byte blue = 0;

                return new SolidColorBrush(Color.FromRgb(red,green,blue));
            }
            return new SolidColorBrush(Colors.Black); 
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
