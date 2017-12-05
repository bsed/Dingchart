using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using cn.lds.chatcore.pcw.Views.Control;

namespace cn.lds.chatcore.pcw.Views.Adorners {
public class WinStateAdorner : Adorner {

    private WinStateControl newMessageControl;
    private VisualCollection visualCollection;
    public WinStateControl NewMessageControl {
        get {
            return newMessageControl;
        }
    }

    public WinStateAdorner(UIElement adornedElement) : base(adornedElement) {
        visualCollection = new VisualCollection(this);
        newMessageControl =  WinStateControl.getInstance();
        visualCollection.Add(newMessageControl);
    }

    protected override int VisualChildrenCount {
        get {
            return visualCollection.Count;
        }
    }

    protected override Visual GetVisualChild(int index) {
        return visualCollection[index];
    }

    protected override void OnRender(DrawingContext drawingContext) {
    }

    protected override Size ArrangeOverride(Size finalSize) {

        double x = finalSize.Width - newMessageControl.Width;
        double y = newMessageControl.Height;


        newMessageControl.Arrange(new Rect(x, 0, newMessageControl.Width, newMessageControl.Height)); // you need to arrange

        // Return the final size.
        return finalSize;
    }
}
}
