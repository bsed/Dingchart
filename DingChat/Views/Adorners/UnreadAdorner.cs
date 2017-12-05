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
public class UnreadAdorner : Adorner {

    private UnReadControl unReadControl;
    private VisualCollection visualCollection;
    public UnReadControl UnReadControl {
        get {
            return unReadControl;
        }
    }

    public UnreadAdorner(UIElement adornedElement) : base(adornedElement) {
        visualCollection = new VisualCollection(this);
        unReadControl = new UnReadControl();
        visualCollection.Add(unReadControl);
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

        double x = finalSize.Width - unReadControl.Width;
        //double y = finalSize.Height - unReadControl.Height;
        double y = 0;

        unReadControl.Arrange(new Rect(x, y, unReadControl.Width, unReadControl.Height)); // you need to arrange

        // Return the final size.
        return finalSize;
    }
}
}
