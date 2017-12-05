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
public class SearchAdorner: Adorner {

    private SearchResultControl searchResultControl;
    private VisualCollection visualCollection;
    public SearchResultControl ResultControl {
        get {
            return searchResultControl;
        }
    }

    public SearchAdorner(UIElement adornedElement) : base(adornedElement) {
        visualCollection = new VisualCollection(this);
        searchResultControl = new SearchResultControl();
        visualCollection.Add(searchResultControl);
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
        // where to position the customControl...this is relative to the element you are adorning
        double x = 0;
        double y = 0;
        searchResultControl.Arrange(new Rect(x, y, finalSize.Width - 1, finalSize.Height)); // you need to arrange

        // Return the final size.
        return finalSize;
    }
}
}
