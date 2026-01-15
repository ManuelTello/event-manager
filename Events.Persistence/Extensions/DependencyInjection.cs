using Events.Domain.Abstractions;
using Events.Persistence.Abstractions;
using Events.Persistence.DatabaseProvider;
using Events.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Events.Persistence.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IDatabaseProvider, DatabaseSourceProvider>();
        services.AddTransient<IEventRepository, EventRepository>();
        return services;
    }
}
