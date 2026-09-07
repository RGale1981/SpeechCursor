using Microsoft.Extensions.DependencyInjection;

using SpeechCursor.Infrastructure.Windowing;
using SpeechCursor.Infrastructure.Windowing.Contracts;

namespace SpeechCursor;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddSingleton<IWindowFactory, WindowFactory>();
        services.AddSingleton<IWindowManager, WindowManager>();
        services.AddSingleton<IMainWindowManager, MainWindowManager>();
        services.AddSingleton<IPopupManager, PopupManager>();

        return services;
    }
}
