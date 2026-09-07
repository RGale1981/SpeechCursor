using System.Windows;
using System.Windows.Controls.Primitives;

namespace SpeechCursor.Presentation.Dialogs.Popups.Contracts;

public interface IPopupManager
{
    void SetHost(Window host);

    void SetHost(Popup host);

    void HidePopup(PopupType type);

    bool IsOpen(PopupType type);

    void ShowPopup(PopupType type, FrameworkElement content);
}
