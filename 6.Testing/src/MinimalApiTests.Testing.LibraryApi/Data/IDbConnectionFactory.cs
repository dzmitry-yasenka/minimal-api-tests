using System.Data;

namespace MinimalApiTests.Testing.LibraryApi.Data;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync();
}