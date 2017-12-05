using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;

namespace cn.lds.chatcore.pcw.Models {
public class ButtonImageCollection : ViewModelBase {

    public string Text {
        get;
        set;
    }
    public string Url {
        get;
        set;
    }
    public String ParentName {
        get;
        set;
    }
    public Boolean IsDefualt {
        get;
        set;
    }
    public Boolean ISVisible {
        get;
        set;
    }

    public String Name {
        get;
        set;
    }


    public BitmapImage LogoPath {
        get;
        set;
    }

    public BitmapImage CheckLogoPath {
        get;
        set;
    }
    private bool unRead;
    public bool UnRead {
        get {
            return unRead;
        } set {
            unRead = value;
            RaisePropertyChanged("UnRead");
        }
    }


    public Boolean HasText {
        get {
            if(String.IsNullOrEmpty(Text)) {
                return false;
            } else {
                return true;
            }
        }
    }

}
}
