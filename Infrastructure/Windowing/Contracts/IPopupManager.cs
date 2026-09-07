using System.Windows;
using System.Windows.Controls.Primitives;

namespace SpeechCursor.Infrastructure.Windowing.Contracts;

public interface IPopupManager
{
    void SetHost(Window windowHost);

    void SetHost(Popup popupHost);

    void HidePopup(PopupType type);

    bool IsOpen(PopupType type);

    void ShowPopup(PopupType type, FrameworkElement content);
}
