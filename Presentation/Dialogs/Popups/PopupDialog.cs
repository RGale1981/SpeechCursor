using SpeechCursor.Presentation.Dialogs.Popups.Contracts;

using System.Windows;
using System.Windows.Controls.Primitives;

namespace SpeechCursor.Presentation.Dialogs.Popups;

public sealed class PopupDialog(PopupType type, Popup popup, FrameworkElement content, IPopupHost host, bool isModal) : IPopupDialog, IDisposable
{
    private bool _disposed;

    public PopupType Type => type;

    public Popup Popup => popup;

    public FrameworkElement Content => content;

    public IPopupHost Host => host;

    public bool IsModal => isModal;

    public void Dispose()
    {
        if (_disposed)
            return;

        Popup.IsOpen = false;
        Popup.Child = null;

        _disposed = true;
    }
}
