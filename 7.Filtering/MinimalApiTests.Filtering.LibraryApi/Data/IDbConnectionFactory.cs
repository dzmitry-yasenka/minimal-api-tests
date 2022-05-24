using System.Data;

namespace MinimalApiTests.Filtering.LibraryApi.Data;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync();
}