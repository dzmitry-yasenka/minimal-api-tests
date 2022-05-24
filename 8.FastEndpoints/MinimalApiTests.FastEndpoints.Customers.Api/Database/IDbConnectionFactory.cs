using System.Data;

namespace MinimalApiTests.FastEndpoints.Customers.Api.Database;

public interface IDbConnectionFactory
{
    public Task<IDbConnection> CreateConnectionAsync();
}