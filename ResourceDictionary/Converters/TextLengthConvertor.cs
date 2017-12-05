using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace ResourceDictionary.Converters {
public class TextLengthConvertor : IValueConverter {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (parameter == null) return value;

        int par = int.Parse(parameter.ToString());


        string txt = value.ToString();

        if (txt.Length > par) {
            return txt.Substring(0,par)+"...";
        }
        return value.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
        throw new NotImplementedException();
    }
    #endregion

}
}
