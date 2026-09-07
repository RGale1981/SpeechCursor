using System.Windows;
using System.Windows.Controls.Primitives;

namespace SpeechCursor.Presentation.Dialogs.Popups.Contracts;

public interface IPopupDialog
{
    PopupType Type { get; }

    Popup Popup { get; }

    FrameworkElement Content { get; }

    public IPopupHost Host { get; }

    bool IsModal { get; }
}
