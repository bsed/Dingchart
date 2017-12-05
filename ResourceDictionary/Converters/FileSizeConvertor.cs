using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ResourceDictionary.Converters {
class FileSizeConvertor : IValueConverter {

    public static string CountSize(long Size) {
        string m_strSize = "";
        long FactSize = 0;
        FactSize = Size;
        if (FactSize < 1024.00)
            m_strSize = FactSize.ToString("F2") + " B";
        else if (FactSize >= 1024.00 && FactSize < 1048576)
            m_strSize = (FactSize / 1024.00).ToString("F2") + " K";
        else if (FactSize >= 1048576 && FactSize < 1073741824)
            m_strSize = (FactSize / 1024.00 / 1024.00).ToString("F2") + " M";
        else if (FactSize >= 1073741824)
            m_strSize = (FactSize / 1024.00 / 1024.00 / 1024.00).ToString("F2") + " G";
        return m_strSize;
    }
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        long size = 0;
        try {
            size= long.Parse(value.ToString());

            return CountSize(size);
        } catch (Exception ex) {
            return size ;
        }


    }


    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
        throw new NotImplementedException();
    }
}
}
