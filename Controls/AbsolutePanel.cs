using System.Windows;
using System.Windows.Controls;

namespace SpeechCursor.Controls;

public class AbsolutePanel : Panel
{
    protected override Size MeasureOverride(Size availableSize)
    {
        double maxRight = 0;
        double maxBottom = 0;

        foreach (UIElement child in InternalChildren)
        {
            child.Measure(availableSize);

            double x = Canvas.GetLeft(child);
            if (double.IsNaN(x)) x = 0;

            double y = Canvas.GetTop(child);
            if (double.IsNaN(y)) y = 0;

            maxRight = Math.Max(maxRight, x + child.DesiredSize.Width);
            maxBottom = Math.Max(maxBottom, y + child.DesiredSize.Height);
        }

        return new Size(maxRight, maxBottom);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        foreach (UIElement child in InternalChildren)
        {
            double x = Canvas.GetLeft(child);
            if (double.IsNaN(x)) x = 0;

            double y = Canvas.GetTop(child);
            if (double.IsNaN(y)) y = 0;

            child.Arrange(new Rect(new Point(x, y), child.DesiredSize));
        }

        return finalSize;
    }
}
