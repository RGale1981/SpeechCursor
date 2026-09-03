using SpeechCursor.Behaviors;
using SpeechCursor.Common;
using SpeechCursor.Record;

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;

using SpeechCursor.Record;

namespace SpeechCursor;

public partial class App : Application
{
    private const double PopupGap = 0;

    private MainWindow? _mainWindow;
    private MainWindowViewModel? _mainWindowViewModel;
    private Popup? _recordPopup;
    private RecordControls? _recordControls;
    private PopupZOrderCoordinator? _popupZOrderCoordinator;
    private bool _isSynchronizingPositions;
    private PopupAnchor _popupAnchor = PopupAnchor.LeftOutsideTopAligned;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _mainWindowViewModel = new MainWindowViewModel();

        _mainWindow = new MainWindow
        {
            DataContext = _mainWindowViewModel,
            ShowActivated = false,
            ShowInTaskbar = false,
            Visibility = Visibility.Hidden,
            WindowStartupLocation = WindowStartupLocation.Manual
        };

        MainWindow = _mainWindow;
        _mainWindow.Closed += OnMainWindowClosed;
        _mainWindow.LocationChanged += OnMainWindowBoundsChanged;
        _mainWindow.SizeChanged += OnMainWindowBoundsChanged;
        _mainWindow.StateChanged += OnMainWindowBoundsChanged;

        _ = new WindowInteropHelper(_mainWindow).EnsureHandle();

