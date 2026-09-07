using System.Windows;

namespace SpeechCursor.Infrastructure.Behaviors;

public static class ActualSizeBehavior
{
    public static readonly DependencyProperty EnableActualWidthReportingProperty =
        DependencyProperty.RegisterAttached("EnableActualWidthReporting", typeof(bool), typeof(ActualSizeBehavior),
            new PropertyMetadata(false, OnEnableActualWidthReportingChanged));

    public static readonly DependencyProperty ReportedActualWidthProperty =
        DependencyProperty.RegisterAttached("ReportedActualWidth", typeof(double), typeof(ActualSizeBehavior),
            new FrameworkPropertyMetadata(default(double), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static bool GetEnableActualWidthReporting(DependencyObject obj)
        => (bool)obj.GetValue(EnableActualWidthReportingProperty);

    public static void SetEnableActualWidthReporting(DependencyObject obj, bool value)
        => obj.SetValue(EnableActualWidthReportingProperty, value);

    public static double GetReportedActualWidth(DependencyObject obj)
        => (double)obj.GetValue(ReportedActualWidthProperty);

    public static void SetReportedActualWidth(DependencyObject obj, double value)
        => obj.SetValue(ReportedActualWidthProperty, value);

    private static void OnEnableActualWidthReportingChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        if (obj is not FrameworkElement element)
            return;

        if ((bool)args.NewValue)
        {
            element.Loaded += OnElementLoaded;
            element.SizeChanged += OnElementSizeChanged;
        }
        else
        {
            element.Loaded -= OnElementLoaded;
            element.SizeChanged -= OnElementSizeChanged;
        }
    }

    private static void OnElementLoaded(object sender, RoutedEventArgs args)
    {
        if (sender is FrameworkElement element)
            SetReportedActualWidth(element, element.ActualWidth);
    }

    private static void OnElementSizeChanged(object sender, SizeChangedEventArgs args)
    {
        if (sender is FrameworkElement element)
            SetReportedActualWidth(element, element.ActualWidth);
    }
}
