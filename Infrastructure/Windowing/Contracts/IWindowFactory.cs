using System.Windows;

namespace SpeechCursor.Infrastructure.Windowing.Contracts;

public interface IWindowFactory
{
    Window CreateWindow(WindowType type, bool isVisible = false, WindowStartupLocation startupLocation = WindowStartupLocation.CenterScreen);
}
