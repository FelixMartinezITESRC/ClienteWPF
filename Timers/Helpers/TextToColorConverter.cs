using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Timers.Helpers
{
    public class TextToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string valor = (string)value;
            if (valor == "Programado")
            {
                return Brushes.Gold;
            }
            if (valor=="En camino")
            {
                return Brushes.Yellow;
            }
            if (valor== "Aterrizo")
            {
                return Brushes.White;
            }

            if (valor== "Abordando")
            {
                return Brushes.Green;
            }

            return Brushes.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
