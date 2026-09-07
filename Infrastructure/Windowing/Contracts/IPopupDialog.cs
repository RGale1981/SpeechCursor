using System.Windows;
using System.Windows.Controls.Primitives;

namespace SpeechCursor.Infrastructure.Windowing.Contracts;

public interface IPopupDialog
{
    PopupType Type { get; }

    Popup Popup { get; }

    FrameworkElement Content { get; }

    public IPopupHost Host { get; }

    bool IsModal { get; }
}
