using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace ResourceDictionary.Converters {
class CornerRadiusConvertor : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

        if (value.ToString() == "left") {
            return new CornerRadius(0, 6, 6, 6);
        } else {
            return new CornerRadius(6, 0, 6, 6);
        }

    }


    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
        throw new NotImplementedException();
    }
}
}
