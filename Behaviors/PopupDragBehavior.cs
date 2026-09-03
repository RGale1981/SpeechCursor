using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Windows.Behaviors;

public static class PopupDragBehavior
{
    private static readonly DependencyProperty DragStateProperty =
        DependencyProperty.RegisterAttached(
            "DragState",
            typeof(DragState),
            typeof(PopupDragBehavior),
            new PropertyMetadata(null));

    public static readonly DependencyProperty EnableDragProperty =
        DependencyProperty.RegisterAttached(
            "EnableDrag",
            typeof(bool),
            typeof(PopupDragBehavior),
            new PropertyMetadata(false, OnEnableDragChanged));

    public static readonly DependencyProperty HostPopupProperty =
        DependencyProperty.RegisterAttached(
            "HostPopup",
            typeof(Popup),
            typeof(PopupDragBehavior),
            new PropertyMetadata(null));

    public static bool GetEnableDrag(DependencyObject obj) => (bool)obj.GetValue(EnableDragProperty);

    public static void SetEnableDrag(DependencyObject obj, bool value) => obj.SetValue(EnableDragProperty, value);

    public static Popup? GetHostPopup(DependencyObject obj) => (Popup?)obj.GetValue(HostPopupProperty);

    public static void SetHostPopup(DependencyObject obj, Popup? value) => obj.SetValue(HostPopupProperty, value);

    private static void OnEnableDragChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not UIElement element)
            return;

        if ((bool)e.NewValue)
        {
            element.PreviewMouseLeftButtonDown += OnMouseDown;
            element.PreviewMouseMove += OnMouseMove;
            element.PreviewMouseLeftButtonUp += OnMouseUp;
        }
        else
        {
            element.PreviewMouseLeftButtonDown -= OnMouseDown;
            element.PreviewMouseMove -= OnMouseMove;
            element.PreviewMouseLeftButtonUp -= OnMouseUp;
        }
    }

    private static void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not UIElement element)
            return;

        if (GetHostPopup(element) is null)
            return;

        var state = new DragState
        {
            StartMouseScreen = element.PointToScreen(e.GetPosition(element)),
            IsDragging = false,
            StartHorizontalOffset = GetHostPopup(element)!.HorizontalOffset,
            StartVerticalOffset = GetHostPopup(element)!.VerticalOffset
        };

        element.SetValue(DragStateProperty, state);
    }

    private static void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (sender is not UIElement element)
            return;

        var popup = GetHostPopup(element);
        var state = (DragState?)element.GetValue(DragStateProperty);

        if (popup is null || state is null)
            return;

        if (e.LeftButton != MouseButtonState.Pressed)
        {
            ClearDragState(element);
            return;
        }

        var currentMouseScreen = element.PointToScreen(e.GetPosition(element));
        var delta = currentMouseScreen - state.StartMouseScreen;

        if (!state.IsDragging)
        {
            if (Math.Abs(delta.X) < SystemParameters.MinimumHorizontalDragDistance &&
                Math.Abs(delta.Y) < SystemParameters.MinimumVerticalDragDistance)
                return;

            state.IsDragging = true;
            element.CaptureMouse();
        }

        popup.HorizontalOffset = state.StartHorizontalOffset + delta.X;
        popup.VerticalOffset = state.StartVerticalOffset + delta.Y;
        e.Handled = true;
    }

    private static void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (sender is not UIElement element)
            return;

        var state = (DragState?)element.GetValue(DragStateProperty);
        if (state?.IsDragging == true)
            e.Handled = true;

        ClearDragState(element);
    }

    private static void ClearDragState(UIElement element)
    {
        if (element.IsMouseCaptured)
            element.ReleaseMouseCapture();

        element.ClearValue(DragStateProperty);
    }

    private sealed class DragState
    {
        public bool IsDragging { get; set; }

        public Point StartMouseScreen { get; set; }

        public double StartHorizontalOffset { get; set; }

        public double StartVerticalOffset { get; set; }
    }
}
