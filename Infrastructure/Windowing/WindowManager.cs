using SpeechCursor.Infrastructure.Extensions;
using SpeechCursor.Infrastructure.Windowing.Contracts;

using System.Windows;

namespace SpeechCursor.Infrastructure.Windowing;

public class WindowManager(IMainWindowManager mainWindowManager, IPopupManager popupManager, IWindowFactory windowFactory) : IWindowManager, IDisposable
{
    private bool _disposed;

    public void BeginStartupSequence()
    {
        CreateStartupWindow();

        // Subscribe to some event on the Startup VM which signals when MainWindow should be shown...?

        // Show SplashScreen Popup
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        popupManager.TryDispose();
        mainWindowManager.TryDispose();

        _disposed = true;
    }

    private void CreateStartupWindow()
    {
        var startupDialog = windowFactory.CreateWindow(WindowType.StartupWindow);

        popupManager.SetHost(startupDialog);

        Application.Current.MainWindow = startupDialog;
    }
}
