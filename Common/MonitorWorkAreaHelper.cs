using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace SpeechCursor.Common;

internal static class MonitorWorkAreaHelper
{
    private const uint MonitorDefaultToNearest = 2;
    private const int DwmwaExtendedFrameBounds = 9;

    public static Point GetCursorPosition()
    {
        GetCursorPos(out var point);
        return new Point(point.X, point.Y);
    }

    public static Rect GetWorkAreaFromPoint(Point screenPoint, Window referenceWindow)
    {
        var nativePoint = new NativePoint
        {
            X = (int)Math.Round(screenPoint.X),
            Y = (int)Math.Round(screenPoint.Y)
        };

        var monitor = MonitorFromPoint(nativePoint, MonitorDefaultToNearest);
        return GetWorkAreaFromMonitor(monitor, referenceWindow);
    }

    public static Rect GetWorkAreaFromWindow(Window window)
    {
        var handle = new WindowInteropHelper(window).Handle;
        if (handle == IntPtr.Zero)
            return SystemParameters.WorkArea;

        var monitor = MonitorFromWindow(handle, MonitorDefaultToNearest);
        return GetWorkAreaFromMonitor(monitor, window);
    }

    public static Rect GetWindowBounds(Window window)
    {
        var handle = new WindowInteropHelper(window).Handle;

        if (handle == IntPtr.Zero)
        {
            return new Rect(window.Left,
                            window.Top,
                            window.ActualWidth > 0 ? window.ActualWidth : window.Width,
                            window.ActualHeight > 0 ? window.ActualHeight : window.Height);
        }

        if (DwmGetWindowAttribute(handle, DwmwaExtendedFrameBounds, out NativeRect dwmRect, Marshal.SizeOf<NativeRect>()) == 0)
        {
            return TransformDeviceRectToDip(handle, new Rect(dwmRect.Left,
                                                             dwmRect.Top,
                                                             dwmRect.Right - dwmRect.Left,
                                                             dwmRect.Bottom - dwmRect.Top));
        }

        if (!GetWindowRect(handle, out var rect))
        {
            return new Rect(window.Left,
                            window.Top,
                            window.ActualWidth > 0 ? window.ActualWidth : window.Width,
                            window.ActualHeight > 0 ? window.ActualHeight : window.Height);
        }

        return TransformDeviceRectToDip(handle, new Rect(rect.Left,
                                                         rect.Top,
                                                         rect.Right - rect.Left,
                                                         rect.Bottom - rect.Top));
    }

    private static Rect GetWorkAreaFromMonitor(IntPtr monitor, Window referenceWindow)
    {
        var monitorInfo = new MonitorInfo
        {
            CbSize = Marshal.SizeOf<MonitorInfo>()
        };

        if (monitor == IntPtr.Zero || !GetMonitorInfo(monitor, ref monitorInfo))
            return SystemParameters.WorkArea;

        var handle = new WindowInteropHelper(referenceWindow).Handle;

        var deviceRect = new Rect(monitorInfo.RcWork.Left,
                                  monitorInfo.RcWork.Top,
                                  monitorInfo.RcWork.Right - monitorInfo.RcWork.Left,
                                  monitorInfo.RcWork.Bottom - monitorInfo.RcWork.Top);

        return TransformDeviceRectToDip(handle, deviceRect);
    }

    private static Rect TransformDeviceRectToDip(IntPtr handle, Rect deviceRect)
    {
        var source = HwndSource.FromHwnd(handle);

        if (source?.CompositionTarget is null)
            return deviceRect;

        var transform = source.CompositionTarget.TransformFromDevice;
        var topLeft = transform.Transform(new Point(deviceRect.Left, deviceRect.Top));
        var bottomRight = transform.Transform(new Point(deviceRect.Right, deviceRect.Bottom));

        return new Rect(topLeft, bottomRight);
    }

    [DllImport("user32.dll")]
    private static extern IntPtr MonitorFromPoint(NativePoint pt, uint dwFlags);

    [DllImport("user32.dll")]
    private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetWindowRect(IntPtr hWnd, out NativeRect lpRect);

    [DllImport("dwmapi.dll")]
    private static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out NativeRect pvAttribute, int cbAttribute);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetCursorPos(out NativePoint lpPoint);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MonitorInfo lpmi);

    [StructLayout(LayoutKind.Sequential)]
    private struct NativePoint
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct MonitorInfo
    {
        public int CbSize;
        public NativeRect RcMonitor;
        public NativeRect RcWork;
        public uint DwFlags;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeRect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
}
