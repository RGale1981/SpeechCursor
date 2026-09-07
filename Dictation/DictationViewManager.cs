using SpeechCursor.Dictation.Contracts;
using SpeechCursor.Presentation.Dialogs.Contracts;

using System.Windows;
using System.Windows.Interop;

namespace SpeechCursor.Dictation;

public sealed class DictationViewManager : IDictationViewManager, IDisposable
{
    private Window? _mainWindow;

    private bool _disposed;
    private readonly IDialogFactory _windowFactory;

    public event EventHandler? WindowBoundsChanged;

    public DictationViewManager(IDialogFactory windowFactory)
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
        MainWindow = _windowFactory.CreateWindow(DialogType.MainWindow);

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
