using System.Windows;

namespace SpeechCursor.Infrastructure.Windowing.Contracts;

public interface IMainWindowManager
{
    event EventHandler? WindowBoundsChanged;

    void Initialise();

    Window MainWindow { get; }
}
