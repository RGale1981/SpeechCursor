using System.Windows;

namespace SpeechCursor.Presentation.Dialogs.Popups.Contracts;

public interface IPopupHost
{
    Rect GetBounds();
    Window? AsWindow();
}
