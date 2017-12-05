using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ResourceDictionary.Converters {
public class OrgBarConvertor : IMultiValueConverter {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
        try {
            if (values[0] != null && values[0].ToString().Length > 5) {
                values[0] = values[0].ToString().Substring(0, 5) + "...";
            }
            if(values[1] == null) {
                return values[0];
            }
            if(values[1].ToString()=="-1"|| values[1].ToString() == "0" ) {
                return values[0];
            } else {
                return ">" + values[0];
            }
        } catch (Exception ex) {
            return values[0];
        }

    }


    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}
}
