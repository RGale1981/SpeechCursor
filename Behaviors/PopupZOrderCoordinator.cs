using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;

namespace Windows.Behaviors;

public sealed class PopupZOrderCoordinator : IDisposable
{
    private static readonly IntPtr HwndBottom = new(1);
    private static readonly IntPtr HwndTopMost = new(-1);

    private const uint SwpNoActivate = 0x0010;
    private const uint SwpNoMove = 0x0002;
    private const uint SwpNoSize = 0x0001;
    private const uint SwpNoOwnerZOrder = 0x0200;
    private const uint SwpNoSendChanging = 0x0400;
    private const uint SwpNoZOrder = 0x0004;

    private readonly Window _mainWindow;
    private readonly Popup _popup;
    private readonly UIElement? _popupChild;
    private bool _hasMainWindowEverBeenShown;
    private bool _isDisposed;

    public PopupZOrderCoordinator(Window mainWindow, Popup popup)
    {
        _mainWindow = mainWindow;
        _popup = popup;
        _popupChild = popup.Child as UIElement;

        _mainWindow.Activated += OnMainWindowActivated;
        _mainWindow.Deactivated += OnMainWindowDeactivated;
        _mainWindow.StateChanged += OnMainWindowStateChanged;
        _mainWindow.IsVisibleChanged += OnMainWindowIsVisibleChanged;
        _popup.Opened += OnPopupOpened;
        _popup.Closed += OnPopupClosed;

        if (_popupChild is not null)
            _popupChild.PreviewMouseLeftButtonDown += OnPopupMouseLeftButtonDown;

        SyncPopupZOrder();
    }

    public void Dispose()
    {
        if (_isDisposed)
            return;

        _mainWindow.Activated -= OnMainWindowActivated;
        _mainWindow.Deactivated -= OnMainWindowDeactivated;
        _mainWindow.StateChanged -= OnMainWindowStateChanged;
        _mainWindow.IsVisibleChanged -= OnMainWindowIsVisibleChanged;
        _popup.Opened -= OnPopupOpened;
        _popup.Closed -= OnPopupClosed;

        if (_popupChild is not null)
            _popupChild.PreviewMouseLeftButtonDown -= OnPopupMouseLeftButtonDown;

        _isDisposed = true;
    }

    private void OnMainWindowActivated(object? sender, EventArgs e) => BringPopupAboveMainWindow();

    private void OnMainWindowDeactivated(object? sender, EventArgs e)
        => _mainWindow.Dispatcher.BeginInvoke(SyncPopupZOrder, DispatcherPriority.ApplicationIdle);

    private void OnMainWindowStateChanged(object? sender, EventArgs e) => SyncPopupZOrder();

    private void OnMainWindowIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) => SyncPopupZOrder();

    private void OnPopupOpened(object? sender, EventArgs e) => SyncPopupZOrder();

    private void OnPopupClosed(object? sender, EventArgs e)
    {
    }

    private void OnPopupMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (!_hasMainWindowEverBeenShown && !_mainWindow.IsVisible)
        {
            BringPopupTopMost();
            return;
        }

        BringPopupAboveMainWindow();
    }

    private void SyncPopupZOrder()
    {
        if (!_popup.IsOpen)
            return;

        if (!_hasMainWindowEverBeenShown && !_mainWindow.IsVisible)
        {
            BringPopupTopMost();
            return;
        }

        if (_mainWindow.IsVisible)
            _hasMainWindowEverBeenShown = true;

        var foregroundWindow = GetForegroundWindow();
        if (foregroundWindow == IntPtr.Zero)
        {
            BringPopupAboveMainWindow();
            return;
        }

        if (IsOwnedByCurrentProcess(foregroundWindow) || IsPopupWindow(foregroundWindow))
        {
            BringPopupAboveMainWindow();
            return;
        }

        SendPopupBehindOtherApps();
    }

    private void BringPopupAboveMainWindow()
    {
        var popupHandle = GetPopupHandle();
        if (popupHandle == IntPtr.Zero)
            return;

        SetWindowPos(
            popupHandle,
            HwndTopMost,
            0,
            0,
            0,
            0,
            SwpNoActivate | SwpNoMove | SwpNoSize | SwpNoOwnerZOrder | SwpNoSendChanging);
    }

    private void BringPopupTopMost()
    {
        var popupHandle = GetPopupHandle();
        if (popupHandle == IntPtr.Zero)
            return;

        SetWindowPos(
            popupHandle,
            HwndTopMost,
            0,
            0,
            0,
            0,
            SwpNoActivate | SwpNoMove | SwpNoSize | SwpNoOwnerZOrder | SwpNoSendChanging);
    }

    private void SendPopupBehindOtherApps()
    {
        var popupHandle = GetPopupHandle();
        if (popupHandle == IntPtr.Zero)
            return;

        SetWindowPos(
            popupHandle,
            HwndBottom,
            0,
            0,
            0,
            0,
            SwpNoActivate | SwpNoMove | SwpNoSize | SwpNoOwnerZOrder | SwpNoSendChanging);
    }

    private IntPtr GetPopupHandle() => (PresentationSource.FromVisual(_popup.Child) as HwndSource)?.Handle ?? IntPtr.Zero;

    private static bool IsOwnedByCurrentProcess(IntPtr hwnd)
    {
        _ = GetWindowThreadProcessId(hwnd, out var processId);
        return processId == Environment.ProcessId;
    }

    private bool IsPopupWindow(IntPtr hwnd)
    {
        var popupHandle = GetPopupHandle();
        return popupHandle != IntPtr.Zero && (hwnd == popupHandle || IsChildWindow(popupHandle, hwnd));
    }

    private static bool IsChildWindow(IntPtr parent, IntPtr candidate)
    {
        var current = candidate;

        while (current != IntPtr.Zero)
        {
            if (current == parent)
                return true;

            current = GetParent(current);
        }

        return false;
    }

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern IntPtr GetParent(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetWindowPos(
        IntPtr hWnd,
        IntPtr hWndInsertAfter,
        int x,
        int y,
        int cx,
        int cy,
        uint uFlags);

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);
}
