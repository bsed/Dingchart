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
public class AtMeAdorner : Adorner {

    private AtMeControl atMeControl;
    private VisualCollection visualCollection;
    public AtMeControl AtMeControl {
        get {
            return atMeControl;
        }
    }

    public AtMeAdorner(UIElement adornedElement) : base(adornedElement) {
        visualCollection = new VisualCollection(this);
        atMeControl = new AtMeControl();
        visualCollection.Add(atMeControl);
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

        double x = finalSize.Width - atMeControl.Width;
        double y = finalSize.Height - atMeControl.Height;


        atMeControl.Arrange(new Rect(x, y, atMeControl.Width, atMeControl.Height)); // you need to arrange

        // Return the final size.
        return finalSize;
    }
}
}
