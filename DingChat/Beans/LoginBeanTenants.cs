using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;

namespace cn.lds.chatcore.pcw.Beans {
public class LoginBeanTenants : ViewModelBase {

    private string _tenant;
    public String tenant {
        get {
            return _tenant;
        } set {
            _tenant = value;
            RaisePropertyChanged("tenant");
        }
    }

    private string _logoID;
    public String logoID {
        get {
            return _logoID;
        } set {
            _logoID = value;
            RaisePropertyChanged("logoID");
        }
    }

    private BitmapImage _logoPath;
    public BitmapImage logoPath {
        get {
            return _logoPath;
        } set {
            _logoPath = value;
            RaisePropertyChanged("logoPath");
        }
    }

    private string _name;
    public String name {
        get {
            return _name;
        } set {
            _name = value;
            RaisePropertyChanged("name");
        }
    }
    private string _shortName;
    public String shortName {
        get {
            return _shortName;
        } set {
            _shortName = value;
            RaisePropertyChanged("shortName");
        }
    }

    private string _sortNum;
    public String sortNum {
        get {
            return _sortNum;
        } set {
            _sortNum = value;
            RaisePropertyChanged("sortNum");
        }
    }


}
}
