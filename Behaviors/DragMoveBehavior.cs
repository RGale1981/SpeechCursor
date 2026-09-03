using System.Windows;
using System.Windows.Input;

using Microsoft.Xaml.Behaviors;

namespace Windows.Behaviors;

public class DragMoveBehavior : Behavior<UIElement>
{
    private bool _dragPending;
    private bool _isDragging;

    private Point _startMouseScreen;
    private double _startWindowLeft;
    private double _startWindowTop;

    protected override void OnAttached()
    {
        AssociatedObject.PreviewMouseLeftButtonDown += OnMouseDown;
        AssociatedObject.PreviewMouseMove += OnMouseMove;
        AssociatedObject.PreviewMouseLeftButtonUp += OnMouseUp;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.PreviewMouseLeftButtonDown -= OnMouseDown;
        AssociatedObject.PreviewMouseMove -= OnMouseMove;
        AssociatedObject.PreviewMouseLeftButtonUp -= OnMouseUp;
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        var window = Window.GetWindow(AssociatedObject);
        if (window is null)
            return;

        _startMouseScreen = AssociatedObject.PointToScreen(e.GetPosition(AssociatedObject));
        _startWindowLeft = window.Left;
        _startWindowTop = window.Top;

        _dragPending = true;
        _isDragging = false;
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (!_dragPending || e.LeftButton != MouseButtonState.Pressed)
            return;

        var window = Window.GetWindow(AssociatedObject);
        if (window is null)
            return;

        var currentMouseScreen = AssociatedObject.PointToScreen(e.GetPosition(AssociatedObject));
        var delta = currentMouseScreen - _startMouseScreen;

        if (!_isDragging)
        {
            if (Math.Abs(delta.X) < SystemParameters.MinimumHorizontalDragDistance &&
                Math.Abs(delta.Y) < SystemParameters.MinimumVerticalDragDistance)
                return;

            _isDragging = true;
        }

        window.Left = _startWindowLeft + delta.X;
        window.Top = _startWindowTop + delta.Y;

        e.Handled = true;
    }

    private void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (_isDragging)
            e.Handled = true;

        _dragPending = false;
        _isDragging = false;
    }
}
