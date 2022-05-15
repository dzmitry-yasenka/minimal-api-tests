using System.Data;

namespace MinimalApiTests.Structuring.LibraryApi.Data;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync();
}