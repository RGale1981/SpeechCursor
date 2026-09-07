using System.Windows;

namespace SpeechCursor.Infrastructure.Windowing.Contracts;

public interface IPopupHost
{
    Rect GetBounds();
    Window? AsWindow();
}
