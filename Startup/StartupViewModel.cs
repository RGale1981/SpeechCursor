using SpeechCursor.Presentation.Dialogs.Contracts;

using System.ComponentModel;

namespace SpeechCursor.Startup;

public class StartupViewModel : IDialogViewModel
{
    public StartupViewModel()
    {

    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
