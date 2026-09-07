using SpeechCursor.Infrastructure.Extensions;
using SpeechCursor.Presentation.Dialogs.Popups.Contracts;

using System.Windows;
using System.Windows.Controls.Primitives;

namespace SpeechCursor.Presentation.Dialogs.Popups;

public sealed class PopupManager(IPopupHost popupHost) : IPopupManager, IDisposable
{
    private readonly Dictionary<PopupType, IPopupDialog> _popups = [];
    private bool _disposed;

    public void SetHost(Window host)
    {
        popupHost = new DialogHost(host);

        RepositionPopupsForNewHost();
    }

    public void SetHost(Popup host)
    {
        popupHost = new PopupHost(host);

        RepositionPopupsForNewHost();
    }

    public bool IsOpen(PopupType type) => _popups.ContainsKey(type);

    public void ShowPopup(PopupType type, FrameworkElement content)
    {
        if (_popups.ContainsKey(type))
            return;

        var isModal = PopupMetadata.IsModal(type);

        var popup = new Popup
        {
            Child = content,
            AllowsTransparency = true,
            StaysOpen = true,
            Placement = PlacementMode.AbsolutePoint
        };

        var host = ResolveHostForPopup(isModal);
        var dialog = new PopupDialog(type, popup, content, host, isModal);

        popup.Closed += OnPopupClosed;

        _popups[type] = dialog;

        popup.IsOpen = true;

        PositionPopup(dialog);
    }

    public void HidePopup(PopupType type)
    {
        if (!_popups.TryGetValue(type, out var dialog))
            return;

        dialog.Popup.Closed -= OnPopupClosed;
        dialog.Popup.IsOpen = false;

        dialog.TryDispose();
        _popups.Remove(type);
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        foreach (var dialog in _popups.Values)
        {
            dialog.Popup.IsOpen = false;
            dialog.Popup.Closed -= OnPopupClosed;
            dialog.TryDispose();
        }

        _popups.Clear();
        _disposed = true;
    }

    private void RepositionPopupsForNewHost()
    {
        foreach (var dialog in _popups.Values)
        {
            if (!dialog.IsModal)
                PositionPopup(dialog);
        }
    }

    private IPopupHost ResolveHostForPopup(bool isModal)
    {
        var activePopup = _popups.Values.LastOrDefault(p => p.IsModal);

        if (isModal && activePopup != null)
            return new PopupHost(activePopup.Popup);

        return popupHost;
    }

    private void PositionPopup(IPopupDialog dialog)
    {
        var popup = dialog.Popup;
        var content = dialog.Content;

        var bounds = dialog.Host.GetBounds();

        var width = GetWidth(content);
        var height = GetHeight(content);

        popup.HorizontalOffset = bounds.Left + ((bounds.Width - width) / 2);
        popup.VerticalOffset = bounds.Top + ((bounds.Height - height) / 2);
    }


    private static double GetWidth(FrameworkElement element)
    {
        if (element.ActualWidth > 0)
            return element.ActualWidth;

        element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        return element.DesiredSize.Width;
    }

    private static double GetHeight(FrameworkElement element)
    {
        if (element.ActualHeight > 0)
            return element.ActualHeight;

        element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        return element.DesiredSize.Height;
    }

    private void OnPopupClosed(object? sender, EventArgs args)
    {
        foreach (var kvp in _popups)
        {
            if (kvp.Value.Popup == sender)
            {
                HidePopup(kvp.Key);
                break;
            }
        }
    }
}
