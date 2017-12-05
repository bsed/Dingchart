using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace ResourceDictionary.Converters {
class PathToImageConvertor : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        try {
            if (value == null) {
                return new BitmapImage(new Uri(@"pack://application:,,,/ResourceDictionary;Component/images/Default_avatar.jpg", UriKind.RelativeOrAbsolute));
            }

            return new BitmapImage(new Uri(value.ToString(), UriKind.RelativeOrAbsolute)); ;
        } catch (Exception ex) {
            return new BitmapImage(new Uri(@"pack://application:,,,/ResourceDictionary;Component/images/Default_avatar.jpg", UriKind.RelativeOrAbsolute));
            //Log.Error(typeof(PathToImageConvertor), ex);
        }

    }


    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
        throw new NotImplementedException();
    }
}
}
