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
public class WaitAdorner : Adorner {

    private LoadingWait waitControl;
    private VisualCollection visualCollection;
    public LoadingWait WaitControl {
        get {
            return waitControl;
        }
    }

    public WaitAdorner(UIElement adornedElement) : base(adornedElement) {
        visualCollection = new VisualCollection(this);
        waitControl = new LoadingWait();
        waitControl.Margin = new Thickness(0);
        visualCollection.Add(waitControl);
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

        //Window window = Window.GetWindow(PcStart.getInstance().TitleBar);
        //Point point = PcStart.getInstance().TitleBar.TransformToAncestor(window).Transform(new Point(0, 0));

        waitControl.Arrange(new Rect(0, 30, PcStart.getInstance().TitleBar.ActualWidth, PcStart.getInstance().TitleBar.ActualHeight)); // you need to arrange

        // Return the final size.
        return finalSize;
    }
}
}
