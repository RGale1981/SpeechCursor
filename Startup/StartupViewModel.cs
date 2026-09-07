using SpeechCursor.Infrastructure.Windowing.Contracts;

using System.ComponentModel;

namespace SpeechCursor.Startup;

public class StartupViewModel : IWindowViewModel
{
    public StartupViewModel()
    {

    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
