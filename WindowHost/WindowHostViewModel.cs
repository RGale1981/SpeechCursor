using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

using Windows.Common;
using Windows.Dictation;
using Windows.Record;

namespace Windows.WindowHost;

public class WindowHostViewModel : INotifyPropertyChanged, IDisposable
{
    private readonly DictationViewModel _dictationViewModel;
    private readonly RecordViewModel _recordViewModel;

    private bool _isDiposed;
    private bool _dictationVisible;

    private double _recordX;
    private double _recordY;
    private double _recordWidth;

    public WindowHostViewModel()
    {
        _dictationViewModel = new();

        _recordViewModel = new();
        _recordViewModel.ToggleStateChanged += OnRecordToggleStateChanged;

        MoveRecordCommand = new RelayCommand(OnMoveRecord);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ICommand MoveRecordCommand { get; }

    public bool DictationVisible
    {
        get => _dictationVisible;
        set
        {
            if (_dictationVisible == value)
                return;

            _dictationVisible = value;
            OnPropertyChanged(nameof(DictationVisible));
        }
    }

    public double RecordX
    {
        get => _recordX;
        set
        {
            if (Math.Abs(_recordX - value) < double.Epsilon)
                return;

            _recordX = value;
            OnPropertyChanged(nameof(RecordX));
            OnPropertyChanged(nameof(DictationX));
        }
    }

    public double RecordY
    {
        get => _recordY;
        set
        {
            if (Math.Abs(_recordY - value) < double.Epsilon)
                return;

            _recordY = value;
            OnPropertyChanged(nameof(RecordY));
            OnPropertyChanged(nameof(DictationY));
        }
    }

    public double RecordWidth
    {
        get => _recordWidth;
        set
        {
            if (Math.Abs(_recordWidth - value) < double.Epsilon)
                return;

            _recordWidth = value;
            OnPropertyChanged(nameof(RecordWidth));
            OnPropertyChanged(nameof(DictationX));
        }
    }

    public double DictationX => RecordX + RecordWidth;

    public double DictationY => RecordY;

    public DictationViewModel DictationViewModel => _dictationViewModel;

    public RecordViewModel RecordViewModel => _recordViewModel;

    private void OnRecordToggleStateChanged(bool isToggled) => DictationVisible = isToggled;

    private void OnMoveRecord(object? parameter)
    {
        if (parameter is not Vector delta)
            return;

        RecordX += delta.X;
        RecordY += delta.Y;
    }

    private void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public void Dispose()
    {
        if (_isDiposed)
            return;

        _recordViewModel.ToggleStateChanged -= OnRecordToggleStateChanged;

        _isDiposed = true;
    }
}
