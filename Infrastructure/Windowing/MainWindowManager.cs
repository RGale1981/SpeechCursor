using SpeechCursor.Infrastructure.Windowing.Contracts;

using System.Windows;
using System.Windows.Interop;

namespace SpeechCursor.Infrastructure.Windowing;

public sealed class MainWindowManager : IMainWindowManager, IDisposable
{
    private Window? _mainWindow;

    private bool _disposed;
    private readonly IWindowFactory _windowFactory;

    public event EventHandler? WindowBoundsChanged;

    public MainWindowManager(IWindowFactory windowFactory)
    {
        _windowFactory = windowFactory;
    }

    public void Initialise()
    {
        CreateMainWindow();
        RegisterWindowEvents();

        if (_mainWindow != null)
            _ = new WindowInteropHelper(_mainWindow).EnsureHandle();
    }

    public Window MainWindow
    {
        get => _mainWindow ?? throw new InvalidOperationException("MainWindow has not been initialised.");

        private set => _mainWindow = value;
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        if (_mainWindow != null)
        {
            _mainWindow.Closed -= OnClosed;
            _mainWindow.LocationChanged -= OnBoundsChanged;
            _mainWindow.SizeChanged -= OnBoundsChanged;
            _mainWindow.StateChanged -= OnBoundsChanged;
        }

        _disposed = true;
    }

    private void CreateMainWindow()
    {
        MainWindow = _windowFactory.CreateWindow(WindowType.MainWindow);

        Application.Current.MainWindow = MainWindow;
    }

    private void RegisterWindowEvents()
    {
        if (_mainWindow == null)
            return;

        _mainWindow.Closed += OnClosed;
        _mainWindow.LocationChanged += OnBoundsChanged;
        _mainWindow.SizeChanged += OnBoundsChanged;
        _mainWindow.StateChanged += OnBoundsChanged;
    }

    private void OnBoundsChanged(object? sender, EventArgs e) => WindowBoundsChanged?.Invoke(this, EventArgs.Empty);

    private void OnClosed(object? sender, EventArgs e)
    {
        Dispose();

        _mainWindow = null;
    }
}
