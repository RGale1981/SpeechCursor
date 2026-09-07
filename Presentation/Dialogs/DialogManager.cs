using SpeechCursor.Dictation.Contracts;
using SpeechCursor.Infrastructure.Extensions;
using SpeechCursor.Presentation.Dialogs.Contracts;
using SpeechCursor.Presentation.Dialogs.Popups.Contracts;

using System.Windows;

namespace SpeechCursor.Presentation.Dialogs;

public class DialogManager(IDictationViewManager mainWindowManager, IPopupManager popupManager, IDialogFactory windowFactory) : IDalogManager, IDisposable
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
        var startupDialog = windowFactory.CreateWindow(DialogType.StartupWindow);

        popupManager.SetHost(startupDialog);

        Application.Current.MainWindow = startupDialog;
    }
}
