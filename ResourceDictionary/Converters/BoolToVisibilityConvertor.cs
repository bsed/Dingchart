using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace ResourceDictionary.Converters {
public class BoolToVisibilityConvertor : IValueConverter {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

        if (Boolean.Parse (value.ToString())==true) {
            return  System.Windows.Visibility.Visible;
        } else {
            return System.Windows.Visibility.Collapsed;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
        throw new NotImplementedException();
    }
    #endregion

}
}
