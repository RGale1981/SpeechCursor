using Microsoft.Extensions.DependencyInjection;

using SpeechCursor.Dictation;
using SpeechCursor.Dictation.Contracts;
using SpeechCursor.Presentation.Dialogs;
using SpeechCursor.Presentation.Dialogs.Contracts;
using SpeechCursor.Presentation.Dialogs.Popups;
using SpeechCursor.Presentation.Dialogs.Popups.Contracts;

namespace SpeechCursor;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.RegisterWindowing();

        return services;
    }

    private static IServiceCollection RegisterWindowing(this IServiceCollection services)
    {
        services.AddSingleton<IDialogFactory, DialogFactory>();
        services.AddSingleton<IDalogManager, DialogManager>();
        services.AddSingleton<IDictationViewManager, DictationViewManager>();
        services.AddSingleton<IPopupManager, PopupManager>();

        return services;
    }
}
