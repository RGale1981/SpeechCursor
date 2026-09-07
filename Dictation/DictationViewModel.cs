using SpeechCursor.Infrastructure.Windowing.Contracts;
using SpeechCursor.Record;

using System.ComponentModel;

namespace SpeechCursor.Dictation;

public class DictationViewModel : IWindowViewModel
{
    private readonly RecordControlsViewModel _recordViewModel;

    public DictationViewModel()
    {
        _recordViewModel = new();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public RecordControlsViewModel RecordViewModel => _recordViewModel;
}
