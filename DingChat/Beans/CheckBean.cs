using java.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using cn.lds.chatcore.pcw.Common.Enums;
using GalaSoft.MvvmLight;

namespace cn.lds.chatcore.pcw.Beans {
public class CheckBean : ViewModelBase {


    private string name;
    /// <summary>
    /// 名字
    /// </summary>
    public String Name {
        get {
            return name;
        } set {
            name = value;
            RaisePropertyChanged("Name");
        }
    }



    private String logoPath;
    public String LogoPath {
        get {
            return logoPath;
        } set {
            logoPath = value;
            //logoPath.Freeze();
            RaisePropertyChanged("LogoPath");

        }
    }




    private String checkLogoPath;
    public String CheckLogoPath {
        get {
            return checkLogoPath;
        } set {
            checkLogoPath = value;
            //logoPath.Freeze();
            RaisePropertyChanged("CheckLogoPath");

        }
    }

}
}
