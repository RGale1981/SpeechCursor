using SpeechCursor.Infrastructure.Common;
using SpeechCursor.Presentation.Dialogs.Popups.Contracts;

using System.Windows;

namespace SpeechCursor.Presentation.Dialogs;

public class DialogHost(Window window) : IPopupHost
{
    public Window? AsWindow() => window;

    public Rect GetBounds() => MonitorWorkAreaHelper.GetWindowBounds(window);
}
