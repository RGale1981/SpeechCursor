using SpeechCursor.Infrastructure.Windowing.Contracts;

using System.ComponentModel;

namespace SpeechCursor.Startup;

public class SplashScreenViewModel : IPopupViewModel
{
    public event PropertyChangedEventHandler? PropertyChanged;
}
