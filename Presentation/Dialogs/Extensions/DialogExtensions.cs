using System.Windows;

namespace SpeechCursor.Presentation.Dialogs.Extensions;

public static class DialogExtensions
{
    public static void SetVisibilityProperties(this Window window, bool isVisible)
    {
        window.ShowActivated = isVisible;
        window.ShowInTaskbar = isVisible;
        window.Visibility = isVisible ? Visibility.Visible : Visibility.Hidden;
    }
}
