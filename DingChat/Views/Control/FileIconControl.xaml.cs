using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace cn.lds.chatcore.pcw.Views.Control {
/// <summary>
/// FileIconControl.xaml 的交互逻辑
/// </summary>
public partial class FileIconControl : UserControl {
    public FileIconControl() {
        InitializeComponent();

        this.DataContext = this;
    }

    public ImageSource FileIcon {
        set;
        get;
    }
    public string FileName {
        set;
        get;
    }

    public string FileFullName {
        set;
        get;
    }
}
}
