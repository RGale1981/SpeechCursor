using System.Windows;

namespace SpeechCursor.Presentation.Dialogs.Contracts;

public interface IDialogFactory
{
    Window CreateWindow(DialogType type, bool isVisible = false, WindowStartupLocation startupLocation = WindowStartupLocation.CenterScreen);
}
