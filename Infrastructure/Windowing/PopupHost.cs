using SpeechCursor.Infrastructure.Windowing.Contracts;

using System.Windows;
using System.Windows.Controls.Primitives;

namespace SpeechCursor.Infrastructure.Windowing;

public class PopupHost(Popup popup) : IPopupHost
{
    public Window? AsWindow() => null;

    public Rect GetBounds()
    {
        var left = popup.HorizontalOffset;
        var top = popup.VerticalOffset;
        var width = popup.Child.RenderSize.Width;
        var height = popup.Child.RenderSize.Height;

        return new Rect(left, top, width, height);
    }
}
