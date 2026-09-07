using SpeechCursor.Dictation;
using SpeechCursor.Presentation.Dialogs.Contracts;
using SpeechCursor.Presentation.Dialogs.Extensions;
using SpeechCursor.Startup;

using System.Windows;

namespace SpeechCursor.Presentation.Dialogs;

internal class DialogFactory : IDialogFactory
{
    public Window CreateWindow(DialogType type, bool isVisible = false, WindowStartupLocation startupLocation = WindowStartupLocation.CenterScreen)
    {
        var window = type switch
        {
            DialogType.MainWindow => CreateMainWindow(),
            DialogType.StartupWindow => CreateStartupWindow(),
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