        CreateRecordPopup(_mainWindowViewModel.RecordViewModel);
    }

    private void CreateRecordPopup(RecordViewModel recordViewModel)
    {
        if (_recordPopup is not null || _mainWindow is null)
            return;

        var recordControls = new RecordControls
        {
            DataContext = recordViewModel
        };

        _recordControls = recordControls;
        recordControls.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

        var popupWidth = recordControls.DesiredSize.Width;
        var popupHeight = recordControls.DesiredSize.Height;
        var workArea = MonitorWorkAreaHelper.GetWorkAreaFromPoint(MonitorWorkAreaHelper.GetCursorPosition(), _mainWindow);
        var popupLeft = workArea.Left + ((workArea.Width - popupWidth) / 2);
        var popupTop = workArea.Top + ((workArea.Height - popupHeight) / 2);

        _recordPopup = new Popup
        {
            AllowsTransparency = true,
            Child = recordControls,
            HorizontalOffset = popupLeft,
            IsOpen = true,
            Placement = PlacementMode.AbsolutePoint,
            StaysOpen = true,
            VerticalOffset = popupTop
        };

        _recordPopup.Closed += OnRecordPopupClosed;

        PopupDragBehavior.SetHostPopup(recordControls, _recordPopup);
        _popupZOrderCoordinator = new PopupZOrderCoordinator(_mainWindow, _recordPopup);
        UpdatePopupDragAvailability();
        PositionMainWindowFromPopup();
        HookPopupPositionTracking(_recordPopup);
        recordViewModel.ToggleStateChanged += OnRecordToggleStateChanged;
    }

    private void OnRecordToggleStateChanged(bool isToggled)
    {
        if (_mainWindow is null)
            return;

        if (isToggled)
        {
            PositionMainWindowFromPopup();
            _mainWindow.ShowInTaskbar = true;

            if (!_mainWindow.IsVisible)
                _mainWindow.Show();

            if (_mainWindow.WindowState == WindowState.Minimized)
                _mainWindow.WindowState = WindowState.Normal;

            _mainWindow.Activate();
            return;
        }

        _mainWindow.Hide();
        _mainWindow.ShowInTaskbar = false;
    }

    private void OnMainWindowClosed(object? sender, EventArgs e)
    {
        if (_mainWindow is not null)
        {
            _mainWindow.LocationChanged -= OnMainWindowBoundsChanged;
            _mainWindow.SizeChanged -= OnMainWindowBoundsChanged;
            _mainWindow.StateChanged -= OnMainWindowBoundsChanged;
        }

        _popupZOrderCoordinator?.Dispose();
        _popupZOrderCoordinator = null;

        if (_recordPopup is not null)
        {
            _recordPopup.Closed -= OnRecordPopupClosed;
            UnhookPopupPositionTracking(_recordPopup);

            _recordPopup.IsOpen = false;
            _recordPopup = null;
            _recordControls = null;
        }

        if (_mainWindowViewModel is not null)
        {
            _mainWindowViewModel.RecordViewModel.ToggleStateChanged -= OnRecordToggleStateChanged;
            _mainWindowViewModel = null;
        }
    }

    private void OnRecordPopupClosed(object? sender, EventArgs e)
    {
        Shutdown();
    }

    private void OnMainWindowBoundsChanged(object? sender, EventArgs e)
    {
        if (_isSynchronizingPositions || _mainWindow is null || _recordPopup is null)
            return;

        try
        {
            _isSynchronizingPositions = true;
            UpdatePopupDragAvailability();
            PositionPopupFromMainWindow();
        }
        finally
        {
            _isSynchronizingPositions = false;
        }
    }

    private void OnPopupPositionChanged(object? sender, EventArgs e)
    {
        if (_isSynchronizingPositions || _mainWindow is null || _recordPopup is null)
            return;

        try
        {
            _isSynchronizingPositions = true;
            PositionMainWindowFromPopup();
        }
        finally
        {
            _isSynchronizingPositions = false;
        }
    }

    private void PositionPopupFromMainWindow()
    {
        if (_mainWindow is null || _recordPopup is null)
            return;

        _popupAnchor = GetPopupAnchorForMainWindow();

        var popupWidth = GetPopupWidth();
        var popupHeight = GetPopupHeight();
        var windowWidth = GetMainWindowWidth();
        var windowHeight = GetMainWindowHeight();

        switch (_popupAnchor)
        {
            case PopupAnchor.RightOutsideTopAligned:
                _recordPopup.HorizontalOffset = _mainWindow.Left + windowWidth + PopupGap;
                _recordPopup.VerticalOffset = _mainWindow.Top;
                break;

            case PopupAnchor.OverlayBottomRight:
                var windowBounds = MonitorWorkAreaHelper.GetWindowBounds(_mainWindow);
                var workArea = GetMainWindowWorkArea();
                var visibleRight = Math.Min(windowBounds.Right, workArea.Right);
                var visibleBottom = Math.Min(windowBounds.Bottom, workArea.Bottom);
                _recordPopup.HorizontalOffset = visibleRight - popupWidth;
                _recordPopup.VerticalOffset = visibleBottom - popupHeight;
                break;

            default:
                _recordPopup.HorizontalOffset = _mainWindow.Left - popupWidth - PopupGap;
                _recordPopup.VerticalOffset = _mainWindow.Top;
                break;
        }
    }

    private void PositionMainWindowFromPopup()
    {
        if (_mainWindow is null || _recordPopup is null)
            return;

        var popupWidth = GetPopupWidth();
        var popupHeight = GetPopupHeight();

        switch (_popupAnchor)
        {
            case PopupAnchor.RightOutsideTopAligned:
                _mainWindow.Left = _recordPopup.HorizontalOffset - PopupGap - GetMainWindowWidth();
                _mainWindow.Top = _recordPopup.VerticalOffset;
                break;

            case PopupAnchor.OverlayBottomRight:
                _mainWindow.Left = _recordPopup.HorizontalOffset + popupWidth - GetMainWindowWidth();
                _mainWindow.Top = _recordPopup.VerticalOffset + popupHeight - GetMainWindowHeight();
                break;

            default:
                _mainWindow.Left = _recordPopup.HorizontalOffset + popupWidth + PopupGap;
                _mainWindow.Top = _recordPopup.VerticalOffset;
                break;
        }
    }

    private PopupAnchor GetPopupAnchorForMainWindow()
    {
        if (_mainWindow is null)
            return PopupAnchor.LeftOutsideTopAligned;

        if (_mainWindow.WindowState == WindowState.Maximized || IsMainWindowDocked())
            return PopupAnchor.OverlayBottomRight;

        var popupWidth = GetPopupWidth();
        var workArea = GetMainWindowWorkArea();
        var popupLeft = _mainWindow.Left - popupWidth - PopupGap;
        var popupRight = _mainWindow.Left + GetMainWindowWidth() + PopupGap + popupWidth;

        if (popupLeft >= workArea.Left)
            return PopupAnchor.LeftOutsideTopAligned;

        if (popupRight <= workArea.Right)
            return PopupAnchor.RightOutsideTopAligned;

        var leftSpace = _mainWindow.Left - workArea.Left;
        var rightSpace = workArea.Right - (_mainWindow.Left + GetMainWindowWidth());

        return rightSpace > leftSpace
            ? PopupAnchor.RightOutsideTopAligned
            : PopupAnchor.LeftOutsideTopAligned;
    }

    private Rect GetMainWindowWorkArea()
    {
        if (_mainWindow is null)
            return SystemParameters.WorkArea;

        return MonitorWorkAreaHelper.GetWorkAreaFromWindow(_mainWindow);
    }

    private bool IsMainWindowDocked()
    {
        if (_mainWindow is null || _mainWindow.WindowState != WindowState.Normal)
            return false;

        const double tolerance = 8;

        var workArea = GetMainWindowWorkArea();
        var bounds = MonitorWorkAreaHelper.GetWindowBounds(_mainWindow);
        var left = bounds.Left;
        var top = bounds.Top;
        var right = bounds.Right;
        var bottom = bounds.Bottom;

        var touchingEdges = 0;

        if (Math.Abs(left - workArea.Left) <= tolerance)
            touchingEdges++;

        if (Math.Abs(top - workArea.Top) <= tolerance)
            touchingEdges++;

        if (Math.Abs(right - workArea.Right) <= tolerance)
            touchingEdges++;

        if (Math.Abs(bottom - workArea.Bottom) <= tolerance)
            touchingEdges++;

        return touchingEdges >= 2;
    }

    private void UpdatePopupDragAvailability()
    {
        if (_recordControls is null || _mainWindow is null)
            return;

        PopupDragBehavior.SetEnableDrag(_recordControls, _mainWindow.WindowState != WindowState.Maximized);
    }

    private double GetPopupWidth()
    {
        if (_recordPopup?.Child is not FrameworkElement element)
            return 0;

        if (element.ActualWidth > 0)
            return element.ActualWidth;

        element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        return element.DesiredSize.Width;
    }

    private double GetPopupHeight()
    {
        if (_recordPopup?.Child is not FrameworkElement element)
            return 0;

        if (element.ActualHeight > 0)
            return element.ActualHeight;

        element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        return element.DesiredSize.Height;
    }

    private double GetMainWindowWidth()
    {
        if (_mainWindow is null)
            return 0;

        return _mainWindow.ActualWidth > 0 ? _mainWindow.ActualWidth : _mainWindow.Width;
    }

    private double GetMainWindowHeight()
    {
        if (_mainWindow is null)
            return 0;

        return _mainWindow.ActualHeight > 0 ? _mainWindow.ActualHeight : _mainWindow.Height;
    }

    private void HookPopupPositionTracking(Popup popup)
    {
        DependencyPropertyDescriptor.FromProperty(Popup.HorizontalOffsetProperty, typeof(Popup))
            .AddValueChanged(popup, OnPopupPositionChanged);
        DependencyPropertyDescriptor.FromProperty(Popup.VerticalOffsetProperty, typeof(Popup))
            .AddValueChanged(popup, OnPopupPositionChanged);
    }

    private void UnhookPopupPositionTracking(Popup popup)
    {
        DependencyPropertyDescriptor.FromProperty(Popup.HorizontalOffsetProperty, typeof(Popup))
            .RemoveValueChanged(popup, OnPopupPositionChanged);
        DependencyPropertyDescriptor.FromProperty(Popup.VerticalOffsetProperty, typeof(Popup))
            .RemoveValueChanged(popup, OnPopupPositionChanged);
    }

    private enum PopupAnchor
    {
        LeftOutsideTopAligned,
        RightOutsideTopAligned,
        OverlayBottomRight
    }
}
