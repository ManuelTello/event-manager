using Events.Application.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace Events.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateEventUseCase>();
        services.AddScoped<PublishEventUseCase>();
        services.AddScoped<CancelEventUseCase>();
        services.AddScoped<PostponeEventUseCase>();
        services.AddScoped<EventPostTicketUseCase>();
        services.AddScoped<EventConfirmTicketUseCase>();
        
        return services;
    }
}
