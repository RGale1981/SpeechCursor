using System;
using System.ComponentModel;

using Windows.Record;

namespace Windows;

public class MainWindowViewModel : INotifyPropertyChanged, IDisposable
{
    private readonly RecordViewModel _recordViewModel;
    private bool _isDisposed;

    public MainWindowViewModel()
    {
        _recordViewModel = new();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public RecordViewModel RecordViewModel => _recordViewModel;

    public void Dispose()
    {
        if (_isDisposed)
            return;

        _isDisposed = true;
        GC.SuppressFinalize(this);
    }
}
