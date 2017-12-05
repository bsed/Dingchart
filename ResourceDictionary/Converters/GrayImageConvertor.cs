using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace ResourceDictionary.Converters {
public class GrayImageConvertor : IValueConverter {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        try {
            BitmapImage bitmapImage = new BitmapImage(new Uri(value.ToString()));
            //BitmapImage bitmapImage = new BitmapImage(new Uri("D:\\Face.jpg"));

            FormatConvertedBitmap newFormatedBitmapSource = new FormatConvertedBitmap();
            newFormatedBitmapSource.BeginInit();
            newFormatedBitmapSource.Source = bitmapImage;
            newFormatedBitmapSource.DestinationFormat = PixelFormats.Gray8;
            newFormatedBitmapSource.EndInit();

            return newFormatedBitmapSource;
        } catch {
            return value;
        }

    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
        throw new NotImplementedException();
    }
    #endregion

}
}
