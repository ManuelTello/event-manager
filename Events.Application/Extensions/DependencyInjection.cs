using Events.Application.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace Events.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<EventCreateUseCase>();
        services.AddScoped<EventPublishUseCase>();
        services.AddScoped<EventCancelUseCase>();
        services.AddScoped<EventPostponeUseCase>();
        services.AddScoped<EventTicketPostUseCase>();
        services.AddScoped<EventTicketConfirmUseCase>();
        services.AddScoped<EventTicketCancelUseCase>();

        return services;
    }
}
