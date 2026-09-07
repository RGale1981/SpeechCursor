namespace SpeechCursor.Infrastructure.Extensions;

public static class DisposableExtensions
{
    public static void TryDispose(this object? instance)
    {
        if (instance is IDisposable disposableInstance)
            disposableInstance.Dispose();
    }
}
