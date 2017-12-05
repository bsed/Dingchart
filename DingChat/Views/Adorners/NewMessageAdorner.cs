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
public class NewMessageAdorner : Adorner {

    private NewMessageControl newMessageControl;
    private VisualCollection visualCollection;
    public NewMessageControl NewMessageControl {
        get {
            return newMessageControl;
        }
    }

    public NewMessageAdorner(UIElement adornedElement) : base(adornedElement) {
        visualCollection = new VisualCollection(this);
        newMessageControl = new NewMessageControl();
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
        double y = finalSize.Height - newMessageControl.Height;


        newMessageControl.Arrange(new Rect(x, y, newMessageControl.Width, newMessageControl.Height)); // you need to arrange

        // Return the final size.
        return finalSize;
    }
}
}
