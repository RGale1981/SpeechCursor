using SpeechCursor.Record;

using System.ComponentModel;

namespace SpeechCursor;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly RecordViewModel _recordViewModel;

    public MainWindowViewModel()
    {
        _recordViewModel = new();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public RecordViewModel RecordViewModel => _recordViewModel;
}
