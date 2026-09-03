using System.ComponentModel;

namespace SpeechCursor.Record;

public class RecordViewModel : INotifyPropertyChanged
{
    private bool _toggleState;

    public event PropertyChangedEventHandler? PropertyChanged;
    public event Action<bool>? ToggleStateChanged;

    public bool Toggled
    {
        get => _toggleState;
        set
        {
            if (_toggleState != value)
            {
                _toggleState = value;
                OnPropertyChanged(nameof(Toggled));
                ToggleStateChanged?.Invoke(_toggleState);
            }
        }
    }

    private void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
