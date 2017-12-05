using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ResourceDictionary.Control {
public class DingScrollview: ScrollViewer {
    public DingScrollview() {


        ControlTemplate style = (ControlTemplate)this.FindResource("MyScrollViewerControlTemplate");

        this.Template = style;


        this.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
        this.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
        this.MouseEnter -= DingScrollview_MouseEnter;
        this.MouseEnter += DingScrollview_MouseEnter;

        this.MouseLeave -= DingScrollview_MouseLeave;
        this.MouseLeave += DingScrollview_MouseLeave;
    }

    private void DingScrollview_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e) {
        this.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
        //this.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
    }

    private void DingScrollview_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e) {
        this.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
        //this.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
    }
}
}
