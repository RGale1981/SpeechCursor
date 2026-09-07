using System.Windows;

namespace SpeechCursor.Dictation.Contracts;

public interface IDictationViewManager
{
    event EventHandler? WindowBoundsChanged;

    void Initialise();

    Window MainWindow { get; }
}
