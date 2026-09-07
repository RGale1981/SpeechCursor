using SpeechCursor.Infrastructure.Common;
using SpeechCursor.Infrastructure.Windowing.Contracts;

using System.Windows;

namespace SpeechCursor.Infrastructure.Windowing;

public class WindowHost(Window window) : IPopupHost
{
    public Window? AsWindow() => window;

    public Rect GetBounds() => MonitorWorkAreaHelper.GetWindowBounds(window);
}
