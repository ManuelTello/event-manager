using Npgsql;

namespace Events.Persistence.Abstractions;

public interface IDatabaseProvider
{
    public NpgsqlDataSource GetDataSource();
}
