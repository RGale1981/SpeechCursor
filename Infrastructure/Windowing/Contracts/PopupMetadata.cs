namespace SpeechCursor.Infrastructure.Windowing.Contracts;

public static class PopupMetadata
{
    private static readonly HashSet<PopupType> ModalPopups =
    [
        PopupType.Confirmation
    ];

    public static bool IsModal(PopupType type)
        => ModalPopups.Contains(type);
}

