using System.Windows;

namespace SpeechCursor.Infrastructure.Windowing.Extensions;

public static class WindowExtensions
{
    public static void SetVisibilityProperties(this Window window, bool isVisible)
    {
        window.ShowActivated = isVisible;
        window.ShowInTaskbar = isVisible;
        window.Visibility = isVisible ? Visibility.Visible : Visibility.Hidden;
    }
}
