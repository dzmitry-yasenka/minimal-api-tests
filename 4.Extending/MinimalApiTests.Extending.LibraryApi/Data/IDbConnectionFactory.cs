using System.Data;

namespace MinimalApiTests.Extending.LibraryApi.Data;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync();
}