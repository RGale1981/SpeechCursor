using SpeechCursor.Dictation;
using SpeechCursor.Infrastructure.Windowing.Contracts;
using SpeechCursor.Infrastructure.Windowing.Extensions;
using SpeechCursor.Startup;

using System.Windows;

namespace SpeechCursor.Infrastructure.Windowing;

internal class WindowFactory : IWindowFactory
{
    public Window CreateWindow(WindowType type, bool isVisible = false, WindowStartupLocation startupLocation = WindowStartupLocation.CenterScreen)
    {
        var window = type switch
        {
            WindowType.MainWindow => CreateMainWindow(),
            WindowType.StartupWindow => CreateStartupWindow(),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        window.WindowStartupLocation = startupLocation;
        window.SetVisibilityProperties(isVisible);

        return window;
    }

    private Window CreateMainWindow()
        => new DictationWindow() { DataContext = new DictationViewModel() };

    private Window CreateStartupWindow()
        => new StartupWindow() { DataContext = new StartupViewModel() };
}
