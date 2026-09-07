using SpeechCursor.Presentation.Dialogs.Contracts;
using SpeechCursor.Record;

using System.ComponentModel;

namespace SpeechCursor.Dictation;

public class DictationViewModel : IDialogViewModel
{
    private readonly RecordControlsViewModel _recordViewModel;

    public DictationViewModel()
    {
        _recordViewModel = new();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public RecordControlsViewModel RecordViewModel => _recordViewModel;
}
