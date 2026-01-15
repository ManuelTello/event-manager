using Testcontainers.PostgreSql;

namespace Events.Test.Implementations;

public abstract class TestContainer:IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres;

    protected TestContainer()
    {
        this._postgres = new PostgreSqlBuilder("postgres")
            .WithExposedPort(32786)
            .WithDatabase("Events")
            .WithUsername("postgres")
            .WithPassword("fbvmYdQIecKUxU4c")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await this._postgres.StartAsync();
        System.Console.WriteLine(this._postgres.GetMappedPublicPort());
    }

    public async Task DisposeAsync()
    {
        await this._postgres.StopAsync();
    }

    protected string GetConnectionString()
    {
        return this._postgres.GetConnectionString();
    }
}